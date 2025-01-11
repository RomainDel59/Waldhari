using System;
using System.Collections.Generic;
using GTA.Native;
using Waldhari.Common.Files;
using Waldhari.Common.UI;

namespace Waldhari.Common.Behavior.Animation
{
    public class Animation
    {
        public enum Color
        {
            // color 0,1=white 2=black 3=grey 6,7,8=red 9,10,11=blue 12,13,14=yellow 15,16,17=orange 18,19,20=green 21,22,23=purple
            White=1,
            Black=2,
            Grey=3,
            Red=8,
            Blue=11,
            Yellow=14,
            Orange=17,
            Green=20,
            Purple=23
        }
        
        public int ShowDuration = 5;
        public int FadeOutDuration = 2;
        
        public string TitleKey;
        public string SubtitleKey;
        public List<string> SubtitleValues;
        public Color FadeOutColor;
        public Action PlaySoundAtBeginning;
        public Action PlaySoundAtEnd = SoundHelper.PlayDisappear;

        private int _id;
        public bool IsLoaded() => Function.Call<bool>(Hash.HAS_SCALEFORM_MOVIE_LOADED, _id);

        public void Create()
        {
            _id = Function.Call<int>(Hash.REQUEST_SCALEFORM_MOVIE, "MIDSIZED_MESSAGE");
        }

        public bool Launch()
        {
            if (!IsLoaded()) return false;

            var postFxDuration = (ShowDuration+FadeOutDuration) * 1000;
            Function.Call(Hash.ANIMPOSTFX_PLAY, "SuccessNeutral", postFxDuration, false);

            PlaySoundAtBeginning?.Invoke();

            Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, _id, "SHOW_SHARD_MIDSIZED_MESSAGE");

            var title = Localization.GetTextByKey(TitleKey);
            Function.Call(Hash.BEGIN_TEXT_COMMAND_SCALEFORM_STRING, "STRING");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, title);
            Function.Call(Hash.END_TEXT_COMMAND_SCALEFORM_STRING);

            var subtitle = Localization.GetTextByKey(SubtitleKey, SubtitleValues);
            Function.Call(Hash.BEGIN_TEXT_COMMAND_SCALEFORM_STRING, "STRING");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, subtitle);
            Function.Call(Hash.END_TEXT_COMMAND_SCALEFORM_STRING);

            Function.Call(Hash.END_SCALEFORM_MOVIE_METHOD);
            
            return true;
        }

        public void Show()
        {
            if (!IsLoaded()) return;
            
            Function.Call(Hash.DRAW_SCALEFORM_MOVIE_FULLSCREEN, _id, 255, 255, 255, 255);
        }
        
        public void FadeOut()
        {
            if (!IsLoaded()) return;
            
            Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, _id, "SHARD_ANIM_OUT");
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, FadeOutColor);
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_FLOAT, 1/3f);
            
            Function.Call(Hash.END_SCALEFORM_MOVIE_METHOD);

            PlaySoundAtEnd?.Invoke();
        }

        public void Hide()
        {
            unsafe
            {
                var id = _id;
                Function.Call(Hash.SET_SCALEFORM_MOVIE_AS_NO_LONGER_NEEDED, &id);
            }
        }
        
        
        
        
        
    }
}