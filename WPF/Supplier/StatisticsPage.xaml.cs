using System;
using System.Linq;
using System.Windows.Controls;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Service;

namespace WPF.Supplier
{
    public partial class StatisticsPage : Page
    {
        private WarehousesService warehousesService;
        private InventoryService inventoryService;

        public StatisticsPage()
        {
            warehousesService = new WarehousesService();
            inventoryService = new InventoryService();
            InitializeComponent();
            LoadStatistics();
        }

        private void LoadStatistics()
        {

            // 1. Tổng số kho hàng
            int totalWarehouses = warehousesService.getAll().Count(w => !w.IsDeleted);
            txtTotalWarehouses.Text = totalWarehouses.ToString();

            // 2. Tổng số lượng sản phẩm trong kho
            int totalProducts = inventoryService.GetInventoryList().Sum(i => i.Quantity);
            txtTotalProducts.Text = totalProducts.ToString();


            
           
  


        }
    }
}
