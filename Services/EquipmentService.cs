using Microsoft.Extensions.Options;
using SportEquipment.Mvc.Models;
using SportEquipment.Mvc.Options;
using SportEquipment.Mvc.Repositories;
using SportEquipment.Mvc.ViewModels;
using Microsoft.Extensions.Logging; 
using Microsoft.EntityFrameworkCore;
using SportEquipment.Mvc.Data;

namespace SportEquipment.Mvc.Services
{   
    
    public class EquipmentService : IEquipmentService
    {
        // TIÊM DEPENDENCY: Gọi Database qua Repository và lấy Cấu hình ngưỡng từ AppSettings
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly SportHubSettings _settings;
        private readonly AppDbContext _context; 
        private readonly ILogger<EquipmentService> _logger;

        public EquipmentService(
            IEquipmentRepository equipmentRepository, 
            IOptions<SportHubSettings> options,
            AppDbContext context, 
            ILogger<EquipmentService> logger)
        {
            _equipmentRepository = equipmentRepository;
            _settings = options.Value;
            _context = context; // GÁN BIẾN
            _logger = logger; // GÁN BIẾN
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
                UpdatedAt = DateTime.Now,
                CategoryId = 1 // Tạm thời gán mặc định vào danh mục 1 để không bị lỗi Database. Ở các bước sau ta sẽ thêm thẻ <select> vào giao diện để chọn.
            };

