using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 消息类 Udp 扩展
    /// </summary>
    public static class MessageUdpExpansion
    { 
        /// <summary>
        /// 注册监听任务
        /// </summary>
        /// <param name="value"></param>
        /// <param name="func"></param>
        public static void RegisterUdpReceiveMessage(this object value, Action<Message> func)
        {
            UdpService udpService = UdpService.GetInstence();
            udpService.ReceiveMessage += func;
        }

        /// <summary>
        /// 注销监听任务
        /// </summary>
        /// <param name="value"></param>
        /// <param name="func"></param>
        public static void UnRegisterUdpReceiveMessage(this object value, Action<Message> func)
        {
            UdpService udpService = UdpService.GetInstence();
            udpService.ReceiveMessage -= func;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="value"></param>
        /// <param name="receiveMessage"></param>
        /// <returns></returns>
        public static bool UdpSend(this Message value, Action<Message> receiveMessage = null)
        {
            bool isOk = false;
            UdpClient udpClient = new UdpClient();
            try
            {

                byte[] bData = value.ConvertToBytes();
                udpClient.Send(bData, bData.Length);
                if (receiveMessage != null)
                {

                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] rData = udpClient.Receive(ref endPoint);
                    Message message = rData.ConvertToObject<Message>();
                    receiveMessage(message);
                }
            }
            catch (Exception ex)
            {
                ex.ToString().WriteToLog();
                ex.ToString();
            }
            finally
            {
                udpClient.Close();
                udpClient = null;
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
        public static void UdpSendAsync(this Message value, Action<Message> receiveMessage = null)
        {
            try
            {
                UdpClient udpClient = new UdpClient();
                byte[] bData = value.ConvertToBytes();
                udpClient.BeginSend(bData, bData.Length, o =>
                {
                    try
                    {
                        UdpClient udpClientSend = o.AsyncState as UdpClient;
                        if (receiveMessage != null)
                        {
                            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                            byte[] rData = udpClientSend.Receive(ref endPoint);
                            Message message = rData.ConvertToObject<Message>();
                            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                receiveMessage(message);
                            }));
                        }
                        udpClientSend.Close();
                    }
                    catch (Exception ex)
                    {
                        ex.ToString().WriteToLog();
                        ex.ToString();
                    }
                }, udpClient);
            }
            catch (Exception ex)
            {
                ex.ToString().WriteToLog();
                ex.ToString();
            }
            GC.Collect();
        }

    }
}
