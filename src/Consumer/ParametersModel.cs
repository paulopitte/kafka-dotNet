namespace Kafka.Consumer.Console
{
    public class ParametersModel
    {
        public ParametersModel()
        {
            BootstrapServer = "localhost:9092";
            TopicName = "topic1";
            GroupId = "Group 1";
        }

        public string BootstrapServer { get; set; }
        public string TopicName { get; set; }
        public string GroupId { get; set; }
    }
}