namespace SportEquipment.Mvc.ViewModels;

public class EquipmentDetailViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Brand { get; set; } = "";
    public int Quantity { get; set; }
    public int MinStock { get; set; }
    
    // Gợi ý quản lý kho tự động dựa trên số lượng tồn
    public string Suggestion => Quantity <= MinStock 
        ? "Cần nhập thêm dụng cụ này để đảm bảo vận hành." 
        : "Số lượng thiết bị đang đủ dùng.";
}