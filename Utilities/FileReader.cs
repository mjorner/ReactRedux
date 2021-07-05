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

        public Task<string> ReadAllTextAsync(string filePath) {
            return System.IO.File.ReadAllTextAsync(filePath);
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

        private static async Task<bool> ReadAllLinesToFileReadContainerAsync(string filePath, FileReadContainer fileReadContainer) {
            using var sourceStream =
                new FileStream(
                    filePath,
                    FileMode.Open, FileAccess.Read, FileShare.Read,
                    bufferSize : 4096, useAsync : true);

            byte[] buffer = new byte[0x1000];
            int numRead;
            int currentCharPos = 0;
            while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0) {
                for (int i = 0; i < numRead; i++) {
                    char ch = Convert.ToChar(buffer[i]);
                    if (fileReadContainer.Lines[fileReadContainer.CurrentLineCount] == null) {
                        fileReadContainer.Lines[fileReadContainer.CurrentLineCount] = new TextLine(fileReadContainer.ConstantLineLength);
                    }
                    fileReadContainer.Lines[fileReadContainer.CurrentLineCount].Chars[currentCharPos++] = ch;
                    if (currentCharPos >= fileReadContainer.Lines[fileReadContainer.CurrentLineCount].Chars.Length) {
                        throw new Exception("Trying to read more chars than allocated by graph_line_length");
                    }
                    if (ch == '\n') {
                        fileReadContainer.CurrentLineCount++;
                        currentCharPos = 0;
                    }
                }
            }
            return true;
        }
    }
}