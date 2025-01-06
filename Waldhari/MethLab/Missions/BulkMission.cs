using System.Collections.Generic;
using GTA;
using GTA.Math;
using Waldhari.Common.Entities;
using Waldhari.Common.Exceptions;
using Waldhari.Common.Misc;
using Waldhari.Common.Mission;
using Waldhari.Common.UI;
using Waldhari.MethLab;

namespace GTAVMods.Missions
{
    public class BulkMission : AbstractMission
    {
        // Scene
        private WBlip _deliveryWBlip;
        private WPed _wholesaler;
        private WVehicle _van;

        private int _amountToDeal;
        private int _priceToDeal;

        public BulkMission() : base("BulkMission", true, "bulk_success")
        {
        }

        protected override bool StartComplement(string arg)
        {
            if (MethLabSave.Instance.Product <= 0)
            {
                NotificationHelper.ShowFailure("deal_no_product");
                return false;
            }

            _amountToDeal = MethLabSave.Instance.Product;
            _priceToDeal = _amountToDeal * GetPricePerGram();

            NotificationHelper.ShowFromRon("bulk_started_ron",
                new List<string> { _priceToDeal.ToString() });

            MethLabSave.Instance.Product -= _amountToDeal;
            MethLabSave.Instance.Save();

            return true;
        }

        protected override void UpdateComplement()
        {
            if (_wholesaler == null || _wholesaler.Ped.IsDead) throw new MissionException("bulk_fail_wholesaler_dead");

            if (_van == null || _van.Vehicle == null || _van.Vehicle.IsConsideredDestroyed) throw new MissionException("bulk_fail_vehicle_destroyed");
        }

        protected override List<string> EndComplement()
        {
            Game.Player.Money += _priceToDeal;
            Game.DoAutoSave();

            return new List<string> { _priceToDeal.ToString() };
        }

        protected override void FailComplement()
        {
            // Nothing
        }

        protected override void SetupSteps()
        {
            AddWantedStep();
            AddRivalStep();
            AddStep(GetStepEnterVehicle(), false);
            AddStep(GetStepRendezvous());
            AddStep(GetStepGetOutVehicle());
            AddStep(GetStepPayment(), false);
        }

        private Step GetStepEnterVehicle()
        {
            return new Step
            {
                Name = "EnterVehicle",
                MessageKey = "bulk_in",
                Action =
                    () =>
                    {
                        _deliveryWBlip.Remove();
                        _van.MakeMissionDestination();
                    },
                CompletionCondition =
                    () => Game.Player.Character.IsInVehicle(_van.Vehicle)
            };
        }

        private Step GetStepRendezvous()
        {
            return new Step
            {
                Name = "Rendezvous",
                MessageKey = "bulk_rendezvous",
                Action =
                    () =>
                    {
                        _van.RemoveMissionDestination();
                        _deliveryWBlip.Create();
                        MarkerHelper.DrawEntityMarkerOnBlip(_deliveryWBlip);
                    },
                CompletionCondition =
                    () => Game.Player.Character.Position.DistanceTo(_deliveryWBlip.Position) <= 10
            };
        }

        private Step GetStepGetOutVehicle()
        {
            return new Step
            {
                Name = "GetOutVehicle",
                MessageKey = "bulk_out",
                Action =
                    () => { _deliveryWBlip.Remove(); },
                CompletionCondition =
                    () => !Game.Player.Character.IsInVehicle(_van.Vehicle),
                CompletionAction =
                    () =>
                    {
                        _van.Vehicle.IsPersistent = false;
                        _van.Vehicle.IsConsideredDestroyed = true;
                        _van.Vehicle.PreviouslyOwnedByPlayer = false;
                        _van.Vehicle.MarkAsNoLongerNeeded();
                    }
            };
        }

        private Step GetStepPayment()
        {
            return new Step
            {
                Name = "Payment",
                MessageKey = "bulk_payment",
                Action = () =>
                {
                    _wholesaler.AttachMissionBlip("bulk_wholesaler");
                    _wholesaler.AttachMissionMarker();
                },
                CompletionCondition =
                    () => !_wholesaler.GtaPed.IsInCombat &&
                          Game.Player.Character.IsInRange(_wholesaler.GtaPed.Position, 2.0f)
            };
        }

        protected override void CreateScene()
        {
            _wholesaler = new WPed(
                PedHash.CartelGuards01GMM,
                new Vector3(1448.546f, 6548.113f, 15.21889f),
                new Vector3(0, 0, 142.5344f)
            );
            _wholesaler.GiveWeapons();
            _wholesaler.PlayScenario("WORLD_HUMAN_SMOKING");

            _deliveryWBlip = new WBlip("bulk_delivery");
            _deliveryWBlip.Position = new Vector3(1444.564f, 6552.647f, 15.07594f);

            _van = new WVehicle(VehicleHash.Burrito, World.GetNextPositionOnStreet(MethLabPositions.LabParking, true));
        }

        protected override void CleanScene()
        {
            if (_wholesaler != null)
            {
                _wholesaler.RemoveBlip();

                if (_wholesaler.GtaPed != null)
                {
                    _wholesaler.GtaPed.IsPositionFrozen = false;
                    _wholesaler.GtaPed.IsPersistent = false;
                    _wholesaler.GtaPed.MarkAsNoLongerNeeded();
                }
            }

            if (_van != null)
            {
                _van.RemoveBlip();

                if (_van.GtaVehicle != null)
                {
                    _van.GtaVehicle.IsPersistent = false;
                    _van.GtaVehicle.MarkAsNoLongerNeeded();
                }
            }

            if (_deliveryWBlip != null)
            {
                _deliveryWBlip.Remove();
            }
        }

        private static int GetPricePerGram()
        {
            return
                RandomHelper.Next(ModOptions.Instance.BulkMinPrice, ModOptions.Instance.BulkMaxPrice + 1) *
                (1 + (MethLabSave.Instance.Blue ? ModOptions.Instance.BluePercent / 100 : 0));
        }
    }
}