using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using SanityRewrittenMod.Patches;
using System.IO;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using InsanityRemasteredMod.General;

namespace SanityRewrittenMod
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class SanityRewrittenBase : BaseUnityPlugin
    {
        private static SanityRewrittenBase Instance;
        public static ManualLogSource mls;

        public const string modGUID = "BudgetAirpods.InsanityRemastered";
        public const string modName = "Insanity Remastered";
        public const string modVersion = "1.0.1";

        private readonly Harmony harmony = new Harmony("BudgetAirpods.InsanityRemastered");

        public static ConfigEntry<bool> config_KillPlayerDuringPanicAttack;

        internal static AudioClip[] auditoryHallucinations;
        internal static AudioClip[] stingers;
        internal static AudioClip[] playerHallucinationSounds;
        internal static AudioClip[] LCGameSFX;
        internal static AudioClip[] drones; 
        internal static GameObject SanityModObject;
        private void Awake()
        {
            
            if (Instance = null)
            {
                Instance = this;
            }
            
            mls = BepInEx.Logging.Logger.CreateLogSource("BudgetAirpods.InsanityRemastered");
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            ConfigFile();
            LoadSounds();
            harmony.PatchAll();
            SetupModManager();
        }
        private void SetupModManager()
        {
            GameObject sanityObject = new GameObject("Sanity Mod");
            sanityObject.AddComponent<SanityMainManager>();
            sanityObject.AddComponent<SanitySoundManager>();
            sanityObject.AddComponent<HallucinationManager>();
            SanityModObject = sanityObject;
            SanityModObject.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(SanityModObject);
        }
        private void OnSceneLoaded(Scene level, LoadSceneMode loadEnum)
        {
            if(level.name == "SampleSceneRelay")
            {
                if (!SanityModObject.activeInHierarchy)
                {
                    SanityModObject.SetActive(true);
                }
            }
            if (level.name == "MainMenu")
            {
                if (SanityModObject)
                {
                    SanitySoundManager.Instance.StopModSounds();
                }
                else if (!SanityModObject)
                {
                    SetupModManager();
                }

            }
        }

        
        private void ConfigFile()
        {
            config_KillPlayerDuringPanicAttack = ((BaseUnityPlugin)this).Config.Bind<bool>("Gameplay", "KillPlayerDuringPanicAttack", false, "Adds the possibility of dying during a panic attack.");
           
        }
        private void LoadSounds()
        {

            string sfxBundle = Path.Combine(Path.GetDirectoryName(((BaseUnityPlugin)this).Info.Location), "SoundResources_SFX");
            string ambientBundle = Path.Combine(Path.GetDirectoryName(((BaseUnityPlugin)this).Info.Location), "SoundResources_Stingers");
            string fakePlayerBundle = Path.Combine(Path.GetDirectoryName(((BaseUnityPlugin)this).Info.Location), "SoundResources_Hallucination");
            string droneBundle = Path.Combine(Path.GetDirectoryName(((BaseUnityPlugin)this).Info.Location), "SoundResources_Drones");
            string lcGameBundle = Path.Combine(Path.GetDirectoryName(((BaseUnityPlugin)this).Info.Location), "SoundResources_LCGame");

            AssetBundle sfx = AssetBundle.LoadFromFile(sfxBundle);
            AssetBundle ambience = AssetBundle.LoadFromFile(ambientBundle);
            AssetBundle fakePlayer = AssetBundle.LoadFromFile(fakePlayerBundle);
            AssetBundle drone = AssetBundle.LoadFromFile(droneBundle);
            AssetBundle lcGame = AssetBundle.LoadFromFile(lcGameBundle);
            if ((object)sfx == null || (object)ambience == null|(object)fakePlayer == null || (object)droneBundle == null || (object)lcGameBundle == null)
            {
                mls.LogError((object)"Failed to load audio assets!");
                return;
            }
            auditoryHallucinations = sfx.LoadAllAssets<AudioClip>();
            stingers = ambience.LoadAllAssets<AudioClip>();
            playerHallucinationSounds = fakePlayer.LoadAllAssets<AudioClip>();
            drones = drone.LoadAllAssets<AudioClip>();
            LCGameSFX = lcGame.LoadAllAssets<AudioClip>();
        }
    }
    enum HallucinationType
    {
        Wandering,
        Approaching,
        Staring,

    }
    internal class AnimationID
    {

        public const string PlayerWalking = "Walking";
        public const string PlayerCrouching = "crouching";
    }

}
