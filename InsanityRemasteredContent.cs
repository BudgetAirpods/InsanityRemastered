using BepInEx;
using System.IO;
using UnityEngine;

namespace InsanityRemasteredMod
{
    internal class InsanityRemasteredContent
    {
        internal static Material[] Materials { get; set; }
        internal static GameObject[] EnemyModels { get; set; }
        internal static Texture2D[] Textures { get; set; }

        internal static AudioClip[] AuditoryHallucinations { get; set; }
        internal static AudioClip[] Stingers { get; set; }
        internal static AudioClip[] PlayerHallucinationSounds { get; set; }
        internal static AudioClip[] LCGameSFX { get; set; }
        internal static AudioClip[] Drones { get; set; }

        private static string DataFolder => Path.GetFullPath(Paths.PluginPath + "/InsanityRemastered");

        public static void LoadContent()
        {
            //LoadEnemy();
            //LoadMaterials();
            LoadSounds();
        }

        public static GameObject GetEnemyModel(string name)
        {
            for (int i = 0; i < EnemyModels.Length; i++)
            {
                if (EnemyModels[i].name == name)
                {
                    return EnemyModels[i];
                }
            }
            return null;
        }
        public static Material GetMaterial(string name)
        {
            for (int i = 0; i < EnemyModels.Length; i++)
            {
                if (Materials[i].name == name)
                {
                    InsanityRemasteredLogger.Log("Sucessfully loaded material: " + name);
                    return Materials[i];
                }
            }
            return null;
        }
        public static Texture2D GetTexture(string name)
        {
            for (int i = 0; i < EnemyModels.Length; i++)
            {
                if (Textures[i].name == name)
                {
                    InsanityRemasteredLogger.Log("Sucessfully loaded texture: " + name);
                    return Textures[i];
                }
            }
            return null;
        }

        private static void LoadEnemy()
        {
            string enemyBundle = Path.Combine(DataFolder, "insanityremastered_enemies");
            InsanityRemasteredLogger.Log(enemyBundle);
            AssetBundle enemies = AssetBundle.LoadFromFile(enemyBundle);
            if (enemies == null)
            {
                InsanityRemasteredLogger.Log("Failed to load enemies.");
            }

            EnemyModels = enemies.LoadAllAssets<GameObject>();
        }
        private static void LoadMaterials()
        {
            string materialBundle = Path.Combine(DataFolder, "insanityremastered_materials");

            AssetBundle materials = AssetBundle.LoadFromFile(materialBundle);

            if (materials == null)
            {
                InsanityRemasteredLogger.Log("Failed to load materials.");
            }

            Materials = materials.LoadAllAssets<Material>();
            Textures = materials.LoadAllAssets<Texture2D>();
            EnemyModels = materials.LoadAllAssets<GameObject>();

        }
        private static void LoadSounds()
        {

            string sfxBundle = Path.Combine(DataFolder, "soundresources_sfx");
            string ambientBundle = Path.Combine(DataFolder, "soundresources_stingers");
            string fakePlayerBundle = Path.Combine(DataFolder, "soundresources_hallucination");
            string droneBundle = Path.Combine(DataFolder, "soundresources_drones");
            string lcGameBundle = Path.Combine(DataFolder, "soundresources_lc");

            AssetBundle sfx = AssetBundle.LoadFromFile(sfxBundle);
            AssetBundle ambience = AssetBundle.LoadFromFile(ambientBundle);
            AssetBundle fakePlayer = AssetBundle.LoadFromFile(fakePlayerBundle);
            AssetBundle drone = AssetBundle.LoadFromFile(droneBundle);
            AssetBundle lcGame = AssetBundle.LoadFromFile(lcGameBundle);
            if (sfx is null || ambience is null | fakePlayer is null || droneBundle is null || lcGameBundle is null)
            {
                InsanityRemasteredLogger.LogError("Failed to load audio assets!");
                return;
            }
            AuditoryHallucinations = sfx.LoadAllAssets<AudioClip>();
            Stingers = ambience.LoadAllAssets<AudioClip>();
            PlayerHallucinationSounds = fakePlayer.LoadAllAssets<AudioClip>();
            Drones = drone.LoadAllAssets<AudioClip>();
            LCGameSFX = lcGame.LoadAllAssets<AudioClip>();
        }
    }
}
