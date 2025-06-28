using UnityEngine;
using OctoberStudio;
using OctoberStudio.Easing;

namespace PalbaGames
{
    /// <summary>
    /// Extends PlayerBehavior with Critical Strike logic and AllIn1Shader hit effects.
    /// Attach to the same GameObject as PlayerBehavior.
    /// </summary>
    public class PlayerBehavior_Extended : MonoBehaviour
    {
        public static PlayerBehavior_Extended Instance { get; private set; }

        [Header("Critical Strike")]
        [Tooltip("Chance for a critical strike in % (0–100)")]
        public float criticalChance = 10f;

        [Tooltip("Minimum critical damage multiplier (e.g., 1.5 = +50%)")]
        public float criticalMultiplierMin = 1.5f;

        [Tooltip("Maximum critical damage multiplier (e.g., 5 = +400%)")]
        public float criticalMultiplierMax = 3f;

        [Header("AllIn1Shader Hit Effect")]
        [SerializeField] private Material allIn1Material;
        [SerializeField] private float hitDuration = 0.1f;
        [SerializeField] private bool enableCustomHitEffect = true;

        [Header("Hit Effect Settings")]
        [SerializeField] private bool useHitEffect = true;
        [SerializeField] private float hitEffectMaxBlend = 0.3f;
        [SerializeField] private string hitEffectBlendProperty = "_HitEffectBlend";

        [Header("Chromatic Aberration Settings")]
        [SerializeField] private bool useChromaticAberration = true;
        [SerializeField] private float chromaticAberrationMaxAmount = 0.132f;
        [SerializeField] private string chromaticAmountProperty = "_ChromAberrAmount";

        [Header("Additional Effects (Optional)")]
        [SerializeField] private bool useGlowIntensity = false;
        [SerializeField] private float glowIntensityMax = 5f;
        [SerializeField] private string glowIntensityProperty = "_GlowIntensity";

        private OctoberStudio.PlayerBehavior player;
        private CharacterBehavior characterBehavior;
        private SpriteRenderer spriteRenderer;
        private IEasingCoroutine hitCoroutine;

        // Property IDs for performance
        private int hitEffectBlendID;
        private int chromaticAmountID;
        private int glowIntensityID;

        // Track last HP to detect damage
        private float lastHP;
        private HealthbarBehavior healthbar;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            // Cache property IDs
            hitEffectBlendID = Shader.PropertyToID(hitEffectBlendProperty);
            chromaticAmountID = Shader.PropertyToID(chromaticAmountProperty);
            glowIntensityID = Shader.PropertyToID(glowIntensityProperty);
        }

        private void Start()
        {
            player = OctoberStudio.PlayerBehavior.Player;
            characterBehavior = GetComponentInChildren<CharacterBehavior>();
            healthbar = GetComponentInChildren<HealthbarBehavior>();

            if (player == null)
            {
                player = FindObjectOfType<OctoberStudio.PlayerBehavior>();
                if (player == null)
                    Debug.LogError("[PlayerBehavior_Extended] PlayerBehavior still not found in Start().");
            }

            // Setup AllIn1 hit effect if enabled
            if (enableCustomHitEffect)
            {
                SetupAllIn1HitEffect();
            }

            // Initialize HP tracking
            if (healthbar != null)
            {
                lastHP = healthbar.HP;
            }
        }

