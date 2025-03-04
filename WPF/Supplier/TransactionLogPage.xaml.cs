using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
using Microsoft.Win32;
using Service;

namespace WPF.Supplier
{
    /// <summary>
    /// Interaction logic for TransactionLogPage.xaml
    /// </summary>
    public partial class TransactionLogPage : Page
    {
        private TransactionLogService service;
        private SupplierService supplierService;
        private DataAccess.Models.User user;
        private DataAccess.Models.Supplier supplier;
        private ProductService productService;
        private WarehousesService warehousesService;
        private UserService userService;
        private List<TransactionLog> transactionLogsLst;
        public TransactionLogPage()
        {
            warehousesService = new WarehousesService();
            productService = new();
            userService = new();
            user = Application.Current.Properties["UserAccount"] as DataAccess.Models.User;
            supplierService = new SupplierService();
            service = new TransactionLogService();
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            transactionLogsLst = lstTran();
            dgTransactionLogs.ItemsSource = transactionLogsLst;
        }

        private List<TransactionLog> lstTran()
        {
            var lst = service.GetAllBySupplierID(supplierService.GetSuppliersByUserId(user.UserId).SupplierId);
            var lstProduct = productService.GetAllProducts();
            var lstWare = warehousesService.getAll();
            var lstSupplier = supplierService.GetAllSuppliers();
            var lstUser = userService.getAll();
            foreach (var pr in lst)
            {
                foreach (var item in lstProduct)
                {
                    if (item.ProductId == pr.ProductId)
                    {
                        pr.Product = item;
                    }
                }
            }
            foreach (var pr in lst)
            {
                foreach (var item in lstWare)
                {
                    if (pr.WarehouseId == item.WarehouseId)
                    {
                        pr.Warehouse = item;
                    }
                }

            }
            foreach (var pr in lst)
            {
                foreach (var item in lstSupplier)
                {
                    if (pr.SupplierId == item.SupplierId)
                    {
                        pr.Supplier = item;
                    }
                }
            }
            foreach (var pr in lst)
            {
                foreach (var item in lstUser)
                {
                    if (pr.UserId == item.UserId)
                    {
                        pr.User = item;
                    }
                }
            }
            return lst;
        }

        private void FilterLogs(object sender, RoutedEventArgs e)
        {

            DateTime from = (DateTime)dpFromDate.SelectedDate;
            DateTime to = (DateTime)dpToDate.SelectedDate;
            if (txtSearch != null)
            {
                foreach (var item in transactionLogsLst)
                {
                    if (item.Product.ProductName.Contains(txtSearch.Text) && item.ChangeDate.Value >= from.Date && item.ChangeDate.Value <= to.Date)
                    {

                    }
                    else
                    {
                        transactionLogsLst.Remove(item);
                    }
                }
            }
            dgTransactionLogs.ItemsSource = transactionLogsLst;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(transactionLogsLst != null)
            {
                var filteredList = transactionLogsLst.Where(item => item.Product.ProductName.Equals(txtSearch.Text)).ToList();
                if (filteredList != null)
                {
                    dgTransactionLogs.ItemsSource = null;
                    dgTransactionLogs.ItemsSource = filteredList;
                }
                
            }
            else
            {
                return;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog jsonSaveFile = new SaveFileDialog();

            jsonSaveFile.DefaultExt = "json";
            jsonSaveFile.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
            if (jsonSaveFile.ShowDialog() == true)
            {
                var jsonOption = new JsonSerializerOptions { WriteIndented = true };

                List<TransactionLog> list = lstTran();

                string jsonContent = JsonSerializer.Serialize(list, jsonOption);

                File.WriteAllText(jsonSaveFile.FileName, jsonContent);
                MessageBox.Show("Lưu File thành công! " + jsonSaveFile.FileName);
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog jsonFile = new OpenFileDialog();
            jsonFile.DefaultExt = "json";
            jsonFile.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";

            if (jsonFile.ShowDialog() == true)
            {
                string jsonText = File.ReadAllText(jsonFile.FileName);
                List<TransactionLog> lst = JsonSerializer.Deserialize<List<TransactionLog>>(jsonText);
                transactionLogsLst = lst;
                dgTransactionLogs.ItemsSource = transactionLogsLst;


            }
        }
    }
}
