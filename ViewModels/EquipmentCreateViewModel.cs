using System.ComponentModel.DataAnnotations;

namespace SportEquipment.Mvc.ViewModels
{
    public class EquipmentCreateViewModel
    {
        [Required(ErrorMessage = "Tên dụng cụ là bắt buộc.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Tên phải từ 3 đến 150 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã thiết bị (Code) là bắt buộc.")]
        [RegularExpression(@"^[A-Z0-9\-]+$", ErrorMessage = "Mã chỉ gồm chữ in hoa, số và dấu '-'.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Thương hiệu là bắt buộc.")]
        public string Brand { get; set; } = string.Empty;

        [Range(1000, 100000000, ErrorMessage = "Giá phải từ 1.000 đến 100.000.000 VNĐ.")]
        public decimal Price { get; set; }

        [Range(0, 10000, ErrorMessage = "Số lượng phải từ 0 đến 10.000.")]
        public int Quantity { get; set; }

        [Range(0, 1000, ErrorMessage = "Mức tối thiểu phải từ 0 đến 1000.")]
        public int MinStock { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        public int CategoryId { get; set; }
    }
}