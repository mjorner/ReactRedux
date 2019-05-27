using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ReactRedux.Utilities {
    internal sealed class FileReader : IFileReader {
        private const char NEWLINE = '\n';
        public List<string> ReadAllLines(string filePath) {
            List<string> lines = new List<string>();
            try {
                using(var reader = new StreamReader(filePath)) {
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

        private bool ReadAllLines(string filePath, FileReadContainer fileReadContainer) {
            try {
                using(var reader = new StreamReader(filePath)) {
                    while (!reader.EndOfStream) {
                        ReadLine(reader, fileReadContainer);
                        fileReadContainer.CurrentLineCount++;
                        if (fileReadContainer.CurrentLineCount >= fileReadContainer.Lines.Length) {
                            throw new Exception($"Trying to read more lines than allocated by graph_line_count for {filePath}.");
                        }
                    }
                    reader.Close();
                    return true;
                }
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return false;
        }

        private static void ReadLine(StreamReader reader, FileReadContainer fileReadContainer) {
            if (fileReadContainer.Lines[fileReadContainer.CurrentLineCount] == null) {
                fileReadContainer.Lines[fileReadContainer.CurrentLineCount] = new TextLine(fileReadContainer.ConstantLineLength);
            }
            int charIndex = 0;
            char ch = '\0';
            do {
                ch = (char) reader.Read();
                if (charIndex >= fileReadContainer.Lines[fileReadContainer.CurrentLineCount].Chars.Length) {
                     throw new Exception("Trying to read more chars than allocated by graph_line_length");
                }
                fileReadContainer.Lines[fileReadContainer.CurrentLineCount].Chars[charIndex++] = ch;
            } while (ch != NEWLINE);
        }

        public Task<List<string>> ReadAllLinesAsync(string filePath) {
            return Task.Run(() => ReadAllLines(filePath));
        }

        public Task<bool> ReadAllLinesAsync(string filePath, FileReadContainer fileReadContainer) {
            return Task.Run(() => ReadAllLines(filePath, fileReadContainer));
        }
    }
}