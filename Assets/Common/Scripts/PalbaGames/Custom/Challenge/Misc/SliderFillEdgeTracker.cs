using UnityEngine;
using UnityEngine.UI;

namespace PalbaGames.Challenges
{
    /// <summary>
    /// Dynamically positions GameObject at the fill edge of a slider.
    /// Attach to ParticleSpawnPoint to follow slider progress.
    /// </summary>
    public class SliderFillEdgeTracker : MonoBehaviour
    {
        [Header("Slider Reference")]
        [Tooltip("Target slider to track. If null, will search in parent.")]
        [SerializeField] private Slider targetSlider;
        
        [Tooltip("Use Fill Image instead of Fill Rect for more accurate positioning.")]
        [SerializeField] private bool useFillImage = true;
        
        [Header("Position Settings")]
        [Tooltip("Offset from calculated fill edge position.")]
        [SerializeField] private Vector3 positionOffset = Vector3.zero;
        
        [Tooltip("Update position every frame (disable for performance if slider changes rarely).")]
        [SerializeField] private bool updateEveryFrame = true;
        
        [Header("Debug")]
        [Tooltip("Show debug information in console.")]
        [SerializeField] private bool showDebugInfo = false;

        private RectTransform fillArea;
        private Image fillImage;
        private float lastSliderValue = -1f;

        private void Start()
        {
            // Auto-find slider if not assigned
            if (targetSlider == null)
            {
                targetSlider = GetComponentInParent<Slider>();
            }

            if (targetSlider != null)
            {
                fillArea = targetSlider.fillRect;
                
                // Try to get fill image for more accurate positioning
                if (useFillImage && targetSlider.fillRect != null)
                {
                    fillImage = targetSlider.fillRect.GetComponent<Image>();
                }
                
                if (showDebugInfo)
                    Debug.Log($"[SliderFillEdgeTracker] Tracking slider: {targetSlider.name}, UseFillImage: {fillImage != null}");
            }
            else
            {
                Debug.LogWarning("[SliderFillEdgeTracker] No target slider found!");
            }

            // Initial position update
            UpdatePosition();
        }

        private void Update()
        {
            if (!updateEveryFrame) return;

            // Only update if slider value changed (performance optimization)
            if (targetSlider != null && Mathf.Abs(targetSlider.value - lastSliderValue) > 0.001f)
            {
                UpdatePosition();
                lastSliderValue = targetSlider.value;
            }
        }

        /// <summary>
        /// Manually update position (call this if updateEveryFrame is disabled).
        /// </summary>
        public void UpdatePosition()
        {
            if (targetSlider == null || fillArea == null) return;

            // Calculate fill edge position
            Vector3 fillEdgePos = CalculateFillEdgePosition();
            
            // Apply offset and update transform
            transform.localPosition = fillEdgePos + positionOffset;

            if (showDebugInfo)
            {
                Debug.Log($"[SliderFillEdgeTracker] Updated position to: {transform.localPosition} (slider value: {targetSlider.value:F3})");
            }
        }

        private Vector3 CalculateFillEdgePosition()
        {
            if (fillArea == null) return Vector3.zero;

            // This slider uses a different fill method - the fill area grows from center
            // We need to find the actual slider background and calculate from there
            
            // Get the slider's handle slide area (this should be the full width)
            RectTransform sliderBackground = targetSlider.GetComponent<RectTransform>();
            if (sliderBackground == null) return Vector3.zero;
            
            Rect bgRect = sliderBackground.rect;
            
            // Calculate position based on slider value and background width
            float fillProgress = targetSlider.value; // 0 to 1
            float totalWidth = bgRect.width;
            float fillPixels = totalWidth * fillProgress;
            
            // Start from the left edge of the slider background
            Vector3 fillEdgePosition = new Vector3(
                bgRect.xMin + fillPixels,
                bgRect.center.y,
                0f
            );

            if (showDebugInfo)
            {
                Debug.Log($"[SliderFillEdgeTracker] Background Rect: {bgRect}");
                Debug.Log($"[SliderFillEdgeTracker] Progress: {fillProgress:F3}, TotalWidth: {totalWidth:F1}, FillPixels: {fillPixels:F1}");
                Debug.Log($"[SliderFillEdgeTracker] Final EdgePos: {fillEdgePosition}");
            }

            return fillEdgePosition;
        }

        /// <summary>
        /// Set target slider programmatically.
        /// </summary>
        public void SetTargetSlider(Slider slider)
        {
            targetSlider = slider;
            
            if (targetSlider != null)
            {
                fillArea = targetSlider.fillRect;
                UpdatePosition();
            }
        }

        private void OnValidate()
        {
            // Update position in editor when values change
            if (Application.isPlaying)
            {
                UpdatePosition();
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Draw gizmo to show current position
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 5f);
        }
    }
}