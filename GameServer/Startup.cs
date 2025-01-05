using GameServer.Implementation.Common;
using GameServer.Models.Config;
using GameServer.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Serilog;
using Serilog.Events;
using System;
using Microsoft.AspNetCore.Http;

namespace GameServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<Database>();
            if (ServerConfig.Instance.EnableRateLimiting)
                services.AddRateLimiter(options => {  // TODO: Tweak
                    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
                    {
                        var partitionKey = ctx.Connection.RemoteIpAddress.ToString();

                        return RateLimitPartition.GetConcurrencyLimiter(partitionKey: partitionKey, _ => new ConcurrencyLimiterOptions
                        {
                            PermitLimit = ServerConfig.Instance.MaxConcurrentRequests,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 2
                        });
                    });
                    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            if (env.IsDevelopment() || ServerConfig.Instance.EnableRequestLogging)
            {
                app.UseSerilogRequestLogging(options =>
                {
                    // Customize the message template
                    options.MessageTemplate = "{RemoteIpAddress} {RequestMethod} {RequestScheme}://{RequestHost}{RequestPath}{RequestQuery} responded {StatusCode} in {Elapsed:0.0000} ms";

                    // Emit debug-level events instead of the defaults
                    options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Information;

                    // Attach additional properties to the request completion event
                    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                    {
                        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                        diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
                        diagnosticContext.Set("RequestQuery", httpContext.Request.QueryString);
                    };
                });
            }

            app.UseRouting();
            app.UseWebSockets();

            if (ServerConfig.Instance.EnableRateLimiting)
                app.UseRateLimiter();

            app.UseStaticFiles(new StaticFileOptions {
                RequestPath = "/resources"
            });

            app.UseEndpoints(endpoints => endpoints.MapControllers());

            appLifetime.ApplicationStopping.Register(() =>
            {
                SessionImpl.DestroyAllSessions();
                ServerCommunicationImpl.DisconnectAllServers("ServerStopping").Wait();
            });
        }
    }
}
