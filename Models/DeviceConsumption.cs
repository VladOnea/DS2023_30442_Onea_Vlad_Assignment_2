namespace MonitoringAndCommunication.Models
{
    public class DeviceConsumption
    {
        public int Id { get; set; }
        public int DeviceID { get; set; }

        public int? UserId { get; set; }

        public string MaxConsumption { get; set; }
    }
}
