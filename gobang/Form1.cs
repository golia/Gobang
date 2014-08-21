using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;

namespace gobang
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string strHostName = string.Empty;
            string IP_Addr = "";
            strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            foreach (IPAddress ip in addr)
            {
                IP_Addr = ip.ToString();
                string[] temp = IP_Addr.Split('.');
                if (ip.AddressFamily == AddressFamily.InterNetwork && temp[0] == "10")
                {
                    break;
                }
            }
            textBox1.Text = IP_Addr;
            textBox2.Text = "1234";
        }
        private Color chess = Color.Black;
        TcpListener Server;
        Socket Client;
        Thread Th_Svr;
        Thread Th_Clt;
        int [,] Chess = new int[19, 19];

        private void ServerSub()
        {
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse(textBox1.Text), int.Parse(textBox2.Text));
            Server = new TcpListener(EP);
            Server.Start(100);
            while (true)
            {
                Client = Server.AcceptSocket();
                Th_Clt = new Thread(Listen);
                Th_Clt.IsBackground = true;
                Th_Clt.Start();
            }
        }

        private void Listen()
        {
            Socket sck = Client;
            Thread Th = Th_Clt;
            while (true)
            {
                try
                {
                    byte[] B = new byte[1023];
                    int inLen = sck.Receive(B);
                    string Msg = Encoding.Default.GetString(B, 0, inLen);
                    string Cmd = Msg.Substring(0, 1);
                    string Str = Msg.Substring(1);
                    switch (Cmd)
                    {
                        case "0":
                            MessageBox.Show("Send from " + Str);
                            break;
                        case "1":
                            MessageBox.Show("Send from " + Str);
                            break;
                        case "9":
                            Th.Abort();
                            break;
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void Send(string Str)
        {
            byte[] B = Encoding.Default.GetBytes(Str);
            Client.Send(B, 0, B.Length, SocketFlags.None);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            Th_Svr = new Thread(ServerSub);
            Th_Svr.IsBackground = true;
            Th_Svr.Start();
            button1.Enabled = false;
            button2.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string IP = textBox1.Text;
            int Port = int.Parse(textBox2.Text);
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse(IP), Port);

            Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                Client.Connect(EP);
                Th_Clt = new Thread(Listen);
                Th_Clt.IsBackground = true;
                Th_Clt.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot connect to server!");
                return;
            }
            button2.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            drawChessBoard();
            resetChessBoard();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (button2.Enabled == false)
            {
                Client.Close();
            }
        }

        private void drawChessBoard()
        {
            Graphics gs = panel1.CreateGraphics();
            gs.Clear(Color.White);
            for (int i = 0; i <= 400; i += 20)
            {
                Point x = new Point(i, 0);
                Point y = new Point(i, 400);
                gs.DrawLine(Pens.Black, x, y);
                x = new Point(0, i);
                y = new Point(400, i);
                gs.DrawLine(Pens.Black, x, y);
            }
        }

        private void resetChessBoard()
        {
            for (int i = 0; i < 19; i++)
            {
                for (int j = 0; j < 19; j++)
                {
                    Chess[i, j] = 0;
                }
            }
        }

    }
}
