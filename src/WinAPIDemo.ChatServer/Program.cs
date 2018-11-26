using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WinAPIDemo.ChatServer.Common;
using static WinAPIDemo.WinAPISignatures.Ws2_32;

namespace WinAPIDemo.ChatServer
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            string ipaddr = "127.0.0.1";
            ushort port = 1044;
            ConnectionHandler connectionHandler = new ConnectionHandler();
            MessageHandler messageHandler = new MessageHandler(connectionHandler);
            connectionHandler.Connect(ipaddr, port);
            await Task.Factory.StartNew(() => connectionHandler.Listen(), TaskCreationOptions.LongRunning);
            //Task.Run(() => connectionHandler.Listen());
            await Task.Delay(-1);
        }
    }
}
