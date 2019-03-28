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
using MVVM.Messaging;

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
            if (this.IsInDesignMode())
            {
                return;
            }

            Messenger.Default.Register<NotificationMessage>(this, ProxieCompleted);

            var dataContext = new MainViewModel();
            this.DataContext = dataContext;
            dataContext.ProxyViewModel.GetProxies();
            webBrowser.Navigating += WebBrowser_Navigating;
            webBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
        }

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser.CurrentIEProxy != null)
            {
                this.Title = webBrowser.DocumentTitle + "，当前代理：" + webBrowser.CurrentIEProxy.IP + ":" + webBrowser.CurrentIEProxy.Port + "。";
            }
            else
            {
                this.Title = webBrowser.DocumentTitle + "，当前没有使用代理。";
            }
        }

        private void WebBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(webBrowser.DocumentTitle))
            {
                this.Title = "正在尝试使用代理：" + webBrowser.CurrentIEProxy.IP + ":" + webBrowser.CurrentIEProxy.Port + "。";
            }
            else if (webBrowser.CurrentIEProxy != null)
            {
                this.Title = webBrowser.DocumentTitle + "，当前代理：" + webBrowser.CurrentIEProxy.IP + ":" + webBrowser.CurrentIEProxy.Port + "。";
            }
            else
            {
                this.Title = webBrowser.DocumentTitle + "，当前没有使用代理。";
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
                webBrowser.IEProxies = (this.DataContext as MainViewModel).ProxyViewModel.IEProxies.ToList();
                webBrowser.NewNavigate("www.taobao.com");
                Messenger.Default.Unregister<NotificationMessage>(this, ProxieCompleted);
            }
        }
    }
}
