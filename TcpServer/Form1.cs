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
using System.Collections;
namespace TcpServer
{
    //[Server]
    public partial class Form1 : Form
    {
        TcpListener ServerListener = null;
        Thread ServerListenerThread = null;
        Thread ClientListenerThread = null;
        Hashtable Htable = null;
        string TcpServerListenerIP
        {
            get
            {
                return TextBox1.Text;
            }
        }
        int TcpServerListenerPort
        {
            get
            {
                return  Convert.ToInt32( TextBox2.Text);
            }
        }

        private string MyIP()
        {
            string hn = Dns.GetHostName();
            IPAddress[] ip = Dns.GetHostEntry(hn).AddressList;
            foreach(IPAddress it in ip)
            {
                if (it.AddressFamily == AddressFamily.InterNetwork)
                {
                    return it.ToString();
                }
            }
            return "";
        }
        public Form1()
        {
            InitializeComponent();
            Htable = new Hashtable();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TextBox1.Text = MyIP();
        }

        private void Button1_Click(object sender, EventArgs e)
        {

            
            ServerListenerThread = new Thread(ServerListenStarter);
            ServerListenerThread.Start();
            Button1.Enabled = false;


        }

        private void ServerListenStarter()
        {
            CheckForIllegalCrossThreadCalls = false;
            ServerListener = new TcpListener(IPAddress.Parse(this.TcpServerListenerIP), this.TcpServerListenerPort);
            ServerListener.Start(100);
            while (true)
            {
               var client= ServerListener.AcceptSocket();

                ClientListenerThread = new Thread(ClientListenStarter);
                ClientListenerThread.Start(new object[] { client, ClientListenerThread });
            }
        }

        private void ClientListenStarter(object obj)
        {
            var param = (object[])obj;
            var thisThread = (Thread)param[1];
            var thisSocket = (Socket)param[0];
            while (true)
            {
                try
                {
                    byte[] B = new byte[1023];
                    int inLine = thisSocket.Receive(B);
                    string message = Encoding.Default.GetString(B, 0, inLine);
                    string command = message.Substring(0, 1);
                    string str = message.Substring(1);
                    thisSocket.Send(B);
                    switch (command)
                    {
                        case "0":
                            Htable.Add(str, thisSocket);
                            ListBox1.Items.Add(str);
                            break;
                        case "9":
                            Htable.Remove(str);
                            ListBox1.Items.Remove(str);
                            thisThread.Abort();
                            break;
                        case "1":
                            SendAll(message);
                            break;
                        default:
                            string[] C = str.Split('|');
                            SentTo(command + C[0], C[1]);
                            break;
                            
                    }
                }
                catch(Exception ex)
                {
                    
                }
            }
        }

        /// <summary>線上名單</summary>
        /// <returns></returns>
        private string OnlineList()
        {
            string L = "L";
            for(int i=0;i<ListBox1.Items.Count;i++)
            {
                L += ListBox1.Items[i];
                if (i < ListBox1.Items.Count - 1) { L += ","; }
            }
            return L;
        }

        /// <summary>指定發送</summary>
        /// <param name="str"></param>
        /// <param name="user"></param>
        private void SentTo(string str,string user)
        {
            byte[] b = Encoding.Default.GetBytes(str);
            Socket sck = (Socket)Htable[user];
            sck.Send(b, 0, b.Length, SocketFlags.None);
        }
        /// <summary>廣播</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendAll(string str)
        {
            byte[] B = Encoding.Default.GetBytes(str);
            foreach(Socket s in Htable.Values)
            {
                s.Send(B, 0, B.Length, SocketFlags.None);
            }
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.ExitThread();
        }
    }
}
