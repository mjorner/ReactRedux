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
    public class SampleDataController : Controller {

        [HttpGet("[action]")]
        public IEnumerable<ReadingFilenames> GetFilenames() {
            List<ReadingFilenames> list = new List<ReadingFilenames>();
            list.Add(new ReadingFilenames() { OutFile = "attictemp.out", CsvFile = "attictemp.csv", Title = "Attic" });
            list.Add(new ReadingFilenames() { OutFile = "edithtemp.out", CsvFile = "edithtemp.csv", Title = "Edith" });
            list.Add(new ReadingFilenames() { OutFile = "ellietemp.out", CsvFile = "ellietemp.csv", Title = "Ellie" });
            list.Add(new ReadingFilenames() { OutFile = "freezer.out", CsvFile = "freezer.csv", Title = "Freezer" });
            list.Add(new ReadingFilenames() { OutFile = "fridge.out", CsvFile = "fridge.csv", Title = "Fridge" });
            list.Add(new ReadingFilenames() { OutFile = "garagetemp.out", CsvFile = "garagetemp.csv", Title = "Garage" });
            list.Add(new ReadingFilenames() { OutFile = "outsidelightlux.out", CsvFile = "outsidelightlux.csv", Title = "Outside lux" });
            list.Add(new ReadingFilenames() { OutFile = "outsideroom.out", CsvFile = "outsideroom.csv", Title = "Conservatory" });
            list.Add(new ReadingFilenames() { OutFile = "outsidetemp2.out", CsvFile = "outsidetemp2.csv", Title = "Outside 2" });
            list.Add(new ReadingFilenames() { OutFile = "outsidetemp.out", CsvFile = "outsidetemp.csv", Title = "Outside 1" });
            list.Add(new ReadingFilenames() { OutFile = "pooltemp.out", CsvFile = "pooltemp.csv", Title = "Pool" });
            return list;
        }

        [HttpGet("[action]")]
        public CopmpressedData WeatherForecasts(string filename) {
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
            return new CopmpressedData() { Bytes = BitConverter.ToString(compressed), OrigLen = inputLength };
        }

        [HttpGet("[action]")]
        public OutResult ReadFile(string filename, string title) {
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