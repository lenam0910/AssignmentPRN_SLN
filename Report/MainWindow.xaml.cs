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
using Microsoft.Win32;
using System.IO;
using OfficeOpenXml;
using Excel = Microsoft.Office.Interop.Excel;

namespace Report
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserSet
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Export User Excel";
            saveFileDialog.Filter = "Excel (*.xlsx)|*.xlsx|Excel 2013 (*.xls)|*.xls";
            try
            {
                exportExcel(saveFileDialog.FileName);
                MessageBox.Show("Xuất file thành công! ");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xuất file không thành công! " + ex.Message);

            }
        }

     
       
            public void exportExcel(string path)
        {
            Excel.Application application = new Excel.Application();
            application.Application.Workbooks.Add(Type.Missing);
            for (int i = 0; i < girdExcel.Columns.Count; i++)
            {
                application.Cells[1, i + 1] = girdExcel.Columns[i].Header.ToString();
            }
            for (int i = 0; i < girdExcel.Items.Count; i++)
            {
                for (int j = 0; j < girdExcel.Columns.Count; j++)
                {
                    TextBlock cellContent = girdExcel.Columns[j].GetCellContent(girdExcel.Items[i]) as TextBlock;
                    application.Cells[i + 2, j + 1] = cellContent != null ? cellContent.Text : "";
                }
            }
            application.Columns.AutoFit();
            application.ActiveWorkbook.SaveCopyAs(path);
            application.ActiveWorkbook.Saved = true;
            application.Quit();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            girdExcel.ItemsSource = 
        }
    }
}
