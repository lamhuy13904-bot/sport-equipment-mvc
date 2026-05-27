using SportEquipment.Mvc.Models;
using SportEquipment.Mvc.ViewModels;

namespace SportEquipment.Mvc.Services;

public class EquipmentService
{
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

    // Các hàm mới phải nằm trong đây
    public List<Equipment> Search(string? keyword, decimal? minPrice)
    {
        var query = _equipments.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(e => e.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) || e.Brand.Contains(keyword, StringComparison.OrdinalIgnoreCase) || e.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }
        if (minPrice.HasValue) query = query.Where(e => e.Price >= minPrice.Value);
        return query.ToList();
    }

    public void Create(EquipmentCreateViewModel model)
    {
        var newId = _equipments.Count == 0 ? 1 : _equipments.Max(e => e.Id) + 1;
        var newEquipment = new Equipment
        {
            Id = newId, Code = $"EQP-NEW-{newId:000}", Name = model.Name, Brand = model.Brand, Price = model.Price, Quantity = model.Quantity, MinStock = model.MinStock, LastRestocked = DateTime.Now
        };
        _equipments.Add(newEquipment);
    }
} // <--- Dấu đóng này đóng class EquipmentService