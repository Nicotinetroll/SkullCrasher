using UnityEngine;

namespace PalbaGames
{
    /// <summary>
    /// Extends PlayerBehavior with Critical Strike logic.
    /// Attach to the same GameObject as PlayerBehavior.
    /// </summary>
    public class PlayerBehavior_Extended : MonoBehaviour
    {
        public static PlayerBehavior_Extended Instance { get; private set; }

        [Tooltip("Chance for a critical strike in % (0–100)")]
        public float criticalChance = 10f;

        [Tooltip("Critical damage multiplier (e.g., 2 = double damage)")]
        public float criticalMultiplier = 2f;

        private OctoberStudio.PlayerBehavior player;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            player = OctoberStudio.PlayerBehavior.Player;

            if (player == null)
            {
                player = FindObjectOfType<OctoberStudio.PlayerBehavior>();
                if (player == null)
                    Debug.LogError("[PlayerBehavior_Extended] PlayerBehavior still not found in Start().");
            }
        }

        /// <summary>
        /// Returns the damage with possible critical strike applied.
        /// </summary>
        public float GetFinalDamage()
        {
            if (player == null)
            {
                Debug.LogWarning("[PlayerBehavior_Extended] Player is null when calculating damage.");
                return 0f;
            }

            if (Random.value <= criticalChance / 100f)
                return player.Damage * criticalMultiplier;

            return player.Damage;
        }

        /// <summary>
        /// Returns calculated damage and whether it was critical.
        /// </summary>
        public float GetFinalDamage(float baseDamage, out bool isCritical)
        {
            isCritical = Random.value <= criticalChance / 100f;
            return isCritical ? baseDamage * criticalMultiplier : baseDamage;
        }
    }
}
