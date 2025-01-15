using System;
using GTA;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Entities;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;

namespace Waldhari.WeedFarm.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class WeedFarmManufactureScript : GenericManufactureScript
    {
        public WeedFarmManufactureScript() : base("WeedFarmManufactureScript"){}
        
        protected override int SupplyAmount => WeedFarmSave.Instance.Supply;

        protected override int ManufactureTime
            => RandomHelper.Next(
                WeedFarmOptions.Instance.ManufactureMinTimeInMinutes,
                WeedFarmOptions.Instance.ManufactureMaxTimeInMinutes + 1);
        
        protected override void DoManufacture()
        {
            var minYield = WeedFarmOptions.Instance.ManufactureMinMadeGramsPerSupplyKg;
            var maxYield = WeedFarmOptions.Instance.ManufactureMaxMadeGramsPerSupplyKg + 1;
            var yieldPerKg = RandomHelper.Next(minYield, maxYield);

            var minLoss = WeedFarmOptions.Instance.ManufactureMinMadeGramsPerSupplyKg/2;
            var maxLoss = WeedFarmOptions.Instance.ManufactureMaxMadeGramsPerSupplyKg/2;
            var loss = RandomHelper.Next(minLoss, maxLoss);

            var amount = RandomHelper.Next(
                WeedFarmOptions.Instance.ManufactureMinSupplyUsageInKg,
                WeedFarmOptions.Instance.ManufactureMaxSupplyUsageInKg + 1
            );

            if (amount > WeedFarmSave.Instance.Supply)
                amount = WeedFarmSave.Instance.Supply;

            //at least one gram producted
            var product = Math.Max(1, amount * yieldPerKg - loss);

            WeedFarmSave.Instance.Supply -= amount;
            WeedFarmSave.Instance.Product += product;
            WeedFarmSave.Instance.Save();

            Logger.Info(
                $"{Name} -> " +
                $"Initial supply: {WeedFarmSave.Instance.Supply + amount}, " +
                $"Used amount: {amount}, " +
                $"Made product: {product}, " +
                $"Remaining supply: {WeedFarmSave.Instance.Supply}"
            );
        }

        protected override PedHash PedHash => PedHash.WeedMale01;
        protected override WPosition Position => WeedFarmHelper.Positions.GetWorkstation();
        protected override string AnimationDictionary => "amb@world_human_gardener_plant@male@base";
        protected override string AnimationName => "base";

    }
}