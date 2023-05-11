using System;
using System.Collections.Generic;

namespace networking
{
    class Program
    {
        static void Main(string[] args)
        {
            List<byte> numbers = new List<byte>();
            List<int> hosts = new List<int>();
            List<string> results = new List<string>();

            Console.Write("Zadaj Ip adresu s prefixom: ");
            string ipAdress = Console.ReadLine();
            Console.Write("Zadaj pocet hostov: ");
            string host_string = Console.ReadLine();
            try
            {
                hosts = NumberParser.parseHosts(host_string);
                numbers = NumberParser.ParseIpAdress(ipAdress);
                hosts = IpAdressCalc.getPrefixList(hosts);
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine("Subnety: ");

                results = IpAdressCalc.printAdresses(hosts, numbers);
            }
            catch
            {
                Console.WriteLine("Zadaj platne hodnoty!");
            }

            foreach (string item in results)
            {
                Console.WriteLine(item);
            }
            Console.ReadLine();
        }
    }


    class NumberParser
    {
        static public List<byte> ParseIpAdress(string ipAdress)
        {
            List<byte> list = new List<byte>();
            const int NumberOfIpAdressParts = 4;

            list.Add(Convert.ToByte(ipAdress.Substring(ipAdress.IndexOf("/") + 1)));
            ipAdress = ipAdress.Substring(0, ipAdress.IndexOf("/"));

            for (int i = 0; i < NumberOfIpAdressParts; i++)
            {
                if (ipAdress.IndexOf(".") == -1)
                {
                    list.Add(Convert.ToByte(ipAdress));
                }
                else if (ipAdress.IndexOf(".") != -1)
                {
                    list.Add(Convert.ToByte(ipAdress.Substring(0, ipAdress.IndexOf("."))));
                    ipAdress = ipAdress.Substring(ipAdress.IndexOf(".") + 1, ipAdress.Length - ipAdress.IndexOf(".") - 1);
                }
            }

            return list;
        }

        static public List<int> parseHosts(string s)
        {
            List<int> hosts = new List<int>();


            for (int i = 0; true; i++)
            {
                if (s[i] == ',')
                {
                    hosts.Add(int.Parse(s.Substring(0, i)));
                    s = s.Substring(i + 1, s.Length - i - 1);
                    i = 0;
                }
                if (s.IndexOf(",") == -1)
                {
                    hosts.Add(int.Parse(s));
                    return hosts;
                }
            }

        }

        static List<int> arrangeList(List<int> list)
        {
            List<int> newList = new List<int>();


            for (int i = 0; i < list.Count; i++)
            {
                int val = 0;
                int pos = 0;

                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j] > val)
                    {
                        val = list[j];
                        pos = j;
                    }
                }
                list[pos] = 0;

                newList.Add(val);
            }

            return newList;
        }
    }
    class IpAdressCalc
    {
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

        static public List<string> printAdresses(List<int> prefixes, List<byte> numbers)
        {
            List<string> ipAdresses = new List<string>();

            int add_value = 0;
            int selected_part = 4;
            int num;

            for (int i = 0; i < prefixes.Count; i++)
            {
                selected_part = calculatePart(prefixes[i])[0];
                num = calculatePart(prefixes[i])[1];
                ipAdresses.Add($"{numbers[1]}.{numbers[2]}.{numbers[3]}.{numbers[4]} /{prefixes[i]} Subnet mask: {getSubnetMask(prefixes[i])}");
                add_value = getValue(num);

                if (add_value + numbers[selected_part] > 255)
                {
                    numbers[selected_part - 1]++;
                    add_value = 0;
                    numbers[selected_part] = 0;
                }
                numbers[selected_part] += Convert.ToByte(add_value);

            }

            return ipAdresses;
        }

        static int[] calculatePart(int prefix)
        {
            int[] res = new int[2];
            if (prefix < 8)
            {
                res[0] = 1;
                res[1] = 8 - prefix;

            }
            else if (prefix < 16)
            {
                res[0] = 2;
                res[1] = 8 - (prefix - 8);

            }
            else if (prefix < 24)
            {
                res[0] = 3;
                res[1] = 8 - (prefix - 16);
            }
            else
            {

                res[0] = 4;
                res[1] = 32 - prefix;
            }
            return res;
        }

        static int getValue(int num)
        {
            return Convert.ToInt32(Math.Pow(2, num));
        }

        static public List<int> getPrefixList(List<int> hosts)
        {
            List<int> prefixes = new List<int>();

            for (int i = 0; i < hosts.Count; i++)
            {
                prefixes.Add(getPrefix(hosts[i]));
            }

            return prefixes;
        }

        static int getPrefix(int hostNum)
        {
            int prefix = 0;
            int powNum = 0;

            for (int i = 0; true; i++)
            {
                powNum = i;

                if (Math.Pow(2, powNum) >= hostNum + 2)
                {
                    prefix = 32 - powNum;
                    break;
                }
            }

            return prefix;
        }
    }
}
