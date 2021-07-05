using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactRedux.Utilities {
    public interface IFileReader {
        List<string> ReadAllLines(string filePath);
        Task<string> ReadAllTextAsync(string filePath);
        Task<List<string>> ReadAllLinesAsync(string filePath);
        Task<bool> ReadAllLinesAsync(string filePath, FileReadContainer fileReadContainer);
    }
}