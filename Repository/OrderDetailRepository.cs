using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;

namespace Repository
{
    public class OrderDetailRepository
    {
        private AssignmentPrnContext _prnContext;
        public OrderDetailRepository()
        {
            _prnContext = new AssignmentPrnContext();
        }

        public List<OrderDetail> getAll()
        {
            return _prnContext.OrderDetails.ToList();
        }

        public OrderDetail get(int id)
        {
            return _prnContext.OrderDetails.FirstOrDefault(x => x.OrderDetailId == id);
        }

        

        public void update(OrderDetail orderDetail)
        {
            _prnContext.OrderDetails.Update(orderDetail);
            _prnContext.SaveChanges();
        }

        public void add(OrderDetail orderDetail)
        {
            _prnContext.OrderDetails.Add(orderDetail);
            _prnContext.SaveChanges();
        }
    }
}
