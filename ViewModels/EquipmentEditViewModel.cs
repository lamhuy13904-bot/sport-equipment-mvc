namespace SportEquipment.Mvc.ViewModels
{
    public class EquipmentEditViewModel : EquipmentCreateViewModel
    {
        public int Id { get; set; }
        
        // RowVersion được chuyển thành chuỗi để có thể đưa vào thẻ <input type="hidden">
        public string? RowVersion { get; set; } = string.Empty; 
    }
}