using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Common
{
    /// <summary>
    /// 消息对象
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    /// </summary>
    public class Message
    { 
        /// <summary>
        /// 消息内容JSON字符串，一般运行程序时指定的参数，比如打开浏览器时指定网址
        /// </summary>
        public string Content { get; set; }
         
        /// <summary>
        /// 目标IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 目标端口
        /// </summary>
        public int Port { get; set; }  
    }
}
