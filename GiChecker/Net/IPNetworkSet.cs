using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using Raize.CodeSiteLogging;

namespace GiChecker.Net
{
    public class IPNetworkSet : SortedSet<IPNetwork>
    {
        const string CIDRReserved = @"0.0.0.0/8 本网络（仅作为源地址时合法）
10.0.0.0/8 专用网络
100.64.0.0/10 地址共享
127.0.0.0/8 环回
169.254.0.0/16 链路本地
172.16.0.0/12 专用网络
192.0.0.0/24 用于IANA的IPv4特殊用途地址表
192.0.2.0/24 TEST-NET-1
192.88.99.0/24 用于6to4任播中继
192.168.0.0/16 专用网络
198.18.0.0/15 用于测试两个不同的子网的网间通信
198.51.100.0/24 TEST-NET-2
203.0.113.0/24 TEST-NET-3
224.0.0.0/3 D类网络和E类网络";
        public static readonly IPNetworkSet IPv4Reserved = new IPNetworkSet();

        static IPNetworkSet()
        {
            IPv4Reserved.Add(CIDRReserved);
        }

        public IPNetworkSet()
        {

        }

        public IPNetworkSet(IComparer<IPNetwork> comparer)
            : base(comparer)
        {

        }

        public IPNetworkSet(IEnumerable<IPNetwork> collection)
            : base(collection)
        {

        }

        public IPNetworkSet(IEnumerable<IPNetwork> collection, IComparer<IPNetwork> comparer)
            : base(collection, comparer)
        {

        }

        protected IPNetworkSet(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        public void Add(string networks)
        {
            var ips = networks.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).AsEnumerable();
            foreach (var ip in ips)
            {
                try
                {
                    IPNetwork network = IPNetwork.Parse(ip);
                    if (ip.StartsWith(network.ToString()))
                        Add(network);
                    else
                        CodeSite.Send(ip, network);
                }
                catch (Exception ex)
                {
                    CodeSite.SendException(ip, ex);
                }
            }

        }

        public bool Contains(IPAddress ipaddress)
        {
            return this.Any(p => IPNetwork.Contains(p, ipaddress));
        }

        public bool Contains(uint ipaddress)
        {
            return Contains(ipaddress.ToIPAddress());
        }
    }
}
