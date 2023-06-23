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


            SetMyIP();
        }

        private void SetMyIP()
        {
            try
            {
                string hostName = Dns.GetHostName();
                IPAddress[] ipAddresses = Dns.GetHostAddresses(hostName);
                MyIP = ipAddresses.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

                // for listening incoming connection
                ConnectionListenerThread = new Thread(() => ListenConnection());
                // for listening incoming message
                MessageListenerThread = new Thread(() => ListenMessage());
                txtYourPort.Text = "1000";
                client = new TcpClient();
            }
            catch (Exception error) { }
        }


        private static void InitApp(int port_number)
        {
            listener = new TcpListener(MyIP, port_number);
            client = new TcpClient();
            isRunning = true;

            ConnectionListenerThread = new Thread(() => ListenConnection());
            MessageListenerThread = new Thread(() => ListenMessage());

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
                        if (client.Connected)
                        {
                            if (client.Client.Available > 0)
                            {
                                byte[] b = new byte[2048];
                                client.GetStream().Read(b, 0, 2048);
                                LogMessageToMessages("user:-", Encoding.Default.GetString(b).TrimStart().TrimEnd());
                            }
                        }
                    }
                    else
                        MessageBox.Show("no sender");
                }
                catch (Exception error) { }
            }
        }

        private static void LogMessageToMessages(string Sender,string message)
        {
            try
            {
                message = message.Trim();
                if (message != "")
                {
                    if (Message.InvokeRequired)
                        Messages.Invoke(new Action<string>(AddToMessages), "( " + Sender + " ) - [ " + DateTime.Now.ToString() + " ] --> " + message);
                    else
                        Messages.Items.Add("( " + Sender + " ) - [ " + DateTime.Now.ToString() + " ] --> " + message);
                }
            }
            catch (Exception error)
            {
                client.Close();
                client.Dispose();
                client = new TcpClient();
            }
        }

        private static void AddToMessages(string message)
        {
            Messages.Items.Add(message);
        }

        private static void ListenConnection()
        {
            TcpClient tempClient = null;

            listener.Start();
            MessageBox.Show("listening started");
            LogMessageToMessages("me", "listening started");
            while (true)
            {
                try
                {
                    tempClient = listener.AcceptTcpClient();
                    if (tempClient != null)
                    {
                        DialogResult result = MessageBox.Show("a machine wants to connect to you, do you want connect with it?","connection request",MessageBoxButtons.YesNo,MessageBoxIcon.Information);
                        if (result == DialogResult.Yes)
                        {
                            client = tempClient;
                            MessageBox.Show(" connected to a machine ");
                            LogMessageToMessages("me", "connected to a machine");                            
                        }
                        else
                        { 
                            tempClient.Close();
                            tempClient.Dispose();
                        }
                    }

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
                //MessageBox.Show(clientIP);
                string[] ip_addressAndPort = clientIP.Split(':');
                //MessageBox.Show(ip_addressAndPort[0] + "\n" + ip_addressAndPort[1]);
                client.Connect(ip_addressAndPort[0], Convert.ToInt32(ip_addressAndPort[1]));

                if (client.Connected)
                {
                    MessageBox.Show("connected to:- " + clientIP);
                    LogMessageToMessages("me", "connected to:- " + clientIP);
                }
                else 
                {
                    MessageBox.Show("client refused to connect:- " + clientIP);
                    LogMessageToMessages("me", "client refused to connect:- " + clientIP);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("unable to connect to the client,please enter client ip and port number in this manner CLIENT_IP:PORT_NUMBER for ex:- 10.2.18.50:1000", "Connection error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                ClientIPAndPort.Clear();
            }
        }

        private static void ConnectToServer()
        {
            try
            {
                string clientIP = ServerIpAndPort.Text;
                string[] ip_addressAndPort = clientIP.Split(':');
                client.Connect(ip_addressAndPort[0], Convert.ToInt32(ip_addressAndPort[1]));
                LogMessageToMessages("me", "connected to:- server");

                //MessageBox.Show(clientIP);
                //MessageBox.Show(ip_addressAndPort[0] + "\n" + ip_addressAndPort[1]);
                //MessageBox.Show("connected to:- server");
                
                Message ack = new Message()
                {
                    type_of_message = "ack",
                    SenderIP = MyIP.ToString(),
                    RecieverIP = "me",
                    content = "hi",
                    timestamp = DateTime.Now.ToString()
                };

                string json_string = JsonSerializer.Serialize(ack);

                //MessageBox.Show(json_string);
                
                client.GetStream().Write(Encoding.Default.GetBytes("(" + json_string + ") " + Message.Text), 0, Encoding.Default.GetBytes("(" + json_string + ") " + Message.Text).Length);
                client.GetStream().Flush();
                
                LogMessageToMessages("me", "ack " + json_string);
            }
            catch (Exception error)
            {
                MessageBox.Show("unable to connect to the server,please enter server ip and port number in this manner SERVER_IP:PORT_NUMBER for ex:- 10.2.18.50:1000","Connection error!",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                ServerIpAndPort.Clear();
            }
        }


        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (client.Connected)
            {
                DialogResult result = MessageBox.Show($"you are currently connected to a client, are you sure want to connect {ClientIPAndPort.Text}?", "warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    client.Close();
                    client.Dispose();
                    client = new TcpClient();
                    ConnectToClient();
                    isConnectedToServer = false;


                    if (MessageListenerThread.ThreadState != ThreadState.Running)
                        MessageListenerThread.Start();
                }
            }
            else
            {
                ConnectToClient();
                isConnectedToServer = false;

                if (MessageListenerThread.ThreadState != ThreadState.Running)
                    MessageListenerThread.Start();
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (client != null)
                {
                    if (Message.Text == "")
                        return;

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
                    MessageBox.Show("No reciever!", "Reciever error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception error)
            {
                MessageBox.Show(" No Connection!", "Connection error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private static void CloseThreads()
        {
            try
            {
                if (MessageListenerThread != null)
                    MessageListenerThread.Abort();


                if (client != null)
                {
                    client.Close();
                    client.Dispose();
                }

                if(listener != null)
                    listener.Stop();

                if (ConnectionListenerThread != null)
                    ConnectionListenerThread.Abort();

                if (listener != null)
                    listener = null;

            }
            catch (Exception error) { }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
           CloseThreads();
        }

        private void btnConnectToServer_Click(object sender, EventArgs e)
        {
            if (client.Connected)
            {
                DialogResult result = MessageBox.Show($"you are currently connected to a client, are you sure want to connect to this server {ClientIPAndPort.Text}?", "warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    client.Close();
                    client.Dispose();
                    client = new TcpClient();
                    ConnectToServer();
                    isConnectedToServer = true;


                    if (MessageListenerThread.ThreadState != ThreadState.Running)
                        MessageListenerThread.Start();
                }
            }
            else
            {
                ConnectToServer();
                isConnectedToServer=true;

                if (MessageListenerThread.ThreadState != ThreadState.Running)
                    MessageListenerThread.Start();
            }
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
