using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Tool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Lấy tham số dòng lệnh
            string[] args = Environment.GetCommandLineArgs();
            string url = args.Length > 1 ? args[1] : null; // args[0] là đường dẫn đến Tool.exe, args[1] là URL

            // Tạo MainWindow và truyền URL
            YouTube mainWindow = new YouTube(url);
            mainWindow.Show();
        }
    }
}
