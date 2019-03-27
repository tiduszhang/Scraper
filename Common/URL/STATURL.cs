using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;  //命名空间
using System.Reflection;               //提供加载类型 Pointer指针
using Microsoft.Win32;                 //RegistryKey

namespace Common
{
    #region COM接口实现获取IE历史记录
    /// <summary>
    /// 自定义结构 IUrlHistory
    /// </summary>
    public struct STATURL
    {
        /// <summary>
        /// 
        /// </summary>
        public static uint SIZEOF_STATURL =
            (uint)Marshal.SizeOf(typeof(STATURL));
        /// <summary>
        /// 
        /// </summary>
        public uint cbSize;                    //网页大小
        /// <summary>
        /// 
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]      //网页Url 
        public string pwcsUrl;
        /// <summary>
        /// 
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]      //网页标题
        public string pwcsTitle;
        /// <summary>
        /// 
        /// </summary>
        public System.Runtime.InteropServices.ComTypes.FILETIME
            ftLastVisited,                     //网页最近访问时间
            ftLastUpdated,                     //网页最近更新时间
            ftExpires;
        /// <summary>
        /// 
        /// </summary>
        public uint dwFlags;
    }

    /// <summary>
    /// ComImport属性通过guid调用com组件
    /// </summary>
    [ComImport, Guid("3C374A42-BAE4-11CF-BF7D-00AA006946EE"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumSTATURL
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="celt"></param>
        /// <param name="rgelt"></param>
        /// <param name="pceltFetched"></param>
        /// <returns></returns>
        [PreserveSig]
        //搜索IE历史记录匹配的搜索模式并复制到指定缓冲区
        uint Next(uint celt, out STATURL rgelt, out uint pceltFetched);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="celt"></param>
        void Skip(uint celt);
        /// <summary>
        /// 
        /// </summary>
        void Reset();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ppenum"></param>
        void Clone(out IEnumSTATURL ppenum);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="poszFilter"></param>
        /// <param name="dwFlags"></param>
        void SetFilter(
            [MarshalAs(UnmanagedType.LPWStr)] string poszFilter,
            uint dwFlags);
    }

    /// <summary>
    /// 
    /// </summary>
    [ComImport, Guid("AFA0DC11-C313-11d0-831A-00C04FD5AE38"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IUrlHistoryStg2
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pocsUrl"></param>
        /// <param name="pocsTitle"></param>
        /// <param name="dwFlags"></param>
        #region IUrlHistoryStg methods
        void AddUrl(
            [MarshalAs(UnmanagedType.LPWStr)] string pocsUrl,
            [MarshalAs(UnmanagedType.LPWStr)] string pocsTitle,
            uint dwFlags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pocsUrl"></param>
        /// <param name="dwFlags"></param>
        void DeleteUrl(
            [MarshalAs(UnmanagedType.LPWStr)] string pocsUrl,
            uint dwFlags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pocsUrl"></param>
        /// <param name="dwFlags"></param>
        /// <param name="lpSTATURL"></param>
        void QueryUrl(
            [MarshalAs(UnmanagedType.LPWStr)] string pocsUrl,
            uint dwFlags,
            ref STATURL lpSTATURL);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pocsUrl"></param>
        /// <param name="riid"></param>
        /// <param name="ppvOut"></param>
        void BindToObject(
            [MarshalAs(UnmanagedType.LPWStr)] string pocsUrl,
            ref Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppvOut);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumSTATURL EnumUrls();
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pocsUrl"></param>
        /// <param name="pocsTitle"></param>
        /// <param name="dwFlags"></param>
        /// <param name="fWriteHistory"></param>
        /// <param name="poctNotify"></param>
        /// <param name="punkISFolder"></param>
        void AddUrlAndNotify(
            [MarshalAs(UnmanagedType.LPWStr)] string pocsUrl,
            [MarshalAs(UnmanagedType.LPWStr)] string pocsTitle,
            uint dwFlags,
            [MarshalAs(UnmanagedType.Bool)] bool fWriteHistory,
            [MarshalAs(UnmanagedType.IUnknown)] object    /*IOleCommandTarget*/
            poctNotify,
            [MarshalAs(UnmanagedType.IUnknown)] object punkISFolder);

        /// <summary>
        /// 
        /// </summary>
        void ClearHistory();       //清除历史记录
    }

    /// <summary>
    /// 
    /// </summary>
    [ComImport, Guid("3C374A40-BAE4-11CF-BF7D-00AA006946EE")]
    public class UrlHistory /* : IUrlHistoryStg[2] */ { }
    #endregion 
}