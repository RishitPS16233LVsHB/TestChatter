using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;

namespace NotWhatsapp
{
    public partial class Form1 : Form
    {
        private static TcpClient client;
        private static TcpListener listener;


        private static string input;

        private static bool isConnectedToServer = false;

        private static bool isRunning = false;
        private static IPAddress MyIP;


        private static Thread ConnectionListenerThread;
        private static Thread MessageListenerThread;


        private static ListBox Messages;
        private static ListBox IPs;
        private static TextBox Message;
        private static TextBox YourPort;
        private static TextBox ClientIPAndPort;
        private static TextBox ServerIpAndPort;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Messages = lsMessages;
            IPs = lsClients;
            Message = txtMessage;
            YourPort = txtYourPort;
            ClientIPAndPort = txtClientIPAndPort;
            ServerIpAndPort = txtServerIpAndPort;
        }


        private static void InitApp(int port_number)
        {
            string hostName = Dns.GetHostName();
            // Get the IP addresses associated with the host name
            IPAddress[] ipAddresses = Dns.GetHostAddresses(hostName);
            MyIP = ipAddresses.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);


            listener = new TcpListener(MyIP, port_number);
            client = new TcpClient();
            isRunning = true;

            ConnectionListenerThread = new Thread(new ThreadStart(ListenConnection));
            MessageListenerThread = new Thread(new ThreadStart(ListenMessage));

            ConnectionListenerThread.Start();
            MessageListenerThread.Start();

            MessageBox.Show("started listening for machine ip :- " +MyIP.ToString()+ " on :- " + port_number);
            LogMessageToMessages("me","started listening for machine ip :- " + MyIP.ToString() + " on :- " + port_number);
        }

        


        private static void ListenMessage()
        {
            while (true)
            {
                try
                {
                    if (client != null)
                    {
                        
                        byte[] b = new byte[2048];
                        client.GetStream().Read(b, 0, 2048);
                        LogMessageToMessages("user:-",Encoding.Default.GetString(b).TrimStart().TrimEnd());
                    }
                    else
                    {
                        MessageBox.Show("no sender ");
                    }
                }
                catch (Exception error) { }
            }
        }

        private static void LogMessageToMessages(string Sender,string message)
        {
            try
            {
                if (Message.InvokeRequired)
                    Messages.Invoke(new Action<string>(AddToMessages), "( " + Sender + " ) - [ " + DateTime.Now.ToString() + " ] --> " + message);
                else
                    Messages.Items.Add("( " + Sender + " ) - [ " + DateTime.Now.ToString() + " ] --> " + message);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message + "\n\n" + error.StackTrace);
            }
        }

        private static void AddToMessages(string message)
        {
            Messages.Items.Add(message);
        }

        private static void ListenConnection()
        {
            listener.Start();
            MessageBox.Show("listening started");
            LogMessageToMessages("me", "listening started");
            while (true)
            {
                try
                {
                    client = listener.AcceptTcpClient();
                    MessageBox.Show(" connected to a machine ");
                    LogMessageToMessages("me", "connected to a machine");
                }
                catch (Exception error) { Console.WriteLine(error.Message); }
            }
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            CloseThreads();
            InitApp(Convert.ToInt32(YourPort.Text));
        }

        private static void ConnectToClient()
        {
            try
            {
                string clientIP = ClientIPAndPort.Text;
                MessageBox.Show(clientIP);
                string[] ip_addressAndPort = clientIP.Split(':');
                MessageBox.Show(ip_addressAndPort[0] + "\n" + ip_addressAndPort[1]);
                client.Connect(ip_addressAndPort[0], Convert.ToInt32(ip_addressAndPort[1]));
                MessageBox.Show("connected to:- " + clientIP);
                LogMessageToMessages("me", "connected to:- " + clientIP);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message + "\n\n" + error.StackTrace);
            }        
        }



        private static void ConnectToServer()
        {
            try
            {

                string clientIP = ServerIpAndPort.Text;
                MessageBox.Show(clientIP);
                string[] ip_addressAndPort = clientIP.Split(':');
                MessageBox.Show(ip_addressAndPort[0] + "\n" + ip_addressAndPort[1]);
                client.Connect(ip_addressAndPort[0], Convert.ToInt32(ip_addressAndPort[1]));
                MessageBox.Show("connected to:- server");
                LogMessageToMessages("me", "connected to:- server");
                Message ack = new Message()
                {
                    type_of_message = "ack",
                    SenderIP = MyIP.ToString(),
                    RecieverIP = "me",
                    content = "hi",
                    timestamp = DateTime.Now.ToString()
                };

                string json_string = JsonSerializer.Serialize(ack);
                MessageBox.Show(json_string);
                client.GetStream().Write(Encoding.Default.GetBytes("(" + json_string + ") " + Message.Text), 0, Encoding.Default.GetBytes("(" + json_string + ") " + Message.Text).Length);
                client.GetStream().Flush();
                LogMessageToMessages("me", "ack " + json_string);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message + "\n\n" + error.StackTrace);
            }
        }


        private void btnConnect_Click(object sender, EventArgs e)
        {
            ConnectToClient();
            
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (client != null && Message.Text != "")
                {
                    string message = "(" + ClientIPAndPort.Text + ") " + Message.Text;
                    Message m = new Message()
                    {
                        SenderIP = MyIP.ToString(),
                        RecieverIP = ClientIPAndPort.Text,
                        content = Message.Text,
                        type_of_message = "message",
                        timestamp = DateTime.Now.ToString()
                    };

                    if (isConnectedToServer)
                        message = JsonSerializer.Serialize(m);

                        client.GetStream().Write(Encoding.Default.GetBytes(message), 0, Encoding.Default.GetBytes(message).Length);
                        client.GetStream().Flush();
                        LogMessageToMessages("me",Message.Text);
                        Message.Clear();
                }
                else
                    MessageBox.Show("no reciever");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message + "\n\n" + error.StackTrace);
            }
        }

        private static void CloseThreads()
        {
            try
            {
                if (ConnectionListenerThread != null)
                    ConnectionListenerThread.Abort();
                if (MessageListenerThread != null)
                    MessageListenerThread.Abort();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message + "\n\n" + error.StackTrace);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if(listener != null)
                    listener.Stop();
                if(client != null)
                    client.Close();
                if(ConnectionListenerThread != null)
                    ConnectionListenerThread.Abort();
                
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message + "\n\n" + error.StackTrace);
            }
            try
            {
                if(MessageListenerThread != null)
                    MessageListenerThread.Abort();
            }
            catch (Exception error1) { }
        }

        private void btnConnectToServer_Click(object sender, EventArgs e)
        {
            ConnectToServer();
            isConnectedToServer = true;

        }
    }
    public class Message
    {
        public string type_of_message { get; set; }
        public string SenderIP { get; set; }
        public string RecieverIP { get; set; }
        public string content { get; set; }
        public string timestamp { get; set; }
    }
}
