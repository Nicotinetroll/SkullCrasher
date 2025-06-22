using UnityEngine;
using OctoberStudio;
using OctoberStudio.Enemy;

namespace PalbaGames
{
    /// <summary>
    /// Extended enemy behavior that handles ability-type damage tracking.
    /// </summary>
    [RequireComponent(typeof(EnemyBehavior))]
    public class EnemyBehavior_Extended : MonoBehaviour
    {
        private EnemyBehavior _enemy;

        private void Awake()
        {
            _enemy = GetComponent<EnemyBehavior>();
        }

        public void TakeDamageFromAbility(float amount, AbilityType source, bool isCritical = false)
        {
            if (!_enemy.IsAlive || _enemy.IsInvulnerable) return;

            DamageTracker.Instance?.ReportDamage(source, amount);
            _enemy.TakeDamage(amount, isCritical);
        }
    }
}