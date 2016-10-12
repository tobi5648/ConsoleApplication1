using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApplication1;

namespace ConsoleApplication1
{
    class Program
    {
        Form1 fm = new Form1();
        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(8888);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;
            serverSocket.Start();
            Console.WriteLine("Chat server Started ....");
            counter = 0;
            while()
        }
    }
}
