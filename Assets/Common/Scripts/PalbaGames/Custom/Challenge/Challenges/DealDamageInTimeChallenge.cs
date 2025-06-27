using System;
using UnityEngine;

namespace PalbaGames.Challenges
{
    /// <summary>
    /// Challenge: Deal specific amount of damage within a time limit.
    /// </summary>
    public class DealDamageInTimeChallenge : BaseChallenge
    {
        private float damageRequired;
        private float damageBefore;
        private Action onSuccess;
        private Action onFail;

        private float lastLoggedSecond = -1;

        public DealDamageInTimeChallenge(float damage, float seconds, Action onSuccess, Action onFail)
        {
            this.damageRequired = damage;
            this.timeLimit = seconds;
            this.onSuccess = onSuccess;
            this.onFail = onFail;
        }

        public override void Start()
        {
            timeRemaining = timeLimit;
            damageBefore = DamageTracker.Instance.TotalDamage;
            Debug.Log($"[Challenge] Started: Deal {damageRequired} damage in {timeLimit} seconds (current total: {damageBefore})");
        }

        public override bool UpdateChallenge(float deltaTime)
        {
            timeRemaining -= deltaTime;

            float currentDamage = DamageTracker.Instance.TotalDamage - damageBefore;

            float floored = Mathf.Floor(timeRemaining);
            if (floored != lastLoggedSecond)
            {
                Debug.Log($"[Challenge] Progress: {currentDamage}/{damageRequired} dmg | {floored} sec left");
                lastLoggedSecond = floored;
            }

            if (currentDamage >= damageRequired)
            {
                Debug.Log("[Challenge] SUCCESS! Challenge completed.");
                WasSuccessful = true;
                onSuccess?.Invoke();
                return true;
            }

            if (timeRemaining <= 0f)
            {
                Debug.Log("[Challenge] FAILED. Time ran out.");
                WasSuccessful = false;
                onFail?.Invoke();
                return true;
            }

            return false;
        }

        public override string GetDisplayName() => "Deal Damage";
        public override string GetProgressString()
        {
            float currentDamage = DamageTracker.Instance.TotalDamage - damageBefore;
            return $"{currentDamage:F0}/{damageRequired:F0} DMG";
        }
    }
}
