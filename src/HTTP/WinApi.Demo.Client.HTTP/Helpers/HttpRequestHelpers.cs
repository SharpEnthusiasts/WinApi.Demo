using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using static WinApi.Demo.Signatures.Wininet;

namespace WinApi.Demo.Client.HTTP.Helpers
{
    public static class HttpRequestHelpers
    {
        public static string HttpSendRequestHelper(string serverName, short serverPort, string httpVerb, string endpoint, string headers, string data = "")
        {
            StringBuilder response = new StringBuilder();
            //INTERNET_OPEN_TYPE_PRECONFIG = 0
            IntPtr hInternet = InternetOpen("DemoAPI", 0, null, null, 0);
            if (IntPtr.Zero == hInternet)
            {
                response.Append("InternetOpen returned null.\n");
                response.Append($"Error code: {Marshal.GetLastWin32Error().ToString("X")}\n");
                return response.ToString();
            }
            response.Append("InternetOpen succeeded.\n");

            //INTERNET_FLAG_SECURE - 0x00800000
            //INTERNET_FLAG_EXISTING_CONNECT - 0x20000000
            //INTERNET_FLAG_PASSIVE - 0x08000000
            IntPtr hConnect = InternetConnect(hInternet, serverName, serverPort, null, null, 3, 0x00800000, (IntPtr)1);
            if (IntPtr.Zero == hConnect)
            {
                response.Append("InternetConnect returned null.\n");
                response.Append($"Error code: {Marshal.GetLastWin32Error().ToString("X")}\n");
                return response.ToString();
            }
            response.Append("InternetConnect succeeded.\n");

            //INTERNET_FLAG_SECURE - 0x00800000
            //INTERNET_FLAG_RELOAD - 0x80000000
            string[] lplpszAcceptedTypes = { "*/*", null };
            IntPtr hRequest = HttpOpenRequest(hConnect, httpVerb, endpoint, null, null,
                lplpszAcceptedTypes, 0x00800000, (IntPtr)1);
            if (IntPtr.Zero == hRequest)
            {
                response.Append("HttpOpenRequest returned null.\n");
                response.Append($"Error code: {Marshal.GetLastWin32Error().ToString("X")}\n");
                return response.ToString();
            }
            response.Append("HttpOpenRequest succeeded.\n");

            //string lpszHeaders = "Content-Type: application/json";
            string lpszHeaders = headers;
            byte[] dataBytes = Encoding.ASCII.GetBytes(data);
            var boolApi = HttpSendRequest(hRequest, lpszHeaders, 0, dataBytes, (uint)dataBytes.Length);
            if (boolApi == false)
            {
                response.Append("HttpSendRequest returned null.\n");
                response.Append($"Error code: {Marshal.GetLastWin32Error().ToString("X")}\n");
                return response.ToString();
            }
            response.Append("HttpSendRequest succeeded.\n");

            byte[] lpBuffer = new byte[1024];
            uint dwBytesRead = 0;
            string strBuffer = "";
            bool isRead;
            do
            {
                isRead = InternetReadFile(hRequest, lpBuffer, (uint)(lpBuffer.Length - 1), ref dwBytesRead);
                strBuffer += Encoding.ASCII.GetString(lpBuffer, 0, (int)dwBytesRead);
            } while (isRead == true && dwBytesRead != 0);

            var lBuffer = new StringBuilder();
            long lBufferLen = 4;
            long lHeaderIndex = 0;
            //dwInfoLevel = 19 / For HTTP_STATUS_CODE
            var res = HttpQueryInfo(hRequest, 19, lBuffer, ref lBufferLen, ref lHeaderIndex);

            InternetCloseHandle(hRequest);
            InternetCloseHandle(hConnect);
            InternetCloseHandle(hInternet);

            lBuffer.AppendLine();
            response.Append("Status Code: " + lBuffer.ToString());
            response.Append("\nResponse body:\n\n");
            response.Append(strBuffer);
            return response.ToString();
        }

        public static string HttpSendRequestHelper(string url)
        {
            StringBuilder response = new StringBuilder();
            IntPtr hInternet = InternetOpen("DemoAPI", 0, null, null, 0);
            IntPtr dwContext = default;
            if (IntPtr.Zero == hInternet)
            {
                response.Append("InternetOpen returned null.\n");
                response.Append($"Error code: {Marshal.GetLastWin32Error().ToString("X")}\n");
                return response.ToString();
            }
            response.Append("InternetOpen succeeded.\n");

            string lpszHeaders = "Content-Type: application/json";
            IntPtr hUrl = InternetOpenUrl(hInternet, url, lpszHeaders, 0, 0x00000400, dwContext);
            if (IntPtr.Zero == hUrl)
            {
                response.Append("InternetOpenUrl returned null.\n");
                response.Append($"Error code: {Marshal.GetLastWin32Error().ToString("X")}\n");
                return response.ToString();
            }
            response.Append("InternetOpenUrl succeeded.\n");

            byte[] lpBuffer = new byte[1024];
            uint dwBytesRead = 0;
            string strBuffer = "";
            bool isRead;
            do
            {
                isRead = InternetReadFile(hUrl, lpBuffer, (uint)(lpBuffer.Length - 1), ref dwBytesRead);
                strBuffer += Encoding.ASCII.GetString(lpBuffer, 0, (int)dwBytesRead);

            } while (isRead == true && dwBytesRead != 0);

            var lBuffer = new StringBuilder();
            long lBufferLen = 4;
            long lHeaderIndex = 0;
            //dwInfoLevel = 19 / For HTTP_STATUS_CODE
            var res = HttpQueryInfo(hUrl, 19, lBuffer, ref lBufferLen, ref lHeaderIndex);

            InternetCloseHandle(hUrl);
            InternetCloseHandle(hInternet);

            lBuffer.AppendLine();
            response.Append("Status Code: " + lBuffer.ToString());
            response.Append("\nResponse body:\n\n");
            response.Append(strBuffer);
            return response.ToString();
        }
    }
}
