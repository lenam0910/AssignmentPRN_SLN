using System.Windows;
using System.Windows.Controls;
using DataAccess.Models;
using Service;
using System.Linq;
using System.Collections.Generic;

namespace WPF.Supplier
{
    public partial class WarehouseManagement : Page
    {
        private readonly WarehousesService warehouseService;
        private readonly ProductService productService;
        private readonly InventoryService inventoryService;
        private readonly User user;
        private UserSupplierService userSupplierService;
        private DataAccess.Models.Supplier supplier;
        public WarehouseManagement()
        {
            userSupplierService = new UserSupplierService();
            InitializeComponent();
            warehouseService = new WarehousesService();
            productService = new ProductService();
            inventoryService = new InventoryService();
            user = Application.Current.Properties["UserAccount"] as User;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            supplier = userSupplierService.GetSupplierByUserId(user.UserId);
            if (supplier.SupplierId != null)
            {
                LoadWarehouses(supplier.SupplierId);
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin nhà cung cấp.");
            }
        }

        private void LoadWarehouses(int supplierId)
        {
            WarehouseDataGrid.ItemsSource = warehouseService.getAllBySupplierId(supplierId);
        }

        private void WarehouseDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WarehouseDataGrid.SelectedItem is Warehouse selectedWarehouse)
            {
                LoadProducts(selectedWarehouse.WarehouseId);
            }
        }

        private void LoadProducts(int warehouseId)
        {
            var inventoryItems = inventoryService.GetInventoryListByWarehouseId(warehouseId);
            var lstProduct = productService.GetAllProducts();
            var products = new List<Product>();
            foreach (var product in inventoryItems)
            {
                foreach (var item in lstProduct)
                {
                    if (product.ProductId == item.ProductId)
                    {
                        products.Add(item);
                    }
                }
            }

            ProductListBox.ItemsSource = products;
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
                var inventoryItems = inventoryService.GetInventoryListByWarehouseId(selectedWarehouse.WarehouseId);
                if (inventoryItems.Any())
                {
                    MessageBox.Show("Không thể xóa kho hàng vì vẫn còn sản phẩm tồn kho.");
                    return;
                }

                var result = MessageBox.Show($"Bạn có chắc muốn xóa kho hàng: {selectedWarehouse.WarehouseName}?", "Xác nhận", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    if (warehouseService.DeleteWarehouses(selectedWarehouse))
                    {
                        MessageBox.Show("Xoá kho hàng thành công!");
                        LoadWarehouses(supplier.SupplierId);
                    }
                    else
                    {
                        MessageBox.Show("Xoá kho hàng thất bại!");
                    }
                }
            }
            else
            {
                MessageBox.Show("Hãy chọn kho hàng để xoá!");
            }
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
                SupplierId = supplier.SupplierId
            };

            if (warehouseService.AddWarehouses(newWarehouse))
            {
                MessageBox.Show("Thêm kho hàng thành công!");
                PopupOverlay.Visibility = Visibility.Collapsed;
                LoadWarehouses(supplier.SupplierId);
                clear();
            }
            else
            {
                MessageBox.Show("Thêm kho hàng thất bại!");
            }
        }

        private void CancelWarehouse_Click(object sender, RoutedEventArgs e)
        {
            PopupOverlay.Visibility = Visibility.Collapsed;
            clear();
        }

        private void SaveEditWarehouse_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtEditCapacity.Text, out int capacity) || capacity <= 0)
            {
                MessageBox.Show("Hãy nhập số lượng >0!");
                return;
            }

            if (WarehouseDataGrid.SelectedItem is Warehouse selectedWarehouse)
            {
                selectedWarehouse.WarehouseName = txtEditWarehouseName.Text;
                selectedWarehouse.Location = txtEditLocation.Text;
                selectedWarehouse.Capacity = capacity;

                if (warehouseService.UpdateWarehouses(selectedWarehouse))
                {
                    MessageBox.Show("Sửa kho hàng thành công!");
                    EditWarehousePopup.Visibility = Visibility.Collapsed;
                    LoadWarehouses(supplier.SupplierId);  
                }
                else
                {
                    MessageBox.Show("Sửa kho hàng thất bại!");
                }
            }
            else
            {
                MessageBox.Show("Hãy chọn kho hàng để sửa!");
            }
        }

        private void CancelEditWarehouse_Click(object sender, RoutedEventArgs e)
        {
            EditWarehousePopup.Visibility = Visibility.Collapsed;
            WarehouseDataGrid.SelectedItem = null;
        }

        private void clear()
        {
            txtWarehouseName.Clear();
            txtLocation.Clear();
            txtCapacity.Clear();
        }
    }
}
