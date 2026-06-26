using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet]
        public IActionResult Create()
        {
            return View(new EquipmentCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EquipmentCreateViewModel model)
        {
            // Kiểm tra xem dữ liệu nhập vào có vi phạm các ràng buộc (Data Annotations) không
            if (!ModelState.IsValid) return View(model);

            // Kiểm tra trùng Code (Custom Validation)
            bool isUnique = await _equipmentService.IsCodeUniqueAsync(model.Code);
            if (!isUnique)
            {
                ModelState.AddModelError("Code", "Mã thiết bị (Code) này đã tồn tại trong hệ thống.");
                return View(model);
            }

            await _equipmentService.CreateAsync(model);
            TempData["SuccessMessage"] = "Đã thêm dụng cụ thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _equipmentService.GetEquipmentForEditAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EquipmentEditViewModel model)
        {
            if (id != model.Id) return NotFound();
            
            if (!ModelState.IsValid) return View(model);

            // Kiểm tra mã Code có bị trùng với sản phẩm KHÁC không
            bool isUnique = await _equipmentService.IsCodeUniqueAsync(model.Code, model.Id);
            if (!isUnique)
            {
                ModelState.AddModelError("Code", "Mã thiết bị (Code) này đã được sử dụng.");
                return View(model);
            }

            try
            {
                await _equipmentService.UpdateEquipmentAsync(model);
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                // Bắt lỗi khi 2 admin cùng sửa 1 sản phẩm
                ModelState.AddModelError(string.Empty, "Dữ liệu đã được người khác cập nhật. Vui lòng tải lại trang và thử lại.");
                return View(model);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _equipmentService.SoftDeleteAsync(id);
            if (!success) return NotFound();

            TempData["SuccessMessage"] = "Đã đưa dụng cụ vào thùng rác.";
            return RedirectToAction(nameof(Index));
        }

        // Mở trang Thùng rác
        [HttpGet]
        public async Task<IActionResult> Trash()
        {
            var trashItems = await _equipmentService.GetTrashAsync();
            return View(trashItems);
        }

        // Khôi phục dữ liệu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var success = await _equipmentService.RestoreAsync(id);
            if (!success) return NotFound();

            TempData["SuccessMessage"] = "Đã khôi phục dụng cụ thành công.";
            return RedirectToAction(nameof(Trash));
        }

        // 7. Chỉnh sua số lượng sản phẩm (Adjust Stock)
        [HttpGet]
        public async Task<IActionResult> AdjustStock(int id)
        {
            var model = await _equipmentService.GetEquipmentForAdjustStockAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdjustStock(int id, AdjustStockViewModel model)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) return View(model);

            try
            {
                await _equipmentService.AdjustStockAsync(model);
                TempData["SuccessMessage"] = "Điều chỉnh số lượng tồn kho thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, "Số lượng đã được người khác cập nhật. Vui lòng tải lại trang và thử lại.");
                return View(model);
            }
        }
    }
}