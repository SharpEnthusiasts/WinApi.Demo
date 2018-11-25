using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static WinAPIDemo.WinAPISignatures.Ws2_32;

namespace WinAPIDemo.ChatServer.Common
{
    public class MessageHandler
    {
        private ConcurrentBag<IntPtr> connectedClients;
        private ConnectionHandler _connectionHandler;

        public MessageHandler(ConnectionHandler connectionHandler)
        {
            connectedClients = new ConcurrentBag<IntPtr>();
            _connectionHandler = connectionHandler;
            _connectionHandler.ClientConnected += ClientConnected;
        }

        private void ClientConnected(object sender, IntPtr e)
        {
            connectedClients.Add(e);
            Task.Run(() => Handle(e));
        }

        private void BroadcastMessage(string message)
        {
            byte[] buff = Encoding.ASCII.GetBytes(message);

            foreach (var connection in connectedClients)
            {
                var iSendResult = send(connection, buff, buff.Length, 0);
                //send(connection, new byte[]{ }, 0, 0);
                if (iSendResult == SOCKET_ERROR)
                {
                    ////ERROR
                    //Console.WriteLine($"Error: {WSAGetLastError()}");
                    //Debugger.Break();
                    //closesocket(_socket);
                    //WSACleanup();
                }
                Console.WriteLine($"Bytes sent {iSendResult}");
                //var iResult = shutdown(connection, 1);
            }
        }

        public async Task Handle(IntPtr client)
        {
            while (true)
            {
                int iResult = 0;
                byte[] buffer = new byte[512];
                do
                {
                    iResult = recv(client, buffer, buffer.Length, 0);
                    if (iResult > 0)
                    {
                        Console.WriteLine($"Bytes recieved {iResult}");

                        string msg = Encoding.ASCII.GetString(buffer).TrimEnd('\0');
                        Console.WriteLine(msg);

                        BroadcastMessage(msg);
                    }
                } while (iResult > 0);
                //else if (iResult == 0)
                //    //Console.WriteLine("Connection closing");
                //else
                //{
                //    ////ERROR
                //    //Console.WriteLine($"Error: {WSAGetLastError()}");
                //    //Debugger.Break();
                //    //closesocket(_socket);
                //    //WSACleanup();
                //}
            }
        }
    }
}
