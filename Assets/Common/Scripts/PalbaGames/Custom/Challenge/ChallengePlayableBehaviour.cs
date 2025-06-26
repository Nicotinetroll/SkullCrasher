using UnityEngine;
using UnityEngine.Playables;
using PalbaGames.Challenges;

namespace PalbaGames.Timeline
{
    public class ChallengePlayableBehaviour : PlayableBehaviour
    {
        private string challengeId;

        public void Init(string id) => challengeId = id;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (!Application.isPlaying) return;
            ChallengeManager.Instance.TriggerChallenge(challengeId);
        }
    }
}