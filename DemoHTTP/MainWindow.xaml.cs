using System.Windows;
using System.Runtime.InteropServices;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using static WinAPISignatures.Wininet;
using System.Web;
using Newtonsoft.Json;
using System.Reflection;

namespace DemoHTTP
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

        #region Buttons

        private void Get_All_Click(object sender, RoutedEventArgs e)
        {
            TextBlock.Text = String.Empty;
            HttpSendRequestHelper("https://localhost:5001/api/todo");
        }

        private void Get_Id_Click(object sender, RoutedEventArgs e)
        {
            TextBlock.Text = String.Empty;
            HttpSendRequestHelper("localhost", 5001, "GET", "/api/todo/1");
        }

        private void Post_Click(object sender, RoutedEventArgs e)
        {
            TextBlock.Text = String.Empty;
            var item = new ToDoItem
            {
                Title = "Buy bread",
                IsCompleted = false
            };
            var json = JsonConvert.SerializeObject(item);
            HttpSendRequestHelper("localhost", 5001, "POST", "/api/todo", json);
        }

        private void Put_Id_Click(object sender, RoutedEventArgs e)
        {
            TextBlock.Text = String.Empty; var item = new ToDoItem
            {
                Title = "Buy bread",
                IsCompleted = false
            };
            var json = JsonConvert.SerializeObject(item);
            HttpSendRequestHelper("localhost", 5001, "PUT", "/api/todo/3", json);
        }

        private void Delete_Id_Click(object sender, RoutedEventArgs e)
        {
            TextBlock.Text = String.Empty;
            HttpSendRequestHelper("localhost", 5001, "DELETE", "/api/todo/3");
        }

        #endregion

        public void HttpSendRequestHelper(string serverName, short serverPort, string httpVerb, string endpoint, string data = "")
        {
            //INTERNET_OPEN_TYPE_PRECONFIG = 0
            IntPtr hInternet = InternetOpen("DemoAPI", 0, null, null, 0);
            if (IntPtr.Zero == hInternet)
            {
                TextBlock.Text += "InternetOpen returned null.\n";
                TextBlock.Text += ($"Error code: {0}\n", Marshal.GetLastWin32Error());
                return;
            }
            TextBlock.Text += "InternetOpen succeeded.\n";

            //INTERNET_FLAG_SECURE - 0x00800000
            IntPtr hConnect = InternetConnect(hInternet, serverName, serverPort, null, null, 3, 0x00800000, (IntPtr)1);
            if (IntPtr.Zero == hConnect)
            {
                TextBlock.Text += "InternetConnect returned null.\n";
                TextBlock.Text += ($"Error code: {0}\n", Marshal.GetLastWin32Error());
                return;
            }
            TextBlock.Text += "InternetConnect succeeded.\n";

            string[] lplpszAcceptedTypes = { "*/*", null };
            IntPtr hRequest = HttpOpenRequest(hConnect, httpVerb, endpoint, null, null, 
                lplpszAcceptedTypes, 0x00800000, (IntPtr)1);
            if (IntPtr.Zero == hRequest)
            {
                TextBlock.Text += "HttpOpenRequest returned null.\n";
                TextBlock.Text += ($"Error code: {0}\n", Marshal.GetLastWin32Error());
                return;
            }
            TextBlock.Text += "HttpOpenRequest succeeded.\n";

            string lpszHeaders = "Content-Type: application/json";
            byte[] dataBytes = Encoding.ASCII.GetBytes(data);
            var boolApi = HttpSendRequest(hRequest, lpszHeaders, 0, dataBytes, (uint)dataBytes.Length);
            if (boolApi == false)
            {
                TextBlock.Text += "HttpSendRequest returned null.\n";
                TextBlock.Text += ($"Error code: {0}\n", Marshal.GetLastWin32Error());
                return;
            }
            TextBlock.Text += "HttpSendRequest succeeded.\n";
            
            byte[] lpBuffer = new byte[1024];
            uint dwBytesRead = 0;
            string strBuffer = "";
            bool isRead;
            do
            {
                isRead = InternetReadFile(hRequest, lpBuffer, (uint)(lpBuffer.Length - 1), ref dwBytesRead);
                if (isRead == false || dwBytesRead == 0)
                    break;

                strBuffer += Encoding.ASCII.GetString(lpBuffer, 0, (int)dwBytesRead);

            } while (isRead == false || dwBytesRead == 0);

            var lBuffer = new StringBuilder();
            long lBufferLen = 4;
            long lHeaderIndex = 0;
            //dwInfoLevel = 19 / For HTTP_STATUS_CODE
            var res = HttpQueryInfo(hRequest, 19, lBuffer, ref lBufferLen, ref lHeaderIndex);

            InternetCloseHandle(hRequest);
            InternetCloseHandle(hConnect);
            InternetCloseHandle(hInternet);

            lBuffer.AppendLine();
            TextBlock.Text += "Status Code: " + lBuffer.ToString();
            TextBlock.Text += strBuffer;
        }

        public void HttpSendRequestHelper(string url)
        {
            IntPtr hInternet = InternetOpen("DemoAPI", 0, null, null, 0);
            IntPtr dwContext = default;
            if (IntPtr.Zero == hInternet)
            {
                TextBlock.Text += "InternetOpen returned null.\n";
                TextBlock.Text += ($"Error code: {0}\n", Marshal.GetLastWin32Error());
                return;
            }
            TextBlock.Text += "InternetOpen succeeded.\n";

            string lpszHeaders = "Content-Type: application/json";
            IntPtr hUrl = InternetOpenUrl(hInternet, url, lpszHeaders, 0, 0x00800000, dwContext);
            if (IntPtr.Zero == hUrl)
            {
                TextBlock.Text += "InternetOpenUrl returned null.\n";
                TextBlock.Text += ($"Error code: {0}\n", Marshal.GetLastWin32Error());
                return;
            }
            TextBlock.Text += "InternetOpenUrl succeeded.\n";

            byte[] lpBuffer = new byte[1024];
            uint dwBytesRead = 0;
            string strBuffer = "";
            bool isRead;
            do
            {
                isRead = InternetReadFile(hUrl, lpBuffer, (uint)(lpBuffer.Length - 1), ref dwBytesRead);
                if (isRead == false || dwBytesRead == 0)
                    break;

                strBuffer += Encoding.ASCII.GetString(lpBuffer, 0, (int)dwBytesRead);

            } while (isRead == false || dwBytesRead == 0);

            var lBuffer = new StringBuilder();
            long lBufferLen = 4;
            long lHeaderIndex = 0;
            //dwInfoLevel = 19 / For HTTP_STATUS_CODE
            var res = HttpQueryInfo(hUrl, 19, lBuffer, ref lBufferLen, ref lHeaderIndex);

            InternetCloseHandle(hUrl);
            InternetCloseHandle(hInternet);

            lBuffer.AppendLine();
            TextBlock.Text += "Status Code: " + lBuffer.ToString();
            TextBlock.Text += strBuffer;
        }
    }
}
