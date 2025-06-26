using UnityEngine;
using UnityEngine.Playables;

namespace PalbaGames.Timeline
{
    [System.Serializable]
    public class ChallengePlayableAsset : PlayableAsset
    {
        public string challengeId;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<ChallengePlayableBehaviour>.Create(graph);
            playable.GetBehaviour().Init(challengeId);
            return playable;
        }
    }
}