using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportEquipment.Mvc.Repositories;
using SportEquipment.Mvc.Services;
using SportEquipment.Mvc.ViewModels;

namespace SportEquipment.Mvc.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IEquipmentRepository _equipmentRepository;

        // Tiêm cả Service xử lý đơn hàng và Repository để lấy danh sách thiết bị
        public OrdersController(IOrderService orderService, IEquipmentRepository equipmentRepository)
        {
            _orderService = orderService;
            _equipmentRepository = equipmentRepository;
        }

        // Hiện form tạo đơn
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var equipments = await _equipmentRepository.GetAllReadOnlyAsync();
            // Đẩy danh sách thiết bị sang View để làm thẻ <select>
            ViewBag.EquipmentList = new SelectList(equipments, "Id", "Name");
            return View();
        }

        // Xử lý khi bấm nút Submit
        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var equipments = await _equipmentRepository.GetAllReadOnlyAsync();
                ViewBag.EquipmentList = new SelectList(equipments, "Id", "Name");
                return View(model);
            }

            try
            {
                // Gọi Transaction
                await _orderService.CreateOrderTransactionAsync(model);
                TempData["SuccessMessage"] = "Tạo đơn xuất kho thành công! Số lượng đã được tự động trừ.";
                return RedirectToAction("Index", "Equipments"); // Thành công thì quay về trang chủ
            }
            catch (Exception ex)
            {
                // Nếu lỗi (ví dụ không đủ tồn kho), Transaction sẽ Rollback và hiện lỗi ra form
                ModelState.AddModelError("", ex.Message);
                var equipments = await _equipmentRepository.GetAllReadOnlyAsync();
                ViewBag.EquipmentList = new SelectList(equipments, "Id", "Name");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var history = await _orderService.GetOrderHistoryAsync();
            return View(history);
        }
    }
}