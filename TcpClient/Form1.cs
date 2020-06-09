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
namespace TcpClient
{
    public partial class Form1 : Form
    {
        Socket T;
        string User;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string IP = textBox1.Text;
            int port = int.Parse(textBox2.Text);
            IPEndPoint Ep = new IPEndPoint(IPAddress.Parse(IP), port);
            T = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            User = textBox3.Text;
            try
            {
                T.Connect(Ep);
               Send("0" + User);
                button1.Enabled = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
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
    }
}
