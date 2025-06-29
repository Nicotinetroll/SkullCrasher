using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PalbaGames;
using FunkyCode; // Smart Lighting 2D

namespace OctoberStudio.Abilities
{
    /// <summary>
    /// Deals damage to enemies within a circular area around the player with flickering horror light effect.
    /// </summary>
    public class GuardianEyeAbilityBehavior : AbilityBehavior<GuardianEyeAbilityData, GuardianEyeAbilityLevel>
    {
        private static readonly int GUARDIAN_EYE_HASH = "Guardian Eye".GetHashCode();

        [SerializeField] CircleCollider2D abilityCollider;
        [SerializeField] Transform visuals;

        [Header("Horror Light Effect Settings")]
        [SerializeField] private Light2D abilityLight;
        [SerializeField] private bool enableHorrorFlicker = true;
        [SerializeField] private float flickerSpeed = 8f; // How fast the flicker changes (slowed down)
        [SerializeField] private float minAlpha = 0.3f; // Minimum light alpha
        [SerializeField] private float maxAlpha = 1f; // Maximum light alpha
        [SerializeField] private float noiseScale = 5f; // Perlin noise scale for randomness
        [SerializeField] private bool enableSizeStress = true; // Enable size flickering
        [SerializeField] private float minSize = 0.7f; // Minimum light size multiplier
        [SerializeField] private float maxSize = 1.3f; // Maximum light size multiplier
        
        private Dictionary<EnemyBehavior, float> enemies = new Dictionary<EnemyBehavior, float>();
        private float lastTimeSound;
        private float horrorFlickerTime;
        private Color originalLightColor;
        private float originalLightSize;

        private void Awake()
        {
            lastTimeSound = -100;
            horrorFlickerTime = 0f;
            
            // Auto-find light if not assigned - search in vfx_guardian eye prefab
            if (abilityLight == null)
            {
                // First try to find by name
                Transform vfxGuardianEye = transform.Find("vfx_guardian eye");
                if (vfxGuardianEye != null)
                {
                    abilityLight = vfxGuardianEye.GetComponentInChildren<Light2D>();
                }
                
                // Fallback to general search
                if (abilityLight == null)
                    abilityLight = GetComponentInChildren<Light2D>();
            }
            
            if (abilityLight != null)
            {
                originalLightColor = abilityLight.color;
                originalLightSize = abilityLight.size;
            }
            else
            {
                Debug.LogWarning($"[GuardianEyeAbilityBehavior] Light2D not found in vfx_guardian eye child object on {gameObject.name}");
            }
        }

        private void LateUpdate()
        {
            transform.position = PlayerBehavior.CenterPosition;

            float sizeMultiplier = PlayerBehavior.Player.SizeMultiplier;
            visuals.localScale = Vector2.one * AbilityLevel.FieldRadius * 2 * sizeMultiplier;
            abilityCollider.radius = AbilityLevel.FieldRadius * sizeMultiplier;
            
            // Update horror flicker effect
            if (enableHorrorFlicker && abilityLight != null)
            {
                UpdateHorrorFlicker();
            }
        }

        /// <summary>
        /// Creates a horror flicker effect using Perlin noise for smooth but erratic light changes
        /// </summary>
        private void UpdateHorrorFlicker()
        {
            horrorFlickerTime += Time.deltaTime * flickerSpeed;
            
            // Use multiple noise layers for more complex flickering
            float noise1 = Mathf.PerlinNoise(horrorFlickerTime, 0f);
            float noise2 = Mathf.PerlinNoise(horrorFlickerTime * 0.7f, 100f) * 0.5f;
            float noise3 = Mathf.PerlinNoise(horrorFlickerTime * 1.3f, 200f) * 0.3f;
            
            // Combine noises for more chaotic effect
            float combinedNoise = (noise1 + noise2 + noise3) / 1.8f;
            
            // Add some sharp variations for more "stress"
            float sharpNoise = Mathf.Sin(horrorFlickerTime * noiseScale) * 0.1f;
            combinedNoise += sharpNoise;
            
            // Clamp and map to alpha range
            combinedNoise = Mathf.Clamp01(combinedNoise);
            float flickerAlpha = Mathf.Lerp(minAlpha, maxAlpha, combinedNoise);
            
            // Apply flicker to light color
            abilityLight.color = new Color(
                originalLightColor.r, 
                originalLightColor.g, 
                originalLightColor.b, 
                flickerAlpha
            );
            
            // Apply size stress effect if enabled
            if (enableSizeStress)
            {
                // Use different noise for size to avoid synchronization
                float sizeNoise1 = Mathf.PerlinNoise(horrorFlickerTime * 0.8f, 500f);
                float sizeNoise2 = Mathf.PerlinNoise(horrorFlickerTime * 1.1f, 750f) * 0.6f;
                
                float combinedSizeNoise = (sizeNoise1 + sizeNoise2) / 1.6f;
                combinedSizeNoise = Mathf.Clamp01(combinedSizeNoise);
                
                float flickerSize = Mathf.Lerp(minSize, maxSize, combinedSizeNoise);
                abilityLight.size = originalLightSize * flickerSize;
            }
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

        /// <summary>
        /// Enable/disable horror flicker effect
        /// </summary>
        public void SetHorrorFlickerEnabled(bool enabled)
        {
            enableHorrorFlicker = enabled;
            
            if (!enabled && abilityLight != null)
            {
                // Reset to original values when disabled
                abilityLight.color = originalLightColor;
                abilityLight.size = originalLightSize;
            }
        }

        /// <summary>
        /// Adjust flicker intensity at runtime
        /// </summary>
        public void SetFlickerIntensity(float newMinAlpha, float newMaxAlpha, float newMinSize = 0.7f, float newMaxSize = 1.3f)
        {
            minAlpha = Mathf.Clamp01(newMinAlpha);
            maxAlpha = Mathf.Clamp01(newMaxAlpha);
            minSize = Mathf.Clamp(newMinSize, 0.1f, 2f);
            maxSize = Mathf.Clamp(newMaxSize, 0.1f, 2f);
        }

        private void OnDisable()
        {
            // Reset light when ability is disabled
            if (abilityLight != null)
            {
                abilityLight.color = originalLightColor;
                abilityLight.size = originalLightSize;
            }
        }
    }
}