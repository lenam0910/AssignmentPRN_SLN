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

namespace WPF.Supplier
{
    /// <summary>
    /// Interaction logic for TransactionLogPage.xaml
    /// </summary>
    public partial class TransactionLogPage : Page
    {

        public TransactionLogPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            

        }

        private void FilterLogs(object sender, RoutedEventArgs e)
        {
            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
