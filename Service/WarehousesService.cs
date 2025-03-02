using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Repository;

namespace Service
{
    public class WarehousesService
    {
        private WarehousesRepository repository;
        public WarehousesService()
        {
            repository = new WarehousesRepository();
        }

        public bool AddWarehouses(Warehouse warehouse)
        {
            bool result = false;
            if (warehouse != null)
            {
                repository.Add(warehouse);
                result = true;
                return result;
            }
            return result;
        }

        public bool UpdateWarehouses(Warehouse warehouse)
        {
            bool result = false;
            if (warehouse != null)
            {
                repository.Update(warehouse);
                result = true;
                return result;
            }
            return result;
        }

        public bool DeleteWarehouses(Warehouse warehouse)
        {
            bool result = false;
            if (warehouse != null)
            {
                warehouse.IsDeleted = true;
                repository.Update(warehouse);
                result = true;
                return result;
            }
            return result;
        }

        public List<Warehouse> GetAllWarehousesByIdSupplier(int id) {
            var list = repository.GetAll();
            List<Warehouse> listWarehouses = new List<Warehouse>();
            foreach (var item in list) { 
                if(item.SupplierId == id && item.IsDeleted == false)
                {
                    listWarehouses.Add(item);
                }
            }
            return listWarehouses;
        }

        public List<Warehouse> getAll()
        {
            var lst = repository.GetAll();
            var lstDis = new List<Warehouse>();
            foreach (Warehouse items in lst)
            {
                if (items.IsDeleted == false)
                {
                    lstDis.Add(items);
                }
            }
            return lstDis;
        }
        public List<Warehouse> getAllBySupplierId(int id)
        {
            var lst = repository.GetAll();
            var lstDis = new List<Warehouse>();
            foreach (Warehouse items in lst)
            {
                if (items.IsDeleted == false && items.SupplierId == id)
                {
                    lstDis.Add(items);
                }
            }
            return lstDis;
        }
        
        public Warehouse GetWarehouseById(int id) { 
            return repository.Get(id);
        }
    }
}
