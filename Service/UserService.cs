using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace Service
{
    public class UserService
    {
        private UserRepository UserRepository;
        private RoleRepository RoleRepository;
        public UserService()
        {
            UserRepository = new UserRepository();
            RoleRepository = new RoleRepository();
        }

        public List<User> getAll()
        {
            var lst = UserRepository.GetAll();
            var display = new List<User>();
            foreach (var item in lst)
            {
                if (item.IsDeleted == false)
                {
                    display.Add(item);
                }
            }
            return display;
        }
        

        public int getId()
        {
            int id = 0;
            foreach (var item in UserRepository.GetAll())
            {
                id = item.UserId;
            }
            id += 1;
            return id;
        }
        public Boolean CreateUser(User u)
        {

            bool check = true;
            if (string.IsNullOrEmpty(u.Username) || string.IsNullOrEmpty(u.Password))
            {
                check = false;
                return check;
            }
            UserRepository.Create(u);
            return check;
        }


        public User GetUserByEmail(string email)
        {
            return UserRepository.GetByEmail(email);
        }
        public Boolean UpdateUser(User u)
        {
            bool check = true;
            if (string.IsNullOrEmpty(u.Username) || string.IsNullOrEmpty(u.Password))
            {
                check = false;
                return check;
            }
            UserRepository.Update(u);
            return check;
        }
        public User Login(string username, string password)
        {
            if (username == null || password == null)
            {
                return null;
            }
            var u = UserRepository.GetUserByNameAndPassword(username, password);
            if (u != null && u.IsDeleted != true)
            {
                return u;
            }
            return null;
        }


        public bool checkDuplicateUserName(string username)
        {
            var u = UserRepository.checkDuplicateUserName(username);
            if (u == null)
            {
                return false;
            }
            return true;
        }
        public bool checkExistedGmail(string mail)
        {
            var u = UserRepository.GetAll();
            foreach (User user in u)
            {
                if (user.Email.ToLower().Equals(mail))
                {
                    return true;
                }
            }

            return false;
        }

        public Role GetRole(string roleName)
        {
            var roles = RoleRepository.GetAll();
            return roles.FirstOrDefault(r => r.RoleName == roleName);

        }
        public bool DeleteUser(User u)
        {
            bool isDeleted = true;
            if (u == null)
            {
              
                return isDeleted;
            }
            else
            {
                u.IsDeleted = true;
                UpdateUser(u);
                return isDeleted;
            }
        }
    }
}
