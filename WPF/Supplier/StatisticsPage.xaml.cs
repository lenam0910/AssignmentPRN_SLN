
using System.Windows.Controls;

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

            
            int totalWarehouses = warehousesService.getAll().Count(w => !w.IsDeleted);
            txtTotalWarehouses.Text = totalWarehouses.ToString();

            
            int totalProducts = inventoryService.GetInventoryList().Sum(i => i.Quantity);
            txtTotalProducts.Text = totalProducts.ToString();


        }
    }
}
