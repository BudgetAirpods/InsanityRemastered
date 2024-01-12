using BepInEx;
using SanityRewrittenMod;
using UnityEngine;
using UnityEngine.Networking;

using System.IO;

using System;
using System.Collections.Generic;
using System.Collections;
using SanityRewrittenMod.Utilities;


namespace InsanityRemasteredMod.Mod_Integrations
{
    internal  class SkinwalkerModIntegration
    {
        private static List<AudioClip> skinwalkerClips = new List<AudioClip>();
        public static bool IsInstalled { get; private set; }
        private static void UpdateClips(ref List<AudioClip> ___cachedAudio)
        {
            skinwalkerClips = ___cachedAudio;
            IsInstalled = true;
        }
        public static AudioClip GetRandomClip()
        {
            return skinwalkerClips[UnityEngine.Random.Range(0, SkinwalkerModIntegration.skinwalkerClips.Count)];
        }
    }
}
