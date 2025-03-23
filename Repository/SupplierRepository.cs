using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class SupplierRepository
    {
        private AssignmentPrnContext _context;

        public SupplierRepository()
        {
            _context = new AssignmentPrnContext();
        }
        public Supplier GetSupplierByIdUser(int id)
        {
            return _context.Suppliers.AsNoTracking().Include(x => x.Users).FirstOrDefault(x => x.Users.Any(x => x.UserId == id));
        }

        public Supplier GetSupplierById(int id)
        {
            return _context.Suppliers.FirstOrDefault(s => s.SupplierId == id);
        }
        public Supplier GetSupplierBySupplierName(string name)
        {
            return _context.Suppliers.FirstOrDefault(s => s.SupplierName == name);
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
