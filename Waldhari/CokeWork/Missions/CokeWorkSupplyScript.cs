using GTA;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Entities;
using Waldhari.Common.Misc;

namespace Waldhari.CokeWork.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class CokeWorkSupplyScript : GenericSupplyMissionScript
    {
        public CokeWorkSupplyScript() : base("CokeWorkSupplyScript"){}
        
        protected override void ShowStartedMessage()
        {
            CokeWorkHelper.ShowFromContact("cokework_supply_started");
        }
        
        protected override int Amount 
            => RandomHelper.Next(CokeWorkOptions.Instance.SupplyMinInKg, CokeWorkOptions.Instance.SupplyMaxInKg + 1);
        
        protected override int CostByUnit
            => RandomHelper.Next(CokeWorkOptions.Instance.SupplyMinCostPerKg, CokeWorkOptions.Instance.SupplyMaxCostPerKg + 1);
        
        protected override string StepDriveMessageKey => "cokework_supply_step_drive";
        protected override string DestinationMessageKey => "cokework_parking";
        
        protected override WPosition Parking => CokeWorkHelper.Positions.Parking;
        
        protected override void AddSupply(int amount)
        {
            CokeWorkSave.Instance.Supply += amount;
            CokeWorkSave.Instance.Save();
        }
        
    }
}