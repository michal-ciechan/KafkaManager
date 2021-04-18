using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Polly;
using Polly.Retry;
using Xunit;
using Xunit.Abstractions;

namespace KafkaManager.Services.Tests
{
    public class UnitTest1
    {
        private RetryPolicy _retryPolicy;
        
        private readonly ITestOutputHelper _testOutputHelper;

        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            static TimeSpan Duration(int i)
            {
                return TimeSpan.FromMilliseconds(Math.Pow(2, i));
            }

            void OnRetry(Exception exception, TimeSpan timeSpan)
            {
                _testOutputHelper.WriteLine("Retrying");
            }

            _retryPolicy = Policy
                .Handle<KafkaException>()
                .WaitAndRetry(10, Duration, OnRetry);
        }

        [Fact]
        public async Task PublishTest()
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092", 
                MessageTimeoutMs = 5000,
                RequestTimeoutMs = 5000,
                SocketTimeoutMs = 5000
            };

            // If serializers are not specified, default serializers from
            // `Confluent.Kafka.Serializers` will be automatically used where
            // available. Note: by default strings are encoded as UTF8.
            using var p = new ProducerBuilder<Null, string>(config).Build();
            
            var dr = await p.ProduceAsync("test-topic", new Message<Null, string> { Value="test" });
                
            _testOutputHelper.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
            
            // catch (ProduceException<Null, string> e)
        }

        [Fact]
        public void ConsumeTest()
        {
            var config = new ConsumerConfig
            {
                GroupId = "test-consumer-group",
                BootstrapServers = "localhost:9092", 
                SocketTimeoutMs = 5000,
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };

            // If serializers are not specified, default serializers from
            // `Confluent.Kafka.Serializers` will be automatically used where
            // available. Note: by default strings are encoded as UTF8.
            using var p = new ConsumerBuilder<Null, string>(config).Build();
            
            p.Subscribe("test-topic");

            var sw = Stopwatch.StartNew();

            while (sw.Elapsed < TimeSpan.FromSeconds(3))
            {
                var dr = p.Consume(TimeSpan.FromSeconds(1));

                if (dr == null)
                {
                    _testOutputHelper.WriteLine("ConsumeResult is null");
                    continue;
                }

                _testOutputHelper.WriteLine($"Consumer message '{dr.Message?.Value}' at '{dr.TopicPartitionOffset}'");
            }
        }

        [Fact]
        public void ListTopicsTest()
        {
            var config = new AdminClientConfig()
            {
                BootstrapServers = "localhost:9092", 
                SocketTimeoutMs = 5000,
            };

            // If serializers are not specified, default serializers from
            // `Confluent.Kafka.Serializers` will be automatically used where
            // available. Note: by default strings are encoded as UTF8.
            using var client = new AdminClientBuilder(config).Build();

            var metadata = client.GetMetadata(TimeSpan.FromSeconds(5));

            var topics = metadata.Topics.Select(x => x.Topic).OrderBy(x => x).ToList();

            foreach (var topic in topics)
                _testOutputHelper.WriteLine(topic);
        }

        [Fact]
        public void TestTopicPartitions()
        {
            var config = new AdminClientConfig()
            {
                BootstrapServers = "localhost:9092", 
                SocketTimeoutMs = 5000,
            };

            // If serializers are not specified, default serializers from
            // `Confluent.Kafka.Serializers` will be automatically used where
            // available. Note: by default strings are encoded as UTF8.
            using var client = new AdminClientBuilder(config).Build();

            var metadata = client.GetMetadata("test-topic", TimeSpan.FromSeconds(5));

            var partitionIds = metadata.Topics[0].Partitions.Select(x => x.PartitionId).ToList();
            
            foreach (var partitionId in partitionIds)
                _testOutputHelper.WriteLine(partitionId.ToString());
        }

        [Fact]
        public void ListTestTopicOffsets()
        {
            var groups = _retryPolicy.Execute(() =>
            {
                // If serializers are not specified, default serializers from
                // `Confluent.Kafka.Serializers` will be automatically used where
                // available. Note: by default strings are encoded as UTF8.
                using var admin = new AdminClientBuilder(new AdminClientConfig
                {
                    BootstrapServers = "localhost:9092",
                    SocketTimeoutMs = 5000,
                }).Build();

                return admin.ListGroups(TimeSpan.FromSeconds(15));
            });

            foreach (var groupInfo in groups)
            {
                using var consumer = new ConsumerBuilder<Null, string>(new ConsumerConfig
                {
                    GroupId = groupInfo.Group,
                    BootstrapServers = "localhost:9092", 
                    SocketTimeoutMs = 5000,
                }).Build();

                var committed = consumer.Committed(new[]
                {
                    new TopicPartition("test-topic", new Partition(0))
                }, TimeSpan.FromSeconds(5));

                foreach (var commit in committed)
                {
                    _testOutputHelper.WriteLine($"{groupInfo.Group} - {commit}");
                }
            }
        }
    }
}