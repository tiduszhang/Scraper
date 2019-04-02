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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MVVM;
using MVVM.Messaging;
using MahApps.Metro.Controls;

namespace Scraper
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private MainViewModel ViewModel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.IsInDesignMode())
            {
                return;
            }

            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;

            this.host.Child = ViewModel.WebBrowser;
            ViewModel.WebBrowser.NewWindowAction = () =>
            {
                var WinCommodityDetial = new WinCommodityDetial();
                WinCommodityDetial.ViewModel = new CommodityDetialViewModel(); 
                WinCommodityDetial.Show();

                return WinCommodityDetial.ViewModel.WebBrowser;
            };
            ViewModel.Navigate();
        }

        /// <summary>
        /// 退出程序，清除代理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            GeckoProxy.InternetSetOption(String.Empty, String.Empty);
            base.OnClosed(e);
        }
    }
}
