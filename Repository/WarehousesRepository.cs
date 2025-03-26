using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
   public class WarehousesRepository
    {
        private AssignmentPrnContext _context;
        public WarehousesRepository()
        {
            _context = new AssignmentPrnContext();
        }
        public void Add(Warehouse warehouse)
        {
            _context.Warehouses.Add(warehouse);
            _context.SaveChanges();
        }

        public Warehouse Get(int id) { 
            return _context.Warehouses.FirstOrDefault(x => x.WarehouseId == id);
        }

       

        public void Update(Warehouse warehouse)
        {
            _context.Warehouses.Update(warehouse);
            _context.SaveChanges();
        }

        public List<Warehouse> GetAll()
        {
            return _context.Warehouses.Include(x => x.Inventories).ToList();
        }
    }
}
