// JotunnModStub
// a Valheim mod skeleton using Jötunn
// 
// File:    JotunnModStub.cs
// Project: JotunnModStub

using BepInEx;
using UnityEngine;
using Jotunn.Utils;
using Jotunn.Managers;
using System;
using System.Reflection;
using BepInEx.Logging;
using Jotunn.Entities;
using Jotunn.Configs;
using NiflheimBespoke.ConsoleCommands;
using HarmonyLib;
using System.Linq;

namespace NiflheimBespoke
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    internal class NiflheimBespoke : BaseUnityPlugin
    {
        public const string PluginGUID = "firoso.niflheim.bespoke";
        public const string PluginName = "NiflheimBespoke";
        public const string PluginVersion = "0.2.1";

        public static ManualLogSource Log { get; private set; }

        private AssetBundle niflheimBespokeAssets;

        private Harmony harmonyInstance;

        private void Awake()
        {          
            Log = this.Logger;
            harmonyInstance = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            this.niflheimBespokeAssets = AssetUtils.LoadAssetBundleFromResources("niflheimbespoke", Assembly.GetExecutingAssembly());
            ItemManager.OnVanillaItemsAvailable += InitializeItemsFromVanillaAvailableEvent;
            CommandManager.Instance.AddConsoleCommand(new DumpAllItemsCommand());
        }

        private void OnDestroy()
        {
            if (harmonyInstance != null) harmonyInstance.UnpatchSelf();
        }

        private void InitializeItemsFromVanillaAvailableEvent()
        {
            ItemManager.OnVanillaItemsAvailable -= InitializeItemsFromVanillaAvailableEvent;
            ConfigureSturdyChest();
            ConfigureGoblinFetish();
            ConfigurePocketPortal();
            ConfigureBoarDrops();
            ConfigureDireBoar();
        }

        private void ConfigureDireBoar()
        {
            Log.LogMessage("Setting up DireBoar Feature");
            var direboar = PrefabManager.Instance.CreateClonedPrefab("DireBoar", "Boar");
            var attack = PrefabManager.Instance.CreateClonedPrefab("direboar_base_attack", "boar_base_attack");
            var direBoarMaterial = this.niflheimBespokeAssets.LoadAsset<Material>("DireBoar");

            direboar.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            direboar.GetComponentsInChildren<SkinnedMeshRenderer>().Where(c=>c.name == "Poly Art Boar").Single().material = direBoarMaterial;
            direboar.GetComponentsInChildren<Component>(true).Where(c => c.name == "Fangs 004").First().gameObject.SetActive(false);
            direboar.GetComponentsInChildren<Component>(true).Where(c => c.name == "Fangs 005").First().gameObject.SetActive(true);
            direboar.GetComponentsInChildren<Component>(true).Where(c => c.name == "Fangs 006").First().gameObject.SetActive(true);

            // setup attack
            var attackShared = attack.GetComponent<ItemDrop>().m_itemData.m_shared;
            attackShared.m_name = "direboar_attack";
            attackShared.m_damages.m_blunt = 22;
            attackShared.m_damages.m_pierce = 6;
            attackShared.m_attackForce = 55;

            // setup humanoid
            var direboarHumanoid = direboar.GetComponent<Humanoid>();
            direboarHumanoid.m_name = "Dire Boar";
            direboarHumanoid.m_defaultItems[0] = attack;
            direboarHumanoid.m_health = 40;

            // setup drops
            var drops = direboar.GetComponent<CharacterDrop>();
            var rawMeatDrop = drops.m_drops.Find(d => d.m_prefab.name == "RawMeat");
            drops.m_drops.Remove(rawMeatDrop);
            
            var leatherScrapsDrop = drops.m_drops.Find(d => d.m_prefab.name == "LeatherScraps");
            leatherScrapsDrop.m_amountMax = 2;

            var trophyBoarDrop = drops.m_drops.Find(d => d.m_prefab.name == "TrophyBoar");
            trophyBoarDrop.m_chance = 0.25f;

            drops.m_drops.Add(new CharacterDrop.Drop { m_amountMax = 1, m_amountMin = 1, m_chance = 1, m_levelMultiplier = true, m_onePerPlayer = false, m_prefab = GetPrefabByName("rk_pork") });
            Log.LogMessage("Registering DireBoar");

            PrefabManager.Instance.AddPrefab(direboar);
        }

        //private void ConfigureDireBoar()
        //{
        //    Log.LogMessage("Setting up DireBoar Feature");
        //    var direBoarPrefab = this.niflheimBespokeAssets.LoadAsset<GameObject>("DireBoar");

        //    var vfxboardeath = PrefabManager.Cache.GetPrefab<GameObject>("vfx_boar_death");
        //    var boarragdoll = PrefabManager.Cache.GetPrefab<GameObject>("boar_ragdoll");
        //    var sfxboardeath = PrefabManager.Cache.GetPrefab<GameObject>("sfx_boar_death");
        //    var vfxboarhit = PrefabManager.Cache.GetPrefab<GameObject>("vfx_boar_hit");
        //    var sfxboarhit = PrefabManager.Cache.GetPrefab<GameObject>("sfx_boar_hit");
        //    var fxbackstab = PrefabManager.Cache.GetPrefab<GameObject>("fx_backstab");
        //    if ((new GameObject[] { vfxboardeath, boarragdoll, sfxboardeath, vfxboarhit, sfxboarhit, fxbackstab }).Any(v=>v == null))
        //    {
        //        Log.LogMessage("Some effect was null");
        //    }

        //    var death = new EffectList { m_effectPrefabs = new EffectList.EffectData[3] { new EffectList.EffectData { m_prefab = vfxboardeath }, new EffectList.EffectData { m_prefab = boarragdoll },  new EffectList.EffectData { m_prefab = sfxboardeath } } };
        //    var hit = new EffectList { m_effectPrefabs = new EffectList.EffectData[2] { new EffectList.EffectData { m_prefab = vfxboarhit }, new EffectList.EffectData { m_prefab = sfxboarhit } } };
        //    var crit = new EffectList { m_effectPrefabs = new EffectList.EffectData[1] { new EffectList.EffectData { m_prefab = fxbackstab } } };

        //    direBoarPrefab.GetComponent<Humanoid>().m_deathEffects = death;
        //    direBoarPrefab.GetComponent<Humanoid>().m_hitEffects = hit;
        //    direBoarPrefab.GetComponent<Humanoid>().m_critHitEffects = crit;

        //    direBoarPrefab.GetComponent<Humanoid>().m_name = "Dire Boar";
        //    Log.LogMessage("Registering DireBoar");
        //    PrefabManager.Instance.AddPrefab(direBoarPrefab);
        //}

        private void ConfigureBoarDrops()
        {
            Log.LogMessage("Setting up Boar Drops Restrictions Feature");

            var characterDropBoar = NiflheimBespoke.GetComponentFromPrefab<CharacterDrop>("Boar");

            var porkDrop = characterDropBoar.m_drops.Find(d => d.m_prefab.name == "rk_pork");

            characterDropBoar.m_drops.Remove(porkDrop);
            Log.LogMessage("Pork removed from drop table");
        }

        private void ConfigurePocketPortal()
        {
            Log.LogMessage("Setting up Pocket Portal Feature");
            // piecePrefab
            var pocketPortalPrefab = PrefabManager.Instance.CreateClonedPrefab("piece_PocketPortal", "portal_wood");
            Log.LogMessage($"{nameof(pocketPortalPrefab)} is {pocketPortalPrefab?.ToString() ?? "null"}");

            // icon
            var portalIcon = pocketPortalPrefab.GetComponent<Piece>().m_icon;
            Log.LogMessage($"{nameof(portalIcon)} is {portalIcon?.ToString() ?? "null"}");

            // item config
            var pocketPortalCustomItemData = new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_workbench",
                Enabled = true,
                Description = "A pocket portal device, can be deployed with a hammer.  Takes up less space than the raw materials.",
                Name = "Pocket Portal",
                Requirements = new RequirementConfig[]
                {
                    new RequirementConfig
                    {
                        Item = "FineWood",
                        Amount = 20
                    },
                    new RequirementConfig
                    {
                        Item = "SurtlingCore",
                        Amount = 2
                    },
                    new RequirementConfig
                    {
                        Item = "GreydwarfEye",
                        Amount = 10
                    },
                },
                Icons = new Sprite[]
                {
                    portalIcon
                }
            };
            Log.LogMessage($"{nameof(pocketPortalCustomItemData)} is {pocketPortalCustomItemData?.ToString() ?? "null"}");

            var pocketPortalItem = new CustomItem("PocketPortal", "SurtlingCore", pocketPortalCustomItemData);
            Log.LogMessage($"{nameof(pocketPortalItem)} is {pocketPortalItem?.ToString() ?? "null"}");

            var pocketPortalPiece = new CustomPiece(pocketPortalPrefab, new PieceConfig
            {
                AllowedInDungeons=false,
                Category="Misc",
                CraftingStation="",
                Description="Deploys a pocket portal device",
                Enabled=true,
                Icon= portalIcon,
                Name="Pocket Portal",
                PieceTable="Hammer",
                Requirements=new RequirementConfig[]
                {
                    new RequirementConfig
                    {
                        Amount=1,
                        Item="PocketPortal",
                        Recover=true
                    }
                }
            });

            pocketPortalItem.ItemDrop.m_itemData.m_shared.m_teleportable = true;

            // remove the old portal item
            var itemDrop = GetComponentFromPrefab<ItemDrop>("Hammer");
            var pieceTable = itemDrop.m_itemData.m_shared.m_buildPieces;
            var portalPiece = pieceTable.m_pieces.Find(piece => piece.name.Equals("portal_wood", StringComparison.InvariantCultureIgnoreCase));
            pieceTable.m_pieces.Remove(portalPiece);

            // wrap it up.
            if (PieceManager.Instance.AddPiece(pocketPortalPiece))
            {
                Log.LogMessage("Pocket Portal Piece registered");
            }
            else
            {
                Log.LogError("Pocket Portal Piece not registered!");
            }

            if (ItemManager.Instance.AddItem(pocketPortalItem))
            {
                Log.LogMessage("Pocket Portal Item registered");
            }
            else
            {
                Log.LogError("Pocket Portal Item not registered!");
            }
        }

        private void ConfigureGoblinFetish()
        {
            Log.LogMessage("Setting up Goblin Fetish Feature");
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

        private void ConfigureSturdyChest()
        {
            Log.LogMessage("Setting up Sturdy Chest Feature");
            var coreWoodChestPrefab = this.niflheimBespokeAssets.LoadAsset<GameObject>("piece_chest_coreWood");
            var coreWoodChestContainer = coreWoodChestPrefab.GetComponent<Container>();
            coreWoodChestContainer.m_width = 6;
            coreWoodChestContainer.m_height = 3;

            var vfxstonebuild = PrefabManager.Cache.GetPrefab<GameObject>("vfx_Place_stone_wall_2x1");
            var vfxwoodhit = PrefabManager.Cache.GetPrefab<GameObject>("vfx_SawDust");
            var sfxwoodbuild = PrefabManager.Cache.GetPrefab<GameObject>("sfx_build_hammer_wood");
            var sfxwoodbreak = PrefabManager.Cache.GetPrefab<GameObject>("sfx_wood_break");

            var buildWood = new EffectList { m_effectPrefabs = new EffectList.EffectData[2] { new EffectList.EffectData { m_prefab = sfxwoodbuild }, new EffectList.EffectData { m_prefab = vfxstonebuild } } };
            var breakWood = new EffectList { m_effectPrefabs = new EffectList.EffectData[2] { new EffectList.EffectData { m_prefab = sfxwoodbreak }, new EffectList.EffectData { m_prefab = vfxwoodhit } } };
            var hitWood = new EffectList { m_effectPrefabs = new EffectList.EffectData[1] { new EffectList.EffectData { m_prefab = vfxwoodhit } } };
            
            coreWoodChestPrefab.GetComponent<Piece>().m_placeEffect = buildWood;
            coreWoodChestPrefab.GetComponent<WearNTear>().m_destroyedEffect = breakWood;
            coreWoodChestPrefab.GetComponent<WearNTear>().m_hitEffect = breakWood;

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