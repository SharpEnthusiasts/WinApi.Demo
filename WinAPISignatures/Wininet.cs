using System;
using System.Runtime.InteropServices;

namespace WinAPISignatures
{
    public static class Wininet
    {
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
            string lpszVersion, string lpszReferer, string[] lplpszAcceptTypes, int dwFlags, IntPtr dwContext);

        [DllImport("wininet.dll", EntryPoint = "HttpSendRequest", CharSet = CharSet.Auto, SetLastError = true)]
        public extern static bool HttpSendRequest(IntPtr hRequest, string lpszHeaders,
            uint dwHeadersLength, byte[] lpOptional, uint dwOptionalLength);

        [DllImport("wininet.dll", EntryPoint = "InternetReadFile", SetLastError = true)]
        public static extern bool InternetReadFile(IntPtr hFile, byte[] lpBuffer,
            uint dwNumberOfBytesToRead, ref uint lpdwNumberOfBytesRead);

        [DllImport("wininet.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool InternetCloseHandle(IntPtr hInternet);
    }
}
