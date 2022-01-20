using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TennisBookings.ScoreProcessor.Sqs;

namespace TennisBookings.ScoreProcessor.BackgroundServices
{
    public class ScoreProcessingService : BackgroundService
    {
        private readonly ILogger<ScoreProcessingService> _logger;
        private readonly ISqsMessageChannel _sqsMessageChannel;
        private readonly IServiceProvider _serviceProvider;
        public ScoreProcessingService(
            ILogger<ScoreProcessingService> logger,
            ISqsMessageChannel sqsMessageChannel,
            IServiceProvider serviceProvider
           )
        {
            _logger = logger;
            _sqsMessageChannel = sqsMessageChannel;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
      
        }
    }
}
