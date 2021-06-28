using System;
using System.Text;
using System.Security.Cryptography;

namespace ReactRedux.Crypto {
    internal sealed class NowTokenManager : INowTokenManager {
        private readonly AppConfiguration Configuration;
        public NowTokenManager(AppConfiguration configuration) {
            Configuration = configuration;
        }

        public string GenerateToken() {
            return GenerateToken(DateTime.UtcNow, Configuration.AuthSalt);
        }
        public bool ValidateToken(string token) {
            return CompareToken(token, Configuration.AuthSalt);
        }

        private static string GenerateToken(DateTime now, string salt) {
            using(var sha = SHA256.Create()) {
                byte[] bytes = Encoding.UTF8.GetBytes($"{salt}{now.Year}{now.Month}{now.Day}{now.Hour}{now.Minute}");
                bytes = sha.ComputeHash(bytes);
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private static bool CompareToken(string token, string salt) {
            DateTime now = DateTime.UtcNow;
            string currentHash = GenerateToken(now, salt);
            if (SlowByteByByteEquals(currentHash, token)) {
                return true;
            }
            currentHash = GenerateToken(now.AddMinutes(-1), salt);
            return SlowByteByByteEquals(currentHash, token);
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