using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ReactRedux {
    public class Program {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) => {
                string currentPath = Directory.GetCurrentDirectory();
                config.SetBasePath(currentPath);
                string configPath = Environment.GetEnvironmentVariable("RECEIVER_CONFIG");
                config.AddJsonFile(configPath, optional : false, reloadOnChange : true);
                configPath = Environment.GetEnvironmentVariable("WEBAPP_CONFIG");
                config.AddJsonFile(configPath, optional : false, reloadOnChange : true);
            })
            .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel(options => {
                        options.Limits.MaxConcurrentConnections = 100;
                    });
                    webBuilder.UseUrls("http://0.0.0.0:4000");
                }).ConfigureLogging(factory => {
                    factory.ClearProviders();
                    factory.AddConsole();
                    factory.AddDebug();
                });
    }
}