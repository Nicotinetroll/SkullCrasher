using UnityEngine;
using OctoberStudio;
using OctoberStudio.Easing;
using MoreMountains.Feedbacks;

namespace PalbaGames
{
    /// <summary>
    /// Extends PlayerBehavior with Critical Strike logic, AllIn1Shader hit effects, Feel camera shake, and Grass Movement animation.
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

        [Header("Camera Shake (Feel Plugin)")]
        [SerializeField] private MMF_Player damageShakeFeedback;
        [SerializeField] private bool enableCameraShake = true;

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

        [Header("Grass Movement / Wind Effect")]
        [SerializeField] private bool useGrassMovement = true;
        [SerializeField] private float grassMovementValue = 0.6f;
        [SerializeField] private float grassAnimationSpeed = 2f;
        [SerializeField] private string grassMovementProperty = "_GrassManualAnim";

        private OctoberStudio.PlayerBehavior player;
        private CharacterBehavior characterBehavior;
        private SpriteRenderer spriteRenderer;
        private IEasingCoroutine hitCoroutine;
        private IEasingCoroutine grassCoroutine;

        // Property IDs for performance
        private int hitEffectBlendID;
        private int chromaticAmountID;
        private int glowIntensityID;
        private int grassMovementID;

        // Track last HP to detect damage
        private float lastHP;
        private HealthbarBehavior healthbar;

        // Movement tracking for grass effect
        private bool wasMoving = false;

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
            grassMovementID = Shader.PropertyToID(grassMovementProperty);
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

