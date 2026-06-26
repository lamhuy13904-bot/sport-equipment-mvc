using System.ComponentModel.DataAnnotations;

namespace SportEquipment.Mvc.ViewModels
{
    public class AdjustStockViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty; // Chỉ dùng để hiển thị, không cho sửa

        [Required(ErrorMessage = "Vui lòng nhập số lượng.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không được phép nhỏ hơn 0.")]
        public int Quantity { get; set; }

        // Bắt buộc phải có để chống ghi đè
        public string? RowVersion { get; set; }
    }
}