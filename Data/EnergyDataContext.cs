using Microsoft.EntityFrameworkCore;
using MonitoringAndCommunication.Models;

namespace MonitoringAndCommunication.Data
{
   public class EnergyDataContext : DbContext 
    {
        public EnergyDataContext(DbContextOptions<EnergyDataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();
        }

        public DbSet<EnergyConsumption> EnergyConsumptions { get; set; }

        public DbSet<DeviceConsumption> DeviceConsumptions { get; set; }
    }
}
