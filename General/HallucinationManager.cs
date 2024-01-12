using BepInEx;
using GameNetcodeStuff;
using SanityRewrittenMod;
using SanityRewrittenMod.Hallucinations;
using SanityRewrittenMod.Patches;
using SanityRewrittenMod.Utilities;
using System;
using UnityEngine;

namespace InsanityRemasteredMod.General
{
    internal class HallucinationManager : MonoBehaviour
    {
        public static event Action OnSanityRecovered;
        private PlayerControllerB localPlayer => GameNetworkManager.Instance.localPlayerController;
        public static HallucinationManager Instance;
        // Level1Flow is Bunker.
        // Level2Flow is Mansion.
        private float droneRNGTimer;
        private float droneRNGFrequency = 60f;
        private float sanityRNGTimer;
        private float sanityRNGFrequency;
        private float slowdownTimer = 120f;
        private float panicAttackLevel;

        private bool panicAttack = false;
        public static bool slowness = false;
        public static bool reduceVision = false;

        public bool LightsOff { get; private set; }
        public bool PanicAttack { get { return panicAttack; } set { panicAttack = value; } }
        public float PanicAttackLevel { get { return panicAttackLevel; } set { panicAttackLevel = value; } }
        public float PanicAttackTimer { get { return slowdownTimer; } }

        private void Start()
        {
            sanityRNGFrequency = InsanityRemasteredBase.timeBetweenEachHallucinationRNGCheck * InsanityRemasteredBase.rngCheckTimerMultiplier;
        }
        private void Awake()
        {
            Instance = this;
            PlayerPatcher.OnInteractWithFakeItem += InteractWithFakeItem;
            GameEvents.OnEnterOrLeaveFacility += OnEnterOrLeaveFacility;
            BunkerHallucinations.OnSoundPlayed += OnHeardHallucinationSound;
        }

        private void OnHeardHallucinationSound()
        {
            if (PlayerPatcher.CurrentSanityLevel >= SanityLevel.Medium)
            {
                localPlayer.insanityLevel += 15f;
            }
        }
        private void OnEnterOrLeaveFacility(bool outside)
        {
            if (outside && InsanityGameManager.Instance.LightsOff)
            {
                ResetLights();
            }
        }

