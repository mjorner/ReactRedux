using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ReactRedux.Crypto;

namespace ReactRedux {
    public sealed class AuthenticationMiddleware {
        private readonly RequestDelegate Next;
        private readonly AppConfiguration Configuration;
        private readonly ILogger<AuthenticationMiddleware> Logger;
        private readonly ICridentialsValidator CridentialsValidator;

        public AuthenticationMiddleware(RequestDelegate next, AppConfiguration configuration, ILogger<AuthenticationMiddleware> logger, ICridentialsValidator cridentialsValidator) {
            Next = next;
            Configuration = configuration;
            Logger = logger;
            CridentialsValidator = cridentialsValidator;
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
                if (CridentialsValidator.Verify(authHeader)) {
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
    }
}