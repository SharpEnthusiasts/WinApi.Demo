using System;
using System.Runtime.InteropServices;
using System.Text;
using static WinApi.Demo.Signatures.Wininet;

namespace WinApi.Demo.Client.HTTP.Helpers
{
    public static class HttpRequestHelpers
    {
        public static string HttpSendRequestHelper(string serverName, short serverPort, string httpVerb, string endpoint, string headers, string data = "")
        {
            StringBuilder response = new StringBuilder();
            IntPtr hInternet = InternetOpen("DemoAPI", INTERNET_OPEN_TYPE_PRECONFIG,
                null, null, INTERNET_FLAG_ASYNC);
            if (IntPtr.Zero == hInternet)
            {
                response.Append("InternetOpen returned null.\n");
                response.Append($"Error code: {Marshal.GetLastWin32Error().ToString("X")}\n");
                return response.ToString();
            }
            response.Append("InternetOpen succeeded.\n");

            IntPtr hConnect = InternetConnect(hInternet, serverName, serverPort, null, null,
                INTERNET_SERVICE_HTTP, INTERNET_FLAG_SECURE, (IntPtr)1);
            if (IntPtr.Zero == hConnect)
            {
                response.Append("InternetConnect returned null.\n");
                response.Append($"Error code: {Marshal.GetLastWin32Error().ToString("X")}\n");
                return response.ToString();
            }
            response.Append("InternetConnect succeeded.\n");

            string[] lplpszAcceptedTypes = { "*/*", null };
            IntPtr hRequest = HttpOpenRequest(hConnect, httpVerb, endpoint, null, null,
                lplpszAcceptedTypes, INTERNET_FLAG_SECURE, (IntPtr)1);
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
            //The size of the additional headers, if -1 and lpszHeaders is not null, length is calculated.
            var boolApi = HttpSendRequest(hRequest, lpszHeaders, -1, dataBytes, (uint)dataBytes.Length);
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

            var res = HttpQueryInfo(hRequest, HTTP_STATUS_CODE, lBuffer, ref lBufferLen, ref lHeaderIndex);

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
            IntPtr hInternet = InternetOpen("DemoAPI", INTERNET_OPEN_TYPE_PRECONFIG, null, null, INTERNET_FLAG_ASYNC);
            IntPtr dwContext = default;
            if (IntPtr.Zero == hInternet)
            {
                response.Append("InternetOpen returned null.\n");
                response.Append($"Error code: {Marshal.GetLastWin32Error().ToString("X")}\n");
                return response.ToString();
            }
            response.Append("InternetOpen succeeded.\n");

            string lpszHeaders = "Content-Type: application/json";
            //The size of the additional headers, if -1 and lpszHeaders is not null, length is calculated.
            IntPtr hUrl = InternetOpenUrl(hInternet, url, lpszHeaders, -1, INTERNET_FLAG_HYPERLINK, dwContext);
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
            var res = HttpQueryInfo(hUrl, HTTP_STATUS_CODE, lBuffer, ref lBufferLen, ref lHeaderIndex);

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
