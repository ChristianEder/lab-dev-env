using Microsoft.Azure.Devices;
using System;
using System.Threading.Tasks;

namespace IoT.Azure.DeviceSimulator
{
    public static class DeviceProvisioning
    {
        public static async Task<DeviceCredentials> Provision(string deviceId)
        {
            // We use the Azure IoT Device SDK to provision the simulated device and its credentials used for authentication directly from the simulator.
            // When dealing with "real" devices, the provisioning step would typically happen during physical production of the device.
            // The RegistryManager requires you to provide a connection string associated with an access policy that has the "Registry read" and "Registry write" permissions
            if (string.IsNullOrEmpty(Constants.IoTHubConnectionString))
            {
                throw new Exception("No IoT Hub connection string was provided");
            }
            var registryManager = RegistryManager.CreateFromConnectionString(Constants.IoTHubConnectionString);

            var device = await EnsureDeviceExistsInIoTHub(registryManager, deviceId);
            return new DeviceCredentials(registryManager, device);
        }

        private static async Task<Device> EnsureDeviceExistsInIoTHub(RegistryManager registryManager, string deviceId)
        {
            var device = await registryManager.GetDeviceAsync(deviceId);
            if (device == null)
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            }

            return device;
        }
    }
}
