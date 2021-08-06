// JotunnModStub
// a Valheim mod skeleton using Jötunn
// 
// File:    JotunnModStub.cs
// Project: JotunnModStub

using BepInEx;
using UnityEngine;
using BepInEx.Configuration;
using Jotunn.Utils;
using Jotunn.Managers;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.IO;
using System.Reflection;
using BepInEx.Logging;
using Jotunn.Entities;
using Jotunn.Configs;
using NiflheimBespoke.ConsoleCommands;

namespace NiflheimBespoke
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    internal class NiflheimBespoke : BaseUnityPlugin
    {
        public const string PluginGUID = "firoso.niflheim.bespoke";
        public const string PluginName = "NiflheimBespoke";
        public const string PluginVersion = "0.1.1";

        public static ManualLogSource Log { get; private set; }

        private AssetBundle niflheimBespokeAssets;

        private void Awake()
        {
            Log = this.Logger;
            this.niflheimBespokeAssets = AssetUtils.LoadAssetBundleFromResources("niflheimbespoke", Assembly.GetExecutingAssembly());
            ItemManager.OnVanillaItemsAvailable += InitializeItemsFromVanillaAvailableEvent;
            CommandManager.Instance.AddConsoleCommand(new DumpAllItemsCommand());
        }

        private void InitializeItemsFromVanillaAvailableEvent()
        {
            ConfigureCoreWoodChest();
            ConfigureGoblinFetish();
        }

        private void ConfigureGoblinFetish()
        {
            var goblinFetishPrefab = PrefabManager.Instance.CreateClonedPrefab("GoblinFetish", "GoblinTotem");

            var goblinFetishItemDrop = goblinFetishPrefab.GetComponent<ItemDrop>();
            goblinFetishItemDrop.m_itemData.m_shared.m_name = "Goblin Fetish";
            goblinFetishItemDrop.m_itemData.m_shared.m_description = "Believed by some supersitious Fulings to be an idol imbuded with great power, protecting them in battle.  Looks like it didn't work.";

            var goblinFetish = new CustomItem(goblinFetishPrefab, false)
            {
                ItemDrop = goblinFetishItemDrop
            };

            ItemManager.Instance.AddItem(goblinFetish);
        }

        private void ConfigureCoreWoodChest()
        {
            var coreWoodChestPrefab = this.niflheimBespokeAssets.LoadAsset<GameObject>("piece_chest_coreWood");
            var coreWoodChestContainer = coreWoodChestPrefab.GetComponent<Container>();
            coreWoodChestContainer.m_width = 6;
            coreWoodChestContainer.m_height = 3;

            CustomPiece piece = new CustomPiece(coreWoodChestPrefab, new PieceConfig
            {
                Category = "Furniture",
                CraftingStation = "piece_workbench",
                Description = "A sturdy chest made from fine and core wood, and a few nails to hold it together.",
                PieceTable = "Hammer",
                Icon = PrefabManager.Cache.GetPrefab<Sprite>("pirate_woodwood"),
                Name = "Sturdy Chest",
                Requirements = new RequirementConfig[]
                {
                        new RequirementConfig{Item = "RoundLog", Amount=5, Recover=true},
                        new RequirementConfig{Item = "FineWood", Amount=5, Recover=true},
                        new RequirementConfig{Item = "BronzeNails", Amount=8, Recover=true},
                }
            });

            if (PieceManager.Instance.AddPiece(piece))
            {
                Log.LogMessage("Sturdy Chest registered");
            }
            else
            {
                Log.LogError("Sturdy Chest not registered!");
            }
        }

        private static GameObject GetPrefabByName(string name)
        {
            var itemPrefab = PrefabManager.Instance.GetPrefab(name);
            if (itemPrefab == null)
            {
                throw new InvalidOperationException($"Unable to resolve prefab by from name '{name}'");
            }

            return itemPrefab;
        }

        private static T GetComponentFromPrefab<T>(GameObject prefab)
        {
            var component = prefab.GetComponent<T>();
            if (component == null)
            {
                throw new InvalidOperationException($"Unable to resolve '{typeof(T)}' component on item named '{prefab.name}'");
            }

            return component;
        }

        private static T GetComponentFromPrefab<T>(string prefabName)
        {
            return GetComponentFromPrefab<T>(GetPrefabByName(prefabName));
        }
    }
}