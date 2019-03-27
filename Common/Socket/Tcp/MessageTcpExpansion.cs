using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 消息类 TCP扩展
    /// </summary>
    public static class MessageTcpExpansion
    {
        /// <summary>
        /// 注册监听任务
        /// </summary>
        /// <param name="value"></param>
        /// <param name="func"></param>
        public static void RegisterTcpReceiveMessage(this object value, Action<Message> func)
        {
            TcpService tcpService = TcpService.GetInstence();
            tcpService.ReceiveMessage += func;
        }

        /// <summary>
        /// 注销监听任务
        /// </summary>
        /// <param name="value"></param>
        /// <param name="func"></param>
        public static void UnRegisterTcpReceiveMessage(this object value, Action<Message> func)
        {
            TcpService tcpService = TcpService.GetInstence();
            tcpService.ReceiveMessage -= func;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="value"></param>
        /// <param name="receiveMessage"></param>
        /// <returns></returns>
        public static bool TcpSend(this Message value, Action<Message> receiveMessage = null)
        {
            bool isOk = false;
            try
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(value.IP, value.Port);
                NetworkStream ns = tcpClient.GetStream();
                byte[] bData = value.ConvertToBytes();
                ns.Write(bData, 0, bData.Length);
                ns.Flush();
                if (receiveMessage != null)
                {
                    MemoryStream ms = new MemoryStream();
                    byte[] brData = new byte[1024];
                    int length = 0;
                    while ((length = ns.Read(brData, 0, brData.Length)) > 0)
                    {
                        ms.Write(brData, 0, length);
                    }
                    byte[] bValue = ms.ToArray();
                    Message message = bValue.ConvertToObject<Message>();
                    receiveMessage(message);

                    ms.Close();
                    ms.Dispose();
                    ms = null;
                }
                ns.Close();
                ns.Dispose();
                ns = null;
                tcpClient.Close();
                tcpClient = null;
                isOk = true;
            }
            catch (Exception ex)
            {
                ex.ToString().WriteToLog();
                ex.ToString();
            }
            GC.Collect();
            return isOk;
        }

        /// <summary>
        /// 发送消息异步处理
        /// </summary>
        /// <param name="value"></param>
        /// <param name="receiveMessage"></param>
        /// <returns></returns>
        public static void TcpSendAsync(this Message value, Action<Message> receiveMessage = null)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    TcpClient tcpClient = new TcpClient();
                    tcpClient.Connect(value.IP, value.Port);
                    NetworkStream ns = tcpClient.GetStream();
                    byte[] bData = value.ConvertToBytes();
                    ns.BeginWrite(bData, 0, bData.Length, o =>
                    {
                        NetworkStream networks = o.AsyncState as NetworkStream;
                        networks.EndWrite(o);
                        networks.Flush();
                        if (receiveMessage != null)
                        {
                            MemoryStream ms = new MemoryStream();
                            byte[] brData = new byte[1024];
                            int length = 0;
                            while ((length = networks.Read(brData, 0, brData.Length)) > 0)
                            {
                                ms.Write(brData, 0, length);
                            }
                            byte[] bValue = ms.ToArray();
                            Message message = bValue.ConvertToObject<Message>();
                            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                receiveMessage(message);
                            }));
                            ms.Close();
                            ms.Dispose();
                            ms = null;
                        }
                        networks.Close();
                        networks.Dispose();
                        networks = null;
                        tcpClient.Close();
                        tcpClient = null;
                    }, ns);
                }
                catch (Exception ex)
                {
                    ex.ToString().WriteToLog("", log4net.Core.Level.Error);
                }
                GC.Collect();
            });
        }

    }
}
