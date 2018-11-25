using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAPIDemo.ChatClient.Model;
using static WinAPIDemo.WinAPISignatures.Ws2_32;

namespace WinAPIDemo.ChatClient.Common
{
    public class MessageHandler
    {
        private Client _client;

        public MessageHandler(Client client)
        {
            _client = client;
        }

        public event EventHandler<string> MessageRecieved;

        public async Task Handle()
        {
            while(true)
            {
                string message = "";
                byte[] buffer = new byte[512];
                int iResult = 0;
                do
                {
                    iResult = recv(_client.Socket, buffer, buffer.Length, 0);
                    if (iResult > 0)
                    {
                        //TextBlock.Text += $"Bytes recieved: {iResult}\n";
                        message = Encoding.ASCII.GetString(buffer).TrimEnd('\0');
                        if (string.IsNullOrEmpty(message) == false)
                            MessageRecieved(this, message);
                    }
                    //if (string.IsNullOrEmpty(message) == false)
                    //    MessageRecieved(this, message);
                    //else if (iResult == 0)
                    //    TextBlock.Text += "Connection closed\n";
                    //else
                    //    TextBlock.Text += $"recv failed: {WSAGetLastError()}\n";
                } while (iResult > 0);
                //if (string.IsNullOrEmpty(message) == false)
                //    MessageRecieved(this, message);
                //TextBlock.Text += message + "\n";
            }     
        }

        public async Task Send(string message)
        {
            byte[] msg = Encoding.ASCII.GetBytes(message);
            var iResult = send(_client.Socket, msg, msg.Length, 0);
            if (iResult == SOCKET_ERROR)
            {
                //ERROR
                //var err = WSAGetLastError();
                //Debugger.Break();
                //closesocket(_socket);
                //WSACleanup();
            }
            //TextBlock.Text += $"Bytes sent: {iResult}\n";
            var sent = iResult.ToString();

            //iResult = shutdown(_client.Socket, 1);
            if (iResult == SOCKET_ERROR)
            {
                ////ERROR
                //var err = WSAGetLastError();
                //Debugger.Break();
                //closesocket(_socket);
                //WSACleanup();
            }
        }
    }
}
