using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Repository;

namespace Service
{
    public class CategoryService
    {
        private CategoryRepository repository;

        public CategoryService()
        {
            repository = new CategoryRepository();
        }

        public List<Category> getAll()
        {
            var lst = repository.GetAllSuppliers();
            var display = new List<Category>();
            foreach (var item in lst)
            {
                if (item.IsDeleted == false)
                {
                    display.Add(item);
                }
            }
            return display;
        }
      
        public bool deleteCategorias(Category category)
        {
            bool isDeleted = true;
            if (category == null)
            {
                isDeleted = false;
                return isDeleted;
            }
            else
            {
                category.IsDeleted = true;
                repository.UpdateCategories(category);
                return isDeleted;
            }
        }

        public bool addCategorias(Category category)
        {
            bool isChecked = true;
            if (category == null)
            {
                isChecked = false;
                return isChecked;
            }
            else
            {
                repository.SaveCategories(category);
                return isChecked;
            }

        }
        public bool updateCategorias(Category category)
        {
            bool isChecked = true;
            if(category == null)
            {
                isChecked = false;
                return isChecked;
            }
            else
            {
                repository.UpdateCategories(category);
                return isChecked;
            }
        }
    }
}
