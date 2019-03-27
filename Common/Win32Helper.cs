using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Win32帮助类 
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    /// </summary> 
    public class Win32Helper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hObject"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpLibFileName"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary")]
        public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpLibFileName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hModule"></param>
        /// <param name="lpProcName"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hModule"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary")]
        public static extern bool FreeLibrary(IntPtr hModule);

        /// <summary>
        /// 动态链接库-指针对象
        /// </summary>
        private IntPtr hLib;
        ///// <summary>
        ///// 是否回收完毕
        ///// </summary>
        //private bool _disposed;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="DLLPath"></param>
        public Win32Helper(String DLLPath)
        {
            hLib = LoadLibrary(DLLPath);
        }

        /// <summary>
        /// 
        /// </summary>
        ~Win32Helper()
        {
            FreeLibrary(hLib);
        }

        /// <summary>
        /// 将要执行的函数转换为委托
        /// </summary>
        /// <param name="APIName"></param>
        /// <param name="t"></param>
        /// <returns></returns> 
        public Delegate Invoke(string APIName, Type t)
        {
            IntPtr api = GetProcAddress(hLib, APIName);
            if (api == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                return Marshal.GetDelegateForFunctionPointer(api, t);
            }
        }
    }
}