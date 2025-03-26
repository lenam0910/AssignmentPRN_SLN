
using System.Windows.Controls;

using Service;

namespace WPF.Supplier
{
    public partial class StatisticsPage : Page
    {
        private WarehousesService warehousesService;
        private InventoryService inventoryService;
        private DataAccess.Models.Supplier supplierReal;
        public StatisticsPage(DataAccess.Models.Supplier supplier)
        {  supplierReal = supplier;
            warehousesService = new WarehousesService();
            inventoryService = new InventoryService();
            InitializeComponent();
            LoadStatistics();
        }

        private void LoadStatistics()
        {

            
            int totalWarehouses = warehousesService.getAll().Count(w => !w.IsDeleted && w.SupplierId == supplierReal.SupplierId);
            txtTotalWarehouses.Text = totalWarehouses.ToString();

            
            int totalProducts = inventoryService.GetInventoryList().Where(a => a.SupplierId == supplierReal.SupplierId).Sum(i => i.Quantity);
            txtTotalProducts.Text = totalProducts.ToString();


        }
    }
}
