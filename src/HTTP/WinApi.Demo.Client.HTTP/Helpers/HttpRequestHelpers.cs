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
            IntPtr hInternet = InternetOpen("DemoAPI", //String that specifies the name of the application calling the WinINet functions.
                INTERNET_OPEN_TYPE_PRECONFIG, //Type of access required.
                null, //String that specifies the name of the proxy server(s). Type must be set to Proxy.
                null, //String that specifies an optional list of host names or IP addresses or both.
                0); //Options can be: INTERNET_FLAG_ASYNC, INTERNET_FLAG_FROM_CACHE or INTERNET_FLAG_OFFLINE.
            if (IntPtr.Zero == hInternet)
            {
                response.Append("InternetOpen returned null.\n");
                response.Append($"Error code: {Marshal.GetLastWin32Error().ToString("X")}\n");
                return response.ToString();
            }
            response.Append("InternetOpen succeeded.\n");

            IntPtr hConnect = InternetConnect(hInternet, //Handle returned by a previous call to InternetOpen.
                serverName, //String that specifies the host name of an Internet server.
                serverPort, //Port of the server we send request to.
                null, //String with username, used in FTP. If it's null, default is 'anonymous'.
                null, //String with username, used in FTP.
                INTERNET_SERVICE_HTTP, //Type of service to access.
                INTERNET_FLAG_SECURE, //Internet options, Secured flag is inherited in next methods.
                IntPtr.Zero); //A pointer to a variable that contains the application-defined value 
                             //that associates this operation with any application data. Used for callbacks.

            if (IntPtr.Zero == hConnect)
            {
                response.Append("InternetConnect returned null.\n");
                response.Append($"Error code: {Marshal.GetLastWin32Error().ToString("X")}\n");
                return response.ToString();
            }
            response.Append("InternetConnect succeeded.\n");

            string[] lplpszAcceptedTypes = { "*/*", null };
            IntPtr hRequest = HttpOpenRequest(hConnect, //A handle to an HTTP session returned by InternetConnect.
                httpVerb, //A pointer to a string that contains the HTTP verb.
                endpoint, //A pointer to a string containing endpoint path on the server.
                null, //A pointer to a string containing HTTP version. If null it'll be choosen automatically.
                null, //String that specifies the URL of the document from which the URL in the request.
                lplpszAcceptedTypes, //Accepted types by client, can be null or null terminated array.
                INTERNET_FLAG_SECURE, //Internet options, Secured flag is inherited in next methods.
                IntPtr.Zero); //A pointer to a variable that contains the application-defined value 
                              //that associates this operation with any application data.
            if (IntPtr.Zero == hRequest)
            {
                response.Append("HttpOpenRequest returned null.\n");
                response.Append($"Error code: {Marshal.GetLastWin32Error().ToString("X")}\n");
                return response.ToString();
            }
            response.Append("HttpOpenRequest succeeded.\n");

            //string lpszHeaders = "Content-Type: application/json";
            string lpszHeaders = headers + '\0';
            byte[] dataBytes = Encoding.ASCII.GetBytes(data);
            
            var boolApi = HttpSendRequest(hRequest, //A handle returned by a call to the HttpOpenRequest function.
                lpszHeaders, //A pointer to a null-terminated string that contains the additional headers.
                -1, //The size of the additional headers, if -1 and lpszHeaders is not null, length is calculated.
                dataBytes, //A pointer to a buffer containing any optional data.
                (uint)dataBytes.Length); //The size of the optional data, in bytes.
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
                isRead = InternetReadFile(hRequest, //Handle returned from a previous call to InternetOpenUrl, FtpOpenFile, or HttpOpenRequest.
                    lpBuffer, //Pointer to a buffer that receives the data.
                    (uint)(lpBuffer.Length - 1), //Number of bytes to be read.
                    ref dwBytesRead); //Pointer to a variable that receives the number of bytes read.

                strBuffer += Encoding.ASCII.GetString(lpBuffer, 0, (int)dwBytesRead);
            } while (isRead == true && dwBytesRead != 0);

            var lBuffer = new StringBuilder();
            long lBufferLen = 4;
            long lHeaderIndex = 0;

            var res = HttpQueryInfo(hRequest, //A handle returned by a call to the HttpOpenRequest or InternetOpenUrl function.
                HTTP_QUERY_STATUS_CODE, //Flag that modifies our query request.
                lBuffer, //Pointer to a buffer to which response is saved.
                ref lBufferLen, //Pointer to a variable holding length of the buffer.
                ref lHeaderIndex); //Pointer to a zero based index header, used for enumeration.

            InternetCloseHandle(hRequest); //Handle to be closed.
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
            IntPtr hInternet = InternetOpen("DemoAPI", //String that specifies the name of the application calling the WinINet functions.
                INTERNET_OPEN_TYPE_PRECONFIG, //Type of access required.
                null, //String that specifies the name of the proxy server(s). Type must be set to Proxy.
                null, //String that specifies an optional list of host names or IP addresses or both.
                0); //Options can be: INTERNET_FLAG_ASYNC, INTERNET_FLAG_FROM_CACHE or INTERNET_FLAG_OFFLINE.
            if (IntPtr.Zero == hInternet)
            {
                response.Append("InternetOpen returned null.\n");
                response.Append($"Error code: {Marshal.GetLastWin32Error().ToString("X")}\n");
                return response.ToString();
            }
            response.Append("InternetOpen succeeded.\n");

            string lpszHeaders = "Content-Type: application/json\0";
            
            IntPtr hUrl = InternetOpenUrl(hInternet, //The handle to the current Internet session.
                url, //String variable that specifies the URL. Only URLs beginning with ftp:, http:, or https: are supported.
                lpszHeaders, //String that specifies the headers to be sent to the HTTP server.
                -1, //The size of the additional headers, if -1 and lpszHeaders is not null, length is calculated.
                INTERNET_FLAG_HYPERLINK, //Option flag for the request.
                IntPtr.Zero); //A pointer to a variable that specifies the application-defined value that is passed, 
                              //along with the returned handle, to any callback functions.
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
                isRead = InternetReadFile(hUrl, //Handle returned from a previous call to InternetOpenUrl, FtpOpenFile, or HttpOpenRequest.
                    lpBuffer, //Pointer to a buffer that receives the data.
                    (uint)(lpBuffer.Length - 1), //Number of bytes to be read.
                    ref dwBytesRead); //Pointer to a variable that receives the number of bytes read.

                strBuffer += Encoding.ASCII.GetString(lpBuffer, 0, (int)dwBytesRead);

            } while (isRead == true && dwBytesRead != 0);

            var lBuffer = new StringBuilder();
            long lBufferLen = 4;
            long lHeaderIndex = 0;
            var res = HttpQueryInfo(hUrl, //A handle returned by a call to the HttpOpenRequest or InternetOpenUrl function.
                HTTP_QUERY_STATUS_CODE, //Flag that modifies our query request.
                lBuffer, //Pointer to a buffer to which response is saved.
                ref lBufferLen, //Pointer to a variable holding length of the buffer.
                ref lHeaderIndex); //Pointer to a zero based index header, used for enumeration.

            InternetCloseHandle(hUrl); //Handle to be closed.
            InternetCloseHandle(hInternet);

            lBuffer.AppendLine();
            response.Append("Status Code: " + lBuffer.ToString());
            response.Append("\nResponse body:\n\n");
            response.Append(strBuffer);
            return response.ToString();
        }
    }
}
