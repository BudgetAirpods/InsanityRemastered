using SanityRewrittenMod.Patches;
using Unity;
using UnityEngine;

namespace SanityRewrittenMod
{
    internal class FakeItem : MonoBehaviour
    {
        private float stayTimer = 50f;
        private void Update()
        {
            stayTimer -= Time.deltaTime;
            if(stayTimer <= 0) 
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
            else if (action <= 0.01)
            {
                PlayerPatcher.LocalPlayer.KillPlayer(Vector3.zero);
            }
            SanitySoundManager.Instance.PlayStinger(true);
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
