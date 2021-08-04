using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DefaultItemsConverter
{
    class Program
    {
        private record ItemData(string name, int price, int sellPrice, bool tradeable, bool sellable, bool ignorewaitfordiscovery);
        static Regex matchExpression = new Regex(@"(?<itemName>[a-zA-Z]+):?\s*,\s*(?<price>\d+)\s*,\s*(?<sellPrice>\d+)\s*,\s*(?<tradeable>true|false)\s*,\s*(?<discovery>true|false)\s*,\s*(?<sellable>true|false)", RegexOptions.IgnoreCase);

        enum SearchState
        {
            Find,
            Price,
            Sell,
            Tradeable,
            Sellable,
            Ignore
        }

        static void Main(string[] args)
        {
            string datafile = @".\testdata.csv";
            string sourceConfigFile = @".\Menthus.bepinex.plugins.BetterTrader.cfg";
            string outFile = @"out.cfg";

            if (args.Length != 0)
            {
                datafile = args[0];
                sourceConfigFile = args[1];
                outFile = args[2];
            }

            Console.WriteLine($"\n\n\n------------------------------\nReading Data File {datafile}\n------------------------------\n\n\n");

            List<ItemData> itemData = new List<ItemData>();
            foreach (var line in File.ReadAllLines(datafile))
            {
                var result = matchExpression.Match(line);

                if (!result.Success)
                {
                    continue;
                }

                var newItemData = new ItemData(
                       result.Groups["itemName"].Value,
                       int.Parse(result.Groups["price"].Value),
                       int.Parse(result.Groups["sellPrice"].Value),
                       bool.Parse(result.Groups["tradeable"].Value),
                       bool.Parse(result.Groups["sellable"].Value),
                       bool.Parse(result.Groups["discovery"].Value));

               itemData.Add(newItemData);

               Console.WriteLine($"Found Item:\n   {newItemData}");
            }

            var configFile = File.ReadAllLines(sourceConfigFile);

            int i = 0;
            SearchState state = SearchState.Find;
            //[C_Items.Ammo.Bigfirearrow]
            Regex headerExpression = new Regex(@"\[C_Items\.\w*\.(?<itemName>[a-zA-Z0-9_]*)\]");
            ItemData matchingItem = default(ItemData);

            Console.WriteLine($"\n\n\n------------------------------\nUpdating Configuration File {sourceConfigFile}\n------------------------------\n\n\n");
            while (i < configFile.Length)
            {
                string line = configFile[i];

                switch (state)
                {
                    case SearchState.Find:
                        var headerMatch = headerExpression.Match(line);
                        if (headerMatch.Success)
                        {
                            var itemName = headerMatch.Groups["itemName"].Value;
                            matchingItem = itemData.SingleOrDefault(i => i.name == itemName);
                            
                            Console.WriteLine($"Found Config Entry:\n   {itemName}");

                            if (matchingItem != null)
                            {
                                state = SearchState.Price;
                                i += 4;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"WARNING : NO ENTRY FOUND FOR {itemName} in {datafile} -- THIS MAY BE EXPECTED!");
                                Console.ResetColor();
                            }
                        }
                        i++;
                        break;
                    case SearchState.Price:
                        configFile[i] = $"Purchase Price = {matchingItem.price}";
                        state = SearchState.Sell;
                        i += 5;
                        break;
                    case SearchState.Sell:
                        configFile[i] = $"Sell Price = {matchingItem.sellPrice}";
                        state = SearchState.Tradeable;
                        i += 5;
                        break;
                    case SearchState.Tradeable:
                        configFile[i] = $"Tradeable = {matchingItem.tradeable.ToString().ToLower()}";
                        state = SearchState.Sellable;
                        i += 5;
                        break;
                    case SearchState.Sellable:
                        configFile[i] = $"Sellable = {matchingItem.sellable.ToString().ToLower()}";
                        state = SearchState.Ignore;
                        i += 5;
                        break;
                    case SearchState.Ignore:
                        configFile[i] = $"Ignore Trader Wait For Discovery = {matchingItem.sellable.ToString().ToLower()}";
                        state = SearchState.Find;
                        i++;
                        break;
                    default:
                        i++;
                        break;
                }
            }
            Console.WriteLine($"\n\n\n------------------------------\nUpdating {outFile}\n------------------------------\n\n\n");
            File.WriteAllLines(outFile, configFile);
        }
    }
}
