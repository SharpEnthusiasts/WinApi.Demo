using System;
using System.Text;
using System.Threading.Tasks;
using static WinApi.Demo.Signatures.Ws2_32;

namespace WinApi.Demo.Chat.Client.Common
{
    public class MessageHandler
    {
        private Model.Client _client;

        public MessageHandler(Model.Client client)
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
                    iResult = recv(_client.Socket, //Socket connection we want to recieve data from.
                        buffer, //Buffer we load data into.
                        buffer.Length, //Length of the buffer.
                        0); //Flag, MSG_PEEK, MSG_OOB or MSG_WAITALL.
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

        public void Send(string message)
        {
            byte[] msg = Encoding.ASCII.GetBytes(message);
            var iResult = send(_client.Socket, //We pick a connection we want to send data to.
                msg, //Buffer, our data payload in bytes.
                msg.Length, //Length of our buffer.
                0); //Flags, either MSG_DONTROUTE or MSG_OOB.
            if (iResult == SOCKET_ERROR)
            {
                OutputMessage(this, $"Error, Sending failed. Code: {WSAGetLastError()}");
                shutdown(_client.Socket, 2); //We shutdown the socket, both ways send/recv.
                closesocket(_client.Socket); //We close the socket and free it.
                WSACleanup(); //Terminates winsockets on all threads.
            }
            OutputMessage(this, $"Bytes sent: {iResult}");
        }
    }
}
