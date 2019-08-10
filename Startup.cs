using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using ReactRedux.Utilities;

namespace ReactRedux {
    public class Startup {
        private readonly ILoggerFactory LoggerFactory;
        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory) {
            Configuration = configuration;
            LoggerFactory = loggerFactory;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => {
                configuration.RootPath = "ClientApp/build";
            });

            AppConfiguration appConfiguration = new AppConfiguration(Configuration);
            appConfiguration.Validate();

            services.AddSingleton<AppConfiguration>(appConfiguration);
            services.AddTransient<IFileReader, FileReader>();
            services.AddTransient<IStringCompressor, SnappyStringCompressor>();
            ILogger<BlockingFileReadContainerPool> poolLogger = LoggerFactory.CreateLogger<BlockingFileReadContainerPool>();
            services.AddSingleton<IFileReadContainerPool>(new BlockingFileReadContainerPool(appConfiguration.GraphConcurrencyCount, appConfiguration.GraphLineCount, appConfiguration.GraphLineLength, poolLogger));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            if (!env.IsDevelopment()) {
                app.UseMiddleware<AuthenticationMiddleware>();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            AppConfiguration appConfiguration = new AppConfiguration(Configuration);
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".stat"] = "text/plain";
            app.UseStaticFiles(new StaticFileOptions {
                FileProvider = new PhysicalFileProvider(appConfiguration.DataPath),
                    RequestPath = "",
                    ContentTypeProvider = provider
            });

            if (appConfiguration.SnapShotPath.Length != 0) {
                provider = new FileExtensionContentTypeProvider();
                provider.Mappings[".jpg"] = "image/jpg";
                app.UseStaticFiles(new StaticFileOptions {
                    FileProvider = new PhysicalFileProvider(appConfiguration.SnapShotPath),
                        RequestPath = "/images",
                        ContentTypeProvider = provider
                });
            }

            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa => {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment()) {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}