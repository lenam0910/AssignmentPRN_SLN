using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;

namespace Repository
{
    public class APIkeyRepository
    {
        private AssignmentPrnContext context;

        public APIkeyRepository()
        {
            context = new AssignmentPrnContext();
        }

        public void add(ApiKey apiKey)
        {
            context.ApiKeys.Add(apiKey);
            context.SaveChanges();
        }
        public void delete(ApiKey apiKey)
        {
            context.ApiKeys.Remove(apiKey);
            context.SaveChanges();
        }
        public List<ApiKey> GetAll()
        {
            return context.ApiKeys.ToList();
        }
    }
}
