using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;


namespace NotWhatsappServer
{
    public partial class Form1 : Form
    {

        private static IPAddress MyIP;
        private static int MyPort;

        private static TcpListener listener;
        private static TcpClient client;

        private static Thread ConnectionListenerThread;
        private static Thread MessageListenerThread;

        private static Dictionary<string,TcpClient> Clients;

        private static ListBox ServerMessages;

        private static CancellationToken ConnectionListenerCancelationToken;
        private static CancellationTokenSource ConnectionListenerCancelationTokenSource;

        private static int ClientCounter = 0;


        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {

            Clients = new Dictionary<string, TcpClient>();
            string hostName = Dns.GetHostName();
            // Get the IP addresses associated with the host name
            IPAddress[] ipAddresses = Dns.GetHostAddresses(hostName);
            MyIP = ipAddresses.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            MyPort = 4000;

            txtIP.Text = MyIP.ToString();
            txtPort.Text = MyPort + "";

            ConnectionListenerCancelationTokenSource = new CancellationTokenSource();
            ConnectionListenerCancelationToken = ConnectionListenerCancelationTokenSource.Token;


            listener = new TcpListener(MyIP,MyPort);
            ConnectionListenerThread = new Thread(() => ListenConnection(ConnectionListenerCancelationToken));
            MessageListenerThread = new Thread(() => ListenMessages(ConnectionListenerCancelationToken));

            ServerMessages = lsServerMessages;
            ConnectionListenerThread.Start();
            MessageListenerThread.Start();
        }


        private static Message LogToServerMessages(string jsonString)
        {
            try 
            {
                jsonString = jsonString.Replace("("," ").Replace(")"," ").TrimEnd().TrimStart();
                jsonString = jsonString.Replace("\0", "");
                //MessageBox.Show(jsonString);
                Message m = JsonSerializer.Deserialize<Message>(jsonString);
                if (m.type_of_message == "ack")
                {
                    ServerMessages.Invoke(new MethodInvoker(delegate ()
                    {
                        ServerMessages.Items.Add("(" + m.SenderIP + ")[" + DateTime.Now.ToString() + "] " + m.type_of_message);
                    }));

                    LogServerMessage("client added :- " + m.SenderIP);
                }
                else
                {
                    ServerMessages.Invoke(new MethodInvoker(delegate ()
                    {
                        ServerMessages.Items.Add("(" + m.SenderIP + ")[" + DateTime.Now.ToString() + "] " + m.content + " (to)--> " + m.RecieverIP);
                    }));

                    string IpAddress = m.RecieverIP.Split(':')[0];
                    TcpClient tcpClient = Clients[IpAddress];
                    if (tcpClient.Connected)
                    {
                        tcpClient.GetStream().Write(Encoding.Default.GetBytes(m.content), 0, Encoding.Default.GetBytes(m.content).Length);
                        tcpClient.GetStream().Flush();
                        LogServerMessage("send :- " + m.content + " to :- " + m.RecieverIP + " from :- " + m.SenderIP);
                    }
                }
                return m;
            }
            catch(Exception error)
            {
                MessageBox.Show(error.Message + "\n\n" + error.StackTrace);
                return null;
            }
        }

        private static void LogServerMessage(string message)
        {
            try
            {
                ServerMessages.Invoke(new MethodInvoker(delegate () {
                    ServerMessages.Items.Add("(server)[" + DateTime.Now.ToString() + "] " + message);
                }));
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message + "\n\n" + error.StackTrace);
            }

        }
        private static void ListenMessages(CancellationToken cancellationToken)
        {
            try
            {

                while (!cancellationToken.IsCancellationRequested)
                {
                    if (client != null)
                    {
                        byte[] byteMessage = new byte[4096];
                        int bytes_read = client.GetStream().Read(byteMessage, 0, 4096);
                        if (bytes_read > 0)
                            LogToServerMessages(Encoding.Default.GetString(byteMessage));                        
                    }
                }
            }
            catch (Exception error)
            {

            }        
        }


        private static void ListenConnection(CancellationToken cancellationToken)
        {
            try
            {
                listener.Start();
                LogServerMessage("started Listening ");
                MessageBox.Show("started Listening ");
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (listener.Pending())
                    {
                        byte[] messageBytes = new byte[4096];
                        client = listener.AcceptTcpClient();
                        client.GetStream().Read(messageBytes, 0, 4096);
                        Message m = LogToServerMessages(Encoding.Default.GetString(messageBytes));

                        Clients.Add(m.SenderIP,client);

                        LogServerMessage("connected");
                        LogServerMessage(Clients.Count + "");
                        foreach (KeyValuePair<string,TcpClient> i in Clients)
                            LogServerMessage((i.Value == null) + " " + i.Key);
                    }
                }
            }
            catch (Exception error)
            {
            }
        }



        private void btnStop_Click(object sender, EventArgs e)
        {
            //ConnectionListenerCancelationTokenSource.Cancel();
            //ConnectionListenerThread.Join();
            try
            {
                foreach (KeyValuePair<string, TcpClient> i in Clients)
                    i.Value.Close();
                Clients.Clear();
                listener.Stop();
                LogServerMessage("Stopped Listening");
                ConnectionListenerThread.Abort();
            }
            catch (Exception error) { }
            try
            {
                MessageListenerThread.Abort();
            }
            catch (Exception error) { }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                foreach (KeyValuePair<string, TcpClient> i in Clients)
                    i.Value.Close();
                Clients.Clear();
                listener.Stop();
                LogServerMessage("Stopped Listening");
                ConnectionListenerThread.Abort();
            }
            catch (Exception error) { }
            try
            {
                MessageListenerThread.Abort();
            }
            catch (Exception error) { }
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
