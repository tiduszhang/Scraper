﻿using MVVM.Model;
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
    public class ProxyViewModel : NotifyBaseModel
    {  
        /// <summary>
       /// 
       /// </summary>
        public ProxyViewModel()
        {
            IEProxies = new ObservableCollection<IEProxy>();
        }
         
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<IEProxy> IEProxies { get; set; }

        /// <summary>
        /// 从指定的网址中获取代理服务器
        /// </summary>
        /// <param name="url"></param>
        public ICommand LoadProxies
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    GetProxies();
                });
            }
        }

        public void GetProxies()
        {
            string url = "www.xicidaili.com";
            IEProxy.InternetSetOption(String.Empty);

            var webBrowser = new System.Windows.Forms.WebBrowser();
            webBrowser.DocumentCompleted += (s, e1) =>
            {
                IEProxies.Clear();

                var ipList = webBrowser.Document.GetElementById("ip_list").GetElementsByTagName("tr");
                foreach (HtmlElement iptr in ipList)
                {
                    if (iptr.GetAttribute("className") != "subtitle"
                        && iptr.Children[0].GetAttribute("className") == "country")
                    {
                        string ip = iptr.Children[1].InnerText;
                        string port = iptr.Children[2].InnerText;
                        IEProxies.Add(new IEProxy() { IP = ip, Port = port });
                    }
                }
            };
            webBrowser.Navigate(url);
        }
    }
}
