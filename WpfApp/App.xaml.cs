using System.Configuration;
using System.Data;
using System.Windows;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Настройка иконки для всех окон (если не задана в каждом окне)
            this.Resources.Add("ApplicationIcon", "/Resources/icon.ico");
        }
    }

}
