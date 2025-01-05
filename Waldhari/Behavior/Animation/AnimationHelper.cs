using System.Collections.Generic;
using GTA;
using Waldhari.Common.Files;
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
                Title = "~g~" + Localization.GetTextByKey("mission_succeed") + "~s~",
                Subtitle = Localization.GetTextByKey(messageKey, messageValues),
                FadeOutColor = Animation.Color.Green,
                PlaySoundAtBeginning = SoundHelper.PlayMissionSuccess
            };
        }
        
        public static void MissionFailed(string messageKey, List<string> messageValues = null)
        {
            var script = Script.InstantiateScript<AnimationScript>();
            
            script.Animation = new Animation
            {
                Title = "~r~" + Localization.GetTextByKey("mission_failed") + "~s~",
                Subtitle = Localization.GetTextByKey(messageKey, messageValues),
                FadeOutColor = Animation.Color.Red,
                PlaySoundAtBeginning = SoundHelper.PlayMissionFailure
            };
        }
        
        public static void PropertyBought(string messageKey, List<string> messageValues = null)
        {
            var script = Script.InstantiateScript<AnimationScript>();
            
            script.Animation = new Animation
            {
                Title = "~y~" + Localization.GetTextByKey("property_bought") + "~s~",
                Subtitle = Localization.GetTextByKey(messageKey, messageValues),
                FadeOutColor = Animation.Color.Yellow,
                PlaySoundAtBeginning = SoundHelper.PlayPayment
            };
        }
        
        
        
    }
}