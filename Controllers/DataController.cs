using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Snappy;

namespace ReactRedux.Controllers {
    [Route("api/[controller]")]
    public class DataController : Controller {

        [HttpGet("[action]")]
        public IEnumerable<ReadingFilenames> GetFilenames() {
            string content = "";
            try {
                using(var reader = new StreamReader("config.json")) {
                    while (!reader.EndOfStream) {
                        content += reader.ReadLine();
                    }
                }
                List<ReadingFilenames> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ReadingFilenames>>(content);
                return list;
            } catch (Exception ex) {
                Console.WriteLine(ex);
                return new List<ReadingFilenames>();
            }
        }

        [HttpGet("[action]")]
        public CopmpressedData ReadGraphData(string filename) {
            List<TempReading> list = new List<TempReading>();
            double prevRead = 0;
            try {
                using(var reader = new StreamReader($"/home/pi/OutRAM/{filename}")) {
                    while (!reader.EndOfStream) {
                        string line = reader.ReadLine();
                        string[] parts = line.Split(";").ToArray();
                        TempReading reading = new TempReading();
                        reading.DateTime = parts[0];
                        double d;
                        if (double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out d)) {
                            reading.TemperatureC = d;
                            if (!IsApproximatelyEqualTo(prevRead, d)) {
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
            string str = Convert.ToBase64String(compressed,0, compressed.Length, Base64FormattingOptions.None);
            return new CopmpressedData() { Bytes = str, OrigLen = inputLength };
        }

        [HttpGet("[action]")]
        public OutResult ReadOutFile(string filename, string title) {
            string line = "";
            try {
                using(var reader = new StreamReader($"/home/pi/OutRAM/{filename}")) {
                    line = reader.ReadLine();
                    reader.Close();
                }
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return new OutResult() { Str = line, Title = title };
        }

        private static bool IsApproximatelyEqualTo(double initialValue, double value) {
            return (Math.Abs(initialValue - value) < 0.00001);
        }

        public class CopmpressedData {
            public string Bytes { get; set; }
            public int OrigLen { get; set; }
        }

        public class OutResult {
            public string Str { get; set; }
            public string Title { get; set; }
        }

        public class TempReading {
            public string DateTime { get; set; }
            public double TemperatureC { get; set; }
        }

        public class ReadingFilenames {
            public string OutFile { get; set; }
            public string CsvFile { get; set; }
            public string Title { get; set; }
        }
    }
}