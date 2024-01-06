﻿using SanityRewrittenMod.Hallucinations;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using SanityRewrittenMod.Patches;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Net;

using GameNetcodeStuff;

using SanityRewrittenMod.Utilities;
using UnityEngine.AI;
using UnityEngine.Windows;
using System;
using DunGen.Graph;
using UnityEngine.SceneManagement;
using InsanityRemasteredMod.General;
using SanityRewrittenMod.General;

namespace SanityRewrittenMod
{
    internal class SanityMainManager : MonoBehaviour
    {

        public static SanityMainManager Instance;

        private PlayerControllerB localPlayer => GameNetworkManager.Instance.localPlayerController;
        // Level1Flow is Bunker.
        // Level2Flow is Mansion.
        public static DungeonFlow MapFlow { get { return RoundManager.Instance.dungeonGenerator.Generator.DungeonFlow; } }
        public GameObject currentHallucinationModel;



        public bool IsNearLightSource { get; set; }
        public bool LightsOff { get; private set; }

        private List<Light> bunkerLights = new List<Light>();
        public List<Light> BunkerLights { get { return bunkerLights; } }
        private List<GameObject> entityModels = new List<GameObject>();
        public List<GameObject> EntityModels { get { return  entityModels; } }

        private void Awake()
        {
            if (Instance == null) { Instance = this; }
            GameEvents.OnGameEnd += OnRoundEnd;
            GameEvents.OnShipLanded += GameEvents_OnShipLanded;
            GameEvents.OnPlayerDied += GameEvents_OnPlayerDied;
            BunkerHallucinations.OnPowerHallucination += PowerHallucination;
            InsanityRemastered_AI.OnHallucinationEnded += TouchedByHallucination;
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void GameEvents_OnShipLanded()
        {
            CacheLights();
        }

        private void PowerHallucination(bool on)
        {
            if (on)
            {
                HallucinationManager.Instance.ResetLights();
                LightsOff = false;
            }
            else if (!on)
            {
                LightsOff = true;
            }
        }

        private void TouchedByHallucination(bool touched)
        {
            if(touched && HallucinationManager.Instance.PanicAttack)
            {
                float killChance = UnityEngine.Random.Range(0f, 1f);
                if(killChance <= 0.15f)
                {
                    localPlayer.KillPlayer(Vector3.zero);
                }
            }
        }

        private void Update()
        {
            if (UnityInput.Current.GetKeyDown("b"))
            {
                BunkerHallucinations.PlayerModelHallucination(currentHallucinationModel);
            }
            if (UnityInput.Current.GetKeyDown("f"))
            {
                HallucinationManager.Instance.PickHallucination();
            }
            if (UnityInput.Current.GetKeyDown("v"))
            {
                BunkerHallucinations.LightHallucination();
            }
        }

        private void SceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            if (scene.name == SceneNames.SampleSceneRelay.ToString())
            {
                SavePlayerModel();
                HallucinationManager.Instance.enabled = true;
            }
            else if (scene.name == SceneNames.MainMenu.ToString()|| scene.name == SceneNames.InitSceneLaunchOptions.ToString())
            {
                HallucinationManager.Instance.enabled = false;
            }
        }

        private void GameEvents_OnPlayerDied()
        {
            SanitySoundManager.Instance.StopModSounds();
        }

        private void OnRoundEnd()
        {
            currentHallucinationModel.SetActive(false);

            localPlayer.insanityLevel = 0;

        }

        private void SavePlayerModel()
        {
            if (currentHallucinationModel != null)
            {
                return;
            }
            //Clone scavenger player model and disables LODGroup component to prevent loading LODS on the fake player.

            GameObject model = Instantiate(GameObject.Find("ScavengerModel"));

            foreach (Transform child in model.transform)
            {
                if (child.name == "LOD1" || child.name == "LOD3")
                {
                child.gameObject.SetActive(false);
                }
                if (child.name == "LOD2")
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
            EntityModels.Add(model);
            currentHallucinationModel = model;
            model.name = "Hallucination Model";
            model.AddComponent<PlayerHallucination>();
            model.AddComponent<NavMeshAgent>();
            DontDestroyOnLoad(model);
            model.SetActive(false);
            

        }/*
        currentHallucinationModel = model;
            currentHallucinationModel.name = "Hallucination Model";
            currentHallucinationModel.SetActive(false);
            model.GetComponent<LODGroup>().enabled = false;
            model.AddComponent<NavMeshAgent>();
            PlayerHallucination hallucinate = model.AddComponent<PlayerHallucination>();
        EntityModels.Add(model);
            DontDestroyOnLoad(model);*/
        private void CacheLights()
        {

            //If the bunkerLights list already has lights in it, reset them.
            //This is to prevent lights from previous rounds from contiuing into next rounds.
            if (bunkerLights.Count > 0)
            {
                bunkerLights.Clear();
            }
            //Loops through all powered lights and adds them to the bunkerLights list. Also adds the light detection script.
            foreach (Light light in RoundManager.Instance.allPoweredLights)
            {
                if (!bunkerLights.Contains(light))
                {
                    bunkerLights.Add(light);
                    //light.gameObject.AddComponent<LightDetection>();
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