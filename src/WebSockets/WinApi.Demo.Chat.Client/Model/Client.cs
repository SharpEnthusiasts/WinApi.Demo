using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
