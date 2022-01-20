using System;
using Amazon.S3;
using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TennisBookings.ResultsProcessing;
using TennisBookings.ScoreProcessor.BackgroundServices;
using TennisBookings.ScoreProcessor.Processing;
using TennisBookings.ScoreProcessor.S3;
using TennisBookings.ScoreProcessor.Sqs;

namespace TennisBookings.ScoreProcessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)                
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<AwsServicesConfiguration>(hostContext.Configuration.GetSection("AWS"));

                    services.AddAWSService<IAmazonSQS>();

                    var useLocalStack = hostContext.Configuration.GetValue<bool>("UseLocalStack");

                    if (hostContext.HostingEnvironment.IsDevelopment() && useLocalStack)
                    {
                        services.AddSingleton<IAmazonSQS>(sp =>
                        {
                            var s3Client = new AmazonSQSClient(new AmazonSQSConfig
                            {
                                ServiceURL = "http://localhost:4576"
                            });

                            return s3Client;
                        });
                    }

                    services.AddSingleton<ISqsMessageChannel, SqsMessageChannel>();
                    services.AddSingleton<ISqsMessageDeleter, SqsMessageDeleter>();
                    services.AddSingleton<ISqsMessageQueue, SqsMessageQueue>();
                });
    }
}
