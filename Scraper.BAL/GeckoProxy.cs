using MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper.BAL
{
    /// <summary>
    /// 火狐代理
    /// </summary>
    public class GeckoProxy : NotifyBaseModel
    {
        public string IP
        {
            get
            {
                return this.GetValue(o => o.IP);
            }
            set
            {
                this.SetValue(o => o.IP, value);
            }
        }
        public string Port
        {
            get
            {
                return this.GetValue(o => o.Port);
            }
            set
            {
                this.SetValue(o => o.Port, value);
            }
        }


        /// <summary>
        /// 设置WebBrowser控件代理服务
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool InternetSetOption(string ip, string port)
        {
            // network.proxy.type 取值
            //0 – Direct connection, no proxy. (Default)
            //1 – Manual proxy configuration.
            //2 – Proxy auto-configuration (PAC).
            //4 – Auto-detect proxy settings.
            //5 – Use system proxy settings (Default in Linux).   

            if (String.IsNullOrWhiteSpace(ip))//判断是否弃用代理
            {
                Gecko.GeckoPreferences.User["network.proxy.type"] = 0; 
                Gecko.GeckoPreferences.User["network.proxy.http"] = "";
                Gecko.GeckoPreferences.User["network.proxy.http_port"] = 0;//int.Parse(port);
                Gecko.GeckoPreferences.User["network.proxy.share_proxy_settings"] = true;
            }
            else
            {
                Gecko.GeckoPreferences.User["network.proxy.type"] = 1;
                Gecko.GeckoPreferences.User["network.proxy.http"] = ip;
                Gecko.GeckoPreferences.User["network.proxy.http_port"] = int.Parse(port);
                // 
                Gecko.GeckoPreferences.User["network.proxy.share_proxy_settings"] = true;
            }
            return true;
        }
    }
}
