using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerData;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;

namespace Server
{
    class Server
    {
        static Socket listenerSocket;
        static List<ClientData> _clients;

        static void Main(string[] args) // start server
        {
            Console.WriteLine("Starting server on " + Packet.GetIP4Address());

            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clients = new List<ClientData>();

            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(Packet.GetIP4Address()), 4242);
            listenerSocket.Bind(ip);

            Thread listenThread = new Thread(ListenThread);
            listenThread.Start();
        }

        //Listener - listens for clients trying to connect
        static void ListenThread()
        {
            for (;;)
            {
                listenerSocket.Listen(0);
                _clients.Add(new ClientData(listenerSocket.Accept()));
            }
        }

        //clientdata thread - receives data from each client indiviually
        public static void Data_IN(object cSocket)
        {
            Socket clientSocket = (Socket)cSocket;
            byte[] Buffer;
            int readBytes;

            for (;;)
            {
                try
                {
                    Buffer = new byte[clientSocket.SendBufferSize];

                    readBytes = clientSocket.Receive(Buffer);

                    if (readBytes > 0)
                    {
                        //handle data
                        Packet packet = new Packet(Buffer);
                        DataManager(packet);
                    }
                }
                catch (SocketException)
                {
                    Console.WriteLine("A client disconnected!");
                    Console.ReadLine();
                    Environment.Exit(0);
                    //throw;
                }
            }
        }

        //data manager
        public static void DataManager (Packet p)
        {
            switch (p.packetType)
            {
                case PacketType.Chat:
                    foreach(ClientData c in _clients)
                        c.clientSocket.Send(p.ToBytes());
                    break;
            }
        }
    }

    class ClientData
    {
        public Socket clientSocket;
        public Thread clientThread;
        public string id;

        public ClientData()
        {
            id = Guid.NewGuid().ToString();
            clientThread = new Thread(Server.Data_IN);
            clientThread.Start(clientSocket);
            SendRegistrationPacket();
        }
        
        public ClientData(Socket clientSocket)
        {
            this.clientSocket = clientSocket;
            id = Guid.NewGuid().ToString();
            clientThread = new Thread(Server.Data_IN);
            clientThread.Start(clientSocket);
            SendRegistrationPacket();
        }

        public void SendRegistrationPacket()
        {
            Packet p = new Packet(PacketType.Registration, "server");
            p.Gdata.Add(id);
            clientSocket.Send(p.ToBytes());
        }
    }
}
