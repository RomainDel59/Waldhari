using GTA;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Entities;
using Waldhari.Common.Misc;
using Waldhari.Common.UI;

namespace Waldhari.MethLab.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class MethLabSupplyScript : GenericSupplyMissionScript
    {
        public MethLabSupplyScript() : base("MethLabSupplyScript"){}
        
        protected override void ShowStartedMessage()
        {
            NotificationHelper.ShowFromRon("supply_started");
        }
        
        protected override int Amount 
            => RandomHelper.Next(MethLabOptions.Instance.SupplyMin, MethLabOptions.Instance.SupplyMax + 1);
        
        protected override int CostByUnit
            => RandomHelper.Next(MethLabOptions.Instance.SupplyMinCost, MethLabOptions.Instance.SupplyMaxCost + 1);
        
        protected override string StepDriveMessageKey => "methlab_supply_step_drive";
        protected override string DestinationMessageKey => "methlab_supply_destination";
        
        protected override WPosition Parking => MethLabHelper.Positions.Parking;
        
        protected override void AddSupply(int amount)
        {
            MethLabSave.Instance.Supply += amount;
            MethLabSave.Instance.Save();
        }
        
    }
}