using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MongoDB.Logging.Test.Services
{
    public class LogService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IApplicationLifetime _appLifetime;
        private CancellationTokenSource _tokenKeep = new CancellationTokenSource();

        public LogService(
            ILogger<LogService> logger,
            IApplicationLifetime appLifetime)
        {
            _logger = logger;
            _appLifetime = appLifetime;
        }
        private void OnStarted()
        {
            _logger.LogWarning("OnStarted");
        }

        private void OnStopping()
        {
            _logger.LogWarning("OnStopping");
            _tokenKeep.Cancel();
        }

        private void OnStopped()
        {
            _logger.LogWarning("OnStopped");

        }
        private void log()
        {
            while (_tokenKeep.IsCancellationRequested == false)
            {
                _logger.LogWarning("Test mongodb logging.");
                Thread.Sleep(100);
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);
            Task.Factory.StartNew(log, _tokenKeep.Token);
            return Task.CompletedTask;
        }
    }
}
