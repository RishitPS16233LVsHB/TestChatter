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

        private static Dictionary<string,TcpClient> Clients;

        private static ListBox ServerMessages;


        private static Dictionary<string,Thread> ClientsMessageListenerThread;

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

            ClientsMessageListenerThread = new Dictionary<string, Thread>();

            listener = new TcpListener(MyIP,MyPort);
            ConnectionListenerThread = new Thread(() => ListenConnection());

            ServerMessages = lsServerMessages;
            ConnectionListenerThread.Start();
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
        private static void ListenMessages(TcpClient MyClient)
        {
            try
            {

                while (true)
                {
                    if (MyClient != null)
                    {
                        byte[] byteMessage = new byte[4096];
                        int bytes_read = MyClient.GetStream().Read(byteMessage, 0, 4096);
                        if (bytes_read > 0)
                            LogToServerMessages(Encoding.Default.GetString(byteMessage));
                    }
                }
            }
            catch (Exception error)
            {

            }        
        }


        private static void ListenConnection()
        {
            try
            {
                listener.Start();
                LogServerMessage("started Listening ");
                while (true)
                {
                    if (listener.Pending())
                    {
                        byte[] messageBytes = new byte[4096];
                        client = listener.AcceptTcpClient();

                        client.GetStream().Read(messageBytes, 0, 4096);
                        Message m = LogToServerMessages(Encoding.Default.GetString(messageBytes));
                        
                        ClientCounter++;
                        Clients.Add(m.SenderIP+"-"+ClientCounter,client);


                        //individual thread creation for each instance of not whatsapp application
                        Thread tempThread = new Thread(() => ListenMessages(client));
                        ClientsMessageListenerThread.Add(m.SenderIP + "-" + ClientCounter, tempThread);
                        tempThread.Start();

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
            try
            {
                //MessageListenerThread.Abort();
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
            }
            catch (Exception error) { }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                foreach (KeyValuePair<string,Thread> t in ClientsMessageListenerThread)
                    t.Value.Abort();

                foreach (KeyValuePair<string, TcpClient> i in Clients)
                    i.Value.Close();

                ClientsMessageListenerThread.Clear();
                Clients.Clear();

                listener.Stop();
                LogServerMessage("Stopped Listening");
                ConnectionListenerThread.Abort();
            }
            catch (Exception error) { }
            try
            {
                //MessageListenerThread.Abort();
            }
            catch (Exception error) { }
        }


        public static void CheckAllConnections()
        {
            try
            {
                List<string> disconnectedClients = new List<string>();

                if (Clients != null)
                {
                    foreach (KeyValuePair<string, TcpClient> i in Clients)
                    {
                        if (i.Value.Client.Poll(0, SelectMode.SelectRead) && i.Value.Available == 0)
                        {
                            LogServerMessage(i.Key + " is not connected now");
                            i.Value.Close();
                            disconnectedClients.Add(i.Key);
                            ClientsMessageListenerThread[i.Key].Abort();
                            ClientsMessageListenerThread.Remove(i.Key);
                            LogServerMessage(i.Key + " is not in session anymore");
                        }
                    }

                    // Remove disconnected clients outside the loop
                    foreach (string key in disconnectedClients)
                    {
                        Clients.Remove(key);
                    }
                }
            }
            catch (Exception error)
            {
                LogServerMessage(error.Message);
            }
        }





        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Interval = 5000;
            timer1.Tick += Timer1_Tick;
            timer1.Start();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            CheckAllConnections();
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
