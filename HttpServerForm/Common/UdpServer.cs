using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CommonLibrary
{
    public class UdpServer
    {
        private  Socket socket;
        private  EndPoint _ep;
        private  byte[] _refData;
        private  ArraySegment<byte> _refSegment;
        public static string Messages { get; set; }
        public  delegate void ReveMessageEventHandler(object sender, ReveMessageEventArgs args);
        //回调函数
        public   event ReveMessageEventHandler ReveMessaged;
        
        public  void InitialUdpServer(int Port = 5000)
        {
           
            _refData = new byte[4096];
            _refSegment = new ArraySegment<byte>(_refData);
            _ep = new IPEndPoint(IPAddress.Any, Port); 
            socket = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.IP,SocketOptionName.PacketInformation,true);
            socket.Bind(_ep);
             
        }
        //引发事件的函数
        public  void  MessageLoop()
        {
            Task.Factory.StartNew(async() => {
                SocketReceiveMessageFromResult res;
                while (true)
                {
                    res = await socket.ReceiveMessageFromAsync(_refSegment,SocketFlags.None,_ep);
                    Messages = Encoding.UTF8.GetString(_refData,0,res.ReceivedBytes);
                    //引发事件
                    ReveMessaged(socket,new ReveMessageEventArgs(Messages));
                    //socket.SendTo(Encoding.UTF8.GetBytes("服务器数据"), SocketFlags.None, res.RemoteEndPoint);
                }
            
            });
        }
        public  async Task SendTo(string ip, int port, byte[] data)
        {
            var dt = new ArraySegment<byte>(data);
            EndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            await socket.SendToAsync(dt, SocketFlags.None, endPoint);
        }
        public  void Close()
        {
            if (socket !=null)
            {
                if (socket.Connected)
                {
                    socket.Disconnect(true);
                }
                socket.Close();
            }
        }
    }
    public class ReveMessageEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public ReveMessageEventArgs(string message)
        {
            Message = message;
        }
    }
}