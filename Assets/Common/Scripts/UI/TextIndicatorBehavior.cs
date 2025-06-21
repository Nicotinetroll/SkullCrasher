using TMPro;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace OctoberStudio.UI
{
    /// <summary>
    /// Handles positioning, text setting and feedback playback for world space damage indicators.
    /// </summary>
    public class TextIndicatorBehavior : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] RectTransform rectTransform;
        [SerializeField] TMP_Text textComponent;

        [Header("Feedbacks")]
        [SerializeField] MMF_Player feedbackPlayer;       // Local (per-instance) feedback
        [SerializeField] MMF_Player globalFeedbacks;      // Global (scene-wide) feedback

        /// <summary>
        /// Sets the displayed text and triggers feedback.
        /// </summary>
        public void SetText(string text)
        {
            textComponent.text = text;

            // Local feedback (on this indicator)
            if (feedbackPlayer != null)
            {
                feedbackPlayer.StopFeedbacks();
                feedbackPlayer.PlayFeedbacks();
            }

            // Global feedback (e.g. Bloom, Camera Shake)
            if (globalFeedbacks != null)
            {
                globalFeedbacks.StopFeedbacks();
                globalFeedbacks.PlayFeedbacks();
            }
        }

        /// <summary>
        /// Sets the anchor point in viewport space.
        /// </summary>
        public void SetAnchors(Vector2 viewportPosition)
        {
            rectTransform.anchorMin = viewportPosition;
            rectTransform.anchorMax = viewportPosition;
        }

        /// <summary>
        /// Sets the position relative to the anchor.
        /// </summary>
        public void SetPosition(Vector2 position)
        {
            rectTransform.anchoredPosition = position;
        }

        /// <summary>
        /// Sets the local scale of the indicator.
        /// </summary>
        public void SetScale(Vector3 scale)
        {
            rectTransform.localScale = scale;
        }
        
        public void SetGlobalFeedbacks(MMF_Player global)
        {
            globalFeedbacks = global;
        }

    }
}
