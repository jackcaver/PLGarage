using GameServer.Implementation.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Utils
{
    public class DailyTickService(ILogger<DailyTickService> logger) : IHostedService, IDisposable
    {
        private readonly ILogger<DailyTickService> Logger = logger;
        private Timer Timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogDebug("DailyTickService started");

            var untilNewDay = TimeUtils.DayStart.AddDays(1) - TimeUtils.Now;

            Timer = new(Tick, null, untilNewDay, TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        private void Tick(object state)
        {
            Logger.LogDebug("DailyTickService tick");

            try
            {
                var database = new Database();
                ContentUpdates.GetNewHotLap(database);
                database.Dispose();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "There was an error trying to process daily tick:");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogDebug("DailyTickService stopped");

            Timer.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Timer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
