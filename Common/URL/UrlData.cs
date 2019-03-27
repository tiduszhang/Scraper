using System;
using System.Collections.Generic;

namespace Common
{ 
    /// <summary>
    /// Url数据
    /// </summary>
    public class UrlData
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            get
            {
                if (String.IsNullOrWhiteSpace(Name))
                {
                    return Url;
                }
                else
                {
                    return string.Format("{0}\r\n{1}", Name, Url);
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<UrlData> GetUrlData()
        {
            ////定义注册表顶级节点 其命名空间是using Microsoft.Win32;
            //RegistryKey historykey;
            ////检索当前用户CurrentUser子项Software\\Microsoft\\Internet Explorer\\typedURLs
            //historykey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Internet Explorer\\typedURLs", true);
            //if (historykey != null)
            //{
            //    //获取检索的所有值
            //    String[] names = historykey.GetValueNames();
            //    foreach (String str in names)
            //    {
            //        txtUrl.Items.Add(historykey.GetValue(str).ToString());
            //    }
            //} 

            var STATURL = new List<UrlData>();
            IUrlHistoryStg2 vUrlHistoryStg2 = (IUrlHistoryStg2)new UrlHistory();
            IEnumSTATURL vEnumSTATURL = vUrlHistoryStg2.EnumUrls();
            STATURL vSTATURL;
            uint vFectched;

            while (vEnumSTATURL.Next(1, out vSTATURL, out vFectched) == 0)
            {
                STATURL.Add(new UrlData()
                {
                    Name = vSTATURL.pwcsTitle,
                    Url = vSTATURL.pwcsUrl,
                });
                //richTextBox1.AppendText(string.Format("{0}\r\n{1}\r\n", vSTATURL.pwcsTitle, vSTATURL.pwcsUrl));
                //txtUrl.Items.Add(string.Format("{0}\r\n{1}\r\n", vSTATURL.pwcsTitle, vSTATURL.pwcsUrl)); 
            }

            return STATURL;
        }
    }
}