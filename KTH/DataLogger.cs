using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Windows.Forms;

namespace KTH {
    public class LoggingItem {

        public int Id;
        public bool Enabled;
        public string Description;
        public string Unit;
        public double Value;

        public LoggingItem(int id, string desc, string unit) {
            Id = id;
            Description = desc;
            Unit = unit;
            Enabled = true;
        }

        override public string ToString() {
            return Description;
        }

    }

    public class DataLogger {

        public List<LoggingItem> LoggingItemList;
        int IdCounter;
        string CSVPath;
        CultureInfo culture_info;

        public DataLogger() {
            IdCounter = 0;
            LoggingItemList = new List<LoggingItem>();
            culture_info = new CultureInfo("en-US");
        }

        public bool SetFilePath(string path, bool overwrite) {
            if(overwrite || !File.Exists(path)) {
                CSVPath = path;
                return true;
            }
            return false;
        }

        public int AddLogItem(string desc, string unit) {
            LoggingItemList.Add(new LoggingItem(IdCounter, desc, unit));
            return IdCounter++;
        }

        public bool RemoveLogItem(int id) {
            int index = LoggingItemList.FindIndex(s => s.Id == id);

            if(index != -1) {
                LoggingItemList.RemoveAt(index);
                return true;
            }

            return false;
        }

        public bool UpdateValue(int id, double value) {
            int index = LoggingItemList.FindIndex(s => s.Id == id);
            if(index != -1) {
                LoggingItemList[index].Value = value;
                return true;
            }

            return false;
        }

        public void WriteHeader() {
            string header_line = "Timestamp,";
            foreach(LoggingItem logging_item in LoggingItemList) {
                if(logging_item.Enabled) {
                    header_line += logging_item.Description + ",";
                }
            }
            header_line = header_line.Substring(0, header_line.Length - 1);
            header_line += Environment.NewLine; 
            try {
                File.WriteAllText(CSVPath, header_line);
            } catch(Exception ex) {
            }
        }

        public void WriteLine() {
            string csv_line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ",";
            foreach(LoggingItem logging_item in LoggingItemList) {
                if(logging_item.Enabled) {
                    csv_line += logging_item.Value.ToString(culture_info) + ",";
                }
            }
            csv_line = csv_line.Substring(0, csv_line.Length - 1);
            csv_line += Environment.NewLine;
            try {
                File.AppendAllText(CSVPath, csv_line);
            } catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
