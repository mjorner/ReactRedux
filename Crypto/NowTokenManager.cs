using System;
using System.Text;
using System.Security.Cryptography;

namespace ReactRedux.Crypto {
    public class NowTokenManager : INowTokenManager {
        private readonly AppConfiguration Configuration;
        public NowTokenManager(AppConfiguration configuration) {
            Configuration = configuration;
        }

        public string GenerateToken() {
            return GenerateToken(DateTime.UtcNow);
        }
        public bool ValidateToken(string token) {
            return CompareToken(token);
        }

        private string GenerateToken(DateTime now) {
            using(var sha = SHA256.Create()) {
                byte[] bytes = Encoding.UTF8.GetBytes($"{Configuration.AuthSalt}{now.Year}{now.Month}{now.Day}");
                bytes = sha.ComputeHash(bytes);
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private bool CompareToken(string token) {
            DateTime now = DateTime.UtcNow;
            string currentHash = GenerateToken(now);
            if (SlowByteByByteEquals(currentHash, token)) {
                return true;
            }
            if (now.Hour == 0 && now.Minute < 5) {
                currentHash = GenerateToken(now.AddMinutes(-10));
                if (SlowByteByByteEquals(currentHash, token)) {
                    return true;
                }
            }
            return false;
        }

        private static bool SlowByteByByteEquals(string a, string b) {
            return SlowByteByByteEquals(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));
        }

        private static bool SlowByteByByteEquals(byte[] a, byte[] b) {
            uint diff = (uint) a.Length ^ (uint) b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++) {
                diff |= (uint) (a[i] ^ b[i]);
            }
            return diff == 0;
        }
    }
}