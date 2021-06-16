using System;
using System.Threading.Tasks;

namespace IoT.Azure.DeviceSimulator
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("You have to start the simulator with the device id as the only argument");
                return 1;
            }

            var deviceCredentials = await DeviceProvisioning.Provision(args[0]);

            var device = new SimulatedDevice(deviceCredentials);
            await device.Simulate();

            await deviceCredentials.Deprovision();

            return 0;

        }
    }
}
