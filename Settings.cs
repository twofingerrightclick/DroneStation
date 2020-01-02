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
    public class Settings {
        string _filePath;
        Dictionary<string, string> _settings;
        public int Count
        {
            get { return _settings.Count; }
        }
        public string Get(string name, string defaultValue) {
            lock (_settings) {
                if (_settings.ContainsKey(name)) {
                    return _settings[name];
                } else {
                    return defaultValue;
                }
            }
        }
        public string Get(string name) {
            return Get(name, default(string));
        }
        public void Set(string name, string value) {
            lock (_settings) {
                if (_settings.ContainsKey(name)) {
                    if (value == null) {
                        _settings.Remove(name);
                    } else {
                        _settings[name] = value;
                    }
                } else {
                    _settings.Add(name, value);
                }
            }
        }
        public Settings(string filePath) {
            _filePath = filePath;
            loadSettings();
        }
        public void SaveSettings() {
            lock (_settings) {
                var writer = new XmlTextWriter(_filePath, Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteStartElement("Settings");
                foreach (var setting in _settings) {
                    writer.WriteStartElement("Setting");
                    writer.WriteAttributeString("Name", setting.Key);
                    writer.WriteAttributeString("Value", setting.Value);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement(); // Settings
                writer.WriteEndDocument();
                writer.Close();
            }
        }
        void loadSettings() {
            _settings = new Dictionary<string, string>();
            if (File.Exists(_filePath)) {
                var xml = new XmlDocument();
                xml.Load(_filePath);
                var xmlSettings = xml.GetElementsByTagName("Setting");
                foreach (XmlElement el in xmlSettings) {
                    _settings.Add(el.GetAttribute("Name"), el.GetAttribute("Value"));
                }
            }
        }
    }
}
