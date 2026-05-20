using Microsoft.AspNetCore.Mvc;
using SportEquipment.Mvc.Services;
using SportEquipment.Mvc.ViewModels;

namespace SportEquipment.Mvc.Controllers;

public class EquipmentsController : Controller
{
    private readonly EquipmentService _service;

    public EquipmentsController(EquipmentService service)
    {
        _service = service;
    }

    // URL: /Equipments
    public IActionResult Index()
    {
        var data = _service.GetAll().Select(e => new EquipmentListItemViewModel
        {
            Id = e.Id, Code = e.Code, Name = e.Name, Brand = e.Brand, Price = e.Price, Quantity = e.Quantity
        }).ToList();
        return View(data);
    }

    // URL: /Equipments/Detail/{id}
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

    // URL: /Equipments/Stats
    public IActionResult Stats()
    {
        return View(_service.GetStats());
    }

    // URL: /Equipments/Welcome (Trả về chuỗi chữ thuần)
    public IActionResult Welcome()
    {
        return Content("Chào mừng đến với Hệ thống Quản lý Thiết bị SportHub.");
    }

    // URL: /Equipments/EquipmentJson (Trả về cấu trúc dữ liệu JSON)
    public IActionResult EquipmentJson()
    {
        return Json(_service.GetAll());
    }

    // URL: /Equipments/GoToList (Chuyển hướng người dùng)
    public IActionResult GoToList()
    {
        return RedirectToAction(nameof(Index));
    }
}