using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DroneStation {
    public partial class MainWindow : Window {

        [DllImport("user32")]
        private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwnd2, int x, int y, int cx, int cy, int flags);

        Proxy _proxy;
        Settings _settings;
        string _localAddress;

        public MainWindow() {
            InitializeComponent();

            var path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Settings.xlm";
            _settings = new Settings(path);
            if (!System.IO.File.Exists(path)) {
                btnSettings_Click(null, null);
            }
            var droneIdString = _settings.Get("DroneId");
            Guid droneId;
            if (!Guid.TryParse(droneIdString, out droneId)) {
                droneId = Guid.NewGuid();
                _settings.Set("DroneId", droneId.ToString());
                _settings.SaveSettings();
                MessageBox.Show("A drone GUID was missing and a new one was generated." + Environment.NewLine + Environment.NewLine +
                    "Open settings to see or edit the new GUID. " + Environment.NewLine +
                    "The GUID must be unique to your RPi drone. " + Environment.NewLine
                    , "PLEASE NOTE:", MessageBoxButton.OK);
            }
            _proxy = new Proxy(droneId);
            _proxy.Start();
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e) {
            DroneConnectionInfo info = _proxy.GetConnectionInfo();
            int lastContactInSec = info.LastUpdate;
            int signalStrength = info.SignalStrength;
            if (lastContactInSec > 10) {
                txtInfo.Text = "DRONE" + Environment.NewLine + "OFFLINE" + Environment.NewLine + Environment.NewLine + lastContactInSec + " s";
                signalStrength = 0;
                txtInfo.Foreground = new SolidColorBrush(Color.FromRgb(255, 20, 20));
            } else {
                txtInfo.Text =
                    signalStrength + "% " + info.ConnectionType + Environment.NewLine +
                    FormatBytes(info.TotalBytesIn) + " in" + Environment.NewLine +
                    FormatBytes(info.TotalBytesOut) + " out" + Environment.NewLine +
                    FormatBytes(info.BytesPerSecIn) + "/s in" + Environment.NewLine +
                    FormatBytes(info.BytesPerSecOut) + "/s out" + Environment.NewLine +
                    lastContactInSec + " s";
            }
            prgConnection.Value = signalStrength;
            prgConnection.ToolTip = prgConnection.Value;
            if (signalStrength > 80) {
                prgConnection.Foreground = new SolidColorBrush(Color.FromRgb(18, 234, 36));
                txtInfo.Foreground = new SolidColorBrush(Color.FromRgb(20, 255, 20));
            } else if (signalStrength > 60) {
                prgConnection.Foreground = new SolidColorBrush(Color.FromRgb(162, 234, 75));
                txtInfo.Foreground = new SolidColorBrush(Color.FromRgb(20, 255, 20));
            } else if (signalStrength > 40) {
                prgConnection.Foreground = new SolidColorBrush(Color.FromRgb(234, 234, 14));
                txtInfo.Foreground = new SolidColorBrush(Color.FromRgb(20, 255, 20));
            } else if (signalStrength > 20) {
                prgConnection.Foreground = new SolidColorBrush(Color.FromRgb(255, 170, 0));
                txtInfo.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
            } else if (signalStrength > 0) {
                prgConnection.Foreground = new SolidColorBrush(Color.FromRgb(255, 56, 56));
                txtInfo.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
            } else {
                prgConnection.Foreground = new SolidColorBrush(Color.FromRgb(255, 20, 20));
            }
            if (!_hasFrontedPlayer) {
                try {
                    IntPtr hWnd = IntPtr.Zero;
                    foreach (Process pList in Process.GetProcesses()) {
                        if (pList.MainWindowTitle.Contains("GStreamer D3D")) {
                            hWnd = pList.MainWindowHandle;
                            SetWindowPos(hWnd, new IntPtr(-1), 0, 0, 0, 0, 0x1 | 0x2);
                            _hasFrontedPlayer = true;
                        }
                    }
                    if (_videoPlayer != null && !_videoPlayer.HasExited) {
                        ShowWindowAsync(_videoPlayer.MainWindowHandle, SW_SHOWMINIMIZED);
                    }
                } catch { }
            }
        }
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        bool _hasFrontedPlayer = false;
                                                                                                            //need to get command for plane  
        private void btnStartArdu_Click(object sender, RoutedEventArgs e) {
            //_proxy.QueCommand("sudo ArduCopter-quad -A udp:" + _localAddress + ":14550");
            _proxy.QueCommand("sudo systemctl start arduplane");

            btnStartArdu.IsEnabled = false;

        }

        private void btnStopArdu_Click(object sender, RoutedEventArgs e) {
            if (MessageBox.Show("Sure you want to stop arduplane?", "Please confirm:", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.Yes) {
                _proxy.QueCommand("sudo systemctl stop arduplane");
                btnStartArdu.IsEnabled = true;
            }
        }

        private void btnPhoto_Click(object sender, RoutedEventArgs e) {
            if (_lastVideoMode != null) _proxy.QueCommand(_settings.Get("StopVideo"));
            _proxy.TakePhoto(DateTime.Now.Ticks.ToString());
            if (_lastVideoMode != null) _proxy.QueCommand(_lastVideoMode);
        }

        string _lastVideoMode = null;
        private void btnVideoHigh_Click(object sender, RoutedEventArgs e) {
            _proxy.QueCommand(_settings.Get("StopVideo"));
            _lastVideoMode = _settings.Get("HighQualityVideo").Replace("#STATION_ADDRESS#", _localAddress);
            _proxy.QueCommand(_lastVideoMode);
        }
        private void btnVideoMed_Click(object sender, RoutedEventArgs e) {
            _proxy.QueCommand(_settings.Get("StopVideo"));
            _lastVideoMode = _settings.Get("MedQualityVideo").Replace("#STATION_ADDRESS#", _localAddress);
            _proxy.QueCommand(_lastVideoMode);
        }
        private void btnVideoLow_Click(object sender, RoutedEventArgs e) {
            _proxy.QueCommand(_settings.Get("StopVideo"));
            _lastVideoMode = _settings.Get("LowQualityVideo").Replace("#STATION_ADDRESS#", _localAddress);
            _proxy.QueCommand(_lastVideoMode);
        }
        private void btnMissionPlanner_Click(object sender, RoutedEventArgs e) {

        }
        private void btnStopVideo_Click(object sender, RoutedEventArgs e) {
            _proxy.QueCommand(_settings.Get("StopVideo"));
            _lastVideoMode = null;
        }
        //[DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        //public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        Process _videoPlayer;
        private void btnOpenVideo_Click(object sender, RoutedEventArgs e) {
            _hasFrontedPlayer = false;
            btnCloseVideo_Click(null, null);
            _videoPlayer = Process.Start(_settings.Get("PathGStreamer"), " -e -v udpsrc port=5600 ! application/x-rtp, payload=96 ! rtpjitterbuffer ! rtph264depay ! avdec_h264 ! fpsdisplaysink sync=false text-overlay=false");
        }
        private void btnCloseVideo_Click(object sender, RoutedEventArgs e) {
            if (_videoPlayer == null) return;
            if (_videoPlayer.HasExited) return;
            _videoPlayer.Kill();
            _videoPlayer = null;
        }
        private void rdb4G_Checked(object sender, RoutedEventArgs e) {
            _localAddress = _proxy.GetStationAddress();
            enableButtons();
        }
        private void rdbWiFi_Checked(object sender, RoutedEventArgs e) {
            _localAddress = _settings.Get("StationLanIp");
            enableButtons();
        }
        void enableButtons() {
            btnCloseVideo.IsEnabled = true;
            btnMissionPlanner.IsEnabled = true;
            btnMissionPlanner.IsEnabled = true;
            btnOpenPutty.IsEnabled = true;
            btnOpenVideo.IsEnabled = true;
            btnPhoto.IsEnabled = true;
            btnSettings.IsEnabled = true;
            btnStartArdu.IsEnabled = true;
            btnStopArdu.IsEnabled = true;
            btnStopVideo.IsEnabled = true;
            btnVideoHigh.IsEnabled = true;
            btnVideoLow.IsEnabled = true;
            btnVideoMed.IsEnabled = true;
        }
        private void btnOpenPutty_Click(object sender, RoutedEventArgs e) {
            string address;
            if ((bool)rdb4G.IsChecked) {
                address = _proxy.GetDroneAddress();
            } else {
                address = _settings.Get("DroneLanIp");
            }
            Process.Start(_settings.Get("PathPutty"), address + ":22");
        }
        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {

        }
        public string FormatBytes(long bytes) {
            const int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB", "B" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders) {
                if (bytes > max)
                    return string.Format("{0:##.#} {1}", decimal.Divide(bytes, max), order);

                max /= scale;
            }
            return "0 Bytes";
        }
        private void ProgressBar_MouseDown(object sender, MouseButtonEventArgs e) {
        }
        private void btnSettings_Click(object sender, RoutedEventArgs e) {
            this.Hide();
            btnCloseVideo_Click(null, null);
            var settingsWindow = new SettingsWindow(_settings);
            settingsWindow.ShowDialog();
            this.Show();
        }

        private void Window_Closed(object sender, EventArgs e) {
            btnCloseVideo_Click(null, null);
        }

        private void btnLog_Click(object sender, RoutedEventArgs e) {
            string url = "http://droneproxy.azurewebsites.net/logs/" + _proxy.DroneId + "/" + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".txt";
            Process.Start(url);
        }

        private void btnPhotos_Click(object sender, RoutedEventArgs e) {
            string url = "http://droneproxy.azurewebsites.net/files/?drone_id=" + _proxy.DroneId;
            Process.Start(url);
        }
    }

    public class WindowHandleInfo {
        private delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr lParam);

        private IntPtr _MainHandle;

        public WindowHandleInfo(IntPtr handle) {
            this._MainHandle = handle;
        }

        public List<IntPtr> GetAllChildHandles() {
            List<IntPtr> childHandles = new List<IntPtr>();

            GCHandle gcChildhandlesList = GCHandle.Alloc(childHandles);
            IntPtr pointerChildHandlesList = GCHandle.ToIntPtr(gcChildhandlesList);

            try {
                EnumWindowProc childProc = new EnumWindowProc(EnumWindow);
                EnumChildWindows(this._MainHandle, childProc, pointerChildHandlesList);
            } finally {
                gcChildhandlesList.Free();
            }

            return childHandles;
        }

        private bool EnumWindow(IntPtr hWnd, IntPtr lParam) {
            GCHandle gcChildhandlesList = GCHandle.FromIntPtr(lParam);

            if (gcChildhandlesList == null || gcChildhandlesList.Target == null) {
                return false;
            }

            List<IntPtr> childHandles = gcChildhandlesList.Target as List<IntPtr>;
            childHandles.Add(hWnd);

            return true;
        }
    }

}
