
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class UdpClient
    {
        private Socket socket;
        private EndPoint _ep;
        private byte[] _refData;
        private ArraySegment<byte> _refSegment;
        public static string Messages { get; set; }
        //回调事件
        public delegate void ReveMessageEventHandler(object sender, ReveMessageEventArgs args);
        public event ReveMessageEventHandler ReveMessaged;

        public void InitialUdpClient(IPAddress ip, int port = 6000)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _refData = new byte[4096];
            _refSegment = new ArraySegment<byte>(_refData);
            _ep = new IPEndPoint(ip, port);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);
            socket.Bind(_ep);
            //socket.Connect(_ep);
            
        }
        public void MessageLoop()
        {
            Task.Factory.StartNew(async () =>
            {
                SocketReceiveMessageFromResult res;
                while (true)
                {
                    res = await socket.ReceiveMessageFromAsync(_refSegment, SocketFlags.None, _ep);
                    Messages = Encoding.UTF8.GetString(_refData, 0, res.ReceivedBytes);
                    //触发回调事件
                    ReveMessaged(socket, new ReveMessageEventArgs(Messages));
                }

            });
        }
        public async Task Send(string ip,int port,byte[] data)
        {
            var s = new ArraySegment<byte>(data);
            EndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            await socket.SendToAsync(s, SocketFlags.None, endPoint);
        }

        public void Close()
        {
            if (socket != null)
            {
                if (socket.Connected)
                {
                    socket.Disconnect(true);
                }
                socket.Close();
            }
        }
    }
}
