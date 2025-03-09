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
        public Order GetOrderByUserId(int id)
        {
            var lst = repository.GetAllOrders();
            var display = new Order();
            foreach (var item in lst)
            {
                if (item.IsDeleted == false && item.UserId == id && item.Status.Equals("Chờ xử lý",StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }
            return null;
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

        public List<Order> GetAllOrdersByUserId(int id)
        {
            var lst = repository.GetAllOrders();
            var display = new List<Order>();
            foreach (var item in lst)
            {
                if (item.IsDeleted == false && item.UserId == id && !item.Status.Equals("Chờ xử lý", StringComparison.OrdinalIgnoreCase))
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
