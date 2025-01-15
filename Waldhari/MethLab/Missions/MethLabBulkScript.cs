using System.Collections.Generic;
using GTA;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Entities;
using Waldhari.Common.Misc;
using Waldhari.Common.UI;

namespace Waldhari.MethLab.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class MethLabBulkScript : GenericBulkMissionScript
    {
        public MethLabBulkScript() : base("MethLabBulkScript") {}

        protected override int Amount 
            => MethLabSave.Instance.Product;

        protected override int PriceByUnit 
            => RandomHelper.Next(MethLabOptions.Instance.BulkMinPriceByGram, MethLabOptions.Instance.BulkMaxPriceByGram + 1);
        protected override void DeductAmount(int amount)
        {
            MethLabSave.Instance.Product -= amount;
            MethLabSave.Instance.Save();
        }

        protected override WPosition Parking
            => MethLabHelper.Positions.Parking;
        protected override void ShowStartedMessage()
        {
            NotificationHelper.ShowFromRon("methlab_bulk_started", new List<string>{ _price.ToString() });
        }
    }
}