using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ReactRedux {
    public sealed class AuthenticationMiddleware {
        private readonly RequestDelegate Next;
        private readonly AppConfiguration Configuration;

        public AuthenticationMiddleware(RequestDelegate next, AppConfiguration configuration) {
            Next = next;
            Configuration = configuration;
        }
        public async Task InvokeAsync(HttpContext context) {
            if (context.Request.Path != null && context.Request.Path.Value.Contains(".stat")) {
                await Next(context);
            }
            if (Configuration.Pw.Length == 0 && Configuration.Uname.Length == 0) {
                await Next(context);
            }
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic")) {
                string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                string[] up = usernamePassword.Split(':');

                if (up[0] == Configuration.Uname && up[1] == Configuration.Pw) {
                    await Next(context);
                } else {
                    context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Secure Area\"";
                    context.Response.StatusCode = 401; //Unauthorized
                    return;
                }
            } else {
                // no authorization header
                context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Secure Area\"";
                context.Response.StatusCode = 401; //Unauthorized
                return;
            }
        }
    }
}