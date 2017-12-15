using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace GiChecker.Net
{
    public static class IPAddressExtension
    {
        public static uint ToUInt32(this IPAddress value)
        {
            if (value.AddressFamily != AddressFamily.InterNetwork)
                throw new SocketException((int)SocketError.OperationNotSupported);
            return BitConverter.ToUInt32(value.GetAddressBytes().Reverse().ToArray(), 0);
        }

        public static IPAddress ToIPAddress(this uint value)
        {
            return new IPAddress(BitConverter.GetBytes(value).Reverse().ToArray());
        }

        public static IPAddress ToIPAddress(this int value)
        {
            return ((uint)value).ToIPAddress();
        }
    }
}
