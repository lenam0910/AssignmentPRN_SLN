
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DataAccess.Models;
using Service;

namespace WPF.Supplier
{
    /// <summary>
    /// Interaction logic for InventoryManagement.xaml
    /// </summary>
    public partial class InventoryManagement : Page
    {
        private InventoryService inventoryService;
        private WarehousesService warehousesService;
        private ProductService productService;
        private DataAccess.Models.User user;
        private TransactionLogService transactionLogService;
        private Warehouse warehouse;
        private Product product;
        private UserSupplierService userSupplierService;
        private DataAccess.Models.Supplier supplier;
        private UserService userService;
        private SupplierService supplierService;

        public InventoryManagement()
        {
            inventoryService = new InventoryService();
            warehousesService = new WarehousesService();
            productService = new ProductService();
            userSupplierService = new UserSupplierService();
            supplierService = new();
            transactionLogService = new TransactionLogService();
            userService = new();
            InitializeComponent();
            user = Application.Current.Properties["UserAccount"] as DataAccess.Models.User;
            LoadData();

        }

        // Load Warehouse List and Inventory Data
        private void LoadData()
        {

            supplier = userSupplierService.GetSupplierByUserId(user.UserId);
            WarehouseComboBox.ItemsSource = warehousesService.GetAllWarehousesByIdSupplier(supplier.SupplierId);
            WarehouseComboBox.DisplayMemberPath = "WarehouseName";
            WarehouseComboBox.SelectedValuePath = "WarehouseId";



        }
        private void loadInvetory(int warehouseId)
        {
            var lst = inventoryService.GetInventoryListByWarehouseId(warehouseId);
            var lstProduct = productService.GetAllProductsBySupplierId(supplier.SupplierId);

            foreach (var item in lst)
            {
                foreach (var itemProduct in lstProduct)
                {
                    if (item.ProductId == itemProduct.ProductId)
                    {
                        item.Product = itemProduct;
                    }
                }
            }

            InventoryDataGrid.ItemsSource = lst;
        }




