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
        // 1. Kiểm tra mã Code (SKU) có bị trùng không
        Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null);

        // 2. Lấy dữ liệu để đổ vào Form Edit
        Task<EquipmentEditViewModel?> GetEquipmentForEditAsync(int id);

        // 3. Cập nhật dữ liệu (Có kiểm tra RowVersion)
        Task UpdateEquipmentAsync(EquipmentEditViewModel model);

        // 4. Xóa mềm (Soft Delete)
        Task<bool> SoftDeleteAsync(int id);

        // 5. Lấy danh sách Thùng rác
        Task<List<EquipmentTrashItemViewModel>> GetTrashAsync();

        // 6. Khôi phục sản phẩm (Restore)
        Task<bool> RestoreAsync(int id);

        // 7. Chỉnh sua số lượng sản phẩm (Adjust Stock)
        Task<AdjustStockViewModel?> GetEquipmentForAdjustStockAsync(int id);
        Task AdjustStockAsync(AdjustStockViewModel model);
    }
}