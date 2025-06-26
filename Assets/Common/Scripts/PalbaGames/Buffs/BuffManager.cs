using UnityEngine;

namespace PalbaGames
{
    public class BuffManager : MonoBehaviour
    {
        public static BuffManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void ApplyBuff(BuffType type, float duration)
        {
            Debug.Log($"Buff applied: {type} for {duration} seconds");
        }
    }

    public enum BuffType
    {
        DamageBoost,
        SpeedBoost,
        CritChance
    }
}