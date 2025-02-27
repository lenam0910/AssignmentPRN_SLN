using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Repository;

namespace Service
{
    public class OrderDetailService
    {
        private OrderDetailRepository repository;
        public OrderDetailService()
        {
            repository = new OrderDetailRepository();
        }

        public OrderDetail GetOrderDetail(OrderDetail orderDetail)
        {
            return repository.get(orderDetail.OrderDetailId);
        }

        public List<OrderDetail> GetAllOrders()
        {
            return repository.getAll();
        }

        public bool AddOrderDetail(OrderDetail orderDetail)
        {
            bool isCheck = false;
            if (orderDetail != null)
            {
                isCheck = true;
                repository.add(orderDetail);
                return isCheck;
            }

            return isCheck;
        }

       

        public bool UpdateOrderDetail(OrderDetail orderDetail)
        {
            bool isCheck = false;
            if(orderDetail != null)
            {
                isCheck = true;
                repository.update(orderDetail);
                return isCheck;
            }
            return isCheck;
        }
    }
}