            // Auto-find shake feedback if not assigned
            if (enableCameraShake)
            {
                if (damageShakeFeedback == null)
                {
                    damageShakeFeedback = FindObjectOfType<MMF_Player>();
                    if (damageShakeFeedback == null)
                    {
                        Debug.LogWarning("[PlayerBehavior_Extended] MMF_Player pre camera shake sa nenašiel!");
                    }
                }
            }
        }

        private void Update()
        {
            // Monitor HP changes to detect hits and trigger effects
            if (healthbar != null)
            {
                float currentHP = healthbar.HP;
                if (currentHP < lastHP)
                {
                    float damageAmount = lastHP - currentHP;
                    float maxHP = healthbar.MaxHP;
                    
                    // Trigger camera shake
                    if (enableCameraShake)
                    {
                        TriggerDamageCameraShake(damageAmount, maxHP);
                    }
                    
                    // Trigger AllIn1 hit effect
                    if (enableCustomHitEffect)
                    {
                        PlayAllIn1HitEffect();
                    }
                }
                lastHP = currentHP;
            }

            // Handle grass movement effect based on player movement
            if (useGrassMovement && player != null)
            {
                HandleGrassMovementEffect();
            }
        }

        /// <summary>
        /// Handles grass movement effect based on player movement input.
        /// </summary>
        private void HandleGrassMovementEffect()
        {
            if (spriteRenderer == null) return;
            
            // Use direct string check instead of cached ID
            if (!spriteRenderer.material.HasProperty("_GrassManualAnim")) return;

            var input = GameController.InputManager.MovementValue;
            bool isMoving = !Mathf.Approximately(input.magnitude, 0) && player.IsMovingAlowed;

            // State changed - animate transition
            if (isMoving != wasMoving)
            {
                AnimateGrassMovement(isMoving);
                wasMoving = isMoving;
            }
        }

        /// <summary>
        /// Animates grass movement property between 0 and target value.
        /// </summary>
        private void AnimateGrassMovement(bool startMoving)
        {
            if (grassCoroutine.ExistsAndActive())
            {
                grassCoroutine.Stop();
            }

            Material mat = spriteRenderer.material;
            
            // Use direct string instead of cached ID for debugging
            string propertyName = "_GrassManualAnim";
            
            if (!mat.HasProperty(propertyName))
            {
   //             Debug.LogError($"[Grass] Material doesn't have property: {propertyName}");
                return;
            }
            
            float currentValue = mat.GetFloat(propertyName);
            float targetValue = startMoving ? grassMovementValue : 0f;
            
//            Debug.Log($"[Grass] Animating {propertyName} from {currentValue:F2} to {targetValue:F2}");

            grassCoroutine = EasingManager.DoFloat(currentValue, targetValue, grassAnimationSpeed, 
                (value) => mat.SetFloat(propertyName, value));
        }

        /// <summary>
        /// Triggers camera shake using prepared MMF_Player.
        /// </summary>
        public void TriggerDamageCameraShake(float damageAmount, float maxHP)
        {
            if (!enableCameraShake || damageShakeFeedback == null) return;

            if (!damageShakeFeedback.enabled)
            {
                damageShakeFeedback.enabled = true;
            }
            
            damageShakeFeedback.PlayFeedbacks();
        }

        /// <summary>
        /// Manually trigger camera shake.
        /// </summary>
        public void TriggerCameraShake()
        {
            if (enableCameraShake && damageShakeFeedback != null)
                damageShakeFeedback.PlayFeedbacks();
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
        /// Test camera shake in editor.
        /// </summary>
        [ContextMenu("Test Camera Shake")]
        public void TestCameraShake() => TriggerCameraShake();

        /// <summary>
        /// Test grass movement effect in editor.
        /// </summary>
        [ContextMenu("Test Grass Movement")]
        public void TestGrassMovement()
        {
            // Refresh property ID in case it was changed in inspector
            grassMovementID = Shader.PropertyToID(grassMovementProperty);
            
            if (spriteRenderer != null)
            {
                AnimateGrassMovement(true);
            }
        }

        /// <summary>
        /// Refresh all cached property IDs - useful when changing property names in inspector.
        /// </summary>
        [ContextMenu("Refresh Property IDs")]
        public void RefreshPropertyIDs()
        {
            hitEffectBlendID = Shader.PropertyToID(hitEffectBlendProperty);
            chromaticAmountID = Shader.PropertyToID(chromaticAmountProperty);
            glowIntensityID = Shader.PropertyToID(glowIntensityProperty);
            grassMovementID = Shader.PropertyToID(grassMovementProperty);
            
            Debug.Log($"[PropertyIDs] Refreshed: {hitEffectBlendProperty}, {chromaticAmountProperty}, {glowIntensityProperty}, {grassMovementProperty}");
        }

        /// <summary>
        /// List all material properties for debugging.
        /// </summary>
        [ContextMenu("List Material Properties")]
        public void ListMaterialProperties()
        {
            if (spriteRenderer?.material != null)
            {
                var material = spriteRenderer.material;
                var shader = material.shader;
                
                Debug.Log($"[DEBUG] Material: {material.name}, Shader: {shader.name}");
                Debug.Log($"[DEBUG] Total properties: {shader.GetPropertyCount()}");
                
                for (int i = 0; i < shader.GetPropertyCount(); i++)
                {
                    string propName = shader.GetPropertyName(i);
                    var propType = shader.GetPropertyType(i);
                    
                    // Try to get current value if it's a float
                    string currentValue = "";
                    if (propType == UnityEngine.Rendering.ShaderPropertyType.Float || 
                        propType == UnityEngine.Rendering.ShaderPropertyType.Range)
                    {
                        try
                        {
                            float val = material.GetFloat(propName);
                            currentValue = $" = {val:F3}";
                        }
                        catch { currentValue = " = (error)"; }
                    }
                    
                    Debug.Log($"[PROP] {i}: {propName} ({propType}){currentValue}");
                }
            }
            else
            {
                Debug.LogError("[DEBUG] SpriteRenderer or material is null!");
            }
        }

        /// <summary>
        /// Enable/disable camera shake at runtime.
        /// </summary>
        public void SetCameraShakeEnabled(bool enabled)
        {
            enableCameraShake = enabled;
        }

        /// <summary>
        /// Enable/disable custom hit effect at runtime.
        /// </summary>
        public void SetCustomHitEffectEnabled(bool enabled)
        {
            enableCustomHitEffect = enabled;
        }

        /// <summary>
        /// Enable/disable grass movement effect at runtime.
        /// </summary>
        public void SetGrassMovementEnabled(bool enabled)
        {
            useGrassMovement = enabled;
        }
    }
}