using HarmonyLib;
using InsanityRemastered.General;
using InsanityRemastered.ModIntegration;
using UnityEngine;
namespace InsanityRemastered.Patches
{
    [HarmonyPatch(typeof(FlashlightItem))]
    internal class ItemPatcher
    {

        private static float walkieRNGFrequency = 35f;
        private static float walkieRNGTimer;

        [HarmonyPatch(typeof(FlashlightItem)), HarmonyPatch("SwitchFlashlight")]
        [HarmonyPostfix]
        private static void OnUse(bool on)
        {
            if (on)
            {
                PlayerPatcher.FlashlightOn = true;
            }
            else if (!on)
            {
                PlayerPatcher.FlashlightOn = false;
            }
        }
        [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.EquipItem))]
        [HarmonyPostfix]
        private static void SetControlTipForPills(ref GrabbableObject __instance)
        {
            if (__instance.itemProperties.name == "PillBottle")
            {
                HUDManager.Instance.ChangeControlTip(1, "Consume pills: [LMB]");
            }
        }
        [HarmonyPatch(typeof(WalkieTalkie), "Update")]
        [HarmonyPostfix]
        private static void WalkieEffects(ref WalkieTalkie __instance)
        {
            if (GameNetworkManager.Instance.gameHasStarted)
            {
                walkieRNGTimer += Time.deltaTime;
                if (walkieRNGTimer > walkieRNGFrequency && __instance.isBeingUsed)
                {
                    walkieRNGTimer = 0;
                    float rng = Random.Range(0f, 1f);
                    if (rng <= 0.35f && SkinwalkerModIntegration.IsInstalled && InsanityGameManager.AreThereOtherPlayers)
                    {
                        __instance.thisAudio.PlayOneShot(SkinwalkerModIntegration.GetRandomClip());
                    }
                }
            }
        }
    }
}
