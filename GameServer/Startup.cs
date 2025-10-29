using GameServer.Implementation.Common;
using GameServer.Models.Config;
using GameServer.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = JWTUtils.SigningKey,
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Cookies.TryGetValue("Token", out string token))
                                context.Token = token;
                            return Task.CompletedTask;
                        }
                    };
                });
            services.AddAuthorizationBuilder()
                .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireClaim(JWTUtils.Role, JWTUtils.RoleUser)
                    .Build())
                .AddPolicy(JWTUtils.ModeratorPolicy, new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireClaim(JWTUtils.Role, JWTUtils.RoleModerator)
                    .Build());
            services.AddDbContext<Database>();
            services.AddHostedService<DailyTickService>();
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

            app.UseAuthentication();
            app.UseRouting();
            app.UseWebSockets();
            app.UseAuthorization();

            if (ServerConfig.Instance.EnableRateLimiting)
                app.UseRateLimiter();

            app.UseStaticFiles(new StaticFileOptions {
                RequestPath = "/resources"
            });

            app.UseEndpoints(endpoints => endpoints.MapControllers());

            appLifetime.ApplicationStopping.Register(() =>
            {
                Session.DestroyAllSessions();
                ServerCommunication.DisconnectAllServers("ServerStopping").Wait();
            });
        }
    }
}
