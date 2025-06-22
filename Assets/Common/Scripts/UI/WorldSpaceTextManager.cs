using OctoberStudio.Pool;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace OctoberStudio.UI
{
    /// <summary>
    /// Handles spawning of floating damage text in world space, including critical hit variations.
    /// </summary>
    public class WorldSpaceTextManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] RectTransform canvasRect;
        [SerializeField] GameObject textIndicatorPrefab;
        [SerializeField] GameObject critTextIndicatorPrefab;

        [Header("Feedbacks")]
        [SerializeField] MMF_Player globalCritFeedbackPlayer;

        [Header("Animation")]
        [SerializeField] AnimationCurve scaleCurve;
        [SerializeField] AnimationCurve positionCurve;
        [SerializeField] float maxScale = 1f;
        [SerializeField] float maxY = 20f;
        [SerializeField] float duration = 1f;

        private PoolComponent<TextIndicatorBehavior> normalPool;
        private PoolComponent<TextIndicatorBehavior> critPool;
        private Queue<IndicatorData> indicators = new();

        private void Start()
        {
            normalPool = new PoolComponent<TextIndicatorBehavior>(textIndicatorPrefab, 500, canvasRect);
            critPool = new PoolComponent<TextIndicatorBehavior>(critTextIndicatorPrefab, 100, canvasRect);
        }

        public void SpawnText(Vector2 worldPos, string text, bool isCritical = false)
        {
            var viewportPos = Camera.main.WorldToViewportPoint(worldPos);
            var pool = isCritical ? critPool : normalPool;

            var indicator = pool.GetEntity();
            indicator.SetText(text);
            indicator.SetAnchors(viewportPos);
            indicator.SetPosition(Vector2.zero);

            // Pass global feedbacks for crit
            if (isCritical && globalCritFeedbackPlayer != null)
            {
                indicator.SetGlobalFeedbacks(globalCritFeedbackPlayer);
            }

            indicators.Enqueue(new IndicatorData
            {
                indicator = indicator,
                spawnTime = Time.time,
                worldPosition = worldPos
            });
        }

        private void Update()
        {
            while (indicators.Count > 0 && Time.time > indicators.Peek().spawnTime + duration)
            {
                indicators.Dequeue().indicator.gameObject.SetActive(false);
            }

            foreach (var data in indicators)
            {
                float t = (Time.time - data.spawnTime) / duration;
                data.indicator.SetPosition(Vector2.up * positionCurve.Evaluate(t) * maxY);
                data.indicator.SetScale(Vector3.one * scaleCurve.Evaluate(t) * maxScale);
                data.indicator.SetAnchors(Camera.main.WorldToViewportPoint(data.worldPosition));
            }
        }

        private class IndicatorData
        {
            public TextIndicatorBehavior indicator;
            public float spawnTime;
            public Vector2 worldPosition;
        }
    }
}