            await _equipmentRepository.AddAsync(newEquipment);
            await _equipmentRepository.SaveChangesAsync(); // Lệnh này mới thực sự lưu vào Database
        }
        // 1. Kiểm tra mã Code duy nhất
        public async Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null)
        {
            var query = _context.Equipments.IgnoreQueryFilters().Where(e => e.Code == code);
            if (excludeId.HasValue)
            {
                query = query.Where(e => e.Id != excludeId.Value);
            }
            return !await query.AnyAsync();
        }

        // 2. Lấy dữ liệu cho Form Edit
        public async Task<EquipmentEditViewModel?> GetEquipmentForEditAsync(int id)
        {
            var equipment = await _context.Equipments.FirstOrDefaultAsync(e => e.Id == id);
            if (equipment == null) return null;

            return new EquipmentEditViewModel
            {
                Id = equipment.Id,
                Name = equipment.Name,
                Code = equipment.Code,
                Brand = equipment.Brand,
                Price = equipment.Price,
                Quantity = equipment.Quantity,
                MinStock = equipment.MinStock,
                CategoryId = equipment.CategoryId,
                // Chuyển mảng byte thành chuỗi Base64 để nhét vào form HTML
                RowVersion = equipment.RowVersion != null ? Convert.ToBase64String(equipment.RowVersion) : ""            
            };
        }

        // 3. Cập nhật dữ liệu & Xử lý Concurrency (Chống ghi đè)
        public async Task UpdateEquipmentAsync(EquipmentEditViewModel model)
        {
            var equipment = await _context.Equipments.FirstOrDefaultAsync(e => e.Id == model.Id);
            if (equipment == null) throw new Exception("Không tìm thấy dụng cụ.");

            // ========================================================
            // CHỐT CHẶN 1: TỰ SO SÁNH MÃ BẢO VỆ (MANUAL CHECK)
            // ========================================================
            var clientRowVersion = Convert.FromBase64String(model.RowVersion ?? "");
            
            // Nếu Database ĐÃ CÓ mã bảo vệ (Length > 0) VÀ mã đó KHÁC với mã Tab 2 gửi lên
            if (equipment.RowVersion != null && equipment.RowVersion.Length > 0)
            {
                if (!equipment.RowVersion.SequenceEqual(clientRowVersion))
                {
                    // Lập tức quăng bom, chặn đứng giao dịch của Tab 2!
                    throw new DbUpdateConcurrencyException("Dữ liệu đã bị thay đổi bởi người khác.");
                }
            }

            // Cập nhật thông tin bình thường
            equipment.Name = model.Name;
            equipment.Code = model.Code;
            equipment.Brand = model.Brand;
            equipment.Price = model.Price;
            equipment.Quantity = model.Quantity;
            equipment.MinStock = model.MinStock;
            equipment.CategoryId = model.CategoryId;
            equipment.UpdatedAt = DateTime.Now;

            // Lưu các thông tin trên vào Database
            await _context.SaveChangesAsync();

            // ========================================================
            // CHỐT CHẶN 2: ÉP SQLITE TẠO MÃ MỚI BẰNG LỆNH RAW SQL
            // ========================================================
            // Lệnh này hoàn toàn qua mặt sự bảo thủ của EF Core, ép SQLite nhét 16 byte ngẫu nhiên vào DB
            await _context.Database.ExecuteSqlRawAsync("UPDATE Equipments SET RowVersion = randomblob(16) WHERE Id = {0}", equipment.Id);
            
            _logger.LogInformation("Đã cập nhật dụng cụ. ID={EquipmentId}", model.Id);
        }

        // 4. Xóa mềm (Soft Delete)
        public async Task<bool> SoftDeleteAsync(int id)
        {
            var equipment = await _context.Equipments.FirstOrDefaultAsync(e => e.Id == id);
            if (equipment == null) return false;

            equipment.IsDeleted = true;
            equipment.DeletedAt = DateTime.Now;
            equipment.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            _logger.LogWarning("Đã xóa mềm dụng cụ. ID={EquipmentId}", id);
            return true;
        }

        // 5. Lấy danh sách thùng rác (Bỏ qua QueryFilter để lấy đồ đã xóa)
        public async Task<List<EquipmentTrashItemViewModel>> GetTrashAsync()
        {
            return await _context.Equipments
                .IgnoreQueryFilters() // Lệnh lôi đồ trong thùng rác ra
                .Where(e => e.IsDeleted)
                .AsNoTracking()
                .Select(e => new EquipmentTrashItemViewModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    Code = e.Code,
                    DeletedAt = e.DeletedAt
                })
                .ToListAsync();
        }

        // 6. Khôi phục (Restore)
        public async Task<bool> RestoreAsync(int id)
        {
            var equipment = await _context.Equipments
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => e.Id == id && e.IsDeleted);

            if (equipment == null) return false;

            equipment.IsDeleted = false;
            equipment.DeletedAt = null;
            equipment.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Đã khôi phục dụng cụ. ID={EquipmentId}", id);
            return true;
        }

        // Lấy dữ liệu mở Form Điều chỉnh kho
        public async Task<AdjustStockViewModel?> GetEquipmentForAdjustStockAsync(int id)
        {
            var equipment = await _context.Equipments.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (equipment == null) return null;

            return new AdjustStockViewModel
            {
                Id = equipment.Id,
                Name = equipment.Name,
                Quantity = equipment.Quantity,
                RowVersion = equipment.RowVersion != null ? Convert.ToBase64String(equipment.RowVersion) : ""
            };
        }

        // Lưu dữ liệu Điều chỉnh kho
        public async Task AdjustStockAsync(AdjustStockViewModel model)
        {
            var equipment = await _context.Equipments.FirstOrDefaultAsync(e => e.Id == model.Id);
            if (equipment == null) throw new Exception("Không tìm thấy dụng cụ.");

            // Tự so sánh RowVersion bằng tay
            var clientRowVersion = Convert.FromBase64String(model.RowVersion ?? "");
            if (equipment.RowVersion != null && equipment.RowVersion.Length > 0)
            {
                if (!equipment.RowVersion.SequenceEqual(clientRowVersion))
                {
                    throw new DbUpdateConcurrencyException("Dữ liệu đã bị thay đổi.");
                }
            }

            // Chỉ cập nhật duy nhất Số lượng
            equipment.Quantity = model.Quantity;
            equipment.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            // Cập nhật mã bảo vệ mới bằng Raw SQL
            await _context.Database.ExecuteSqlRawAsync("UPDATE Equipments SET RowVersion = randomblob(16) WHERE Id = {0}", equipment.Id);

            _logger.LogInformation("Đã điều chỉnh tồn kho. ID={EquipmentId}, NewQty={Quantity}", model.Id, model.Quantity);
        }
    }
}