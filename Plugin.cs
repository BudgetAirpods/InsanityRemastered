using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using InsanityRemasteredMod.General;
using BepInEx.Bootstrap;
using InsanityRemasteredMod.Mod_Integrations;
using static UnityEngine.Rendering.DebugUI;


namespace SanityRewrittenMod
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class InsanityRemasteredBase : BaseUnityPlugin
    {
        private static InsanityRemasteredBase Instance;
        public static ManualLogSource mls;

        public const string modGUID = "BudgetAirpods.InsanityRemastered";
        public const string modName = "Insanity Remastered";
        public const string modVersion = "1.0.5";
        private readonly Harmony harmony = new Harmony("BudgetAirpods.InsanityRemastered");

        public static int maxPlayerCountForScalingInsanitySpeed;
        public static bool enableChanceToDieDuringPanicAttack;
        public static bool auditoryHallucinationsEnabled;
        public static bool modelHallucinationsEnabled;
        public static bool fakeItemsEnabled;
        public static bool powerLossEventEnabled;
        public static bool panicAttacksEnabled;

        public static float insanitySpeedScalingForSolo;
        public static float lossWhenNotNearOthers;
        public static float lossWhenLightsAreOut;
        public static float lossWhenPanicking;
        public static float lossWhenOutsideDuringSundown;
        public static float sanityGainWhenInsideShip;
        public static float sanityGainWhenNearOthers;
        
        
        internal static AudioClip[] auditoryHallucinations;
        internal static AudioClip[] stingers;
        internal static AudioClip[] playerHallucinationSounds;
        internal static AudioClip[] LCGameSFX;
        internal static AudioClip[] drones; 
        internal static GameObject SanityModObject;

    
        ///Dissonance_Diagnostics
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
            
        }

        private void SetupModManager()
        {
            GameObject sanityObject = new GameObject("Sanity Mod");
            sanityObject.AddComponent<SanityMainManager>();
            sanityObject.AddComponent<SanitySoundManager>();
            sanityObject.AddComponent<HallucinationManager>().enabled = false;
            SanityModObject = sanityObject;
            SanityModObject.hideFlags = HideFlags.HideAndDontSave;
            SetupIntegrations();
            DontDestroyOnLoad(SanityModObject);
        }
        private static void SetupIntegrations()
        {
            if (ModExists(SkinwalkerModIntegration.SkinwalkerModName))
            {
                SanityModObject.AddComponent<SkinwalkerModIntegration>().SetupIntegration();
            }
        }
        private void OnSceneLoaded(Scene level, LoadSceneMode loadEnum)
        {
            if(level.name == SceneNames.SampleSceneRelay.ToString())
            {
                if (!SanityModObject.activeInHierarchy)
                {
                    SanityModObject.SetActive(true);
                }
            }
            if (level.name == SceneNames.MainMenu.ToString())
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
        public static bool ModExists(string modName)
        {
            foreach (var mod in Chainloader.PluginInfos)
            {
                var modData = mod.Value.Metadata;
                if (modData.Name == modName)
                {
                    return true;
                }
            }
            return false;
        }

        private void ConfigFile()
        {
            //maxPlayerCountForScalingInsanitySpeed = Config.Bind<int>("Sanity Loss", "Max player count for sanity loss scaling.", 10, "Sets the max amount of players for sanity loss scaling.").Value;
            insanitySpeedScalingForSolo = Config.Bind<float>("Sanity Loss", "Solo insanity speed scaling", 1.5f, "Sets the insanity speed scaling value if playing solo.").Value;
            lossWhenLightsAreOut = Config.Bind<float>("Sanity Loss", "Sanity loss during lights out.", 0.31f, "Sets sanity loss during the lights out event.").Value;
            lossWhenNotNearOthers = Config.Bind<float>("Sanity Loss", "Sanity loss when not near others.", 0.26f, "Sets sanity loss when you are not near other players.").Value;
            lossWhenPanicking = Config.Bind<float>("Sanity Loss", "Sanity loss during a panic attack.", 1.26f, "Sets sanity loss during a panic attack.").Value;
            lossWhenOutsideDuringSundown = Config.Bind<float>("Sanity Loss", "Sanity loss during nighttime outside.", 0.2f, "Sets sanity loss when it's night and you are outside.").Value;

            sanityGainWhenInsideShip = Config.Bind<float>("Sanity Gain", "Sanity gain while inside ship", -0.55f, "Sets the sanity gain when inside the ship.").Value;
            sanityGainWhenNearOthers = Config.Bind<float>("Sanity Gain", "Sanity gain while near others", -0.1f, "Sets the sanity gain when near other players.").Value;

            enableChanceToDieDuringPanicAttack = Config.Bind<bool>("Hallucinations", "Able to die during panic attack.", false, "Enables or Disables the chance to die when having a panic attack.").Value;
            panicAttacksEnabled = Config.Bind<bool>("Hallucinations", "Able to experience a panic attack.", true, "Enables or disables panic attacks.").Value;
            auditoryHallucinationsEnabled =Config.Bind<bool>("Hallucinations", "Able to experience auditory hallucinations.", true, "Enables or Disables the ability to have auditory hallucinations.").Value;
            fakeItemsEnabled = Config.Bind<bool>("Hallucinations", "Fake items can spawn.", true, "Enables or Disables the chance for fake items to spawn").Value;
            modelHallucinationsEnabled = Config.Bind<bool>("Hallucinations", "Able to experience the model hallucination", true, "Enables or Disables the ability to experience the fake player or enemy model hallucination.").Value;
            powerLossEventEnabled = Config.Bind<bool>("Hallucinations", "Able to experience Lights off.", true, "Enables or disable the chance to experience the power shutoff event.").Value;
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
