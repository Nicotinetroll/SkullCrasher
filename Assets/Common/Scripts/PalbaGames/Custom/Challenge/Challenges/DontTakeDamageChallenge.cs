using System;
using OctoberStudio;
using UnityEngine;

namespace PalbaGames.Challenges
{
    /// <summary>
    /// Challenge: Don't take any damage for a set duration.
    /// </summary>
    public class DontTakeDamageChallenge : BaseChallenge
    {
        private float hpBefore;
        private Action onSuccess;
        private Action onFail;
        private HealthbarBehavior hpBar;

        public DontTakeDamageChallenge(float seconds, Action onSuccess, Action onFail)
        {
            this.timeLimit = seconds;
            this.onSuccess = onSuccess;
            this.onFail = onFail;
        }

        public override void Start()
        {
            timeRemaining = timeLimit;

            hpBar = PlayerBehavior.Player.GetComponentInChildren<HealthbarBehavior>();
            if (hpBar == null)
            {
                Debug.LogError("[Challenge] HealthbarBehavior not found!");
                WasSuccessful = false;
                onFail?.Invoke();
                return;
            }

            hpBefore = hpBar.HP;
            Debug.Log($"[Challenge] Started: Don't take damage for {timeLimit} seconds.");
        }

        public override bool UpdateChallenge(float deltaTime)
        {
            timeRemaining -= deltaTime;

            if (hpBar != null && hpBar.HP < hpBefore)
            {
                Debug.Log("[Challenge] FAILED. Player took damage.");
                WasSuccessful = false;
                onFail?.Invoke();
                return true;
            }

            if (timeRemaining <= 0f)
            {
                Debug.Log("[Challenge] SUCCESS! No damage taken.");
                WasSuccessful = true;
                onSuccess?.Invoke();
                return true;
            }

            return false;
        }

        public override string GetDisplayName() => "Avoid Damage";
        public override string GetProgressString()
        {
            return $"{timeLimit - timeRemaining:F1}/{timeLimit:F1} s SAFE";
        }
    }
}
