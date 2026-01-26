using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Lamb.UI;
using Lamb.UI.UpgradeMenu;

namespace CultOfArchipelago.Patches
{
    /*[HarmonyPatch(typeof(UIUpgradeShopMenuController), nameof(UIUpgradeShopMenuController.OnShowStarted))]
    public static class APPatches
    {
        public static void Prefix(UIUpgradeShopMenuController __instance)
        {
            // Example patch logic
            Plugin.Logger.LogInfo("UIUpgradeShopMenuController OnShowStarted called.");
            foreach(UIUpgradeShopMenuController.AvailableUpgrades upgrade in __instance._upgrades)
            {
                Plugin.Logger.LogInfo($"Available Upgrade: {upgrade.Type}, {upgrade.RequireUnlockedType}, {upgrade.RequireUnlocked}, {upgrade.CheckUnlocked}");
            }
        }
    }

    [HarmonyPatch(typeof(UpgradeTreeNode), nameof(UpgradeTreeNode.IsAvailable))]
    public static class UpgradeTree
    {
        public static bool Prefix(ref bool __result)
        {
            Plugin.Logger.LogInfo($"UpgradeTreeNode Prefix was hit!");
            __result = true;
            return false;
        }
    }*/

    [HarmonyPatch(typeof(UIUpgradeUnlockOverlayController), nameof(UIUpgradeUnlockOverlayController.IsAvailable))]
    public static class UpgradeUnlockOverlayControllerPatch
    {
        public static bool Prefix(ref bool __result)
        {
            //__result = true;
            Plugin.Logger.LogInfo($"UIUpgradeUnlockOverlayController IsAvailable Prefix was hit!");
            return true;
        }
    }
}
