using System.Collections.Generic;
using GTA;
using Waldhari.Behavior.Mission;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;
using Waldhari.Common.UI;

namespace Waldhari.MethLab.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class ManufactureMissionScript : AbstractMissionScript
    {
        private int _amountToManufacture;
        private int _waitUntil;

        public ManufactureMissionScript() : base("ManufactureMission", false,
            null)
        {
        }

        protected override bool StartComplement(string arg)
        {
            if (MethLabSave.Instance.Supply == 0)
            {
                NotificationHelper.ShowFailure("methlab_manufacture_no_supply");
                return false;
            }

            _amountToManufacture = RandomHelper.Next(MethLabOptions.Instance.ManufactureMin,
                MethLabOptions.Instance.ManufactureMax);

            if (_amountToManufacture > MethLabSave.Instance.Supply) _amountToManufacture = MethLabSave.Instance.Supply;

            return true;
        }

        protected override bool OnTickComplement()
        {
            // No need of OnTick for this mission, whatever happens
            return false;
        }

        protected override List<string> EndComplement()
        {
            var product = CalculateProduct();

            MethLabSave.Instance.Supply -= _amountToManufacture;
            MethLabSave.Instance.Product += product;
            MethLabSave.Instance.Save();

            var values = new List<string> { product.ToString() };
            NotificationHelper.ShowFromRon("methlab_manufacture_finished", values);

            return null;
        }

        protected override void FailComplement()
        {
            // Nothing
        }

        private int CalculateProduct()
        {
            var multiplier = RandomHelper.Next(MethLabOptions.Instance.ManufactureMinGramsPerSupply,
                MethLabOptions.Instance.ManufactureMaxGramsPerSupply + 1);
            var substracter = RandomHelper.Next(MethLabOptions.Instance.ManufactureMinGramsPerSupply,
                MethLabOptions.Instance.ManufactureMaxGramsPerSupply + 1);

            var product = _amountToManufacture * multiplier - substracter;

            Logger.Debug("_amountToManufacture=" + multiplier);
            Logger.Debug("multiplier=" + multiplier);
            Logger.Debug("substracter=" + substracter);
            Logger.Debug("product=" + product);

            if (product < 1) product = 1;

            return product;
        }

        protected override void SetupSteps()
        {
            AddStep(GetStepAccept(), false);
            AddStep(GetStepCooking(), false);
        }

        private Step GetStepAccept()
        {
            return new Step
            {
                Name = "Accept",
                MessageKey = null,
                // Nothing to do during this step
                Action = () => { },
                // Go to completion action at the first executing of step
                CompletionCondition = () => true,
                CompletionAction = () =>
                {
                    NotificationHelper.ShowFromRon("methlab_manufacture_started");
                    _waitUntil = Game.GameTime + RandomHelper.Next(1,5) * 60 * 1000;
                }
            };
        }

        private Step GetStepCooking()
        {
            return new Step
            {
                Name = "Cooking",
                MessageKey = null,
                // Nothing to do during this step
                Action = () => { },
                CompletionCondition = () => _waitUntil < Game.GameTime
            };
        }
        
        protected override void CreateScene()
        {
            // Nothing
        }

        protected override void CleanScene()
        {
            // Nothing
        }
    }
}