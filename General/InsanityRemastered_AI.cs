using GameNetcodeStuff;
using SanityRewrittenMod.Hallucinations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AI;
using UnityEngine;
using SanityRewrittenMod.Utilities;

namespace SanityRewrittenMod.General
{
    internal class InsanityRemastered_AI : MonoBehaviour
    {
        /// <summary>
        /// Called when the hallucination fails to find a proper spawning point.
        /// </summary>
        public static event Action OnFailedToSpawn;
        /// <summary>
        /// When the hallucination ends, this is fired. The bool specifies whether the player was touched at all during the chasing type.
        /// </summary>
        public static event Action<bool> OnHallucinationEnded;
        protected PlayerControllerB localPlayer => GameNetworkManager.Instance.localPlayerController;
        
        private float duration = 30f;
        private float agentStoppingDistance = 3f;
        private float durationTimer;
        private bool lookedAtFirstTime = true;
        private bool wanderSpot = false;
        private bool setup = false;
        public HallucinationType hallucinationType;
        public HallucinationSpawnType hallucinationSpawnType = HallucinationSpawnType.NotLooking;
        /// <summary>
        /// How long the hallucination should last for before despawning.
        /// </summary>
        protected float Duration { get { return duration; } set { duration = value; } }
        protected AudioSource SFXSource { get { return sfxSource; } }
        protected AudioClip[] SFX { get { return sfx; } set { sfx = value; } }
        protected AudioSource sfxSource;
        protected AudioClip[] sfx;
        protected GameObject[] aiNodes;
        protected Animator hallucinationAnimator;
        protected NavMeshAgent agent;
        public virtual void Start()
        {
            SetupVariables();
        }
        public virtual void Spawn()
        {
            LoadAINodes();
            Vector3 spawnPosition = FindSpawnPosition();
            if (spawnPosition != Vector3.zero || spawnPosition != null)
            {
                transform.position = spawnPosition;
                wanderSpot = false;
                lookedAtFirstTime = true;
                agent.enabled = true;
            }
            else
            {
                BunkerHallucinations.PlaySound();
                OnFailedToSpawn?.Invoke();
                PoolForLater();
            }
        }
        public virtual void LookingAtHallucination()
        {

        }
        public virtual void LookAtHallucinationFirstTime()
        {
            lookedAtFirstTime = false;
            localPlayer.JumpToFearLevel(0.4f);     
        }
        public virtual void FinishHallucination(bool touched)
        {
            if (touched)
            {
                float scareRNG = UnityEngine.Random.Range(0f, 1f);

                if (scareRNG <= 0.15f)
                {
                    SanitySoundManager.Instance.PlayJumpscare();
                }
                OnHallucinationEnded?.Invoke(touched);
            }
            else if (!touched)
            {
                OnHallucinationEnded?.Invoke(touched);
            }
            PoolForLater();

        }
        public virtual void Wander()
        {
            if (!wanderSpot)
            {
                agent.SetDestination(RoundManager.Instance.GetRandomNavMeshPositionInRadius(aiNodes[UnityEngine.Random.Range(0, aiNodes.Length)].transform.position, 12)); ;
                wanderSpot = true;
            }
            if (Vector3.Distance(transform.position, agent.destination) <= agentStoppingDistance && wanderSpot)
            {
                PoolForLater();
                OnHallucinationEnded?.Invoke(false);
            }
        }
        public virtual void ChasePlayer()
        {
            if (Vector3.Distance(transform.position, localPlayer.transform.position) <= agentStoppingDistance)
            {
                FinishHallucination(true);
            }
            agent.SetDestination(localPlayer.transform.position);
        }
        public virtual void PoolForLater()
        {
            agent.enabled = false;
            transform.position = Vector3.zero;
            gameObject.SetActive(false);
        }
        
        private void Awake()
        {
            GameEvents.OnShipLanded += LoadAINodes;
        }
        private void LoadAINodes()
        {
            if (localPlayer.isInsideFactory)
            {
                aiNodes = GameObject.FindGameObjectsWithTag("AINode");
            }
            else
            {
                aiNodes = GameObject.FindGameObjectsWithTag("OutsideAINode");
            }
        }
        public virtual void Update()
        {
            durationTimer += Time.deltaTime;
            if (durationTimer > duration)
            {
                durationTimer = 0;
                FinishHallucination(false);
            }
            //If player can see the fake model,
            if (localPlayer.HasLineOfSightToPosition(transform.position))
            {
                if (lookedAtFirstTime)
                {
                    LookAtHallucinationFirstTime();
                }
                LookingAtHallucination();
            }
            if (hallucinationType == HallucinationType.Approaching)
            {
                ChasePlayer();
            }
            if (hallucinationType == HallucinationType.Wandering)
            {
                Wander();
            }
            
        }
        public virtual void SetupVariables()
        {
            if (!setup)
            {
                agent = GetComponent<NavMeshAgent>();
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.spatialBlend = 1;

                agent.angularSpeed = Mathf.Infinity;
                agent.speed = 3f;
                agent.stoppingDistance = agentStoppingDistance;
                setup = true;
            }
        }
        private Vector3 FindSpawnPosition()
        {
            if(hallucinationSpawnType == HallucinationSpawnType.NotLooking) 
            {
                for (int i = 0; i < aiNodes.Length; i++)
                {
                    if ((!Physics.Linecast(localPlayer.gameplayCamera.transform.position, aiNodes[i].transform.position, StartOfRound.Instance.collidersAndRoomMaskAndDefault)) && !localPlayer.HasLineOfSightToPosition(aiNodes[i].transform.position, 45f, 20, 8f))
                    {
                        return aiNodes[i].transform.position;
                    }
                }
            }
            return Vector3.zero;
        }

        private void OnEnable()
        {
            if (!setup)
            {
                SetupVariables();
                setup = true;
                sfx = InsanityRemasteredBase.playerHallucinationSounds;
            }
            Spawn();
        }
    }
    public enum HallucinationSpawnType
    {
        /// <summary>
        /// Spawns in an area the player cannot see.
        /// </summary>
        NotLooking,
        /// <summary>
        /// Spawns in an area the player can see.
        /// </summary>
        Visible,

    }
}
