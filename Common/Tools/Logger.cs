using log4net;
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
        private static log4net.ILog log = null;

        /// <summary>
        /// 写入日志 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="level"></param>
        public static void WriteToLog(this string value, log4net.Core.Level level = null)
        {

            string path = WorkPath.ExecPath + @"\Config\Log4net.config";
            if (File.Exists(path))
            {
                if (log == null)
                {
                    log4net.GlobalContext.Properties["LogUrl"] = WorkPath.ApplicationWorkPath + @"\Logs\";
                    log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(path));
                    log = log4net.LogManager.GetLogger("lognet");
                }
            }

            if (log == null)
            {
                return;
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
        }
    }
}
