using DunGen.Graph;
using SanityRewrittenMod.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Windows;

namespace SanityRewrittenMod.Hallucinations
{
    internal class BunkerHallucinations
    {
        /// <summary>
        /// When the power hallucination is used. Invoked with a bool that determines whether the power is being turned on or off.
        /// </summary>
        public static event Action<bool> OnPowerHallucination;
        /// <summary>
        /// When the fake player hallucination is starting.
        /// </summary>
        public static event Action OnPlayerHallucinationStarted;
        /// <summary>
        /// When a fake item is spawned into the world.
        /// </summary>
        public static event Action OnSpawnFakeItem;
        /// <summary>
        /// When a random sound from the mod is played.
        /// </summary>
        public static event Action OnSoundPlayed;
        /// <summary>
        /// Toggles lights inside the map, sets the fear level, and plays a sound effect.
        /// </summary>
        public static void LightHallucination()
        {
            if (InsanityRemasteredBase.powerLossEventEnabled)
            {
                if (InsanityGameManager.MapFlow.name == "Level2Flow")
                {
                    return;
                }
                if (!InsanityGameManager.Instance.LightsOff)
                {
                    foreach (Light light in InsanityGameManager.Instance.BunkerLights)
                    {
                        light.enabled = false;
                        PlayerPatcher.LocalPlayer.JumpToFearLevel(0.8f);
                    }
                    OnPowerHallucination?.Invoke(false);
                    InsanitySoundManager.Instance.PlayHallucinationSound("Hallucination_Bunker_06", true);
                }
                else if (InsanityGameManager.Instance.LightsOff)
                {
                    foreach (Light light in InsanityGameManager.Instance.BunkerLights)
                    {
                        light.enabled = true;
                    }
                    OnPowerHallucination?.Invoke(true);
                }
            }
        }
        /// <summary>
        /// Spawns a fake player that can chase or wander the facility. Can be lethal.
        /// </summary>
        public static void PlayerModelHallucination(GameObject model)
        {
            if (InsanityRemasteredBase.modelHallucinationsEnabled)
            {
                if (!model.activeInHierarchy)
                {
                    model.SetActive(true);
                }
                OnPlayerHallucinationStarted?.Invoke();
            }
        }
        /// <summary>
        /// Plays a random sound from the mod.
        /// </summary>
        public static void PlaySound()
        {
            if (InsanityRemasteredBase.auditoryHallucinationsEnabled) 
            { 
                InsanitySoundManager.Instance.PlayHallucinationSound();
            }
        }
        /// <summary>
        /// Spawns a fake scrap item that invokes an event when picked up.
        /// </summary>
        public static void SpawnFakeObject()
        {
            if (InsanityRemasteredBase.fakeItemsEnabled)
            {
                Vector3 position;
                SpawnableItemWithRarity fakeScrap = RoundManager.Instance.currentLevel.spawnableScrap[UnityEngine.Random.Range(0, RoundManager.Instance.currentLevel.spawnableScrap.Count)];
                position = RoundManager.Instance.GetRandomNavMeshPositionInRadiusSpherical(PlayerPatcher.LocalPlayer.transform.position, 10) + Vector3.up * fakeScrap.spawnableItem.verticalOffset;
                GameObject itemObject = UnityEngine.GameObject.Instantiate(fakeScrap.spawnableItem.spawnPrefab, position, Quaternion.identity);
                GrabbableObject scrap = itemObject.GetComponent<GrabbableObject>();
                scrap.SetScrapValue(UnityEngine.Random.Range(fakeScrap.spawnableItem.minValue, fakeScrap.spawnableItem.maxValue + 50));
                itemObject.AddComponent<FakeItem>();
                OnSpawnFakeItem?.Invoke();
            }
        }
    }
}
