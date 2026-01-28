using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace CultOfArchipelago
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        private static Harmony harmony;

        public Plugin()
        {
        }

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
            //harmony = Harmony.CreateAndPatchAll(typeof(Patches));
            harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            /*new GameObject
            {
                name = "CultOfArchipelagoController",
                hideFlags = HideFlags.HideAndDontSave
            }.AddComponent<Plugin>();*/
        }

        private void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
    }
}
