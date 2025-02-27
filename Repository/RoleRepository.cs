using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;

namespace Repository
{
    public class RoleRepository
    {
        private AssignmentPrnContext _context;

        public RoleRepository()
        {
            _context = new AssignmentPrnContext();
        }

        public List<Role> GetAll()
        {
            return _context.Roles.ToList();
        }
        
    }
}
