using GameNetcodeStuff;
using HarmonyLib;
using System;

namespace InsanityRemastered.Utilities
{
    [HarmonyPatch]
    internal class GameEvents
    {
        /// <summary>
        /// Called when the player enters or leaves the facility. 
        /// </summary>
        public static event Action<bool> OnEnterOrLeaveFacility;
        /// <summary>
        /// Called when the game starts generating a level.
        /// </summary>
        public static event Action OnGameStart;
        /// <summary>
        /// Called when the game finishes generating the level.
        /// </summary>
        public static event Action OnLateGameStart;
        /// <summary>
        /// Called when the game ends.
        /// </summary>
        public static event Action OnGameEnd;
        /// <summary>
        /// Called when an enemy spawns on the map.
        /// </summary>
        public static event Action OnEnemySpawned;
        /// <summary>
        /// Fires when the ship has landed.
        /// </summary>
        public static event Action OnShipLanded;
        public static event Action<GrabbableObject> OnItemSwitch;
        public static event Action OnPlayerJoin;
        public static event Action OnPlayerLeave;
        public static event Action OnPlayerDied;
        public static event Action<int> OnTakeDamage;
        public static event Action<EnemyAI> OnKillEnemy;

        [HarmonyPatch(typeof(EntranceTeleport), nameof(EntranceTeleport.TeleportPlayer))]
        [HarmonyPrefix]
        static void OnEnterLeaveFacility(EntranceTeleport __instance)
        {
            OnEnterOrLeaveFacility?.Invoke((__instance.isEntranceToBuilding ? true : false));
        }

        [HarmonyPatch(typeof(RoundManager), "GenerateNewLevelClientRpc")]
        [HarmonyPostfix]
        static void GameStart()
        {
            OnGameStart?.Invoke();
        }
        [HarmonyPatch(typeof(RoundManager), "FinishGeneratingNewLevelClientRpc")]
        [HarmonyPostfix]
        static void LateGameStart()
        {
            OnLateGameStart?.Invoke();
        }
        [HarmonyPatch(typeof(RoundManager), "DespawnPropsAtEndOfRound")]
        [HarmonyPostfix]
        static void GameEnd()
        {
            OnGameEnd?.Invoke();
        }
        [HarmonyPatch(typeof(StartOfRound), "OnShipLandedMiscEvents")]
        [HarmonyPostfix]
        static void ShipLanded()
        {
            OnShipLanded?.Invoke();
        }
        [HarmonyPatch(typeof(RoundManager), "SpawnEnemyGameObject")]
        [HarmonyPostfix]
        static void SpawnEnemy()
        {
            OnEnemySpawned?.Invoke();
        }

        [HarmonyPatch(typeof(StartOfRound), "OnClientDisconnect")]
        [HarmonyPostfix]
        static void PlayerLeave()
        {
            OnPlayerLeave?.Invoke();
        }
        [HarmonyPatch(typeof(PlayerControllerB), "SwitchToItemSlot")]
        [HarmonyPostfix]
        static void SwitchItem(ref GrabbableObject ___currentlyHeldObjectServer)
        {
            if (___currentlyHeldObjectServer is object)
            {
                //print(___currentlyHeldObjectServer.itemProperties.name);
                OnItemSwitch?.Invoke(___currentlyHeldObjectServer);
            }
        }
        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.KillPlayer))]
        [HarmonyPostfix]
        static void OnPlayerDeath(ref PlayerControllerB __instance)
        {

            OnPlayerDied?.Invoke();
        }
        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DamagePlayer))]
        [HarmonyPostfix]
        static void TakeDamage(int damageNumber)
        {
            OnTakeDamage?.Invoke(damageNumber);
        }
        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.KillEnemy))]
        [HarmonyPostfix]
        static void KillEnemy(EnemyAI __instance)
        {
            //print(__instance.enemyType.enemyName);
            OnKillEnemy?.Invoke(__instance); ;
        }
    }
}
