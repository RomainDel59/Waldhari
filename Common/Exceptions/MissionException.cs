using System;

namespace GTAVMods.Missions
{
    public class MissionException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">Should always be a messageKey to be shown to the player</param>
        public MissionException(string message) : base(message)
        {
        }
    }
}