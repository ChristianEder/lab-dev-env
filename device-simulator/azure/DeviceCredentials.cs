using Microsoft.Azure.Devices;
using System;
using System.Threading.Tasks;
using static IoT.Azure.DeviceSimulator.DeviceProvisioning;

namespace IoT.Azure.DeviceSimulator
{
    public class DeviceCredentials
    {
        private readonly RegistryManager registryManager;
        private readonly Device device;

        public DeviceCredentials(RegistryManager registryManager, Device device)
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

        public string ConnectionString => $"HostName={IotHubConnectionStringBuilder.Create(Constants.IoTHubConnectionString).HostName};DeviceId={device.Id};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}";
    }
}
