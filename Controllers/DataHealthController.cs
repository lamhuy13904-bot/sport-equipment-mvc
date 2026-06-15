using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportEquipment.Mvc.Data;
using SportEquipment.Mvc.ViewModels;

namespace SportEquipment.Mvc.Controllers
{
    public class DataHealthController : Controller
    {
        private readonly AppDbContext _context;

        // Bơm trực tiếp DbContext vào để test kết nối hạ tầng
        public DataHealthController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DataHealthViewModel();
            try
            {
                // 1. Kiểm tra kết nối Database xem Migration đã tạo DB thật chưa
                model.CanConnectToDatabase = await _context.Database.CanConnectAsync();
                
                // 2. Lấy tên Provider (Để chứng minh bạn đang dùng SQLite)
                model.DatabaseProvider = _context.Database.ProviderName ?? "Unknown";

                // 3. Đếm dữ liệu Seed (Dùng AsNoTracking theo đúng yêu cầu đề bài để tăng tốc)
                model.TotalEquipments = await _context.Equipments.AsNoTracking().CountAsync();
                model.TotalCategories = await _context.Categories.AsNoTracking().CountAsync();
                model.TotalOrders = await _context.Orders.AsNoTracking().CountAsync();
            }
            catch (Exception)
            {
                model.CanConnectToDatabase = false;
            }

            return View(model);
        }
    }
}