using System.Threading.Tasks;
using WinApi.Demo.Chat.Server.Common;

namespace WinApi.Demo.Chat.Server
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
