using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Entities;
using Waldhari.Common.Misc;
using Waldhari.Common.UI;

namespace Waldhari.MethLab.Missions
{
    public class MethLabBulkScript : GenericBulkMissionScript
    {
        public MethLabBulkScript() : base("MethLabBulkScript") {}

        protected override int Amount 
            => MethLabSave.Instance.Product;

        protected override int PriceByUnit 
            => RandomHelper.Next(MethLabOptions.Instance.BulkMinPrice, MethLabOptions.Instance.BulkMaxPrice + 1);
        protected override void DeductAmount(int amount)
        {
            MethLabSave.Instance.Product -= amount;
            MethLabSave.Instance.Save();
        }

        protected override WPosition Parking
            => MethLabHelper.Positions.Parking;
        protected override void ShowStartedMessage()
        {
            NotificationHelper.ShowFromRon("methlab_bulk_started");
        }
    }
}