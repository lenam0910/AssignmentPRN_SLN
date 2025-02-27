using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;

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
            return _prnContext.TransactionLogs.ToList();
        }
        public void Add(TransactionLog transactionLog)
        {
            _prnContext.Add(transactionLog);
            _prnContext.SaveChanges();
        }

      

        public void Update(TransactionLog transactionLog)
        {
            _prnContext.Update(transactionLog);
            _prnContext.SaveChanges();
        }

    }
}
