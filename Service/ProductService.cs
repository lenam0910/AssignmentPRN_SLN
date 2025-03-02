using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Repository;

namespace Service
{
    public class ProductService
    {
        private ProductRepository repository;
        
        public ProductService()
        {
            repository = new ProductRepository();
        }


        public Product GetProductById(int id)
        {
            return repository.Get(id);
        }

        public List<Product> GetAllProducts()
        {
            var lst =repository.GetAll();
            var lstDis = new List<Product>();
            foreach (Product items in lst)
            {
                if (items.IsDeleted == false)
                {
                    lstDis.Add(items);
                }
            }

            return lstDis;
        }

        
        public List<Product> GetAllProductsBySupplierId(int id)
        {
            var lst = repository.GetAll();
            var lstDis = new List<Product>();
            foreach (Product items in lst)
            {
                if (items.IsDeleted == false && items.SupplierId == id)
                {
                    lstDis.Add(items);
                }
            }

            return lstDis;
        }
        

        public bool AddProduct(Product product)
        {
            bool isCheked = false;
            if (product != null)
            {
                isCheked = true;
                repository.Add(product);
                return isCheked;
            }
            return isCheked;
        }

        public bool DeleteProduct(Product product)
        {
            bool isCheked = false;
            if (product != null) { 
                isCheked= true;
                product.IsDeleted = true;
                repository.Update(product);
                return isCheked;

            }
            return isCheked;
        }
        public bool UpdaterProduct(Product product)
        {
            bool isChecked = false;
            if (product != null)
            {
                isChecked = true;
                repository.Update(product);
                return isChecked;
            }
            return isChecked;
        }

    }
}
