using System.Collections.Generic;
using GTA;
using GTA.Math;
using Waldhari.Behavior.Mission;
using Waldhari.Behavior.Ped;
using Waldhari.Common.Entities;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Exceptions;
using Waldhari.Common.UI;

namespace Waldhari.MethLab.Missions
{
        [ScriptAttributes(NoDefaultInstance = true)]
    public class SupplyMission : AbstractMissionScript
    {
        // Scene
        private WBlip _deliveryWBlip;
        private PedActingScript _sellerScript;
        private WVehicle _van;

        private int _amountToSupply;
        private int _costToSupply;

        public SupplyMission() : base("SupplyMission", true, "supply_success")
        {
        }

        protected override bool StartComplement(string arg)
        {
            _amountToSupply = int.Parse(arg);
            _costToSupply = _amountToSupply * MethLabOptions.Instance.SupplyCost;

            if (Game.Player.Money < _costToSupply)
            {
                NotificationHelper.ShowFailure("supply_not_enough_money");
                return false;
            }

            NotificationHelper.ShowFromRon("supply_started_ron", new List<string> { arg });

            return true;
        }

        protected override void OnTickComplement()
        {
            if (_sellerScript == null || _sellerScript.WPed.Ped.IsDead) throw new MissionException("supply_fail_supplier_dead");

            if (_van == null || _van.Vehicle.IsConsideredDestroyed) throw new MissionException("supply_fail_vehicle_destroyed");
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
                MessageKey = "supply_rendezvous",
                Action = () =>
                {
                    _sellerScript.WPed.MakeMissionDestination("supply_supplier");
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
                MessageKey = "supply_payment",
                Action = () =>
                {
                    _sellerScript.WPed.MakeMissionDestination("supply_supplier");
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
                MessageKey = "supply_drive_lab",
                Action = () =>
                {
                    _sellerScript.WPed.RemoveMissionDestination();
                    
                    // if player go back from vehicle : show vehicle as mission objective
                    if (!Game.Player.Character.IsInVehicle(_van.Vehicle))
                    {
                        //todo: make a step to go inside vehicle
                        _deliveryWBlip.Remove();
                        _van.MakeMissionDestination("van_vehicle");
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
                MessageKey = "supply_out",
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
                InitialPosition = new WPosition
                {
                    Position = new Vector3(1448.546f, 6548.113f, 15.21889f),
                    Rotation = new Vector3(0, 0, 142.5344f)
                    //todo: Add heading
                },
                Scenario = "WORLD_HUMAN_SMOKING"
            };
            seller.Create();
            seller.AddWeapon(WeaponsHelper.GetRandomGangWeapon());

            _van = new WVehicle
            {
                VehicleHash = VehicleHash.Burrito,
                InitialPosition = new WPosition
                {
                    Position = World.GetNextPositionOnStreet(new Vector3(1444.564f, 6552.647f, 15.07594f), true),
                    Rotation = new Vector3(0f, 0f, 135.7104f)
                    //todo: Add heading
                }
            };
            
            _deliveryWBlip = WBlipHelper.GetMission("supply_destination");
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
            }

            _deliveryWBlip?.Remove();
        }
    }
}