
using System.IO;

using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

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
    
        private List<TransactionLog> transactionLogsLst;
        public TransactionLogPage()
        {
        
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
        private List<TransactionLog> lstAll()
        {
            var lst = service.GetAllTransactions();
            return lst;
        }
        private List<TransactionLog> lstTran()
        {
            var lst = service.GetAllBySupplierID(supplierService.GetSupplierByUserId(user.UserId).SupplierId);
            return lst;
        }

        private void FilterLogs(object sender, RoutedEventArgs e)
        {
            if (!dpFromDate.SelectedDate.HasValue || !dpToDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn ngày bắt đầu và ngày kết thúc!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime from = dpFromDate.SelectedDate.Value;
            from = from.Date; 

            DateTime to = dpToDate.SelectedDate.Value;
            to = to.Date.AddHours(23).AddMinutes(59).AddSeconds(59); 

            if (from > to)
            {
                MessageBox.Show("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            from = from.ToUniversalTime();
            to = to.ToUniversalTime();
            if(txtSearch.Text.Equals("Tìm sản phẩm", StringComparison.OrdinalIgnoreCase))
            {
                txtSearch.Text = "";
            }
            var filteredLogs = new List<TransactionLog>();
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                for (int i = 0; i < transactionLogsLst.Count(); i++)
                {
                    if (transactionLogsLst.ElementAt(i).Product.ProductName.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase) && transactionLogsLst.ElementAt(i).ChangeDate.Value >= from && transactionLogsLst.ElementAt(i).ChangeDate.Value <= to)
                    {
                        filteredLogs.Add(transactionLogsLst.ElementAt(i));
                    }
                }
            }
            else
            {
                for (int i = 0; i < transactionLogsLst.Count(); i++)
                {
                    if (transactionLogsLst.ElementAt(i).ChangeDate.Value >= from && transactionLogsLst.ElementAt(i).ChangeDate.Value <= to)
                    {
                        filteredLogs.Add(transactionLogsLst.ElementAt(i));
                    }
                }
            }
                
           

            dgTransactionLogs.ItemsSource = filteredLogs.ToList();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e) 
        {
            LoadData();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(transactionLogsLst != null)
            {
                var filteredList = transactionLogsLst.Where(item => item.Product.ProductName.Contains(txtSearch.Text,StringComparison.OrdinalIgnoreCase)).ToList();
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

                foreach (var item in list)
                {
                    item.Product = null;
                    item.Warehouse = null;
                    item.Supplier = null;
                    item.User = null;
                }

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
                var lstOrigin = lstAll();
                foreach (var item in lst)
                {
                    foreach(var item2 in lstOrigin)
                    {
                        if(item.TransactionId == item2.TransactionId)
                        {
                            item.Product = item2.Product;
                            item.Warehouse = item2.Warehouse;
                            item.Supplier = item2.Supplier;
                            item.User = item2.User;
                        }
                    }
                }
                transactionLogsLst = lst;
                dgTransactionLogs.ItemsSource = transactionLogsLst;


            }
        }
    }
}
