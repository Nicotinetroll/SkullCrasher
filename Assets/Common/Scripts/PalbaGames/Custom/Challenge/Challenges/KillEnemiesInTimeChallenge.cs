using System;
using OctoberStudio;
using UnityEngine;

namespace PalbaGames.Challenges
{
    /// <summary>
    /// Challenge: Kill a number of enemies within a time limit.
    /// </summary>
    public class KillEnemiesInTimeChallenge : BaseChallenge
    {
        private int killsRequired;
        private int killsBefore;
        private Action onSuccess;
        private Action onFail;

        private float lastLoggedSecond = -1;

        public KillEnemiesInTimeChallenge(int kills, float seconds, Action onSuccess, Action onFail)
        {
            this.killsRequired = kills;
            this.timeLimit = seconds;
            this.onSuccess = onSuccess;
            this.onFail = onFail;
        }

        public override void Start()
        {
            timeRemaining = timeLimit;
            killsBefore = GameController.SaveManager.GetSave<StageSave>("Stage").EnemiesKilled;
            Debug.Log($"[Challenge] Started: Kill {killsRequired} enemies in {timeLimit} seconds (current total: {killsBefore})");
        }

        public override bool UpdateChallenge(float deltaTime)
        {
            timeRemaining -= deltaTime;

            int currentKills = GameController.SaveManager.GetSave<StageSave>("Stage").EnemiesKilled - killsBefore;

            float floored = Mathf.Floor(timeRemaining);
            if (floored != lastLoggedSecond)
            {
                Debug.Log($"[Challenge] Progress: {currentKills}/{killsRequired} kills | {floored} sec left");
                lastLoggedSecond = floored;
            }

            if (currentKills >= killsRequired)
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

        public override string GetDisplayName() => "Kill Enemies";

        public override string GetProgressString()
        {
            int currentKills = GameController.SaveManager.GetSave<StageSave>("Stage").EnemiesKilled - killsBefore;
            return $"{currentKills}/{killsRequired} KILLS";
        }

        public override float ProgressNormalized
        {
            get
            {
                int currentKills = GameController.SaveManager.GetSave<StageSave>("Stage").EnemiesKilled - killsBefore;
                return Mathf.Clamp01((float)currentKills / killsRequired);
            }
        }
    }
}
