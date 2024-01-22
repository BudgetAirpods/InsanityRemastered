using GameNetcodeStuff;
using InsanityRemastered.General;
using UnityEngine;

namespace InsanityRemasteredMod.General
{
    internal class HallucinationEffects
    {
        private static float PanicLevel => HallucinationManager.Instance.PanicAttackLevel;
        private static float EffectTransitionTime => HallucinationManager.Instance.EffectTransition;
        private static PlayerControllerB localPlayer => GameNetworkManager.Instance.localPlayerController;
        public static void LessenPanicEffects()
        {
            if (GameNetworkManager.Instance.gameHasStarted)
            {
                if (!localPlayer.isInsideFactory && PanicLevel >= 0 || localPlayer.isInsideFactory && !localPlayer.isPlayerAlone && PanicLevel >= 0)
                {
                    if (!InsanityRemasteredConfiguration.disablePanicAttackEffects)
                    {
                        HUDManager.Instance.insanityScreenFilter.weight = Mathf.MoveTowards(HUDManager.Instance.insanityScreenFilter.weight, 0f, EffectTransitionTime - 100f * Time.deltaTime);
                        SoundManager.Instance.SetDiageticMixerSnapshot(0, EffectTransitionTime - 100);
                    }
                }
            }
        }
        public static void IncreasePanicEffects()
        {
            if (!InsanityRemasteredConfiguration.disablePanicAttackEffects)
            {
                HUDManager.Instance.insanityScreenFilter.weight = Mathf.MoveTowards(HUDManager.Instance.insanityScreenFilter.weight, 0.5f, EffectTransitionTime * Time.deltaTime);
                SoundManager.Instance.SetDiageticMixerSnapshot(1, EffectTransitionTime);
            }
        }

    }
}
