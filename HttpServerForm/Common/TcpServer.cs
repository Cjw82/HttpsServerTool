using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class TcpServer
    {
        private Socket socket;
        private List<Socket> _socketAccepts =new List<Socket> ();
        private int Length = 0;
        private EndPoint _ep;
        private List<TP> tps = new List<TP> ();
        private byte[] _refData = new byte[2048];
        public static string Messages { get; set; }

        public delegate void ReveMessageEventHandler(object sender, string message,Flag flag);
        //回调函数
        public event ReveMessageEventHandler ReveMessaged;
        //刷新客户端个数
        public delegate void ReFlshTcpClients(List<TP> tp);
        public event ReFlshTcpClients ReFlshTcpCliented;
        public void InitialTcpServer(string IP,int Port)
        {
            try
            {
                socket = null;
                //创建监听的socket
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //设置IP和端口
                _ep = new IPEndPoint(IPAddress.Parse(IP), Port);
                // 绑定IP和端口
                socket.Bind(_ep);
                // 开启监听。。
                //设置监听队列的最大值
                socket.Listen(10);
                //连接成功
                ReveMessaged(socket, "开启监听...", Flag.disconnect);
                tps.Add(new TP(0,"全部"));
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        //新创建的连接
                       var socketAccept = socket.Accept();
                        //连接成功
                        if (socketAccept.Connected)
                        {
                            ReveMessaged(socket, "连接成功!!", Flag.connect);
                            _socketAccepts.Add(socketAccept);
                            tps.Add(new TP( _socketAccepts.Count(),"客户端"+ _socketAccepts.Count().ToString()));
                            ReFlshTcpCliented(tps);
                        }
                        //在开启一个线程不停的接收
                        Task.Factory.StartNew(() =>
                        {
                            while (true)
                            { 
                                Length = socketAccept.Receive(_refData);
                                if (Length == 0)
                                {
                                    break;
                                }
                                Messages = Encoding.UTF8.GetString(_refData, 0, Length);
                                ReveMessaged(socketAccept, Messages, Flag.connect);
                            }
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                ReveMessaged(null, "通讯中断", Flag.disconnect);
                throw ex;
            }           
        }
        public async Task SendTo(string str,int index)
        {
            var dt = new ArraySegment<byte>(Encoding.UTF8.GetBytes(str));
            if (_socketAccepts.Count>0)
            {
                if (index == 0)
                {
                    foreach (var item in _socketAccepts)
                    {
                        if ( item.Connected)
                        {
                            await item.SendAsync(dt, SocketFlags.None);
                        }
                        else
                        {
                            Messages = item.LocalEndPoint.AddressFamily.ToString() + "未连接到远程主机!!";
                            ReveMessaged(item, Messages, Flag.disconnect);
                        }
                    }
                }
                else
                {
                    -- index;
                    for (int i = 0; i < _socketAccepts.Count; i++)
                    {
                        if (i == index)
                        {
                            if (_socketAccepts[i].Connected)
                            {
                                await _socketAccepts[i].SendAsync(dt, SocketFlags.None);
                            }
                            else
                            {
                                Messages = _socketAccepts[i].LocalEndPoint.AddressFamily.ToString() + "未连接到远程主机!!";
                                ReveMessaged(_socketAccepts[i], Messages, Flag.disconnect);
                            }
                        }
                    }
                }

            }
        }

        public void Close()
        {
            if (_socketAccepts.Count > 0)
            {
                foreach (var item in _socketAccepts)
                {
                    if (item.Connected)
                        item.Close();
                    item.Dispose();
                }
            }
            if (socket != null)
            {
                if (socket.Connected)
                {
                    socket.Close();
                }
            }
            _socketAccepts.Clear();
            tps.Clear();
            socket.Dispose();
            ReveMessaged(null, "服务关闭成功!", Flag.disconnect);
        }
    }
    //用于区分Tcp客户端
    public class TP
    {
        public int index { get; set; }
        public string  name { get; set; }
        public TP(int Index,string Name)
        {
            this.index = Index;
            this.name = Name;   
        }
    }
}
