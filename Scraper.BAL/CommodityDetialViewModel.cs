using Gecko.Events;
using MVVM.Messaging;
using MVVM.Model;
using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Scraper.BAL
{
    public class CommodityDetialViewModel : NotifyBaseModel
    {
        public static readonly string FindCommodityNode = "FindCommodityNode";

        private WebBrowserEx _WebBrowser = null;
        public WebBrowserEx WebBrowser
        {
            get
            {
                return _WebBrowser;
            }
            set
            {
                _WebBrowser = value;
                if (isNotIni)
                {
                    _WebBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
                    _WebBrowser.ProgressChanged += WebBrowser_ProgressChanged;
                }
                isNotIni = false;
            }
        }

        public string Url { get; set; }

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

        private bool isNotIni = true;
        /// <summary>
        /// 
        /// </summary>
        public CommodityDetialViewModel()
        {
            Title = "";
        }

        private void WebBrowser_ProgressChanged(object sender, Gecko.GeckoProgressEventArgs e)
        {
            SetTitle();
        }
        private bool Completed { get; set; }
        private void WebBrowser_DocumentCompleted(object sender, GeckoDocumentCompletedEventArgs e)
        {
            if (Completed)
            {
                return;
            }
            Completed = true;
            SetTitle();

            //开始爬数据
            System.Threading.ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    System.Threading.Thread.Sleep(new Random().Next(5, 10) * 1000);
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        Messenger.Default.Send(new NotificationMessage
                        {
                            Key = CommodityDetialViewModel.FindCommodityNode
                        });
                        Completed = false;
                    })); 
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            });
        }

        /// <summary>
        /// 设置标题
        /// </summary>
        private void SetTitle()
        {
            //if (String.IsNullOrWhiteSpace(WebBrowser.DocumentTitle) && WebBrowser.CurrentProxy != null)
            //{
            //    this.Title = "正在尝试使用代理：" + WebBrowser.CurrentProxy.IP + ":" + WebBrowser.CurrentProxy.Port + "。";
            //}
            //else if (WebBrowser.CurrentProxy != null)
            //{
            //    this.Title = WebBrowser.DocumentTitle + "，当前代理：" + WebBrowser.CurrentProxy.IP + ":" + WebBrowser.CurrentProxy.Port + "。";
            //}
            //else if (!String.IsNullOrWhiteSpace(WebBrowser.DocumentTitle))
            //{
            //    this.Title = WebBrowser.DocumentTitle + "，当前没有使用代理。";
            //}
            if (WebBrowser.Url != null)
            {
                this.Title = WebBrowser.DocumentTitle + WebBrowser.Url.ToString();
            }
            else
            {
                this.Title = WebBrowser.DocumentTitle;
            }

        }

        /// <summary>
        /// 刷新
        /// </summary>
        public ICommand RefCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {

                    WebBrowser.Reload();
                });
            }
        }
    }
}
