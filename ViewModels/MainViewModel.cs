using System.Windows;
using MemoryGame.ViewModels;
using MemoryGame.Views;

namespace MemoryGame.ViewModels
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var loginViewModel = new LoginViewModel();
                var loginView = new LoginView
                {
                    DataContext = loginViewModel
                };

                loginView.Show();

                var mainWindow = Application.Current.MainWindow;
                if (mainWindow != null && mainWindow.GetType() == typeof(MainWindow))
                { 
                    mainWindow.Close();
                }
            });
        }
    }
}
