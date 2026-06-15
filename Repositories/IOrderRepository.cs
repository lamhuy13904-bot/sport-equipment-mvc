using SportEquipment.Mvc.Models;

namespace SportEquipment.Mvc.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetOrderHistoryReadOnlyAsync();
    }
}