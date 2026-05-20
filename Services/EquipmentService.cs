using SportEquipment.Mvc.Models;
using SportEquipment.Mvc.ViewModels;

namespace SportEquipment.Mvc.Services;

public class EquipmentService
{
    // Hard-code dữ liệu mẫu về thiết bị thể thao
    private readonly List<Equipment> _equipments = new()
    {
        new Equipment { Id = 1, Code = "SHOE-NK-01", Name = "Nike Air Zoom Pegasus", Brand = "Nike", Price = 3200000, Quantity = 12, MinStock = 5, LastRestocked = DateTime.Now },
        new Equipment { Id = 2, Code = "BALL-AD-01", Name = "Adidas Al Rihla Pro", Brand = "Adidas", Price = 2500000, Quantity = 3, MinStock = 5, LastRestocked = DateTime.Now },
        new Equipment { Id = 3, Code = "DUMB-10KG", Name = "Tạ tay cao su 10kg", Brand = "Kensports", Price = 450000, Quantity = 0, MinStock = 10, LastRestocked = DateTime.Now }
    };

    public List<Equipment> GetAll() => _equipments;
    public Equipment? GetById(int id) => _equipments.FirstOrDefault(e => e.Id == id);
    
    public EquipmentStatsViewModel GetStats()
    {
        return new EquipmentStatsViewModel
        {
            TotalItems = _equipments.Count,
            OutOfStockCount = _equipments.Count(e => e.Quantity == 0),
            LowStockCount = _equipments.Count(e => e.Quantity > 0 && e.Quantity <= e.MinStock)
        };
    }
}