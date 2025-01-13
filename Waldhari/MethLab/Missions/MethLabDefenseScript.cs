using GTA;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Behavior.Mission.Helper;

namespace Waldhari.MethLab.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class MethLabDefenseScript : GenericDefenseMissionScript
    {
        public MethLabDefenseScript() : base("MethLabDefenseScript", "methlab_defense_success"){}

        protected override void ResetAll()
        {
            MethLabSave.Instance.Supply = 0;
            MethLabSave.Instance.Product = 0;
            MethLabSave.Instance.Save();
        }

        protected override void AddCooldown()
        {
            DefenseMissionHelper.AddCooldown<MethLabDefenseScript>();
        }

        protected override string StepDefendingMessageKey => "methlab_defense_defending";
    }
}