using Azure.Messaging.ServiceBus;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace PerfStressDriver
{
    class DevOpsServiceBusMessage
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private readonly ServiceBusReceivedMessage _message;

        public DevOpsServiceBusMessage(ServiceBusReceivedMessage message)
        {
            _message = message;
        }

        public string Body => Deserialize<string>(_message.Body.ToArray());

        public string PlanUrl => (string)_message.Properties["PlanUrl"];
        public string ProjectId => (string)_message.Properties["ProjectId"];
        public string HubName => (string)_message.Properties["HubName"];
        public string PlanId => (string)_message.Properties["PlanId"];
        public string JobId => (string)_message.Properties["JobId"];
        public string TimelineId => (string)_message.Properties["TimelineId"];
        public string TaskInstanceName => (string)_message.Properties["TaskInstanceName"];
        public string TaskInstanceId => (string)_message.Properties["TaskInstanceId"];
        public string AuthToken => (string)_message.Properties["AuthToken"];

        public async Task SignalCompletion(bool succeeded)
        {
            // https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/utility/publish-to-azure-service-bus?view=azure-devops#where-should-a-task-signal-completion
            // https://github.com/microsoft/azure-pipelines-extensions/blob/master/ServerTaskHelper/HttpRequestSampleWithoutHandler/MyApp.cs#L258

            var url = $"{PlanUrl}/{ProjectId}/_apis/distributedtask/hubs/{HubName}/plans/{PlanId}/events?api-version=2.0-preview.1";

            var body = new
            {
                name = "TaskCompleted",
                taskId = TaskInstanceId,
                jobId = JobId,
                result = succeeded ? "succeeded" : "failed",
            };

            var content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(body));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var message = new HttpRequestMessage(HttpMethod.Post, url);
            message.Headers.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(":" + AuthToken)));
            message.Content = content;

            var response = await _httpClient.SendAsync(message);

            response.EnsureSuccessStatusCode();
        }

        private static T Deserialize<T>(byte[] data)
        {
            var serializer = new DataContractSerializer(typeof(T));
            using (var stream = new MemoryStream(data))
            using (var reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max))
            {
                return (T)serializer.ReadObject(reader);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Properties");
            foreach (var p in _message.Properties)
            {
                sb.AppendLine($"  {p.Key}: {p.Value}");
            }

            sb.AppendLine();
            sb.AppendLine("Body");
            sb.AppendLine(Body);

            return sb.ToString();
        }
    }
}
