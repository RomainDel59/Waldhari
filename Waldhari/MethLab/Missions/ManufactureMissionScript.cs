using System.Collections.Generic;
using GTA;
using Waldhari.Behavior.Mission;
using Waldhari.Common.Exceptions;
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

        protected override void StartComplement()
        {
            if (MethLabSave.Instance.Supply == 0) 
                throw new MissionException("methlab_manufacture_no_supply");

            _amountToManufacture = RandomHelper.Next(MethLabOptions.Instance.ManufactureMin,
                MethLabOptions.Instance.ManufactureMax);

            if (_amountToManufacture > MethLabSave.Instance.Supply) _amountToManufacture = MethLabSave.Instance.Supply;
        }

        protected override bool OnTickComplement()
        {
            // Never need to test if player is dead,
            // or if police is running on the player : 
            // so it executes step directly and passes
            // the rest of OnTick method
            ExecuteStep();
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
                    // between 1 and 5 minutes (60 secondes)
                    var minutes = RandomHelper.Next(1, 5);
                    _waitUntil = Game.GameTime + minutes * 60 * 1000;
                    Logger.Debug($"Will wait for {minutes} minutes");
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