        private void WarehouseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WarehouseComboBox.SelectedValue != null)
            {
                var selectedWarehouse = (int)WarehouseComboBox.SelectedValue;
                loadInvetory(selectedWarehouse);
                txtProductName.ItemsSource = productService.GetAllProductsBySupplierId(supplier.SupplierId);
                txtProductName.DisplayMemberPath = "ProductName";
                txtProductName.SelectedValuePath = "ProductId";
                WarehouseComboBox.IsEnabled = false;
            }
            else
            {
                txtProductName.ItemsSource = null;
            }

        }


        private void SaveInventory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra xem kho hàng đã được chọn chưa
                warehouse = WarehouseComboBox.SelectedItem as Warehouse;
                if (warehouse == null)
                {
                    MessageBox.Show("Hãy chọn Kho hàng đầu tiên!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

  
                product = txtProductName.SelectedItem as Product;
                if (product == null)
                {
                    MessageBox.Show("Hãy chọn sản phẩm để thêm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra số lượng nhập hợp lệ
                if (!int.TryParse(txtQuantity.Text.Trim(), out int quantityToAdd) || quantityToAdd <= 0)
                {
                    MessageBox.Show("Số lượng phải là một số nguyên lớn hơn 0!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra tình trạng kho
                int warehouseCapacity = warehouse.Capacity ?? 0;
                int currentInventory = inventoryService.getTotalQuantityInventoyByID(warehouse.WarehouseId);

                if (currentInventory + quantityToAdd > warehouseCapacity)
                {
                    MessageBox.Show("Kho hàng không đủ sức chứa số lượng sản phẩm này!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (quantityToAdd > product.QuantityInStock)
                {
                    MessageBox.Show("Số lượng sản phẩm trong kho không đủ để thêm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra tình trạng sản phẩm
                if (txtStockStatus.SelectedItem is not ComboBoxItem selectedItem)
                {
                    MessageBox.Show("Hãy chọn tình trạng tồn kho!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Cập nhật kho hàng
                warehouse.Capacity -= quantityToAdd;
                if (!warehousesService.UpdateWarehouses(warehouse))
                {
                    MessageBox.Show("Lỗi khi cập nhật kho hàng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Cập nhật số lượng sản phẩm
                product.QuantityInStock -= quantityToAdd;
                if (!productService.UpdaterProduct(product))
                {
                    MessageBox.Show("Lỗi khi cập nhật sản phẩm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Kiểm tra xem sản phẩm đã tồn tại trong kho chưa
                var existingInventory = inventoryService.GetInventoryListByWarehouseId(warehouse.WarehouseId)
                                                        .FirstOrDefault(item => item.ProductId == product.ProductId);

                if (existingInventory != null)
                {
                    // Nếu sản phẩm đã tồn tại, cập nhật số lượng
                    existingInventory.Quantity += quantityToAdd;
                    if (!inventoryService.UpdateInventory(existingInventory))
                    {
                        MessageBox.Show("Lỗi khi cập nhật kho hàng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    // Nếu sản phẩm chưa có, thêm mới vào kho
                    var newInventory = new Inventory
                    {
                        ProductId = product.ProductId,
                        WarehouseId = warehouse.WarehouseId,
                        SupplierId = warehouse.SupplierId,
                        Quantity = quantityToAdd,
                        StockStatus = selectedItem.Content.ToString()
                    };

                    if (!inventoryService.AddInventory(newInventory))
                    {
                        MessageBox.Show("Lỗi khi thêm sản phẩm vào kho!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                // Ghi nhật ký giao dịch
                var transactionLog = new TransactionLog
                {
                    ProductId = product.ProductId,
                    WarehouseId = warehouse.WarehouseId,
                    SupplierId = warehouse.SupplierId,
                    ChangeType = "Thêm",
                    QuantityChanged = quantityToAdd,
                    ChangeDate = DateTime.Now,
                    UserId = user?.UserId ?? 0,
                    Remarks = txtNote.Text?.Trim()
                };

                if (!transactionLogService.AddTransactionLog(transactionLog))
                {
                    MessageBox.Show("Lỗi khi ghi nhật ký giao dịch!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Thông báo thành công
                MessageBox.Show("Thêm sản phẩm vào kho thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                // Cập nhật giao diện
                loadInvetory(warehouse.WarehouseId);
                clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Hủy chỉnh sửa
        private void CancelEdit_Click(object sender, RoutedEventArgs e)
        {
            WarehouseComboBox.SelectedValue = null;
            InventoryDataGrid.ItemsSource = null;
            clear();
            ExportBtn.Visibility = Visibility.Collapsed;
            SaveBtn.Visibility = Visibility.Visible;
            OtherWareHouse.Visibility = Visibility.Collapsed;
            ProductStack.Visibility = Visibility.Collapsed;
            status.Visibility = Visibility.Visible;
            txtStockStatus.SelectedValue = null;
        }
        private void clear()
        {
            product = null;
            warehouse = null;
            txtProductName.SelectedValue = null;
            txtQuantity.Clear();
            txtStockStatus.SelectedValue = null;
            txtRemain.Text = "";
            txtProductName.IsEnabled = true;
            txtStockStatus.IsEnabled = true;
            WarehouseComboBox.IsEnabled = true; 

            InventoryDataGrid.ItemsSource = null;
            txtRemain.Text = "";
            txtOtherWarehouse.IsEnabled = true;
            txtOtherWarehouse.SelectedValue = null;

        }
        // Sửa kho hàng
        private void EditInventory_Click(object sender, RoutedEventArgs e)
        {
            var selectedInventory = (Inventory)InventoryDataGrid.SelectedItem;
            if (selectedInventory != null)
            {
                EditBtn.Visibility = Visibility.Visible;
                SaveBtn.Visibility = Visibility.Collapsed;
                txtProductName.Text = selectedInventory.Product.ProductName;
                Product p = productService.GetProductById(selectedInventory.Product.ProductId);
                txtRemain.Text = "(Còn lại: " + (selectedInventory.Quantity + p.QuantityInStock) + ")";
                int oldQuantity = selectedInventory.Quantity;
                txtProductName.SelectedValue = selectedInventory.Product.ProductId;
                txtStockStatus.Text = selectedInventory.StockStatus;
                txtQuantity.Text = selectedInventory.Quantity.ToString();


                loadInvetory(selectedInventory.WarehouseId);


            }
        }

        // Xóa kho hàng
        private void DeleteInventory_Click(object sender, RoutedEventArgs e)
        {
            var selectedInventory = (Inventory)InventoryDataGrid.SelectedItem;
            if (selectedInventory != null)
            {
                warehouse = (Warehouse)WarehouseComboBox.SelectedItem;

                if (inventoryService.DeleteInventory(selectedInventory))
                {
                    int oldQuantity = selectedInventory.Quantity;
                    Product p = productService.GetProductById(selectedInventory.Product.ProductId);
                    p.QuantityInStock += oldQuantity;
                    if (productService.UpdaterProduct(p))
                    {
                        warehouse.Capacity += oldQuantity;
                        if (warehousesService.UpdateWarehouses(warehouse))
                        {
                            var transactionLog = new TransactionLog
                            {
                                ProductId = selectedInventory.Product.ProductId,
                                WarehouseId = selectedInventory.WarehouseId,
                                SupplierId = selectedInventory.SupplierId,
                                ChangeType = "Xóa",
                                QuantityChanged = -selectedInventory.Quantity,
                                ChangeDate = DateTime.Now,
                                UserId = user.UserId,
                                Remarks = "Xóa kho hàng"
                            };
                            if (transactionLogService.AddTransactionLog(transactionLog))
                            {
                                var selectedWarehouse = (int)WarehouseComboBox.SelectedValue;
                                if (selectedWarehouse != null)
                                {
                                    clear();
                                    loadInvetory(selectedWarehouse);
                                }
                                MessageBox.Show("Đã xóa kho hàng.");
                            }
                        }

                    }

                }
            }
        }

        private void txtProductName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedProduct = txtProductName.SelectedItem as Product;
            if (selectedProduct != null)
            {
                txtProductName.IsEnabled = false;
                txtRemain.Text = "(Còn lại: " + selectedProduct.QuantityInStock + ")";
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra xem mục đã chọn trong DataGrid có tồn tại không
                if (InventoryDataGrid.SelectedItem is not Inventory selectedInventory)
                {
                    MessageBox.Show("Hãy chọn một sản phẩm trong kho để sửa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(txtProductName.Text) ||
                    string.IsNullOrWhiteSpace(txtStockStatus.Text) ||
                    !int.TryParse(txtQuantity.Text.Trim(), out int newQuantity) || newQuantity <= 0)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin và số lượng hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Lấy thông tin sản phẩm từ database
                Product p = productService.GetProductById(selectedInventory.Product.ProductId);
                if (p == null)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Lấy sức chứa kho hàng
                int warehouseCapacity = warehouse.Capacity ?? 0;
                int oldQuantity = selectedInventory.Quantity;
                int totalAvailableStock = oldQuantity + p.QuantityInStock.Value; // Số lượng sản phẩm có thể chỉnh sửa

                if (newQuantity > totalAvailableStock || (warehouseCapacity + oldQuantity) < newQuantity)
                {
                    MessageBox.Show("Số lượng sản phẩm không đủ để sửa hoặc kho đã đầy!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Cập nhật số lượng sản phẩm trong kho
                p.QuantityInStock = totalAvailableStock - newQuantity;
                if (!productService.UpdaterProduct(p))
                {
                    MessageBox.Show("Lỗi khi cập nhật số lượng sản phẩm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Cập nhật thông tin tồn kho
                selectedInventory.Product.ProductName = txtProductName.Text.Trim();
                selectedInventory.Quantity = newQuantity;
                selectedInventory.StockStatus = txtStockStatus.Text.Trim();

                if (!inventoryService.UpdateInventory(selectedInventory))
                {
                    MessageBox.Show("Lỗi khi cập nhật kho hàng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Cập nhật sức chứa của kho
                warehouse.Capacity = (warehouse.Capacity + oldQuantity) - newQuantity;
                if (!warehousesService.UpdateWarehouses(warehouse))
                {
                    MessageBox.Show("Lỗi khi cập nhật kho hàng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Ghi nhật ký giao dịch
                var transactionLog = new TransactionLog
                {
                    ProductId = selectedInventory.Product.ProductId,
                    WarehouseId = selectedInventory.WarehouseId,
                    SupplierId = selectedInventory.SupplierId,
                    ChangeType = "Edit",
                    QuantityChanged = newQuantity - oldQuantity,
                    ChangeDate = DateTime.Now,
                    UserId = user?.UserId ?? 0,
                    Remarks = txtNote.Text?.Trim()
                };

                if (!transactionLogService.AddTransactionLog(transactionLog))
                {
                    MessageBox.Show("Lỗi khi ghi nhật ký giao dịch!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Thông báo thành công
                MessageBox.Show("Cập nhật kho hàng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                // Cập nhật giao diện
                loadInvetory(selectedInventory.WarehouseId);
                EditBtn.Visibility = Visibility.Collapsed;
                SaveBtn.Visibility = Visibility.Visible;
                clear();
                InventoryDataGrid.SelectedItem = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void txtStockStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            warehouse = (Warehouse)WarehouseComboBox.SelectedItem;
            if (warehouse == null)
            {
                MessageBox.Show("Hãy chọn Kho hàng đầu tiên!");
                txtStockStatus.SelectedValue = null;
                return;
            }

            

            // Lấy giá trị từ ComboBoxItem
            if (txtStockStatus.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content != null)
            {
                txtStockStatus.IsEnabled = false;
                ProductStack.Visibility = Visibility.Visible;
                string selectedText = selectedItem.Content.ToString().Trim();

                if (string.Equals(selectedText.Normalize(NormalizationForm.FormD),
                                          "Xuất".Normalize(NormalizationForm.FormD),
                                          StringComparison.OrdinalIgnoreCase))
                {
                    if (InventoryDataGrid.Items.Count > 0) // Kiểm tra có dữ liệu không
                    {
                        InventoryDataGrid.SelectedIndex = 0; // Chọn dòng đầu tiên
                        var selectedInventory = (Inventory)InventoryDataGrid.SelectedItem;
                        if (selectedInventory == null)
                        {
                            MessageBox.Show("Hãy chọn vật phẩm để chuyển");
                            return;
                        }
                        txtProductName.SelectedValue = selectedInventory.ProductId;
                        txtRemain.Text = "(Còn lại: " + selectedInventory.Quantity + ")";
                        ExportBtn.Visibility = Visibility.Visible;
                        OtherWareHouse.Visibility = Visibility.Visible;
                        EditBtn.Visibility = Visibility.Collapsed;
                        SaveBtn.Visibility = Visibility.Collapsed;
                        // Lấy danh sách kho ngoại trừ kho hiện tại
                        var lstWare = warehousesService.GetAllWarehousesByIdSupplier(supplier.SupplierId)
                                                       .Where(w => w.WarehouseId != warehouse.WarehouseId)
                                                       .ToList();

                        if (lstWare.Count == 0)
                        {
                            MessageBox.Show("Không có kho nào để chuyển hàng!");
                            OtherWareHouse.Visibility = Visibility.Collapsed;
                            txtStockStatus.SelectedValue = null;
                            txtStockStatus.IsEnabled = true;
                            return;
                        }

                        txtOtherWarehouse.ItemsSource = lstWare;
                        txtOtherWarehouse.DisplayMemberPath = "WarehouseName";
                        txtOtherWarehouse.SelectedValuePath = "WarehouseId";
                    }
                    
                }
                else
                {
                    ExportBtn.Visibility = Visibility.Collapsed;
                    OtherWareHouse.Visibility = Visibility.Collapsed;
                    SaveBtn.Visibility = Visibility.Visible;
                }
            }
        }


        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra xem kho hàng đã được chọn chưa
                warehouse = WarehouseComboBox.SelectedItem as Warehouse;
                if (warehouse == null)
                {
                    MessageBox.Show("Hãy chọn Kho hàng đầu tiên!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra xem trạng thái sản phẩm đã được chọn chưa
                if (txtStockStatus.SelectedItem is not ComboBoxItem selectedItem || string.IsNullOrEmpty(selectedItem.Content?.ToString()))
                {
                    MessageBox.Show("Hãy chọn trạng thái sản phẩm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra xem sản phẩm đã được chọn chưa
                var selectedInventory = InventoryDataGrid.SelectedItem as Inventory;
                if (selectedInventory == null)
                {
                    MessageBox.Show("Hãy chọn sản phẩm để chuyển kho!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra số lượng nhập hợp lệ
                if (!int.TryParse(txtQuantity.Text.Trim(), out int quantityToMove) || quantityToMove <= 0)
                {
                    MessageBox.Show("Số lượng phải là một số nguyên lớn hơn 0!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra kho hàng đích
                var otherWarehouse = txtOtherWarehouse.SelectedItem as Warehouse;
                if (otherWarehouse == null)
                {
                    MessageBox.Show("Hãy chọn kho hàng đích!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                txtOtherWarehouse.IsEnabled = false;

                // Kiểm tra sức chứa kho đích
                int otherWarehouseCapacity = otherWarehouse.Capacity ?? 0;
                int currentInventoryInOtherWarehouse = inventoryService.getTotalQuantityInventoyByID(otherWarehouse.WarehouseId);

                if (currentInventoryInOtherWarehouse + quantityToMove > otherWarehouseCapacity)
                {
                    MessageBox.Show("Kho hàng đích không đủ sức chứa số lượng sản phẩm này!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra số lượng tồn kho có đủ để chuyển không
                if (quantityToMove > selectedInventory.Quantity)
                {
                    MessageBox.Show("Số lượng sản phẩm trong kho không đủ để chuyển!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Giảm số lượng sản phẩm trong kho nguồn
                selectedInventory.Quantity -= quantityToMove;
                if (!inventoryService.UpdateInventory(selectedInventory))
                {
                    MessageBox.Show("Lỗi khi cập nhật kho nguồn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Giảm sức chứa của kho đích
                otherWarehouse.Capacity -= quantityToMove;
                if (!warehousesService.UpdateWarehouses(otherWarehouse))
                {
                    MessageBox.Show("Lỗi khi cập nhật kho hàng đích!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Kiểm tra xem sản phẩm đã tồn tại trong kho đích chưa
                var existingInventory = inventoryService.GetInventoryListByWarehouseId(otherWarehouse.WarehouseId)
                                                        .FirstOrDefault(item => item.ProductId == selectedInventory.ProductId);

                if (existingInventory != null)
                {
                    // Nếu sản phẩm đã tồn tại, cập nhật số lượng
                    existingInventory.Quantity += quantityToMove;
                    if (!inventoryService.UpdateInventory(existingInventory))
                    {
                        MessageBox.Show("Lỗi khi cập nhật kho hàng đích!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    // Nếu sản phẩm chưa có, thêm mới vào kho đích
                    var newInventory = new Inventory
                    {
                        ProductId = selectedInventory.ProductId,
                        WarehouseId = otherWarehouse.WarehouseId,
                        SupplierId = otherWarehouse.SupplierId,
                        Quantity = quantityToMove,
                        StockStatus = selectedItem.Content.ToString()
                    };

                    if (!inventoryService.AddInventory(newInventory))
                    {
                        MessageBox.Show("Lỗi khi thêm sản phẩm vào kho đích!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                // Ghi nhật ký giao dịch
                var transactionLog = new TransactionLog
                {
                    ProductId = selectedInventory.ProductId,
                    WarehouseId = warehouse.WarehouseId,
                    SupplierId = warehouse.SupplierId,
                    ChangeType = "Chuyển kho",
                    QuantityChanged = quantityToMove,
                    ChangeDate = DateTime.Now,
                    UserId = user?.UserId ?? 0,
                    Remarks = txtNote.Text?.Trim()
                };

                if (!transactionLogService.AddTransactionLog(transactionLog))
                {
                    MessageBox.Show("Lỗi khi ghi nhật ký giao dịch!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Thông báo thành công
                MessageBox.Show($"Chuyển sản phẩm sang kho {otherWarehouse.WarehouseName} thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                // Cập nhật giao diện
                loadInvetory(warehouse.WarehouseId);
                clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
