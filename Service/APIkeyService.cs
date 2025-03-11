using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Repository;

namespace Service
{
   public class APIkeyService
    {
        private APIkeyRepository apiKeyRepository;
        public APIkeyService()
        {
            apiKeyRepository = new APIkeyRepository();
        }   

        public bool Add(ApiKey apiKey)
        {
            try
            {
                apiKeyRepository.add(apiKey);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(ApiKey apiKey)
        {
            try
            {
                apiKeyRepository.delete(apiKey);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<ApiKey> GetAll()
        {
            return apiKeyRepository.GetAll();
        }

        public ApiKey GetApiNewest()
        {
            var lst = GetAll(); 
            ApiKey key = null;

            if (lst != null && lst.Count > 0)
            {
                key = lst.OrderByDescending(item => item.CreatedAt).FirstOrDefault();
            }

            return key;
        }

    }
}
