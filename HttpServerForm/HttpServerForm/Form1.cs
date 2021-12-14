using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HttpServerForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        // UDP 服务端
        private void button1_Click(object sender, EventArgs e)
        {
            UdpServerForm udpServerForm = new UdpServerForm();
            udpServerForm.Show();
        }
        // UDP 客户端
        private void button2_Click(object sender, EventArgs e)
        {
            UdpClientForm udpClientForm = new UdpClientForm();
            udpClientForm.Show();
        }
        // Tcp服务端
        private void button3_Click(object sender, EventArgs e)
        {
            TcpServerForm tcpServer = new TcpServerForm();
            tcpServer.Show();
        }
        // Tcp客户端
        private void button4_Click(object sender, EventArgs e)
        {
            TcpClientForm tcpClient = new TcpClientForm();
            tcpClient.Show();
        }
    }
}
