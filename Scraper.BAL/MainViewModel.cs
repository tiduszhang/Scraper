using MVVM.Messaging;
using MVVM.Model;
using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Scraper.BAL
{
    /// <summary>
    /// 
    /// </summary>
    public class MainViewModel : NotifyBaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ProxyViewModel ProxyViewModel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public WebBrowserEx WebBrowser { get; set; }

        /// <summary>
        /// url
        /// </summary>
        public string Url { get; set; } = "www.taobao.com";

        /// <summary>
        /// 使用代理
        /// </summary>
        public bool UserProxy
        {
            get
            {
                return this.GetValue(o => o.UserProxy);
            }
            set
            {
                this.SetValue(o => o.UserProxy, value);
            }
        }

        /// <summary>
        /// 是否正在爬网
        /// </summary>
        public bool IsScrapering
        {
            get
            {
                return this.GetValue(o => o.IsScrapering);
            }
            set
            {
                this.SetValue(o => o.IsScrapering, value);
            }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValue(o => o.Title);
            }
            set
            {
                this.SetValue(o => o.Title, value);
            }
        }

        /// <summary>
        /// 开始访问
        /// </summary>
        public void Navigate()
        {
            WebBrowser.NewNavigate(Url);
        }


        /// <summary>
        /// 
        /// </summary>
        public MainViewModel()
        {
            Messenger.Default.Register<NotificationMessage>(this, ProxieCompleted);
            ProxyViewModel = new ProxyViewModel();
            WebBrowser = new WebBrowserEx();

            WebBrowser.Navigating += WebBrowser_Navigating;

        }

        /// <summary>
        /// 启动代理
        /// </summary>
        public ICommand CheckProxy
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    WebBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
                    if (UserProxy == true)
                    {
                        ProxyViewModel.GetProxies();
                    }
                    else
                    {
                        WebBrowser.IEProxies.Clear();
                        WebBrowser.NewNavigate(Url);
                    }
                });
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
                WebBrowser.IEProxies.Clear();
                WebBrowser.IEProxies = ProxyViewModel.IEProxies.ToList();
                WebBrowser.NewNavigate(Url);
            }
        }

        /// <summary>
        /// 加载完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser.DocumentCompleted -= WebBrowser_DocumentCompleted;
            if (WebBrowser.ReadyState == System.Windows.Forms.WebBrowserReadyState.Complete)
            {
                //加载完成 
                if (IsScrapering)
                {
                    //todo:执行解析网页
                    //todo:执行下一个查询
                }
            }

            SetTitle();
        }

        /// <summary>
        /// 加载时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            SetTitle();
        }


        /// <summary>
        /// 设置标题
        /// </summary>
        private void SetTitle()
        {
            if (String.IsNullOrWhiteSpace(WebBrowser.DocumentTitle) && WebBrowser.CurrentIEProxy != null)
            {
                this.Title = "正在尝试使用代理：" + WebBrowser.CurrentIEProxy.IP + ":" + WebBrowser.CurrentIEProxy.Port + "。";
            }
            else if (WebBrowser.CurrentIEProxy != null)
            {
                this.Title = WebBrowser.DocumentTitle + "，当前代理：" + WebBrowser.CurrentIEProxy.IP + ":" + WebBrowser.CurrentIEProxy.Port + "。";
            }
            else if (!String.IsNullOrWhiteSpace(WebBrowser.DocumentTitle))
            {
                this.Title = WebBrowser.DocumentTitle + "，当前没有使用代理。";
            }
            if (WebBrowser.Url != null)
            {
                this.Title += WebBrowser.Url.ToString();
            }
        }

        /// <summary>
        /// 开始爬网
        /// </summary>
        public ICommand ExecScraper
        {
            get
            {
                return new DelegateCommand<string>(query =>
                {
                    IsScrapering = !IsScrapering;

                    if (IsScrapering == true)
                    {
                        //开始请求查询页面
                    }
                });
            }
        }


    }
}
