using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using GameServer.Utils;
using Microsoft.EntityFrameworkCore;
using Serilog.Events;
using System.IO;

namespace GameServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var log = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#endif
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger = log;

            // Create placeholder images if they do not already exist
            if (!File.Exists("./placeholder.png") &&
                !File.Exists("./placeholder_128x128.png") &&
                !File.Exists("./placeholder_64x64.png"))
            {
                using (var fs = File.OpenWrite("./placeholder.png"))
                using (var fs128 = File.OpenWrite("./placeholder_128x128.png"))
                using (var fs64 = File.OpenWrite("./placeholder_64x64.png"))
                {
                    var data = (byte[])Properties.Resources.ResourceManager.GetObject("placeholder");
                    using (var rs = new MemoryStream(data))
                        rs.CopyTo(fs);
                    using (var rs = new MemoryStream(data))
                    using (var rs128 = UserGeneratedContentUtils.Resize(rs, 128, 128))
                        rs128.CopyTo(fs128);
                    using (var rs = new MemoryStream(data))
                    using (var rs64 = UserGeneratedContentUtils.Resize(rs, 64, 64))
                        rs64.CopyTo(fs64);
                }
            }
            if (!File.Exists("./placeholderALT.png") &&
                !File.Exists("./placeholderALT_128x128.png") &&
                !File.Exists("./placeholderALT_64x64.png"))
            {
                using (var fs = File.OpenWrite("./placeholderALT.png"))
                using (var fs128 = File.OpenWrite("./placeholderALT_128x128.png"))
                using (var fs64 = File.OpenWrite("./placeholderALT_64x64.png"))
                {
                    var data = (byte[])Properties.Resources.ResourceManager.GetObject("placeholderALT");
                    using (var rs = new MemoryStream(data))
                        rs.CopyTo(fs);
                    using (var rs = new MemoryStream(data))
                    using (var rs128 = UserGeneratedContentUtils.Resize(rs, 128, 128))
                        rs128.CopyTo(fs128);
                    using (var rs = new MemoryStream(data))
                    using (var rs64 = UserGeneratedContentUtils.Resize(rs, 64, 64))
                        rs64.CopyTo(fs64);
                }
            }

            Database database = new();
            database.Database.Migrate();
            database.Dispose();

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
