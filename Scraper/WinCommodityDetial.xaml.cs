using MahApps.Metro.Controls;
using Scraper.BAL;
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
using System.Windows.Shapes;

namespace Scraper
{
    /// <summary>
    /// WinCommodityDetial.xaml 的交互逻辑
    /// </summary>
    public partial class WinCommodityDetial : MetroWindow
    {
        public WinCommodityDetial()
        {
            InitializeComponent();
            this.Loaded += WinCommodityDetial_Loaded;
        }
        public CommodityDetialViewModel ViewModel { get; set; }

        private void WinCommodityDetial_Loaded(object sender, RoutedEventArgs e)
        {
            //ViewModel = new CommodityDetialViewModel();
            this.DataContext = ViewModel;
            this.host.Child = ViewModel.WebBrowser;
        }


    }
}
