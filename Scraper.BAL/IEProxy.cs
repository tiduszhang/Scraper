using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MVVM;
using MVVM.Model;
using System.Collections.ObjectModel;

namespace Scraper.BAL
{
    /// <summary>
    /// 设置WebBrowser控件的代理服务
    /// 设置后不会影响IE浏览器
    /// MSDN:http://msdn.microsoft.com/en-us/library/aa385114%28v=vs.85%29.aspx
    /// </summary>
    public class IEProxy : NotifyBaseModel
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

        private const int INTERNET_OPTION_PROXY = 38;
        private const int INTERNET_OPEN_TYPE_PROXY = 3;
        private const int INTERNET_OPEN_TYPE_DIRECT = 1;

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

        public struct Struct_INTERNET_PROXY_INFO
        {
            public int dwAccessType;
            public IntPtr proxy;
            public IntPtr proxyBypass;
        }

        /// <summary>
        /// 设置WebBrowser控件代理服务
        /// </summary>
        /// <param name="strProxy"></param>
        /// <returns></returns>
        public static bool InternetSetOption(string strProxy)
        {
            int bufferLength;
            IntPtr intptrStruct;
            Struct_INTERNET_PROXY_INFO struct_IPI;

            if (string.IsNullOrEmpty(strProxy) || strProxy.Trim().Length == 0)
            {
                strProxy = string.Empty;
                struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_DIRECT;
            }
            else
            {
                struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_PROXY;
            }

            struct_IPI.proxy = Marshal.StringToHGlobalAnsi(strProxy);
            struct_IPI.proxyBypass = Marshal.StringToHGlobalAnsi("local");

            bufferLength = Marshal.SizeOf(struct_IPI);
            intptrStruct = Marshal.AllocCoTaskMem(bufferLength);
            Marshal.StructureToPtr(struct_IPI, intptrStruct, true);

            return InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY, intptrStruct, bufferLength);
        }

    }
}