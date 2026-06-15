using SportEquipment.Mvc.ViewModels;

namespace SportEquipment.Mvc.Services
{
    public interface IOrderService
    {
        Task CreateOrderTransactionAsync(OrderCreateViewModel model);
        Task<List<OrderHistoryViewModel>> GetOrderHistoryAsync(); 
    }
}