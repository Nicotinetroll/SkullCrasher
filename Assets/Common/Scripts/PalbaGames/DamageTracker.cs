using System.Collections.Generic;
using UnityEngine;
using OctoberStudio;

namespace PalbaGames
{
    /// <summary>
    /// Tracks total damage dealt per ability during a run.
    /// </summary>
    public class DamageTracker : MonoBehaviour
    {
        public static DamageTracker Instance { get; private set; }

        public Dictionary<AbilityType, float> DamageByAbility { get; private set; } = new();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Call when starting a new run.
        /// </summary>
        public void ResetStats()
        {
            DamageByAbility.Clear();
        }

        /// <summary>
        /// Call whenever an enemy takes damage from an ability.
        /// </summary>
        public void ReportDamage(AbilityType type, float amount)
        {
            if (!DamageByAbility.ContainsKey(type))
                DamageByAbility[type] = 0;

            DamageByAbility[type] += amount;
        }

        /// <summary>
        /// Print all damage results to the console.
        /// </summary>
        public void PrintDamageReport()
        {
            float total = 0f;
            Debug.Log("🔥 DAMAGE REPORT 🔥");

            foreach (var kvp in DamageByAbility)
            {
                Debug.Log($"{kvp.Key}: {kvp.Value} dmg");
                total += kvp.Value;
            }

            Debug.Log($"TOTAL: {total} dmg");
        }
        
        public float TotalDamage
        {
            get
            {
                float total = 0f;
                foreach (var dmg in DamageByAbility.Values)
                {
                    total += dmg;
                }

                Debug.Log($"[DamageTracker] TotalDamage = {total}");
                return total;
            }
        }


    }
}