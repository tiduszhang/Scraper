using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Logger
    /// </summary>
    public static class Logger
    {
        ///// <summary>
        ///// 日志对象
        ///// </summary>
        //private static log4net.ILog log = null;

        /// <summary>
        /// 写入日志 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <param name="level"></param>
        public static void WriteToLog(this string value, string path = "", log4net.Core.Level level = null)
        {  
    		log4net.ILog log = null;

            log4net.GlobalContext.Properties["LogUrl"] = WorkPath.ApplicationWorkPath + "\\logs\\" + path;

            string config = WorkPath.ExecPath + @"\Log4net.config";
            if (File.Exists(path))
            {
                log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(config));
                log = log4net.LogManager.GetLogger("");
            }
            else
            {
                log = log4net.LogManager.GetLogger("");
            }
			
            if (level == log4net.Core.Level.Debug)
            {
                log.Debug(value);
            }
            else if (level == log4net.Core.Level.Info)
            {
                log.Info(value);
            }
            else if (level == log4net.Core.Level.Error)
            {
                log.Error(value);
            }
            else if (level == log4net.Core.Level.Fatal)
            {
                log.Fatal(value);
            }
            else if (level == log4net.Core.Level.Warn)
            {
                log.Warn(value);
            }
            else
            {
                log.Info(value);
            }
            //try
            //{
            //    using (StreamWriter s = System.IO.File.AppendText(System.IO.Path.GetFullPath(Constant.ApplicationWorkPath + @"\Log.log")))
            //    {
            //        s.WriteLine(value);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ex.ToString();
            //}
        }
    }
}