        private void InteractWithFakeItem()
        {

        }
        private void Update()
        {
            if (localPlayer.isPlayerControlled && !localPlayer.isPlayerDead)
            {
                if (localPlayer.isInsideFactory && localPlayer.isPlayerControlled && !localPlayer.isPlayerDead)
                {
                    if ((int)PlayerPatcher.CurrentSanityLevel >= 1)
                    {
                        sanityRNGTimer += Time.deltaTime;
                        if (sanityRNGTimer > sanityRNGFrequency)
                        {
                            sanityRNGTimer = 0;
                            PickHallucination();
                        }
                    }
                    if (PlayerPatcher.CurrentSanityLevel == SanityLevel.Max)
                    {
                        droneRNGTimer += Time.deltaTime;
                        if (droneRNGTimer > droneRNGFrequency)
                        {
                            droneRNGTimer = 0;
                            InsanitySoundManager.Instance.PlayDrone();
                            if (panicAttackLevel == 1 && InsanityRemasteredBase.panicAttacksEnabled)
                            {
                                PanicAttackSymptom();
                            }
                        }
                        if (localPlayer.isInsideFactory && localPlayer.isPlayerAlone)
                        {
                            HighSanityEffects();
                        }
                        else
                        {
                            LessenPanicEffects();
                        }
                    }
                }
            }
            if (panicAttackLevel >= 0)
            {
                LessenPanicEffects();
            }
            if (!GameNetworkManager.Instance.gameHasStarted)
            {
                ResetPanicValues();
            }
        }
        private void LessenPanicEffects()
        {
            if (PanicAttackLevel <= 0f)
            {
                ResetPanicValues();
                return;
            }
            if (GameNetworkManager.Instance.gameHasStarted)
            {
                if (!localPlayer.isInsideFactory && panicAttackLevel >= 0 || localPlayer.isInsideFactory && !localPlayer.isPlayerAlone && panicAttackLevel >= 0)
                {
                    panicAttackLevel = Mathf.MoveTowards(panicAttackLevel, 0, 0.5f*Time.deltaTime);
                    if (!InsanityRemasteredBase.disablePanicAttackEffects)
                    {
                        HUDManager.Instance.insanityScreenFilter.weight = Mathf.MoveTowards(HUDManager.Instance.insanityScreenFilter.weight, 0f, slowdownTimer - 100f *Time.deltaTime);
                        SoundManager.Instance.SetDiageticMixerSnapshot(0, slowdownTimer - 100);
                    }
                }
            }
        }
        private void HighSanityEffects()
        {
            panicAttackLevel = Mathf.MoveTowards(panicAttackLevel, 1,  slowdownTimer*Time.deltaTime);
            if (!InsanityRemasteredBase.disablePanicAttackEffects)
            {
                HUDManager.Instance.insanityScreenFilter.weight = Mathf.MoveTowards(HUDManager.Instance.insanityScreenFilter.weight, 0.5f, slowdownTimer * Time.deltaTime);
                SoundManager.Instance.SetDiageticMixerSnapshot(1, slowdownTimer);
            }
        }
        public void Debug_SpawnItem(string itemName)
        {
            // Get a list of all items
            foreach(Item item in StartOfRound.Instance.allItemsList.itemsList)
            {
                if (item.name == itemName)
                {
                    var prop = UnityEngine.Object.Instantiate(
                    item.spawnPrefab,
                    localPlayer.transform.position + localPlayer.transform.forward * 2.0f,
                    localPlayer.transform.rotation,
                    RoundManager.Instance.spawnedScrapContainer
                );
                    var grabbable = prop.GetComponent<GrabbableObject>();
                    grabbable.fallTime = 1.0f;
                    grabbable.scrapPersistedThroughRounds = false;
                    grabbable.grabbable = true;

                    // Check if it's scrap
                    if (item.isScrap)
                    {
                        // Set the scrap value
                        grabbable.SetScrapValue(
                            UnityEngine.Random.Range(item.minValue, item.maxValue)
                        );
                        grabbable.NetworkObject.Spawn(false);
                    }
                }
            }           
        }
        public void ResetPanicValues()
        {

            SoundManager.Instance.SetDiageticMixerSnapshot();
            panicAttack = false;
            slowness = false;
            reduceVision = false;
            OnSanityRecovered?.Invoke();
        }
        public void PanicAttackSymptom()
        {
            panicAttack = true;
            localPlayer.JumpToFearLevel(0.55f);
            RandomIntenseHallucination();
            int symptom = UnityEngine.Random.Range(0, 2);
            switch (symptom)
            {
                case 0:
                    slowness = true;
                    break;
                case 1:
                    reduceVision = true;
                    break;
                case 2:
                    if (InsanityRemasteredBase.enableChanceToDieDuringPanicAttack)
                    {
                        localPlayer.KillPlayer(Vector3.zero);
                    }
                    else
                    {
                        slowness = true;
                    }
                    break;
            }
        }
        /// <summary>
        /// Picks a random hallucination based off the player's sanity.
        /// </summary>
        public void PickHallucination()
        {
            int hallucinationType = UnityEngine.Random.Range(0, 2);
            if (hallucinationType == 0)
            {
                RandomSmallHallucination();
                return;
            }
            else if (hallucinationType == 1)
            {
                RandomMildHallucination();
                return;
            }
            else if (hallucinationType == 2 && PlayerPatcher.CurrentSanityLevel == SanityLevel.High)
            {
                float tryForLesserHallucination = UnityEngine.Random.Range(0f, 1f);
                if (tryForLesserHallucination >= 0.35)
                {
                    RandomMildHallucination();
                }
                RandomIntenseHallucination();
                return;
            }
        }
        public void RandomSmallHallucination()
        {
            int hallucinationRNG = UnityEngine.Random.Range(0, 1);
            if (hallucinationRNG == 0f)
            {
                BunkerHallucinations.SpawnFakeObject();
                return;
            }
            else
            {
                BunkerHallucinations.PlaySound();
                return;
            }
        }
        public void RandomMildHallucination()
        {
            int hallucinationRNG = UnityEngine.Random.Range(0, 1);
            if (hallucinationRNG == 0)
            {
                BunkerHallucinations.PlaySound();
                return;
            }
            else if (hallucinationRNG == 1)
            {
                BunkerHallucinations.SpawnFakeObject();
                return;
            }
        }
        public void RandomIntenseHallucination()
        {
            int hallucinationRNG = UnityEngine.Random.Range(0, 1);
            if (hallucinationRNG == 0f)
            {
                BunkerHallucinations.PlayerModelHallucination(InsanityGameManager.Instance.currentHallucinationModel);
                return;
            }
            else if (hallucinationRNG == 1)
            {
                BunkerHallucinations.LightHallucination();
                return;
            }

        }
        public void ResetLights()
        {
            foreach (Light light in InsanityGameManager.Instance.BunkerLights)
            {
                light.enabled = true;
            }
        }
    }
}