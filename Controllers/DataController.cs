using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ReactRedux.Dtos;
using ReactRedux.Utilities;

namespace ReactRedux.Controllers {
    [Route("api/[controller]")]
    public class DataController : Controller {
        private readonly AppConfiguration Configuration;
        private readonly IFileReader FileReader;
        private readonly IStringCompressor StringCompressor;

        public DataController(AppConfiguration configuration, IFileReader fileReader, IStringCompressor stringCompressor) {
            Configuration = configuration;
            FileReader = fileReader;
            StringCompressor = stringCompressor;
        }

        [HttpGet("[action]")]
        public async Task<InitInfoDto> GetFilenames() {
            string currentPath = Directory.GetCurrentDirectory();
            string configPath = $"{Directory.GetParent(currentPath)}{Path.DirectorySeparatorChar}webconfig.json"; 
            List<string> lines = await FileReader.ReadAllLinesAsync(configPath);
            string content = string.Join("", lines);
            List<ReadingFilenamesDto> list = JsonConvert.DeserializeObject<List<ReadingFilenamesDto>>(content);
            return new InitInfoDto() { FileNames = list, TimePeriods = TimePeriods.AllTimePeriods };
        }

        [HttpGet("[action]")]
        public async Task<CopmpressedDataDto> ReadGraphData(string filename, int columnIndex, string timeSpan) {
            List<ValueReadingDto> list = new List<ValueReadingDto>();
            if (filename == null) {
                return new CopmpressedDataDto() { Base64Bytes = StringCompressor.Compress(list) };
            }

            DateTime? first = null;
            List<string> lines = await FileReader.ReadAllLinesAsync($"{Configuration.DataPath}{filename}");
            lines.Reverse();
            foreach (string line in lines) {
                ValueReadingDto reading = null;
                if (StringParser.TryParseLine(line, columnIndex, out reading)) {
                    DateTime dt = DateTime.Parse(reading.DateTime);
                    if (!first.HasValue) {
                        first = dt;
                    }
                    if (!TimePeriods.IsDateWithinBoundry(dt, first.Value, timeSpan)) {
                        break;
                    }
                    list.Add(reading);
                }
            }
            string str = await StringCompressor.CompressAsync(list);
            return new CopmpressedDataDto() { Base64Bytes = str };
        }

        [HttpGet("[action]")]
        public async Task<OutResultDto> ReadOutFile(string filename, string title) {
            List<string> lines = await FileReader.ReadAllLinesAsync($"{Configuration.DataPath}{filename}");
            string line = string.Join("", lines);
            return new OutResultDto() { Str = line, Title = title };
        }

        [HttpGet("[action]")]
        public async Task<TxtDto> ReadTextFile(string filename) {
            List<string> lines = await FileReader.ReadAllLinesAsync($"{Configuration.DataPath}{filename}");
            string line = string.Join("\n", lines);
            return new TxtDto() { Text = line };
        }

        [HttpGet("[action]")]
        public ConfigurationDto GetAppConfiguration() {
            return new ConfigurationDto() { AppTitle = Configuration.AppTitle, SnapShotFile = Configuration.SnapShotFile, LogFiles = Configuration.LogFiles };
        }

        [HttpGet("[action]")]
        public async Task<TxtDto> ReadSysLog(string filename) {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                return new TxtDto() {Text = "Not Linux!"};
            }
            List<string> lines = await FileReader.ReadAllLinesAsync($"/var/log/{filename}");
            string line = string.Join("\n", lines);
            return new TxtDto() { Text = line };
        }
    }
}