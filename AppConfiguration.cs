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

        public string LogFiles {
            get { 
                string s = Configuration["log_files"];
                return s == null ? "" : s;
            }
        }

        private readonly IConfiguration Configuration;
        public AppConfiguration (IConfiguration configuration) {
            Configuration = configuration;
        }

        public void Validate () {
            if (DataPath.Last () != System.IO.Path.DirectorySeparatorChar) {
                throw new System.Exception ($"{DataPath} must end with {System.IO.Path.DirectorySeparatorChar}.");
            }
            if (!System.IO.Directory.Exists (DataPath)) {
                throw new System.Exception ($"{DataPath} does not exist.");
            }
            if (SnapShotFile.Length == 0 && SnapShotPath.Length != 0) {
                throw new System.Exception ($"{SnapShotFile} is missing.");
            }
            if (SnapShotFile.Length != 0 && SnapShotPath.Length == 0) {
                throw new System.Exception ($"{SnapShotPath} is missing.");
            }
            if (Uname == null) {
                 throw new System.Exception ($"{Uname} is missing.");
            }
            if (Pw == null) {
                 throw new System.Exception ($"{Pw} is missing.");
            }
        }
    }
}