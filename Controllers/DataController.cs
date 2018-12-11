using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ReactRedux.Dtos;
using Snappy;

namespace ReactRedux.Controllers {
    [Route("api/[controller]")]
    public class DataController : Controller {
        private readonly AppConfiguration Configuration;

        public DataController(AppConfiguration configuration) {
            Configuration = configuration;
        }

        //We also need to inject a file reader.
        //config.json should come from AppConfiguration and Not the file.
        [HttpGet("[action]")]
        public InitInfoDto GetFilenames() {
            string content = "";
            try {
                using(var reader = new StreamReader("config.json")) {
                    while (!reader.EndOfStream) {
                        content += reader.ReadLine();
                    }
                }
                List<ReadingFilenamesDto> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ReadingFilenamesDto>>(content);
                return new InitInfoDto() { FileNames = list, TimePeriods = new List<string>(new [] { "24h", "48h", "7d", "14d" }) };
            } catch (Exception ex) {
                Console.WriteLine(ex);
                return new InitInfoDto();
            }
        }

        private bool IsDateWithinBoundry(DateTime dt, DateTime first, string timeSpan) {
            TimeSpan ts = first.Subtract(dt);
            if (timeSpan == "24h") {
                return ts.TotalSeconds < (24 * 60 * 60);
            } else if (timeSpan == "48h") {
                return ts.TotalSeconds < (2 * 24 * 60 * 60);
            } else if (timeSpan == "7d") {
                return ts.TotalSeconds < (7 * 24 * 60 * 60);
            }
            return ts.TotalSeconds < (14 * 24 * 60 * 60);
        }

        private List<string> ReadAllLines(string filename) {
            List<string> lines = new List<string>();
            try {
                using(var reader = new StreamReader($"{Configuration.DataPath}{filename}")) {
                    while (!reader.EndOfStream) {
                        lines.Add(reader.ReadLine());
                    }
                    reader.Close();
                }
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return lines;
        }

        [HttpGet("[action]")]
        public CopmpressedDataDto ReadGraphData(string filename, int columnIndex, string timeSpan) {
            List<ValueReadingDto> list = new List<ValueReadingDto>();
            DateTime first = DateTime.MaxValue;
            List<string> lines = ReadAllLines(filename);
            lines.Reverse();
            foreach (string line in lines) {
                string[] parts = line.Split(";").ToArray();
                ValueReadingDto reading = new ValueReadingDto();
                reading.DateTime = parts[0];
                DateTime dt = DateTime.Parse(reading.DateTime);
                if (first == DateTime.MaxValue) {
                    first = dt;
                }
                double d;
                if (double.TryParse(parts[columnIndex], NumberStyles.Any, CultureInfo.InvariantCulture, out d)) {
                    reading.Value = d;
                    if (IsDateWithinBoundry(dt, first, timeSpan)) {
                        list.Add(reading);
                    }
                } else {
                    Console.WriteLine($"Unable to parse double: {parts[1]}");
                }
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            byte[] array = Encoding.UTF8.GetBytes(json);
            var compressed = SnappyCodec.Compress(array);
            var inputLength = array.Length;
            string str = Convert.ToBase64String(compressed, 0, compressed.Length, Base64FormattingOptions.None);
            return new CopmpressedDataDto() { Base64Bytes = str, OrigLen = inputLength };
        }

        [HttpGet("[action]")]
        public OutResultDto ReadOutFile(string filename, string title) {
            string line = "";
            try {
                using(var reader = new StreamReader($"{Configuration.DataPath}{filename}")) {
                    while (!reader.EndOfStream) {
                        line += reader.ReadLine();
                    }
                    reader.Close();
                }
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return new OutResultDto() { Str = line, Title = title };
        }

        [HttpGet("[action]")]
        public TxtDto ReadTextFile(string filename) {
            string line = "";
            try {
                using(var reader = new StreamReader($"{Configuration.DataPath}{filename}")) {
                    while (!reader.EndOfStream) {
                        line += reader.ReadLine() + "\n";
                    }
                    reader.Close();
                }
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return new TxtDto() { Text = line };
        }

        /* private static bool IsApproximatelyEqualTo(double initialValue, double value) {
            return (Math.Abs(initialValue - value) < 0.00001);
        }*/
    }
}