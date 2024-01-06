using SanityRewrittenMod.Patches;
using UnityEngine;

namespace SanityRewrittenMod
{

    internal class SanitySoundManager : MonoBehaviour
    {
        public static SanitySoundManager Instance;

        private AudioSource hallucinationSource;
        private AudioSource droneSource;
        private void Awake()
        {
            if(Instance== null)
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
            droneSource.volume = 0.56f;
        }

        public AudioClip LoadFakePlayerSound()
        {
            int randomClip = UnityEngine.Random.Range(0, SanityRewrittenBase.playerHallucinationSounds.Length);
            if (SanityRewrittenBase.playerHallucinationSounds[randomClip])
            {
                {
                    if(SanityRewrittenBase.playerHallucinationSounds[randomClip].name != "JumpScare")
                    {
                        return SanityRewrittenBase.playerHallucinationSounds[randomClip];
                    }
                }
            }
            return null;
        }
        public void PlayJumpscare()
        {
            foreach(AudioClip clip in SanityRewrittenBase.playerHallucinationSounds)
            {
                if(clip.name == "JumpScare")
                {
                    hallucinationSource.clip =clip;
                    hallucinationSource.Play();
                }
            }
        }
        public void PlayStinger(bool mono = true)
        {
            if (mono)
            {
                hallucinationSource.clip = LoadStingerSound();
                hallucinationSource.Play();
            }
            else
            {
                SoundManager.Instance.PlaySoundAroundLocalPlayer(LoadStingerSound(),2);
            }
        }
        public void PlayStinger(string name)
        {
            hallucinationSource.clip = LoadStingerSound(name);
            hallucinationSource.Play();

        }
        public void PlayHallucinationSound()
        {
            SoundManager.Instance.PlaySoundAroundLocalPlayer(LoadHallucinationSound(), 3f);


        }
        public void PlayHallucinationSound(bool mono = false)
        {
            if (!mono)
            {
                SoundManager.Instance.PlaySoundAroundLocalPlayer(LoadHallucinationSound(name), 3f);
            }
        }
        public void PlayHallucinationSound(string name, bool mono=false)
        {
            if (mono)
            {
                hallucinationSource.PlayOneShot(LoadHallucinationSound(name), hallucinationSource.volume + 0.25f);
            }
            else if(!mono)
            {
                SoundManager.Instance.PlaySoundAroundLocalPlayer(LoadHallucinationSound(name),2f);
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
                int randomClip = UnityEngine.Random.Range(0, SanityRewrittenBase.LCGameSFX.Length);
                if (SanityRewrittenBase.LCGameSFX[randomClip])
                {
                    return SanityRewrittenBase.LCGameSFX[randomClip];
                }
            }
            else
            {
                int randomClip = UnityEngine.Random.Range(0, SanityRewrittenBase.auditoryHallucinations.Length);
                if (SanityRewrittenBase.auditoryHallucinations[randomClip])
                {
                    return SanityRewrittenBase.auditoryHallucinations[randomClip];
                }
            }
            return null;
        }
        private AudioClip LoadHallucinationSound(string clipName)
        {
            foreach (AudioClip clip in SanityRewrittenBase.auditoryHallucinations)
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
            int randomClip = UnityEngine.Random.Range(0, SanityRewrittenBase.stingers.Length);
            if (SanityRewrittenBase.stingers[randomClip])
            {
                return SanityRewrittenBase.stingers[randomClip];
            }
            return null;
        }
        private AudioClip LoadDroneSound()
        {
            int randomClip = UnityEngine.Random.Range(0, SanityRewrittenBase.drones.Length);
            if (SanityRewrittenBase.drones[randomClip])
            {
                return SanityRewrittenBase.drones[randomClip];
            }
            return null;
        }
        private AudioClip LoadStingerSound(string clipName)
        {
            foreach(AudioClip clip in SanityRewrittenBase.stingers)
            {
                if(clip.name == clipName)
                {
                    return clip;
                }
            }
            return null;
        }
    }

   
}
