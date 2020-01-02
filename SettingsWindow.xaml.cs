using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DroneStation {
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window {
        Settings _settings;
        public SettingsWindow(Settings settings) {
            InitializeComponent();
            _settings = settings;
            if (_settings.Count == 0) {
                txtDroneId.Text = Guid.NewGuid().ToString();
                addDefaultSettings();
                MessageBox.Show("Default settings and a new drone GUID was created. ","PLEASE NOTE:");
                saveSettings();
            } else {
                txtDroneId.Text = _settings.Get("DroneId");
                txtStationLanIp.Text = _settings.Get("StationLanIp");
                txtDroneLanIp.Text = _settings.Get("DroneLanIp");
                txtHighQualityVideo.Text = _settings.Get("HighQualityVideo");
                txtLowQualityVideo.Text = _settings.Get("LowQualityVideo");
                txtMedQualityVideo.Text = _settings.Get("MedQualityVideo");
                txtStopVideo.Text = _settings.Get("StopVideo");
                txtPathPutty.Text = _settings.Get("PathPutty");
                txtPathGStreamer.Text = _settings.Get("PathGStreamer");
            }
        }

        private void btnDownloadPutty_Click(object sender, RoutedEventArgs e) {
            Process.Start("http://www.putty.org/");
        }

        private void btnNewGuid_Click(object sender, RoutedEventArgs e) {
            txtDroneId.Text = Guid.NewGuid().ToString();
        }

        private void btnDownloadGStreamer_Click(object sender, RoutedEventArgs e) {
            Process.Start("https://gstreamer.freedesktop.org/");
        }
        void addDefaultSettings() {
            txtStationLanIp.Text = "192.168.1.101";
            txtDroneLanIp.Text = "192.168.1.100";
            txtHighQualityVideo.Text = "raspivid -hf -n -t 0 -w 960 -h 720 -fps 30 -b 2000000 -co 0 -sh 50 -sa 0 -o - | gst-launch-1.0 -e -vvvv fdsrc ! h264parse ! rtph264pay pt=96 config-interval=5 ! udpsink host=#STATION_ADDRESS# port=5000";
            txtLowQualityVideo.Text = "raspivid -hf -n -t 0 -w 320 -h 240 -fps 30 -b 250000 -co 0 -sh 50 -sa 0 -o - | gst-launch-1.0 -e -vvvv fdsrc ! h264parse ! rtph264pay pt=96 config-interval=5 ! udpsink host=#STATION_ADDRESS# port=5000";
            txtMedQualityVideo.Text = "raspivid -hf -n -t 0 -w 640 -h 480 -fps 30 -b 600000 -co 0 -sh 50 -sa 0 -o - | gst-launch-1.0 -e -vvvv fdsrc ! h264parse ! rtph264pay pt=96 config-interval=5 ! udpsink host=#STATION_ADDRESS# port=5000";
            //_proxy.QueCommand("gst-launch-1.0 -v v4l2src device=/dev/video0 ! video/x-h264,width=320,height=180,framerate=30/1 ! h264parse ! rtph264pay pt=96 config-interval=4 ! udpsink host=" + _localAddress + " port=5000");
            txtStopVideo.Text = "killall raspivid";
            //_proxy.QueCommand("killall gst-launch-1.0");
            txtPathPutty.Text = @"C:\Users\Ole\OneDrive\Raspberry\putty.exe";
            txtPathGStreamer.Text = @"C:\gstreamer\1.0\x86_64\bin\gst-launch-1.0.exe";
        }
        private void btnDefaults_Click(object sender, RoutedEventArgs e) {
            if (MessageBox.Show("Add default settings?" + Environment.NewLine + Environment.NewLine
                + "(Drone GUID is not affected) ", "PLEASE CONFIRM:", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            addDefaultSettings();
        }

        string getScript() {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "DroneStation.droneproxy.py";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName)) {
                using (StreamReader reader = new StreamReader(stream)) {
                    string result = reader.ReadToEnd();
                    return result.Replace("#DRONE_ID#", _settings.Get("DroneId"));
                }
            }
        }
        private void btnRPiScript_Click(object sender, RoutedEventArgs e) {
            saveSettings();
            var script = getScript();
            var w = new ScriptWindow(script);
            w.Show();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        void saveSettings() {
            _settings.Set("DroneId", txtDroneId.Text);
            _settings.Set("StationLanIp", txtStationLanIp.Text);
            _settings.Set("DroneLanIp", txtDroneLanIp.Text);
            _settings.Set("HighQualityVideo", txtHighQualityVideo.Text);
            _settings.Set("LowQualityVideo", txtLowQualityVideo.Text);
            _settings.Set("MedQualityVideo", txtMedQualityVideo.Text);
            _settings.Set("StopVideo", txtStopVideo.Text);
            _settings.Set("PathPutty", txtPathPutty.Text);
            _settings.Set("PathGStreamer", txtPathGStreamer.Text);
            _settings.SaveSettings();
        }
        private void btnOk_Click(object sender, RoutedEventArgs e) {
            saveSettings();
            Close();
        }
    }
}
