using Microsoft.EntityFrameworkCore;
using SportEquipment.Mvc.Data;
using SportEquipment.Mvc.Models;

namespace SportEquipment.Mvc.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        // Dùng Include để lấy chi tiết và AsNoTracking để tối ưu hiệu năng
        public Task<List<Order>> GetOrderHistoryReadOnlyAsync()
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Equipment) // Lấy thêm thông tin Dụng cụ để lấy Tên hiển thị
                .AsNoTracking()
                .OrderByDescending(o => o.CreatedAt) // Sắp xếp đơn mới nhất lên đầu
                .ToListAsync();
        }
    }
}