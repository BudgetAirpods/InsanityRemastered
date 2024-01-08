using BepInEx;
using GameNetcodeStuff;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System;
using SanityRewrittenMod.Hallucinations;
using UnityEngine.Windows;

namespace SanityRewrittenMod.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerPatcher : BaseUnityPlugin
    {
        public static PlayerControllerB LocalPlayer => GameNetworkManager.Instance.localPlayerController;

        public static bool FlashlightOn { get; set; }
        public static bool test = false;
        [HarmonyPatch(typeof(PlayerControllerB), "Start")]
        [HarmonyPostfix]
        private static void _Start(PlayerControllerB __instance)
        {
            print("Player controller patched");
            __instance.insanityLevel = 60;
            __instance.maxInsanityLevel = 200;
            __instance.insanitySpeedMultiplier = 90f;
        }
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void _Update()
        {
            if (test)
            {
                LocalPlayer.movementSpeed = 1;
            }
        }
    }

    [HarmonyPatch(typeof(RoundManager))]
    internal class RoundManagerPatcher : BaseUnityPlugin
    {
        public static event Action OnGameStart;
        public static event Action OnGameEnd;
        [HarmonyPatch("GenerateNewLevelClientRpc")]
        [HarmonyPostfix]
        static void GameStart()
        {
            OnGameStart?.Invoke();
        }
        [HarmonyPatch("DespawnPropsAtEndOfRound")]
        [HarmonyPostfix]
        static void GameEnd()
        {
            OnGameEnd?.Invoke();
        }
    }
}
