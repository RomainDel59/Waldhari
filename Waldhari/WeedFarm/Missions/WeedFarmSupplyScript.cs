using GTA;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Entities;
using Waldhari.Common.Misc;

namespace Waldhari.WeedFarm.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class WeedFarmSupplyScript : GenericSupplyMissionScript
    {
        public WeedFarmSupplyScript() : base("WeedFarmSupplyScript"){}
        
        protected override void ShowStartedMessage()
        {
            WeedFarmHelper.ShowFromContact("weedfarm_supply_started");
        }
        
        protected override int Amount 
            => RandomHelper.Next(WeedFarmOptions.Instance.SupplyMinInKg, WeedFarmOptions.Instance.SupplyMaxInKg + 1);
        
        protected override int CostByUnit
            => RandomHelper.Next(WeedFarmOptions.Instance.SupplyMinCostPerKg, WeedFarmOptions.Instance.SupplyMaxCostPerKg + 1);
        
        protected override string StepDriveMessageKey => "weedfarm_supply_step_drive";
        protected override string DestinationMessageKey => "weedfarm_parking";
        
        protected override WPosition Parking => WeedFarmHelper.Positions.Parking;
        
        protected override void AddSupply(int amount)
        {
            WeedFarmSave.Instance.Supply += amount;
            WeedFarmSave.Instance.Save();
        }
        
    }
}