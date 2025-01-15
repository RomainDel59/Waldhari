using System;
using GTA;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Entities;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;

namespace Waldhari.MethLab.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class MethLabManufactureScript : GenericManufactureScript
    {
        public MethLabManufactureScript() : base("MethLabManufactureScript"){}
        
        protected override int SupplyAmount => MethLabSave.Instance.Supply;

        protected override int ManufactureTime
            => RandomHelper.Next(
                MethLabOptions.Instance.ManufactureMinTimeInMinutes,
                MethLabOptions.Instance.ManufactureMaxTimeInMinutes + 1);
        
        protected override void DoManufacture()
        {
            var minYield = MethLabOptions.Instance.ManufactureMinMadeGramsPerSupplyKg;
            var maxYield = MethLabOptions.Instance.ManufactureMaxMadeGramsPerSupplyKg + 1;
            var yieldPerKg = RandomHelper.Next(minYield, maxYield);

            var minLoss = MethLabOptions.Instance.ManufactureMinMadeGramsPerSupplyKg/2;
            var maxLoss = MethLabOptions.Instance.ManufactureMaxMadeGramsPerSupplyKg/2;
            var loss = RandomHelper.Next(minLoss, maxLoss);

            var amount = RandomHelper.Next(
                MethLabOptions.Instance.ManufactureMinSupplyUsageInKg,
                MethLabOptions.Instance.ManufactureMaxSupplyUsageInKg + 1
            );

            if (amount > MethLabSave.Instance.Supply)
                amount = MethLabSave.Instance.Supply;

            //at least one gram producted
            var product = Math.Max(1, amount * yieldPerKg - loss);

            MethLabSave.Instance.Supply -= amount;
            MethLabSave.Instance.Product += product;
            MethLabSave.Instance.Save();

            Logger.Info(
                $"{Name} -> " +
                $"Initial supply: {MethLabSave.Instance.Supply + amount}, " +
                $"Used amount: {amount}, " +
                $"Made product: {product}, " +
                $"Remaining supply: {MethLabSave.Instance.Supply}"
            );
        }

        protected override PedHash PedHash => PedHash.MethMale01;
        protected override WPosition Position => MethLabHelper.Positions.GetWorkstation();
        protected override string AnimationDictionary => "anim@amb@business@meth@meth_monitoring_cooking@cooking@";
        protected override string AnimationName => "chemical_pour_short_cooker";
    }
}