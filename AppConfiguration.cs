using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using ReactRedux.Dtos;

namespace ReactRedux {
    public sealed class AppConfiguration {
        public string DataPath { get { return Configuration["data_path"]; } }
        public string Uname { get { return Configuration["uname"]; } }
        public string Pw { get { return Configuration["pw"]; } }

        private readonly IConfiguration Configuration;
        public AppConfiguration(IConfiguration configuration) {
            Configuration = configuration;
        }

        public void Validate() {
            if (DataPath.Last() != System.IO.Path.DirectorySeparatorChar) {
                throw new System.Exception($"{DataPath} must end with {System.IO.Path.DirectorySeparatorChar}.");
            }
            if (!System.IO.Directory.Exists(DataPath)) {
                throw new System.Exception($"{DataPath} does not exist.");
            }
        }
    }
}