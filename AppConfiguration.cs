using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using ReactRedux.Dtos;

namespace ReactRedux {
    public sealed class AppConfiguration {
        public string DataPath { get { return Configuration["data_path"]; } }
        public string Uname { get { return Configuration["uname"]; } }
        public string Pw { get { return Configuration["pw"]; } }
        public string AppTitle { get { return Configuration["app_title"]; } }

        public string SnapShotPath {
            get {
                string s = Configuration["snap_shot_path"];
                return s == null ? "" : s;
            }
        }

        public string SnapShotFile {
            get {
                string s = Configuration["snap_shot_file"];
                return s == null ? "" : s;
            }
        }

        public string LogPath {
            get { 
                string s = Configuration["log_path"];
                return s == null ? "" : s;
            }
        }

        private string LogFilesRaw {
            get { 
                string s = Configuration["log_files"];
                return s == null ? "" : s;
            }
        }
        public string LogFiles {
            get {
                return ValidateAvailableFiles(LogFilesRaw);
            }
        }

        private string ValidateAvailableFiles(string allLogFiles) {
            string[] files = allLogFiles.Split(';').Where(x => File.Exists($"{LogPath}{x}")).ToArray();
            if (files.Length == 0) { 
                return "";
            }
            return files.Aggregate((current, next) => current + ';' + next);
        }

        private readonly IConfiguration Configuration;
        public AppConfiguration (IConfiguration configuration) {
            Configuration = configuration;
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