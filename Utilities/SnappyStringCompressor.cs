using System;
using System.Text;
using System.Threading.Tasks;
using Snappy;
using Utf8Json;

namespace ReactRedux.Utilities {
    internal sealed class SnappyStringCompressor : IStringCompressor {
        public string Compress(object obj) {
            byte[] array = JsonSerializer.Serialize(obj);
            byte[] compressed = SnappyCodec.Compress(array);
            string str = Convert.ToBase64String(compressed, 0, compressed.Length, Base64FormattingOptions.None);
            return str;
        }
    }
}