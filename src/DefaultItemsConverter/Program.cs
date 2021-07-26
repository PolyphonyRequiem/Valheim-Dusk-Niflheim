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
        static Regex matchExpression = new Regex(@"(?<itemName>[a-zA-Z]+):\s*Price\s*=\s*(?<price>\d+)\s*SellPrice\s*=\s*(?<sellPrice>\d+)\sTradeable\s*=\s*(?<tradeable>true|false)\s*IgnoreWaitForDiscovery\s*=\s*(?<discovery>true|false)\s*Sellable\s*=\s*(?<sellable>true|false)");

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
            List<ItemData> itemData = new List<ItemData>();
            foreach (var line in File.ReadAllLines(@".\vendor.cfg"))
            {
                var result = matchExpression.Match(line);

                if (!result.Success)
                {
                    continue;
                }

                itemData.Add(
                    new ItemData(
                        result.Groups["itemName"].Value,
                        int.Parse(result.Groups["price"].Value),
                        int.Parse(result.Groups["sellPrice"].Value),
                        bool.Parse(result.Groups["tradeable"].Value),
                        bool.Parse(result.Groups["sellable"].Value),
                        bool.Parse(result.Groups["discovery"].Value)));
            }

            var configFile = File.ReadAllLines(@".\Menthus.bepinex.plugins.BetterTrader.cfg");

            int i = 0;
            SearchState state = SearchState.Find;
            //[C_Items.Ammo.Bigfirearrow]
            Regex headerExpression = new Regex(@"\[C_Items\.\w*\.(?<itemName>[a-zA-Z0-9_]*)\]");
            ItemData matchingItem = default(ItemData);

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

                            if (matchingItem != null)
                            {
                                state = SearchState.Price;
                                i += 4;
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

            File.WriteAllLines("out.cfg", configFile);
        }
    }
}
