using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;

namespace spam
{
    class Program
    {
        public static ConsoleKeyInfo cki;
        public static List<string> onions = new List<string>();
        public static int scanned;
        public static int open;
        public static string export;
        static void Main(string[] args)
        {
            Console.Title = "OniScan v1.0 Alpha";

            if (!funcs.Tor_Startup())
            {
                Console.WriteLine("Starting tor failed. Please close any running instances of tor and try agian.");
            }
            else
            {
                Console.Write("[+] Tor client bootstrapped successfully... \n\n");

                do
                {
                    Console.WriteLine("\n\n1) Scrape onions");
                    Console.WriteLine("2) Scan onions [phpmyadmin]");
                    Console.WriteLine("3) Exit\n\n");
                    switch (Console.ReadLine())
                    {
                        case "1":
                            GetOnions();
                            break;
                        case "2":
                            ScanOnions();
                            break;
                        case "3":
                            funcs.exit();
                            break;
                    }
                } while (cki.Key != ConsoleKey.Escape);


            }     

        }
        public static void GetOnions()
        {
            
            Console.Write("\nEnter page to siphon onions from: ");
            if(Console.ReadLine() != "")
            {
                string page = funcs.Request(Console.ReadLine());
                if (page != "" && !page.Contains("Failed"))
                {
                    Console.WriteLine("\n\n[+] Page sucessfully loaded.");

                    string all_onions = filter.Onion_Filter(page);
                    Console.Write("\n\n[+] Filtering onions");

                    if (all_onions != "")
                    {

                        Console.WriteLine("\n[+] Total onions found: " + filter.onions_total);
                        Console.Write("\n\nEnter filename: ");
                        StreamWriter sw = new StreamWriter(Console.ReadLine());
                        sw.Write(all_onions);
                        sw.Close();
                        Console.Write("Saved.");
                        filter.onions_total = 0;
                        filter.all_t = "";
                        return;

                    }
                    else
                    {
                        Console.Write("\n[!] List is empty.\n\n" + page);
                    }
                    return;
                }
                else
                {
                    Console.WriteLine("\n[!] Failed to load the page.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("\n[!] Giving me no input won't do anything.");
                return;
            }
            
        }
        public static void ScanOnions()
        {
            scanned = 0;
            open = 0;
            export = "";
            Console.WriteLine("\n\n1) Single scan\n2) Mass scan\n\n");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Write("\nEnter page to scan: ");
                    string page = funcs.Request(Console.ReadLine());
                    if (page != "" && !page.Contains("Failed"))
                    {
                        Console.Write("Found.");
                    }
                    else
                    {
                        Console.Write("Nothing found.");
                    }
                    break;

                case "2":
                    Console.Write("\n\nPath to onion list: ");
                    string p = Console.ReadLine();
                    p = p.Replace(@"""", "");
                    if (File.Exists(p))
                    {
                        string[] onion_array = File.ReadAllLines(p);

                        foreach (string onion_link in onion_array)
                        {
                            try
                            {
                                string resp = funcs.Request(onion_link + "/phpmyadmin");
                                if (resp != "" && !resp.Contains("Failed") && resp.Contains("themes"))
                                {
                                    Console.Write("\n[+] " + onion_link + " | phpMyAdmin Found");
                                    if (resp.Contains("SQL upload"))
                                    {
                                        Console.Write(" | !OPEN!");
                                        onions.Add(onion_link + "phpmyadmin | !OPEN!");
                                        open++;
                                    }
                                    else
                                    {
                                        onions.Add(onion_link + "phpmyadmin");
                                    }
                                }
                                else
                                {
                                    Console.Write("\n[!] " + onion_link);
                                }
                            }
                            catch
                            {
                                Console.Write("\n[!] " + onion_link);
                            }
                            scanned++;

                            if(scanned == onion_array.Length)
                            {
                                StreamWriter sw = new StreamWriter("Scanned_Onions.txt");

                                foreach(string o in onions)
                                {
                                    export += o + "\n";
                                }
                                sw.Write(export);
                                sw.Close();
                                Console.WriteLine("\n\n[+] Scan finished!\n\nOnions scanned: " + onion_array.Length + "\nOnions with open phpMyAdmin: " + open + "\nReport saved to file: Scanned_Onions.txt");
                                Console.ReadLine();
                            }

                            Console.Title = "Scanning in progress... |  Scanned [ " + scanned + "/" + onion_array.Length + " ]  |  Open: [ " + open + " ]";                        
                            System.Threading.Thread.Sleep(10);

                        }

                    }
                    else
                    {
                        Console.WriteLine("Invalid Filepath.\n");
                        return;
                    }
                    break;

                default:
                    throw new NotImplementedException("Unrecognized value.");
                    break;
            }
        }
    }
}
