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

        [HttpGet("[action]")]
        public IEnumerable<ReadingFilenamesDto> GetFilenames() {
            string content = "";
            try {
                using(var reader = new StreamReader("config.json")) {
                    while (!reader.EndOfStream) {
                        content += reader.ReadLine();
                    }
                }
                List<ReadingFilenamesDto> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ReadingFilenamesDto>>(content);
                return list;
            } catch (Exception ex) {
                Console.WriteLine(ex);
                return new List<ReadingFilenamesDto>();
            }
        }

        [HttpGet("[action]")]
        public CopmpressedDataDto ReadGraphData(string filename) {
            List<TempReadingDto> list = new List<TempReadingDto>();
            double prevRead = 0;
            try {
                using(var reader = new StreamReader($"{Configuration.DataPath}{filename}")) {
                    while (!reader.EndOfStream) {
                        string line = reader.ReadLine();
                        string[] parts = line.Split(";").ToArray();
                        TempReadingDto reading = new TempReadingDto();
                        reading.DateTime = parts[0];
                        double d;
                        if (double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out d)) {
                            if (!IsApproximatelyEqualTo(prevRead, d)) {
                                reading.TemperatureC = d;
                                list.Add(reading);
                                prevRead = d;
                            }
                        } else {
                            Console.WriteLine($"Unable to parse double: {parts[1]}");
                        }
                    }
                    reader.Close();
                }
            } catch (Exception ex) {
                Console.WriteLine(ex);
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

        private static bool IsApproximatelyEqualTo(double initialValue, double value) {
            return (Math.Abs(initialValue - value) < 0.00001);
        }
    }
}