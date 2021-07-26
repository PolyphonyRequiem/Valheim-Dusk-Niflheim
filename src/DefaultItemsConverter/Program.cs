using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace DefaultItemsConverter
{
    class Program
    {
        static Regex matchExpression = new Regex(@"(?<itemName>[a-zA-Z]+):\s*Stack\s*=\s*(?<stackSize>\d+?)\s*Price\s*=\s*(?<price>\d+)\s*SellPrice\s*=\s*(?<sellPrice>\d+)\sTradeable\s*=\s*(?<tradeable>true|false)\s*IgnoreWaitForDiscovery\s*=\s*(?<discovery>true|false)\s*Sellable\s*=\s*(?<sellable>true|false)");
        static void Main(string[] args)
        {
            List<string> output = new List<string>();
            foreach (var line in File.ReadAllLines(@".\vendor.cfg"))
            {
                var result = matchExpression.Match(line);

                if (!result.Success)
                {
                    continue;
                }

                output.Add($"[C_Items.Misc.{result.Groups["itemName"]}]");
                output.Add($"Purchase Price = {Math.Ceiling((double)int.Parse(result.Groups["price"].Value)/int.Parse(result.Groups["stackSize"].Value))}");
                output.Add($"Sell Price = {Math.Ceiling((double)int.Parse(result.Groups["sellPrice"].Value) / int.Parse(result.Groups["stackSize"].Value))}");
                output.Add($"Tradeable = {bool.Parse(result.Groups["tradeable"].Value)}");
                output.Add($"Sellable = {bool.Parse(result.Groups["sellable"].Value)}");
                output.Add($"Ignore Trader Wait For Discovery = {bool.Parse(result.Groups["discovery"].Value)}");
                output.Add($"");
            }

            File.WriteAllLines("out.cfg", output);
        }
    }
}
