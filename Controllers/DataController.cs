using System;
using System.Collections.Generic;
using System.Linq;
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
        public InitInfoDto GetFilenames() {
            List<string> lines = FileReader.ReadAllLines("config.json");
            string content = string.Join("", lines);
            List<ReadingFilenamesDto> list = JsonConvert.DeserializeObject<List<ReadingFilenamesDto>>(content);
            return new InitInfoDto() { FileNames = list, TimePeriods = TimePeriods.AllTimePeriods };
        }

        [HttpGet("[action]")]
        public CopmpressedDataDto ReadGraphData(string filename, int columnIndex, string timeSpan) {
            List<ValueReadingDto> list = new List<ValueReadingDto>();
            DateTime? first = null;
            List<string> lines = FileReader.ReadAllLines($"{Configuration.DataPath}{filename}");
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
            string str = StringCompressor.Compress(list);
            return new CopmpressedDataDto() { Base64Bytes = str };
        }

        [HttpGet("[action]")]
        public OutResultDto ReadOutFile(string filename, string title) {
            List<string> lines = FileReader.ReadAllLines($"{Configuration.DataPath}{filename}");
            string line = string.Join("", lines);
            return new OutResultDto() { Str = line, Title = title };
        }

        [HttpGet("[action]")]
        public TxtDto ReadTextFile(string filename) {
            List<string> lines = FileReader.ReadAllLines($"{Configuration.DataPath}{filename}");
            string line = string.Join("\n", lines);
            return new TxtDto() { Text = line };
        }
    }
}