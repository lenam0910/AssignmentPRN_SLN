using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
   public class ProductRepository
    {
        private AssignmentPrnContext _prnContext;
        public ProductRepository()
        {
            _prnContext = new AssignmentPrnContext();
        }

        public void Add(Product product)
        {
            _prnContext.Products.Add(product);
            _prnContext.SaveChanges();
        }

        public void Update(Product product)
        {
            _prnContext.Products.Update(product);
            _prnContext.SaveChanges();
        }


        public Product Get(int id)
        {
            return _prnContext.Products.FirstOrDefault(x => x.ProductId == id);
        }
        public List<Product> GetAll()
        {
            return _prnContext.Products.AsNoTracking().ToList();
        }

    }
}
