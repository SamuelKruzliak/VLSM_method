using System;
using System.Collections.Generic;

namespace VLSM_final_version
{
    class Program:IpAddress
    {
        static void Main(string[] args)
        {
           UserInput[0] = "172.168.0.0/16"; //User input[0] is for Ip address
           UserInput[1] = "1000,700,500,200,100,50,20,2"; //User input[1] is for hosts
           InputParser.parseInput(); //Parses both inputs into parts
           SubnetMaker.getPrefixList();
           SubnetMaker.printAdresses();
           foreach(string item in Subnets){
            System.Console.WriteLine(item);
           }
        }
    }
    class IpAddress
    {
        static public string[] UserInput = new string[2];
        static public int[] IpAddressParts;
        static public int[] Hosts;
        
        static public List<string> Subnets = new List<string>();
    }
    class InputParser:IpAddress
    {
        static public void parseInput(){
            char[] separators = new char[] {'.', '/'};
            IpAddressParts = Array.ConvertAll(UserInput[0].Split(separators), s => int.Parse(s));
            Hosts = Array.ConvertAll(UserInput[1].Split(','), s => int.Parse(s));
        }
    }
    class SubnetMaker:IpAddress
    {
        static public int[] Prefixes = new int[Hosts.Length];
        static public void getPrefixList(){
            for(int i = 0; i < Hosts.Length; i++){
                Prefixes[i] = getPrefix(Hosts[i]);
            }
        }
        static public void printAdresses()
        {
            int add_value = 0;
            int selected_part = 4;
            int num;

            for (int i = 0; i < Prefixes.Length; i++)
            {
                selected_part = calculatePart(Prefixes[i])[0];
                num = calculatePart(Prefixes[i])[1];
                Subnets.Add($"{IpAddressParts[0]}.{IpAddressParts[1]}.{IpAddressParts[2]}.{IpAddressParts[3]} /{Prefixes[i]} Subnet mask: {getSubnetMask(Prefixes[i])}");
                add_value = Convert.ToInt32(Math.Pow(2, num));

                if (add_value + IpAddressParts[selected_part] > 255)
                {
                    IpAddressParts[selected_part - 1]++;
                    add_value = 0;
                    IpAddressParts[selected_part] = 0;
                }
                IpAddressParts[selected_part] += add_value;

            }
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
