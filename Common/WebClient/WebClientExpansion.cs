using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Common
{
    /// <summary>
    /// Web客户端
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    /// </summary>
    public static class WebClientExpansion
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="contentType"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="head"></param>
        /// <param name="fast"></param>
        /// <returns></returns>
        public static WebClient CreatedWebClient(this Uri value, string contentType = "application/json", string user = "", string pass = "", string head = "", bool fast = false)
        {
            StrongWebClient webClient = new StrongWebClient();
            webClient.Encoding = Encoding.UTF8;
            webClient.Headers["Content-type"] = contentType;
            if (fast)//快速返回
            {
                webClient.Timeout = 1000;
            }

            if (!String.IsNullOrWhiteSpace(head))//头
            {
                webClient.Headers["x-auth-token"] = head;//todo:用户Appkey 
            }

            if (!String.IsNullOrWhiteSpace(user))//账户
            {
                webClient.Credentials = new NetworkCredential(user, pass);
            }



            return webClient;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="contentType"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="head"></param>
        /// <param name="fast"></param>
        /// <returns></returns>
        public static WebClient CreatedWebClient(this string value, string contentType, string user = "", string pass = "", string head = "", bool fast = false)
        {
            return new Uri(value).CreatedWebClient(contentType, user, pass, head, fast);
        }

        /// <summary>
        /// 根据URL得到数据对象
        /// </summary>
        /// <param name="value">URL对象</param>
        /// <param name="data">需要提交的数据</param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="head">头部信息</param>
        /// <param name="fast">快速访问</param>
        /// <returns></returns>
        public static string DownloadString(this Uri value, object data = null, string user = "", string pass = "", string head = "", bool fast = false)
        {
            string result = "";
            ("请求地址：" + value.ToString()).WriteToLog("", log4net.Core.Level.Debug);
            try
            {
                using (var webClient = value.CreatedWebClient("application/json", user, pass, head, fast))
                {
                    if (data == null || String.IsNullOrWhiteSpace(data.ToString()))
                    {
                        result = webClient.DownloadString(value);
                    }
                    else if (data != null && data is string)
                    {
                        result = webClient.UploadString(value, data.ToString());
                    }
                    else if (data != null && data is NameValueCollection)
                    {
                        result = webClient.UploadValues(value, (data as NameValueCollection)).ConvertToUTF8String();
                    }
                    else if (data != null)
                    {
                        result = webClient.UploadData(value, data.ConvertToBytes()).ConvertToUTF8String();
                    }
                }
            }
            catch (Exception ex)
            {
                if (data == null || String.IsNullOrWhiteSpace(data.ToString()))
                {
                    ("请求地址：" + value.ToString() + "出现异常！" + System.Environment.NewLine + ex.ToString()).WriteToLog("", log4net.Core.Level.Error);
                }
                else
                {
                    ("请求地址：" + value.ToString() + "出现异常！提交数据：" + data + System.Environment.NewLine + ex.ToString()).WriteToLog("", log4net.Core.Level.Error);
                }
                result.Trim().WriteToLog("", log4net.Core.Level.Debug);
                result = "";
                throw ex;
            }
            //GC.Collect();
            ("服务器返回：" + result.Trim()).WriteToLog("", log4net.Core.Level.Debug);
            return result.Trim();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fileName"></param>
        /// <param name="head"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        public static string UploadFile(this Uri value, string fileName, string user = "", string pass = "", string head = "")
        {
            string result = "";
            ("请求上传文件：" + value.ToString()).WriteToLog("", log4net.Core.Level.Debug);
            try
            {
                using (var webClient = value.CreatedWebClient("application/octet-stream", user, pass, head, false))
                {
                    byte[] uploadResult = webClient.UploadFile(value, fileName);
                    result = Encoding.UTF8.GetString(uploadResult);
                }
            }
            catch (Exception ex)
            {
                ("上传过程中出现异常：" + value.ToString() + "出现异常！" + System.Environment.NewLine + ex.ToString()).WriteToLog("", log4net.Core.Level.Error);
                throw ex;
            }
            //GC.Collect();
            ("服务器返回：" + result.Trim()).WriteToLog("", log4net.Core.Level.Debug);
            return result;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fileName"></param>
        /// <param name="head"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        public static string UploadFile(this string value, string fileName, string user = "", string pass = "", string head = "")
        {
            Uri url = new Uri(value);
            return url.UploadFile(fileName, user, pass, head);
        }


        /// <summary>
        /// 上传文件
        /// </summary>  
        /// <param name="value"></param> 
        /// <param name="fileName"></param> 
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="head"></param>
        public static WebClient UploadFileAsync(this Uri value, string fileName, string user = "", string pass = "", string head = "")
        {
            ("请求上传文件地址：" + value.ToString()).WriteToLog("", log4net.Core.Level.Debug);
            var webClient = value.CreatedWebClient("application/octet-stream", user, pass, head, false);
            try
            {
                webClient.UploadFileAsync(value, fileName);
                webClient.UploadFileCompleted += (sender, e) =>
                {
                    try
                    {
                        if (e.Error != null)
                        {
                            ("上传过程中出现异常：" + value.ToString() + "出现异常！" + System.Environment.NewLine + e.Error.ToString()).WriteToLog("", log4net.Core.Level.Error);
                        }
                        webClient.Dispose();
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                };
            }
            catch (Exception ex)
            {
                ("请求上传文件地址：" + value.ToString() + "出现异常！" + System.Environment.NewLine + ex.ToString()).WriteToLog("", log4net.Core.Level.Error);
            }
            return webClient;
        }

        /// <summary>
        /// 下载文件
        /// </summary>  
        /// <param name="value"></param> 
        /// <param name="fileName"></param> 
        /// <param name="head"></param>
        public static WebClient UploadFileAsync(this string value, string fileName, string head = "")
        {
            Uri url = new Uri(value);
            return url.UploadFileAsync(fileName, head);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fileName"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="head"></param>
        public static void DownloadFile(this Uri value, string fileName, string user = "", string pass = "", string head = "")
        {
            ("请求下载文件地址：" + value.ToString()).WriteToLog("",log4net.Core.Level.Debug);
            try
            {
                using (var webClient = value.CreatedWebClient("application/octet-stream", user, pass, head, false))
                {
                    webClient.DownloadFile(value, fileName);
                }
            }
            catch (Exception ex)
            {
                ("下载过程中出现异常：" + value.ToString() + "出现异常！" + System.Environment.NewLine + ex.ToString()).WriteToLog("",log4net.Core.Level.Error);
                throw ex;
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fileName"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="head"></param>
        public static void DownloadFile(this string value, string fileName, string user = "", string pass = "", string head = "")
        {
            Uri url = new Uri(value);
            url.DownloadFile(fileName, user, pass, head);
        }


        /// <summary>
        /// 下载数据
        /// </summary>
        /// <param name="value"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="head"></param>
        public static byte[] DownloadData(this Uri value, string user = "", string pass = "", string head = "")
        {
            ("请求下载文件地址：" + value.ToString()).WriteToLog("", log4net.Core.Level.Debug);

            byte[] bData = null;
            try
            {
                var webClient = value.CreatedWebClient("application/octet-stream", user, pass, head, false);
                bData = webClient.DownloadData(value);
                webClient.Dispose();
            }
            catch (Exception ex)
            {
                ("下载过程中出现异常：" + value.ToString() + "出现异常！" + System.Environment.NewLine + ex.ToString()).WriteToLog("", log4net.Core.Level.Error);
                throw ex;
            }
            return bData;
        }

        /// <summary>
        /// 下载数据
        /// </summary>
        /// <param name="value"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="head"></param>
        public static byte[] DownloadData(this string value, string user = "", string pass = "", string head = "")
        {
            Uri url = new Uri(value);
            return url.DownloadData(user, pass, head);
        }

        /// <summary>
        /// 下载文件
        /// </summary>  
        /// <param name="value"></param> 
        /// <param name="fileName"></param> 
        /// <param name="head"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param> 
        public static WebClient DownloadFileAsync(this Uri value, string fileName, string user = "", string pass = "", string head = "")
        {
            ("请求下载文件地址：" + value.ToString()).WriteToLog("", log4net.Core.Level.Debug);
            var webClient = value.CreatedWebClient("application/octet-stream", user, pass, head, false);
            try
            {
                webClient.DownloadFileAsync(value, fileName);
                webClient.DownloadFileCompleted += (sender, e) =>
                {
                    try
                    {
                        if (e.Error != null)
                        {
                            ("下载过程中出现异常：" + value.ToString() + "出现异常！" + System.Environment.NewLine + e.Error.ToString()).WriteToLog("", log4net.Core.Level.Error);
                        }
                        webClient.Dispose();
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                };
            }
            catch (Exception ex)
            {
                ("请求下载文件地址：" + value.ToString() + "出现异常！" + System.Environment.NewLine + ex.ToString()).WriteToLog("", log4net.Core.Level.Error);
            }
            return webClient;
        }

        /// <summary>
        /// 下载文件
        /// </summary>  
        /// <param name="value"></param> 
        /// <param name="fileName"></param> 
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="head"></param>
        public static WebClient DownloadFileAsync(this string value, string fileName, string user = "", string pass = "", string head = "")
        {
            Uri url = new Uri(value);
            return url.DownloadFileAsync(fileName, user, pass, head);
        }

        /// <summary>
        /// 根据URL得到数据对象
        /// </summary>
        /// <param name="value">URL对象</param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="head">头部信息</param>
        /// <param name="fast">快速访问</param>
        /// <returns></returns>
        public static string DownloadString(this Uri value, string user = "", string pass = "", string head = "", bool fast = false)
        {
            return value.DownloadString(null, user, pass, head, fast);
        }

        /// <summary>
        /// 根据URL得到数据对象
        /// </summary>
        /// <typeparam name="T">泛型实体类</typeparam>
        /// <param name="value">URL对象</param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="head">头部信息</param>
        /// <param name="fast">快速访问</param>
        /// <returns></returns>
        public static T DownloadObject<T>(this Uri value, string user = "", string pass = "", string head = "", bool fast = false) where T : class
        {
            return value.DownloadObject<T>(user, pass, head, fast);
        }

        /// <summary>
        /// 根据URL得到数据对象
        /// </summary>
        /// <typeparam name="T">泛型实体类</typeparam>
        /// <param name="value">URL对象</param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="data">需要提交的数据</param>
        /// <param name="head">头部信息</param>
        /// <param name="fast">快速访问</param>
        /// <returns></returns>
        public static T DownloadObject<T>(this Uri value, object data = null, string user = "", string pass = "", string head = "", bool fast = false) where T : class
        {
            string strObject = value.DownloadString(data, user, pass, head, fast);
            T Tobject = strObject.JsParse<T>();
            return Tobject;
        }

        /// <summary>
        /// 根据URL字符串得到数据对象
        /// </summary>
        /// <param name="value">URL字符串</param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="head">头部信息</param>
        /// <param name="fast">快速访问</param>
        /// <returns></returns>
        public static string DownloadString(this string value, string user = "", string pass = "", string head = "", bool fast = false)
        {
            return new Uri(value).DownloadString(user, pass, head, fast);
        }

        /// <summary>
        /// 根据URL字符串得到数据对象
        /// </summary>
        /// <param name="value">URL字符串</param>
        /// <param name="data">需要提交的数据</param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="head">头部信息</param>
        /// <param name="fast">快速访问</param>
        /// <returns></returns>
        public static string DownloadString(this string value, object data, string user = "", string pass = "", string head = "", bool fast = false)
        {
            Uri url = new Uri(value);
            string strData = url.DownloadString(data, user, pass, head, fast);
            return strData;
        }

        /// <summary>
        /// 批量提交内容
        /// </summary>
        /// <param name="value">URL字符串</param>
        /// <param name="data">需要提交的数组</param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="head">头部信息</param>
        /// <param name="fast">快速访问</param>
        /// <returns></returns>
        public static string DownloadString(this string value, List<string> data, string user = "", string pass = "", string head = "", bool fast = false)
        {
            Uri url = new Uri(value);
            string strData = url.DownloadString(String.Join("&", data.ToArray()), user, pass, head, fast);
            return strData;
        }

        /// <summary>
        /// 批量提交内容
        /// </summary>
        /// <param name="value">URL字符串</param>
        /// <param name="key">需要提交的数组的Key</param>
        /// <param name="data">需要提交的数组</param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="head">头部信息</param>
        /// <param name="fast">快速访问</param>
        /// <returns></returns>
        public static string DownloadString<T>(this string value, string key, List<T> data, string user = "", string pass = "", string head = "", bool fast = false)
        {
            Uri url = new Uri(value);
            var lstData = data.ConvertAll<string>(o => "key=" + o.JsStringify());
            string strData = url.DownloadString(String.Join("&", lstData.ToArray()), user, pass, head, fast);
            return strData;
        }

        /// <summary>
        /// 根据URL字符串得到数据对象
        /// </summary>
        /// <typeparam name="T">泛型实体类</typeparam>
        /// <param name="value">URL字符串</param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="head">头部信息</param>
        /// <param name="fast">快速访问</param>
        /// <returns></returns>
        public static T DownloadObject<T>(this string value, string user = "", string pass = "", string head = "", bool fast = false) where T : class
        {
            return value.DownloadObject<T>(user, pass, head, fast);
        }

        /// <summary>
        /// 根据URL字符串得到数据对象
        /// </summary>
        /// <typeparam name="T">泛型实体类</typeparam>
        /// <param name="value">URL字符串</param>
        /// <param name="data">需要提交的数据</param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="head">头部信息</param>
        /// <param name="fast">快速访问</param>
        /// <returns></returns>
        public static T DownloadObject<T>(this string value, object data = null, string user = "", string pass = "", string head = "", bool fast = false) where T : class
        {
            string strObject = value.DownloadString(data, user, pass, head, fast);
            T Tobject = strObject.JsParse<T>();
            return Tobject;
        }
    }
}
