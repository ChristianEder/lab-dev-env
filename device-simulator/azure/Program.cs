using IoT.Azure.DeviceSimulator.Provisioning;
using System;
using System.Threading.Tasks;

namespace IoT.Azure.DeviceSimulator
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            IDeviceCredentials deviceCredentials;
            if (args.Length == 1)
            {
                deviceCredentials = await DeviceProvisioning.SelfProvisionInIotHub(args[0]);
            }
            else if (args.Length == 3)
            {
                deviceCredentials = await DeviceProvisioning.ProvisionUsingDeviceProvisioningService(args[0], args[1], args[2]);
            }
            else
            {
                Console.Error.WriteLine("You have to start the simulator either with the device id as the only argument, or with the id scope, registration id and primary key when using DPS.");
                return 1;
            }

            var device = new SimulatedDevice(deviceCredentials);
            await device.Simulate();

            await deviceCredentials.Deprovision();

            return 0;

        }
    }
}
