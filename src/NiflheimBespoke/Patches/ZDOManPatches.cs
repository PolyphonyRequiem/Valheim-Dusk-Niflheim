using HarmonyLib;
using System.Collections.Generic;

namespace NiflheimBespoke.Patches
{
    [HarmonyPatch(typeof(ZDOMan), nameof(ZDOMan.GetAllZDOsWithPrefabIterative))]
    public static class ZDOMan_etAllZDOsWithPrefabIterative_Patch
    {
        static void Prefix(ZDOMan __instance, string prefab, ref List<ZDO> zdos, ref int index)
        {
            if (prefab == "portal_wood")
            {
                int fakeIndex = index;
                __instance.GetAllZDOsWithPrefabIterative("piece_PocketPortal", zdos, ref fakeIndex);
            }
        }
    }
}
