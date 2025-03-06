using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Repository;

namespace Service
{
    public class UserSupplierService
    {
        private UserSupplierRepository repository;
        private SupplierRepository _supplierRepository;

        public UserSupplierService()
        {
            repository = new UserSupplierRepository();
            _supplierRepository = new SupplierRepository();
        }

        public Supplier GetSupplierByUserId(int id)
        {
            var lst = repository.GetAll();
            var lstSupplier = _supplierRepository.GetAllSuppliers();
            foreach (Supplier item in lstSupplier)
            {
                if (item.IsDeleted == true)
                {
                    lstSupplier.Remove(item);
                }
            }
            var supplier = new Supplier();
            int suppplierId = 0;
            foreach (var item in lst)
            {
                if (item.UserId == id)
                {
                    suppplierId = item.SupplierId;
                }
            }
            foreach (var item in lstSupplier)
            {
                if (item.SupplierId == suppplierId)
                {
                    if(item.IsApproved == true)
                    {
                        return item;
                    }
                }
            }
            return null;
        }
        public Supplier GetSupplierByUserIdForLogin(int id)
        {
            var lst = repository.GetAll();
            var lstSupplier = _supplierRepository.GetAllSuppliers();
            foreach (Supplier item in lstSupplier)
            {
                if (item.IsDeleted == true)
                {
                    lstSupplier.Remove(item);
                }
            }
            var supplier = new Supplier();
            int suppplierId = 0;
            foreach (var item in lst)
            {
                if (item.UserId == id)
                {
                    suppplierId = item.SupplierId;
                }
            }
            foreach (var item in lstSupplier)
            {
                if (item.SupplierId == suppplierId)
                {

                        return item;
                    
                }
            }
            return null;
        }

        public void Add(UserSupplier userSupplier)
        {
            repository.Add(userSupplier);
        }
    }
}
