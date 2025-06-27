using System;
using OctoberStudio;
using UnityEngine;

namespace PalbaGames.Challenges
{
    /// <summary>
    /// Challenge: Stay still for a specific duration without moving.
    /// </summary>
    public class DontMoveForTimeChallenge : BaseChallenge
    {
        private Action onSuccess;
        private Action onFail;
        private Vector3 lastPosition;

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
            Debug.Log($"[Challenge] Started: Don't move for {timeLimit} seconds.");
        }

        public override bool UpdateChallenge(float deltaTime)
        {
            timeRemaining -= deltaTime;

            float movedDistance = Vector3.Distance(lastPosition, PlayerBehavior.Player.transform.position);
            if (movedDistance > 0.1f)
            {
                Debug.Log("[Challenge] FAILED. Player moved.");
                WasSuccessful = false;
                onFail?.Invoke();
                return true;
            }

            if (timeRemaining <= 0f)
            {
                Debug.Log("[Challenge] SUCCESS! Completed without moving.");
                WasSuccessful = true;
                onSuccess?.Invoke();
                return true;
            }

            return false;
        }

        public override string GetDisplayName() => "Don't Move";
        public override string GetProgressString()
        {
            return $"{timeLimit - timeRemaining:F1}/{timeLimit:F1} s STILL";
        }
    }
}