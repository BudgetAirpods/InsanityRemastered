
using GameNetcodeStuff;
using SanityRewrittenMod.Patches;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine;
using SanityRewrittenMod.Utilities;
using SanityRewrittenMod.Hallucinations;
using System;
using SanityRewrittenMod.General;
using InsanityRemasteredMod.General;
using InsanityRemasteredMod.Mod_Integrations;

namespace SanityRewrittenMod
{
    internal class PlayerHallucination : InsanityRemastered_AI
    {
        //private Transform lookAtController;
        public override void Start()
        {
            base.Start();
            //lookAtController = transform.Find("metarig/spine/spine.001/spine.002/spine.003");
            SFX = InsanityRemasteredBase.playerHallucinationSounds;
            hallucinationAnimator.runtimeAnimatorController = StartOfRound.Instance.localClientAnimatorController;
        }
        public override void Update()
        {
            base.Update();

        }
        public override void LookAtHallucinationFirstTime()
        {
            base.LookAtHallucinationFirstTime();
            InsanitySoundManager.Instance.PlayStinger();
        }
        public override void FinishHallucination(bool touched)
        {
            if (touched)
            {
                float effect = UnityEngine.Random.Range(0f, 1f);
                if (effect <= 0.15f)
                {
                    localPlayer.DamagePlayer(UnityEngine.Random.Range(1, 5), false, true, CauseOfDeath.Mauling);
                    return;
                }
                else if (effect <= 0.45f)
                {
                    HallucinationManager.Instance.PanicAttack = true;
                }
                base.FinishHallucination(true);
            }
            else
            {
                base.FinishHallucination(false);
            }
        }

    public override void Spawn()
        {

            base.Spawn();
            float skinWalkerChance = UnityEngine.Random.Range(0f, 1f);
            if (skinWalkerChance >= 0.5f && SkinwalkerModIntegration.IsInstalled && StartOfRound.Instance.connectedPlayersAmount > 0)
            {
                sfxSource.clip = SkinwalkerModIntegration.GetRandomClip();
            }
            else
            {
                sfxSource.clip = InsanitySoundManager.Instance.LoadFakePlayerSound();
                
            }
            sfxSource.Play();
            hallucinationType = (HallucinationType)UnityEngine.Random.Range(0, 2);
            if(hallucinationType > 0)
            {
                hallucinationAnimator.SetBool(AnimationID.PlayerWalking, true);
            }
            else
            {
                hallucinationAnimator.SetBool(AnimationID.PlayerWalking, false);
            }
        }
        public override void SetupVariables()
        {
            base.SetupVariables();
            Duration = 12;
            hallucinationAnimator.runtimeAnimatorController = StartOfRound.Instance.localClientAnimatorController;
            hallucinationSpawnType = HallucinationSpawnType.NotLooking;;
        }
        
    }   
    
}
