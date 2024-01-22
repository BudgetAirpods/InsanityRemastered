﻿using InsanityRemastered.General;
using InsanityRemastered.Patches;
using UnityEngine;

namespace InsanityRemastered.Hallucinations
{
    internal class FakeItem : MonoBehaviour
    {
        private float stayTimer = 50f;
        private void Update()
        {
            stayTimer -= Time.deltaTime;
            if (stayTimer <= 0)
            {
                gameObject.SetActive(false);
            }
        }
        private void Interaction()
        {
            float action = Random.Range(0, 1);
            if (action <= 0.35f)
            {
                PlayerPatcher.LocalPlayer.DropBlood();
            }
            else if (action <= 0.0001)
            {
                PlayerPatcher.LocalPlayer.KillPlayer(Vector3.zero);
            }
            InsanitySoundManager.Instance.PlayStinger();
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            PlayerPatcher.OnInteractWithFakeItem += Interaction;
        }
        private void OnDisable()
        {
            PlayerPatcher.OnInteractWithFakeItem -= Interaction;

        }
    }
}
