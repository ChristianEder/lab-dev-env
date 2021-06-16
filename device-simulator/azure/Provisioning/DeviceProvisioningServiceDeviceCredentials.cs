using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;
using System.Threading.Tasks;

namespace IoT.Azure.DeviceSimulator.Provisioning
{
    public class DeviceProvisioningServiceDeviceCredentials : IDeviceCredentials
    {
        private string idScope;
        private string registrationId;
        private string primaryKey;

        public DeviceProvisioningServiceDeviceCredentials(string idScope, string registrationId, string primaryKey)
        {
            this.idScope = idScope;
            this.registrationId = registrationId;
            this.primaryKey = primaryKey;
        }

        public async Task<DeviceClient> Connect()
        {
            using var security = new SecurityProviderSymmetricKey(registrationId, primaryKey, null);
            using var transportHandler = new ProvisioningTransportHandlerMqtt();

            var provisioningClient = ProvisioningDeviceClient.Create(
                "global.azure-devices-provisioning.net",
                idScope,
                security,
                transportHandler);

            var provisioningResult = await provisioningClient.RegisterAsync();

            var auth = new DeviceAuthenticationWithRegistrySymmetricKey(
                provisioningResult.DeviceId,
                security.GetPrimaryKey());

            return DeviceClient.Create(provisioningResult.AssignedHub, auth);
        }

        public Task Deprovision()
        {
            // We could remove the devices enrollment in the DPS here, but we dont need to
            return Task.CompletedTask;
        }
    }
}
