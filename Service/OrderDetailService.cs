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
            var lstDpl = new List<OrderDetail>();
            var lst = repository.getAll();
            foreach (var item in lst)
            {
                if (item.IsDeleted == false)
                {
                    lstDpl.Add(item);
                }
            }
            return lstDpl;
        }


     
        public List<OrderDetail> GetAllOrdersByOrderId(int orderId)
        {
            var lstDpl = new List<OrderDetail>();
            var lst = repository.getAllInfor();
            foreach (var item in lst)
            {
                if (item.OrderId == orderId && item.IsDeleted == false)
                {
                    lstDpl.Add(item);
                }
            }
            return lstDpl;
        }
        public OrderDetail GetOrdersDetailByOrderidAndWarehouseID(int warehouseId, int orderId)
        {
            var lst = repository.getAll(); 
            foreach (var item in lst)
            {
                if (item.WarehouseId == warehouseId && item.OrderId == orderId && item.IsDeleted ==false )
                {
                    return item; 
                }
            }
            return null;
        }
        public OrderDetail GetOrdersDetailByOrderidAndWarehouseIDAndProductId(int warehouseId, int orderId,int productId)
        {
            var lst = repository.getAll();
            foreach (var item in lst)
            {
                if (item.WarehouseId == warehouseId && item.OrderId == orderId && item.IsDeleted == false && item.ProductId == productId)
                {
                    return item;
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
