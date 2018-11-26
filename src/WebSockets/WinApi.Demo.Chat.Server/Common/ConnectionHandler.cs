using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static WinApi.Demo.Signatures.Ws2_32;

namespace WinApi.Demo.Chat.Server.Common
{
    public class ConnectionHandler
    {
        public IntPtr Socket { get; private set; }

        public event EventHandler<IntPtr> ClientConnected;

        public void Connect(string ipaddr, ushort port)
        {
            WSAData wsaData;
            //RESULT OF MAKEWORD(2,2) MACRO -/ (512 + 2) -> 514
            //We indicate that we want to use WinSocket in version 2.2
            Int16 wVersionRequested = 0b0000_0010_0000_0010;
            int result = WSAStartup(wVersionRequested, out wsaData);
            if (result != 0)
            {
                Console.WriteLine("Initialization Error!");
                return;
            }
            Console.WriteLine("Initialization Succedeed!");

            Socket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
            if ((uint)Socket == INVALID_SOCKET)
            {
                Console.WriteLine($"Failed to create socket! Code: {WSAGetLastError()}");
                WSACleanup();
                return;
            }
            Console.WriteLine("Socket created!");

            sockaddr_in service = new sockaddr_in();
            service.sin_addr = new in_addr();
            service.sin_family = AF_INET;
            service.sin_addr.s_addr = (uint)IPAddress.Parse(ipaddr).Address;
            service.sin_port = port;

            if (bind(Socket, ref service, Marshal.SizeOf(service)) == SOCKET_ERROR)
            {
                Console.WriteLine($"Failed to bind socket! Code: {WSAGetLastError()}");
                closesocket(Socket);
                WSACleanup();
                return;
            }   
        }

        public async Task Listen()
        {
            if (listen(Socket, 1) == SOCKET_ERROR)
            {
                Console.WriteLine($"Socket listen error! Code: {WSAGetLastError()}");
                return;
            }

            Console.WriteLine("Beginning to listen!");
            while (true)
            {
                IntPtr acceptedSocket = (IntPtr)SOCKET_ERROR;
                while (acceptedSocket == (IntPtr)SOCKET_ERROR)
                {
                    acceptedSocket = accept(Socket, IntPtr.Zero, 0);
                }
                ClientConnected(this, acceptedSocket);
                Console.WriteLine("Client connected!");
            }
        }
    }
}
