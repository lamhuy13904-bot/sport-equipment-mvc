using System.ComponentModel.DataAnnotations;

namespace SportEquipment.Mvc.ViewModels;

public class EquipmentCreateViewModel
{
    [Required(ErrorMessage = "Tên dụng cụ không được để trống")]
    [StringLength(100, ErrorMessage = "Tên không quá 100 ký tự")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Thương hiệu không được để trống")]
    public string Brand { get; set; } = "";

    [Range(10000, 50000000, ErrorMessage = "Giá bán phải từ 10.000 đến 50.000.000 VND")]
    public decimal Price { get; set; }

    [Range(0, 5000, ErrorMessage = "Số lượng không hợp lệ (0 - 5000)")]
    public int Quantity { get; set; }

    [Range(0, 1000, ErrorMessage = "Mức tồn tối thiểu không hợp lệ")]
    public int MinStock { get; set; }
}