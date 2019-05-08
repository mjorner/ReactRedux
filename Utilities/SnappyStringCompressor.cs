using System;
using System.Text;
using System.Threading.Tasks;
using Snappy;

namespace ReactRedux.Utilities {
    internal sealed class SnappyStringCompressor : IStringCompressor {
        public string Compress(object obj) {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            byte[] array = Encoding.UTF8.GetBytes(json);
            var compressed = SnappyCodec.Compress(array);
            string str = Convert.ToBase64String(compressed, 0, compressed.Length, Base64FormattingOptions.None);
            return str;
        }
    }
}