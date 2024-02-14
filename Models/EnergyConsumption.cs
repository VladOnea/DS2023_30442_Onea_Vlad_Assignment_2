namespace MonitoringAndCommunication.Models
{
    public class EnergyConsumption
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }

        public float Consumption {  get; set; }

        public string Timestamp { get; set; }
    }
}
