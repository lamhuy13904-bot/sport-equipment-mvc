namespace SportEquipment.Mvc.ViewModels
{
    public class EquipmentTrashItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DateTime? DeletedAt { get; set; }
    }
}