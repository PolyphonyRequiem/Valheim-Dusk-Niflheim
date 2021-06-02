using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace PatchNotesExtender
{
    [BepInPlugin("firoso.niflheim.patchnotesextender", PatchNotesExtender.ModName, PatchNotesExtender.Version)]
    public class PatchNotesExtender : BaseUnityPlugin
    {
        public const string Version = "0.1";
        public const string ModName = "Patch Notes Extender";
        Harmony _Harmony;
        public static ManualLogSource Log;

        public static ConfigEntry<string> PatchNotesUri;

        private void Awake()
        {
#if DEBUG
			Log = Logger;
#else
            Log = new ManualLogSource(null);
#endif
            _Harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);

            PatchNotesUri = Config.Bind("General", "PatchNotesUri", "", "The URI where patch notes can be located.");
        }

        private void OnDestroy()
        {
            if (_Harmony != null) _Harmony.UnpatchSelf();
        }
    }
}
