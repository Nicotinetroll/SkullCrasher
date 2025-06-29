using OctoberStudio.Easing;
using OctoberStudio.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PalbaGames;
using FunkyCode; // Smart Lighting 2D

namespace OctoberStudio.Abilities
{
    /// <summary>
    /// Active ability that spawns lightning strikes with smooth light fade-out effects.
    /// </summary>
    public class LightningAmuletAbilityBehavior : AbilityBehavior<LightningAmuletAbilityData, LightningAmuletAbilityLevel>
    {
        private static readonly int LIGHTNING_AMULET_HASH = "Lightning Amulet".GetHashCode();

        [SerializeField] GameObject lightningPrefab;
        public GameObject LightningPrefab => lightningPrefab;

        [Header("Light Fade Settings")]
        [SerializeField] private float lightDuration = 0.5f;
        [SerializeField] private float lightFadeDuration = 0.3f;
        [SerializeField] private AnimationCurve lightFadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

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

                    // 🔥 Start lightning effect with smooth light fade
                    StartCoroutine(HandleLightningEffect(particle));

                    GameController.AudioManager.PlaySound(LIGHTNING_AMULET_HASH);
                }

                yield return new WaitForSeconds(AbilityLevel.AbilityCooldown * PlayerBehavior.Player.CooldownMultiplier - AbilityLevel.DurationBetweenHits);
            }
        }

        /// <summary>
        /// Handle lightning effect with smooth light fade-out
        /// </summary>
        private IEnumerator HandleLightningEffect(ParticleSystem particle)
        {
            // Find Light2D component in the lightning prefab
            Light2D lightComponent = particle.GetComponentInChildren<Light2D>();
            
            float originalAlpha = 1f;
            if (lightComponent != null)
            {
                originalAlpha = lightComponent.color.a;
            }

            // Wait for light duration before starting fade
            yield return new WaitForSeconds(lightDuration);

            // Fade out the light
            if (lightComponent != null)
            {
                yield return StartCoroutine(FadeLightCoroutine(lightComponent, originalAlpha));
            }

            // Wait a bit more for complete effect cleanup
            yield return new WaitForSeconds(0.1f);

            // Deactivate the particle system
            particle.gameObject.SetActive(false);
        }

        /// <summary>
        /// Smooth fade-out coroutine for Light2D alpha
        /// </summary>
        private IEnumerator FadeLightCoroutine(Light2D lightComponent, float startAlpha)
        {
            float elapsedTime = 0f;
            Color originalColor = lightComponent.color;

            while (elapsedTime < lightFadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / lightFadeDuration;
                float curveValue = lightFadeCurve.Evaluate(progress);

                // Fade from original alpha to 0
                float currentAlpha = Mathf.Lerp(startAlpha, 0f, curveValue);
                lightComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, currentAlpha);

                yield return null;
            }

            // Ensure final alpha is 0
            lightComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
            
            // Reset alpha for next use
            lightComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, startAlpha);
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