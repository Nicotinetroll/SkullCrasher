using System;
using OctoberStudio;
using UnityEngine;

namespace PalbaGames.Challenges
{
    /// <summary>
    /// Challenge: Kill a number of enemies within a time limit.
    /// Uses StageSave.EnemiesKilled to track kills.
    /// </summary>
    public class KillXEnemiesInYSecondsChallenge : BaseChallenge
    {
        private int killsRequired;
        private int killsBefore;
        private Action onSuccess;

        private float lastLoggedSecond = -1;

        public KillXEnemiesInYSecondsChallenge(int kills, float seconds, Action onSuccess)
        {
            this.killsRequired = kills;
            this.timeLimit = seconds;
            this.onSuccess = onSuccess;
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
                onSuccess?.Invoke();
                return true;
            }

            if (timeRemaining <= 0f)
            {
                Debug.Log("[Challenge] FAILED. Time ran out.");
                return true;
            }

            return false;
        }
    }
}