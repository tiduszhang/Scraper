using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 进程工具类
    /// </summary>
    public static class ProcessTools
    {
        /// <summary>
        /// 获取进程
        /// </summary>
        /// <param name="lstProcessNames"></param>
        /// <returns></returns>
        public static System.Diagnostics.Process GetProcessByListProcessName(this List<string> lstProcessNames)
        {
            System.Diagnostics.Process HideProces = null;
            //if (isProcessCover)
            //{
            //    return isProcessCover;
            //}
            if (lstProcessNames != null && lstProcessNames.Count > 0)
            {
                //获取进程列表
                List<System.Diagnostics.Process> HideProcess = System.Diagnostics.Process.GetProcesses().ToList();

                HideProces = HideProcess.Find(o =>
                {
                    try
                    {
                        string strFileName = o.ProcessName;
                        return lstProcessNames.Find(x => x.ToLower().Contains(strFileName.ToLower())) != null;
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                    return false;
                });
            }
            return HideProces;
        }

    }
}
