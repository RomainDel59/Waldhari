using System.Collections.Generic;
using GTA;
using Waldhari.Common.Behavior.Ped;
using Waldhari.Common.Entities;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Files;

namespace Waldhari.Common.Behavior.Mission
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public abstract class GenericManufactureScript : AbstractMissionScript
    {
        // Param
        public PedActingScript WorkerScript;
        
        protected abstract int SupplyAmount { get; }
        protected abstract int ManufactureTime { get; }
        protected abstract void DoManufacture();
        protected abstract PedHash PedHash { get; }
        protected abstract WPosition Position { get; }
        protected abstract string AnimationDictionary { get; }
        protected abstract string AnimationName { get; }


        protected GenericManufactureScript(string name)
            : base(name, false, null)
        {
            CheckIfAnotherMissionIsActiveToLaunch = false;
        }

        protected override void StartComplement()
        {
            // Nothing to do
        }

        private int _nextTick;
        protected override bool OnTickComplement()
        {
            // Script is executed every 5 seconds only
            if(_nextTick > Game.GameTime) return false;
            _nextTick = Game.GameTime + 5 * 1000;
            
            // Nothing will stop the script
            ExecuteStep();
            return false;
        }
        
        protected override List<string> EndComplement()
        {
            Logger.Warning("GenericManufactureScript: EndComplement should never be called");
            return null;
        }
        
        protected override void FailComplement()
        {
            Logger.Warning("GenericManufactureScript: FailComplement should never be called");
        }
        
        protected override void SetupSteps()
        {
            AddStep(GetStepCheckSupply(), false);
            AddStep(GetStepWaitEnd(), false);
        }
        
        private int _waitEndTime;
        private Step GetStepCheckSupply()
        {
            return new Step
            {
                Name = "CheckSupply",
                MessageKey = null,
                Action = () => { },
                CompletionCondition =
                    () => SupplyAmount > 0,
                CompletionAction = () =>
                {
                    var minutes = ManufactureTime;
                    Logger.Debug($"Will wait {minutes} minutes before complete manufacture");
                    _waitEndTime = Game.GameTime + minutes * 60 * 1000;
                }
            };
        }
        
        private Step GetStepWaitEnd()
        {
            return new Step
            {
                Name = "WaitEnd",
                MessageKey = null,
                Action = () => { },
                CompletionCondition =
                    () => _waitEndTime < Game.GameTime,
                CompletionAction = () =>
                {
                    DoManufacture();
                    CurrentStep = GetFirstStep()-1;
                }
            };
        }
        
        protected override void CreateScene()
        {
            if (WorkerScript == null)
            {
                Logger.Debug($"Instantiating Worker script for {Name}...");
                WorkerScript = InstantiateScript<PedActingScript>();
                WorkerScript.StopActing();
                
                WorkerScript.WPed = new WPed
                {
                    PedHash = PedHash,
                    InitialPosition = Position
                };
                WorkerScript.WPed.Create();
                WorkerScript.WPed.AddWeapon(WeaponsHelper.GetRandomGangWeapon());
                WorkerScript.WPed.AddWeapon(WeaponsHelper.GetRandomGangWeapon());
                WorkerScript.WPed.Ped.Weapons.Select(WeaponHash.Unarmed);
                WorkerScript.WPed.Ped.RelationshipGroup = Game.Player.Character.RelationshipGroup;
            }
            
            WorkerScript.WPed.Ped.IsInvincible = true;
            WorkerScript.WPed.InitialPosition = Position;
            WorkerScript.WPed.Scenario = null;
            WorkerScript.WPed.AnimationDictionnary = AnimationDictionary;
            WorkerScript.WPed.AnimationName = AnimationName;
            WorkerScript.ActWithoutWeapon = true;
            WorkerScript.RestartActing();
            
        }
        
        protected override void CleanScene()
        {
            Logger.Warning("GenericManufactureScript: CleanScene should never be called");
        }
        
        
    }
}