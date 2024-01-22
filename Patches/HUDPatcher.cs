using BepInEx;
using HarmonyLib;
using InsanityRemastered.General;
using InsanityRemastered.Patches;
using InsanityRemastered.Utilities;
using UnityEngine;

namespace InsanityRemasteredMod.Patches
{
    [HarmonyPatch]
    internal class HUDPatcher
    {
        private static bool hudOn;
        private static bool alreadyWarned;
        private static void ResetWarningFacility(bool outside)
        {
            if (outside)
            {
                alreadyWarned = false;
            }
        }
        private static void ResetWarning()
        {

        }
        [HarmonyPatch(typeof(HUDManager), "Awake")]
        [HarmonyPostfix]
        private static void _Awake()
        {
            HallucinationManager.OnUIHallucination += PlayUISFX;
            GameEvents.OnEnterOrLeaveFacility += ResetWarningFacility;
            GameEvents.OnGameEnd += ResetWarning;
            GameEvents.OnPlayerDied += ResetWarning;
            
        }
        [HarmonyPatch(typeof(HUDManager), "Update")]
        [HarmonyPostfix]
        private static void _Update()
        {
            if (UnityInput.Current.GetKeyDown(KeyCode.Alpha0))
            {
                ToggleHUD();
            }
            if (!alreadyWarned && PlayerPatcher.CurrentSanityLevel >= SanityLevel.Medium)
            {
                HUDManager.Instance.DisplayTip("WARNING!", "Heartrate level is above normal. Please exercise caution.", true);
                alreadyWarned = true;
            }
            if (!alreadyWarned && PlayerPatcher.CurrentSanityLevel >= SanityLevel.High)
            {
                HUDManager.Instance.DisplayTip("WARNING!", "Heartrate is . Please exercise caution.", true);
                alreadyWarned = true;
            }
        }

        private static void PlayUISFX()
        {
            InsanitySoundManager.Instance.PlayUISound(HUDManager.Instance.warningSFX[Random.Range(0, HUDManager.Instance.warningSFX.Length)]);
        }
        private static void ToggleHUD()
        {
            if (!hudOn)
            {
                HUDManager.Instance.HideHUD(true);
                hudOn = true;
            }
            else if (hudOn)
            {
                HUDManager.Instance.HideHUD(false);
                hudOn = false;
            }
        }
    }
}
