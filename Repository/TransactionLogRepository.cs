using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class TransactionLogRepository
    {
        private AssignmentPrnContext _prnContext;

        public TransactionLogRepository()
        {
            _prnContext = new AssignmentPrnContext();
        }

        public TransactionLog GetTransactionLog(TransactionLog transactionLog)
        {
            return _prnContext.TransactionLogs.FirstOrDefault(x => x.TransactionId == transactionLog.TransactionId);
        }

        public List<TransactionLog> GetAll()
        {
            return _prnContext.TransactionLogs.Include(x=>x.Product).Include(y => y.Warehouse).Include(z => z.Supplier).Include(a => a.User).AsNoTracking().ToList();  
        }
        public void Add(TransactionLog transactionLog)
        {
            _prnContext.TransactionLogs.Add(transactionLog);
            _prnContext.SaveChanges();
        }

      

        public void Update(TransactionLog transactionLog)
        {
            _prnContext.TransactionLogs.Update(transactionLog);
            _prnContext.SaveChanges();
        }

    }
}
