using InventoryService.DTOs;

namespace InventoryService.Repositories.Interfaces
{
    public interface IInventory
    {
        Task<bool> CheckAndReduceStock(InventoryDto order);
    }
}
