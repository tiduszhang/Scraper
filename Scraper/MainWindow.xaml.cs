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

            Messenger.Default.Register<NotificationMessage>(this, ProxieCompleted);

            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;
            ViewModel.WebBrowser.Navigating += WebBrowser_Navigating;
            ViewModel.WebBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
            ViewModel.WebBrowser.NewNavigate("www.taobao.com");
             
            this.host.Child = ViewModel.WebBrowser;
        }

        /// <summary>
        /// 加载完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowser_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            ViewModel.WebBrowser.DocumentCompleted -= WebBrowser_DocumentCompleted;
            if (ViewModel.WebBrowser.ReadyState == System.Windows.Forms.WebBrowserReadyState.Complete)
            {
                //加载完成 
            }

            SetTitle();
        }

        /// <summary>
        /// 加载过程中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowser_Navigating(object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e)
        {
            SetTitle();
        }

        /// <summary>
        /// 设置标题
        /// </summary>
        private void SetTitle()
        {
            if (String.IsNullOrWhiteSpace(ViewModel.WebBrowser.DocumentTitle) && ViewModel.WebBrowser.CurrentIEProxy != null)
            {
                this.Title = "正在尝试使用代理：" + ViewModel.WebBrowser.CurrentIEProxy.IP + ":" + ViewModel.WebBrowser.CurrentIEProxy.Port + "。";
            }
            else if (ViewModel.WebBrowser.CurrentIEProxy != null)
            {
                this.Title = ViewModel.WebBrowser.DocumentTitle + "，当前代理：" + ViewModel.WebBrowser.CurrentIEProxy.IP + ":" + ViewModel.WebBrowser.CurrentIEProxy.Port + "。";
            }
            else if (!String.IsNullOrWhiteSpace(ViewModel.WebBrowser.DocumentTitle))
            {
                this.Title = ViewModel.WebBrowser.DocumentTitle + "，当前没有使用代理。";
            }
            if (ViewModel.WebBrowser.Url != null)
            {
                this.Title += ViewModel.WebBrowser.Url.ToString();
            }
        }

        /// <summary>
        /// 代理加载完成
        /// </summary>
        /// <param name="notificationMessage"></param>
        private void ProxieCompleted(NotificationMessage notificationMessage)
        {
            if (notificationMessage.Key == ProxyViewModel.ProxieCompleted)
            {
                ViewModel.WebBrowser.IEProxies.Clear();
                ViewModel.WebBrowser.IEProxies = (this.DataContext as MainViewModel).ProxyViewModel.IEProxies.ToList();
                ViewModel.WebBrowser.NewNavigate("www.taobao.com");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.WebBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
            CheckBox checkBox = sender as CheckBox;
            if (checkBox.IsChecked == true)
            {
                (this.DataContext as MainViewModel).ProxyViewModel.GetProxies();
            }
            else
            {
                ViewModel.WebBrowser.IEProxies.Clear();
                ViewModel.WebBrowser.NewNavigate("www.taobao.com");
            }
        }

        /// <summary>
        /// 退出程序，清除代理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            IEProxy.InternetSetOption(String.Empty);
            base.OnClosed(e);
        }
    }
}
