using Dissonance;
using DunGen.Graph;
using GameNetcodeStuff;
using InsanityRemastered.Hallucinations;
using InsanityRemastered.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace InsanityRemastered.General
{
    internal class InsanityGameManager : MonoBehaviour
    {

        public static InsanityGameManager Instance;

        private PlayerControllerB LocalPlayer => GameNetworkManager.Instance.localPlayerController;
        // Level1Flow is Bunker.
        // Level2Flow is Mansion.
        public static DungeonFlow MapFlow { get { return RoundManager.Instance.dungeonGenerator.Generator.DungeonFlow; } }
        public static bool AreThereOtherPlayers => StartOfRound.Instance.connectedPlayersAmount > 0;
        public bool IsNearPlayers => NearOtherPlayers();
        public bool IsNearLightSource => NearLightSource();
        public bool IsPlayerTalking => PlayerTalking();

        public bool LightsOff { get; private set; }

        private List<Light> bunkerLights = new List<Light>();
        public List<Light> BunkerLights { get { return bunkerLights; } }
        public GameObject currentHallucinationModel;

        private void Awake()
        {
            if (Instance == null) { Instance = this; }
            GameEvents.OnGameEnd += OnRoundEnd;
            GameEvents.OnShipLanded += GameEvents_OnShipLanded;
            GameEvents.OnPlayerDied += GameEvents_OnPlayerDied;
            GameEvents.OnEnterOrLeaveFacility += OnEnterOrLeaveFacility;
            HallucinationManager.OnPowerHallucination += PowerHallucination;
            SceneManager.sceneLoaded += SceneLoaded;
        }
        private void GameEvents_OnShipLanded()
        {
            CacheLights();
        }

        private void Update()
        {

            if (GameNetworkManager.Instance.gameHasStarted)
            {
                if (RoundManager.Instance.powerOffPermanently)
                {

                    LightsOff = true;
                }
            }
        }

        private void PowerHallucination(bool on)
        {
            if (on)
            {
                LightsOff = false;
            }
            else if (!on)
            {
                LightsOff = true;
            }
        }
        private void SceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            if (scene.name == SceneNames.SampleSceneRelay.ToString())
            {
                SavePlayerModel();
                HallucinationManager.Instance.enabled = true;
            }
            else if (scene.name == SceneNames.MainMenu.ToString() || scene.name == SceneNames.InitSceneLaunchOptions.ToString())
            {
                HallucinationManager.Instance.enabled = false;
            }
        }
        private void GameEvents_OnPlayerDied()
        {
            InsanitySoundManager.Instance.StopModSounds();
            HallucinationManager.Instance.ResetPanicValues();
        }
        private void OnRoundEnd()
        {
            currentHallucinationModel.SetActive(false);

            LocalPlayer.insanityLevel = 0;
        }
        private void OnEnterOrLeaveFacility(bool outside)
        {
            if (outside && Instance.LightsOff)
            {
                ResetLights();
            }
        }
        public void ResetLights()
        {
            if (LightsOff)
            {
                HallucinationManager.Instance.Hallucinate(HallucinationID.PowerLoss);
            }
        }
        private bool NearOtherPlayers(PlayerControllerB playerScript = null, float checkRadius = 16f)
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
        private bool PlayerTalking()
        {

            VoicePlayerState voiceState = StartOfRound.Instance.voiceChatModule.FindPlayer(StartOfRound.Instance.voiceChatModule.LocalPlayerName);
            float volume = Mathf.Clamp(voiceState.Amplitude, 0, 1f);
            return voiceState.IsSpeaking && volume > 0.85f;
        }
        private bool NearLightSource(float checkRadius = 10f)
        {
            for (int i = 0; i < RoundManager.Instance.allPoweredLights.Count; i++)
            {
                float lightDistance = Vector3.Distance(RoundManager.Instance.allPoweredLights[i].transform.position, LocalPlayer.transform.position);
                //bool isBlocked = Physics.Linecast(LocalPlayer.transform.position, RoundManager.Instance.allPoweredLightsAnimators[i].transform.position, 8);
                if (lightDistance < checkRadius && RoundManager.Instance.allPoweredLightsAnimators[i].GetBool("on"))
                {
                    return true;
                }
            }
            return false;
        }
        private void SavePlayerModel()
        {

            GameObject model = Instantiate(GameObject.Find("ScavengerModel"));

            foreach (Transform child in model.transform)
            {
                if (child.name == "LOD2" || child.name == "LOD3")
                {
                    child.gameObject.SetActive(false);
                }
                if (child.name == "LOD1")
                {
                    child.gameObject.SetActive(true);
                }
                if (child.name == "metarig")
                {
                    foreach (Transform _child in child.transform)
                    {
                        if (_child.name == "ScavengerModelArmsOnly")
                        {
                            _child.gameObject.SetActive(false);
                        }
                        if (_child.name == "CameraContainer")
                        {
                            _child.gameObject.SetActive(false);
                        }
                    }
                }
            }
            model.SetActive(false);
            model.AddComponent<PlayerHallucination>();
            model.AddComponent<NavMeshAgent>();
            model.GetComponent<LODGroup>().enabled = false;
            currentHallucinationModel = model;
        }
        private void CacheLights()
        {
            BunkerLights.Clear();
            foreach (Light light in RoundManager.Instance.allPoweredLights)
            {
                if (!bunkerLights.Contains(light))
                {
                    bunkerLights.Add(light);
                }
            }
        }
    }
}
public enum SanityLevel
{
    Low,
    Medium,
    High,
    Max,
}
public enum SceneNames
{
    InitSceneLaunchOptions,
    MainMenu,
    SampleSceneRelay,
}