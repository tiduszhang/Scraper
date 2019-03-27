using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Socket通信设置
    /// </summary>
    public class SocketConfig
    {
        /// <summary>
        ///  Socket通信设置文件名
        /// </summary>
        public static readonly string SocketConfigFileName = "SocketConfig.json";

        /// <summary>
        /// 通信端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {
            this.SaveJsonFile(WorkPath.ApplicationWorkPath, SocketConfigFileName);
        }

        /// <summary>
        /// 加载Sokcet类
        /// </summary>
        /// <returns></returns>
        public static SocketConfig Load()
        {
            return WorkPath.ApplicationWorkPath.LoadJsonFile<SocketConfig>(SocketConfigFileName);
        }
    }
}
