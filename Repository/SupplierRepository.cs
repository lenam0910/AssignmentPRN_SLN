using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;

namespace Repository
{
    public class SupplierRepository
    {
        private AssignmentPrnContext _context;

        public SupplierRepository()
        {
            _context = new AssignmentPrnContext();
        }


        public Supplier GetSupplierById(int id)
        {
            return _context.Suppliers.FirstOrDefault(s => s.SupplierId == id);
        }

        public List<Supplier> GetAllSuppliers()
        {
            return _context.Suppliers.ToList();
        }

        public void SaveSupplier(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            _context.SaveChanges();
        }
     
        public void UpdateSupplier(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            _context.SaveChanges();
        }

    }
}
