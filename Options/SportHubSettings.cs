namespace SportEquipment.Mvc.Options
{
    public class SportHubSettings
    {
        public string AppName { get; set; } = string.Empty;
        public int LowStockThreshold { get; set; }
        public string SupportEmail { get; set; } = string.Empty;
    }
}