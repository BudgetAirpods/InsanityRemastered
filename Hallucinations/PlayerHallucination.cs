using GameNetcodeStuff;
using InsanityRemastered.General;
using InsanityRemastered.ModIntegration;
using InsanityRemastered.Patches;
using InsanityRemasteredMod;
using InsanityRemasteredMod.General;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace InsanityRemastered.Hallucinations
{
    internal class PlayerHallucination : InsanityRemastered_AI
    {

        private float stareTimer = 5;
        private float waitTimeForNewWander = 5;
        private float speakTimer = 3;
        private float wanderTimer;
        private float stareDuration;
        private float speakDuration;

        private float rotationSpeed = 0.95f;
        private float footstepDistance = 1.5f;

        private int minWanderPoints = 3;
        private int maxWanderPoints = 5;
        private int currentFootstepSurfaceIndex;
        private List<Vector3> wanderPositions = new List<Vector3>();
        private Vector3 lastStepPosition;
        private AudioSource footstepSource;
        private bool spoken;
        private bool seenPlayer;
        public static SkinnedMeshRenderer suitRenderer;



        private void StopAndStare()
        {
            sfxSource.Stop();
            seenPlayer = true;
            hallucinationAnimator.SetBool(AnimationID.PlayerWalking, false);
            agent.isStopped = true;
            Stare(localPlayer.transform.position);
            if (SkinwalkerModIntegration.IsInstalled && InsanityGameManager.AreThereOtherPlayers && !spoken)
            {
                sfxSource.PlayOneShot(SkinwalkerModIntegration.GetRandomClip());
                spoken = true;
            }
            stareDuration += Time.deltaTime;
            if (stareDuration > stareTimer)
            {
                agent.isStopped = false;
                hallucinationAnimator.enabled = true;
                hallucinationAnimator.SetBool(AnimationID.PlayerWalking, true);
                hallucinationType = HallucinationType.Approaching;
            }
        }
        private void Stare(Vector3 position)
        {
            Quaternion lookRotation = Quaternion.LookRotation(position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
        private void GenerateNewDestination()
        {
            hallucinationAnimator.SetBool(AnimationID.PlayerWalking, true);

            Vector3 randomPosition = wanderPositions[Random.Range(0, wanderPositions.Count)];
            wanderPositions.Remove(randomPosition);
            agent.SetDestination(randomPosition);
            agent.isStopped = false;
            wanderSpot = true;
        }
        private void ReachDestination()
        {
            if (wanderPositions.Count == 0)
            {
                FinishHallucination(false);
            }
            hallucinationAnimator.SetBool(AnimationID.PlayerWalking, false);
            agent.isStopped = true;
            wanderTimer += Time.deltaTime;
            if (wanderTimer > waitTimeForNewWander)
            {
                wanderTimer = 0;
                wanderSpot = false;
            }
        }
        private void GenerateWanderPoints()
        {
            if (wanderPositions.Count > 0)
            {
                wanderPositions.Clear();
            }
            int randomWanderPointAmount = Random.Range(minWanderPoints, maxWanderPoints);
            for (int i = 0; i < randomWanderPointAmount; i++)
            {
                wanderPositions.Add(RoundManager.Instance.GetRandomNavMeshPositionInRadius(transform.position, 20));
            }
        }
        private void PlayFootstepSound()
        {
            GetCurrentMaterialStandingOn();
            int num = Random.Range(0, StartOfRound.Instance.footstepSurfaces[currentFootstepSurfaceIndex].clips.Length);

            footstepSource.pitch = Random.Range(0.93f, 1.07f);

            footstepSource.PlayOneShot(StartOfRound.Instance.footstepSurfaces[currentFootstepSurfaceIndex].clips[num], 5.5f);
        }
        private void GetCurrentMaterialStandingOn()
        {
            Ray materialRay = new Ray(transform.position + Vector3.up, -Vector3.up);
            RaycastHit hit;
            if (!Physics.Raycast(materialRay, out hit, 6f, StartOfRound.Instance.walkableSurfacesMask, QueryTriggerInteraction.Ignore) || hit.collider.CompareTag(StartOfRound.Instance.footstepSurfaces[currentFootstepSurfaceIndex].surfaceTag))
            {
                return;
            }
            for (int i = 0; i < StartOfRound.Instance.footstepSurfaces.Length; i++)
            {
                if (hit.collider.CompareTag(StartOfRound.Instance.footstepSurfaces[i].surfaceTag))
                {
                    currentFootstepSurfaceIndex = i;
                    break;
                }
            }
        }
        private void Footstep()
        {
            if (Vector3.Distance(transform.position, lastStepPosition) > footstepDistance)
            {
                lastStepPosition = transform.position;
                PlayFootstepSound();
            }
        }
        private int GetRandomPlayerSuitID()
        {
            PlayerControllerB randomPlayer = StartOfRound.Instance.allPlayerScripts[Random.Range(0, StartOfRound.Instance.allPlayerScripts.Length)];
            if (randomPlayer.isPlayerControlled)
            {
                return randomPlayer.currentSuitID;
            }
            else
            {
                GetRandomPlayerSuitID();
            }
            return localPlayer.currentSuitID;
        }
        private void SetSuit(int id)
        {

            Material suitMaterial = StartOfRound.Instance.unlockablesList.unlockables[id].suitMaterial;
            suitRenderer.material = suitMaterial;
        }
        private void Speak()
        {
            speakDuration += Time.deltaTime;
            if (speakDuration > speakTimer)
            {
                speakDuration = 0;
                sfxSource.PlayOneShot(SkinwalkerModIntegration.GetRandomClip());
            }
        }


        public override void Start()
        {
            base.Start();
            SFX = InsanityRemasteredContent.PlayerHallucinationSounds;
            hallucinationAnimator.runtimeAnimatorController = StartOfRound.Instance.localClientAnimatorController;
            footstepSource = gameObject.AddComponent<AudioSource>();
            footstepSource.spatialBlend = 1;
        }
        public override void Update()
        {
            base.Update();
            if (hallucinationType == HallucinationType.Wandering)
            {
                if (HasLineOfSightToPosition(transform, localPlayer.transform.position, 45, 45) || Vector3.Distance(transform.position, localPlayer.transform.position) < 3 || seenPlayer)
                {
                    StopAndStare();
                }
                else
                {
                    Wander();
                }
            }
            else if (hallucinationType == HallucinationType.Approaching)
            {
                ChasePlayer();
            }
            if (hallucinationType == HallucinationType.Staring)
            {
                Stare(localPlayer.transform.position);
                TimerTick();
            }
            Footstep();
            if (SkinwalkerModIntegration.IsInstalled && InsanityGameManager.AreThereOtherPlayers)
            {
                Speak();
            }
        }
        public override void LookAtHallucinationFirstTime()
        {
            base.LookAtHallucinationFirstTime();
            //InsanitySoundManager.Instance.PlayStinger();
        }
        public override void FinishHallucination(bool touched)
        {
            if (touched)
            {
                float effect = UnityEngine.Random.Range(0f, 1f);
                if (effect <= 0.15f)
                {
                    localPlayer.DamagePlayer(UnityEngine.Random.Range(1, 5), false, true, CauseOfDeath.Mauling);
                    return;
                }
                else if (effect <= 0.45f)
                {
                    HallucinationManager.Instance.PanicAttack = true;
                }
                base.FinishHallucination(true);
            }
            else
            {
                base.FinishHallucination(false);
            }
        }
        public override void Wander()
        {
            /*if(poi == null || poi == Vector3.zero)
            {
                poi = GeneratePointOfInterest();
            }*/
            if (wanderPositions.Count != 0 && !wanderSpot)
            {
                GenerateNewDestination();
            }
            if (Vector3.Distance(transform.position, agent.destination) <= agent.stoppingDistance && wanderSpot)
            {
                ReachDestination();
            }
            // LookAtPointOfInterest();
        }

        public override void Spawn()
        {

            base.Spawn();
            seenPlayer = false;
            SetSuit(GetRandomPlayerSuitID());
            hallucinationType = HallucinationType.Staring;
            if (PlayerPatcher.CurrentSanityLevel >= SanityLevel.Medium)
            {
                GenerateWanderPoints();
                hallucinationType = HallucinationType.Wandering;
            }
            float skinWalkerChance = Random.Range(0f, 1f);
            float messageChance = Random.Range(0f, 1f);
            if (skinWalkerChance >= 0.5f && SkinwalkerModIntegration.IsInstalled && InsanityGameManager.AreThereOtherPlayers)
            {
                sfxSource.clip = SkinwalkerModIntegration.GetRandomClip();
            }
            else
            {
                sfxSource.clip = InsanitySoundManager.Instance.LoadFakePlayerSound();
            }
            if (messageChance > 0.25f)
            {
                HUDManager.Instance.DisplayTip("", InsanityRemasteredConfiguration.hallucinationTipTexts[0], true);
            }
            spoken = false;
            sfxSource.Play();
            hallucinationAnimator.SetBool(AnimationID.PlayerWalking, false);
            stareDuration = 0;

        }
        public override void SetupVariables()
        {
            base.SetupVariables();
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            suitRenderer = GetComponentInChildren<SkinnedMeshRenderer>(false);
            Duration = 30;
            hallucinationAnimator.runtimeAnimatorController = StartOfRound.Instance.localClientAnimatorController;
            hallucinationSpawnType = HallucinationSpawnType.NotLooking; ;
            lastStepPosition = transform.position;
        }
    }

}
