using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace DroneStation {
    public class DroneConnectionInfo {
        public int TotalBytesIn;
        public int TotalBytesOut;
        public int BytesPerSecIn;
        public int BytesPerSecOut;
        public string ConnectionType;
        public int LastUpdate;
        public int SignalStrength;
    }
    public class Proxy {
        bool _isRunning;
        DateTime _lastUpdate;
        DateTime _droneLastContactUtc;
        Guid _droneId;
        public Proxy(Guid droneId) {
            _droneId = droneId;
        }
        public Guid DroneId { get { return _droneId; } }
        public void Start() {
            //_timer = new Timer(statusUpdate, null, 1000, 1000);
            send("reg_station_adr", null);
        }
        public DateTime DroneLastContactUtc
        {
            get
            {
                return _droneLastContactUtc;
            }
        }
        public void QueCommand(string cmd) {
            NameValueCollection nvc = new NameValueCollection();
            nvc["command_name"] = "shell";
            nvc["command_params"] = cmd;
            send("enqueue_command", nvc);
        }
        public void TakePhoto(string id) {
            NameValueCollection nvc = new NameValueCollection(); 
            nvc["command_name"] = "upload_photo";
            nvc["command_params"] = id;
            send("enqueue_command", nvc);
        }
        public string GetDroneAddress() {
            return send("get_drone_adr", null);
        }
        public string GetStationAddress() {
            return send("get_station_adr", null);
        }
        public DroneConnectionInfo GetConnectionInfo() {

            var info = new DroneConnectionInfo();
            try {
                var xml = send("get_connection_status", null);
                XmlDocument d = new XmlDocument();
                d.Load(new StringReader(xml));
                var signalStrengthStr = d.SelectSingleNode("response/SignalStrength").InnerText;
                if (string.IsNullOrEmpty(signalStrengthStr)) {
                    info.SignalStrength = int.Parse(d.SelectSingleNode("response/SignalIcon").InnerText) * 20;
                } else {
                    info.SignalStrength = int.Parse(signalStrengthStr);
                }

                var networkType = int.Parse(d.SelectSingleNode("response/CurrentNetworkType").InnerText);
                switch (networkType) {
                    case 0: info.ConnectionType = "No service"; break;
                    case 1: info.ConnectionType = "GSM"; break;
                    case 2: info.ConnectionType = "GPRS"; break;
                    case 3: info.ConnectionType = "EDGE"; break;
                    case 4: info.ConnectionType = "WCDMA"; break;
                    case 5: info.ConnectionType = "HSDPA"; break;
                    case 6: info.ConnectionType = "HSUPA"; break;
                    case 7: info.ConnectionType = "HSPA"; break;
                    case 8: info.ConnectionType = "TDSCDMA"; break;
                    case 9: info.ConnectionType = "HSPA+"; break;
                    case 10: info.ConnectionType = "EVDO rev 0"; break;
                    case 11: info.ConnectionType = "EVDO rev A"; break;
                    case 12: info.ConnectionType = "EVDO rev B"; break;
                    case 13: info.ConnectionType = "1xRTT"; break;
                    case 14: info.ConnectionType = "UMB"; break;
                    case 15: info.ConnectionType = "1xEVDV"; break;
                    case 16: info.ConnectionType = "3xRTT"; break;
                    case 17: info.ConnectionType = "HSPA+ 64QAM"; break;
                    case 18: info.ConnectionType = "HSPA+ MIMO"; break;
                    case 19: info.ConnectionType = "LTE"; break;
                    case 41: info.ConnectionType = "3G"; break;
                    default: info.ConnectionType = "Unknown"; break;
                }
            } catch {
                info.SignalStrength = -1;
                info.ConnectionType = "No signal";
            }
            try {
                var xml = send("get_connection_traffic", null);
                XmlDocument d = new XmlDocument();
                d.Load(new StringReader(xml));
                info.BytesPerSecIn = int.Parse(d.SelectSingleNode("response/CurrentDownloadRate").InnerText);
                info.BytesPerSecOut = int.Parse(d.SelectSingleNode("response/CurrentUploadRate").InnerText);
                info.TotalBytesIn = int.Parse(d.SelectSingleNode("response/CurrentDownload").InnerText);
                info.TotalBytesOut = int.Parse(d.SelectSingleNode("response/CurrentUpload").InnerText);
            } catch {
                info.BytesPerSecIn = -1;
                info.BytesPerSecOut = -1;
                info.TotalBytesIn = -1;
                info.TotalBytesOut = -1;
            }
            try {
                info.LastUpdate = int.Parse(send("get_drone_lastcontact", null));
            } catch {
                info.LastUpdate = -1;
            }
            return info;
        }
        string send(string actionName, NameValueCollection actionParams) {
            var url = "http://droneproxy.azurewebsites.net?drone_id=" + _droneId + "&action=" + WebUtility.UrlEncode(actionName);
            StringBuilder sb = new StringBuilder();
            if (actionParams != null) {
                foreach (var k in actionParams.AllKeys) {
                    sb.Append("&");
                    sb.Append(WebUtility.UrlEncode(k));
                    sb.Append("=");
                    sb.Append(WebUtility.UrlEncode(actionParams[k]));
                }
            }
            using (WebClient wc = new WebClient()) {
                var result = wc.DownloadString(url + sb.ToString());
                if (result == "ERROR") throw new Exception("Error");
                return result;
            }
        }
        void statusUpdate(object stateInfo) {
            if (_isRunning) return;
            _isRunning = true;
            NameValueCollection nvc = new NameValueCollection();
            var lastContactString = send("get_drone_lastcontact", nvc);
            _droneLastContactUtc = new DateTime(long.Parse(lastContactString));
            var statusXml = send("get_connection_status", nvc);
            // add parsing to xml
            _lastUpdate = DateTime.Now;
            _isRunning = false;
        }
    }
}
