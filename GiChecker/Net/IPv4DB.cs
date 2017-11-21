using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Net;

namespace GiChecker.Net
{
    public static class IPv4DB
    {
        //private static void Main(string[] args)
        //{
        //    Console.WriteLine(string.Join("\n", Find("8.8.8.8")));
        //    Console.WriteLine(string.Join("\n", Find("255.255.255.255")));
        //    Console.ReadKey(true);
        //}

        class IPRang : IComparable<IPRang>
        {
            public uint IP;
            public int Offset;

            public IPRang(uint ip, int offset = -1)
            {
                IP = ip;
                Offset = offset;
            }

            public int CompareTo(IPRang other)
            {
                return IP.CompareTo(other.IP);
            }

            public override string ToString()
            {
                return string.Format("{0}-{1}", IP.ToIPAddress(), Offset);
            }
        }

        static readonly List<IPRang> listIP = new List<IPRang>();
        static readonly SortedDictionary<int, string> dictArea = new SortedDictionary<int, string>();

        public static string[] Find(string ip)
        {
            return Find(IPAddress.Parse(ip));
        }

        public static string[] Find(IPAddress ip)
        {
            return Find(BitConverter.ToUInt32(ip.GetAddressBytes().Reverse().ToArray(), 0));
        }

        public static string[] Find(uint ip)
        {
            if (listIP.Count == 0) return new string[] { "没有加载数据文件" };
            int i = listIP.BinarySearch(new IPRang(ip));
            if (i < 0) i = ~i;
            return dictArea[listIP[i].Offset].Split('\t');
        }

        static IPv4DB()
        {
            const string fn = "17monipdb.dat";
            if (File.Exists(fn)) Load(fn);
        }

        public static void Load(string filename)
        {
            byte[] data = File.ReadAllBytes(filename);
            byte[] idata = new byte[4];
            Func<int, bool, int> ToInt32 = (start, netorder) =>
                {
                    Array.Copy(data, start, idata, 0, 4);
                    if (netorder) idata = idata.Reverse().ToArray();
                    return BitConverter.ToInt32(idata, 0);
                };

            int indexLength = ToInt32(0, true);
            List<IPRang> list = new List<IPRang>();
            for (int i = 1028; i < indexLength - 1024; i += 8)
            {
                uint uip = (uint)ToInt32(i, true);
                int offset = ToInt32(i + 4, false);
                int len = offset >> 24;
                offset &= 0xFFFFFF;
                list.Add(new IPRang(uip, offset));
                if (!dictArea.ContainsKey(offset))
                    dictArea[offset] = Encoding.UTF8.GetString(data, indexLength - 1024 + offset, len);
            }
            //压缩
            listIP.Clear();
            for (int i = 0; i < list.Count - 2; i++) if (list[i].Offset != list[i + 1].Offset) listIP.Add(list[i]);
            listIP.Add(list.Last());
        }
    }
}