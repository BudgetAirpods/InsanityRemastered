using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using HarmonyLib;
namespace SanityRewrittenMod.Patches
{
    [HarmonyPatch(typeof(FlashlightItem))]
    internal class ItemPatcher
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void OnUse(ref FlashlightItem __instance)
        {
            if (__instance.isHeld)
            {
                if (__instance.isBeingUsed)
                {
                    PlayerPatcher.FlashlightOn = true;
                }
                else
                {
                    PlayerPatcher.FlashlightOn = false;
                }
            }
        }

    }
}
