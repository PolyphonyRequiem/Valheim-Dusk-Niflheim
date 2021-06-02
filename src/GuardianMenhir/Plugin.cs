using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace GuardianMenhir
{
    [BepInPlugin("firoso.niflheim.guardianmenhir", Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "0.1";
        public const string ModName = "Guardian Menhir";
        Harmony _Harmony;
        public static ManualLogSource Log;

        private void Awake()
        {
#if DEBUG
			Log = Logger;
#else
            Log = new ManualLogSource(null);
#endif
            _Harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }

        private void OnDestroy()
        {
            if (_Harmony != null) _Harmony.UnpatchSelf();
        }
    }
}
