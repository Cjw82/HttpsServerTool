using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public class TcpClient
    {
        private Socket socket;
        private int Length = 0;
        private EndPoint _ep;
        ////任务的取消
        //private static CancellationTokenSource tokenSource = new CancellationTokenSource();
        //private CancellationToken token = tokenSource.Token;
        private byte[] _refData = new byte[2048];
        public static string Messages { get; set; }
        /// <summary>
        /// 消息回调函数
        /// </summary>
        /// <param name="sender">socket对象</param>
        /// <param name="message">消息</param>
        /// <param name="flag">是否连接标志</param>
        public delegate void ReveMessageEventHandler(object sender, string message, Flag flag);
        //回调函数
        public event ReveMessageEventHandler ReveMessaged;
        public void InitialTcpClient(string IP, int Port)
        {
            try
            {
                //创建监听的socket
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //设置IP和端口
                _ep = new IPEndPoint(IPAddress.Parse(IP), Port);
                // 连接IP和端口
                socket.Connect(_ep);
                if (socket.Connected)
                {
                    ReveMessaged(socket, "连接成功!!", Flag.connect);
                }
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    { 
                        Length = socket.Receive(_refData);
                        if (Length == 0)
                        {
                            break;
                        }
                        Messages = Encoding.UTF8.GetString(_refData, 0, Length);
                        ReveMessaged(socket, Messages, Flag.connect);
                    }
                });
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public async Task SendTo(string str)
        {
            var dt = new ArraySegment<byte>(Encoding.UTF8.GetBytes(str));
            if (socket.Connected)
            {
                await socket.SendAsync(dt, SocketFlags.None);
            }
            else
            {
                ReveMessaged(socket, "未连接到远程主机!!", Flag.disconnect);
            }

        }
        public void Close()
        {
            if (socket != null)
            {
                if (socket.Connected)
                {
                    socket.Close();
                }
            }
            socket.Dispose();
            ReveMessaged(socket, "连接关闭成功!", Flag.disconnect);
        }
    }
   public enum Flag
    { 
        connect = 1,
        disconnect = 0
    }
}
