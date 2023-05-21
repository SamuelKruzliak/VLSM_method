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
       // static int[] Prefixes = new int[Hosts.Length];
        static List<int> Prefixes = new List<int>();
        static void getPrefixList()
        {
            for (int i = 0; i < Hosts.Length; i++)
            {
                Prefixes.Add(getPrefix(Hosts[i]));
            }
        }
        public List<string> CalculateSubnets()
        {
            List<string> Subnets = new List<string>();
            int add_value = 0;
            int selected_part = 4;
            int num;
            string currentSubnet;
            getPrefixList();

            for (int i = 0; i < Prefixes.Count; i++)
            {
                selected_part = calculatePart(Prefixes[i])[0];
                num = calculatePart(Prefixes[i])[1];
                currentSubnet = $"{IpAddressParts[0]}.{IpAddressParts[1]}.{IpAddressParts[2]}.{IpAddressParts[3]} /{Prefixes[i]} Subnet mask: {GetSubnetMask(Prefixes[i])}";
                Subnets.Add($"{currentSubnet}");
                add_value = Convert.ToInt32(Math.Pow(2, num));

                if (add_value + IpAddressParts[selected_part] > 255)
                {
                    IpAddressParts[selected_part - 1]++;
                    add_value = 0;
                    IpAddressParts[selected_part] = 0;
                }
                IpAddressParts[selected_part] += add_value;
            }

            return Subnets;
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
        
        static int[] calculatePart(int prefix)
        {
            int[] res = new int[2];
            if (prefix < 8)
            {
                res[0] = 0;
                res[1] = 8 - prefix;
            }
            else if (prefix < 16)
            {
                res[0] = 1;
                res[1] = 8 - (prefix - 8);
            }
            else if (prefix < 24)
            {
                res[0] = 2;
                res[1] = 8 - (prefix - 16);
            }
            else
            {
                res[0] = 3;
                res[1] = 32 - prefix;
            }
            return res;
        }
        static int getPrefix(int hostNum)
        {
            for (int i = 0; true; i++)
                if (Math.Pow(2, i) >= hostNum + 2)
                    return 32 - i;
        }
    }
}
