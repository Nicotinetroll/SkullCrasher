using System;
using OctoberStudio;
using UnityEngine;

namespace PalbaGames.Challenges
{
    /// <summary>
    /// Challenge: Survive for a duration without dying.
    /// </summary>
    public class SurviveForTimeChallenge : BaseChallenge
    {
        private Action onSuccess;
        private Action onFail;

        public SurviveForTimeChallenge(float seconds, Action onSuccess, Action onFail)
        {
            this.timeLimit = seconds;
            this.onSuccess = onSuccess;
            this.onFail = onFail;
        }

        public override void Start()
        {
            timeRemaining = timeLimit;
            PlayerBehavior.Player.onPlayerDied += Fail;
            Debug.Log($"[Challenge] Started: Survive for {timeLimit} seconds.");
        }

        public override bool UpdateChallenge(float deltaTime)
        {
            timeRemaining -= deltaTime;

            if (timeRemaining <= 0f)
            {
                Debug.Log("[Challenge] SUCCESS! Survived.");
                WasSuccessful = true;
                PlayerBehavior.Player.onPlayerDied -= Fail;
                onSuccess?.Invoke();
                return true;
            }

            return false;
        }

        private void Fail()
        {
            Debug.Log("[Challenge] FAILED. Player died.");
            WasSuccessful = false;
            PlayerBehavior.Player.onPlayerDied -= Fail;
            onFail?.Invoke();
        }

        public override string GetDisplayName() => "Survive!";
        public override string GetProgressString()
        {
            return $"{timeLimit - timeRemaining:F1}/{timeLimit:F1} s";
        }
    }
}