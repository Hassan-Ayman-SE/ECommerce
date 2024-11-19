using OrderService.DTOs;
using OrderService.Models;

namespace OrderService.Repositories.Interfaces
{
    public interface IOrder
    {
        Task<Order> Save(OrderDto orderDto);
    }
}
