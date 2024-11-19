using InventoryService.Data;
using InventoryService.DTOs;
using InventoryService.Models;
using InventoryService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace InventoryService.Repositories.Services
{
    public class InventoryRepository : IInventory
    {
        private readonly InventoryDbContext _context;

        public InventoryRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckAndReduceStock(InventoryDto order)
        {
            var inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.ProductId == order.ProductId);

            if (inventory == null || inventory.Stock < order.Quantity)
                return false;

            inventory.Stock -= order.Quantity;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
