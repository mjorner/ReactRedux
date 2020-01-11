using System.Text;
using System.Security.Cryptography;

namespace ReactRedux.Crypto {
    public class CachedSha256HashValidator : Argon2CridentialsValidator {
        private static byte[] ValidatedSha256AuthHeaderBytes = null;
        private readonly int ShaSaltBytesLength = 16;
        private byte[] ShaSaltBytes {
            get {
                lock(SaltLock) {
                    if (vShaSaltBytes == null) {
                        using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
                            vShaSaltBytes = new byte[ShaSaltBytesLength];
                            rng.GetBytes(vShaSaltBytes);
                        }
                    }
                }
                return vShaSaltBytes;
            }
        }
        private static byte[] vShaSaltBytes = null;
        private static readonly object SaltLock = new object();

        public CachedSha256HashValidator(AppConfiguration configuration) : base(configuration) {
            ShaSaltBytesLength = configuration.ShaRandomSaltLength;
        }

        public override bool Verify(string authHeader) {
            if (ValidatedSha256AuthHeaderBytes != null) {
                bool shaVerified = ByteByByteEquals(ComputeSha256Bytes(authHeader), ValidatedSha256AuthHeaderBytes);
                return shaVerified;
            }

            bool verified = base.Verify(authHeader);
            if (verified) {
                ValidatedSha256AuthHeaderBytes = ComputeSha256Bytes(authHeader);
            }
            return verified;
        }

        private static bool ByteByByteEquals(byte[] a, byte[] b) {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++) {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }

        private byte[] ComputeSha256Bytes(string input) {
            using (var sha = SHA256.Create()) {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                bytes = AddSalt(bytes);
                int iteration = 0;
                while (iteration++ < base.Configuration.ShaIterations) {
                    bytes = sha.ComputeHash(bytes);
                }
                return bytes;
            }
        }

        private byte[] AddSalt(byte[] bytes) {
            byte[] newSaltedBytes = new byte[bytes.Length+ShaSaltBytesLength];
            for(int i = 0; i < ShaSaltBytesLength; i++) {
                newSaltedBytes[i] = ShaSaltBytes[i];
            }
            for(int i = 0; i < bytes.Length; i++) {
                newSaltedBytes[ShaSaltBytesLength+i] = bytes[i];
            }
            return newSaltedBytes;
        }
    }
}