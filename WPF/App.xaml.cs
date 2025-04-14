using System;
using System.Windows;

namespace WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            int? userId = null;
            if (e.Args.Length > 0)
            {
                for (int i = 0; i < e.Args.Length; i++)
                {
                    if (e.Args[i] == "--userId" && i + 1 < e.Args.Length)
                    {
                        if (int.TryParse(e.Args[i + 1], out int id))
                        {
                            userId = id;
                        }
                        break;
                    }
                }
            }

            Login loginWindow = new Login();
            loginWindow.Show();

            if (userId.HasValue)
            {
                loginWindow.AutoLogin(userId.Value);

            }
        }
    }
}