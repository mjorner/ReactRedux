using System.Collections.Generic;

namespace ReactRedux.Utilities {
    public interface IFileReader {
        List<string> ReadAllLines(string filePath);
        
    }
}