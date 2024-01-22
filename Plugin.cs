using BepInEx;
using HarmonyLib;
using InsanityRemastered.General;
using InsanityRemastered.ModIntegration;
using InsanityRemasteredMod;
using InsanityRemasteredMod.General;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace InsanityRemastered
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class InsanityRemasteredBase : BaseUnityPlugin
    {
        public static InsanityRemasteredBase Instance;

        public const string modGUID = "BudgetAirpods.InsanityRemastered";
        public const string modName = "Insanity Remastered";
        public const string modVersion = "1.1.0";

        private readonly Harmony harmony = new Harmony("BudgetAirpods.InsanityRemastered");

        internal static GameObject SanityModObject;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;

            InsanityRemasteredLogger.Initialize(modGUID);
            InsanityRemasteredConfiguration.Initialize(Config);
            InsanityRemasteredConfiguration.ValidateSettings();
            InsanityRemasteredContent.LoadContent();

            harmony.PatchAll();
        }

        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            ModIntegrator.BeginIntegrations(args.LoadedAssembly);
        }


        private void SetupModManager()
        {
            GameObject sanityObject = new GameObject("Sanity Mod");
            sanityObject.AddComponent<InsanityGameManager>();
            sanityObject.AddComponent<InsanitySoundManager>();
            sanityObject.AddComponent<HallucinationManager>().enabled = false;
            SanityModObject = sanityObject;
            SanityModObject.hideFlags = HideFlags.HideAndDontSave;

        }
        private void OnSceneLoaded(Scene level, LoadSceneMode loadEnum)
        {
            if (level.name == SceneNames.SampleSceneRelay.ToString())
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
                    InsanitySoundManager.Instance.StopModSounds();
                    SanityModObject.hideFlags = HideFlags.HideAndDontSave;

                }
                else if (!SanityModObject)
                {
                    SetupModManager();
                    SanityModObject.hideFlags = HideFlags.HideAndDontSave;
                }

            }
        }






    }
    public enum HallucinationType
    {
        Staring,
        Wandering,
        Approaching,
    }

    internal class AnimationID
    {

        public const string PlayerWalking = "Walking";
        public const string PlayerCrouching = "crouching";
        public const string SpiderMoving = "moving";
        public const string BrackenMoving = "sneak";
    }

}
