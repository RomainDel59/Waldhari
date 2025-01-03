using System.Collections.Generic;
using GTA;
using GTA.Native;
using Waldhari.Common.Files;

namespace Waldhari.Common.UI
{
    public static class MissionAnimationHelper
    {
        private const int TimeToShow = 5;

        private static int _nextStepTime;
        private static int _step = -1;
        private static int _movie;
        private static string _title;
        private static string _subtitle;

        // color 0,1=white 2=black 3=grey 6,7,8=red 9,10,11=blue 12,13,14=yellow 15,16,17=orange 18,19,20=green 21,22,23=purple
        private static int _color;

        private static bool _success;

        public static void Success(string messageKey, List<string> messageValues = null)
        {
            if (_step != -1) return;
            
            var message = Localization.GetTextByKey(messageKey, messageValues);

            _title = "~g~" + Localization.GetTextByKey("mission_success") + "~s~";
            _subtitle = message;
            _color = 20;
            _success = true;
            _step = 0;
        }

        public static void Fail(string messageKey, List<string> messageValues = null)
        {
            if (_step != -1) return;

            var message = Localization.GetTextByKey(messageKey, messageValues);

            _title = "~r~" + Localization.GetTextByKey("mission_fail") + "~s~";
            _subtitle = message;
            _color = 8;
            _success = false;
            _step = 0;
        }

        public static void Show()
        {
            if (_step == -1) return;

            switch (_step)
            {
                case 0:
                    _movie = Function.Call<int>(Hash.REQUEST_SCALEFORM_MOVIE, "MIDSIZED_MESSAGE");
                    _nextStepTime = Game.GameTime;
                    _step = 1;
                    break;

                case 1:
                    if (Game.GameTime > _nextStepTime)
                    {
                        if (Function.Call<bool>(Hash.HAS_SCALEFORM_MOVIE_LOADED, _movie))
                        {
                            Function.Call(Hash.ANIMPOSTFX_PLAY, "SuccessNeutral", 8500, false);

                            if (_success)
                            {
                                SoundHelper.PlayMissionSuccess();
                            }
                            else
                            {
                                SoundHelper.PlayMissionFailure();
                            }

                            Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, _movie, "SHOW_SHARD_MIDSIZED_MESSAGE");

                            Function.Call(Hash.BEGIN_TEXT_COMMAND_SCALEFORM_STRING, "STRING");
                            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, _title);
                            Function.Call(Hash.END_TEXT_COMMAND_SCALEFORM_STRING);

                            Function.Call(Hash.BEGIN_TEXT_COMMAND_SCALEFORM_STRING, "STRING");
                            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, _subtitle);
                            Function.Call(Hash.END_TEXT_COMMAND_SCALEFORM_STRING);

                            Function.Call(Hash.END_SCALEFORM_MOVIE_METHOD);

                            _nextStepTime = Game.GameTime + TimeToShow * 1000;

                            _step = 2;
                        }
                    }

                    break;

                case 2:
                    if (Function.Call<bool>(Hash.HAS_SCALEFORM_MOVIE_LOADED, _movie))
                    {
                        if (Game.GameTime <= _nextStepTime)
                        {
                            Function.Call(Hash.DRAW_SCALEFORM_MOVIE_FULLSCREEN, _movie, 255, 255, 255, 255);
                        }
                        else if (Game.GameTime < _nextStepTime + 100)
                        {
                            Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, _movie, "SHARD_ANIM_OUT");
                            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, _color);
                            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_FLOAT, .33f);

                            Function.Call(Hash.END_SCALEFORM_MOVIE_METHOD);
                            _nextStepTime -= 100;

                            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ROUND_ENDING_STINGER_CUSTOM", "CELEBRATION_SOUNDSET", true);
                        }
                        else if (Game.GameTime < _nextStepTime + 2000)
                        {
                            Function.Call(Hash.DRAW_SCALEFORM_MOVIE_FULLSCREEN, _movie, 255, 255, 255, 255);
                        }
                        else
                        {
                            _step = 3;
                        }
                    }

                    break;

                case 3:
                    unsafe
                    {
                        var movie = _movie;

                        Function.Call(Hash.SET_SCALEFORM_MOVIE_AS_NO_LONGER_NEEDED, &movie);

                        _step = -1;
                    }

                    break;
            }
        }
    }
}