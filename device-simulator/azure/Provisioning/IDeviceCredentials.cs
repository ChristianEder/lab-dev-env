using Microsoft.Azure.Devices.Client;
using System.Threading.Tasks;

namespace IoT.Azure.DeviceSimulator.Provisioning
{
    public interface IDeviceCredentials
    {
        Task<DeviceClient> Connect();
        Task Deprovision();
    }
}
