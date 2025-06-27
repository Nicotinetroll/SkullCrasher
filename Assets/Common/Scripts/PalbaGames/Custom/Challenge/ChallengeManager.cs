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

        [SerializeField] private ChallengeUI challengeUI;

        private readonly List<BaseChallenge> activeChallenges = new();

        /// <summary>
        /// Currently active challenge (only one at a time).
        /// </summary>
        public BaseChallenge CurrentChallenge => activeChallenges.Count > 0 ? activeChallenges[0] : null;

        /// <summary>
        /// Last completed challenge (used for debug display).
        /// </summary>
        public BaseChallenge LastChallenge { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Update()
        {
            for (int i = activeChallenges.Count - 1; i >= 0; i--)
            {
                var challenge = activeChallenges[i];

                if (challenge.UpdateChallenge(Time.deltaTime))
                {
                    bool success = challenge.WasSuccessful;

                    LastChallenge = challenge;

                    var debugUI = FindObjectOfType<PlayerStatsDebugUI>();
                    debugUI?.ShowChallengeResult(success);

                    activeChallenges.RemoveAt(i);
                    challengeUI?.Hide();
                }
            }
        }

        /// <summary>
        /// Adds and starts a new runtime challenge.
        /// </summary>
        public void AddChallenge(BaseChallenge challenge, string description = null)
        {
            Debug.Log($"[ChallengeManager] Started challenge: {challenge.GetType().Name}");
            activeChallenges.Add(challenge);
            challenge.Start();

            challengeUI?.ShowChallenge(challenge, description ?? challenge.GetProgressString());
        }
    }
}
