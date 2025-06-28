using System.Collections;
using UnityEngine;
using OctoberStudio;

namespace PalbaGames.Challenges
{
    /// <summary>
    /// Manages temporary penalties applied to the player.
    /// Handles duration-based effects without modifying original assets.
    /// </summary>
    public class PenaltyManager : MonoBehaviour
    {
        public static PenaltyManager Instance { get; private set; }

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = true;

        private PlayerStatModifier playerModifier;
        private Coroutine activePenaltyCoroutine;

        /// <summary>
        /// Currently active penalty type.
        /// </summary>
        public PenaltyType ActivePenalty { get; private set; } = PenaltyType.None;

        /// <summary>
        /// Remaining time for current penalty.
        /// </summary>
        public float RemainingTime { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            // Get or create PlayerStatModifier
            playerModifier = PlayerBehavior.Player?.GetComponent<PlayerStatModifier>();
            if (playerModifier == null && PlayerBehavior.Player != null)
            {
                playerModifier = PlayerBehavior.Player.gameObject.AddComponent<PlayerStatModifier>();
            }

            if (playerModifier == null)
            {
                Debug.LogError("[PenaltyManager] PlayerStatModifier not found and could not be created!");
            }
        }

        /// <summary>
        /// Apply a penalty with specified duration.
        /// </summary>
        public void ApplyPenalty(PenaltyType type, float duration = 0f)
        {
            if (type == PenaltyType.None) return;

            // Cancel existing penalty first
            if (activePenaltyCoroutine != null)
            {
                StopCoroutine(activePenaltyCoroutine);
                RemoveCurrentPenalty();
            }

            ActivePenalty = type;
            RemainingTime = duration;

            if (showDebugLogs)
                Debug.Log($"[PenaltyManager] Applying penalty: {type} for {duration} seconds");

            switch (type)
            {
                case PenaltyType.TakeDamage:
                    ApplyInstantDamage();
                    break;
                case PenaltyType.Slowdown:
                    ApplySlowdown(duration);
                    break;
                case PenaltyType.Curse:
                    ApplyCurse(duration);
                    break;
                case PenaltyType.LockAbilities:
                    ApplyZeroDamage(duration);
                    break;
            }
        }

        private void ApplyInstantDamage()
        {
            PlayerBehavior.Player?.TakeDamage(50);
            ActivePenalty = PenaltyType.None;
            RemainingTime = 0f;
        }

        private void ApplySlowdown(float duration)
        {
            if (playerModifier != null)
            {
                playerModifier.speedMultiplier = 0.5f;
                playerModifier.ApplyMultipliers();
            }

            activePenaltyCoroutine = StartCoroutine(RemovePenaltyAfterTime(duration));
        }

        private void ApplyCurse(float duration)
        {
            if (playerModifier != null)
            {
                playerModifier.xpMultiplier = 0.5f;
                playerModifier.goldMultiplier = 0.5f;
                playerModifier.ApplyMultipliers();
            }

            activePenaltyCoroutine = StartCoroutine(RemovePenaltyAfterTime(duration));
        }

        private void ApplyZeroDamage(float duration)
        {
            if (playerModifier != null)
            {
                playerModifier.damageMultiplier = 0f;
                playerModifier.ApplyMultipliers();
            }

            activePenaltyCoroutine = StartCoroutine(RemovePenaltyAfterTime(duration));
        }

        private IEnumerator RemovePenaltyAfterTime(float duration)
        {
            while (RemainingTime > 0f)
            {
                RemainingTime -= Time.deltaTime;
                yield return null;
            }

            RemoveCurrentPenalty();
        }

        /// <summary>
        /// Manually remove current penalty.
        /// </summary>
        public void RemoveCurrentPenalty()
        {
            if (ActivePenalty == PenaltyType.None) return;

            if (showDebugLogs)
                Debug.Log($"[PenaltyManager] Removing penalty: {ActivePenalty}");

            // Reset all multipliers to default
            if (playerModifier != null)
            {
                playerModifier.damageMultiplier = 1f;
                playerModifier.speedMultiplier = 1f;
                playerModifier.xpMultiplier = 1f;
                playerModifier.goldMultiplier = 1f;
                playerModifier.ApplyMultipliers();
            }

            ActivePenalty = PenaltyType.None;
            RemainingTime = 0f;
            activePenaltyCoroutine = null;
        }

        /// <summary>
        /// Get penalty description for UI display.
        /// </summary>
        public string GetPenaltyDescription()
        {
            return ActivePenalty switch
            {
                PenaltyType.Slowdown => "Movement speed reduced by 50%",
                PenaltyType.Curse => "XP and Gold reduced by 50%",
                PenaltyType.LockAbilities => "Damage disabled",
                _ => ""
            };
        }
    }
}