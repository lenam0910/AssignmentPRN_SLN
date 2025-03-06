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
        public List<OrderDetail> GetAllOrdersByOrderId(int orderId)
        {
            var lstDpl = new List<OrderDetail>();
            var lst = repository.getAll();
            foreach (var item in lst)
            {
                if (item.OrderId == orderId)
                {
                    lstDpl.Add(item);
                }
            }
            return lstDpl;
        }
        public OrderDetail GetOrdersDetailByProductId(int productId)
        {
            var lstDpl = new OrderDetail();
            var lst = repository.getAll();
            foreach (var item in lst)
            {
                if (item.ProductId == productId)
                {
                    return lstDpl;
                }
            }
            return null; 
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
