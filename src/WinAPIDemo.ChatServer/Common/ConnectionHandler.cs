using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static WinAPIDemo.WinAPISignatures.Ws2_32;

namespace WinAPIDemo.ChatServer.Common
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
                Console.WriteLine("INVALID_SOCKET");
                WSACleanup();
                return;
            }
            Console.WriteLine("Socket initialized.");

            //uint _socket = socket2(AF_INET, SOCK_STREAM, IPPROTO_TCP);
            //IntPtr _socket = WSASocket(AF_INET, SOCK_STREAM, IPPROTO_TCP, IntPtr.Zero, 0x01, 0x01);
            //if ((uint)_socket == INVALID_SOCKET)
            //{
            //    TextBlock.Text += "INVALID_SOCKET\n";
            //    WSACleanup();
            //    return;
            //}
            //TextBlock.Text += "Socket initialized.\n";

            sockaddr_in service = new sockaddr_in();
            service.sin_addr = new in_addr();
            service.sin_family = AF_INET;
            service.sin_addr.s_addr = (uint)IPAddress.Parse(ipaddr).Address;
            service.sin_port = port;

            if (bind(Socket, ref service, Marshal.SizeOf(service)) == SOCKET_ERROR)
            {
                Console.WriteLine("FAILED TO CONNECT!");
                var err = WSAGetLastError();
                closesocket(Socket);
                return;
            }   
        }

        public async Task Listen()
        {
            if (listen(Socket, 1) == SOCKET_ERROR)
            {
                Console.WriteLine("ERROR SZMATO!");
            }

            while (true)
            {
                IntPtr acceptedSocket = (IntPtr)SOCKET_ERROR;
                Console.WriteLine("Waiting for a client to connect!");

                while (acceptedSocket == (IntPtr)SOCKET_ERROR)
                {
                    acceptedSocket = accept(Socket, IntPtr.Zero, 0);
                }
                ClientConnected(this, acceptedSocket);
            }

            //IntPtr acceptedSocket = (IntPtr)SOCKET_ERROR;
            //Console.WriteLine("Waiting for a client to connect!");

            //while (acceptedSocket == (IntPtr)SOCKET_ERROR)
            //{
            //    acceptedSocket = accept(Socket, IntPtr.Zero, 0);
            //}
            //ClientConnected(this, acceptedSocket);
        }
    }
}
