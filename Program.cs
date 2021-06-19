using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ReactRedux {
    public class Program {
        public static void Main(string[] args) {
            CreateWebHostBuilder(args).Build().Run();
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) => {
                string currentPath = Directory.GetCurrentDirectory();
                config.SetBasePath(currentPath);
                string configPath = Environment.GetEnvironmentVariable("RECEIVER_CONFIG");
                config.AddJsonFile(configPath, optional : false, reloadOnChange : true);
                configPath = Environment.GetEnvironmentVariable("WEBAPP_CONFIG");
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