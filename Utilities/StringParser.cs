using System;
using System.Globalization;
using System.Linq;
using ReactRedux.Dtos;

namespace ReactRedux.Utilities {
    internal static class StringParser {
        public static bool TryParseLine(string line, int columnIndex, out ValueReadingDto reading) {
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
    }
}