using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WinApi.Demo.Signatures
{
    public static class Wininet
    {
        public const int INTERNET_FLAG_SECURE = 0x00800000;
        public const int INTERNET_FLAG_EXISTING_CONNECT = 0x20000000;
        public const int INTERNET_FLAG_PASSIVE = 0x08000000;
        public const uint INTERNET_FLAG_RELOAD = 0x80000000;
        public const byte INTERNET_OPEN_TYPE_PRECONFIG = 0;
        public const byte HTTP_STATUS_CODE = 19;
        public const short INTERNET_FLAG_HYPERLINK = 0x00000400;
        public const int INTERNET_FLAG_ASYNC = 0x10000000;
        public const byte INTERNET_SERVICE_HTTP = 3;

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr InternetOpen(string lpszAgent, int dwAccessType, string lpszProxyName,
            string lpszProxyBypass, int dwFlags);

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr InternetOpenUrl(IntPtr hInternet, string lpszUrl, string lpszHeaders,
            int dwHeadersLength, int dwFlags, IntPtr dwContext);

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr InternetConnect(IntPtr hInternet, string lpszServerName, short nServerPort,
            string lpszUsername, string lpszPassword, int dwService, int dwFlags, IntPtr dwContext);

        [DllImport("wininet.dll", SetLastError = true)]
        public static extern IntPtr HttpOpenRequest(IntPtr hConnect, string lpszVerb, string lpszObjectName,
            string lpszVersion, string lpszReferer, string[] lplpszAcceptTypes, uint dwFlags, IntPtr dwContext);

        [DllImport("wininet.dll", EntryPoint = "HttpSendRequest", CharSet = CharSet.Auto, SetLastError = true)]
        public extern static bool HttpSendRequest(IntPtr hRequest, string lpszHeaders,
            int dwHeadersLength, byte[] lpOptional, uint dwOptionalLength);

        [DllImport("wininet.dll", EntryPoint = "InternetReadFile", SetLastError = true)]
        public static extern bool InternetReadFile(IntPtr hFile, byte[] lpBuffer,
            uint dwNumberOfBytesToRead, ref uint lpdwNumberOfBytesRead);

        [DllImport("wininet.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool InternetCloseHandle(IntPtr hInternet);

        [DllImport("wininet.dll", EntryPoint = "HttpQueryInfo", SetLastError = true)]
        public static extern bool HttpQueryInfo(IntPtr hInternet, int dwInfoLevel, StringBuilder buffer,
            ref long lpdwBufferLength, ref long lpdwIndex);

    }
}
