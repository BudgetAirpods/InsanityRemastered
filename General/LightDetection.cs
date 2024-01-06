using System;
using UnityEngine;
using BepInEx;
using HarmonyLib;
using SanityRewrittenMod.Patches;
namespace SanityRewrittenMod
{
    internal class LightDetection : MonoBehaviour
    {
        //public static event Action OnEnterLightProximity;
        //public static event Action OnLeaveLightProximity;

        private Transform localPlayer;
        private float distanceThreshold = 9.2f;
        public void Start()
        {
            localPlayer = PlayerPatcher.LocalPlayer.transform;
        }
        private void Update()
        {
            if(Vector3.Distance(transform.position, localPlayer.position) < distanceThreshold)
            {
                SanityMainManager.Instance.IsNearLightSource = true;
                return;
            }
            else
            {
                SanityMainManager.Instance.IsNearLightSource = false;
            }
        }
    }
}
