using SportEquipment.Mvc.Models;

namespace SportEquipment.Mvc.Repositories
{
    public interface IEquipmentRepository
    {
        Task<List<Equipment>> GetAllReadOnlyAsync();
        Task<List<Equipment>> GetFilteredReadOnlyAsync(int? categoryId, decimal? minPrice, decimal? maxPrice);
        Task<Equipment?> GetByIdAsync(int id);
        Task<List<Category>> GetAllCategoriesReadOnlyAsync();
        Task AddAsync(Equipment equipment);
        Task SaveChangesAsync();
    }
}