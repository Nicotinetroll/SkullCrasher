using System;
using OctoberStudio;
using UnityEngine;

namespace PalbaGames.Challenges
{
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
                onFail?.Invoke();
                return;
            }

            hpBefore = hpBar.HP;

            Debug.Log($"[Challenge] Started: Don't take damage for {timeLimit} seconds.");
        }

        public override bool UpdateChallenge(float deltaTime)
        {
            timeRemaining -= deltaTime;

            if (hpBar.HP < hpBefore)
            {
                Debug.Log("[Challenge] FAILED. Player took damage.");
                onFail?.Invoke();
                return true;
            }

            if (timeRemaining <= 0f)
            {
                Debug.Log("[Challenge] SUCCESS! No damage taken.");
                onSuccess?.Invoke();
                return true;
            }

            return false;
        }
    }
}