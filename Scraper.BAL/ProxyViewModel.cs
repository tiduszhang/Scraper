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
    /// 代理ViewMode
    /// </summary>
    public class ProxyViewModel : NotifyBaseModel
    {
        /// <summary>
        /// 代理加载完成
        /// </summary>
        public static readonly string ProxieCompleted = "ProxieCompleted";

        /// <summary>
        /// 
        /// </summary>
        public ProxyViewModel()
        {
            Proxies = new ObservableCollection<GeckoProxy>(); 
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<GeckoProxy> Proxies
        {
            get
            {
                return this.GetValue(o => o.Proxies);
            }
            set
            {
                this.SetValue(o => o.Proxies, value);
            }
        }

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
            GeckoProxy.InternetSetOption(String.Empty, String.Empty);

            var webBrowser = new System.Windows.Forms.WebBrowser();
            webBrowser.DocumentCompleted += (s, e1) =>
            {
                if (webBrowser.ReadyState != System.Windows.Forms.WebBrowserReadyState.Complete)
                {
                    return;
                }

                Proxies.Clear();

                var ipList = webBrowser.Document.GetElementById("ip_list").GetElementsByTagName("tr");
                foreach (HtmlElement iptr in ipList)
                {
                    if (iptr.GetAttribute("className") != "subtitle"
                        && iptr.Children[0].GetAttribute("className") == "country")
                    {
                        string ip = iptr.Children[1].InnerText;
                        string port = iptr.Children[2].InnerText;
                        Proxies.Add(new GeckoProxy() { IP = ip, Port = port });
                    }
                }

                Messenger.Default.Send(new NotificationMessage() { Key = ProxieCompleted, Data = Proxies });
            };
            webBrowser.Navigate(url);
        }
    }
}
