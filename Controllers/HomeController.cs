using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportEquipment.Mvc.Data;

namespace SportEquipment.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Dùng AsNoTracking và IgnoreQueryFilters để tối ưu hóa việc đếm dữ liệu
            var totalEquipments = await _context.Equipments.IgnoreQueryFilters().AsNoTracking().CountAsync();
            var activeEquipments = await _context.Equipments.AsNoTracking().CountAsync(); // Tự động lọc IsDeleted = false
            var deletedEquipments = await _context.Equipments.IgnoreQueryFilters().AsNoTracking().CountAsync(e => e.IsDeleted);

            // Truyền dữ liệu sang View bằng ViewBag
            ViewBag.Total = totalEquipments;
            ViewBag.Active = activeEquipments;
            ViewBag.Deleted = deletedEquipments;

            return View();
        }
    }
}