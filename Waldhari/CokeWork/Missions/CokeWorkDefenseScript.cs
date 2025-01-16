using GTA;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Behavior.Mission.Helper;

namespace Waldhari.CokeWork.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class CokeWorkDefenseScript : GenericDefenseMissionScript
    {
        public CokeWorkDefenseScript() : base("CokeWorkDefenseScript", "cokework_defense_success"){}

        protected override void ResetAll()
        {
            CokeWorkSave.Instance.Supply = 0;
            CokeWorkSave.Instance.Product = 0;
            CokeWorkSave.Instance.Save();
        }

        protected override void AddCooldown()
        {
            DefenseMissionHelper.AddCooldown<CokeWorkDefenseScript>();
        }

        protected override string StepDefendingMessageKey => "cokework_defense_defending";
    }
}