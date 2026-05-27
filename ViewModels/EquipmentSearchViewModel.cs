namespace SportEquipment.Mvc.ViewModels;

public class EquipmentSearchViewModel
{
    public string Keyword { get; set; } = "";
    public decimal? MinPrice { get; set; }
    public List<EquipmentListItemViewModel> Results { get; set; } = new();
}