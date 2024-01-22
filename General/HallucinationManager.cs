using GameNetcodeStuff;
using InsanityRemastered.Hallucinations;
using InsanityRemastered.Patches;
using InsanityRemasteredMod;
using InsanityRemasteredMod.General;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InsanityRemastered.General
{
    internal class HallucinationManager : MonoBehaviour
    {
        public static event Action<bool> OnPowerHallucination;
        public static event Action OnPlayerHallucinationStarted;
        public static event Action OnSpawnFakeItem;
        public static event Action OnSoundPlayed;
        public static event Action OnSanityRecovered;
        public static event Action OnExperiencePanicAttack;
        public static event Action OnUIHallucination;

        private PlayerControllerB localPlayer => GameNetworkManager.Instance.localPlayerController;
        private SanityLevel SanityLevel => PlayerPatcher.CurrentSanityLevel;

        public static HallucinationManager Instance;

        private float droneRNGTimer;
        private float droneRNGFrequency = 60f;
        private float sanityRNGTimer;
        private float sanityRNGFrequency = 40f;
        private float slowdownTimer = 120f;
        private float panicAttackLevel;

        private bool panicAttack = false;
        public static bool slowness = false;
        public static bool reduceVision = false;

        public bool PanicAttack { get { return panicAttack; } set { panicAttack = value; } }
        public float PanicAttackLevel { get { return panicAttackLevel; } set { panicAttackLevel = value; } }
        public float EffectTransition { get { return slowdownTimer; } }
        /// <summary>
        /// Contains all hallucinations and the SanityLevel they should occur at.
        /// </summary>
        public Dictionary<string, SanityLevel> Hallucinations { get { return hallucinations; } }
        private readonly Dictionary<string, SanityLevel> hallucinations = new Dictionary<string, SanityLevel>
        {
            {HallucinationID.Auditory, SanityLevel.Low },
            {HallucinationID.CrypticStatusEffect, SanityLevel.Low},
            {HallucinationID.CrypticMessage, SanityLevel.Low },
            {HallucinationID.FakePlayer,  SanityLevel.Low},
            {HallucinationID.FakeItem, SanityLevel.Medium },
            {HallucinationID.PowerLoss, SanityLevel.High },
        };

        private void Start()
        {

            sanityRNGFrequency = sanityRNGFrequency * InsanityRemasteredConfiguration.rngCheckTimerMultiplier;

        }
        private void Awake()
        {
            Instance = this;
        }


        private void Update()
        {
            //InsanityRemasteredDebug.QuickHotkeyTesting();
            if (localPlayer.isPlayerControlled && !localPlayer.isPlayerDead)
            {
                if (localPlayer.isInsideFactory && localPlayer.isPlayerControlled && !localPlayer.isPlayerDead)
                {
                    sanityRNGTimer += Time.deltaTime;
                    if (sanityRNGTimer > sanityRNGFrequency)
                    {
                        sanityRNGTimer = 0;
                        float rng = UnityEngine.Random.Range(0f, 1f);
                        if (rng <= .45f)
                        {
                            Hallucinate(GetRandomHallucination());
                        }
                    }
                    if (localPlayer.insanityLevel == localPlayer.maxInsanityLevel)
                    {
                        droneRNGTimer += Time.deltaTime;
                        if (droneRNGTimer > droneRNGFrequency)
                        {
                            droneRNGTimer = 0;
                            float rng = UnityEngine.Random.Range(0f, 1f);
                            if (rng <= .45f)
                            {
                                InsanitySoundManager.Instance.PlayDrone();
                                if (panicAttackLevel == 1 && InsanityRemasteredConfiguration.panicAttacksEnabled)
                                {
                                    PanicAttackSymptom();
                                }
                            }
                        }

                        if (localPlayer.isInsideFactory && localPlayer.isPlayerAlone && !InsanityGameManager.Instance.IsNearLightSource)
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
                    panicAttackLevel = Mathf.MoveTowards(panicAttackLevel, 0, 0.5f * Time.deltaTime);
                    HallucinationEffects.LessenPanicEffects();
                }
            }
        }
        private void HighSanityEffects()
        {
            panicAttackLevel = Mathf.MoveTowards(panicAttackLevel, 1, slowdownTimer * Time.deltaTime);
            HallucinationEffects.IncreasePanicEffects();
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
                    if (InsanityRemasteredConfiguration.enableChanceToDieDuringPanicAttack)
                    {
                        localPlayer.KillPlayer(Vector3.zero);
                    }
                    else
                    {
                        slowness = true;
                    }
                    break;
            }
            HUDManager.Instance.DisplayTip("WARNING!", "Heartrate is at dangerous levels. Please seek help immediately.", true);
            OnExperiencePanicAttack?.Invoke();

        }
        public void Hallucinate(string id)
        {
            InsanityRemasteredLogger.Log("Performing hallucination with ID: " + id);

            float chanceForAnotherHallucination = UnityEngine.Random.Range(0, 1f);
            switch (id)
            {
                case HallucinationID.CrypticStatusEffect:
                    UpdateStatusEffect();
                    break;
                case HallucinationID.CrypticMessage:
                    ShowHallucinationTip();
                    break;
                case HallucinationID.FakePlayer:
                    PlayerModelHallucination(InsanityGameManager.Instance.currentHallucinationModel);
                    break;
                case HallucinationID.Auditory:
                    PlaySound();
                    break;
                case HallucinationID.FakeItem:
                    SpawnFakeObject();
                    break;
                case HallucinationID.PowerLoss:
                    LightHallucination();
                    break;
            }
            if (chanceForAnotherHallucination <= 0.15f && hallucinations[id] >= SanityLevel.Medium)
            {
                Hallucinate(GetRandomHallucination());
            }
        }
        public string GetRandomHallucination()
        {

            var randomHallucination = hallucinations.ElementAt(UnityEngine.Random.Range(0, hallucinations.Count()));
            if (randomHallucination.Value <= SanityLevel)
            {
                return randomHallucination.Key;
            }
            else
            {
                GetRandomHallucination();
            }
            return HallucinationID.Auditory;
        }
        #region Hallucination Code
        private void UpdateStatusEffect()
        {
            if (InsanityRemasteredConfiguration.enableHallucinationMessages)
            {
                string randomString = InsanityRemasteredConfiguration.statusEffectTexts[UnityEngine.Random.Range(0, InsanityRemasteredConfiguration.statusEffectTexts.Count)];
                HUDManager.Instance.DisplayStatusEffect(randomString);
                OnUIHallucination?.Invoke();
            }
        }
        private void LightHallucination()
        {
            if (InsanityRemasteredConfiguration.powerLossEventEnabled)
            {
                if (!InsanityGameManager.Instance.LightsOff)
                {
                    foreach (Animator light in RoundManager.Instance.allPoweredLightsAnimators)
                    {
                        light.SetBool("on", false);
                    }
                    PlayerPatcher.LocalPlayer.JumpToFearLevel(0.3f);
                    OnPowerHallucination?.Invoke(false);
                }
                else if (InsanityGameManager.Instance.LightsOff)
                {
                    foreach (Animator light in RoundManager.Instance.allPoweredLightsAnimators)
                    {
                        light.SetBool("on", true);
                    }
                    OnPowerHallucination?.Invoke(true);
                }
            }
        }

        private void ShowHallucinationTip()
        {
            if (InsanityRemasteredConfiguration.enableHallucinationMessages)
            {
                string randomString = InsanityRemasteredConfiguration.hallucinationTipTexts[UnityEngine.Random.Range(0, InsanityRemasteredConfiguration.hallucinationTipTexts.Count)];
                if (randomString == InsanityRemasteredConfiguration.hallucinationTipTexts[1])
                {
                    float rng = UnityEngine.Random.Range(0f, 1f);
                    if (rng <= 0.35f)
                    {
                        Invoke("LightHallucination", 3);
                    }
                }
                OnUIHallucination?.Invoke();
                HUDManager.Instance.DisplayTip("", randomString, true);
            }
        }
        private void PlayerModelHallucination(GameObject model)
        {
            if (InsanityRemasteredConfiguration.modelHallucinationsEnabled)
            {
                if (!model.activeInHierarchy)
                {
                    model.SetActive(true);
                }
                OnPlayerHallucinationStarted?.Invoke();
            }
        }
        private void PlaySound()
        {
            if (InsanityRemasteredConfiguration.auditoryHallucinationsEnabled)
            {
                InsanitySoundManager.Instance.PlayHallucinationSound();
            }
        }
        private void SpawnFakeObject()
        {
            if (InsanityRemasteredConfiguration.fakeItemsEnabled)
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
        #endregion

    }
    public class HallucinationID
    {
        public const string Observer = "Observer";
        public const string CrypticStatusEffect = "CrypticStatus";
        public const string Auditory = "AuditoryHallucination";
        public const string CrypticMessage = "CrypticMessage";
        public const string FakeItem = "Fake Item";
        public const string FakePlayer = "Fake Player";
        public const string PowerLoss = "Power loss";
    }
}