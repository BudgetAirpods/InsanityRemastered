using BepInEx;
using InsanityRemastered.General;
using InsanityRemastered.Patches;
using UnityEngine;

namespace InsanityRemasteredMod
{
    internal class InsanityRemasteredDebug
    {
        public static void QuickHotkeyTesting()
        {
            if (UnityInput.Current.GetKeyDown("f"))
            {
                //Debug stuff
                SpawnFakePlayer();
            }
            if (UnityInput.Current.GetKeyDown("v"))
            {
                //Debug stuff
                HallucinationManager.Instance.Hallucinate(HallucinationID.PowerLoss);
            }
        }
        public static void SpawnFakePlayer()
        {
            HallucinationManager.Instance.PanicAttackLevel = 1;
            PlayerPatcher.LocalPlayer.insanityLevel = 100f;
            HallucinationManager.Instance.Hallucinate(HallucinationID.FakePlayer);
        }
        public static void SpawnItem(string itemName)
        {
            // Get a list of all items
            foreach (Item item in StartOfRound.Instance.allItemsList.itemsList)
            {
                if (item.name == itemName)
                {
                    var prop = MonoBehaviour.Instantiate(
                    item.spawnPrefab,
                    PlayerPatcher.LocalPlayer.transform.position + PlayerPatcher.LocalPlayer.transform.forward * 2.0f,
                    PlayerPatcher.LocalPlayer.transform.rotation,
                    RoundManager.Instance.spawnedScrapContainer
                );
                    var grabbable = prop.GetComponent<GrabbableObject>();
                    grabbable.fallTime = 1.0f;
                    grabbable.scrapPersistedThroughRounds = false;
                    grabbable.grabbable = true;

                    // Check if it's scrap
                    if (item.isScrap)
                    {
                        // Set the scrap value
                        grabbable.SetScrapValue(
                            UnityEngine.Random.Range(item.minValue, item.maxValue)
                        );
                        grabbable.NetworkObject.Spawn(false);
                    }
                }
            }
        }
        public static void SpawnObserver()
        {
            HallucinationManager.Instance.Hallucinate(HallucinationID.Observer);
        }
    }
}
