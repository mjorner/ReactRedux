using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;

namespace ReactRedux {
    public sealed class AppConfiguration {
        public string DataPath { get { return Configuration["data_path"]; } }

        private IConfiguration Configuration;
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