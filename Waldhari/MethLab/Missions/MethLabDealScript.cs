using GTA;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Entities;
using Waldhari.Common.Misc;
using Waldhari.Common.UI;

namespace Waldhari.MethLab.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class MethLabDealScript : GenericDealMissionScript
    {
        public MethLabDealScript() : base("MethLabDealScript") {}
        
        protected override int Amount 
            => RandomHelper.Next(MethLabOptions.Instance.DealMinGramsPerPack, MethLabOptions.Instance.DealMaxGramsPerPack+1);
        protected override int PriceByUnit 
            => RandomHelper.Next(MethLabOptions.Instance.DealMinPrice, MethLabOptions.Instance.DealMaxPrice + 1);
        protected override void DeductAmount(int amount)
        {
            MethLabSave.Instance.Product -= amount;
            MethLabSave.Instance.Save();
        }
        protected override WPosition Cabinet
            => MethLabHelper.Positions.Cabinet;
        protected override void ShowStartedMessage()
        {
            NotificationHelper.ShowFromRon("methlab_deal_started");
        }
    }
}