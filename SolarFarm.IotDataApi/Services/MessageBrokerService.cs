using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace SolarFarm.IotDataApi.Services
{
    public interface IMessageBrokerService
    {
        Task InitializeTopicsAsync();
    }

    public class KafkaMessageBrokerService : IMessageBrokerService
    {
        private readonly AdminClientConfig _adminConfig;

        public KafkaMessageBrokerService(IConfiguration configuration)
        {
            _adminConfig = new AdminClientConfig();
            configuration.Bind("AdminClientConfig", _adminConfig);
        }

        public async Task InitializeTopicsAsync()
        {
            using var adminClient = new AdminClientBuilder(_adminConfig).Build();
            Metadata metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));

            string[] topicNames = metadata.Topics.Select(t => t.Topic).ToArray();

            List<TopicSpecification> topicSpecs = new List<TopicSpecification>();

            if (!topicNames.Contains(EventConstants.SolarPanelData))
            {
                topicSpecs.Add(new TopicSpecification
                {
                    Name = EventConstants.SolarPanelData,
                    NumPartitions = 20
                });
            }

            if (topicSpecs.Count > 0)
            {
                await adminClient.CreateTopicsAsync(topicSpecs);

                Console.WriteLine($"Finish creating topics: " +
                    $"{string.Join(", ", topicSpecs.Select(spec => spec.Name))}");
            }
        }
    }
}
