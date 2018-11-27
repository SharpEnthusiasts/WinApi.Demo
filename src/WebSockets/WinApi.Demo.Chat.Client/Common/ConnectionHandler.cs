using System;
using System.Net;
using System.Runtime.InteropServices;
using static WinApi.Demo.Signatures.Ws2_32;

namespace WinApi.Demo.Chat.Client.Common
{
    public class ConnectionHandler
    {
        public event EventHandler<string> OutputMessage;

        public IntPtr Connect(string ipaddr, string port)
        {
            WSAData wsaData;
            //RESULT OF MAKEWORD(2,2) MACRO -/ (512 + 2) -> 514
            //We indicate that we want to use WinSocket in version 2.2
            Int16 wVersionRequested = 0b0000_0010_0000_0010;
            int result = WSAStartup(wVersionRequested, //Version of WebSockets we want to use.
                out wsaData); //Data structure we recieve from function.
            if (result != 0)
            {
                OutputMessage(this, "Initialization Error!");
                return IntPtr.Zero;
            }
            OutputMessage(this, "Initialization Succedeed!");

            IntPtr _socket = socket(AF_INET, //Address family - AF_INET (IPv4).
                SOCK_STREAM, //Type of winsocket, stream is used for TCP and AF_INET or AF_INET6.
                IPPROTO_TCP); //Protocol type, used only if type of winsocket is RAW.
            if ((uint)_socket == INVALID_SOCKET)
            {
                OutputMessage(this, $"Failed to create socket! Code: {WSAGetLastError()}");
                WSACleanup(); //Terminates winsockets on all threads.
                return IntPtr.Zero;
            }
            OutputMessage(this, "Socket created!");

            sockaddr_in service = new sockaddr_in();
            service.sin_addr = new in_addr();
            service.sin_family = AF_INET;
            service.sin_addr.s_addr = (uint)IPAddress.Parse(ipaddr).Address;
            service.sin_port = ushort.Parse(port);

            var iResult = connect(_socket,  //We choose socket we want to connect. 
                ref service,  //Reference to the structure with IP Address and protocol.
                Marshal.SizeOf(service)); //Size of this structure.

            if (iResult == SOCKET_ERROR)
            {
                OutputMessage(this, "Connection failed!");
                closesocket(_socket); //We close the socket and free it.
                WSACleanup(); //Terminates winsockets on all threads.
                return IntPtr.Zero;
            }
            OutputMessage(this, "Connected!");
            return _socket;
        }

        public void Disconnect(Model.Client client)
        {
            shutdown(client.Socket, 2); //We shutdown the socket, both ways send/recv.
            closesocket(client.Socket); //We close the socket and free it.
            WSACleanup(); //Terminates winsockets on all threads.
            OutputMessage(this, "Disconnected!");
        }
    }
}
