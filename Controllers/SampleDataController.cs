using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ReactRedux.Controllers {
    [Route("api/[controller]")]
    public class SampleDataController : Controller {
        private static string[] Summaries = new [] {
            "Freezing",
            "Bracing",
            "Chilly",
            "Cool",
            "Mild",
            "Warm",
            "Balmy",
            "Hot",
            "Sweltering",
            "Scorching"
        };

        [HttpGet("[action]")]
        public IEnumerable<ReadingFilenames> GetFilenames() {
            List<ReadingFilenames> list = new List<ReadingFilenames>();
            list.Add(new ReadingFilenames() { OutFile = "attictemp.out", CsvFile = "attictemp.csv" });
            list.Add(new ReadingFilenames() { OutFile = "edithtemp.out", CsvFile = "edithtemp.csv" });
            list.Add(new ReadingFilenames() { OutFile = "ellietemp.out", CsvFile = "ellietemp.csv" });
            list.Add(new ReadingFilenames() { OutFile = "freezer.out", CsvFile = "freezer.csv" });
            list.Add(new ReadingFilenames() { OutFile = "fridge.out", CsvFile = "fridge.csv" });
            list.Add(new ReadingFilenames() { OutFile = "garagetemp.out", CsvFile = "garagetemp.csv" });
            list.Add(new ReadingFilenames() { OutFile = "outsidelightlux.out", CsvFile = "outsidelightlux.csv" });
            list.Add(new ReadingFilenames() { OutFile = "outsideroom.out", CsvFile = "outsideroom.csv" });
            list.Add(new ReadingFilenames() { OutFile = "outsidetemp2.out", CsvFile = "outsidetemp2.csv" });
            list.Add(new ReadingFilenames() { OutFile = "outsidetemp.out", CsvFile = "outsidetemp.csv" });
            list.Add(new ReadingFilenames() { OutFile = "pooltemp.out", CsvFile = "pooltemp.csv" });
            return list;
        }

        [HttpGet("[action]")]
        public IEnumerable<TempReading> WeatherForecasts(string filename) {
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
                }
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return list;
        }

        [HttpGet("[action]")]
        public OutResult ReadFile(string filename) {
            string line = "";
            try {
                using(var reader = new StreamReader($"/home/pi/OutRAM/{filename}")) {
                    line = reader.ReadLine();
                }
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return new OutResult() { Str = line, Filename = filename };
        }

        private static bool IsApproximatelyEqualTo(double initialValue, double value) {
            return (Math.Abs(initialValue - value) < 0.00001);
        }

        public class OutResult {
            public string Str { get; set; }
            public string Filename { get; set; }
        }

        public class TempReading {
            public string DateTime { get; set; }
            public double TemperatureC { get; set; }
        }

        public class ReadingFilenames {
            public string OutFile { get; set; }
            public string CsvFile { get; set; }
        }
    }
}