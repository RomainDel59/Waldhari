using System.Collections.Generic;
using GTA;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Entities;
using Waldhari.Common.Misc;

namespace Waldhari.WeedFarm.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class WeedFarmDealScript : GenericDealMissionScript
    {
        public WeedFarmDealScript() : base("WeedFarmDealScript") {}
        
        protected override int Amount 
            => RandomHelper.Next(WeedFarmOptions.Instance.DealMinGramsPerSale, WeedFarmOptions.Instance.DealMaxGramsPerSale+1);
        protected override int PriceByUnit 
            => RandomHelper.Next(WeedFarmOptions.Instance.DealMinPriceByGram, WeedFarmOptions.Instance.DealMaxPriceByGram + 1);
        protected override void DeductAmount(int amount)
        {
            WeedFarmSave.Instance.Product -= amount;
            WeedFarmSave.Instance.Save();
        }
        protected override WPosition Storage
            => WeedFarmHelper.Positions.Storage;
        protected override void ShowStartedMessage()
        {
            WeedFarmHelper.ShowFromContact("weedfarm_deal_started", new List<string>{ _amount.ToString() });
        }
    }
}