using System.Collections.Generic;
using GTA;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Entities;
using Waldhari.Common.Misc;

namespace Waldhari.MethLab.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class MethLabDealScript : GenericDealMissionScript
    {
        public MethLabDealScript() : base("MethLabDealScript") {}
        
        protected override int Amount 
            => RandomHelper.Next(MethLabOptions.Instance.DealMinGramsPerSale, MethLabOptions.Instance.DealMaxGramsPerSale+1);
        protected override int PriceByUnit 
            => RandomHelper.Next(MethLabOptions.Instance.DealMinPriceByGram, MethLabOptions.Instance.DealMaxPriceByGram + 1);
        protected override void DeductAmount(int amount)
        {
            MethLabSave.Instance.Product -= amount;
            MethLabSave.Instance.Save();
        }
        protected override WPosition Cabinet
            => MethLabHelper.Positions.Cabinet;
        protected override void ShowStartedMessage()
        {
            MethLabHelper.ShowFromContact("methlab_deal_started", new List<string>{ _amount.ToString() });
        }
    }
}