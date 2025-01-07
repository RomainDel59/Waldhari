using System.Collections.Generic;
using GTA;
using GTA.Math;

namespace Waldhari.MethLab.Missions
{
    public class SupplyMission : AbstractMission
    {
        [ScriptAttributes(NoDefaultInstance = true)]
        // Scene
        private PedGroup _group;
        private WPed _seller;
        private List<WPed> _guards;
        private WVehicle _van;
        private readonly WBlip _deliveryWBlip;

        private int _amountToSupply;
        private int _costToSupply;

        public SupplyMission() : base("SupplyMission", true, "supply_success")
        {
            _deliveryWBlip = new WBlip("supply_destination")
            {
                Position = MethLabPositions.LabParking
            };
        }

        protected override bool StartComplement(string arg)
        {
            _amountToSupply = int.Parse(arg);
            _costToSupply = (int)(_amountToSupply * ModOptions.Instance.SupplyCost * GetNegotiation());

            if (Game.Player.Money < _costToSupply)
            {
                NotificationHelper.ShowFail("supply_not_enough_money");
                return false;
            }

            NotificationHelper.ShowFromRon("supply_started_ron", new List<string> { arg });

            return true;
        }

        protected override void UpdateComplement()
        {
            if (_seller == null || _seller.GtaPed.IsDead) throw new MissionException("supply_fail_supplier_dead");

            if (_van == null || _van.IsDestroyed()) throw new MissionException("supply_fail_vehicle_destroyed");
        }

        protected override List<string> EndComplement()
        {
            ModSave.Instance.Supply += _amountToSupply;
            ModSave.Instance.Save();

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
                Action = () => { _seller.AttachMissionBlip("supply_supplier"); },
                CompletionCondition =
                    () => Game.Player.Character.Position.DistanceTo(_seller.GtaPed.Position) <= 25
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
                    _seller.AttachMissionBlip("supply_supplier");
                    _seller.AttachMissionMarker();
                },
                CompletionCondition =
                    () => !_seller.GtaPed.IsInCombat &&
                          Game.Player.Character.IsInRange(_seller.GtaPed.Position, 2.0f),
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
                    _seller.RemoveBlip();
                    // if player go back from vehicle : show vehicle as mission objective
                    if (!Game.Player.Character.IsInVehicle(_van.GtaVehicle))
                    {
                        _deliveryWBlip.Remove();
                        _van.AttachMissionBlip("van_vehicle");
                        _van.AttachMissionMarker();
                    }
                    // if player is in vehicle : show destination as mission objective
                    else
                    {
                        _van.RemoveBlip();
                        _deliveryWBlip.Create();
                        _deliveryWBlip.AttachMissionMarkerToPositionBlip();
                    }
                },
                CompletionCondition =
                    () => Game.Player.Character.Position.DistanceTo(_deliveryWBlip.Position) <= 10
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
                    _van.RemoveBlip();
                    _deliveryWBlip.Remove();
                },
                CompletionCondition =
                    () => !Game.Player.Character.IsInVehicle(_van.GtaVehicle)
            };
        }

        private static float GetNegotiation()
        {
            if (!ModSave.Instance.Negotiation) return 1;
            return 1 + ModOptions.Instance.NegotiationPercent / 100f;
        }

        protected override void CreateScene()
        {
            _group = new PedGroup();
            _group.Formation = Formation.Loose;

            _seller = new WPed(
                PedHash.CartelGuards01GMM,
                new Vector3(1448.546f, 6548.113f, 15.21889f),
                new Vector3(0, 0, 142.5344f)
            );
            _seller.GiveWeapons();
            _seller.PlayScenario("WORLD_HUMAN_SMOKING");
            _group.Add(_seller.GtaPed, true);

            _guards = new List<WPed>
            {
                new WPed(
                    PedHash.CartelGuards01GMM,
                    new Vector3(1450.007f, 6546.529f, 15.24918f),
                    new Vector3(0f, 0f, 109.082f)
                ),
                new WPed(
                    PedHash.CartelGuards02GMM,
                    new Vector3(1446.683f, 6548.604f, 15.23975f),
                    new Vector3(0f, 0f, -166.3122f)
                )
            };
            foreach (var guard in _guards)
            {
                guard.GiveWeapons();
                guard.PlayScenario("WORLD_HUMAN_GUARD_STAND");
                _group.Add(guard.GtaPed, false);
            }

            _van = new WVehicle(
                VehicleHash.Burrito,
                new Vector3(1444.564f, 6552.647f, 15.07594f),
                new Vector3(0f, 0f, 135.7104f)
            );
        }

        protected override void CleanScene()
        {
            if (_seller != null)
            {
                _seller.RemoveBlip();
                if (_seller.GtaPed != null)
                {
                    _seller.GtaPed.MarkAsNoLongerNeeded();
                }
            }

            if (_guards != null)
            {
                foreach (var guard in _guards)
                {
                    if (guard != null && guard.GtaPed != null)
                    {
                        guard.GtaPed.MarkAsNoLongerNeeded();
                    }
                }
            }

            if (_van != null)
            {
                _van.RemoveBlip();
                if (_van.GtaVehicle != null)
                {
                    _van.GtaVehicle.IsConsideredDestroyed = true;
                    _van.GtaVehicle.PreviouslyOwnedByPlayer = false;
                    _van.GtaVehicle.MarkAsNoLongerNeeded();
                }
            }

            if (_deliveryWBlip != null)
            {
                _deliveryWBlip.Remove();
            }

            if (_group != null)
            {
                _group.Delete();
            }
        }
    }
}