using System.Collections.Generic;
using GTA;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Entities;
using Waldhari.Common.Misc;

namespace Waldhari.WeedFarm.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class WeedFarmBulkScript : GenericBulkMissionScript
    {
        public WeedFarmBulkScript() : base("WeedFarmBulkScript") {}

        protected override int Amount 
            => WeedFarmSave.Instance.Product;

        protected override int PriceByUnit 
            => RandomHelper.Next(WeedFarmOptions.Instance.BulkMinPriceByGram, WeedFarmOptions.Instance.BulkMaxPriceByGram + 1);
        protected override void DeductAmount(int amount)
        {
            WeedFarmSave.Instance.Product -= amount;
            WeedFarmSave.Instance.Save();
        }

        protected override WPosition Parking
            => WeedFarmHelper.Positions.Parking;
        protected override void ShowStartedMessage()
        {
            WeedFarmHelper.ShowFromContact("weedfarm_bulk_started", new List<string>{ _price.ToString() });
        }
    }
}