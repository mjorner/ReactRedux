using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public async Task<List<string>> ReadAllLinesAsync(string filePath) {
            string[] lines = await System.IO.File.ReadAllLinesAsync(filePath, System.Text.Encoding.UTF8);
            return lines.ToList();
        }

        public Task<bool> ReadAllLinesAsync(string filePath, FileReadContainer fileReadContainer) {
            return ReadAllLinesToFileReadContainerAsync(filePath, fileReadContainer);
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

        private async Task<bool> ReadAllLinesToFileReadContainerAsync(string filePath, FileReadContainer fileReadContainer) {
            string[] lines = await System.IO.File.ReadAllLinesAsync(filePath, System.Text.Encoding.UTF8);
            if (lines.Length >= fileReadContainer.Lines.Length) {
                throw new Exception($"Trying to read more lines than allocated by graph_line_count for {filePath}.");
            }
            for (int i = 0; i < lines.Length; i++) {
                AppendTextToFileReadContainer(fileReadContainer, lines[i]);
                fileReadContainer.CurrentLineCount++;
            }
            return true;
        }

        private static void AppendTextToFileReadContainer(FileReadContainer fileReadContainer, string text) {
            for (int i = 0; i < text.Length; i++) {
                if (fileReadContainer.Lines[fileReadContainer.CurrentLineCount] == null) {
                    fileReadContainer.Lines[fileReadContainer.CurrentLineCount] = new TextLine(fileReadContainer.ConstantLineLength);
                }
                if (i >= fileReadContainer.Lines[fileReadContainer.CurrentLineCount].Chars.Length) {
                    throw new Exception("Trying to read more chars than allocated by graph_line_length");
                }
                fileReadContainer.Lines[fileReadContainer.CurrentLineCount].Chars[i] = text[i];
            }
            fileReadContainer.Lines[fileReadContainer.CurrentLineCount].Chars[text.Length] = NEWLINE;
        }
    }
}