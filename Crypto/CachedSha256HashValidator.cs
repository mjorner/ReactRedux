using System.Security.Cryptography;
using System.Text;

namespace ReactRedux.Crypto {
    public class CachedSha256HashValidator : Argon2CridentialsValidator {
        private static byte[] ValidatedSha256AuthHeaderBytes = null;
        private const int ShaSaltBytesLength = 16;
        private static readonly byte[] ShaSaltBytes;

        static CachedSha256HashValidator() {
            using(RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
                ShaSaltBytes = new byte[ShaSaltBytesLength];
                rng.GetBytes(ShaSaltBytes);
            }
        }

        public CachedSha256HashValidator(AppConfiguration configuration) : base(configuration) { }

        public override bool Verify(string authHeader) {
            if (VerifyIfCached(authHeader)) { return true; }

            bool verified = base.Verify(authHeader);
            if (verified) {
                ValidatedSha256AuthHeaderBytes = ComputeSha256Bytes(authHeader);
            }
            return verified;
        }

        private bool VerifyIfCached(string authHeader) {
            if (ValidatedSha256AuthHeaderBytes == null) { return false; }
            bool verified = SlowByteByByteEquals(ComputeSha256Bytes(authHeader), ValidatedSha256AuthHeaderBytes);
            return verified;
        }

        private static bool SlowByteByByteEquals(byte[] a, byte[] b) {
            uint diff = (uint) a.Length ^ (uint) b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++) {
                diff |= (uint) (a[i] ^ b[i]);
            }
            return diff == 0;
        }

        private byte[] ComputeSha256Bytes(string input) {
            using(var sha = SHA256.Create()) {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                bytes = AddSaltBytes(bytes);
                int iteration = 0;
                while (iteration++ < base.Configuration.ShaIterations) {
                    bytes = sha.ComputeHash(bytes);
                }
                return bytes;
            }
        }

        private byte[] AddSaltBytes(byte[] bytes) {
            byte[] newSaltedBytes = new byte[bytes.Length + ShaSaltBytesLength];
            for (int i = 0; i < ShaSaltBytesLength; i++) {
                newSaltedBytes[i] = ShaSaltBytes[i];
            }
            for (int i = 0; i < bytes.Length; i++) {
                newSaltedBytes[ShaSaltBytesLength + i] = bytes[i];
            }
            return newSaltedBytes;
        }
    }
}