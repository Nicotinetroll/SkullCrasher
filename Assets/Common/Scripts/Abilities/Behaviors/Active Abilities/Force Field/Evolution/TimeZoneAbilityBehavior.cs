using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PalbaGames;

namespace OctoberStudio.Abilities
{
    /// <summary>
    /// Damaging aura that slows down enemies inside its zone.
    /// Deals damage periodically and optionally applies critical strikes.
    /// </summary>
    public class TimeZoneAbilityBehavior : AbilityBehavior<TimeGazerAbilityData, TimeGazerAbilityLevel>
    {
        public static readonly int TIME_GAZER_HASH = "Time Gazer".GetHashCode();

        [SerializeField] CircleCollider2D abilityCollider;
        [SerializeField] Transform visuals;

        private readonly Dictionary<EnemyBehavior, float> enemies = new();
        private Effect slowDownEffect;
        private float lastTimeSound;

        private void Awake()
        {
            slowDownEffect = new Effect(EffectType.Speed, 1);
            lastTimeSound = -100f;
        }

        public override void Init(AbilityData data, int stageId)
        {
            base.Init(data, stageId);
            slowDownEffect.SetModifier(AbilityLevel.SlowDownMultiplier);
        }

        private void LateUpdate()
        {
            transform.position = PlayerBehavior.CenterPosition;

            float radius = AbilityLevel.FieldRadius * PlayerBehavior.Player.SizeMultiplier;
            visuals.localScale = Vector2.one * radius * 2f;
            abilityCollider.radius = radius;

            if (Time.time - lastTimeSound > 5f)
            {
                lastTimeSound = Time.time;
                GameController.AudioManager.PlaySound(TIME_GAZER_HASH);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var enemy = collision.GetComponent<EnemyBehavior>();
            if (enemy == null || enemies.ContainsKey(enemy)) return;

            enemies.Add(enemy, Time.time);
            ApplyDamage(enemy);
            enemy.AddEffect(slowDownEffect);
        }

        private void Update()
        {
            float cooldown = AbilityLevel.DamageCooldown * PlayerBehavior.Player.CooldownMultiplier;

            foreach (var enemy in enemies.Keys.ToList())
            {
                if (Time.time - enemies[enemy] > cooldown)
                {
                    enemies[enemy] = Time.time;
                    ApplyDamage(enemy);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var enemy = collision.GetComponent<EnemyBehavior>();
            if (enemy == null || !enemies.ContainsKey(enemy)) return;

            enemies.Remove(enemy);
            enemy.RemoveEffect(slowDownEffect);
        }

        private void ApplyDamage(EnemyBehavior enemy)
        {
            float baseDamage = PlayerBehavior.Player.Damage * AbilityLevel.Damage;
            float finalDamage = baseDamage;
            bool isCrit = false;

            if (AbilityLevel.CanCrit && PlayerBehavior_Extended.Instance != null)
            {
                finalDamage = PlayerBehavior_Extended.Instance.GetFinalDamage(baseDamage, out isCrit);
            }

            var extended = enemy.GetComponent<EnemyBehavior_Extended>();
            if (extended != null)
            {
                extended.TakeDamageFromAbility(finalDamage, AbilityType.TimeGazer, isCrit);
            }
        }
    }
}
