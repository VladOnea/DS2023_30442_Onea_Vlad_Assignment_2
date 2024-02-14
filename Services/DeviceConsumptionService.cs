using MonitoringAndCommunication.Data;
using MonitoringAndCommunication.Models;

namespace MonitoringAndCommunication.Services
{
    public class DeviceConsumptionService : IDeviceConsumptionService
    {
        private readonly EnergyDataContext _dbContext;

        public DeviceConsumptionService(EnergyDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DeviceConsumption?> AddConsumption(DeviceConsumption deviceConsumption)
        {
            await _dbContext.DeviceConsumptions.AddAsync(deviceConsumption);

            _dbContext.SaveChanges();

            return deviceConsumption;
        }

        public  List<DeviceConsumption> GetAll()
        {
            return  _dbContext.DeviceConsumptions.ToList();
        }

        public DeviceConsumption? GetDevice(int deviceId)
        {
            return _dbContext.DeviceConsumptions.Where(d => d.DeviceID == deviceId).FirstOrDefault();
        }
    }
}
