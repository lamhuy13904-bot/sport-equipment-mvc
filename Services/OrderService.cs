using Microsoft.EntityFrameworkCore;
using SportEquipment.Mvc.Data;
using SportEquipment.Mvc.Models;
using SportEquipment.Mvc.ViewModels;
using SportEquipment.Mvc.Repositories;

namespace SportEquipment.Mvc.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IOrderRepository _orderRepository;

        public OrderService(AppDbContext context, IOrderRepository orderRepository)
        {
            _context = context;
            _orderRepository = orderRepository;
        }

        public async Task CreateOrderTransactionAsync(OrderCreateViewModel model)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var equipment = await _context.Equipments.FirstOrDefaultAsync(e => e.Id == model.EquipmentId);
                if (equipment == null) throw new Exception("Dụng cụ không tồn tại.");
                if (equipment.Quantity < model.Quantity) throw new Exception("Không đủ số lượng tồn kho.");

                var order = new Order
                {
                    CustomerName = model.CustomerName,
                    CreatedAt = DateTime.Now,
                    TotalAmount = equipment.Price * model.Quantity
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                var item = new OrderItem
                {
                    OrderId = order.Id,
                    EquipmentId = equipment.Id,
                    Quantity = model.Quantity,
                    UnitPrice = equipment.Price
                };
                _context.OrderItems.Add(item);

                equipment.Quantity -= model.Quantity;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<OrderHistoryViewModel>> GetOrderHistoryAsync()
        {
            var orders = await _orderRepository.GetOrderHistoryReadOnlyAsync();
            
            return orders.Select(o => new OrderHistoryViewModel
            {
                OrderId = o.Id,
                CreatedAt = o.CreatedAt,
                CustomerName = o.CustomerName,
                TotalAmount = o.TotalAmount,
                Items = o.OrderItems.Select(i => new OrderHistoryItemViewModel
                {
                    EquipmentName = i.Equipment?.Name ?? "Dụng cụ đã xóa",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            }).ToList();
        }
    }
}