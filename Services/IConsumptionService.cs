using MonitoringAndCommunication.Models;

namespace MonitoringAndCommunication.Services
{
    public interface IConsumptionService
    {
        public Task<EnergyConsumption?> AddConsumption(EnergyConsumption energyConsumption);
    }
}
