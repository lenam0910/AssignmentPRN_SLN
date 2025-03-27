using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Repository;

namespace Service
{
    public class InventoryService
    {
        private InventoryRepository repository;


        public InventoryService()
        {
            repository = new InventoryRepository();
        }


        public Inventory GetInventory(int id)
        {
            return repository.GetInventory(id);
        }
        public int getTotalQuantityInventoyByID(int id)
        {
            var lst = repository.GetAllInventory();
            var display = new List<Inventory>();
            foreach (var item in lst)
            {
                if (item.IsDeleted == false && item.WarehouseId == id)
                {
                    display.Add(item);
                }
            }
            int total = 0;
            if (display != null)
            {
                foreach (var item in display)
                {
                    total += item.Quantity;
                }
                
            }
            return total;   
        }
        public int getTotalQuantityByProductId(int id)
        {
            var lst = repository.GetAllInventory();
            var display = new List<Inventory>();
            int total = 0;
            foreach (var item in lst)
            {
                if (item.IsDeleted == false && item.ProductId == id)
                {
                    total += item.Quantity;
                }
            }
           
            return total;
        }
        public bool AddInventory(Inventory inventory)
        {
            bool success = false;
            if (inventory != null)
            {
                repository.AddInventory(inventory);
                success = true;
                return success;
            }
            return success;
        }

        public bool DeleteInventory(Inventory inventory)
        {
            bool success = false;
            if (inventory != null)
            {
                inventory.IsDeleted = true;
                repository.UpdateInventory(inventory);
                success = true;
                return success;
            }
            return success;
        }

        public bool UpdateInventory(Inventory inventory)
        {
            bool success = false;
            if (inventory != null)
            {
                repository.UpdateInventory(inventory);
                success = true;
                return success;
            }
            return success;
        }
        public List<Inventory> GetInventoryByWarehousesID(int id)
        {
            var lst = repository.GetAllInventory();
            var display = new List<Inventory>();
            foreach (var item in lst)
            {
                if (item.IsDeleted == false && item.WarehouseId == id)
                {
                    display.Add(item);
                }
            }
            return display;
        }
        public List<Inventory> GetInventoryList()
        {
            var lst = repository.GetAllInventory();
            var display = new List<Inventory>();
            foreach (var item in lst)
            {
                if (item.IsDeleted == false)
                {
                    display.Add(item);
                }
            }
            return display;
        }
        public List<Inventory> GetInventoryListBySupplierId(int id)
        {
            var lst = repository.GetAllInventory();
            var display = new List<Inventory>();
            foreach (var item in lst)
            {
                if (item.IsDeleted == false && item.InventoryId == id)
                {
                    display.Add(item);
                }
            }
            return display;
        }

        public List<Inventory> GetInventoryListByWarehouseId(int id)
        {
            var lst = repository.GetAllInventory();
            var display = new List<Inventory>();
            foreach (var item in lst)
            {
                if (item.IsDeleted == false && item.WarehouseId == id )
                {
                    display.Add(item);
                }
            }
            return display;
        }

        public Inventory GetInventoryByProductIdAndWarehouseId(int productId, int warehouseId)
        {
            var lst = repository.GetAllInventory();
            var display = new Inventory();
            foreach (var item in lst)
            {
                if (item.IsDeleted == false && item.WarehouseId == warehouseId && item.ProductId == productId)
                {
                    display = item;
                    break;
                }
            }
            return display;
        }
    }

}
