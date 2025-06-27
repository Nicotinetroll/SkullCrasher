using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PalbaGames.Challenges
{
    /// <summary>
    /// Manages challenges and coordinates with ChallengeUI
    /// </summary>
    public class ChallengeManager : MonoBehaviour
    {
        public static ChallengeManager Instance { get; private set; }

        [SerializeField] private ChallengeUI challengeUI;
        [SerializeField] private float preStartDelay = 4f; // Should match ChallengeUI introDisplayTime

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
            // Update active challenges
            for (int i = activeChallenges.Count - 1; i >= 0; i--)
            {
                var challenge = activeChallenges[i];

                if (challenge.UpdateChallenge(Time.deltaTime))
                {
                    // Challenge finished
                    OnChallengeCompleted(challenge);
                    activeChallenges.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Adds a new challenge with UI sequence
        /// </summary>
        public void AddChallenge(BaseChallenge challenge, string description = "")
        {
            DebugLogger.Log($"[ChallengeManager] Adding challenge: {challenge.GetType().Name}");
            
            // Start UI sequence immediately
            challengeUI?.StartChallengeSequence(challenge, description);
            
            // Start challenge logic after delay
            StartCoroutine(StartChallengeAfterDelay(challenge, preStartDelay));
        }

        private IEnumerator StartChallengeAfterDelay(BaseChallenge challenge, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            activeChallenges.Add(challenge);
            challenge.Start();
            DebugLogger.Log($"[ChallengeManager] Challenge started: {challenge.GetType().Name}");
        }

        private void OnChallengeCompleted(BaseChallenge challenge)
        {
            bool success = challenge.WasSuccessful;
            LastChallenge = challenge;

            DebugLogger.Log($"[ChallengeManager] Challenge completed: {challenge.GetType().Name}, Success: {success}");

            // Notify debug UI
            var debugUI = FindObjectOfType<PlayerStatsDebugUI>();
            debugUI?.ShowChallengeResult(success);
        }

        /// <summary>
        /// Legacy method for backwards compatibility
        /// </summary>
        public void ShowChallenge(BaseChallenge challenge)
        {
            AddChallenge(challenge, challenge.GetDisplayName());
        }

        /// <summary>
        /// Cancel current challenge
        /// </summary>
        public void CancelCurrentChallenge()
        {
            activeChallenges.Clear();
            challengeUI?.ForceHide();
            DebugLogger.Log("[ChallengeManager] Challenge cancelled");
        }
    }
}