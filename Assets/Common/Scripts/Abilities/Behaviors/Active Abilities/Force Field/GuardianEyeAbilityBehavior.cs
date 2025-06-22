using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PalbaGames;

namespace OctoberStudio.Abilities
{
    /// <summary>
    /// Deals damage to enemies within a circular area around the player.
    /// </summary>
    public class GuardianEyeAbilityBehavior : AbilityBehavior<GuardianEyeAbilityData, GuardianEyeAbilityLevel>
    {
        private static readonly int GUARDIAN_EYE_HASH = "Guardian Eye".GetHashCode();

        [SerializeField] CircleCollider2D abilityCollider;
        [SerializeField] Transform visuals;

        private Dictionary<EnemyBehavior, float> enemies = new Dictionary<EnemyBehavior, float>();
        private float lastTimeSound;

        private void Awake()
        {
            lastTimeSound = -100;
        }

        private void LateUpdate()
        {
            transform.position = PlayerBehavior.CenterPosition;

            float sizeMultiplier = PlayerBehavior.Player.SizeMultiplier;
            visuals.localScale = Vector2.one * AbilityLevel.FieldRadius * 2 * sizeMultiplier;
            abilityCollider.radius = AbilityLevel.FieldRadius * sizeMultiplier;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            EnemyBehavior enemy = collision.GetComponent<EnemyBehavior>();
            if (!enemies.ContainsKey(enemy))
            {
                enemies.Add(enemy, Time.time);
                ApplyDamage(enemy);
            }
        }

        private void Update()
        {
            foreach (var enemy in enemies.Keys.ToList())
            {
                if (Time.time - enemies[enemy] > AbilityLevel.DamageCooldown * PlayerBehavior.Player.CooldownMultiplier)
                {
                    enemies[enemy] = Time.time;
                    ApplyDamage(enemy);
                }
            }

            if (Time.time - lastTimeSound > 5f)
            {
                lastTimeSound = Time.time;
                GameController.AudioManager.PlaySound(GUARDIAN_EYE_HASH);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            EnemyBehavior enemy = collision.GetComponent<EnemyBehavior>();
            if (enemies.ContainsKey(enemy))
            {
                enemies.Remove(enemy);
            }
        }

        private void ApplyDamage(EnemyBehavior enemy)
        {
            float baseDamage = PlayerBehavior.Player.Damage;
            float finalDamage = PlayerBehavior_Extended.Instance?.GetFinalDamage() ?? baseDamage;

            bool isCrit = !Mathf.Approximately(baseDamage, finalDamage);
            float totalDamage = AbilityLevel.Damage * finalDamage;

            var extended = enemy.GetComponent<EnemyBehavior_Extended>();
            extended?.TakeDamageFromAbility(totalDamage, AbilityType.GuardianEye, isCrit);
        }
    }
}
