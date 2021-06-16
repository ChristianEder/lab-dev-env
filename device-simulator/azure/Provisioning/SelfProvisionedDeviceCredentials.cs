using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using System;
using System.Threading.Tasks;

namespace IoT.Azure.DeviceSimulator.Provisioning
{
    public class SelfProvisionedDeviceCredentials : IDeviceCredentials
    {
        private readonly RegistryManager registryManager;
        private readonly Device device;

        public SelfProvisionedDeviceCredentials(RegistryManager registryManager, Device device)
        {
            this.registryManager = registryManager;
            this.device = device;
        }

        public async Task Deprovision()
        {
            Console.WriteLine("Do you want to deprovision the simulated device \"" + device.Id + "\"? Type [y|Y]es if you want to deprovision");
            if (Console.ReadLine().ToLowerInvariant().StartsWith("y"))
            {
                await registryManager.RemoveDeviceAsync(device);
            }
        }

        public Task<DeviceClient> Connect()
        {
            var deviceConnectionString = $"HostName={Microsoft.Azure.Devices.IotHubConnectionStringBuilder.Create(Constants.IoTHubConnectionString).HostName};DeviceId={device.Id};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}";
            var client = DeviceClient.CreateFromConnectionString(deviceConnectionString);
            return Task.FromResult(client);
        }
    }
}
