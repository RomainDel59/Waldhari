using System.Collections.Generic;
using GTA;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Entities;
using Waldhari.Common.Misc;

namespace Waldhari.CokeWork.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class CokeWorkDealScript : GenericDealMissionScript
    {
        public CokeWorkDealScript() : base("CokeWorkDealScript") {}
        
        protected override int Amount 
        {
            get
            {
                var product = RandomHelper.Next(CokeWorkOptions.Instance.DealMinGramsPerSale,
                    CokeWorkOptions.Instance.DealMaxGramsPerSale + 1);

                if (product > CokeWorkSave.Instance.Product) product = CokeWorkSave.Instance.Product;

                if (product <= 0)
                {
                    CokeWorkSave.Instance.Product = 0;
                    CokeWorkSave.Instance.Save();

                    return 0;
                }

                return product;
            }
        }
        
        protected override int PriceByUnit 
            => RandomHelper.Next(CokeWorkOptions.Instance.DealMinPriceByGram, CokeWorkOptions.Instance.DealMaxPriceByGram + 1);
        protected override void DeductAmount(int amount)
        {
            CokeWorkSave.Instance.Product -= amount;
            CokeWorkSave.Instance.Save();
        }
        protected override WPosition Storage
            => CokeWorkHelper.Positions.Storage;
        
        protected override string GetProductMessage => "cokework_deal_step_get_product";
        
        protected override void ShowStartedMessage()
        {
            CokeWorkHelper.ShowFromContact("cokework_deal_started", new List<string>{ _amount.ToString() });
        }
    }
}