using System.Text.Json;
using Confluent.Kafka;

namespace Kafka.Producer.API;
public class ProducerService
{
    private const string CRITICAL_MESSAGE = "FALHA CRITICA AO ENVIAR MENSAGEM";
    private readonly IConfiguration _configuration;
    private readonly ProducerConfig _producerConfig;
    private readonly ILogger<ProducerService> _logger;
    public ProducerService(IConfiguration configuration, ILogger<ProducerService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        var bootstrap = _configuration.GetSection("KafkaConfig").GetSection("BootstrapServer").Value;

        _producerConfig = new ProducerConfig()
        {
            BootstrapServers = bootstrap
        };
    }

    public async Task<string> SendMessage(PessoaModel pessoa)
    {
        var topic = _configuration.GetSection("KafkaConfig").GetSection("TopicName").Value;
        try
        {
            if (string.IsNullOrEmpty(topic))
            {
                _logger.LogCritical($"{CRITICAL_MESSAGE} - TOPICO NAO ENCONTRADO");
                return ($"{CRITICAL_MESSAGE} - TOPICO NAO ENCONTRADO");
            }

            using var producer = new ProducerBuilder<Null, string>(_producerConfig).Build();
            var message = JsonSerializer.Serialize(pessoa);
            var result = await producer.ProduceAsync(topic: topic, new() { Value = message });
            _logger.LogInformation(result.Status.ToString() + " - " + message);

            return result.Status.ToString() + " - " + message;
        }
        catch
        {
            _logger.LogCritical(CRITICAL_MESSAGE);
            return CRITICAL_MESSAGE;
        }
    }
}
