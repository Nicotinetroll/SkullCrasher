using UnityEngine;
using UnityEngine.UI;

namespace PalbaGames.Challenges
{
    /// <summary>
    /// Simple particle controller for progress bar effects.
    /// Add this to a GameObject with ParticleSystem in your progress block prefab.
    /// </summary>
    public class ChallengeProgressParticles : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Slider targetSlider;
        [SerializeField] private RectTransform fillArea;
        [SerializeField] private ParticleSystem particles;
        
        [Header("Settings")]
        [SerializeField] private float progressThreshold = 0.05f;
        [SerializeField] private int particlesPerProgress = 5;
        [SerializeField] private float maxEmissionRate = 30f;
        
        private float lastSliderValue = -1f;
        private float currentIntensity = 1.0f;
        private bool particlesEnabled = false;
        private float lastEmissionTime;
        private const float MIN_EMISSION_INTERVAL = 0.1f;

        private void Awake()
        {
            // Auto-find components if not assigned
            if (particles == null)
                particles = GetComponent<ParticleSystem>();
            
            if (targetSlider == null)
                targetSlider = GetComponentInParent<Slider>();
                
            if (fillArea == null && targetSlider != null)
                fillArea = targetSlider.fillRect;

            // Ensure particles are stopped initially
            if (particles != null)
            {
                particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        private void Update()
        {
            if (!particlesEnabled || targetSlider == null || particles == null) return;

            float currentValue = targetSlider.value;
            
            // Check if progress changed significantly
            if (Mathf.Abs(currentValue - lastSliderValue) >= progressThreshold)
            {
                // Throttle emissions for performance
                if (Time.time - lastEmissionTime >= MIN_EMISSION_INTERVAL)
                {
                    TriggerProgressParticles(currentValue);
                    lastEmissionTime = Time.time;
                }
                
                lastSliderValue = currentValue;
            }

            // Update particle position to follow fill edge
            UpdateParticlePosition(currentValue);
        }

        /// <summary>
        /// Enable particle effects with specified intensity.
        /// </summary>
        public void EnableParticles(GameObject particlePrefab = null, float intensity = 1.0f)
        {
            particlesEnabled = true;
            currentIntensity = Mathf.Clamp(intensity, 0.1f, 3.0f);
            lastSliderValue = targetSlider != null ? targetSlider.value : 0f;
            
            if (particles != null)
            {
                // Ensure emission is manual
                var emission = particles.emission;
                emission.enabled = false;
                emission.rateOverTime = 0f;
                
                particles.Play();
                Debug.Log("[ChallengeProgressParticles] Particles enabled");
            }
        }

        /// <summary>
        /// Disable particle effects and clear existing particles.
        /// </summary>
        public void DisableParticles()
        {
            particlesEnabled = false;
            if (particles != null)
            {
                particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        private void TriggerProgressParticles(float sliderValue)
        {
            if (particles == null) return;

            // Calculate emission count based on progress change and intensity
            float progressDelta = Mathf.Abs(sliderValue - lastSliderValue);
            int emissionCount = Mathf.RoundToInt(particlesPerProgress * progressDelta * currentIntensity);
            emissionCount = Mathf.Clamp(emissionCount, 1, Mathf.RoundToInt(maxEmissionRate * Time.deltaTime));
            
            // Debug log for testing
            Debug.Log($"[ChallengeProgressParticles] Emitting {emissionCount} particles at progress {sliderValue:F2}");
            
            // Emit particles
            particles.Emit(emissionCount);
        }

        private void UpdateParticlePosition(float sliderValue)
        {
            if (fillArea == null || particles == null) return;

            // Calculate fill edge position
            Vector3 fillEdgePosition = GetFillEdgePosition(sliderValue);
            
            // Move particle system to fill edge
            transform.localPosition = fillEdgePosition;
        }

        private Vector3 GetFillEdgePosition(float sliderValue)
        {
            if (fillArea == null) return Vector3.zero;

            // Get fill area bounds
            Rect fillRect = fillArea.rect;
            
            // Calculate fill edge position based on slider value
            float fillWidth = fillRect.width * sliderValue;
            
            // Return local position relative to fill area
            Vector3 localEdgePos = new Vector3(fillRect.xMin + fillWidth, fillRect.center.y, 0f);
            
            return localEdgePos;
        }

        /// <summary>
        /// Manual trigger for testing.
        /// </summary>
        public void ManualTrigger(int particleCount = 10)
        {
            if (!particlesEnabled || particles == null) return;
            
            particles.Emit(particleCount);
        }

        private void OnValidate()
        {
            // Clamp values in inspector
            particlesPerProgress = Mathf.Max(1, particlesPerProgress);
            progressThreshold = Mathf.Clamp(progressThreshold, 0.01f, 1f);
            maxEmissionRate = Mathf.Max(1f, maxEmissionRate);
        }
    }
}