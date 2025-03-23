using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
   public class InventoryRepository
    {
        private AssignmentPrnContext prnContext;

        public InventoryRepository()
        {
            prnContext = new AssignmentPrnContext();
        }

        public void AddInventory(Inventory inventory)
        {
            prnContext.Inventories.Add(inventory);
            prnContext.SaveChanges();
        }

        public Inventory GetInventory(int id)
        {
            return prnContext.Inventories.FirstOrDefault(x => x.InventoryId == id);
        }

        public void UpdateInventory(Inventory inventory)
        {
            prnContext.Inventories.Update(inventory);
            prnContext.SaveChanges();
        }
       
        public List<Inventory> GetAllInventory()
        {
            return prnContext.Inventories.Include(x => x.Product).Include(x => x.Warehouse).Include(x => x.Supplier).AsNoTracking().ToList();
        }

    }
}
