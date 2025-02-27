using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Repository;

namespace Service
{
    public class RoleService
    {
        private RoleRepository roleRepository;
        public RoleService()
        {
            roleRepository = new RoleRepository();
        }
        public List<Role> GetAll()
        {
            return roleRepository.GetAll();
        }
    }
}
