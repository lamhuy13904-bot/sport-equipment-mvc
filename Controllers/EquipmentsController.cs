using Microsoft.AspNetCore.Mvc;
using SportEquipment.Mvc.Services;
using SportEquipment.Mvc.ViewModels;

namespace SportEquipment.Mvc.Controllers;

public class EquipmentsController : Controller
{
    private readonly EquipmentService _service;
    public EquipmentsController(EquipmentService service) { _service = service; }

    public IActionResult Index() 
    {
        var data = _service.GetAll().Select(e => new EquipmentListItemViewModel 
        { Id = e.Id, Code = e.Code, Name = e.Name, Brand = e.Brand, Price = e.Price, Quantity = e.Quantity }
        ).ToList(); return View(data);
    }
    public IActionResult Detail(int id) 
    { 
        var eq = _service.GetById(id); 
        if (eq == null) return NotFound($"Không tìm thấy dụng cụ ID {id}"); 
        var vm = new EquipmentDetailViewModel 
        { 
            Id = eq.Id, Name = eq.Name, Brand = eq.Brand, Quantity = eq.Quantity, MinStock = eq.MinStock 
        }; 
        return View(vm);
    }
    public IActionResult Stats() { return View(_service.GetStats()); }
    public IActionResult Welcome() { return Content("..."); }
    public IActionResult EquipmentJson() { return Json(_service.GetAll()); }
    public IActionResult GoToList() { return RedirectToAction(nameof(Index)); }

    // Các Action mới phải nằm trong cặp ngoặc nhọn của class EquipmentsController
    [HttpGet]
    public IActionResult Search(string? keyword, decimal? minPrice)
    {
        var results = _service.Search(keyword, minPrice).Select(e => new EquipmentListItemViewModel { Id = e.Id, Code = e.Code, Name = e.Name, Brand = e.Brand, Price = e.Price, Quantity = e.Quantity }).ToList();
        var vm = new EquipmentSearchViewModel { Keyword = keyword ?? "", MinPrice = minPrice, Results = results };
        return View(vm);
    }

    [HttpGet]
    public IActionResult Create() { return View(new EquipmentCreateViewModel { Quantity = 1, MinStock = 1 }); }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(EquipmentCreateViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        _service.Create(model);
        TempData["SuccessMessage"] = $"Đã thêm dụng cụ '{model.Name}' thành công!";
        return RedirectToAction(nameof(Index));
    }
} // <--- Dấu đóng này đóng class EquipmentsController