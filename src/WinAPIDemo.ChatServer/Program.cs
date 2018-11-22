using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using static WinAPIDemo.WinAPISignatures.Ws2_32;

namespace WinAPIDemo.ChatServer
{
    class Program
    {
        static void Main(string[] args)
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

            IntPtr _socket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
            if ((uint)_socket == INVALID_SOCKET)
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
            service.sin_addr.s_addr = (uint)IPAddress.Parse("127.0.0.1").Address;
            service.sin_port = 1044;

            if (bind(_socket, ref service, Marshal.SizeOf(service)) == SOCKET_ERROR)
            {
                Console.WriteLine("FAILED TO CONNECT!");
                var err = WSAGetLastError();
                Debugger.Break();
                closesocket(_socket);
                return;
            }

            if (listen(_socket, 1) == SOCKET_ERROR)
            {
                Console.WriteLine("ERROR SZMATO!");
            }

            IntPtr acceptedSocket = (IntPtr)SOCKET_ERROR;
            Console.WriteLine("Waiting for a client to connect!");

            while (acceptedSocket == (IntPtr)SOCKET_ERROR)
            {
                acceptedSocket = accept(_socket, IntPtr.Zero, 0);
            }
            _socket = acceptedSocket;

            int iResult = 0;
            byte[] buffer = new byte[1024];
            do
            {
                iResult = recv(acceptedSocket, buffer, buffer.Length, 0);
                if (iResult > 0)
                {
                    Console.WriteLine($"Bytes recieved {iResult}");

                    string msg = Encoding.ASCII.GetString(buffer).TrimEnd('\0');
                    Console.WriteLine(msg);

                    byte[] buff = Encoding.ASCII.GetBytes(msg);

                    var iSendResult = send(acceptedSocket, buff, buff.Length, 0);
                    if (iSendResult == SOCKET_ERROR)
                    {
                        //ERROR
                        Console.WriteLine($"Error: {WSAGetLastError()}");
                        Debugger.Break();
                        closesocket(_socket);
                        WSACleanup();
                    }
                    Console.WriteLine($"Bytes sent {iSendResult}");
                }
                else if (iResult == 0)
                    Console.WriteLine("Connection closing");
                else
                {
                    //ERROR
                    Console.WriteLine($"Error: {WSAGetLastError()}");
                    Debugger.Break();
                    closesocket(_socket);
                    WSACleanup();
                }

            } while (iResult > 0);

            iResult = shutdown(acceptedSocket, 1);
            if (iResult == SOCKET_ERROR)
            {
                //ERROR
                var err = WSAGetLastError();
                Debugger.Break();
                closesocket(_socket);
                WSACleanup();
            }
        }
    }
}
