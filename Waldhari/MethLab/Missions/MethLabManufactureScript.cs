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
        protected override void DoManufacture()
        {
            var multiplier = RandomHelper.Next(
                MethLabOptions.Instance.ManufactureMinGramsPerSupply,
                MethLabOptions.Instance.ManufactureMaxGramsPerSupply + 1
            );
            var substracter = RandomHelper.Next(
                MethLabOptions.Instance.ManufactureMinGramsPerSupply,
                MethLabOptions.Instance.ManufactureMaxGramsPerSupply + 1
            );

            var amount = RandomHelper.Next(
                MethLabOptions.Instance.ManufactureMin,
                MethLabOptions.Instance.ManufactureMax + 1
            );
            
            var product = amount * multiplier - substracter;

            if (amount > MethLabSave.Instance.Supply) 
                amount = MethLabSave.Instance.Supply;
            
            MethLabSave.Instance.Supply -= amount;
            MethLabSave.Instance.Product += product;
            MethLabSave.Instance.Save();
            
            Logger.Info($"{Name} -> Used amount: {amount}, made product: {product}");
        }
        protected override PedHash PedHash => PedHash.MethMale01;
        protected override WPosition Position => MethLabHelper.Positions.GetWorkstation();
        protected override string AnimationDictionary => "anim@amb@business@meth@meth_monitoring_cooking@cooking@";
        protected override string AnimationName => "chemical_pour_short_cooker";
    }
}