        private void Update()
        {
            // Monitor HP changes to detect hits and trigger custom effect
            if (enableCustomHitEffect && healthbar != null)
            {
                float currentHP = healthbar.HP;
                if (currentHP < lastHP)
                {
                    // Player took damage - trigger our AllIn1 hit effect
                    PlayAllIn1HitEffect();
                }
                lastHP = currentHP;
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
            {
                float multiplier = Random.Range(criticalMultiplierMin, criticalMultiplierMax);
                return player.Damage * multiplier;
            }

            return player.Damage;
        }

        /// <summary>
        /// Returns calculated damage and whether it was critical.
        /// </summary>
        public float GetFinalDamage(float baseDamage, out bool isCritical)
        {
            isCritical = Random.value <= criticalChance / 100f;

            if (isCritical)
            {
                float multiplier = Random.Range(criticalMultiplierMin, criticalMultiplierMax);
                return baseDamage * multiplier;
            }

            return baseDamage;
        }

        /// <summary>
        /// Setup AllIn1Shader material and sprite renderer.
        /// </summary>
        private void SetupAllIn1HitEffect()
        {
            spriteRenderer = characterBehavior?.GetComponentInChildren<SpriteRenderer>();

            if (spriteRenderer == null)
            {
                Debug.LogError("[PlayerBehavior_Extended] SpriteRenderer not found for AllIn1 hit effect!");
                enableCustomHitEffect = false;
                return;
            }

            // Replace material if provided
            if (allIn1Material != null)
            {
                spriteRenderer.material = allIn1Material;
                Debug.Log("[PlayerBehavior_Extended] AllIn1 material applied to player sprite.");
            }
        }

        /// <summary>
        /// Custom hit effect using AllIn1Shader Hit Effect and Chromatic Aberration.
        /// </summary>
        public void PlayAllIn1HitEffect()
        {
            if (!enableCustomHitEffect || spriteRenderer == null) return;
            if (hitCoroutine.ExistsAndActive()) return;

            Material mat = spriteRenderer.material;
            
            // Start multiple effect animations simultaneously
            StartCoroutine(AnimateHitEffects(mat));
        }

        private System.Collections.IEnumerator AnimateHitEffects(Material mat)
        {
            float halfDuration = hitDuration * 0.5f;
            
            // Animate Hit Effect Blend: 0 -> max -> 0
            if (useHitEffect && mat.HasProperty(hitEffectBlendID))
            {
                StartCoroutine(AnimateFloatProperty(mat, hitEffectBlendID, 0f, hitEffectMaxBlend, halfDuration, halfDuration));
            }
            
            // Animate Chromatic Aberration: 0 -> max -> 0
            if (useChromaticAberration && mat.HasProperty(chromaticAmountID))
            {
                StartCoroutine(AnimateFloatProperty(mat, chromaticAmountID, 0f, chromaticAberrationMaxAmount, halfDuration, halfDuration));
            }
            
            // Animate Glow Intensity: current -> max -> current (optional)
            if (useGlowIntensity && mat.HasProperty(glowIntensityID))
            {
                float currentGlow = mat.GetFloat(glowIntensityID);
                StartCoroutine(AnimateFloatProperty(mat, glowIntensityID, currentGlow, glowIntensityMax, halfDuration, halfDuration));
            }
            
            yield return null;
        }

        private System.Collections.IEnumerator AnimateFloatProperty(Material mat, int propertyID, float startValue, float maxValue, float riseTime, float fallTime)
        {
            // Rise phase: start -> max
            yield return StartCoroutine(TweenFloat(mat, propertyID, startValue, maxValue, riseTime));
            
            // Fall phase: max -> start
            yield return StartCoroutine(TweenFloat(mat, propertyID, maxValue, startValue, fallTime));
        }

        private System.Collections.IEnumerator TweenFloat(Material mat, int propertyID, float fromValue, float toValue, float duration)
        {
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float currentValue = Mathf.Lerp(fromValue, toValue, t);
                mat.SetFloat(propertyID, currentValue);
                yield return null;
            }
            
            // Ensure final value is set
            mat.SetFloat(propertyID, toValue);
        }

        /// <summary>
        /// Test hit effect in editor.
        /// </summary>
        [ContextMenu("Test AllIn1 Hit Effect")]
        public void TestHitEffect()
        {
            PlayAllIn1HitEffect();
        }

        /// <summary>
        /// Enable/disable custom hit effect at runtime.
        /// </summary>
        public void SetCustomHitEffectEnabled(bool enabled)
        {
            enableCustomHitEffect = enabled;
        }
    }
}