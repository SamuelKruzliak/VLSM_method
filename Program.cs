using System;
using System.Collections.Generic;

namespace VLSM_final_version
{
    class Program
    {
        static void Main(string[] args)
        {
           List<string> list = new List<string>();
           SubnetMaker subnetMaker = new SubnetMaker();

           subnetMaker.ParseIpAddress("172.168.0.0/16");
           subnetMaker.ParseHosts("1000,700,500,200,100,50,20,2");
           list = subnetMaker.CalculateSubnets();

           foreach(string item in list){
            Console.WriteLine(item);
           }

        }
    }
    class InputParser
    {
        static public int[] IpAddressParts;
        static public int[] Hosts;
        public void ParseIpAddress(string ipAddress){
            char[] separators = new char[] {'.', '/'};
            IpAddressParts = Array.ConvertAll(ipAddress.Split(separators), s => int.Parse(s));
        }
        public void ParseHosts(string hosts){
            Hosts = Array.ConvertAll(hosts.Split(','), s => int.Parse(s));
        }
    }
    
    class SubnetMaker:InputParser
    {
        static public int[] Prefixes = new int[Hosts.Length];
        static void getPrefixList(){
            for(int i = 0; i < Hosts.Length; i++){
                Prefixes[i] = getPrefix(Hosts[i]);
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

            for (int i = 0; i < Prefixes.Length; i++)
            {
                selected_part = calculatePart(Prefixes[i])[0];
                num = calculatePart(Prefixes[i])[1];
                currentSubnet = $"{IpAddressParts[0]}.{IpAddressParts[1]}.{IpAddressParts[2]}.{IpAddressParts[3]} /{Prefixes[i]} Subnet mask: {getSubnetMask(Prefixes[i])}";
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
        static string getSubnetMask(int prefix)
        {
            string subnet_mask = null;
            int add_value = 0;

            for (int i = 0; i < 4; i++)
            {
                if (prefix - 8 > 0)
                {
                    if (i > 0)
                    {
                        subnet_mask += ".";
                    }
                    subnet_mask += "255";
                    prefix -= 8;
                }
                else if (prefix == 0)
                {
                    subnet_mask += ".";
                    subnet_mask += "0";
                }
                else if (prefix - 8 <= 0)
                {
                    for (int j = 7; j >= 8 - prefix; j--)
                    {
                        add_value += Convert.ToInt32(Math.Pow(2, j));
                    }
                    if (i > 0)
                    {
                        subnet_mask += ".";
                    }
                    subnet_mask += add_value;
                    prefix = 0;
                }

            }
            return subnet_mask;
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
            {
                if (Math.Pow(2, i) >= hostNum + 2)
                {
                    return 32 - i;
                }
            }
        }

    }
}
