using System.Collections.Generic;
using UnityEngine;

namespace PalbaGames.Challenges
{
    /// <summary>
    /// Handles all runtime challenges triggered from Timeline.
    /// </summary>
    public class ChallengeManager : MonoBehaviour
    {
        public static ChallengeManager Instance { get; private set; }

        private readonly List<BaseChallenge> activeChallenges = new();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Update()
        {
            for (int i = activeChallenges.Count - 1; i >= 0; i--)
            {
                if (activeChallenges[i].UpdateChallenge(Time.deltaTime))
                    activeChallenges.RemoveAt(i);
            }
        }

        /// <summary>
        /// Adds and starts a new runtime challenge.
        /// </summary>
        public void AddChallenge(BaseChallenge challenge)
        {
            Debug.Log($"[ChallengeManager] Started challenge: {challenge.GetType().Name}");
            activeChallenges.Add(challenge);
            challenge.Start();
        }

        /// <summary>
        /// LEGACY HARDCODED MODE – DEPRECATED.
        /// Left here only as fallback.
        /// </summary>
        public void TriggerChallenge(string challengeId)
        {
            if (challengeId == "Kill10In5s")
            {
                AddChallenge(new KillEnemiesInTimeChallenge(
                    kills: 10,
                    seconds: 5f,
                    onSuccess: () =>
                    {
                        Debug.Log("[ChallengeManager] Reward: DamageBoost");
                        BuffManager.Instance.ApplyBuff(BuffType.DamageBoost, 10f);
                    },
                    onFail: () =>
                    {
                        Debug.Log("[ChallengeManager] Challenge failed – no reward.");
                    }
                ));
            }
            else
            {
                Debug.LogWarning($"[ChallengeManager] Unknown challengeId: {challengeId}");
            }
        }
    }
}
