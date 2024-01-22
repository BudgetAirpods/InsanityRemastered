using HarmonyLib;
using InsanityRemasteredMod;
using System;
using System.Reflection;

namespace InsanityRemastered.ModIntegration
{
    public class ModIntegrator
    {
        public static void BeginIntegrations(Assembly assembly)
        {
            if (assembly.FullName.StartsWith("SkinwalkerMod"))
            {
                SkinwalkerModIntegration.IsInstalled = true;
                InsanityRemasteredLogger.Log("Skinwalker mod installed, starting integration.");

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
                InsanityRemasteredLogger.Log("AdvancedCompany mod installed, starting integration.");

                Harmony harmony = new Harmony("advancecompany ");
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i].Name == "NightVision" && types[i].Namespace == "AdvancedCompany.Objects")
                    {
                        InsanityRemasteredLogger.Log("NightVision object found, starting method patching.");
                        MethodInfo useFlashlight = types[i].GetMethod("SwitchFlashlight", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                        MethodInfo unequip = types[i].GetMethod("Unequipped", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                        HarmonyMethod nightVisionUse = new HarmonyMethod(typeof(AdvancedCompanyCompatibility).GetMethod("HeadLightUtilityUse", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy));
                        HarmonyMethod unequipGoggles = new HarmonyMethod(typeof(AdvancedCompanyCompatibility).GetMethod("UnequipHeadLightUtility", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy));

                        harmony.Patch(useFlashlight, nightVisionUse);
                        harmony.Patch(unequip, unequipGoggles);
                    }
                    if (types[i].Name == "HelmetLamp" && types[i].Namespace == "AdvancedCompany.Objects")
                    {
                        InsanityRemasteredLogger.Log("Helmet Lamp object found, starting fix.");
                        MethodInfo useHelmetLight = types[i].GetMethod("SwitchFlashlight", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                        MethodInfo unequip = types[i].GetMethod("Unequipped", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                        HarmonyMethod helmetLampUse = new HarmonyMethod(typeof(AdvancedCompanyCompatibility).GetMethod("HeadLightUtilityUse", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy));
                        HarmonyMethod unequipHelmetLamp = new HarmonyMethod(typeof(AdvancedCompanyCompatibility).GetMethod("UnequipHeadLightUtility", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy));

                        harmony.Patch(useHelmetLight, helmetLampUse);
                        harmony.Patch(unequip, unequipHelmetLamp);

                    }
                    if (types[i].Name == "TacticalHelmet" && types[i].Namespace == "AdvancedCompany.Objects")
                    {
                        InsanityRemasteredLogger.Log("TacticalHelmet object found, starting fix.");
                        MethodInfo useHelmetLight = types[i].GetMethod("SwitchFlashlight", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                        MethodInfo unequip = types[i].GetMethod("Unequipped", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                        HarmonyMethod helmetLampUse = new HarmonyMethod(typeof(AdvancedCompanyCompatibility).GetMethod("HeadLightUtilityUse", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy));
                        HarmonyMethod unequipHelmetLamp = new HarmonyMethod(typeof(AdvancedCompanyCompatibility).GetMethod("UnequipHeadLightUtility", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy));

                        harmony.Patch(useHelmetLight, helmetLampUse);
                        harmony.Patch(unequip, unequipHelmetLamp);

                    }
                }
            }
        }

    }
}
