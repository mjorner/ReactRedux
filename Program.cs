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
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appconfig.json", optional: false, reloadOnChange: true);
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