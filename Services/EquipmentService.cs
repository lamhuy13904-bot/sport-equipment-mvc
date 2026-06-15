using Microsoft.Extensions.Options;
using SportEquipment.Mvc.Models;
using SportEquipment.Mvc.Options;
using SportEquipment.Mvc.Repositories;
using SportEquipment.Mvc.ViewModels;

namespace SportEquipment.Mvc.Services
{
    public class EquipmentService : IEquipmentService
    {
        // TIÊM DEPENDENCY: Gọi Database qua Repository và lấy Cấu hình ngưỡng từ AppSettings
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly SportHubSettings _settings;

        public EquipmentService(IEquipmentRepository equipmentRepository, IOptions<SportHubSettings> options)
        {
            _equipmentRepository = equipmentRepository;
            _settings = options.Value;
        }

        // 1. Lấy danh sách hiển thị
        public async Task<List<EquipmentListItemViewModel>> GetEquipmentListAsync()
        {
            var equipments = await _equipmentRepository.GetAllReadOnlyAsync();
            return equipments.Select(e => new EquipmentListItemViewModel
            {
                Id = e.Id,
                Code = e.Code, // Dùng biến Code của bạn
                Name = e.Name,
                Price = e.Price,
                Quantity = e.Quantity, // Dùng biến Quantity của bạn
                CategoryName = e.Category?.Name ?? "N/A",
                IsLowStock = e.Quantity <= _settings.LowStockThreshold && e.Quantity > 0 // Feature 1 của Lab 04
            }).ToList();
        }

        // 2. Tìm kiếm và lọc (Kết hợp Search cũ của bạn và Filter mới của Lab 04)
        public async Task<EquipmentFilterViewModel> GetFilteredEquipmentsAsync(int? categoryId, decimal? minPrice, decimal? maxPrice, string? keyword)
        {
            var equipments = await _equipmentRepository.GetFilteredReadOnlyAsync(categoryId, minPrice, maxPrice);

            // Xử lý tìm kiếm bằng keyword của bạn
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                equipments = equipments.Where(e => 
                    e.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) || 
                    e.Brand.Contains(keyword, StringComparison.OrdinalIgnoreCase) || 
                    e.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var categories = await _equipmentRepository.GetAllCategoriesReadOnlyAsync();

            return new EquipmentFilterViewModel
            {
                Categories = categories,
                SelectedCategoryId = categoryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Equipments = equipments.Select(e => new EquipmentListItemViewModel
                {
                    Id = e.Id,
                    Code = e.Code,
                    Name = e.Name,
                    Price = e.Price,
                    Quantity = e.Quantity,
                    CategoryName = e.Category?.Name ?? "N/A",
                    IsLowStock = e.Quantity <= _settings.LowStockThreshold && e.Quantity > 0
                }).ToList()
            };
        }

        // 3. Lấy chi tiết (Nâng cấp hàm cũ)
        public async Task<Equipment?> GetByIdAsync(int id)
        {
            return await _equipmentRepository.GetByIdAsync(id);
        }

        // 4. Lấy thống kê (Nâng cấp hàm cũ)
        public async Task<EquipmentStatsViewModel> GetStatsAsync()
        {
            var equipments = await _equipmentRepository.GetAllReadOnlyAsync();
            return new EquipmentStatsViewModel
            {
                TotalItems = equipments.Count,
                OutOfStockCount = equipments.Count(e => e.Quantity == 0),
                LowStockCount = equipments.Count(e => e.Quantity > 0 && e.Quantity <= e.MinStock)
            };
        }

        // 5. Thêm mới (Nâng cấp hàm cũ để lưu vào Database thật)
        public async Task CreateAsync(EquipmentCreateViewModel model)
        {
            var equipments = await _equipmentRepository.GetAllReadOnlyAsync();
            var count = equipments.Count;
            
            var newEquipment = new Equipment
            {
                // Tự sinh mã nếu không nhập
                Code = $"EQP-NEW-{(count + 1):000}", 
                Name = model.Name, 
                Brand = model.Brand, 
                Price = model.Price, 
                Quantity = model.Quantity, 
                MinStock = model.MinStock, 
                LastRestocked = DateTime.Now,
                CategoryId = 1 // Tạm thời gán mặc định vào danh mục 1 để không bị lỗi Database. Ở các bước sau ta sẽ thêm thẻ <select> vào giao diện để chọn.
            };

            await _equipmentRepository.AddAsync(newEquipment);
            await _equipmentRepository.SaveChangesAsync(); // Lệnh này mới thực sự lưu vào Database
        }
    }
}