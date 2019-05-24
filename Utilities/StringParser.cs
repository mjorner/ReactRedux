using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ReactRedux.Dtos;

namespace ReactRedux.Utilities {
    internal static class StringParser {
        private static bool FindPartLength(char[] line, char separator, int start, out int length) {
            length = 0;
            for (int i = start; i < line.Length; i++) {
                length++;
                if (line[i] == separator || line[i] == '\n') {
                    return true;
                }
            }
            return false;
        }

        private static bool TryParseLine(char[] line, int columnIndex, ValueReadingDto reading) {
            int length = 0;
            int start = 0;
            if (!FindPartLength(line, ';', start, out length)) {
                return false;
            }
            reading.DateTime = new string(new Span<char>(line, 0, length - 1));
            for (int i = 0; i < columnIndex; i++) {
                start += length;
                if (!FindPartLength(line, ';', start, out length)) {
                    return false;
                }
            }
            double value;
            if (double.TryParse(new ReadOnlySpan<char>(line, start, length - 1), NumberStyles.Any, CultureInfo.InvariantCulture, out value)) {
                reading.Value = value;
                return true;
            } else {
                Console.WriteLine($"Unable to parse double: {line}");
            }
            return false;
        }

        public static int ParseValueReadings(FileReadContainer fileReadContainer, int columnIndex, string timeSpan) {
            int count = 0;
            DateTime? first = null;
            for (int i = fileReadContainer.CurrentLineCount; i > 0; i--) {
                if (fileReadContainer.Values[count] == null) {
                    fileReadContainer.Values[count] = new ValueReadingDto();
                }
                if (StringParser.TryParseLine(fileReadContainer.Lines[i - 1].Chars, columnIndex, fileReadContainer.Values[count])) {
                    DateTime dt = DateTime.Parse(fileReadContainer.Values[count].DateTime);
                    if (!first.HasValue) {
                        first = dt;
                    }
                    if (!TimePeriods.IsDateWithinBoundry(dt, first.Value, timeSpan)) {
                        break;
                    }
                    count++;
                }
            }
            return count;
        }
    }
}