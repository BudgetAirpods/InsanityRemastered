using System.Collections.Generic;
using UnityEngine;

namespace InsanityRemastered.ModIntegration
{
    public class SkinwalkerModIntegration
    {

        private static List<AudioClip> skinwalkerClips = new List<AudioClip>();
        public static bool IsInstalled { get; set; }
        private static void UpdateClips(ref List<AudioClip> ___cachedAudio)
        {
            skinwalkerClips = ___cachedAudio;
        }
        public static AudioClip GetRandomClip()
        {
            return skinwalkerClips[Random.Range(0, skinwalkerClips.Count)];
        }
    }
}
