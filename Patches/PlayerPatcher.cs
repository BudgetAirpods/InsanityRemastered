using GameNetcodeStuff;
using HarmonyLib;
using InsanityRemastered.General;
using InsanityRemastered.Hallucinations;
using InsanityRemastered.ModIntegration;
using InsanityRemastered.Utilities;
using InsanityRemasteredMod.General;
using System;
using UnityEngine;

namespace InsanityRemastered.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerPatcher
    {
        public static SanityLevel CurrentSanityLevel;
        /// <summary>
        /// When the player interacts with a fake item.
        /// </summary>
        public static event Action OnInteractWithFakeItem;
        public static int PlayersConnected;

        public static PlayerControllerB LocalPlayer => GameNetworkManager.Instance.localPlayerController;

        public static float InsanityLevel;
        public static bool FlashlightOn { get; set; }
        private static bool HoldingPills { get; set; }

        private static GrabbableObject heldItem;
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void _Awake(ref PlayerControllerB __instance)
        {
            //__instance.maxInsanityLevel = 200;
            InsanityRemastered_AI.OnHallucinationEnded += LoseSanity;
            GameEvents.OnItemSwitch += OnItemSwitch;
        }


        [HarmonyPatch(typeof(PlayerControllerB), "SetPlayerSanityLevel")]
        [HarmonyPrefix]
        private static bool PlayerInsanityPatch()
        {
            if (GameNetworkManager.Instance.gameHasStarted)
            {
                InsanityLevel = LocalPlayer.insanityLevel;
                if (StartOfRound.Instance.inShipPhase || !TimeOfDay.Instance.currentDayTimeStarted)
                {
                    LocalPlayer.insanityLevel = 0f;
                    return false;
                }

                if (LocalPlayer.isInsideFactory)
                {
                    if (PlayersConnected > 1)
                    {
                        MultiplayerInsanity();
                    }
                    else if (PlayersConnected == 1)
                    {
                        SoloInsanity();
                    }
                }
                else
                {
                    if (TimeOfDay.Instance.dayMode <= DayMode.Noon)
                    {
                        LocalPlayer.insanitySpeedMultiplier = -0.2f;
                    }
                }
                if (InsanityGameManager.Instance.IsNearLightSource)
                {
                    LocalPlayer.insanitySpeedMultiplier = -0.07f;
                }

                if (FlashlightOn || AdvancedCompanyCompatibility.nightVision || (InsanityGameManager.Instance.IsPlayerTalking && LocalPlayer.isPlayerAlone))
                {
                    LocalPlayer.insanitySpeedMultiplier /= InsanityRemasteredConfiguration.sanityLossReductionWhenUsingFlashlights * Mathf.Log(Mathf.Sqrt(PlayersConnected));
                }
                if (LocalPlayer.isInHangarShipRoom)
                {
                    LocalPlayer.insanitySpeedMultiplier = InsanityRemasteredConfiguration.sanityGainWhenInsideShip;
                }
                if (LocalPlayer.insanitySpeedMultiplier < 0f)
                {
                    LocalPlayer.insanityLevel = Mathf.MoveTowards(LocalPlayer.insanityLevel, 0f, Time.deltaTime * (0f - LocalPlayer.insanitySpeedMultiplier));
                    return false;
                }
                if (LocalPlayer.insanityLevel > LocalPlayer.maxInsanityLevel)
                {
                    LocalPlayer.insanityLevel = Mathf.MoveTowards(LocalPlayer.insanityLevel, LocalPlayer.maxInsanityLevel, Time.deltaTime * 2f);
                    return false;
                }
                LocalPlayer.insanityLevel = Mathf.MoveTowards(LocalPlayer.insanityLevel, LocalPlayer.maxInsanityLevel, Time.deltaTime * LocalPlayer.insanitySpeedMultiplier);
                return false;
            }
            return false;
        }
        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyPostfix]
        private static void _Update()
        {
            PlayersConnected = StartOfRound.Instance.connectedPlayersAmount + 1;
            PlayersConnected = Mathf.Clamp(PlayersConnected, 1, InsanityRemasteredConfiguration.maxPlayerCountForScalingInsanitySpeed);
            if (GameNetworkManager.Instance.gameHasStarted && LocalPlayer.isPlayerControlled && !LocalPlayer.isPlayerDead)
            {
                UpdateStatusEffects();
                if (HallucinationManager.Instance.PanicAttackLevel >= 1f)
                {
                    CurrentSanityLevel = SanityLevel.Max;
                    return;
                }
                else if (LocalPlayer.insanityLevel >= 100)
                {
                    CurrentSanityLevel = SanityLevel.High;
                    return;
                }
                else if (LocalPlayer.insanityLevel >= 50)
                {
                    CurrentSanityLevel = SanityLevel.Medium;
                    return;
                }
                else if (LocalPlayer.insanityLevel <= 49)
                {
                    CurrentSanityLevel = SanityLevel.Low;
                    return;
                }
            }
        }

        private static void UpdateStatusEffects()
        {
            if (HallucinationManager.slowness)
            {
                LocalPlayer.movementSpeed = Mathf.MoveTowards(LocalPlayer.movementSpeed, 2.3f, 5f * Time.deltaTime);
            }
            else if (!HallucinationManager.slowness && !LocalPlayer.isSprinting && !LocalPlayer.isCrouching)
            {
                LocalPlayer.movementSpeed = Mathf.MoveTowards(LocalPlayer.movementSpeed, 4.6f, 5f * Time.deltaTime);
            }
            if (HallucinationManager.reduceVision)
            {
                HUDManager.Instance.increaseHelmetCondensation = true;

            }
        }

        private static void SoloInsanity()
        {
            LocalPlayer.insanitySpeedMultiplier = InsanityRemasteredConfiguration.lossWhenNotNearOthers;

            if (InsanityGameManager.Instance.LightsOff)
            {
                LocalPlayer.insanitySpeedMultiplier = InsanityRemasteredConfiguration.lossWhenLightsAreOut;
            }
            else if (HallucinationManager.Instance.PanicAttackLevel > 0)
            {
                LocalPlayer.insanitySpeedMultiplier = InsanityRemasteredConfiguration.lossWhenPanicking;
            }

            LocalPlayer.insanitySpeedMultiplier *= InsanityRemasteredConfiguration.insanitySpeedScalingForSolo;

            LocalPlayer.isPlayerAlone = true;
        }
        private static void MultiplayerInsanity()
        {
            if (!InsanityGameManager.Instance.IsNearPlayers)
            {
                LocalPlayer.insanitySpeedMultiplier = InsanityRemasteredConfiguration.lossWhenNotNearOthers * Mathf.Log(Mathf.Sqrt(PlayersConnected));

                if (InsanityGameManager.Instance.LightsOff)
                {
                    LocalPlayer.insanitySpeedMultiplier = InsanityRemasteredConfiguration.lossWhenLightsAreOut * Mathf.Log(Mathf.Sqrt(PlayersConnected));
                }
                else if (HallucinationManager.Instance.PanicAttackLevel > 0)
                {
                    LocalPlayer.insanitySpeedMultiplier = InsanityRemasteredConfiguration.lossWhenPanicking * Mathf.Log(Mathf.Sqrt(PlayersConnected));
                }

                LocalPlayer.isPlayerAlone = true;
            }
            else if (InsanityGameManager.Instance.IsNearPlayers && !InsanityGameManager.Instance.IsNearLightSource)
            {
                LocalPlayer.insanitySpeedMultiplier *= -InsanityRemasteredConfiguration.sanityGainWhenNearOthers * Mathf.Log(Mathf.Sqrt(PlayersConnected));
                LocalPlayer.isPlayerAlone = false;
            }

        }


        private static void OnItemSwitch(GrabbableObject item)
        {
            if (item.itemProperties.name == "PillBottle")
            {
                heldItem = item;
                HoldingPills = true;
            }
            else
            {
                HoldingPills = false;
            }
        }
        private static void LoseSanity(bool touched)
        {
            if (touched)
            {
                LocalPlayer.insanityLevel += 15;
                LocalPlayer.JumpToFearLevel(1);
            }
        }
        private static void OnHeardHallucinationSound()
        {
            if (CurrentSanityLevel >= SanityLevel.Medium)
            {
                LocalPlayer.insanityLevel += 5f;
            }
        }
        [HarmonyPatch(typeof(PlayerControllerB), "ActivateItem_performed")]
        [HarmonyPostfix]
        private static void _UseItem(PlayerControllerB __instance)
        {
            if (HoldingPills)
            {
                heldItem.DestroyObjectInHand(LocalPlayer);
                HoldingPills = false;
                LocalPlayer.insanityLevel = 0;
                SoundManager.Instance.SetDiageticMixerSnapshot();
            }
        }
        [HarmonyPatch(typeof(PlayerControllerB), "Interact_performed")]
        [HarmonyPostfix]
        private static void InteractPatch(PlayerControllerB __instance)
        {
            Ray interactRay = new Ray(LocalPlayer.gameplayCamera.transform.position, __instance.gameplayCamera.transform.forward);
            RaycastHit hit;
            if (!Physics.Raycast(interactRay, out hit, __instance.grabDistance, 832) || hit.collider.gameObject.layer == 8 || !(hit.collider.tag == "PhysicsProp") || LocalPlayer.twoHanded || __instance.sinkingValue > 0.73f)
            {
                return;
            }
            FakeItem fakeItem = hit.collider.transform.gameObject.GetComponent<FakeItem>();
            if (fakeItem)
            {
                LocalPlayer.insanityLevel += 14;
                LocalPlayer.JumpToFearLevel(0.4f);

                OnInteractWithFakeItem?.Invoke();
            }
        }
    }
}
