using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Threading.Tasks;

namespace IoT.AWS.DeviceSimulator
{
    public class LoggingHandler : IConnectingFailedHandler, IMqttClientConnectedHandler, IMqttClientDisconnectedHandler, IApplicationMessageSkippedHandler, IApplicationMessageProcessedHandler
    {
        public Task HandleApplicationMessageProcessedAsync(ApplicationMessageProcessedEventArgs eventArgs)
        {
            Console.WriteLine("Message sent: " + (eventArgs.HasFailed ? "FAILED" : eventArgs.HasSucceeded ? "SUCCEEDED" : "UNKNOWN"));
            return Task.CompletedTask;
        }

        public Task HandleApplicationMessageSkippedAsync(ApplicationMessageSkippedEventArgs eventArgs)
        {
            Console.WriteLine("Message skipped");
            return Task.CompletedTask;
        }

        public Task HandleConnectedAsync(MqttClientConnectedEventArgs eventArgs)
        {
            Console.WriteLine("Connected");
            return Task.CompletedTask;
        }

        public Task HandleConnectingFailedAsync(ManagedProcessFailedEventArgs eventArgs)
        {
            Console.Error.WriteLine("Failed to connect");
            return Task.CompletedTask;
        }

        public Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs eventArgs)
        {
            Console.WriteLine("Disconnected");
            return Task.CompletedTask;
        }
    }
}
