using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    /// Interaction logic for ScriptWindow.xaml
    /// </summary>
    public partial class ScriptWindow : Window {
        public ScriptWindow(string script) {
            InitializeComponent();
            txtScript.Text = script;
        }
        private void btnOk_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e) {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "droneproxy.py"; // Default file name
            dlg.DefaultExt = ".py"; // Default file extension
            dlg.Filter = "Python Script (.py)|*.py"; // Filter files by extension
            var result = dlg.ShowDialog();
            if (result == true) {
                File.WriteAllText(dlg.FileName, txtScript.Text);
            }
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e) {
            Clipboard.SetText(txtScript.Text);
        }
    }
}
