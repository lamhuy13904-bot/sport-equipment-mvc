using Microsoft.AspNetCore.Mvc;
using SportEquipment.Mvc.Services;
using SportEquipment.Mvc.ViewModels;

namespace SportEquipment.Mvc.Controllers
{
    public class EquipmentsController : Controller
    {
        private readonly IEquipmentService _equipmentService;

        // Tiêm IEquipmentService vào Controller
        public EquipmentsController(IEquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        // 1. Trang danh sách
        public async Task<IActionResult> Index()
        {
            var data = await _equipmentService.GetEquipmentListAsync();
            return View(data);
        }

        // 2. Chức năng Lọc / Tìm kiếm 
        public async Task<IActionResult> Search(int? categoryId, decimal? minPrice, decimal? maxPrice, string? keyword)
        {
            var data = await _equipmentService.GetFilteredEquipmentsAsync(categoryId, minPrice, maxPrice, keyword);
            return View(data); 
        }

        // 3. Xem chi tiết
        public async Task<IActionResult> Detail(int id)
        {
            var equipment = await _equipmentService.GetByIdAsync(id);
            if (equipment == null) return NotFound();
            return View(equipment);
        }

        // 4. Thống kê
        public async Task<IActionResult> Stats()
        {
            var stats = await _equipmentService.GetStatsAsync();
            return View(stats);
        }

        // 5. Form tạo mới (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 6. Xử lý dữ liệu tạo mới (POST)
        [HttpPost]
        public async Task<IActionResult> Create(EquipmentCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _equipmentService.CreateAsync(model);
            TempData["SuccessMessage"] = "Thêm dụng cụ thể thao thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}