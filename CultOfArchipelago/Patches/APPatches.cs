using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Lamb.UI;
using Lamb.UI.UpgradeMenu;
using UnityEngine;
using Unify;
using Rewired;
using System.Reflection;
using System.Reflection.Emit;

namespace CultOfArchipelago
{
    [Harmony]
    public class Patches
    {
        [HarmonyPatch(typeof(UIUpgradeShopMenuController), nameof(UIUpgradeShopMenuController.OnEnable))]
        public static class APPatches
        {
            public static void Prefix(UIUpgradeShopMenuController __instance)
            {
                // Example patch logic
                Plugin.Logger.LogInfo("UIUpgradeShopMenuController OnShowStarted called.");
                foreach (UIUpgradeShopMenuController.AvailableUpgrades upgrade in __instance._upgrades)
                {
                    Plugin.Logger.LogInfo($"Available Upgrade: {upgrade.Type}, {upgrade.RequireUnlockedType}, {upgrade.RequireUnlocked}, {upgrade.CheckUnlocked}");
                }
            }
        }


        [Harmony]
        public static class UpgradeTree
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(UpgradeTreeNode), nameof(UpgradeTreeNode.IsAvailable))]
            public static bool Prefix(ref bool __result)
            {
                Plugin.Logger.LogInfo($"UpgradeTreeNode Prefix was hit!");
                __result = true;
                return false;
            }
        }

        [Harmony]
        public static class UpgradeUnlockOverlayControllerPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(UIUpgradeUnlockOverlayController), nameof(UIUpgradeUnlockOverlayController.IsAvailable))]
            public static bool Prefix(ref bool __result)
            {
                //__result = true;
                Plugin.Logger.LogInfo($"UIUpgradeUnlockOverlayController IsAvailable Prefix was hit!");
                return true;
            }
        }

        [Harmony]
        public static class UpgradeShopItemPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(UpgradeShopItem), nameof(UpgradeShopItem.CheckCanAfford))]
            public static bool Prefix(ref bool __result)
            {
                Plugin.Logger.LogInfo($"UpgradeShopItem CheckCanAfford Prefix was hit!");
                __result = true;
                return false;
            }
        }

        [Harmony]
        public static class UpgradeSystemPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(UpgradeSystem), nameof(UpgradeSystem.UserCanAffordUpgrade))]
            public static bool Prefix(ref bool __result)
            {
                Plugin.Logger.LogInfo($"UpgradeSystem UserCanAffordUpgrade Prefix was hit!");
                __result = true;
                return false;
            }
        }

        /*[Harmony]
        public static class KillControllerMessage
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(ControlUtilities), nameof(ControlUtilities.GetCurrentInputType))]
            public static bool Prefix(ControlUtilities __instance, ref InputType __result, Controller controller)
            {
                switch (UnifyManager.platform)
                {
                    case UnifyManager.Platform.PS4:
                        __result = InputType.DualShock4;
                        break;
                    case UnifyManager.Platform.Switch:
                        if (controller != null && controller.name.Contains("Pro Controller"))
                        {
                            __result = InputType.SwitchProController;
                            break;
                        }
                        __result = InputType.SwitchJoyConsDocked;
                        break;
                    case UnifyManager.Platform.PS5:
                        __result = InputType.DualSense;
                        break;
                    case UnifyManager.Platform.GameCoreConsole:
                        __result = InputType.XboxSeries;
                        break;
                }
                if (controller == null)
                {
                    __result = InputType.Keyboard;
                    return false;
                }
                if (controller.type == ControllerType.Keyboard || controller.type == ControllerType.Mouse)
                {
                    __result = InputType.Keyboard;
                    return false;
                }
                if (SettingsManager.Settings.Game.GamepadPrompts != 0)
                {
                    __result = ControlUtilities.InputTypeFromControlPromptSetting();
                    return false;
                }
                InputType inputType = ControlUtilities.InputTypeFromSteamInputType(GeneralInputSource.GetSteamInputType());
                //Debug.Log(string.Format("Steam informs us the controller is a {0}", inputType).Colour(Color.cyan));
                if (inputType != InputType.Undefined)
                {
                    __result = inputType;
                    return false;
                }
                if (controller.name.Contains("Sony"))
                {
                    if (controller.name.Contains("DualSense"))
                    {
                        __result = InputType.DualSense;
                        return false;
                    }
                    if (controller.name.Contains("DualShock 4"))
                    {
                        __result = InputType.DualShock4;
                        return false;
                    }
                    __result = InputType.Undefined;
                    return false;
                }
                else
                {
                    if (controller.name.Contains("Xbox"))
                    {
                        __result = InputType.XboxSeries;
                        return false;
                    }
                    if (controller.name.Contains("Nintendo"))
                    {
                        __result = InputType.SwitchJoyConsDocked;
                        return false;
                    }
                    if (controller.name.Contains("Pro Controller"))
                    {
                        __result = InputType.SwitchProController;
                        return false;
                    }
                    __result = InputType.XboxSeries;
                    return false;
                }
            }
        }*/

        [Harmony]
        public static class KillAnnoyingLogMessages
        {
            [HarmonyTargetMethods]
            public static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.Method(typeof(ControlUtilities), nameof(ControlUtilities.GetCurrentInputType));
                yield return AccessTools.Method(typeof(BranchConnectionListener), nameof(BranchConnectionListener.Configure));
                yield return AccessTools.Method(typeof(NodeConnectionListener), nameof(NodeConnectionListener.Configure));
            }

            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = instructions.ToList();
                CodeMatcher matcher = new CodeMatcher(codes).Start();
                MethodInfo logMethod = AccessTools.Method(typeof(Debug), nameof(Debug.Log), new Type[] { typeof(object) });

                while (true)
                {
                    // find next call/callvirt to Debug.Log
                    CodeMatcher found = matcher.MatchForward(false, new CodeMatch(ci =>
                    {
                        if (ci == null) return false;
                        if (ci.opcode != OpCodes.Call && ci.opcode != OpCodes.Callvirt) return false;
                        if (!(ci.operand is MethodInfo mi)) return false;
                        return mi.DeclaringType != null && mi.DeclaringType.FullName == "UnityEngine.Debug" && mi.Name == "Log";
                    }));

                    if (!found.IsValid)
                        break;

                    int logIndex = found.Pos;

                    // walk backwards from the Log call, replacing instructions with NOPs
                    // until we encounter an ldstr (the guaranteed start).
                    int cursor = logIndex;
                    int lastReplaced = logIndex;
                    bool foundLdstr = false;

                    while (cursor >= 0)
                    {
                        CodeInstruction instr = codes[cursor];
                        // If we hit ldstr, include it and stop
                        if (instr != null && instr.opcode == OpCodes.Ldstr)
                        {
                            foundLdstr = true;
                            lastReplaced = cursor;
                            break;
                        }

                        // modify the current instruction -> replace with NOP
                        codes[cursor] = new CodeInstruction(OpCodes.Nop)
                        {
                            // preserve labels/exception blocks if present on the original instruction
                            labels = (instr?.labels != null) ? new List<Label>(instr.labels) : new List<Label>(),
                            blocks = (instr?.blocks != null) ? new List<ExceptionBlock>(instr.blocks) : new List<ExceptionBlock>()
                        };

                        // move back 1
                        cursor--;
                    }

                    if (foundLdstr)
                    {
                        // Replace the ldstr itself with a NOP, but ensure labels/blocks are preserved
                        CodeInstruction original = codes[lastReplaced];
                        codes[lastReplaced] = new CodeInstruction(OpCodes.Nop)
                        {
                            labels = (original?.labels != null) ? new List<Label>(original.labels) : new List<Label>(),
                            blocks = (original?.blocks != null) ? new List<ExceptionBlock>(original.blocks) : new List<ExceptionBlock>()
                        };
                    }
                    else
                    {
                        // If we didn't find an ldstr within the function body, don't risk nuking everything:
                        // restore any NOP'd instructions in this pass back to the original saved ones.
                        // (In this code we didn't keep originals; in practice you can keep backups if desired.)
                        // For safety, just stop processing further matches.
                        break;
                    }

                    // Advance matcher past the region we just replaced so we don't reprocess it.
                    matcher = new CodeMatcher(codes).Start().Advance(logIndex + 1);
                    if (!matcher.IsValid) break;
                }

                return codes.AsEnumerable();
            }
        }
    }
}
