using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace TestChatter2
{
    public class Program
    {
        private static TcpClient sender_Clients;
        private static TcpListener listener;

        private static TcpClient reciever_client;


        private static int ClientCounter = 0;

        private static bool isRunning = false;
        private static IPAddress MyIP;


        private static Thread ConnectionListenerThread;
        private static Thread MessageListenerThread;
        private static string input;
        static void Main(string[] args)
        {
            InitApp();
            while (isRunning)
            {
                askForInput();
            }
            Console.ReadKey();
        }

        private static void InitApp()
        {
            string hostName = Dns.GetHostName();
            // Get the IP addresses associated with the host name
            IPAddress[] ipAddresses = Dns.GetHostAddresses(hostName);
            MyIP = ipAddresses.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);


            listener = new TcpListener(MyIP, 1001);
            sender_Clients = new TcpClient();
            isRunning = true;
            ConnectionListenerThread = new Thread(new ThreadStart(ListenConnection));
            MessageListenerThread = new Thread(new ThreadStart(ListenMessage));
            ConnectionListenerThread.Start();
            MessageListenerThread.Start();
        }

        private static void askForInput()
        {
            input = String.Empty;
            input = Console.ReadLine();
            
            string[] message = input.Split("<->".ToCharArray());
            if (message[0] == "connect")
            {
                reciever_client = new TcpClient();
                reciever_client.Connect(message[1], 1000);
                Console.WriteLine("connected:- " + message[1]);
            }
            
            else if (message[0] == "close")
            {
                MessageListenerThread.Abort();
                ConnectionListenerThread.Abort();
                isRunning = false;
            }
            else
            {
                try
                {
                    if (reciever_client != null)
                    {
                        reciever_client.GetStream().Write(Encoding.Default.GetBytes(input), 0, Encoding.Default.GetBytes(input).Length);
                        reciever_client.GetStream().Flush();
                    }
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.Message + " " + error.StackTrace);
                }
            }
        }







        private static void ListenMessage()
        {
            while (true)
            {
                try
                {
                    if (sender_Clients != null)
                    {
                        byte[] b = new byte[2048];
                        sender_Clients.GetStream().Read(b, 0, 2048);
                        Console.WriteLine(" -[" + Encoding.Default.GetString(b) + "]");
                    }
                }
                catch (Exception error) {  }
            }
        }

        private static void ListenConnection()
        {
            listener.Start();
            Console.WriteLine("here");
            while (true)
            {
                try
                {
                    Console.WriteLine("listening1");
                    sender_Clients = listener.AcceptTcpClient();
                    Console.WriteLine(" connected to a machine ");
                }
                catch (Exception error) { Console.WriteLine(error.Message); }
            }
        }
    }
}


