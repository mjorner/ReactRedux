using System.Text;
using Isopoh.Cryptography.Argon2;

namespace ReactRedux.Crypto {
    public class Argon2CridentialsValidator : ICridentialsValidator {
        protected readonly AppConfiguration Configuration;

        public Argon2CridentialsValidator(AppConfiguration configuration) {
            Configuration = configuration;
        }

        public bool Verify(string usernamePassword) {
            Argon2Config config = GetArgon2Config(usernamePassword);
            bool verified = Argon2.Verify(Configuration.AuthToken, config);
            return verified;
        }

        private Argon2Config GetArgon2Config(string usernamePassword) {
            byte[] saltBytes = Encoding.UTF8.GetBytes(Configuration.AuthSalt);
            var config = new Argon2Config();
            config.Password = Encoding.UTF8.GetBytes(usernamePassword);
            config.SecureArrayCall = null;
            config.Secret = saltBytes;
            config.Salt = saltBytes;
            config.MemoryCost = 10000;
            return config;
        }
    }
}