using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kafka.Consumer.Console
{
    public class ConsumerService : BackgroundService
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly ConsumerConfig _consumerConfig;
        private readonly ILogger<ConsumerService> _logger;
        private readonly ParametersModel _parameters;
        public ConsumerService(ILogger<ConsumerService> logger)
        {
            _logger = logger;
            _parameters = new ParametersModel();            
            _consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = _parameters.BootstrapServer,
                GroupId = _parameters.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Aguardando mensagens");
            _consumer.Subscribe(_parameters.TopicName);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Run(() =>
                {
                    var result = _consumer.Consume(stoppingToken);
                    var pessoa = JsonSerializer.Deserialize<PessoaModel> (result.Message.Value);
                    _logger.LogInformation($"GroupId: {_parameters.GroupId} Mensagem: {result.Message.Value}");
                    _logger.LogInformation(pessoa.ToString());
                });
            }
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _consumer.Close();
            _logger.LogInformation("Aplicação parou, conexão fechada");
            return Task.CompletedTask;
        }
    }
}