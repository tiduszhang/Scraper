using Common;
using MahApps.Metro.Controls;
using MVVM.Messaging;
using Scraper.BAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        /// <summary>
        /// 
        /// </summary>
        public WinCommodityDetial()
        {
            InitializeComponent();
            this.Loaded += WinCommodityDetial_Loaded;
        }

        /// <summary>
        /// 
        /// </summary>
        public CommodityDetialViewModel ViewModel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WinCommodityDetial_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.IsInDesignMode())
            {
                return;
            }

            //ViewModel = new CommodityDetialViewModel();
            this.DataContext = ViewModel;
            ViewModel.WebBrowser.NewWindowAction = WebBrowserEx =>
            {
                this.ViewModel.WebBrowser = WebBrowserEx;
            };
            this.host.Child = ViewModel.WebBrowser;

            Messenger.Default.Register<NotificationMessage>(this, DoMessageSomeThing);
        }

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="notificationMessage"></param>
        private void DoMessageSomeThing(NotificationMessage notificationMessage)
        {
            if (notificationMessage.Key == CommodityDetialViewModel.FindCommodityNode)
            {
                this.Close();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            //Messenger.Default.Unregister<NotificationMessage>(this, DoMessageSomeThing);
            e.Cancel = true;
            base.OnClosing(e);
            this.Hide();
        }
         
    }
}
