using Microsoft.Azure.Devices.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoT.Azure.DeviceSimulator
{
    public class SimulatedDevice
    {
        private readonly DeviceCredentials deviceCredentials;

        public SimulatedDevice(DeviceCredentials deviceCredentials)
        {
            this.deviceCredentials = deviceCredentials;
        }

        public async Task Simulate()
        {
            Console.WriteLine("Connecting device client...");
            using (var deviceClient = DeviceClient.CreateFromConnectionString(deviceCredentials.ConnectionString))
            {
                Console.WriteLine("   ... connected");
                Console.WriteLine("Starting to simulate. Hit enter to stop...");

                var cancellationTokenSource = new CancellationTokenSource();
                var simulation = Task.Run(() => Simulate(deviceClient, cancellationTokenSource.Token));
                Console.ReadLine();
                Console.WriteLine("Stopping...");
                cancellationTokenSource.Cancel();
                await simulation;
            }
            Console.WriteLine("   ...stopped");
        }

        private async Task Simulate(DeviceClient deviceClient, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {                
                await deviceClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(
                            new
                            {
                                temperature = 21
                            }))));

                Console.WriteLine("Message sent");

                try
                {
                    await Task.Delay(5000, cancellationToken);
                }
                catch (TaskCanceledException) { }
            }
        }
    }
}
