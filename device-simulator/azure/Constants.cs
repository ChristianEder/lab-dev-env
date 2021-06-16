using System;

namespace IoT.Azure.DeviceSimulator
{
    public static class Constants
    {
        public static string IoTHubConnectionString => Environment.GetEnvironmentVariable("IoTHubConnectionString");
    }
}
