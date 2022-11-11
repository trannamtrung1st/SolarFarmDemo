using Confluent.Kafka;
using Newtonsoft.Json;
using SolarFarm.Entities;

namespace SolarFarm.IotDataHandler
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ConsumerConfig _baseConfig;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceProvider = serviceProvider;

            _baseConfig = new ConsumerConfig();
            _configuration.Bind("ConsumerConfig", _baseConfig);
        }

        private void StartConsumerThread(int idx, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Consumer {idx} starting");

            Thread thread = new Thread(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await ConsumeAsync(idx, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex);
                        await Task.Delay(7000);
                    }
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        private async Task ConsumeAsync(int idx, CancellationToken cancellationToken)
        {
            var config = new Dictionary<string, string>();

            var batchSize = _configuration.GetValue<int>("ProcessingBatchSize");

            using (IConsumer<string, string> consumer
                = new ConsumerBuilder<string, string>(_baseConfig).Build())
            {
                try
                {
                    consumer.Subscribe(EventConstants.SolarPanelData);

                    List<ConsumeResult<string, string>> batch = new List<ConsumeResult<string, string>>();
                    bool isTimeout = false;

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        ConsumeResult<string, string> message = consumer.Consume(100);

                        if (message != null)
                        {
                            Console.WriteLine($"Consumer {idx} is handling message {message.Message.Key} - {message.Message.Timestamp.UtcDateTime}");

                            batch.Add(message);
                        }
                        else
                        {
                            isTimeout = true;
                        }

                        if (batch.Count > 0 && (batch.Count >= batchSize || isTimeout))
                        {
                            Console.WriteLine($"Consumer {idx} begins processing batch of {batch.Count} messages");

                            List<SolarPanelData> dataRecords = batch
                                .Select(m => JsonConvert.DeserializeObject<SolarPanelData>(m.Message.Value))
                                .ToList();

                            using (IServiceScope scope = _serviceProvider.CreateScope())
                            {
                                var dbContext = scope.ServiceProvider.GetRequiredService<IotDataContext>();

                                await dbContext.AddRangeAsync(dataRecords);

                                await dbContext.SaveChangesAsync(cancellationToken);

                                Console.WriteLine($"Finish saving interaction batch of {dataRecords.Count}");
                            }

                            batch.Clear();
                            isTimeout = false;
                        }
                    }
                }
                finally
                {
                    consumer.Close();
                }
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumerCount = _configuration.GetValue<int>("ConsumerCount");

            for (int i = 0; i < consumerCount; i++)
            {
                StartConsumerThread(i, stoppingToken);
            }

            return Task.CompletedTask;
        }
    }
}