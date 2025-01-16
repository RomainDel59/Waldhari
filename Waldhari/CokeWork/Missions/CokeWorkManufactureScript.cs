using System;
using GTA;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Entities;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;

namespace Waldhari.CokeWork.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class CokeWorkManufactureScript : GenericManufactureScript
    {
        public CokeWorkManufactureScript() : base("CokeWorkManufactureScript"){}
        
        protected override int SupplyAmount => CokeWorkSave.Instance.Supply;

        protected override int ManufactureTime
            => RandomHelper.Next(
                CokeWorkOptions.Instance.ManufactureMinTimeInMinutes,
                CokeWorkOptions.Instance.ManufactureMaxTimeInMinutes + 1);
        
        protected override void DoManufacture()
        {
            var minYield = CokeWorkOptions.Instance.ManufactureMinMadeGramsPerSupplyKg;
            var maxYield = CokeWorkOptions.Instance.ManufactureMaxMadeGramsPerSupplyKg + 1;
            var yieldPerKg = RandomHelper.Next(minYield, maxYield);

            var minLoss = CokeWorkOptions.Instance.ManufactureMinMadeGramsPerSupplyKg/2;
            var maxLoss = CokeWorkOptions.Instance.ManufactureMaxMadeGramsPerSupplyKg/2;
            var loss = RandomHelper.Next(minLoss, maxLoss);

            var amount = RandomHelper.Next(
                CokeWorkOptions.Instance.ManufactureMinSupplyUsageInKg,
                CokeWorkOptions.Instance.ManufactureMaxSupplyUsageInKg + 1
            );

            if (amount > CokeWorkSave.Instance.Supply)
                amount = CokeWorkSave.Instance.Supply;

            //at least one gram produced
            var product = Math.Max(1, amount * yieldPerKg - loss);

            CokeWorkSave.Instance.Supply -= amount;
            CokeWorkSave.Instance.Product += product;
            CokeWorkSave.Instance.Save();

            Logger.Info(
                $"{Name} -> " +
                $"Initial supply: {CokeWorkSave.Instance.Supply + amount}, " +
                $"Used amount: {amount}, " +
                $"Made product: {product}, " +
                $"Remaining supply: {CokeWorkSave.Instance.Supply}"
            );
        }

        protected override PedHash PedHash => PedHash.MexThug01AMY;
        protected override WPosition Position => CokeWorkHelper.Positions.GetWorkstation();
        protected override string AnimationDictionary => "amb@world_human_hang_out_street@male_c@base";
        protected override string AnimationName => "base";

    }
}