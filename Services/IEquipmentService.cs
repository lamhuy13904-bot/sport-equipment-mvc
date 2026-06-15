using SportEquipment.Mvc.Models;
using SportEquipment.Mvc.ViewModels;

namespace SportEquipment.Mvc.Services
{
    public interface IEquipmentService
    {
        Task<List<EquipmentListItemViewModel>> GetEquipmentListAsync();
        Task<EquipmentFilterViewModel> GetFilteredEquipmentsAsync(int? categoryId, decimal? minPrice, decimal? maxPrice, string? keyword);
        
        // Các hàm cũ của bạn được nâng cấp lên dạng Bất đồng bộ (Task)
        Task<Equipment?> GetByIdAsync(int id);
        Task<EquipmentStatsViewModel> GetStatsAsync();
        Task CreateAsync(EquipmentCreateViewModel model);
    }
}