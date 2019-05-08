using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReactRedux.Dtos;

namespace ReactRedux.Utilities {
    internal static class StringParser {
        private static bool TryParseLine(string line, int columnIndex, out ValueReadingDto reading) {
            string[] parts = line.Split(";").ToArray();
            reading = new ValueReadingDto();
            reading.DateTime = parts[0];
            DateTime dt = DateTime.Parse(reading.DateTime);
            double d;
            if (double.TryParse(parts[columnIndex], NumberStyles.Any, CultureInfo.InvariantCulture, out d)) {
                reading.Value = d;
                return true;
            } else {
                Console.WriteLine($"Unable to parse double: {parts[1]}");
            }
            return false;
        }

        public static List<ValueReadingDto> ParseValueReadings(List<string> lines, int columnIndex, string timeSpan) {
            lines.Reverse(); //Start from the end!

            DateTime? first = null;
            List<ValueReadingDto> list = new List<ValueReadingDto>();
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
            return list;
        }
    }
}