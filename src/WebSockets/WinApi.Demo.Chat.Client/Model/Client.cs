using System;

namespace WinApi.Demo.Chat.Client.Model
{
    public class Client
    {
        public IntPtr Socket { get; private set; }

        public Client(IntPtr socket)
        {
            Socket = socket;
        }
    }
}
