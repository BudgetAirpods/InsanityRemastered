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
    internal  class SkinwalkerModIntegration : MonoBehaviour
    {
        public static SkinwalkerModIntegration Instance { get; set; }
        public const string SkinwalkerModName = "Skinwalker Mod";
        public static bool IsInstalled { get; private set; }
        public List<AudioClip> SkinwalkerRecordings = new List<AudioClip>();
        private string SkinwalkerRecordingPath { get; set; }
        private string[] SkinwalkerFiles { get; set; }
        private float timeBetweenRemovingSounds;
        private void Awake()
        {
            GameEvents.OnGameEnd += ClearRecordings;
        }
        private void ClearRecordings()
        {
            if (Time.realtimeSinceStartup > timeBetweenRemovingSounds)
            {
                timeBetweenRemovingSounds = Time.realtimeSinceStartup + 9f;
                SkinwalkerRecordings.Clear();
            }
        }
        private void Update()
        {
            if(Time.realtimeSinceStartup > timeBetweenRemovingSounds)
            {
                ClearRecordings();
            }
        }
        public void SetupIntegration()
        {
            Instance = this;
            IsInstalled = true;
            SkinwalkerRecordingPath = Path.Combine(Application.dataPath, "..", "Dissonance_Diagnostics");
        }

        public AudioClip GetRandomRecording()
        {
            if (!Directory.Exists(SkinwalkerRecordingPath))
            {
                return null;
            }
            UpdateRecordings();
            if (SkinwalkerRecordings.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, SkinwalkerRecordings.Count - 1);
                AudioClip result = SkinwalkerRecordings[index];
                SkinwalkerRecordings.RemoveAt(index);
                return result;
            }
            while (SkinwalkerRecordings.Count > 200)
            {
                SkinwalkerRecordings.RemoveAt(0);
            }
            return null;
        }
        public void UpdateRecordings()
        {
            SkinwalkerFiles = Directory.GetFiles(SkinwalkerRecordingPath);
            foreach(string path in SkinwalkerFiles)
            {
                StartCoroutine(LoadWavFile(path, delegate(AudioClip clip) { SkinwalkerRecordings.Add(clip); }));
            }
        }
        private IEnumerator LoadWavFile(string path, Action<AudioClip> callback)
        {
            print("Trying to load a file.");
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, (AudioType)20);
            try
            {
                yield return www.SendWebRequest();
                if ((int)www.result == 1)
                {
                    print("Loaded a clip.");
                    AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                    if (audioClip.length > 0.3f)
                    {
                        print(audioClip.name);
                        callback(audioClip);
                    }
                    try
                    {
                        File.Delete(SkinwalkerRecordingPath);
                    }
                    catch (Exception e)
                    {
                        InsanityRemasteredBase.mls.LogWarning(e);
                    }
                }
            }
            finally
            {
                ((IDisposable)www)?.Dispose();
            }
        }
    }
}
