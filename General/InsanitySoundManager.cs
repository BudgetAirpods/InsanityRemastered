using InsanityRemasteredMod.Mod_Integrations;
using UnityEngine;
namespace SanityRewrittenMod
{

    internal class InsanitySoundManager : MonoBehaviour
    {
        public static InsanitySoundManager Instance;

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
            droneSource.volume = InsanityRemasteredBase.sfxVolume;
        }

        public AudioClip LoadFakePlayerSound()
        {
            int randomClip = UnityEngine.Random.Range(0, InsanityRemasteredBase.playerHallucinationSounds.Length);
            if (InsanityRemasteredBase.playerHallucinationSounds[randomClip])
            {
                {
                    if (InsanityRemasteredBase.playerHallucinationSounds[randomClip].name != "JumpScare")
                    {
                        return InsanityRemasteredBase.playerHallucinationSounds[randomClip];
                    }
                }
            }
            return null;
        }
        public void PlayJumpscare() 
        {
            foreach (AudioClip clip in InsanityRemasteredBase.playerHallucinationSounds)
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

            float skinWalkerChance = UnityEngine.Random.Range(0f, 1f);
            if (skinWalkerChance >= 0.5f && SkinwalkerModIntegration.IsInstalled && StartOfRound.Instance.connectedPlayersAmount > 0)
            {
                SoundManager.Instance.PlaySoundAroundLocalPlayer(SkinwalkerModIntegration.GetRandomClip(), 2.5f);
            }
            else
            {
                SoundManager.Instance.PlaySoundAroundLocalPlayer(LoadHallucinationSound(), 2f);
            }
           
        }
        public void PlayHallucinationSound(string name, bool mono = false)
        {
            if (mono)
            {
                hallucinationSource.PlayOneShot(LoadHallucinationSound(name), hallucinationSource.volume + 0.25f);
            }
            else if (!mono)
            {
                SoundManager.Instance.PlaySoundAroundLocalPlayer(LoadHallucinationSound(name), 2f);
            }

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
        private AudioClip LoadHallucinationSound()
        {

            float vanillagameSFXRNG = UnityEngine.Random.Range(0f, 1f);
            if (vanillagameSFXRNG <= 0.4f)
            {
                int randomClip = UnityEngine.Random.Range(0, InsanityRemasteredBase.LCGameSFX.Length);
                if (InsanityRemasteredBase.LCGameSFX[randomClip])
                {
                    return InsanityRemasteredBase.LCGameSFX[randomClip];
                }
            }
            else
            {
                int randomClip = UnityEngine.Random.Range(0, InsanityRemasteredBase.auditoryHallucinations.Length);
                if (InsanityRemasteredBase.auditoryHallucinations[randomClip])
                {
                    return InsanityRemasteredBase.auditoryHallucinations[randomClip];
                }
                
            }
            return null;
        }
        private AudioClip LoadHallucinationSound(string clipName)
        {
            foreach (AudioClip clip in InsanityRemasteredBase.auditoryHallucinations)
            {
                if (clip.name == clipName)
                {
                    return clip;
                }
            }
            return null;
        }
        private AudioClip LoadStingerSound()
        {
            int randomClip = UnityEngine.Random.Range(0, InsanityRemasteredBase.stingers.Length);
            if (InsanityRemasteredBase.stingers[randomClip])
            {
                return InsanityRemasteredBase.stingers[randomClip];
            }
            return null;
        }
        private AudioClip LoadDroneSound()
        {
            int randomClip = UnityEngine.Random.Range(0, InsanityRemasteredBase.drones.Length);
            if (InsanityRemasteredBase.drones[randomClip])
            {
                return InsanityRemasteredBase.drones[randomClip];
            }
            return null;
        }
        private AudioClip LoadStingerSound(string clipName)
        {
            foreach (AudioClip clip in InsanityRemasteredBase.stingers)
            {
                if (clip.name == clipName)
                {
                    return clip;
                }
            }
            return null;
        }
    }


}
