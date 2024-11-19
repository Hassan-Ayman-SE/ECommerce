using OrderService.Data;
using OrderService.DTOs;
using OrderService.Models;
using OrderService.RabbitMQ;
using OrderService.Repositories.Interfaces;

namespace OrderService.Repositories.Services
{

    public class OrderRepository : IOrder
    {
        private readonly OrderDbContext  _context;
        //Inject message producer
        private readonly IMessageProducer _messageProducer;
        public OrderRepository(OrderDbContext context, IMessageProducer messageProducer)
        {
            _context = context;
            _messageProducer = messageProducer;
        }

        public async Task<Order> Save(OrderDto orderDto)
        {
            
            var order = new Order
            {
                ProductId = orderDto.ProductId,
                Quantity = orderDto.Quantity
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task SaveOrder(OrderDto orderDto) 
        {
            var order = await Save(orderDto);
            if (order is not null) {
                _messageProducer.SendMessage(order);
            }
        }

    }
}
