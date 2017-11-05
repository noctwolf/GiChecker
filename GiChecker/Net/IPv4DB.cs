using System;
using System.IO;
using System.Text;

namespace GiChecker.Net
{
    static class IPv4DB
    {
        //private static void Main(string[] args)
        //{
        //    GiChecker.Net.IP.EnableFileWatch = true;
        //    GiChecker.Net.IP.Load("17monipdb.dat");

        //    Console.WriteLine(string.Join("\n", GiChecker.Net.IP.Find("8.8.8.8")));
        //    Console.WriteLine(string.Join("\n", GiChecker.Net.IP.Find("255.255.255.255")));
        //    Console.ReadKey(true);
        //}

        static int offset;
        static readonly uint[] index = new uint[256];
        static byte[] dataBuffer;
        static byte[] indexBuffer;

        public static string[] Find(string ip)
        {
            var ips = ip.Split('.');
            var ip_prefix_value = int.Parse(ips[0]);
            long ip2long_value = BytesToLong(byte.Parse(ips[0]), byte.Parse(ips[1]), byte.Parse(ips[2]),
                byte.Parse(ips[3]));
            var start = index[ip_prefix_value];
            var max_comp_len = offset - 1028;
            long index_offset = -1;
            var index_length = -1;
            for (start = start * 8 + 1024; start < max_comp_len; start += 8)
            {
                if (
                    BytesToLong(indexBuffer[start + 0], indexBuffer[start + 1], indexBuffer[start + 2],
                        indexBuffer[start + 3]) >= ip2long_value)
                {
                    index_offset = BytesToLong(0, indexBuffer[start + 6], indexBuffer[start + 5],
                        indexBuffer[start + 4]);
                    index_length = 0xFF & indexBuffer[start + 7];
                    break;
                }
            }
            var areaBytes = new byte[index_length];
            Array.Copy(dataBuffer, offset + (int)index_offset - 1024, areaBytes, 0, index_length);
            return Encoding.UTF8.GetString(areaBytes).Split('\t');
        }

        public static void Load(string filename)
        {
            var file = new FileInfo(filename);

            dataBuffer = new byte[file.Length];
            using (var fin = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                fin.Read(dataBuffer, 0, dataBuffer.Length);
            }

            var indexLength = BytesToLong(dataBuffer[0], dataBuffer[1], dataBuffer[2], dataBuffer[3]);
            indexBuffer = new byte[indexLength];
            Array.Copy(dataBuffer, 4, indexBuffer, 0, indexLength);
            offset = (int)indexLength;

            for (var loop = 0; loop < 256; loop++)
            {
                index[loop] = BytesToLong(indexBuffer[loop * 4 + 3], indexBuffer[loop * 4 + 2],
                    indexBuffer[loop * 4 + 1],
                    indexBuffer[loop * 4]);
            }
        }

        private static uint BytesToLong(byte a, byte b, byte c, byte d)
        {
            return ((uint)a << 24) | ((uint)b << 16) | ((uint)c << 8) | d;
        }
    }
}
