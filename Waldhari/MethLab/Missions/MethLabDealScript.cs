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
        {
            get
            {
                var product = RandomHelper.Next(MethLabOptions.Instance.DealMinGramsPerSale,
                    MethLabOptions.Instance.DealMaxGramsPerSale + 1);

                if (product > MethLabSave.Instance.Product) product = MethLabSave.Instance.Product;

                if (product <= 0)
                {
                    MethLabSave.Instance.Product = 0;
                    MethLabSave.Instance.Save();

                    return 0;
                }

                return product;
            }
        }
        
        protected override int PriceByUnit 
            => RandomHelper.Next(MethLabOptions.Instance.DealMinPriceByGram, MethLabOptions.Instance.DealMaxPriceByGram + 1);
        protected override void DeductAmount(int amount)
        {
            MethLabSave.Instance.Product -= amount;
            MethLabSave.Instance.Save();
        }
        protected override WPosition Storage
            => MethLabHelper.Positions.Storage;
        
        protected override string GetProductMessage => "methlab_deal_step_get_product";
        
        protected override void ShowStartedMessage()
        {
            MethLabHelper.ShowFromContact("methlab_deal_started", new List<string>{ _amount.ToString() });
        }
    }
}