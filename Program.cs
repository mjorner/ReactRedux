using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            .UseStartup<Startup>()
            .UseKestrel(options => {
                options.Limits.MaxConcurrentConnections = 100;
            })
            .ConfigureLogging(factory => {
                factory.ClearProviders();
                factory.AddConsole();
                factory.AddDebug();
                //factory.AddFilter("Console", level => level >= LogLevel.Information);
                //factory.AddFilter("Debug", level => level >= LogLevel.Information);
            })
            .UseUrls("http://0.0.0.0:4000");
    }
}