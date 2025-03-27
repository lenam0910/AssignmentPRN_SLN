using System.Windows;
using System.Windows.Controls;
using DataAccess.Models;
using Service;


namespace WPF.Supplier
{
    public partial class WarehouseManagement : Page
    {
        private readonly WarehousesService warehouseService;
        private readonly ProductService productService;
        private readonly InventoryService inventoryService;
        private readonly DataAccess.Models.User user;
        private DataAccess.Models.Supplier supplier;
        private SupplierService SupplierService;
        public WarehouseManagement()
        {
            user = Application.Current.Properties["UserAccount"] as DataAccess.Models.User;
            InitializeComponent();
            warehouseService = new WarehousesService();
            SupplierService = new();
            productService = new ProductService();
            inventoryService = new InventoryService();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            supplier = SupplierService.GetSupplierByUserId(user.UserId);
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
            var products = new List<Inventory>();
            foreach (var product in inventoryItems)
            {
                products.Add(product);
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
            try
            {
                if (supplier == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin nhà cung cấp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtWarehouseName.Text) || string.IsNullOrWhiteSpace(txtLocation.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin kho hàng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(txtCapacity.Text.Trim(), out int capacity) || capacity <= 0)
                {
                    MessageBox.Show("Sức chứa phải là một số nguyên lớn hơn 0!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Warehouse newWarehouse = new Warehouse
                {
                    WarehouseName = txtWarehouseName.Text.Trim(),
                    Location = txtLocation.Text.Trim(),
                    Capacity = capacity,
                    SupplierId = supplier.SupplierId
                };

                if (warehouseService.AddWarehouses(newWarehouse))
                {
                    MessageBox.Show("Thêm kho hàng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    PopupOverlay.Visibility = Visibility.Collapsed;
                    LoadWarehouses(supplier.SupplierId);
                    clear();
                }
                else
                {
                    MessageBox.Show("Thêm kho hàng thất bại. Vui lòng thử lại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CancelWarehouse_Click(object sender, RoutedEventArgs e)
        {
            PopupOverlay.Visibility = Visibility.Collapsed;
            clear();
        }

        private void SaveEditWarehouse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtEditWarehouseName.Text) || string.IsNullOrWhiteSpace(txtEditLocation.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin kho hàng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                if (!int.TryParse(txtEditCapacity.Text.Trim(), out int capacity) || capacity <= 0)
                {
                    MessageBox.Show("Sức chứa phải là một số nguyên lớn hơn 0!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                if (WarehouseDataGrid.SelectedItem is not Warehouse selectedWarehouse)
                {
                    MessageBox.Show("Hãy chọn kho hàng để sửa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                selectedWarehouse.WarehouseName = txtEditWarehouseName.Text.Trim();
                selectedWarehouse.Location = txtEditLocation.Text.Trim();
                selectedWarehouse.Capacity = capacity;
                selectedWarehouse.IsApproved = false;

                if (warehouseService.UpdateWarehouses(selectedWarehouse))
                {
                    MessageBox.Show("Sửa kho hàng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    EditWarehousePopup.Visibility = Visibility.Collapsed;

                    if (supplier != null)
                    {
                        LoadWarehouses(supplier.SupplierId);
                    }
                }
                else
                {
                    MessageBox.Show("Sửa kho hàng thất bại. Vui lòng thử lại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EditWarehousePopup.Visibility = Visibility.Collapsed;
            WarehouseDataGrid.SelectedItem = null;
            clear();
        }
    }
}
