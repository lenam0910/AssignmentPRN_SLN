using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Repository;

namespace Service
{
    public class OrderService
    {
        private OrderRepository repository;

        public OrderService()
        {
            repository = new OrderRepository();
        }

        public Order GetOrderById(int id)
        {
            return repository.GetOrder(id);
        }

        public List<Order> GetAllOrders()
        {
            var lst = repository.GetAllOrders();
            var display = new List<Order>();
            foreach (var item in lst)
            {
                if (item.IsDeleted == false)
                {
                    display.Add(item);
                }
            }
            return display;
        }

        public bool addOrder(Order order)
        {
            bool isCheck = false;
            if (order != null)
            {
                isCheck = true;
                repository.AddOrder(order);
                return isCheck;
            }
            return isCheck;
        }

        public bool removeOrder(Order order)
        {
            bool isCheck = false;
            if (order != null)
            {
                isCheck = true;
                order.IsDeleted = true;
                repository.UpdateOrder(order);
                return isCheck;
            }
            return isCheck;
        }

        public bool updateOrder(Order order)
        {
            bool isCheck = false;
            if (order != null)
            {
                isCheck = true;
                repository.UpdateOrder(order);
                return isCheck;
            }
            return isCheck;
        }




    }
}
