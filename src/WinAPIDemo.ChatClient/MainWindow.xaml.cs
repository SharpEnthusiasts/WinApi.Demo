using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static WinAPIDemo.WinAPISignatures.Ws2_32;

namespace WinAPIDemo.ChatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            TextBlock.Text += TextBox.Text + "\n";
            TextBox.Text = string.Empty;
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            WSAData wsaData;
            //RESULT OF MAKEWORD(2,2) MACRO -/ (512 + 2) -> 514
            //We indicate that we want to use WinSocket in version 2.2
            Int16 wVersionRequested = 0b0000_0010_0000_0010;
            int result = WSAStartup(wVersionRequested, out wsaData);
            if (result != 0)
            {
                TextBlock.Text += "Initialization Error!\n";
                return;
            }
            TextBlock.Text += "Initialization Succedeed!\n";

            IntPtr _socket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
            if ((uint)_socket == INVALID_SOCKET)
            {
                TextBlock.Text += "INVALID_SOCKET\n";
                WSACleanup();
                return;
            }
            TextBlock.Text += "Socket initialized.\n";

            //uint _socket = socket2(AF_INET, SOCK_STREAM, IPPROTO_TCP);
            //IntPtr _socket = WSASocket(AF_INET, SOCK_STREAM, IPPROTO_TCP, IntPtr.Zero, 0x01, 0x01);
            //if ((uint)_socket == INVALID_SOCKET)
            //{
            //    TextBlock.Text += "INVALID_SOCKET\n";
            //    WSACleanup();
            //    return;
            //}
            //TextBlock.Text += "Socket initialized.\n";

            sockaddr_in service = new sockaddr_in();
            service.sin_addr = new in_addr();
            service.sin_family = AF_INET;
            service.sin_addr.s_addr = (uint)IPAddress.Parse("127.0.0.1").Address;
            service.sin_port = 1044;

            var iResult = connect(_socket, ref service, Marshal.SizeOf(service));

            if (iResult == SOCKET_ERROR)
            {
                TextBlock.Text += "FAILED TO CONNECT!\n";
                var err = WSAGetLastError();
                Debugger.Break();
                WSACleanup();
                return;
            }

            byte[] msg = Encoding.ASCII.GetBytes("This is test message!");
            iResult = send(_socket, msg, msg.Length, 0);
            if (iResult == SOCKET_ERROR)
            {
                //ERROR
                var err = WSAGetLastError();
                Debugger.Break();
                closesocket(_socket);
                WSACleanup();
            }
            TextBlock.Text += $"Bytes sent: {iResult}\n";
            var sent = iResult.ToString();

            iResult = shutdown(_socket, 1);
            if (iResult == SOCKET_ERROR)
            {
                //ERROR
                var err = WSAGetLastError();
                Debugger.Break();
                closesocket(_socket);
                WSACleanup();
            }

            string message = "";
            byte[] buffer = new byte[1024];
            do
            {   
                iResult = recv(_socket, buffer, buffer.Length, 0);
                if (iResult > 0)
                {
                    TextBlock.Text += $"Bytes recieved: {iResult}\n";
                    message += Encoding.ASCII.GetString(buffer).TrimEnd('\0');
                }
                else if (iResult == 0)
                    TextBlock.Text += "Connection closed\n";
                else
                    TextBlock.Text += $"recv failed: {WSAGetLastError()}\n";
            } while (iResult > 0);

            TextBlock.Text += message + "\n";
        }
    }
}
