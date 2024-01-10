using HarmonyLib;
using InsanityRemasteredMod.Mod_Integrations;
using SanityRewrittenMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace InsanityRemasteredMod
{
    internal class ModIntegrator
    {
        public static void BeginIntegrations(Assembly assembly)
        {
            if (assembly.FullName.StartsWith("SkinwalkerMod"))
            {
                InsanityRemasteredBase.mls.LogMessage("Skinwalker mod installed, starting integration.");

                Harmony harmony = new Harmony("skinwalker");
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i].Name == "SkinwalkerModPersistent")
                    {
                        MethodInfo test = types[i].GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                        HarmonyMethod harmonyMethod = new HarmonyMethod(typeof(SkinwalkerModIntegration).GetMethod("UpdateClips", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy));
                        harmony.Patch(test, harmonyMethod);
                    }
                }
            }
            if (assembly.FullName.StartsWith("AdvancedCompany"))
            {
                InsanityRemasteredBase.mls.LogMessage("AdvancedCompany mod installed, starting integration.");

                Harmony harmony = new Harmony("advancecompany ");
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i].Name == "NightVision" && types[i].Namespace == "AdvancedCompany.Objects")
                    {
                        InsanityRemasteredBase.mls.LogMessage("NightVision object found, starting method patching.");
                        MethodInfo useFlashlight = types[i].GetMethod("SwitchFlashlight", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                        MethodInfo unequip = types[i].GetMethod("Unequipped", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                        HarmonyMethod nightVisionUse = new HarmonyMethod(typeof(AdvancedCompanyIntegration).GetMethod("NightVisionUse", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy));
                        HarmonyMethod unequipGoggles = new HarmonyMethod(typeof(AdvancedCompanyIntegration).GetMethod("UnequipNightVision", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy));

                        harmony.Patch(useFlashlight, nightVisionUse);
                        harmony.Patch(unequip, unequipGoggles);
                    }

                }
            }
        }

    }
}
