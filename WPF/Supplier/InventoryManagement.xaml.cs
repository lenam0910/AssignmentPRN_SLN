using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        private User user;
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
            user = Application.Current.Properties["UserAccount"] as User;
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
                status.Visibility = Visibility.Visible;
            }
            else
            {
                txtProductName.ItemsSource = null;
            }

        }


        // Lưu thông tin kho hàng (thêm hoặc sửa)
        private void SaveInventory_Click(object sender, RoutedEventArgs e)
        {
            warehouse = (Warehouse)WarehouseComboBox.SelectedItem;
            if (warehouse == null)
            {
                MessageBox.Show("Hãy chọn Kho hàng đầu tiên!");
            }
            else
            {
                product = (Product)txtProductName.SelectedItem;
                if (product != null)
                {
                    var selectedItem = (ComboBoxItem)txtStockStatus.SelectedItem;
                    int quantityWare = warehousesService.GetWarehouseById(warehouse.WarehouseId).Capacity.Value;
                    int quantityCheck = inventoryService.getTotalQuantityInventoyByID(warehouse.WarehouseId);

                    if (quantityCheck <= quantityWare)
                    {
                        if (int.Parse(txtQuantity.Text) <= product.QuantityInStock && quantityWare >= int.Parse(txtQuantity.Text))
                        {
                            warehouse.Capacity = warehouse.Capacity - int.Parse(txtQuantity.Text);
                            if (warehousesService.UpdateWarehouses(warehouse))
                            {
                                product.QuantityInStock = product.QuantityInStock - int.Parse(txtQuantity.Text);
                                if (productService.UpdaterProduct(product))
                                {
                                    var inventory = new Inventory
                                    {

                                        ProductId = product.ProductId,
                                        WarehouseId = warehouse.WarehouseId,
                                        SupplierId = warehouse.SupplierId,
                                        Quantity = int.Parse(txtQuantity.Text),
                                        StockStatus = selectedItem.Content.ToString()
                                    };
                                    if (inventoryService.AddInventory(inventory))
                                    {
                                        var transactionLog = new TransactionLog
                                        {
                                            ProductId = product.ProductId,
                                            WarehouseId = warehouse.WarehouseId,
                                            SupplierId = warehouse.SupplierId,
                                            ChangeType = "Thêm",
                                            QuantityChanged = int.Parse(txtQuantity.Text),
                                            ChangeDate = DateTime.Now,
                                            UserId = user.UserId,
                                            Remarks = txtNote.Text
                                        };
                                        if (transactionLogService.AddTransactionLog(transactionLog))
                                        {
                                            MessageBox.Show("Đã thêm kho hàng mới.");
                                            loadInvetory(warehouse.WarehouseId);
                                            clear();
                                        }

                                    }
                                }
                                else
                                {
                                    MessageBox.Show("ERR PRodcut");

                                }
                            }
                            else
                            {
                                MessageBox.Show("ERR");

                            }


                        }
                        else
                        {
                            MessageBox.Show("Số lượng sản phẩm không đủ hoặc kho đã đầy!");
                        }

                    }
                    else
                    {
                        MessageBox.Show("Số lượng trong kho không đủ để thêm!");
                    }

                }
                else
                {
                    MessageBox.Show("Hãy chọn sản phẩm để thêm!");
                }

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
            status.Visibility = Visibility.Collapsed;
            OtherWareHouse.Visibility = Visibility.Collapsed;
        }
        private void clear()
        {
            product = null;
            warehouse = null;
            txtProductName.SelectedValue = null;
            txtQuantity.Clear();
            txtStockStatus.SelectedValue = null;
            txtRemain.Text = "";


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
                txtRemain.Text = "(Còn lại: " + selectedProduct.QuantityInStock + ")";
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedInventory = (Inventory)InventoryDataGrid.SelectedItem;
            if (selectedInventory != null)
            {
                EditBtn.Visibility = Visibility.Visible;
                SaveBtn.Visibility = Visibility.Collapsed;

                selectedInventory.Product.ProductName = txtProductName.Text;
                int oldQuantity = selectedInventory.Quantity;
                selectedInventory.Product.ProductId = (int)txtProductName.SelectedValue;
                selectedInventory.StockStatus = txtStockStatus.Text;
                int newQuantity = int.Parse(txtQuantity.Text);
                Product p = productService.GetProductById(selectedInventory.Product.ProductId);

                int quantityWare = warehousesService.GetWarehouseById(warehouse.WarehouseId).Capacity.Value;
                if (newQuantity <= (oldQuantity + p.QuantityInStock) && (quantityWare + oldQuantity) >= int.Parse(txtQuantity.Text))
                {
                    p.QuantityInStock = (oldQuantity + p.QuantityInStock) - newQuantity;
                    if (productService.UpdaterProduct(p))
                    {
                        selectedInventory.Quantity = newQuantity;


                        if (inventoryService.UpdateInventory(selectedInventory))
                        {
                            warehouse.Capacity = (warehouse.Capacity + oldQuantity) - int.Parse(txtQuantity.Text);
                            warehousesService.UpdateWarehouses(warehouse);
                            var transactionLog = new TransactionLog
                            {
                                ProductId = selectedInventory.Product.ProductId,
                                WarehouseId = selectedInventory.WarehouseId,
                                SupplierId = selectedInventory.SupplierId,
                                ChangeType = "Edit",  // Loại thay đổi
                                QuantityChanged = int.Parse(txtQuantity.Text) - selectedInventory.Quantity,  // Số lượng thay đổi
                                ChangeDate = DateTime.Now,
                                UserId = user.UserId,
                                Remarks = txtNote.Text
                            };
                            if (transactionLogService.AddTransactionLog(transactionLog))
                            {
                                MessageBox.Show("Đã sửa kho hàng.");
                                loadInvetory(selectedInventory.WarehouseId);
                                EditBtn.Visibility = Visibility.Collapsed;
                                SaveBtn.Visibility = Visibility.Visible;
                                clear();
                                InventoryDataGrid.SelectedItem = null;
                            }


                        }
                    }
                    else
                    {
                        MessageBox.Show("err");
                    }
                }
                else
                {
                    MessageBox.Show("Số lượng sửa không đủ để sửa !");
                }
            }

        }

        private void txtStockStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            warehouse = (Warehouse)WarehouseComboBox.SelectedItem;
            if (warehouse == null)
            {
                MessageBox.Show("Hãy chọn Kho hàng đầu tiên!");
                return;
            }

            

            // Lấy giá trị từ ComboBoxItem
            if (txtStockStatus.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content != null)
            {
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
            warehouse = (Warehouse)WarehouseComboBox.SelectedItem;
            if (warehouse == null)
            {
                MessageBox.Show("Hãy chọn Kho hàng đầu tiên!");
            }
            else
            {

                if (txtStockStatus.SelectedItem is ComboBoxItem selectedItem)
                {
                    var selectedInventory = (Inventory)InventoryDataGrid.SelectedItem;
                    if (selectedInventory != null)
                    {
                        if (txtStockStatus.SelectedItem is ComboBoxItem selected && selectedItem.Content != null)
                        {
                            string selectedText = selected.Content.ToString().Trim();

                            if (string.Equals(selectedText.Normalize(NormalizationForm.FormD),
                                                      "Xuất".Normalize(NormalizationForm.FormD),
                                                      StringComparison.OrdinalIgnoreCase))
                            {

                                var otherWareHouse = txtOtherWarehouse.SelectedItem as Warehouse;
                                if (otherWareHouse != null)
                                {

                                    int quantityWare = warehousesService.GetWarehouseById(otherWareHouse.WarehouseId).Capacity.Value;
                                    int quantityCheck = inventoryService.getTotalQuantityInventoyByID(otherWareHouse.WarehouseId);

                                    if (quantityCheck <= quantityWare)
                                    {
                                        if (int.Parse(txtQuantity.Text) <= selectedInventory.Quantity && quantityWare >= int.Parse(txtQuantity.Text))
                                        {
                                            selectedInventory.Quantity = selectedInventory.Quantity - int.Parse(txtQuantity.Text);
                                            inventoryService.UpdateInventory(selectedInventory);
                                            otherWareHouse.Capacity = otherWareHouse.Capacity - int.Parse(txtQuantity.Text);
                                            warehousesService.UpdateWarehouses(otherWareHouse);
                                            var inventory = new Inventory
                                            {
                                                ProductId = selectedInventory.ProductId,
                                                WarehouseId = otherWareHouse.WarehouseId,
                                                SupplierId = otherWareHouse.SupplierId,
                                                Quantity = int.Parse(txtQuantity.Text),
                                                StockStatus = selectedItem.Content.ToString()
                                            };
                                            if (inventoryService.AddInventory(inventory))
                                            {
                                                var transactionLog = new TransactionLog
                                                {
                                                    ProductId = selectedInventory.ProductId,
                                                    WarehouseId = otherWareHouse.WarehouseId,
                                                    SupplierId = otherWareHouse.SupplierId,
                                                    ChangeType = "Chuyển kho",
                                                    QuantityChanged = int.Parse(txtQuantity.Text),
                                                    ChangeDate = DateTime.Now,
                                                    UserId = user.UserId,
                                                    Remarks = txtNote.Text
                                                };
                                                if (transactionLogService.AddTransactionLog(transactionLog))
                                                {
                                                    MessageBox.Show("Đã chuyển sang kho hàng " + otherWareHouse.WarehouseName + " thành công !");
                                                    loadInvetory(warehouse.WarehouseId);
                                                    clear();
                                                }

                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Số lượng sản phẩm không đủ!");
                                        }

                                    }
                                    else
                                    {
                                        MessageBox.Show("Số lượng trong kho không đủ để thêm!");
                                    }

                                }
                                else
                                {
                                    MessageBox.Show("Hãy chọn sản phẩm để chuyển!");
                                }

                            }
                        }

                    }
                }
            }
        }
    }
}
