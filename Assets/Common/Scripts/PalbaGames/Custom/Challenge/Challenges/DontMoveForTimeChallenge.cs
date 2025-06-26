using System;
using OctoberStudio;
using UnityEngine;

namespace PalbaGames.Challenges
{
    public class DontMoveForTimeChallenge : BaseChallenge
    {
        private Action onSuccess;
        private Action onFail;
        private Vector3 lastPosition;
        private float movedTimer;

        public DontMoveForTimeChallenge(float seconds, Action onSuccess, Action onFail)
        {
            this.timeLimit = seconds;
            this.onSuccess = onSuccess;
            this.onFail = onFail;
        }

        public override void Start()
        {
            timeRemaining = timeLimit;
            lastPosition = PlayerBehavior.Player.transform.position;
            movedTimer = 0f;
            Debug.Log($"[Challenge] Started: Don't move for {timeLimit} seconds.");
        }

        public override bool UpdateChallenge(float deltaTime)
        {
            timeRemaining -= deltaTime;

            float movedDistance = Vector3.Distance(lastPosition, PlayerBehavior.Player.transform.position);
            if (movedDistance > 0.1f)
            {
                Debug.Log("[Challenge] FAILED. Player moved.");
                onFail?.Invoke();
                return true;
            }

            if (timeRemaining <= 0f)
            {
                Debug.Log("[Challenge] SUCCESS! Completed without moving.");
                onSuccess?.Invoke();
                return true;
            }

            return false;
        }
    }
}