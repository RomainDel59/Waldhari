using GTA;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Entities;
using Waldhari.Common.Misc;

namespace Waldhari.MethLab.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class MethLabSupplyScript : GenericSupplyMissionScript
    {
        public MethLabSupplyScript() : base("MethLabSupplyScript"){}
        
        protected override void ShowStartedMessage()
        {
            MethLabHelper.ShowFromContact("methlab_supply_started");
        }
        
        protected override int Amount 
            => RandomHelper.Next(MethLabOptions.Instance.SupplyMinInKg, MethLabOptions.Instance.SupplyMaxInKg + 1);
        
        protected override int CostByUnit
            => RandomHelper.Next(MethLabOptions.Instance.SupplyMinCostPerKg, MethLabOptions.Instance.SupplyMaxCostPerKg + 1);
        
        protected override string StepDriveMessageKey => "methlab_supply_step_drive";
        protected override string DestinationMessageKey => "methlab_parking";
        
        protected override WPosition Parking => MethLabHelper.Positions.Parking;
        
        protected override void AddSupply(int amount)
        {
            MethLabSave.Instance.Supply += amount;
            MethLabSave.Instance.Save();
        }
        
    }
}