using System.Collections.Generic;
using GTA;
using Waldhari.Common.Behavior.Ped;
using Waldhari.Common.Entities;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Exceptions;
using Waldhari.Common.UI;

namespace Waldhari.Common.Behavior.Mission
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public abstract class GenericDealMissionScript : AbstractMissionScript
    {
        // Scene
        private PedActingScript _clientScript;
        private WBlip _cabinetBlip;
        
        protected int _amount;
        protected int _price;
        
        protected abstract int Amount { get; }
        protected abstract int PriceByUnit { get; }
        protected abstract void DeductAmount(int amount);
        protected abstract WPosition Storage { get; }
        protected abstract string GetProductMessage { get; }
        protected abstract void ShowStartedMessage();

        protected GenericDealMissionScript(string name)
            : base(name, true, "deal_success") {}
        
        protected override void StartComplement()
        {
            _amount = Amount;
            if (_amount <= 0)
                throw new MissionException("no_product");
            //Amount is deducted when player take it from the cabinet

            _price = _amount * PriceByUnit;
            
            ShowStartedMessage();
        }
        
        protected override bool OnTickComplement()
        {
            if (_clientScript?.WPed?.Ped == null || _clientScript.WPed.Ped.IsDead)
                throw new MissionException("deal_fail_client_dead");

            return true;
        }
        
        protected override List<string> EndComplement()
        {
            SoundHelper.PlayPayment();
            Game.Player.Money += _price;
            Game.DoAutoSave();

            return new List<string> { _price.ToString() };
        }
        
        protected override void FailComplement()
        {
            // Nothing
        }
        
        protected override void SetupSteps()
        {
            AddWantedStep();
            AddRivalStep();
            AddStep(GetStepGetProduct(), false);
            AddStep(GetStepRendezvous(), false);
            AddStep(GetStepPayment(), false);
        }
        
        private Step GetStepGetProduct()
        {
            return new Step
            {
                Name = "GetProduct",
                MessageKey = GetProductMessage,
                Action = () =>
                {
                    _cabinetBlip.Create();
                    MarkerHelper.DrawGroundMarkerOnBlip(_cabinetBlip, 1);
                },
                CompletionCondition = () =>
                    WPositionHelper.IsNear(Game.Player.Character.Position,_cabinetBlip.Position,1),
                CompletionAction = () =>
                {
                    DeductAmount(_amount);
                    SoundHelper.PlayTake();
                }
            };
        }
        
        private Step GetStepRendezvous()
        {
            return new Step
            {
                Name = "Rendezvous",
                MessageKey = "deal_step_rendezvous",
                Action = () =>
                {
                    _cabinetBlip.Remove();
                    _clientScript.WPed.MakeMissionDestination("client");
                },
                CompletionCondition = () =>
                    WPositionHelper.IsNear(Game.Player.Character.Position,_clientScript.WPed.Ped.Position,25)
            };
        }
        
        private Step GetStepPayment()
        {
            return new Step
            {
                Name = "Payment",
                MessageKey = "deal_step_payment",
                Action = () =>
                {
                    _clientScript.WPed.MakeMissionDestination("supply_supplier");
                },
                CompletionCondition = () =>
                    !_clientScript.WPed.Ped.IsInCombat &&
                    WPositionHelper.IsNear(Game.Player.Character.Position,_clientScript.WPed.Ped.Position, 2)
            };
        }
        
        protected override void CreateScene()
        {
            _cabinetBlip = WBlipHelper.GetMission("storage");
            _cabinetBlip.Position = Storage.Position;
            
            var client = new WPed
            { 
                PedHash = PedHash.Rurmeth01AMM,
                InitialPosition = WPositionHelper.GetRandomAlonePedPosition(),
                Scenario = "WORLD_HUMAN_SMOKING"
            };
            client.Create();
            client.AddWeapon(WeaponsHelper.GetRandomGangWeapon());
            
            client.Ped.RelationshipGroup = Game.Player.Character.RelationshipGroup;
            
            _clientScript = InstantiateScript<PedActingScript>();
            _clientScript.WPed = client;
        }
        
        protected override void CleanScene()
        {
            _clientScript?.WPed?.Ped?.MarkAsNoLongerNeeded();
            _clientScript?.Abort();
            
            _cabinetBlip?.Remove();
        }

    }
}