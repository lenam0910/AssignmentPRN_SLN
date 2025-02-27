using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Repository;

namespace Service
{
    public class TransactionLogService
    {
        private TransactionLogRepository repository;

        public TransactionLogService()
        {
            repository = new TransactionLogRepository();
        }

        public TransactionLog GetTransactionLogById(TransactionLog transactionId)
        {
            return repository.GetTransactionLog(transactionId);
        }

        public List<TransactionLog> GetAllTransactions()
        {
            return repository.GetAll();
        }

        public bool AddTransactionLog(TransactionLog transactionLog)
        {
            bool isChecked = false;
            if (transactionLog != null)
            {
                isChecked = true;
                repository.Add(transactionLog);
                return isChecked;
            }
            return isChecked;
        }
    
        public bool UpgradeTransactionLog(TransactionLog transactionLog)
        {
            bool isChecked = false;
            if (transactionLog != null)
            {
                isChecked = true;
                repository.Update(transactionLog);
                return !isChecked;
            }
            return isChecked;
        }
    }
}
