using GTA.Native;

namespace Waldhari.Common.UI
{
    public static class SoundHelper
    {
        public static void PlayMissionSuccess()
        {
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "package_delivered_success", "DLC_GR_Generic_Mission_Sounds", true);
        }

        public static void PlayMissionFailure()
        {
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ScreenFlash", "MissionFailedSounds", true);
        }
        
        public static void PlaySuccess()
        {
            //todo: find the good sound for success in game
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "PURCHASE", "HUD_LIQUOR_STORE_SOUNDSET");
        }
        
        public static void PlayEmail()
        {
            //todo: find the good sound for email received in game
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "PURCHASE", "HUD_LIQUOR_STORE_SOUNDSET");
        }

        public static void PlayPayment()
        {
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "PURCHASE", "HUD_LIQUOR_STORE_SOUNDSET");
        }

        public static void PlayFailure()
        {
            //todo: find the good sound for failure in game
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "OTHER_TEXT", "HUD_AWARDS");
        }

        public static void PlayDisappear()
        {
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "ROUND_ENDING_STINGER_CUSTOM", "CELEBRATION_SOUNDSET", true);
        }
    }
}