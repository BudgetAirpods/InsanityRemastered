using GameNetcodeStuff;
using InsanityRemastered.General;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace InsanityRemastered.Hallucinations
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
        protected bool wanderSpot = false;
        private bool setup = false;
        public HallucinationType hallucinationType;
        public HallucinationSpawnType hallucinationSpawnType = HallucinationSpawnType.NotLooking;
        /// <summary>
        /// How long the hallucination should last for before despawning.
        /// </summary>
        protected float Duration { get { return duration; } set { duration = value; } }
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
                OnFailedToSpawn?.Invoke();
                PoolForLater();
            }
        }
        public virtual bool HasLineOfSightToPosition(Transform eye, Vector3 pos, float width = 45f, int range = 60, float proximityAwareness = -1f)
        {
            if (Vector3.Distance(eye.position, pos) < range && !Physics.Linecast(eye.position, pos, StartOfRound.Instance.collidersAndRoomMaskAndDefault))
            {
                Vector3 to = pos - eye.position;
                if (Vector3.Angle(eye.forward, to) < width || Vector3.Distance(transform.position, pos) < proximityAwareness)
                {
                    return true;
                }
            }
            return false;
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
                    InsanitySoundManager.Instance.PlayJumpscare();
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
        public virtual void TimerTick()
        {
            durationTimer += Time.deltaTime;
            if (durationTimer > duration)
            {
                durationTimer = 0;
                FinishHallucination(false);
            }
        }
        public virtual void ChasePlayer()
        {
            TimerTick();
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

        private void LoadAINodes()
        {
            /*if (localPlayer.isInsideFactory)
            {
                aiNodes = GameObject.FindGameObjectsWithTag("AINode");
            }
            else
            {
                aiNodes = GameObject.FindGameObjectsWithTag("OutsideAINode");
            }*/
        }
        public virtual void Update()
        {

            if (localPlayer.HasLineOfSightToPosition(transform.position))
            {
                if (lookedAtFirstTime)
                {
                    LookAtHallucinationFirstTime();
                }
                LookingAtHallucination();
            }
        }
        public virtual void SetupVariables()
        {
            if (!setup)
            {
                aiNodes = GameObject.FindGameObjectsWithTag("AINode");
                agent = GetComponent<NavMeshAgent>();
                hallucinationAnimator = GetComponentInChildren<Animator>();
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.spatialBlend = 1;

                agent.angularSpeed = Mathf.Infinity;
                agent.speed = 3f;
                agent.stoppingDistance = agentStoppingDistance;
                setup = true;
                agent.areaMask = StartOfRound.Instance.walkableSurfacesMask;
            }
        }
        private Vector3 FindSpawnPosition()
        {
            if (hallucinationSpawnType == HallucinationSpawnType.NotLooking)
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
            }
            else
            {
                Spawn();
            }
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
