using System.Threading.Tasks;
namespace ReactRedux.Utilities {
    public interface IStringCompressor {
        string Compress(object obj);
    }
}