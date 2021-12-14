using CommonLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HttpServerForm
{
    public partial class UdpServerForm : Form
    {
        UdpServer udpServer =new UdpServer();
        public UdpServerForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            udpServer.InitialUdpServer(Convert.ToInt32(Port.Text.Trim()));
            udpServer.MessageLoop();
            //注册事件
            udpServer.ReveMessaged += UdpServer_ReveMessaged; ;
            this.richTextBox1.Text = "开始监听..";
        }
        //事件处理
        private void UdpServer_ReveMessaged(object sender, ReveMessageEventArgs args)
        {
            this.richTextBox1.Invoke(new Action(() =>
            {
                this.richTextBox1.Text = args.Message;
            }));
        }

         
        private void btnClose_Click(object sender, EventArgs e)
        {
            udpServer.ReveMessaged -= UdpServer_ReveMessaged;
            udpServer.Close();
            this.richTextBox1.Text = "资源已关闭...";
            this.sendtxt.Text = String.Empty;

        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
           await udpServer.SendTo(sendIP.Text.Trim(),Convert.ToInt32(sendPort.Text.Trim()),Encoding.UTF8.GetBytes(sendtxt.Text.Trim()));
        }

    }
}
