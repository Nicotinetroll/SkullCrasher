using OctoberStudio.Easing;
using OctoberStudio.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PalbaGames;

namespace OctoberStudio.Abilities
{
    /// <summary>
    /// Active ability that spawns lightning strikes dealing damage and possibly crits to enemies in range.
    /// </summary>
    public class LightningAmuletAbilityBehavior : AbilityBehavior<LightningAmuletAbilityData, LightningAmuletAbilityLevel>
    {
        private static readonly int LIGHTNING_AMULET_HASH = "Lightning Amulet".GetHashCode();

        [SerializeField] GameObject lightningPrefab;
        public GameObject LightningPrefab => lightningPrefab;

        public PoolComponent<ParticleSystem> lightningPool;

        private List<IEasingCoroutine> easingCoroutines = new List<IEasingCoroutine>();
        private Coroutine abilityCoroutine;

        private void Awake()
        {
            lightningPool = new PoolComponent<ParticleSystem>("Lightning ability particle", LightningPrefab, 6);
        }

        public override void Init(AbilityData data, int stageId)
        {
            base.Init(data, stageId);
            abilityCoroutine = StartCoroutine(AbilityCoroutine());
        }

        private IEnumerator AbilityCoroutine()
        {
            while (true)
            {
                for (int i = 0; i < AbilityLevel.LightningsCount; i++)
                {
                    yield return new WaitForSeconds(AbilityLevel.DurationBetweenHits);

                    var particle = lightningPool.GetEntity();
                    var spawner = StageController.EnemiesSpawner;
                    var enemy = spawner.GetRandomVisibleEnemy();

                    if (enemy != null)
                    {
                        particle.transform.position = enemy.transform.position;

                        float baseDamage = PlayerBehavior.Player.Damage * AbilityLevel.Damage;
                        float finalDamage = baseDamage;
                        bool isCrit = false;

                        if (PlayerBehavior_Extended.Instance != null)
                        {
                            finalDamage = PlayerBehavior_Extended.Instance.GetFinalDamage(baseDamage, out isCrit);
                        }

                        // ✅ TRACKNI damage
                        var ext = enemy.GetComponent<EnemyBehavior_Extended>();
                        ext?.TakeDamageFromAbility(finalDamage, AbilityType.LightningAmulet, isCrit);

                        var enemiesInRadius = spawner.GetEnemiesInRadius(enemy.transform.position, AbilityLevel.AdditionalDamageRadius);

                        foreach (var closeEnemy in enemiesInRadius)
                        {
                            if (closeEnemy != enemy)
                            {
                                float splashBase = PlayerBehavior.Player.Damage * AbilityLevel.AdditionalDamage;
                                float splashFinal = splashBase;
                                bool splashCrit = false;

                                if (PlayerBehavior_Extended.Instance != null)
                                {
                                    splashFinal = PlayerBehavior_Extended.Instance.GetFinalDamage(splashBase, out splashCrit);
                                }

                                var splashExt = closeEnemy.GetComponent<EnemyBehavior_Extended>();
                                splashExt?.TakeDamageFromAbility(splashFinal, AbilityType.LightningAmulet, splashCrit);
                            }
                        }
                    }
                    else
                    {
                        particle.transform.position = PlayerBehavior.Player.transform.position + Vector3.up + Vector3.left;
                    }

                    IEasingCoroutine easingCoroutine = null;
                    easingCoroutine = EasingManager.DoAfter(1f, () =>
                    {
                        particle.gameObject.SetActive(false);
                        easingCoroutines.Remove(easingCoroutine);
                    });
                    easingCoroutines.Add(easingCoroutine);

                    GameController.AudioManager.PlaySound(LIGHTNING_AMULET_HASH);
                }

                yield return new WaitForSeconds(AbilityLevel.AbilityCooldown * PlayerBehavior.Player.CooldownMultiplier - AbilityLevel.DurationBetweenHits);
            }
        }

        public override void Clear()
        {
            if (abilityCoroutine != null)
                StopCoroutine(abilityCoroutine);

            foreach (var easing in easingCoroutines)
            {
                if (easing.ExistsAndActive())
                    easing.Stop();
            }

            easingCoroutines.Clear();
            lightningPool.Destroy();
            base.Clear();
        }
    }
}
