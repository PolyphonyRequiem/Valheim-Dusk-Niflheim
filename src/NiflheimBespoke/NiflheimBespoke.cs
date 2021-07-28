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

namespace RecipeRewriter
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    internal class NiflheimBespoke : BaseUnityPlugin
    {
        public const string PluginGUID = "firoso.niflheim.bespoke";
        public const string PluginName = "NiflheimBespoke";
        public const string PluginVersion = "0.1.0";

        public static ManualLogSource Log { get; private set; }

        private AssetBundle niflheimBespokeAssets;

        private void Awake()
        {
            this.niflheimBespokeAssets = AssetUtils.LoadAssetBundleFromResources("niflheimbespoke", Assembly.GetExecutingAssembly());
            ItemManager.OnVanillaItemsAvailable += InitializeItemsFromVanillaAvailableEvent;
            Log = this.Logger;
        }

        private void InitializeItemsFromVanillaAvailableEvent()
        {
            //ConfigureCoreWoodChest();

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

        //private void ConfigureCoreWoodChest()
        //{          
        //    var coreWoodChestPrefab = PrefabManager.Instance.CreateClonedPrefab("piece_chest_corewood", "piece_chest_wood");
        //    var coreWoodChestContainer = coreWoodChestPrefab.GetComponent<Container>();

        //    // build visuals/colliders
        //    var visualSourceChestPrefab = PrefabManager.Instance.GetPrefab("Chest");
        //    if (visualSourceChestPrefab == null)
        //    {
        //        Log.LogError("visualSourceChestPrefab is null");
        //    }

        //    var visualSourceChestBase = visualSourceChestPrefab.transform.Find("Chest");
        //    if (visualSourceChestBase == null)
        //    {
        //        Log.LogError("visualSourceChestBase is null");
        //    }

        //    //Filter
        //    var chestBase = coreWoodChestPrefab.transform.Find("New/woodchest");
        //    if (chestBase == null)
        //    {
        //        Log.LogError("chestbase is null");
        //    }

        //    if (chestBase.gameObject == null)
        //    {
        //        Log.LogError("base gameobject is null");
        //    }

        //    if (visualSourceChestBase.gameObject == null)
        //    {
        //        Log.LogError("visualSource gameobject is null");
        //    }

        //    if (chestBase.gameObject == null)
        //    {
        //        Log.LogError("base gameobject is null");
        //    }

        //    if (chestBase.gameObject.GetComponent<MeshFilter>() == null)
        //    {
        //        Log.LogError("base meshfilter didn't exist");
        //    }

        //    if (visualSourceChestBase.gameObject.GetComponent<MeshFilter>() == null)
        //    {
        //        Log.LogError("visualSource meshfilter didn't exist");
        //    }

        //    if (chestBase.gameObject.GetComponent<MeshFilter>().mesh == null)
        //    {
        //        Log.LogError("base meshfilter mesh member didn't exist");
        //    }

        //    if (visualSourceChestBase.gameObject.GetComponent<MeshFilter>().mesh == null)
        //    {
        //        Log.LogError("visualSource meshfilter mesh member didn't exist");
        //    }

        //    chestBase.gameObject.GetComponent<MeshFilter>().mesh = visualSourceChestBase.gameObject.GetComponent<MeshFilter>().mesh;
        //    chestBase.gameObject.GetComponent<MeshFilter>().mesh = visualSourceChestBase.gameObject.GetComponent<MeshFilter>().mesh;


        //    //Collider
        //    var targetCollider = chestBase.gameObject.GetComponent<MeshCollider>();
        //    var sourceCollider = visualSourceChestBase.gameObject.GetComponent<MeshCollider>();
        //    targetCollider.sharedMesh = sourceCollider.sharedMesh;
        //    targetCollider.sharedMaterial = sourceCollider.sharedMaterial;
        //    targetCollider.material = sourceCollider.material;
        //    targetCollider.cookingOptions = sourceCollider.cookingOptions;
        //    targetCollider.convex = sourceCollider.convex;

        //    //Renderer
        //    var targetRenderer = chestBase.gameObject.GetComponent<MeshRenderer>();
        //    var sourceRenderer = visualSourceChestBase.gameObject.GetComponent<MeshRenderer>();

        //    //copy via reflection?
        //    targetRenderer.materials = sourceRenderer.materials;

        //    coreWoodChestContainer.m_width = 6;
        //    coreWoodChestContainer.m_height = 3;

        //    CustomPiece piece = new CustomPiece(coreWoodChestPrefab, new PieceConfig
        //    {
        //        Category = "Furniture",
        //        CraftingStation = "piece_workbench",
        //        Description = "A sturdy chest made from fine and core wood, and a few nails to hold it together.",
        //        PieceTable = "Hammer",
        //        Icon = PrefabManager.Cache.GetPrefab<Sprite>("pirate_woodwood"),
        //        Name = "Sturdy Chest",
        //        Requirements = new RequirementConfig[]
        //        {
        //            new RequirementConfig{Item = "RoundLog", Amount=5, Recover=true},
        //            new RequirementConfig{Item = "FineWood", Amount=5, Recover=true},
        //            new RequirementConfig{Item = "BronzeNails", Amount=8, Recover=true},
        //        }
        //    });

        //    if (PieceManager.Instance.AddPiece(piece))
        //    {
        //        Log.LogMessage("Sturdy Chest registered");
        //    }
        //    else
        //    {
        //        Log.LogError("Sturdy Chest not registered!");
        //    }
        //}

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