using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;

namespace Repository
{
    public class UserRepository
    {
        private AssignmentPrnContext _context;


       
        public UserRepository()
        {
            _context = new AssignmentPrnContext();
        }
        public User GetUserByNameAndPassword(string username, string password)
        {
            return _context.Users.FirstOrDefault(x => x.Username == username && x.Password == password);
        }

        

        public User checkDuplicateUserName(string username)
        {
            return _context.Users.FirstOrDefault(x => x.Username == username);
        }
        public List<User> GetAll()
        {
            var lst = _context.Users.ToList();
            var newLst = new List<User>();
            foreach(User user in lst)
            {
                if(user.IsDeleted == false)
                {
                    newLst.Add(user);
                }
            }
            return newLst ;
        }

        public void Create(User u)
        {
            _context.Users.Add(u);
            _context.SaveChanges();
        }
        
       
       
        public User GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
        }

        public User getbyid(int id)
        {
            return _context.Users.FirstOrDefault(x => x.UserId == id);
        }
        public bool Update(User u)
        {
            bool isUpdated = true;
            if(u != null)
            {
                _context.Users.Update(u);
                _context.SaveChanges();
            }
            else
            {
                isUpdated = false;
                return isUpdated;
            }
            return isUpdated;
        }
    }
}
