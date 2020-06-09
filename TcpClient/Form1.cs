using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TcpClient
{
    public partial class Form1 : Form
    {
        Socket T;
        string User;
        Thread Th;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            User = textBox3.Text;
            string IP = textBox1.Text;
            int port = int.Parse(textBox2.Text);
            textBox4.Text = "";
            try
            {
                IPEndPoint Ep = new IPEndPoint(IPAddress.Parse(IP), port);
                T = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                T.Connect(Ep);
                Th = new Thread(Listen);
                Th.IsBackground = false;
                Th.Start();
             //   Send("0" + User);
                button1.Enabled = false;
                button2.Enabled = true;
            }
            catch(Exception ex)
            {
                textBox4.Text = "無法連上伺服器！" + "\r\n"; //連線失敗時顯示訊息 
                return;
            }
           
        }

        private void Send(string str)
        {
            byte[] B = Encoding.Default.GetBytes(str);
            T.Send(B, 0, B.Length, SocketFlags.None);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (button1.Enabled)
            {
                Send("9" + User);
                T.Close();
            }
        }

        private void Listen()
        {
            while (true)
            {
                byte[] B = new byte[1023];
                int inLine = T.Receive(B);//從Server端回復
                string message = Encoding.Default.GetString(B, 0, inLine);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            byte[] B = Encoding.Default.GetBytes(textBox5.Text);
            T.Send(B, 0, B.Length, SocketFlags.None);
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
