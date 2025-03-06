using System;
using System.Collections.Generic;
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

namespace WPF.User
{
    /// <summary>
    /// Interaction logic for OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        private OrderService orderService;
        private OrderDetailService orderDetailService;
        private DataAccess.Models.User user;
        private Order order;
        public OrdersPage()
        {
            user = Application.Current.Properties["UserAccount"] as DataAccess.Models.User;
            InitializeComponent();
        }

        private void load()
        {
            order = orderService.GetOrderByUserId(user.UserId);
            var lstOrder = orderDetailService.GetAllOrders();
            OrdersListView.ItemsSource = lstOrder;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            load();
        }

        private void NumericUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {

        }
    }
}
