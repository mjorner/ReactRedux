using System;
using System.Collections.Generic;
using System.IO;

namespace ReactRedux.Utilities {
    internal sealed class FileReader : IFileReader {
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
    }
}