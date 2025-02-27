using System.Windows;
using System.Windows.Controls;
using DataAccess.Models;
using Service;

namespace WPF.Supplier
{
    public partial class WarehouseManagement : Page
    {
        private WarehousesService warehouseService;
        public User user = null;
        public WarehouseManagement()
        {
            InitializeComponent();
            warehouseService = new WarehousesService();
            user = Application.Current.Properties["UserAccount"] as User;
        }

        private void LoadWarehouses(int id)
        {
            WarehouseDataGrid.ItemsSource = warehouseService.getAllBySupplierId(id);
        }

        private void AddWarehouse_Click(object sender, RoutedEventArgs e)
        {
            PopupOverlay.Visibility = Visibility.Visible;
        }

        private void EditWarehouse_Click(object sender, RoutedEventArgs e)
        {
            if (WarehouseDataGrid.SelectedItem is Warehouse selectedWarehouse)
            {
                EditWarehousePopup.Visibility = Visibility.Visible;
                txtEditWarehouseName.Text = selectedWarehouse.WarehouseName;
                txtEditLocation.Text = selectedWarehouse.Location;
                txtEditCapacity.Text = selectedWarehouse.Capacity.ToString();
            }
            else
            {
                MessageBox.Show("Hãy chọn kho hàng để sửa!");
            }
        }

        private void DeleteWarehouse_Click(object sender, RoutedEventArgs e)
        {
            if (WarehouseDataGrid.SelectedItem is Warehouse selectedWarehouse)
            {
                if (warehouseService.DeleteWarehouses(selectedWarehouse))
                {
                    MessageBox.Show("Xoá kho hàng thành công!");
                    LoadWarehouses(user.SupplierId.Value);
                }
                else
                {
                    MessageBox.Show("Xoá kho hàng thất bại!");
                }
            }
            else
            {
                MessageBox.Show("Hãy chọn kho hàng để xoá!");
            }
        }

        private void ViewWarehouse_Click(object sender, RoutedEventArgs e)
        {
            if (WarehouseDataGrid.SelectedItem is Warehouse selectedWarehouse)
            {
                ViewWarehousePopup.Visibility = Visibility.Visible;
                txtViewWarehouseName.Text = selectedWarehouse.WarehouseName;
                txtViewLocation.Text = selectedWarehouse.Location;
                txtViewCapacity.Text = selectedWarehouse.Capacity.ToString();
            }
            else
            {
                MessageBox.Show("Hãy chọn kho hàng để xem!");
            }
        }

        private void CloseViewWarehousePopup_Click(object sender, RoutedEventArgs e)
        {
            ViewWarehousePopup.Visibility = Visibility.Collapsed;
        }

        private void SaveWarehouse_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtCapacity.Text, out int capacity) || capacity <= 0)
            {
                MessageBox.Show("Hãy nhập số lượng >0!");
                return;
            }

            Warehouse newWarehouse = new Warehouse
            {
                WarehouseName = txtWarehouseName.Text,
                Location = txtLocation.Text,
                Capacity = capacity,
                SupplierId = user.SupplierId.Value
            };

            if (warehouseService.AddWarehouses(newWarehouse))
            {
                MessageBox.Show("Thêm kho hàng thành công!");
                PopupOverlay.Visibility = Visibility.Collapsed;
                LoadWarehouses(user.SupplierId.Value);
            }
            else
            {
                MessageBox.Show("Thêm kho hàng thất bại!");
            }
        }

        private void CancelWarehouse_Click(object sender, RoutedEventArgs e)
        {
            PopupOverlay.Visibility = Visibility.Collapsed;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadWarehouses(user.SupplierId.Value);
        }

        private void CancelEditWarehouse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveEditWarehouse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void WarehouseDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
