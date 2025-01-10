using System.Collections.Generic;
using GTA;
using GTA.Math;
using Waldhari.Behavior.Mission;
using Waldhari.Behavior.Ped;
using Waldhari.Common.Entities;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Exceptions;
using Waldhari.Common.Misc;
using Waldhari.Common.UI;

namespace Waldhari.MethLab.Missions
{
        [ScriptAttributes(NoDefaultInstance = true)]
    public class SupplyMissionScript : AbstractMissionScript
    {
        // Scene
        private WBlip _deliveryWBlip;
        private PedActingScript _sellerScript;
        private WVehicle _van;

        private int _amountToSupply;
        private int _costToSupply;

        public SupplyMissionScript() : base("MethLabSupplyMission", true, "methlab_supply_success")
        {
        }

        protected override void StartComplement()
        {
            _amountToSupply = RandomHelper.Next(MethLabOptions.Instance.SupplyMin, MethLabOptions.Instance.SupplyMax+1);
            _costToSupply = _amountToSupply * RandomHelper.Next(MethLabOptions.Instance.SupplyMinCost, MethLabOptions.Instance.SupplyMaxCost+1);

            if (Game.Player.Money < _costToSupply) 
                throw new MissionException("methlab_supply_not_enough_money");

            NotificationHelper.ShowFromRon("methlab_supply_started", new List<string> { _amountToSupply.ToString() });
        }

        protected override bool OnTickComplement()
        {
            
            if (_sellerScript == null || _sellerScript.WPed.Ped.IsDead) throw new MissionException("methlab_supply_fail_supplier_dead");

            if (_van == null || _van.Vehicle.IsConsideredDestroyed) throw new MissionException("methlab_supply_fail_vehicle_destroyed");

            return true;
        }

        protected override List<string> EndComplement()
        {
            MethLabSave.Instance.Supply += _amountToSupply;
            MethLabSave.Instance.Save();

            return new List<string> { _amountToSupply.ToString() };
        }

        protected override void FailComplement()
        {
            // Nothing
        }

        protected override void SetupSteps()
        {
            AddWantedStep();
            AddRivalStep();
            AddStep(GetStepRendezvous(), false);
            AddStep(GetStepPayment());
            AddStep(GetStepDrive(), false);
            AddStep(GetStepOut());
        }

        private Step GetStepRendezvous()
        {
            return new Step
            {
                Name = "Rendezvous",
                MessageKey = "methlab_supply_rendezvous",
                Action = () =>
                {
                    _sellerScript.WPed.MakeMissionDestination("methlab_supply_supplier");
                },
                CompletionCondition =
                    () => WPositionHelper.IsNear(Game.Player.Character.Position,_sellerScript.WPed.Ped.Position,25)
            };
        }

        private Step GetStepPayment()
        {
            return new Step
            {
                Name = "Payment",
                MessageKey = "methlab_supply_payment",
                Action = () =>
                {
                    _sellerScript.WPed.MakeMissionDestination("methlab_supply_supplier");
                },
                CompletionCondition =
                    () => !_sellerScript.WPed.Ped.IsInCombat &&
                          WPositionHelper.IsNear(Game.Player.Character.Position,_sellerScript.WPed.Ped.Position, 2),
                // Pay seller when completed
                CompletionAction = () =>
                {
                    SoundHelper.PlayPayment();
                    Game.Player.Money -= _costToSupply;
                    Game.DoAutoSave();
                }
            };
        }

        private Step GetStepDrive()
        {
            return new Step
            {
                Name = "Drive",
                MessageKey = "methlab_supply_drive_lab",
                Action = () =>
                {
                    _sellerScript.WPed.RemoveMissionDestination();
                    
                    // if player go back from vehicle : show vehicle as mission objective
                    if (!Game.Player.Character.IsInVehicle(_van.Vehicle))
                    {
                        //todo: make a step to go inside vehicle
                        _deliveryWBlip.Remove();
                        _van.MakeMissionDestination("vehicle_van");
                    }
                    // if player is in vehicle : show destination as mission objective
                    else
                    {
                        _van.RemoveMissionDestination();
                        _deliveryWBlip.Create();
                        MarkerHelper.DrawGroundMarkerOnBlip(_deliveryWBlip);
                    }
                },
                CompletionCondition =
                    () => WPositionHelper.IsNear(Game.Player.Character.Position,_deliveryWBlip.Position, 10)
            };
        }

        private Step GetStepOut()
        {
            return new Step
            {
                Name = "Out",
                MessageKey = "methlab_supply_out",
                Action = () =>
                {
                    _deliveryWBlip.Remove();
                },
                CompletionCondition =
                    () => !Game.Player.Character.IsInVehicle(_van.Vehicle)
            };
        }

        protected override void CreateScene()
        {
            //todo: there is a position for a ped (wholesaler) and a position for a vehicle (for the van that has to be driven there)
            // make a new object for that, like "scene" object ?
            // this object can have multiple ped positions, and one vehicle position,
            // so the script can create multiple peds
            
            var seller = new WPed
            {
                PedHash = PedHash.CartelGuards01GMM,
                InitialPosition = new WPosition{
                    Position = new Vector3(1450.669f, 6543.706f, 15.30578f),
                    Rotation = new Vector3(0f, 0f, 127.8216f),
                    Heading = 127.8216f
                },
                Scenario = "WORLD_HUMAN_SMOKING"
            };
            seller.Create();
            seller.AddWeapon(WeaponsHelper.GetRandomGangWeapon());
            
            _sellerScript = InstantiateScript<PedActingScript>();
            _sellerScript.WPed = seller;

            _van = new WVehicle
            {
                VehicleHash = VehicleHash.Burrito,
                InitialPosition = new WPosition{
                    Position = new Vector3(1444.813f, 6553.167f, 15.05603f),
                    Rotation = new Vector3(0f, 0f, 138.7879f),
                    Heading = 138.7879f
                }
            };
            _van.Create();
            
            _deliveryWBlip = WBlipHelper.GetMission("methlab_supply_destination");
            _deliveryWBlip.Position =  MethLabHelper.LabParking;
        }

        protected override void CleanScene()
        {
            _sellerScript?.WPed?.Ped?.MarkAsNoLongerNeeded();
            _sellerScript?.Abort();

            if (_van != null)
            {
                _van.WBlip?.Remove();
                _van.Vehicle?.MarkAsNoLongerNeeded();
                if(_van.Vehicle != null)
                {
                    if (Game.Player.Character.IsInVehicle(_van.Vehicle))
                    {
                        Game.Player.Character.Task.LeaveVehicle();
                    }
                    _van.Vehicle.IsConsideredDestroyed = true;
                }
            }

            _deliveryWBlip?.Remove();
        }
    }
}