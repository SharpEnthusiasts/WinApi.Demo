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
        public event EventHandler<string> OutputMessage;

        public async Task Handle()
        {
            while(true)
            {
                string message;
                int iResult = 0;
                do
                {
                    byte[] buffer = new byte[512];
                    iResult = recv(_client.Socket, buffer, buffer.Length, 0);
                    if (iResult > 0)
                    {
                        OutputMessage(this, $"Bytes recieved: {iResult}");
                        message = Encoding.ASCII.GetString(buffer).TrimEnd('\0');
                        if (string.IsNullOrEmpty(message) == false)
                            MessageRecieved(this, message);
                    }
                } while (iResult > 0);
            }     
        }

        public async Task Send(string message)
        {
            byte[] msg = Encoding.ASCII.GetBytes(message);
            var iResult = send(_client.Socket, msg, msg.Length, 0);
            if (iResult == SOCKET_ERROR)
            {
                OutputMessage(this, $"Error, Sending failed. Code: {WSAGetLastError()}");
                shutdown(_client.Socket, 2);
                closesocket(_client.Socket);
                WSACleanup();
            }
            OutputMessage(this, $"Bytes sent: {iResult}");
        }
    }
}
