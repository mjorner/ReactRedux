using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ReactRedux {
    public sealed class AuthenticationMiddleware {
        private readonly RequestDelegate Next;
        private readonly AppConfiguration Configuration;
        private readonly ILogger<AuthenticationMiddleware> Logger;
        private byte[] ValidatedSha256AuthHeaderBytes = null;
        private readonly int ShaSaltBytesLength = 16;

        private byte[] ShaSaltBytes {
            get {
                if (vShaSaltBytes == null) {
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
                        vShaSaltBytes = new byte[ShaSaltBytesLength];
                        rng.GetBytes(vShaSaltBytes);
                    }
                }
                return vShaSaltBytes;
            }
        }
        private byte[] vShaSaltBytes = null;

        public AuthenticationMiddleware(RequestDelegate next, AppConfiguration configuration, ILogger<AuthenticationMiddleware> logger) {
            Next = next;
            Configuration = configuration;
            ShaSaltBytesLength = Configuration.ShaRandomSaltLength;
            Logger = logger;
        }

        public async Task InvokeAsync(HttpContext context) {
            if (!Configuration.AuthToken.Any()) {
                await Next(context);
                return;
            }
            if (CanContinueWithoutAuth(context.Request.Path)) {
                await Next(context);
                return;
            }
            
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic")) {
                if (VerifyArgon2Hash(authHeader)) {
                    await Next(context);
                    return;
                } else {
                    context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Secure Area\"";
                    context.Response.StatusCode = 401; //Unauthorized 
                    Logger.LogWarning("Unauth wrong basic:" + GetAuthLogStr(authHeader, context.Request.Path));  
                    return; 
                }
            } else {
                context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Secure Area\"";
                context.Response.StatusCode = 401; //Unauthorized
                Logger.LogWarning("Unauth No basic found:" + GetAuthLogStr(authHeader, context.Request.Path));
                return;
            }
        }

        private static string GetAuthLogStr(string authHeader, PathString path) {
            string s = $"AuthHeader: \"{authHeader}\", path: \"{(path.HasValue?path.Value:"no value")}\"";
            return s;
        }

        private static bool CanContinueWithoutAuth(PathString path) {
            if (path == null) { return true; }
            if (!path.HasValue) { return true; }
            return !path.Value.Contains("api");
        }

        private bool VerifyArgon2Hash(string authHeader) {
            if (ValidateAlreadyValidatedAuthHeader(authHeader)) { return true; }
            string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));      
            Argon2Config config = GetArgon2Config(usernamePassword);
            bool verified = Argon2.Verify(Configuration.AuthToken, config);
            if (verified) {
                ValidatedSha256AuthHeaderBytes = ComputeSha256Bytes(authHeader);
            }
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

        private bool ValidateAlreadyValidatedAuthHeader(string authHeader) {
            if (ValidatedSha256AuthHeaderBytes == null) {
                return false;
            }
            bool validated = ByteByByteEquals(ComputeSha256Bytes(authHeader), ValidatedSha256AuthHeaderBytes);
            return validated;
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
                while (iteration++ < Configuration.ShaIterations) {
                    bytes = sha.ComputeHash(bytes);
                }
                return bytes;
            }
        }

        private byte[] AddSalt(byte[] bytes) {
            byte[] newBytes = new byte[bytes.Length+ShaSaltBytesLength];
            for(int i = 0; i < ShaSaltBytesLength; i++) {
                newBytes[i] = ShaSaltBytes[i];
            }
            for(int i = 0; i < bytes.Length; i++) {
                newBytes[ShaSaltBytesLength+i] = bytes[i];
            }
            return newBytes;
        }
    }
}