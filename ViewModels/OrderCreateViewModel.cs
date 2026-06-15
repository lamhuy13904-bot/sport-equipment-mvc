using System.ComponentModel.DataAnnotations;

namespace SportEquipment.Mvc.ViewModels
{
    public class OrderCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên khách hàng")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn dụng cụ")]
        public int EquipmentId { get; set; }

        [Range(1, 100, ErrorMessage = "Số lượng mua phải từ 1 đến 100")]
        public int Quantity { get; set; }
    }
}