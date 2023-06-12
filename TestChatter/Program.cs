using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;


namespace TestChatter
{

    public class Program
    {
        private static TcpClient client;
        private static TcpListener listener;


        private static string input;

        private static bool isRunning = false;
        private static IPAddress MyIP;


        private static Thread ConnectionListenerThread;
        private static Thread MessageListenerThread;

        
        static void Main(string[] args)
        {
            Console.WriteLine(" enter port number to init app on:- ");
            InitApp(Convert.ToInt32(Console.ReadLine()));
            while (isRunning)
            {
                askForInput();
            }
            Console.ReadKey();
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
        }

        private static void askForInput()
        {
            input = String.Empty;
            input = Console.ReadLine();

            string[] message = input.Split('-');
            if (message[0] == "connect")
            {
 
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
                    if (client != null)
                    {
                        client.GetStream().Write(Encoding.Default.GetBytes(input), 0, Encoding.Default.GetBytes(input).Length);
                        client.GetStream().Flush();
                    }
                    else
                        Console.WriteLine("no reciever");
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
                    if (client != null)
                    {
                        byte[] b = new byte[2048];
                        client.GetStream().Read(b, 0, 2048);
                        Console.WriteLine(Encoding.Default.GetString(b).TrimStart().TrimEnd());
                    }
                    else
                    {
                        Console.WriteLine("no sender ");
                    }
                }
                catch (Exception error) {  }
            }
        }

        private static void ListenConnection()
        {
            listener.Start();
            while (true)
            {
                try
                {
                    Console.WriteLine("listening");
                    client = listener.AcceptTcpClient();
                    Console.WriteLine(" connected to a machine ");
                }
                catch (Exception error) { Console.WriteLine(error.Message); }
            }
        }
    }
}
