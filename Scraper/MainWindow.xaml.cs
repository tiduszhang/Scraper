using Common;
using Scraper.BAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MVVM; 
namespace Scraper
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
           if(this.IsInDesignMode())
            {
                return;
            }
            var dataContext = new MainViewModel();
            this.DataContext = dataContext;
            dataContext.ProxyViewModel.GetProxies();
            webBrowser.IEProxies = dataContext.ProxyViewModel.IEProxies.ToList();
            webBrowser.Navigate("www.taobao.com");
        }
          
    }
}
