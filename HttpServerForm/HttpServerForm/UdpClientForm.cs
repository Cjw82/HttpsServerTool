using CommonLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HttpServerForm
{
    public partial class UdpClientForm : Form
    {
        UdpClient udpClient =new UdpClient();
        public UdpClientForm()
        {
            InitializeComponent();
        }
 
        private void Connect_Click(object sender, EventArgs e)
        {
            udpClient.InitialUdpClient(IPAddress.Parse(IP.Text.Trim()), Convert.ToInt32(LisentPort.Text.Trim()));
            udpClient.MessageLoop();
            udpClient.ReveMessaged += UdpClient_ReveMessaged;
            this.richTextBox1.Text = "初始化成功...";
        }
        private void UdpClient_ReveMessaged(object sender, ReveMessageEventArgs args)
        {
            this.richTextBox1.BeginInvoke(new Action(() => { 
                this.richTextBox1.Text = args.Message; 
            }));
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            await udpClient.Send(IP.Text.Trim(), Convert.ToInt32(sendPort.Text.Trim()), Encoding.UTF8.GetBytes(this.Sendtxt.Text.Trim()));
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            udpClient.ReveMessaged -= UdpClient_ReveMessaged;
            udpClient.Close();
            this.richTextBox1.Text = "资源已关闭...";
            this.Sendtxt.Text = "";
        }

    }
}
