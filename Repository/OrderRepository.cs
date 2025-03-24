using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class OrderRepository
    {
        private AssignmentPrnContext _prnContext;

        public OrderRepository()
        {
            _prnContext = new AssignmentPrnContext();
        }

        public void AddOrder(Order order)
        {
            _prnContext.Orders.Add(order);
            _prnContext.SaveChanges();
        }

        

        public Order GetOrder(int id) {
            return _prnContext.Orders.FirstOrDefault(x => x.OrderId == id);
        }

        public List<Order> GetAllOrders()
        {
            return _prnContext.Orders.Include(x=> x.OrderDetails).AsNoTracking().ToList();
        }

        public List<Order> GetAllOrdersInfor()
        {
            return _prnContext.Orders.Include(x => x.OrderDetails).ToList();
        }

        public void UpdateOrder(Order order) { 
            _prnContext.Orders.Update(order);
            _prnContext.SaveChanges();
        }
    }
}
