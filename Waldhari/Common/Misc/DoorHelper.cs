using GTA.Native;

namespace Waldhari.Common.Misc
{
    public static class DoorHelper
    {
        private enum State {
            Invalid = -1,
            Unlocked = 0,
            Locked = 1,
            ForceLockedUntilOutOfArea = 2,
            ForceUnlockedThisFrame = 3,
            ForceLockedThisFrame = 4,
            ForceOpenThisFrame = 5,
            ForceClosedThisFrame = 6
        };

        public static void Open(int door, bool open)
        {
            var state = open ? State.Unlocked : State.Locked;
            
            //DOOR_SYSTEM_SET_DOOR_STATE(Hash doorHash, int state, bool requestDoor, bool forceUpdate)
            Function.Call(Hash.DOOR_SYSTEM_SET_DOOR_STATE, door, state, false,false);
        }
    }
}