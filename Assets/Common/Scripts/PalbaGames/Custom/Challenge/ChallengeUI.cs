using TMPro;
using UnityEngine;

namespace PalbaGames.Challenges
{
    /// <summary>
    /// Displays active challenge title, progress and timer.
    /// Assumes only one active challenge at a time.
    /// </summary>
    public class ChallengeUI : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private TextMeshProUGUI timerText;

        private BaseChallenge current;

        private void Update()
        {
            if (current == null)
            {
                if (root.activeSelf)
                    root.SetActive(false);
                return;
            }

            if (!root.activeSelf)
                root.SetActive(true);

            timerText.text = $"{Mathf.CeilToInt(current.RemainingTime)}s";
            progressText.text = current.GetProgressString();
        }

        /// <summary>
        /// Call when a new challenge is started.
        /// </summary>
        public void ShowChallenge(BaseChallenge challenge)
        {
            current = challenge;
            titleText.text = challenge.GetDisplayName();
            root.SetActive(true);
        }

        /// <summary>
        /// Call when the challenge ends.
        /// </summary>
        public void Hide()
        {
            current = null;
            root.SetActive(false);
        }
    }
}