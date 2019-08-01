using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Logging;
using MongoDB.Logging.Test.Services;

namespace MongoDB.Logging.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder().
                       SetBasePath(Path.Combine(AppContext.BaseDirectory)).
                       AddJsonFile("appsettings.json", optional: true).Build();

            var host = new HostBuilder()
                 .ConfigureHostConfiguration(configHost =>
                 {
                     configHost.SetBasePath(Directory.GetCurrentDirectory());
                     configHost.AddJsonFile("hostsettings.json", optional: true);
                     configHost.AddCommandLine(args);
                 })
                  .ConfigureAppConfiguration((hostContext, configApp) =>
                  {
                      configApp.AddJsonFile("appsettings.json", optional: true);
                      configApp.AddCommandLine(args);
                  })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<LogService>();
                })
                  .ConfigureLogging((hostContext, log) =>
                  {
                      var loggingSettings = config.GetSection("Logging");
                      log.AddConfiguration(loggingSettings);
                      log.AddConsole();
                      var mongoDBSettings = loggingSettings == null ? null : loggingSettings.GetSection("MongoDB");
                      log.AddMongoDB((option) =>
                      {
                          option.Connstr = mongoDBSettings == null ? "" : mongoDBSettings["Conn"];
                          option.Database = mongoDBSettings == null ? "" : mongoDBSettings["Database"];
                          option.Collection = mongoDBSettings == null ? "" : mongoDBSettings["Collection"];
                      });
                  })
                  .UseConsoleLifetime().Build();
            host.Run();
        }
    }
}
