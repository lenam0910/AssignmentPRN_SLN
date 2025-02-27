using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class CategoryRepository
    {
        private AssignmentPrnContext _context;

        public CategoryRepository()
        {
            _context = new AssignmentPrnContext();
        }

        public Category GetSupplierById(int id)
        {
            return _context.Categories.FirstOrDefault(s => s.CategoryId == id);
        }

        public List<Category> GetAllSuppliers()
        {
            return _context.Categories.ToList();
        }

        public void SaveCategories(Category Category)
        {
            _context.Categories.Add(Category);
            _context.SaveChanges();
        }
      
        public void UpdateCategories(Category Category)
        {
            _context.Categories.Update(Category);
            _context.SaveChanges();
        }
    }
}
