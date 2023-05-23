using System;
using System.Collections.Generic;

namespace ConsoleApp100
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> list = new List<string>();
            SubnetMaker subnetMaker = new SubnetMaker();

            Console.Write("Zadaj Ip adresu s prefixom: "); 
            subnetMaker.ParseIpAddress(Console.ReadLine());
            
            Console.Write("Zadaj pocty hostov: ");
            subnetMaker.ParseHosts(Console.ReadLine());

            list = subnetMaker.CalculateSubnets();
            
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Subnety: ");

            foreach (string item in list)
            {
                Console.WriteLine(item);
            }
            Console.ReadLine();
            
        }
    }
    class InputParser
    {
        public static int[] IpAddressParts;
        public static int[] Hosts;
        public void ParseIpAddress(string ipAddress)
        {
            char[] separators = new char[] { '.', '/' };
            IpAddressParts = Array.ConvertAll(ipAddress.Split(separators), s => int.Parse(s));
        }
        public void ParseHosts(string hosts)
        {
            Hosts = Array.ConvertAll(hosts.Split(','), s => int.Parse(s));
        }
    }
    class SubnetMaker : InputParser
    {
        static List<int> Prefixes = new List<int>();
        static void getPrefixList()
        {
            for(int i = 0; i < Hosts.Length; i++)
                Prefixes.Add(getPrefix(Hosts[i]));
        }
        public List<string> CalculateSubnets()
        {
            List<string> Subnets = new List<string>();
            string currentSubnet;
            getPrefixList();
            int add_value;
            int[] nn = IpAddressParts;
            foreach(int prefix in Prefixes)
            {
                add_value = Convert.ToInt32(Math.Pow(2, 8*(prefix/8+1)-prefix));
                currentSubnet = $"{IpAddressParts[0]}.{IpAddressParts[1]}.{IpAddressParts[2]}.{IpAddressParts[3]} /{prefix} Subnet mask: {GetSubnetMask(prefix)} ";
                currentSubnet += GetIpInfo(IpAddressParts, prefix, prefix/8);
                
                Subnets.Add(currentSubnet);
                if (add_value + IpAddressParts[prefix/8] > 255)
                {
                    IpAddressParts[prefix/8 - 1]++;
                    IpAddressParts[prefix/8] = 0;
                }
                else
                    IpAddressParts[prefix/8] += add_value;
            }

            return Subnets;
        }
        private string GetIpInfo(int[] x, int prefix, int part){
        string firstHost;
        string lastHost;
        string broadcast;
        byte[] IpParts = Array.ConvertAll(x, s=>Convert.ToByte(s));
        if(part != 3)
            IpParts[part+1] += 1;
        else
            IpParts[part] += 1;
        firstHost = $"{IpParts[0]}.{IpParts[1]}.{IpParts[2]}.{IpParts[3]}";

        if(part != 3)
            IpParts[part+1] -= 1;
        else
            IpParts[part] -= 1;
        

        for(int i = 8*(prefix/8+1)-prefix-1; i >=0; i--)
            if(IpParts[part] != 255)
                IpParts[part] += Convert.ToByte(Math.Pow(2,i));

        for(int i = part+1; i<=3; i++)
            IpParts[i] = 255;

        broadcast = $"{IpParts[0]}.{IpParts[1]}.{IpParts[2]}.{IpParts[3]}";
        IpParts[3] -= 1;
        lastHost = $"{IpParts[0]}.{IpParts[1]}.{IpParts[2]}.{IpParts[3]}";

        return $"First host: {firstHost} Last host: {lastHost} Broadcast: {broadcast}";

        }
        static string GetSubnetMask(int prefix){
            byte[] parts = new byte[4];
            for(int j = 0; j < 4; j++){
                for(int i = 0; i < prefix & i < 8; i++)
                    parts[j] += Convert.ToByte(Math.Pow(2,7-i));
                prefix -= 8;
            }

            return $"{parts[0]}.{parts[1]}.{parts[2]}.{parts[3]}";
        }
        static int getPrefix(int hostNum)
        {
            for (int i = 0; true; i++)
                if (Math.Pow(2, i) >= hostNum + 2)
                    return 32 - i;
        }
    }
}
