import urllib
import urllib.request
from time import sleep
import subprocess
import base64

droneId = "#DRONE_ID#"
interval = 1
routerSessionCookie = ""

try:
    unicode
except NameError:
    def _is_unicode(x):
        return 0
else:
    def _is_unicode(x):
        return isinstance(x, unicode)

def parse_qsl(qs, keep_blank_values=0, strict_parsing=0):
    pairs = [s2 for s1 in qs.split('&') for s2 in s1.split(';')]
    r = []
    for name_value in pairs:
        if not name_value and not strict_parsing:
            continue
        nv = name_value.split('=', 1)
        if len(nv) != 2:
            if strict_parsing:
                raise(ValueError, "bad query field: %r" % (name_value,))
            # Handle case of a control-name with no equal sign
            if keep_blank_values:
                nv.append('')
            else:
                continue
        if len(nv[1]) or keep_blank_values:
            name = unquote(nv[0].replace('+', ' '))
            value = unquote(nv[1].replace('+', ' '))
            r.append((name, value))

    return dict((x, y) for x, y in r)

_hexdig = '0123456789ABCDEFabcdef'
_hextochr = dict((a+b, chr(int(a+b,16)))
                 for a in _hexdig for b in _hexdig)

def unquote(s):
    """unquote('abc%20def') -> 'abc def'."""
    if _is_unicode(s):
        if '%' not in s:
            return s
        bits = _asciire.split(s)
        res = [bits[0]]
        append = res.append
        for i in range(1, len(bits), 2):
            append(unquote(str(bits[i])).decode('latin1'))
            append(bits[i + 1])
        return ''.join(res)

    bits = s.split('%')
    # fastpath
    if len(bits) == 1:
        return s
    res = [bits[0]]
    append = res.append
    for item in bits[1:]:
        try:
            append(_hextochr[item[:2]])
            append(item[2:])
        except KeyError:
            append('%')
            append(item)
    return ''.join(res)


def callProxy(action):
    try :
        url = "http://droneproxy.azurewebsites.net"
        url = url + "?drone_id=" + droneId + "&action=" + action
        response = urllib.request.urlopen(url,timeout=5).read().decode("utf-8")
        return response
    except Exception as e:
        print("FAILED TO CALL: " + url)
        raise ExceptionToThrow(e)

def logEvent(desc):
        try :
            print(desc)
            callProxy("log_drone_event&"+ urllib.parse.urlencode({ "desc" : desc } ))
        except :
            print("ERROR: Logging Drone Event")

def getRouterSessionCookie():
    try :
        url = "http://192.168.1.1/"      #try this url out when with the modem
        resp = urllib.request.urlopen(url,timeout=5)
        info = dict(resp.info())
        return info["Set-Cookie"] 
    except :
        return "NA"
    # these methods read the html of the huwaei modem
def getConnectionStatus():   
    url = "http://192.168.1.1/api/monitoring/status"
    req = urllib.request.Request(url)
    if routerSessionCookie != "NA" : 
        req.add_header("Cookie", routerSessionCookie)
    resp = urllib.request.urlopen(req,timeout=3)
    content = resp.read().decode("utf-8")
    return content

def getConnectionTraffic():
    url = "http://192.168.1.1/api/monitoring/traffic-statistics"
    req = urllib.request.Request(url)
    if routerSessionCookie != "NA" : 
        req.add_header("Cookie", routerSessionCookie)
    resp = urllib.request.urlopen(req,timeout=3)
    content = resp.read().decode("utf-8")
    return content

def uploadPhoto(photoId):
    print("Started photo")
    url = "http://droneproxy.azurewebsites.net"
    url = url + "?drone_id=" + droneId + "&photo_id=" + photoId + "&action=upload_photo"
    path = "/home/pi/photo.jpg"
    p = subprocess.Popen("raspistill -hf -vf -o " + path + " -w 1024 -h 768" , shell=True)
    p.wait()
    print("Captured photo")
    with open(path, "rb") as image_file:
        encoded_string = base64.b64encode(image_file.read())   # encodes the photo in utf-8 and uploads to the server
    print("Encoded data")
    query_args = { "photo" : encoded_string }
    data = urllib.parse.urlencode(query_args).encode("utf-8")
    response = urllib.request.urlopen(url,data).read().decode("utf-8")
    print("Uploaded data")
    return response

connected = False

i = 0

while not connected :
    i = i + 1
    try :
        if callProxy("reg_drone_adr") == "OK" :
            callProxy("reset_command_queue")
            routerSessionCookie = getRouterSessionCookie()
            logEvent("CONNECT: Connected with server on attempt " + str(i))
            connected = True
    except :
        print("Trying to connect, attempt " + str(i))
    sleep(1)

i = 0

while True :
    if i == 0 :
        try :
            callProxy("reg_drone_adr")
            s = getConnectionStatus()
            callProxy("update_connection_status&"+ urllib.parse.urlencode({ "connection_status" : s } ))
            print("Connection Status Update")
            s = getConnectionTraffic()
            callProxy("update_connection_traffic&"+ urllib.parse.urlencode({ "connection_traffic" : s } ))
        except Exception as e:
            logEvent("ERROR: Update connection status: {0}".format(str(e)))

    try :
        cmd = callProxy("dequeue_command")
        if cmd != "OK" :
            print("RAW COMMAND: " + cmd)
            p = parse_qsl(cmd)      # P is a dictionary of instructions for easy finding  
            cmdName = p["command_name"]
            cmdParams = p["command_params"]
            logEvent("COMMAND " + cmdName + ": " + cmdParams) 
            if cmdName == "shell" :
                p = subprocess.Popen(cmdParams, shell=True)
            if cmdName == "upload_photo" :
                photoId = cmdParams
                uploadPhoto(photoId)
        else :
            print("No commands")
    except Exception as e:
        logEvent("ERROR: Dequeue Command: {0}".format(str(e)))

    i = i + 1
    if i > 3 :
        i = 0

    sleep(interval)
