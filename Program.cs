using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

//npm audit fix
//setx ASPNETCORE_ENVIRONMENT "Development" windows
//export ASPNETCORE_ENVIRONMENT="Development" Linux
//dotnet publish -c Release -r linux-arm

//TODO: use IOptionsMonitor for monitoring changes to configuration.
namespace ReactRedux {
    public class Program {
        public static void Main(string[] args) {
            // config json files should be in args 
            CreateWebHostBuilder(args).Build().Run();
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) => {
                string currentPath = Directory.GetCurrentDirectory();
                config.SetBasePath(currentPath);
                string configPath = $"{Directory.GetParent(currentPath)}{Path.DirectorySeparatorChar}webappconfig.json";
                config.AddJsonFile(configPath, optional : false, reloadOnChange : true);
                configPath = $"{Directory.GetParent(currentPath)}{Path.DirectorySeparatorChar}webauth.json";
                config.AddJsonFile(configPath, optional : false, reloadOnChange : true);
            })
            .UseStartup<Startup>()
            .UseKestrel(options => {
                options.Limits.MaxConcurrentConnections = 100;
            })
            .ConfigureLogging(factory => {
                factory.ClearProviders();
                factory.AddConsole();
                factory.AddDebug();
            })
            .UseUrls("http://0.0.0.0:4000");
    }
}