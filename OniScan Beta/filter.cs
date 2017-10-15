using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace spam
{
    class filter
    {
        public static string all_t;
        public static int onions_total;
        public static string Onion_Filter(string input)
        {
            var linkParser = new Regex(@"https?://[\w\.]+\.\w+(:\d{1,5})?(/[\w?&.=]+)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);      //Update this, since new TOR update with longer URLs has been released.
            foreach (Match m in linkParser.Matches(input))
            {
                all_t += m.Value + "\n";

                Console.WriteLine(m.Value);

                onions_total += 1;
            }
            return all_t;
        }
    }
}
