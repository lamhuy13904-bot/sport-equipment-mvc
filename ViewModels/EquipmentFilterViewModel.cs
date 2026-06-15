using SportEquipment.Mvc.Models;

namespace SportEquipment.Mvc.ViewModels
{
    public class EquipmentFilterViewModel
    {
        public List<EquipmentListItemViewModel> Equipments { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public int? SelectedCategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}