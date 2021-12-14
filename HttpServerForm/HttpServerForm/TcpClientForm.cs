using Common;
using System;
using System.Drawing;
using System.Windows.Forms;
namespace HttpServerForm
{
    public partial class TcpClientForm : Form
    {
        private readonly string _path = AppDomain.CurrentDomain.BaseDirectory + "imgs//";
        private TcpClient tcpClient;
        public TcpClientForm()
        {
            InitializeComponent();
        }
        private  void Initial()
        {
            tcpClient = new TcpClient();
            tcpClient.ReveMessaged += TcpClient_ReveMessaged;
            tcpClient.InitialTcpClient(remoteIP.Text.Trim(),Convert.ToInt32(remotePort.Text.Trim()));
        }

        private void TcpClient_ReveMessaged(object sender, string message,Flag flag)
        {
            this.richTextBox1.BeginInvoke(new Action(() => {
                this.richTextBox1.Text = message;
            }));
            if (flag == Flag.connect)
            {
                this.pictureBox1.BeginInvoke(new Action(() => {
                    this.pictureBox1.Image = Bitmap.FromFile(_path + "c.png");
                }));
            }
            else
            {
                this.pictureBox1.BeginInvoke(new Action(() => {
                    this.pictureBox1.Image = Bitmap.FromFile(_path + "d.png");
                }));
            }
        }
 
        private void btnConnect_Click(object sender, EventArgs e)
        {
            Initial();
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            await tcpClient.SendTo(this.sendText.Text.Trim());
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            tcpClient.Close();
        }
    }
}
