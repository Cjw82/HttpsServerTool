using Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Windows.Forms;

namespace HttpServerForm
{
    public partial class TcpServerForm : Form
    {
        private   readonly string _path = AppDomain.CurrentDomain.BaseDirectory + "imgs//";
        private TcpServer tcpServer;
        public TcpServerForm()
        {
            InitializeComponent();
           
        }
        private void Initial()
        {
            tcpServer = new TcpServer();
            tcpServer.ReveMessaged += TcpServer_ReveMessaged;
            tcpServer.ReFlshTcpCliented += TcpServer_ReFlshTcpCliented;
            tcpServer.InitialTcpServer(this.localIP.Text.Trim(),Convert.ToInt32(this.localPort.Text.Trim()));
            
        }

        private void TcpServer_ReFlshTcpCliented(List<TP> sockets)
        {
            this.comboBox1.BeginInvoke(new Action(() => {
                comboBox1.DataSource = null;
                comboBox1.DisplayMember = "Name";
                comboBox1.ValueMember = "Index";
                comboBox1.DataSource = sockets;
                comboBox1.Refresh();
            }));
           
        }

        private void TcpServer_ReveMessaged(object sender, string message,Flag flag)
        {
            this.richTextBox1.BeginInvoke(new Action(() => { 
              this.richTextBox1.Text = message;
            }));
            if (flag == Flag.connect)
            {
                this.pictureBox1.BeginInvoke(new Action(() => {
                    this.pictureBox1.Image = Bitmap.FromFile(_path + "c.png");
                }));
            }else
            {
                this.pictureBox1.BeginInvoke(new Action(() => {
                    this.pictureBox1.Image = Bitmap.FromFile(_path + "d.png");
                }));
            }
          
        }
        private async void btnSend_Click(object sender, EventArgs e)
        {
           await tcpServer.SendTo(this.sendTxt.Text.Trim(),Convert.ToInt32(this.comboBox1.SelectedValue.ToString()));
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            Initial();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            tcpServer.Close();
        }
    }
}
