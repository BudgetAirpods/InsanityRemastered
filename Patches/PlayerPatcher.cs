using BepInEx;
using GameNetcodeStuff;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System;
using SanityRewrittenMod.Hallucinations;
using UnityEngine.Windows;
using SanityRewrittenMod.Utilities;
using SanityRewrittenMod.General;
using InsanityRemasteredMod.General;

namespace SanityRewrittenMod.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerPatcher : BaseUnityPlugin
    {
        public static SanityLevel CurrentSanityLevel;
        /// <summary>
        /// When the player interacts with a fake item.
        /// </summary>
        public static event Action OnInteractWithFakeItem;
        public static int PlayersConnected;
        public static PlayerControllerB LocalPlayer => GameNetworkManager.Instance.localPlayerController;

        public static bool FlashlightOn { get; set; }
        private static bool HoldingPills { get; set; }

        private static GrabbableObject heldItem;
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void _Awake()
        {
            InsanityRemastered_AI.OnHallucinationEnded += LoseSanity;
            GameEvents.OnItemSwitch += OnItemSwitch;
        }
        [HarmonyPatch(typeof(PlayerControllerB), "SetPlayerSanityLevel")]
        [HarmonyPrefix]
        private static bool PlayerInsanityPatch()
        {
            if (GameNetworkManager.Instance.gameHasStarted)
            {
                if (StartOfRound.Instance.inShipPhase || !TimeOfDay.Instance.currentDayTimeStarted)
                {
                    LocalPlayer.insanityLevel = 0f;
                    return false;
                }
                //If the player is inside the factory and not near other players.
                if (LocalPlayer.isInsideFactory)
                {
                    if (!NearOtherPlayers())
                    {
                        LocalPlayer.insanitySpeedMultiplier = InsanityRemasteredBase.lossWhenNotNearOthers;
                        LocalPlayer.isPlayerAlone = true;
                        if (SanityMainManager.Instance.LightsOff)
                        {
                            LocalPlayer.insanitySpeedMultiplier = InsanityRemasteredBase.lossWhenLightsAreOut;
                        }
                        else if (HallucinationManager.Instance.PanicAttack)
                        {
                            LocalPlayer.insanitySpeedMultiplier = InsanityRemasteredBase.lossWhenPanicking;
                        }
                        if (FlashlightOn)
                        {
                            LocalPlayer.insanitySpeedMultiplier /= 2;
                        }
                    }
                    
                    else if (NearOtherPlayers())
                    {
                        LocalPlayer.insanitySpeedMultiplier = -InsanityRemasteredBase.sanityGainWhenNearOthers;
                        LocalPlayer.isPlayerAlone = false;
                    }
                }
                if (!LocalPlayer.isInsideFactory)
                {
                    if (TimeOfDay.Instance.dayMode >= DayMode.Sundown && !FlashlightOn)
                    {
                        LocalPlayer.insanitySpeedMultiplier = 0.5f;
                    }
                    if (TimeOfDay.Instance.dayMode <= DayMode.Noon)
                    {
                        LocalPlayer.insanitySpeedMultiplier = -0.1f;
                    }
                }
                if(StartOfRound.Instance.connectedPlayersAmount == 0)
                {
                    LocalPlayer.insanitySpeedMultiplier *= InsanityRemasteredBase.insanitySpeedScalingForSolo;
                }
                if (LocalPlayer.isInHangarShipRoom  || LocalPlayer.isInElevator)
                {
                    LocalPlayer.insanitySpeedMultiplier = InsanityRemasteredBase.sanityGainWhenInsideShip;
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
            if (GameNetworkManager.Instance.gameHasStarted && LocalPlayer.isPlayerControlled && !LocalPlayer.isPlayerDead)
            {
                UpdateStatusEffects();
                if(LocalPlayer.insanityLevel == LocalPlayer.maxInsanityLevel)
                {
                    CurrentSanityLevel = SanityLevel.Max;
                    return;
                }
                if (LocalPlayer.insanityLevel >= 100)
                {
                    CurrentSanityLevel = SanityLevel.High;
                    return;
                }
                else if (LocalPlayer.insanityLevel >= 40)
                {
                    CurrentSanityLevel = SanityLevel.Medium;
                    return;
                }
                else
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
            if (HallucinationManager.reduceStamina)
            {
                LocalPlayer.sprintMeter = 25f;
            }
            if (HallucinationManager.reduceVision)
            {
                HUDManager.Instance.increaseHelmetCondensation = true;
            }
        }
        private static bool NearOtherPlayers(PlayerControllerB playerScript = null, float checkRadius = 10f)
        {
            if (playerScript == null)
            {
                playerScript = LocalPlayer;
            }
            LocalPlayer.gameObject.layer = 0;
            bool result = Physics.CheckSphere(playerScript.transform.position, checkRadius, 8, QueryTriggerInteraction.Ignore);
            LocalPlayer.gameObject.layer = 3;
            return result;
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
