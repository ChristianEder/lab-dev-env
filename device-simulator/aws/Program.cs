using System;
using System.Threading.Tasks;

namespace IoT.AWS.DeviceSimulator
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // *******************************
            // *****   ATTENTION *************
            // *******************************
            // when running this for the first time, you will be asked to change the values provided in Constants.cs
            Constants.FailUntilConstantsHaveBeenCustomized();

            if(args.Length != 1)
            {
                Console.Error.WriteLine("You have to start the simulator with the thing name as the only argument");
                return 1;
            }

            var thingCredentials = await DeviceProvisioning.Provision(args[0]);

            var device = new SimulatedDevice(thingCredentials);
            await device.Simulate();

            await thingCredentials.Deprovision();

            return 0;
        }
    }
}
