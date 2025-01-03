using GTA;
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
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "PURCHASE", "HUD_LIQUOR_STORE_SOUNDSET");
        }

        public static void PlayPayment()
        {
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "PURCHASE", "HUD_LIQUOR_STORE_SOUNDSET");
        }

        public static void PlayFailure()
        {
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "OTHER_TEXT", "HUD_AWARDS");
        }

        public static void PedSays(Ped ped, string text)
        {
            Function.Call(Hash.PLAY_PED_AMBIENT_SPEECH_NATIVE, ped, text,
                "Speech_Params_Force_Normal_Clear");
        }
    }
}