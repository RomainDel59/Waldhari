using System.Collections.Generic;
using GTA;
using Waldhari.Common.UI;

namespace Waldhari.Behavior.Animation
{
    public static class AnimationHelper
    {
        
        public static void MissionSucceed(string messageKey, List<string> messageValues = null)
        {
            var script = Script.InstantiateScript<AnimationScript>();
            
            script.Animation = new Animation
            {
                TitleKey = "mission_succeed",
                SubtitleKey = messageKey,
                SubtitleValues = messageValues,
                FadeOutColor = Animation.Color.Green,
                PlaySoundAtBeginning = SoundHelper.PlayMissionSuccess
            };
        }
        
        public static void MissionFailed(string messageKey, List<string> messageValues = null)
        {
            var script = Script.InstantiateScript<AnimationScript>();
            
            script.Animation = new Animation
            {
                TitleKey = "mission_failed",
                SubtitleKey = messageKey,
                SubtitleValues = messageValues,
                FadeOutColor = Animation.Color.Red,
                PlaySoundAtBeginning = SoundHelper.PlayMissionFailure
            };
        }
        
        public static void PropertyBought(string messageKey, List<string> messageValues = null)
        {
            var script = Script.InstantiateScript<AnimationScript>();
            
            script.Animation = new Animation
            {
                TitleKey = "property_bought",
                SubtitleKey = messageKey,
                SubtitleValues = messageValues,
                FadeOutColor = Animation.Color.Yellow,
                PlaySoundAtBeginning = SoundHelper.PlayPayment
            };
        }
        
        
        
    }
}