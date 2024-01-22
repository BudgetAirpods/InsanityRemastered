using BepInEx.Configuration;
using System.Collections.Generic;

namespace InsanityRemasteredMod.General
{
    internal class InsanityRemasteredConfiguration
    {
        public static int maxPlayerCountForScalingInsanitySpeed;

        public static bool enableChanceToDieDuringPanicAttack { get; set; }
        public static bool auditoryHallucinationsEnabled { get; set; }
        public static bool modelHallucinationsEnabled { get; set; }
        public static bool fakeItemsEnabled { get; set; }
        public static bool powerLossEventEnabled { get; set; }
        public static bool panicAttacksEnabled { get; set; }
        public static bool disablePanicAttackEffects { get; set; }
        public static bool enableHallucinationMessages { get; set; }
        public static bool onlyUseVanillaSFX { get; set; }
        public static bool enableSanityLevelReminders { get; set; }

        public static float sanityLossReductionWhenUsingFlashlights { get; set; }
        public static float sfxVolume { get; set; }
        public static float rngCheckTimerMultiplier { get; set; }
        public static float insanitySpeedScalingForSolo { get; set; }
        public static float lossWhenNotNearOthers { get; set; }
        public static float lossWhenLightsAreOut { get; set; }
        public static float lossWhenPanicking { get; set; }
        public static float lossWhenOutsideDuringSundown { get; set; }
        public static float sanityGainWhenInsideShip { get; set; }
        public static float sanityGainWhenNearOthers { get; set; }

        public static List<string> hallucinationTipTexts = new List<string> { "I'm always watching.", "behind you.", "You will never make it out of here.", "Did you see that?", "The company will never be satisfied. This is all pointless.", "you are the only one alive." };
        public static List<string> statusEffectTexts = new List<string> { "WARNING:\n\nMultiple organ failures detected. Please lie down and hope it ends quick.", "SYSTEM ERROR:\n\nLife support power is dropping. Please return to your ship immediately.", "Unknown lifeform detected nearby." };

        public static void Initialize(ConfigFile Config)
        {
            sfxVolume = Config.Bind<float>("SFX", "Stinger/Drone volume", 0.25f, "Sets the volume of the stinger and drone sounds. Max value is 1.").Value;

            panicAttacksEnabled = Config.Bind<bool>("General", "Able to experience a panic attack.", true, "Enables panic attacks.").Value;

            enableChanceToDieDuringPanicAttack = Config.Bind<bool>("General", "Able to die during panic attack.", false, "Enables the chance to die when having a panic attack.").Value;
            disablePanicAttackEffects = Config.Bind<bool>("General", "Disable panic attack effects.", false, "Disables the panic attack audio and visual effects.").Value;
            enableSanityLevelReminders = Config.Bind<bool>("General", "Notifications for sanity levels.", true, "Allows for notifications telling you your sanity level is increasing.").Value;


            maxPlayerCountForScalingInsanitySpeed = Config.Bind<int>("Scaling", "Max player count for sanity loss scaling.", 5, "Sets the max amount of players to take into account when scaling sanity loss.").Value;
            insanitySpeedScalingForSolo = Config.Bind<float>("Scaling", "Solo insanity speed scaling", 0.85f, "Sets the insanity speed scaling value if playing solo.").Value;

            lossWhenLightsAreOut = Config.Bind<float>("Sanity Loss", "Sanity loss during lights out.", 0.21f, "Sets sanity loss during the lights out event.").Value;
            lossWhenNotNearOthers = Config.Bind<float>("Sanity Loss", "Sanity loss when not near others.", 0.16f, "Sets sanity loss when you are not near other players.").Value;
            lossWhenPanicking = Config.Bind<float>("Sanity Loss", "Sanity loss during a panic attack.", 1.26f, "Sets sanity loss during a panic attack.").Value;
            lossWhenOutsideDuringSundown = Config.Bind<float>("Sanity Loss", "Sanity loss during nighttime outside.", 0.2f, "Sets sanity loss when it's night and you are outside.").Value;


            sanityGainWhenInsideShip = Config.Bind<float>("Sanity Gain", "Sanity gain while inside ship", -0.55f, "Sets the sanity gain when inside the ship.").Value;
            sanityGainWhenNearOthers = Config.Bind<float>("Sanity Gain", "Sanity gain while near others", -0.26f, "Sets the sanity gain when near other players.").Value;

            enableHallucinationMessages = Config.Bind<bool>("Hallucinations", "Allow the game to randomly disply cryptic messages.", true, "Enables/Disables the random hallucination messages that can show when playing the game.").Value;
            auditoryHallucinationsEnabled = Config.Bind<bool>("Hallucinations", "Able to experience auditory hallucinations.", true, "Enables or Disables the ability to have auditory hallucinations.").Value;
            fakeItemsEnabled = Config.Bind<bool>("Hallucinations", "Fake items can spawn.", true, "Enables or Disables the chance for fake items to spawn").Value;
            modelHallucinationsEnabled = Config.Bind<bool>("Hallucinations", "Able to experience the model hallucination", true, "Enables or Disables the ability to experience the fake player or enemy model hallucination.").Value;
            powerLossEventEnabled = Config.Bind<bool>("Hallucinations", "Able to experience Lights off.", true, "Enables or disable the chance to experience the power shutoff event.").Value;


            sanityLossReductionWhenUsingFlashlights = Config.Bind<float>("Misc", "Reduction when using flashlights.", 2f, "Sets the sanity loss reduction amount when using a flashlight").Value;
            rngCheckTimerMultiplier = Config.Bind<float>("Misc", "Multiplier for hallucination RNG check.", 1, "A multiplier for the hallucination RNG timer. 2 means it will happens twice as often, 0.5 meaning half as often.").Value;
            onlyUseVanillaSFX = Config.Bind<bool>("Misc", "Only play vanilla sfx for sound hallucinations", false, "Only allow vanilla sounds to play when experiencing auditory hallucinations.").Value;


        }
        public static void ValidateSettings()
        {
            if (sfxVolume > 1)
            {
                sfxVolume = 1;
            }
            if (maxPlayerCountForScalingInsanitySpeed <= 0)
            {
                maxPlayerCountForScalingInsanitySpeed = 1;
            }
            if (rngCheckTimerMultiplier <= 0)
            {
                rngCheckTimerMultiplier = 1;
            }

        }
    }
}
