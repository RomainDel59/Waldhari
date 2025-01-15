using GTA;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Behavior.Mission.Helper;

namespace Waldhari.WeedFarm.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class WeedFarmDefenseScript : GenericDefenseMissionScript
    {
        public WeedFarmDefenseScript() : base("WeedFarmDefenseScript", "weedfarm_defense_success"){}

        protected override void ResetAll()
        {
            WeedFarmSave.Instance.Supply = 0;
            WeedFarmSave.Instance.Product = 0;
            WeedFarmSave.Instance.Save();
        }

        protected override void AddCooldown()
        {
            DefenseMissionHelper.AddCooldown<WeedFarmDefenseScript>();
        }

        protected override string StepDefendingMessageKey => "weedfarm_defense_defending";
    }
}