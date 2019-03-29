﻿using MVVM.Messaging;
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
        /// 商品列表
        /// </summary>
        public List<Commodity> Commoditys { get; set; }

        /// <summary>
        /// url
        /// </summary>
        public string Url { get; set; } = "www.taobao.com"; // "about:config";//

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
            WebBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
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
                    //WebBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
                    if (UserProxy == true)
                    {
                        ProxyViewModel.GetProxies();
                    }
                    else
                    {
                        WebBrowser.Proxies.Clear();
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
                WebBrowser.Proxies.Clear();
                WebBrowser.Proxies = ProxyViewModel.Proxies.ToList();
                WebBrowser.NewNavigate(Url);
            }
        }

        /// <summary>
        /// 加载完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowser_DocumentCompleted(object sender, Gecko.Events.GeckoDocumentCompletedEventArgs e)
        {
            //WebBrowser.DocumentCompleted -= WebBrowser_DocumentCompleted;
            SetTitle();
            //if (WebBrowser.ReadyState == System.Windows.Forms.WebBrowserReadyState.Complete)
            //{
            //加载完成 
            if (!IsScrapering)
            {
                return;
            }
            //执行解析网页
            DoCommoditys();

            //执行下一个查询
            if (QueryStrings == null || QueryIndex == QueryStrings.Length)
            {
                IsScrapering = false;
                return;
            }

            System.Threading.Thread.Sleep(1000);
            DoQuery(QueryStrings[QueryIndex]);
            //} 
        }

        /// <summary>
        /// 加载时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowser_Navigating(object sender, Gecko.Events.GeckoNavigatingEventArgs e)
        {
            SetTitle();
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
            //if (WebBrowser.Url != null)
            //{
            //    this.Title += WebBrowser.Url.ToString();
            //}
            this.Title = WebBrowser.DocumentTitle + WebBrowser.Url.ToString();
        }

        /// <summary>
        /// 查询关键字
        /// </summary>
        string[] QueryStrings { get; set; }
        /// <summary>
        /// 查询关键字索引
        /// </summary>
        int QueryIndex { get; set; }
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

                    if (IsScrapering != true)
                    {
                        //开始请求查询页面
                        return;
                    }
                    if (String.IsNullOrWhiteSpace(query))
                    {
                        MessageBox.Show("请先输入检索关键字,多个关键字请使用“,”分割!");
                        return;
                    }

                    QueryIndex = 0;
                    QueryStrings = query.Split(',');

                    DoQuery(QueryStrings[QueryIndex]);
                });
            }
        }

        /// <summary>
        /// 执行一次查询
        /// </summary>
        /// <param name="queryString"></param>
        private void DoQuery(string queryString)
        {
            QueryIndex++;
            var search = WebBrowser.Document.GetElementById("q");
            if (search.TagName.ToLower() == "input")
            {
                search.SetAttribute("value", queryString);
            }

            var from = WebBrowser.Document.GetElementById("J_TSearchForm");
            if (from == null)
            {
                from = WebBrowser.Document.GetElementById("J_SearchForm");
            }

            //btn-search tb-bg
            //submit icon-btn-search
            var buttons = from.GetElementsByTagName("button");
            if (buttons != null && buttons.Length > 0)
            {
                foreach (var button in buttons)
                {
                    if (button.GetAttribute("class").Contains("btn-search"))
                    {
                        //WebBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
                        (button as Gecko.GeckoHtmlElement).Click();//.("click");

                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 解析搜索列表网页
        /// </summary>
        private void DoCommoditys()
        {
            //var dom = WebBrowser.Document;
            ////var dom = (mshtml.IHTMLDocument3)WebBrowser.Document.DomDocument;
            //var mainDivs = dom.getElementById("main");



            var main = WebBrowser.Document.GetElementById("main");
            //var divs = main.Children.GetElementsByName("div"); 
            //foreach (HtmlElement div in divs)
            //{
            //    if (div.GetAttribute("className") == "m-itemlist")
            //    {

            //    }
            //}

        }
    }
}
