using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace Tool
{
    /// <summary>
    /// Interaction logic for YouTube.xaml
    /// </summary>
    public partial class YouTube : Window
    {
        public YouTube()
        {
            InitializeComponent();
        }
        public YouTube(string url)
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    utube.Address = url;    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi mở URL: " + ex.Message);
                }
            }

        }
    }
}
