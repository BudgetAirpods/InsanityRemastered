using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine;
using HarmonyLib;
namespace SanityRewrittenMod.Patches
{
    [HarmonyPatch(typeof(FlashlightItem))]
    internal class ItemPatcher : BaseUnityPlugin
    {
        [HarmonyPatch(typeof(FlashlightItem)), HarmonyPatch("SwitchFlashlight")]
        [HarmonyPostfix]
        private static void OnUse(bool on)
        {
            if (on)
            {
                PlayerPatcher.FlashlightOn = true;
            }
            else if(!on)
            {
                PlayerPatcher.FlashlightOn = false;
            }
        }

    }
}
