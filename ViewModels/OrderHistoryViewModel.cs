namespace SportEquipment.Mvc.ViewModels
{
    public class OrderHistoryItemViewModel
    {
        public string EquipmentName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class OrderHistoryViewModel
    {
        public int OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public List<OrderHistoryItemViewModel> Items { get; set; } = new();
    }
}