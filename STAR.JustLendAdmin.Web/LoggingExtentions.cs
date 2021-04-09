using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace STAR.JustLendAdmin.Web
{
    public static class LoggerExtensions
    {
        public static IHostBuilder ConfigureCooperLogging(this IHostBuilder builder, string applicationName)
            => builder.ConfigureLogging((ctx, logging) =>
            {
                var assembly = Assembly.GetEntryAssembly() ?? typeof(LoggerExtensions).Assembly;
                var assemblyName = assembly.GetName();
                var informationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

                var logger = (ctx.HostingEnvironment.IsDevelopment() || System.Diagnostics.Debugger.IsAttached) switch
                {
                    true => new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .Enrich.FromLogContext()
                            .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Information)
                            .WriteTo.Console()
                            .CreateLogger(),
                    _ => new LoggerConfiguration()
                            .Enrich.WithProperty("Application", applicationName)
                            .Enrich.WithProperty("AssemblyName", assemblyName.Name)
                            .Enrich.WithProperty("AssemblyVersion", assemblyName.Version?.ToString())
                            .Enrich.WithProperty("AssemblyInformationalVersion", informationalVersion?.ToString())
                            .MinimumLevel.Information()
                            .Enrich.FromLogContext()
                            .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Information)
                            .WriteTo.Console(new ElasticsearchJsonFormatter(), Serilog.Events.LogEventLevel.Verbose)
                            .CreateLogger()
                };

                logging.ClearProviders();
                Log.Logger = logger;
                logging.AddSerilog(logger);
            });
    }
}
