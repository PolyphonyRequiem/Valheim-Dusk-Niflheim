using Jotunn.Entities;
using Jotunn.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiflheimBespoke.ConsoleCommands
{
    public class DumpAllItemsCommand : ConsoleCommand
    {
        public override string Name => "dumpitems";

        public override string Help => "Writes information about all items currently registered to itemsdump.txt in the dll path for NiflheimBespoke.";

        public override void Run(string[] args)
        {
            NiflheimBespoke.Log.LogMessage("Dumping items to itemsdump.txt");
            List<string> outputContents = new List<string>();
            foreach (ItemDrop.ItemData.ItemType itemType in Enum.GetValues(typeof(ItemDrop.ItemData.ItemType)))
            {
                var allItemsOfType = ObjectDB.instance.GetAllItems(itemType, "");

                outputContents.Add($"---{Enum.GetName(typeof(ItemDrop.ItemData.ItemType), itemType)}---");
                outputContents.Add($"");
                foreach (var item in allItemsOfType)
                {
                    outputContents.Add($"{item.name}:{item.m_itemData.m_shared.m_name}");
                }
                outputContents.Add($"");
            }

            File.WriteAllLines("itemsdump.txt", outputContents);
        }
    }
}
