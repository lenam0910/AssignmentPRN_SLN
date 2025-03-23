
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DataAccess.Models;
using Service;

namespace WPF.Supplier
{
    
    public partial class InventoryManagement : Page
    {
        private InventoryService inventoryService;
        private WarehousesService warehousesService;
        private ProductService productService;
        private DataAccess.Models.User user;
        private TransactionLogService transactionLogService;
        private Warehouse warehouse;
        private Product product;
        private DataAccess.Models.Supplier supplier;
        private UserService userService;
        private SupplierService supplierService;

        public InventoryManagement()
        {
            inventoryService = new InventoryService();
            warehousesService = new WarehousesService();
            productService = new ProductService();
            supplierService = new();
            transactionLogService = new TransactionLogService();
            userService = new();
            InitializeComponent();
            user = Application.Current.Properties["UserAccount"] as DataAccess.Models.User;
            LoadData();

        }

        private void LoadData()
        {

            supplier = supplierService.GetSupplierByUserId(user.UserId);
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

                if (!int.TryParse(txtQuantity.Text.Trim(), out int quantityToAdd) || quantityToAdd <= 0)
                {
                    MessageBox.Show("Số lượng phải là một số nguyên lớn hơn 0!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

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

                if (txtStockStatus.SelectedItem is not ComboBoxItem selectedItem)
                {
                    MessageBox.Show("Hãy chọn tình trạng tồn kho!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                warehouse.Capacity -= quantityToAdd;
                if (!warehousesService.UpdateWarehouses(warehouse))
                {
                    MessageBox.Show("Lỗi khi cập nhật kho hàng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                product.QuantityInStock -= quantityToAdd;
                if (!productService.UpdaterProduct(product))
                {
                    MessageBox.Show("Lỗi khi cập nhật sản phẩm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var existingInventory = inventoryService.GetInventoryListByWarehouseId(warehouse.WarehouseId)
                                                        .FirstOrDefault(item => item.ProductId == product.ProductId);

                if (existingInventory != null)
                {
                    existingInventory.Quantity += quantityToAdd;
                    existingInventory.LastUpdated = DateTime.Now;
                    if (!inventoryService.UpdateInventory(existingInventory))
                    {
                        MessageBox.Show("Lỗi khi cập nhật kho hàng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
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

                MessageBox.Show("Thêm sản phẩm vào kho thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                loadInvetory(warehouse.WarehouseId);
                clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CancelEdit_Click(object sender, RoutedEventArgs e)
        {
           
            clear();
            
        }
        private void clear()
        {
            txtProductName.ItemsSource = productService.GetAllProductsBySupplierId(supplier.SupplierId);
            txtProductName.DisplayMemberPath = "ProductName";
            txtProductName.SelectedValuePath = "ProductId";
            product = null;
            warehouse = null;
            txtProductName.SelectedValue = null;
            txtQuantity.Clear();
            txtStockStatus.SelectedValue = null;
            txtRemain.Text = "";
            txtProductName.IsEnabled = true;
            txtStockStatus.IsEnabled = true;
            WarehouseComboBox.IsEnabled = true;
            ExportBtn.Visibility = Visibility.Collapsed;
            SaveBtn.Visibility = Visibility.Visible;
            OtherWareHouse.Visibility = Visibility.Collapsed;
            ProductStack.Visibility = Visibility.Collapsed;
            status.Visibility = Visibility.Visible;
            txtStockStatus.SelectedValue = null;
            WarehouseComboBox.SelectedValue = null;
            InventoryDataGrid.ItemsSource = null;
            txtRemain.Text = "";
            txtOtherWarehouse.IsEnabled = true;
            txtOtherWarehouse.SelectedValue = null;
            EditBtn.Visibility = Visibility.Collapsed;

        }
        
        private void EditInventory_Click(object sender, RoutedEventArgs e)
        {
            
            var selectedInventory = (Inventory)InventoryDataGrid.SelectedItem;
            if(selectedInventory.Product.IsDeleted == true)
            {
                MessageBox.Show("Sản phẩm không còn để sửa!");
                return;
            }
            else
            {
                SaveBtn.Visibility = Visibility.Collapsed;
                EditBtn.Visibility = Visibility.Visible;
                if (selectedInventory != null)
                {

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
            
        }

   
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
                if (InventoryDataGrid.SelectedItem is not Inventory selectedInventory)
                {
                    MessageBox.Show("Hãy chọn một sản phẩm trong kho để sửa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtProductName.Text) ||
                    string.IsNullOrWhiteSpace(txtStockStatus.Text) ||
                    !int.TryParse(txtQuantity.Text.Trim(), out int newQuantity) || newQuantity <= 0)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin và số lượng hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Product p = productService.GetProductById(selectedInventory.Product.ProductId);
                if (p == null)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int warehouseCapacity = warehouse.Capacity ?? 0;
                int oldQuantity = selectedInventory.Quantity;
                int totalAvailableStock = oldQuantity + p.QuantityInStock.Value; 
                if (newQuantity > totalAvailableStock || (warehouseCapacity + oldQuantity) < newQuantity)
                {
                    MessageBox.Show("Số lượng sản phẩm không đủ để sửa hoặc kho đã đầy!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                p.QuantityInStock = totalAvailableStock - newQuantity;
                if (!productService.UpdaterProduct(p))
                {
                    MessageBox.Show("Lỗi khi cập nhật số lượng sản phẩm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                selectedInventory.Product.ProductName = txtProductName.Text.Trim();
                selectedInventory.Quantity = newQuantity;
                selectedInventory.StockStatus = txtStockStatus.Text.Trim();
                selectedInventory.LastUpdated = DateTime.Now;
                if (!inventoryService.UpdateInventory(selectedInventory))
                {
                    MessageBox.Show("Lỗi khi cập nhật kho hàng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

     
                warehouse.Capacity = (warehouse.Capacity + oldQuantity) - newQuantity;
                if (!warehousesService.UpdateWarehouses(warehouse))
                {
                    MessageBox.Show("Lỗi khi cập nhật kho hàng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

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

          
                MessageBox.Show("Cập nhật kho hàng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

    
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

            

        
            if (txtStockStatus.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content != null)
            {
                txtStockStatus.IsEnabled = false;
                ProductStack.Visibility = Visibility.Visible;
                string selectedText = selectedItem.Content.ToString().Trim();

                if (string.Equals(selectedText.Normalize(NormalizationForm.FormD),
                                          "Xuất".Normalize(NormalizationForm.FormD),
                                          StringComparison.OrdinalIgnoreCase))
                {
                    
                    txtProductName.ItemsSource = productService.GetAllProductsBySupplierIdForExport(supplier.SupplierId);
                    txtProductName.DisplayMemberPath = "ProductName";
                    txtProductName.SelectedValuePath = "ProductId";
                    if (InventoryDataGrid.Items.Count > 0) 
                    {
                        InventoryDataGrid.SelectedIndex = 0;
                        txtProductName.IsEnabled = false;
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
                warehouse = WarehouseComboBox.SelectedItem as Warehouse;
                if (warehouse == null)
                {
                    MessageBox.Show("Hãy chọn Kho hàng đầu tiên!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (txtStockStatus.SelectedItem is not ComboBoxItem selectedItem || string.IsNullOrEmpty(selectedItem.Content?.ToString()))
                {
                    MessageBox.Show("Hãy chọn trạng thái sản phẩm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var selectedInventory = InventoryDataGrid.SelectedItem as Inventory;
                if (selectedInventory == null)
                {
                    MessageBox.Show("Hãy chọn sản phẩm để chuyển kho!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(txtQuantity.Text.Trim(), out int quantityToMove) || quantityToMove <= 0)
                {
                    MessageBox.Show("Số lượng phải là một số nguyên lớn hơn 0!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var otherWarehouse = txtOtherWarehouse.SelectedItem as Warehouse;
                if (otherWarehouse == null)
                {
                    MessageBox.Show("Hãy chọn kho hàng đích!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                txtOtherWarehouse.IsEnabled = false;

                int otherWarehouseCapacity = otherWarehouse.Capacity ?? 0;
                int currentInventoryInOtherWarehouse = inventoryService.getTotalQuantityInventoyByID(otherWarehouse.WarehouseId);

                if (currentInventoryInOtherWarehouse + quantityToMove > otherWarehouseCapacity)
                {
                    MessageBox.Show("Kho hàng đích không đủ sức chứa số lượng sản phẩm này!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (quantityToMove > selectedInventory.Quantity)
                {
                    MessageBox.Show("Số lượng sản phẩm trong kho không đủ để chuyển!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var inventoryToUpdate = new Inventory
                {
                    InventoryId = selectedInventory.InventoryId,
                    ProductId = selectedInventory.ProductId,
                    WarehouseId = selectedInventory.WarehouseId,
                    SupplierId = selectedInventory.SupplierId,
                    Quantity = selectedInventory.Quantity - quantityToMove,
                    StockStatus = "Nhập"
                };

                if (!inventoryService.UpdateInventory(inventoryToUpdate))
                {
                    MessageBox.Show("Lỗi khi cập nhật kho nguồn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                otherWarehouse.Capacity -= quantityToMove;
                warehouse.Capacity += quantityToMove;
                if (!warehousesService.UpdateWarehouses(otherWarehouse) || !warehousesService.UpdateWarehouses(warehouse))
                {
                    MessageBox.Show("Lỗi khi cập nhật kho hàng đích!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var existingInventory = inventoryService.GetInventoryListByWarehouseId(otherWarehouse.WarehouseId)
                                                        .FirstOrDefault(item => item.ProductId == selectedInventory.ProductId);

                if (existingInventory != null)
                {
                    existingInventory.Quantity += quantityToMove;
                    existingInventory.LastUpdated = DateTime.Now;
                    if (!inventoryService.UpdateInventory(existingInventory))
                    {
                        MessageBox.Show("Lỗi khi cập nhật kho hàng đích!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    var newInventory = new Inventory
                    {
                        ProductId = selectedInventory.ProductId,
                        WarehouseId = otherWarehouse.WarehouseId,
                        SupplierId = otherWarehouse.SupplierId,
                        Quantity = quantityToMove,
                        StockStatus = "Nhập",

                    };

                    if (!inventoryService.AddInventory(newInventory))
                    {
                        MessageBox.Show("Lỗi khi thêm sản phẩm vào kho đích!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

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

                MessageBox.Show($"Chuyển sản phẩm sang kho {otherWarehouse.WarehouseName} thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                txtProductName.ItemsSource = productService.GetAllProductsBySupplierId(supplier.SupplierId);
                txtProductName.DisplayMemberPath = "ProductName";
                txtProductName.SelectedValuePath = "ProductId";
                loadInvetory(warehouse.WarehouseId);
                clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void InventoryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OtherWareHouse.IsEnabled = true;
            if (txtStockStatus.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content != null)
            {
                string selectedText = selectedItem.Content.ToString().Trim();
                if (string.Equals(selectedText.Normalize(NormalizationForm.FormD),
                                            "Xuất".Normalize(NormalizationForm.FormD),
                                            StringComparison.OrdinalIgnoreCase))
                {
                    if (InventoryDataGrid.Items.Count > 0) 
                    {
                        txtProductName.IsEnabled = false;
                        var selectedInventory = (Inventory)InventoryDataGrid.SelectedItem;
                        if (selectedInventory == null)
                        {
                            return;
                        }

                        txtProductName.SelectedValue = selectedInventory.ProductId;
                        txtRemain.Text = "(Còn lại: " + selectedInventory.Quantity + ")";
                        ExportBtn.Visibility = Visibility.Visible;
                        OtherWareHouse.Visibility = Visibility.Visible;
                        EditBtn.Visibility = Visibility.Collapsed;
                        SaveBtn.Visibility = Visibility.Collapsed;

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
            }
        }
    }
}
