using Microsoft.EntityFrameworkCore;
using SportEquipment.Mvc.Data;
using SportEquipment.Mvc.Models;

namespace SportEquipment.Mvc.Repositories
{
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly AppDbContext _context;

        public EquipmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Equipment>> GetAllReadOnlyAsync()
            => _context.Equipments.Include(e => e.Category).AsNoTracking().ToListAsync();

        public async Task<List<Equipment>> GetFilteredReadOnlyAsync(int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Equipments.Include(e => e.Category).AsNoTracking().AsQueryable();
            if (categoryId.HasValue) query = query.Where(e => e.CategoryId == categoryId.Value);
            if (minPrice.HasValue) query = query.Where(e => e.Price >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(e => e.Price <= maxPrice.Value);
            return await query.ToListAsync();
        }

        public Task<Equipment?> GetByIdAsync(int id)
            => _context.Equipments.Include(e => e.Category).FirstOrDefaultAsync(e => e.Id == id);

        public Task<List<Category>> GetAllCategoriesReadOnlyAsync()
            => _context.Categories.AsNoTracking().ToListAsync();

        public async Task AddAsync(Equipment equipment)
            => await _context.Equipments.AddAsync(equipment);

        public Task SaveChangesAsync()
            => _context.SaveChangesAsync();
    }
}