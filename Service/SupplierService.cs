using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Repository;

namespace Service
{
    public class SupplierService
    {
        private SupplierRepository supplierRepository;
        private UserSupplierService userSupplierService;
        public SupplierService()
        {
            supplierRepository = new();
            userSupplierService = new UserSupplierService();
        }

        public Supplier GetSupplierById(int id)
        {
            return supplierRepository.GetSupplierById(id);
        }
        public Supplier GetSupplierBySupplierName(string supplierName)
        {
            return supplierRepository.GetSupplierBySupplierName(supplierName);
        }
        public Supplier GetSuppliersByUserId(int userId)
        {
            var lst = userSupplierService.GetSupplierByUserId(userId);
            return lst;
        }
        public List<Supplier> GetAllSuppliers()
        {
            var lst = supplierRepository.GetAllSuppliers();
            var lstDis = new List<Supplier>();
            foreach (Supplier items in lst)
            {
                if (items.IsDeleted == false)
                {
                    lstDis.Add(items);
                }
            }
            return lstDis;
        }

      
        public bool SaveSupplier(Supplier supplier)
        {
            bool isChecked = true;
            if (supplier == null)
            {
                isChecked = false;
                return isChecked;
            }
            else
            {
                supplierRepository.SaveSupplier(supplier);
                return isChecked;
            }

        }

        public bool deleteSupplier(Supplier supplier)
        {
            try
            {
                supplier.IsDeleted = true;
                supplierRepository.UpdateSupplier(supplier);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UpdateSupplier(Supplier supplier)
        {
            try
            {
                supplierRepository.UpdateSupplier(supplier);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
