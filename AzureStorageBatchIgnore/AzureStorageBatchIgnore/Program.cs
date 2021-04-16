using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureStorageBatchIgnore
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var host = BuildHost(args).Build();
            await host.RunAsync();
        }

        private static IHostBuilder BuildHost(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((builder) =>
                {
                    // Setup Web Job Specific Secrets to be setup as in memory objects so Web Jobs SDK can pick them up
                    var azureWebJobsSecrets = new Dictionary<string, string>();
                    var storageConnectionString = "UseDevelopmentStorage=true";
                    azureWebJobsSecrets["AzureWebJobsStorage"] = storageConnectionString;
                    azureWebJobsSecrets["AzureWebJobsDashboard"] = storageConnectionString;
                    azureWebJobsSecrets["Storage"] = storageConnectionString;

                    // Add in Memory Collection to allow for Web Jobs SDK to pick up required configs
                    builder.AddInMemoryCollection(azureWebJobsSecrets);
                })
                .ConfigureWebJobs((b) =>
                {
                    b.AddAzureStorageCoreServices();
                    b.AddAzureStorageQueues();
                    b.AddBuiltInBindings();
                })
                .ConfigureLogging((loggingBuilder) =>
                {
                    loggingBuilder.ClearProviders();
                })
                .ConfigureServices((builderContext, services) =>
                {
                    services.AddTransient<IQueueProcessorFactory, CustomSettingsQueueProcessorFactory>();
                    services.AddTransient<Functions>();
                })
                .UseConsoleLifetime();
        }

        public class CustomSettingsQueueProcessorFactory : IQueueProcessorFactory
        {
            /// <inheritdoc/>
            public QueueProcessor Create(QueueProcessorOptions queueProcessorOptions)
            {
                queueProcessorOptions.Options.BatchSize = 4;
                return new CustomQueueProcess(queueProcessorOptions);
            }
        }
        internal class CustomQueueProcess : QueueProcessor
        {
            public CustomQueueProcess(QueueProcessorOptions options)
            : base(options)
            {
            }
        }
    }
}
