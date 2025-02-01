using GTA;
using Waldhari.Common.Behavior.Mission;

namespace Waldhari.WeedFarm.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class WeedFarmGuardScript : GenericGuardScript
    {
        public WeedFarmGuardScript() : base("WeedFarmGuardScript")
        {
            NumberOfGuards = WeedFarmSave.Instance.Guards;
            GuardPositionsList = WeedFarmHelper.Positions.GuardPositionsList;
        }
        
        protected override void Save()
        {
            WeedFarmSave.Instance.Guards = NumberOfGuards;
            WeedFarmSave.Instance.Save();
        }
    }
}