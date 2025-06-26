using UnityEngine.Timeline;

namespace PalbaGames.Timeline
{
    [TrackClipType(typeof(ChallengePlayableAsset))]
    [TrackBindingType(typeof(UnityEngine.GameObject))]
    public class ChallengeTrack : TrackAsset { }
}