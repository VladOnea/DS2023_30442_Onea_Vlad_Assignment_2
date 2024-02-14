using MonitoringAndCommunication.Models;

namespace MonitoringAndCommunication.Services
{
    public interface IDeviceConsumptionService
    {
        public  Task<DeviceConsumption?> AddConsumption(DeviceConsumption deviceConsumption);

        public List<DeviceConsumption> GetAll();

        public DeviceConsumption? GetDevice(int deviceId);
    }
}
