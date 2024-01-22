using InsanityRemastered.ModIntegration;
using InsanityRemasteredMod;
using InsanityRemasteredMod.General;
using UnityEngine;
namespace InsanityRemastered.General
{

    internal class InsanitySoundManager : MonoBehaviour
    {
        public static InsanitySoundManager Instance;
        public AudioClip[] hallucinationEffects;
        public AudioClip[] drones;
        public AudioClip[] playerHallucinationSounds;
        public AudioClip[] vanillaSFX;
        public AudioClip[] stingers;
        public AudioSource hallucinationSource;
        private AudioSource droneSource;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            hallucinationSource = gameObject.AddComponent<AudioSource>();
            hallucinationSource.spatialBlend = 0;
            droneSource = gameObject.AddComponent<AudioSource>();
            droneSource.spatialBlend = 0;
            droneSource.volume = InsanityRemasteredConfiguration.sfxVolume;
            CacheSFX();
        }
        private void CacheSFX()
        {
            stingers = InsanityRemasteredContent.Stingers;
            vanillaSFX = InsanityRemasteredContent.LCGameSFX;
            drones = InsanityRemasteredContent.Drones;
            hallucinationEffects = InsanityRemasteredContent.AuditoryHallucinations;
            playerHallucinationSounds = InsanityRemasteredContent.PlayerHallucinationSounds;
        }
        public AudioClip LoadFakePlayerSound()
        {
            int randomClip = Random.Range(0, playerHallucinationSounds.Length);
            if (playerHallucinationSounds[randomClip])
            {
                {
                    if (playerHallucinationSounds[randomClip].name != "JumpScare")
                    {
                        return playerHallucinationSounds[randomClip];
                    }
                }
            }
            return null;
        }
        public void PlayJumpscare()
        {
            foreach (AudioClip clip in playerHallucinationSounds)
            {
                if (clip.name == "JumpScare")
                {
                    hallucinationSource.clip = clip;
                    hallucinationSource.Play();
                }
            }
        }
        public void PlayStinger(bool mono = true)
        {
            if (mono)
            {
                droneSource.clip = LoadStingerSound();
                droneSource.Play();
            }
        }

        public void PlayHallucinationSound()
        {

            float skinWalkerChance = Random.Range(0f, 1f);
            if (skinWalkerChance >= 0.5f && SkinwalkerModIntegration.IsInstalled && StartOfRound.Instance.connectedPlayersAmount > 0)
            {
                SoundManager.Instance.PlaySoundAroundLocalPlayer(SkinwalkerModIntegration.GetRandomClip(), 2.5f);
            }
            else
            {
                if (InsanityRemasteredConfiguration.onlyUseVanillaSFX)
                {
                    SoundManager.Instance.PlaySoundAroundLocalPlayer(vanillaSFX[Random.Range(0, vanillaSFX.Length)], 1.4f);
                    return;
                }
                SoundManager.Instance.PlaySoundAroundLocalPlayer(LoadHallucinationSound(), 0.85f);
            }

        }
        public void PlayUISound(AudioClip sfx)
        {
            hallucinationSource.PlayOneShot(sfx, 0.8f);
        }
        public void PlayDrone()
        {
            if (droneSource.isPlaying)
            {
                return;
            }
            droneSource.clip = LoadDroneSound();
            droneSource.Play();
        }
        public void StopModSounds()
        {
            hallucinationSource.Stop();
            droneSource.Stop();
        }
        public AudioClip LoadHallucinationSound()
        {

            float vanillagameSFXRNG = Random.Range(0f, 1f);
            if (vanillagameSFXRNG <= 0.4f)
            {
                int randomClip = Random.Range(0, vanillaSFX.Length);
                if (vanillaSFX[randomClip])
                {
                    return vanillaSFX[randomClip];
                }
            }
            else
            {
                int randomClip = Random.Range(0, hallucinationEffects.Length);
                if (hallucinationEffects[randomClip])
                {
                    return hallucinationEffects[randomClip];
                }

            }
            return null;
        }
        private AudioClip LoadStingerSound()
        {
            int randomClip = Random.Range(0, stingers.Length);
            if (stingers[randomClip])
            {
                return stingers[randomClip];
            }
            return null;
        }
        private AudioClip LoadDroneSound()
        {
            int randomClip = Random.Range(0, drones.Length);
            if (drones[randomClip])
            {
                return drones[randomClip];
            }
            return null;
        }
    }


}
