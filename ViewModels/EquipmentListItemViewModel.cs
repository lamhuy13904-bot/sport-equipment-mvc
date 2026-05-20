namespace SportEquipment.Mvc.ViewModels;

public class EquipmentListItemViewModel
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Brand { get; set; } = "";
    public decimal Price { get; set; }
    public string PriceText => $"{Price:N0} VND";
    public int Quantity { get; set; }
    
    // Logic nghiệp vụ tự động phân loại trạng thái hàng hóa
    public string Status 
    {
        get 
        {
            if (Quantity == 0) return "Hết hàng";
            if (Quantity <= 5) return "Sắp hết";
            return "Sẵn sàng";
        }
    }
}