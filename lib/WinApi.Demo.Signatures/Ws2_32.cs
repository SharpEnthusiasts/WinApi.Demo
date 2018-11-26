using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WinApi.Demo.Signatures
{
    public class Ws2_32
    {
        public const byte AF_INET = 2;
        public const byte SOCK_STREAM = 1;
        public const byte IPPROTO_TCP = 6;
        public const uint INVALID_SOCKET = unchecked((uint)(~0));
        public const sbyte SOCKET_ERROR = -1;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct WSAData
        {
            public Int16 version;
            public Int16 highVersion;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 257)]
            public String description;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 129)]
            public String systemStatus;

            public Int16 maxSockets;
            public Int16 maxUdpDg;
            public IntPtr vendorInfo;
        }

        [StructLayout(LayoutKind.Explicit, Size = 16)]
        public struct sockaddr
        {
            [FieldOffset(0)] public ushort sa_family;
            //Should be size of 14 bytes.
            [FieldOffset(2)] public byte[] sa_data;
        };

        [StructLayout(LayoutKind.Explicit, Size = 4)]
        public struct in_addr
        {
            [FieldOffset(0)] public byte s_b1;
            [FieldOffset(1)] public byte s_b2;
            [FieldOffset(2)] public byte s_b3;
            [FieldOffset(3)] public byte s_b4;

            [FieldOffset(0)] public ushort s_w1;
            [FieldOffset(2)] public ushort s_w2;

            [FieldOffset(0)] public uint s_addr;
        }

        [StructLayout(LayoutKind.Explicit, Size = 16)]
        public struct sockaddr_in
        {
            [FieldOffset(0)] public short sin_family;
            [FieldOffset(2)] public ushort sin_port;
            [FieldOffset(4)] public in_addr sin_addr;
            //sin_zero should be size of 8
            [FieldOffset(8)] public byte[] sin_zero;
        };

        [DllImport("ws2_32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Int32 WSAStartup(Int16 wVersionRequested, out WSAData wsaData);

        [DllImport("ws2_32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr socket(int af, int type, int protocol);

        [DllImport("ws2_32.dll", EntryPoint = "socket", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint socket2(int af, int type, int protocol);

        [DllImport("ws2_32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Int32 WSACleanup();

        [DllImport("Ws2_32.dll")]
        public static extern int bind(IntPtr s, ref sockaddr_in addr, int addrsize);

        [DllImport("ws2_32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int closesocket(IntPtr s);

        [DllImport("Ws2_32.dll")]
        public static extern int connect(IntPtr s, ref sockaddr_in addr, int addrsize);

        [DllImport("ws2_32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr WSASocket(short af, short socket_type, short protocol,
            IntPtr lpProtocolInfo, Int32 group, short dwFlags);

        [DllImport("ws2_32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 WSAGetLastError();

        [DllImport("Ws2_32.dll")]
        public static extern int listen(IntPtr s, int backlog);

        [DllImport("Ws2_32.dll")]
        public static extern IntPtr accept(IntPtr s, IntPtr addr, int addrsize);

        [DllImport("ws2_32.dll")]
        public static extern int send(IntPtr s, byte[] buf, int len, int flags);

        [DllImport("ws2_32.dll")]
        public static extern int recv(IntPtr s, byte[] buf, int len, int flags);

        [DllImport("ws2_32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int shutdown(IntPtr s, int how);
    }
}