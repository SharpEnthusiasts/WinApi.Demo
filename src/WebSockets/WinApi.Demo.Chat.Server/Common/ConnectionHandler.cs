using System;
using System.Net;
using System.Runtime.InteropServices;
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
            int result = WSAStartup(wVersionRequested, //Version of WebSockets we want to use.
                out wsaData); //Data structure we recieve from function.
            if (result != 0)
            {
                Console.WriteLine("Initialization Error!");
                return;
            }
            Console.WriteLine("Initialization Succedeed!");

            Socket = socket(AF_INET, //Address family - AF_INET (IPv4).
                SOCK_STREAM, //Type of winsocket, stream is used for TCP and AF_INET or AF_INET6.
                IPPROTO_TCP); //Protocol type, used only if type of winsocket is RAW.

            if ((uint)Socket == INVALID_SOCKET)
            {
                Console.WriteLine($"Failed to create socket! Code: {WSAGetLastError()}");
                WSACleanup(); //Terminates winsockets on all threads.
                return;
            }
            Console.WriteLine("Socket created!");

            sockaddr_in service = new sockaddr_in();
            service.sin_addr = new in_addr();
            service.sin_family = AF_INET;
            service.sin_addr.s_addr = (uint)IPAddress.Parse(ipaddr).Address;
            service.sin_port = port;

            if (bind(Socket, //We choose socket we want to bind. 
                ref service, //Reference to the structure with IP Address and protocol.
                Marshal.SizeOf(service) //Size of this structure.
                ) == SOCKET_ERROR)
            {
                Console.WriteLine($"Failed to bind socket! Code: {WSAGetLastError()}");
                closesocket(Socket); //We close the socket and free it.
                WSACleanup(); //Terminates winsockets on all threads.
                return;
            }   
        }

        public async Task Listen()
        {
            if (listen(Socket, //Socket we want to listen on.
                1 //Maximum length of the queue of pending connections.
                ) == SOCKET_ERROR)
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
                    acceptedSocket = accept(Socket, //We choose on what socket we want to accept.
                        IntPtr.Zero, //Optional pointer to a buffer that recieves address of connecting entity.
                        0); //Optional pointer to an integer that contains length of structure from param above.
                }
                ClientConnected(this, acceptedSocket);
                Console.WriteLine("Client connected!");
            }
        }
    }
}
