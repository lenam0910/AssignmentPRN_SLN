using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;

namespace Repository
{
    public class UserSupplierRepository
    {

        private AssignmentPrnContext _context;
        private SupplierRepository _supplierRepository;
        public UserSupplierRepository()
        {
            _context = new AssignmentPrnContext();
            _supplierRepository = new SupplierRepository();
        }


        public List<UserSupplier> GetAll()
        {
            return _context.UserSuppliers.ToList();
        }

        public void Add(UserSupplier userSupplier)
        {
            _context.UserSuppliers.Add(userSupplier);
            _context.SaveChanges();
        }
       
    }
}
