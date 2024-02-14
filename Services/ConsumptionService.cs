using MonitoringAndCommunication.Data;
using MonitoringAndCommunication.Models;

namespace MonitoringAndCommunication.Services
{
    public class ConsumptionService : IConsumptionService
    {
        private readonly EnergyDataContext _dbContext;

        public ConsumptionService(EnergyDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EnergyConsumption?> AddConsumption(EnergyConsumption energyConsumption)
        {
            await _dbContext.EnergyConsumptions.AddAsync(energyConsumption);

            _dbContext.SaveChanges();

            return energyConsumption;
        }
    }
}