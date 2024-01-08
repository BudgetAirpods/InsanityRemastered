
using GameNetcodeStuff;
using SanityRewrittenMod.Patches;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AI;
using UnityEngine;
using SanityRewrittenMod.Utilities;
using SanityRewrittenMod.Hallucinations;
using System;
using SanityRewrittenMod.General;
using InsanityRemasteredMod.General;
using InsanityRemasteredMod.Mod_Integrations;

namespace SanityRewrittenMod
{
    internal class PlayerHallucination :  InsanityRemastered_AI
    {
        
        
        public override void LookAtHallucinationFirstTime()
        {
            base.LookAtHallucinationFirstTime();
            SanitySoundManager.Instance.PlayStinger();
        }
        public override void FinishHallucination(bool touched)
        {
            if (touched)
            {
                float effect = UnityEngine.Random.Range(0f, 1f);
                if(effect <= 0.15f)
                {
                    localPlayer.DamagePlayer(UnityEngine.Random.Range(5, 15), false, true, CauseOfDeath.Mauling);
                    return;
                }
                else if(effect <= 0.45f)
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
            SFXSource.clip = SanitySoundManager.Instance.LoadFakePlayerSound();
            SFXSource.Play();
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
            hallucinationSpawnType = HallucinationSpawnType.NotLooking;;
            hallucinationAnimator = GetComponentInChildren<Animator>();
            hallucinationAnimator.runtimeAnimatorController = StartOfRound.Instance.localClientAnimatorController;
            SFX = InsanityRemasteredBase.playerHallucinationSounds;
        }
    }
    
}
