using System.Collections.Generic;
using GTA;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Entities;
using Waldhari.Common.Misc;

namespace Waldhari.CokeWork.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class CokeWorkBulkScript : GenericBulkMissionScript
    {
        public CokeWorkBulkScript() : base("CokeWorkBulkScript") {}

        protected override int Amount 
            => CokeWorkSave.Instance.Product;

        protected override int PriceByUnit 
            => RandomHelper.Next(CokeWorkOptions.Instance.BulkMinPriceByGram, CokeWorkOptions.Instance.BulkMaxPriceByGram + 1);
        protected override void DeductAmount(int amount)
        {
            CokeWorkSave.Instance.Product -= amount;
            CokeWorkSave.Instance.Save();
        }

        protected override WPosition Parking
            => CokeWorkHelper.Positions.Parking;
        protected override void ShowStartedMessage()
        {
            CokeWorkHelper.ShowFromContact("cokework_bulk_started", new List<string>{ _price.ToString() });
        }
    }
}