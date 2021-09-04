using Jotunn.Entities;
using Jotunn.Managers;
using SimpleJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NiflheimBespoke.ConsoleCommands
{
    public class KitCommand : ConsoleCommand
    {
        public override string Name => "kit";

        public override string Help => "Spawns a tester's kit. usage: kit <group> <kit-id>";

        // ─┬ google
        //  ├─
        //
        //
        //
        public override void Run(string[] args)
        {
            var kitsfile = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, "kits.json");
            var contents = File.ReadAllText(kitsfile);
            List<KitGroup> kits = SimpleJson.SimpleJson.DeserializeObject<List<KitGroup>>(contents, SimpleJson.SimpleJson.PocoJsonSerializerStrategy);

            if (args.Length != 2)
            {
                NiflheimBespoke.Log.LogMessage("usage: kit<group> <kit-id>");

                var firstgroup = true;
                int gc = 1;
                foreach (var g in kits)
                {
                    var lastgroup = kits.Count == gc++;
                    var gSigil = " ├";

                    if (firstgroup && lastgroup)
                    {
                        gSigil = "─┬";
                    }
                    else if (firstgroup)
                    {
                        gSigil = "─┬";
                    }
                    else if (lastgroup)
                    {
                        gSigil = " └";
                    }

                    NiflheimBespoke.Log.LogMessage($"{gSigil}{g.Group}");
                    firstgroup = false;
                    int kc = 1;
                    foreach (var k in g.Kits)
                    {
                        var lastkit = g.Kits.Count == kc++;

                        var kSigil = $"{(lastgroup ? " ":" │")}  ├─";

                        if (lastkit)
                        {
                            kSigil = $"{(lastgroup ? " " : " │")}  └─";
                        }

                        NiflheimBespoke.Log.LogMessage($"{kSigil}{k.Id}");
                    }
                }

                return;
            }

            NiflheimBespoke.Log.LogMessage("Attempting to spawn kit...");

            var group = args[0];

            var kit = args[1];

            if (String.IsNullOrWhiteSpace(group))
            {
                NiflheimBespoke.Log.LogMessage("usage: kit <group> <kit-id>.  Group was empty");
            }

            if (String.IsNullOrWhiteSpace(kit))
            {
                NiflheimBespoke.Log.LogMessage("usage: kit <group> <kit-id>.  Kit-id was empty");
            }

            var selectedGroup = kits.SingleOrDefault(g => g.Group.Equals(group, StringComparison.InvariantCultureIgnoreCase));

            if (selectedGroup == null)
            {
                NiflheimBespoke.Log.LogMessage($"group {group} not found in kits.json");
                return;
            }

            var selectedKit = selectedGroup.Kits.SingleOrDefault(k => k.Id.Equals(kit, StringComparison.InvariantCultureIgnoreCase));

            if (selectedKit == null)
            {
                NiflheimBespoke.Log.LogMessage($"kit {kit} not found in group {group} in kits.json");
                return;
            }

            foreach (var i in selectedKit.Items)
            {
                try
                {
                    NiflheimBespoke.Log.LogMessage($"Spawning Kit Item: {i.Name} count of {i.Count ?? 1}");

                    for (int _ = 0; _ < (i.Count ?? 1); _++)
                    {
                        var o = UnityEngine.Object.Instantiate(PrefabManager.Instance.GetPrefab(i.Name), Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.forward * 2f + Vector3.up, Quaternion.identity);
                        var drop = o.GetComponent<ItemDrop>();
                        if (drop != null)
                        {
                            try
                            {
                                drop.m_itemData.m_quality = (int)Math.Min((uint)(i.Quality ?? 1), (int)(drop?.m_itemData?.m_shared?.m_maxQuality));
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    NiflheimBespoke.Log.LogError(e);
                }
                
            }

            foreach (var s in selectedKit.Skills)
            {
                try
                {
                    NiflheimBespoke.Log.LogMessage($"Setting Skill {s.Name} to {s.Level??0.00f}");
                    Player.m_localPlayer.m_skills.CheatResetSkill(s.Name);
                    Player.m_localPlayer.m_skills.CheatRaiseSkill(s.Name, s.Level?? 0.00f);
                }
                catch (Exception e)
                {
                    NiflheimBespoke.Log.LogError(e);
                }
            }

            NiflheimBespoke.Log.LogMessage($"Finished spawning kit.  Here are testing guidelines for this kit: \r\n {selectedKit.Help}");
        }

        private class KitGroup
        {
            public string Group { get; set; }

            public List<Kit> Kits { get; set; } = new List<Kit>();
        }

        private class Kit
        {
            public string Id { get; set; }

            public List<ItemDef> Items { get; set; } = new List<ItemDef>();

            public List<SkillDef> Skills { get; set; } = new List<SkillDef>();

            public string Help { get; set; }
        }

        private class ItemDef
        {
            public string Name { get; set; }
            public int? Count { get; set; }
            public int? Quality { get; set; }
        }

        private class SkillDef
        {
            public string Name { get; set; }
            public float? Level { get; set; }
        }
    }
}
