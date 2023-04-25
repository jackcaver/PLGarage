using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using GameServer.Utils;
using Microsoft.EntityFrameworkCore;
#if RELEASE
using Serilog.Events;
#endif

namespace GameServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var log = new LoggerConfiguration()
#if RELEASE
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
#endif
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger = log;

            Database database = new Database();
            database.Database.Migrate();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseWebRoot("GameResources");
                });
    }
}
