using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoT.AWS.DeviceSimulator
{
    public class SimulatedDevice
    {
        private readonly ThingCredentials thingCredentials;

        public SimulatedDevice(ThingCredentials thingCredentials)
        {
            this.thingCredentials = thingCredentials;
        }

        public async Task Simulate()
        {
            Console.WriteLine("Connecting mqtt client...");
            using (var mqttClient = await Connect())
            {
                Console.WriteLine("   ... connected");
                Console.WriteLine("Starting to simulate. Hit enter to stop...");

                var cancellationTokenSource = new CancellationTokenSource();
                var simulation = Task.Run(() => Simulate(mqttClient, cancellationTokenSource.Token));
                Console.ReadLine();
                Console.WriteLine("Stopping...");
                cancellationTokenSource.Cancel();
                await simulation;
            }
            Console.WriteLine("   ...stopped");
        }

        private async Task Simulate(IManagedMqttClient mqttClient, CancellationToken cancellationToken)
        {
            var topic = Constants.TopicForThing(thingCredentials.ThingName);

            while (!cancellationToken.IsCancellationRequested)
            {
                await mqttClient.PublishAsync(new ManagedMqttApplicationMessage
                {
                    Id = Guid.NewGuid(),
                    ApplicationMessage = new MqttApplicationMessage
                    {
                        Topic = topic,
                        Payload = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(
                            new
                            {
                                temperature = 21
                            }))
                    }
                });

                try
                {
                    await Task.Delay(5000, cancellationToken);
                }
                catch (TaskCanceledException) { }
            }
        }

        private async Task<IManagedMqttClient> Connect()
        {
            var amazonRootCertificate = new X509Certificate2(await new WebClient().DownloadDataTaskAsync("https://www.amazontrust.com/repository/AmazonRootCA1.pem"));
            var deviceCertificate = thingCredentials.Certificate(amazonRootCertificate);
            var endpoint = await thingCredentials.IoTCoreEndpoint;

            var options = new ManagedMqttClientOptionsBuilder()
                  .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId(thingCredentials.ThingName)
                    .WithCommunicationTimeout(TimeSpan.FromSeconds(40))
                    .WithTcpServer(endpoint, 8883)
                    .WithTls(new MqttClientOptionsBuilderTlsParameters()
                    {
                        UseTls = true,
                        SslProtocol = System.Security.Authentication.SslProtocols.Tls12,
                        Certificates = new X509Certificate[]
                        {
                            amazonRootCertificate,
                            deviceCertificate
                        }
                    })
                  .Build())
                .Build();

            var factory = new MqttFactory();
            var mqttClient = factory.CreateManagedMqttClient();
            var loggingHandler = new LoggingHandler();
            mqttClient.ConnectedHandler = loggingHandler;
            mqttClient.ConnectingFailedHandler = loggingHandler;
            mqttClient.DisconnectedHandler = loggingHandler;
            mqttClient.ApplicationMessageSkippedHandler = loggingHandler;
            mqttClient.ApplicationMessageProcessedHandler = loggingHandler;
            await mqttClient.StartAsync(options);
            return mqttClient;
        }
    }
}
