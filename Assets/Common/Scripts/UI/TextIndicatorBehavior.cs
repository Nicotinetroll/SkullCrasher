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
        [SerializeField] RectTransform rectTransform;
        [SerializeField] TMP_Text textComponent;

        [Header("Feedbacks")]
        [SerializeField] MMF_Player feedbackPlayer;

        /// <summary>
        /// Sets the displayed text and triggers feedback.
        /// </summary>
        public void SetText(string text)
        {
            textComponent.text = text;

            if (feedbackPlayer != null)
            {
                feedbackPlayer.StopFeedbacks();     // Reset
                feedbackPlayer.PlayFeedbacks();     // Spusti znova
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
    }
}