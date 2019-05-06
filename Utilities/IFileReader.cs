using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactRedux.Utilities {
    public interface IFileReader {
        List<string> ReadAllLines(string filePath);
        Task<List<string>> ReadAllLinesAsync(string filePath);
    }
}