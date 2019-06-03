using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using ReactRedux.Dtos;

namespace ReactRedux {
    public sealed class AppConfiguration {
        private readonly IConfiguration Configuration;
        public AppConfiguration (IConfiguration configuration) {
            Configuration = configuration;
        }
        public string DataPath { get { return Configuration["data_path"]; } }
        public string Uname { get { return Configuration["uname"]; } }
        public string Pw { get { return Configuration["pw"]; } }
        public string AppTitle { get { return Configuration["app_title"]; } }
        public string SnapShotPath => TryParseStringEmptyDefault("snap_shot_path");
        public string SnapShotFile => TryParseStringEmptyDefault("snap_shot_file");
        public string LogPath => TryParseStringEmptyDefault("log_path");
        private string LogFilesRaw => TryParseStringEmptyDefault("log_files");
        public string LogFiles => ValidateAvailableFiles(LogFilesRaw);
        public int GraphLineLength => TryParseIntWithDefault("graph_line_length", 100);
        public int GraphConcurrencyCount => TryParseIntWithDefault("graph_concurrency_count", 10);
        public int GraphLineCount => TryParseIntWithDefault("graph_line_count", 50000);

        private string TryParseStringEmptyDefault(string key) {
            string s = Configuration[key];
            return s == null ? "" : s;
        }
        private int TryParseIntWithDefault(string key, int defaultValue) {
            string s = Configuration[key];
            int value;
            if (int.TryParse(s, out value)) {
                return value;
            }
            return defaultValue;
        }

        private string ValidateAvailableFiles(string allLogFiles) {
            string[] files = allLogFiles.Split(';').Where(x => File.Exists($"{LogPath}{x}")).ToArray();
            if (files.Length == 0) { 
                return "";
            }
            return files.Aggregate((current, next) => current + ';' + next);
        }

        public void Validate () {
            if (DataPath.Last () != System.IO.Path.DirectorySeparatorChar) {
                throw new System.Exception ($"DataPath {DataPath} must end with {System.IO.Path.DirectorySeparatorChar}.");
            }
            if (!System.IO.Directory.Exists (DataPath)) {
                throw new System.Exception ($"DataPath does not exist.");
            }
            if (SnapShotFile.Length == 0 && SnapShotPath.Length != 0) {
                throw new System.Exception ($"SnapShotFile is missing.");
            }
            if (SnapShotFile.Length != 0 && SnapShotPath.Length == 0) {
                throw new System.Exception ($"SnapShotPath is missing.");
            }
            if (LogPath.Length == 0 && LogFilesRaw.Length != 0) {
                throw new System.Exception ($"LogPath is missing.");
            }
            if (LogPath.Length != 0 && LogFilesRaw.Length == 0) {
                throw new System.Exception ($"LogFiles is missing.");
            }
            if (Uname == null) {
                 throw new System.Exception ($"Uname is missing.");
            }
            if (Pw == null) {
                 throw new System.Exception ($"Pw is missing.");
            }
        }
    }
}