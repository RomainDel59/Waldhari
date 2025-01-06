using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.UI;

namespace Waldhari.MethLab.Missions
{
    public class ManufactureMission : AbstractMission
    {
        private const int SecondsToReachDestination = 20;

        public static readonly Vector3 AnimationPosition = new Vector3(1389.3273f, 3604.7805f, 38.9419f);
        private static readonly Vector3 AnimationRotation = new Vector3(0f, 0f, -70.5439f);
        private static readonly Vector3 IdlePosition = new Vector3(1398.467f, 3616.657f, 39.00075f);
        private static readonly Vector3 IdleRotation = new Vector3(0f, 0f, 17.85464f);

        private int _amountToManufacture;
        private DateTime _waitUntil;
        private Camera _playerCamera;
        private WPed _chemist;

        public ManufactureMission() : base("ManufactureMission", false,
            null)
        {
        }

        protected override bool StartComplement(string arg)
        {
            _amountToManufacture = int.Parse(arg);

            if (ModSave.Instance.Supply < _amountToManufacture)
            {
                NotificationHelper.ShowFail("manufacture_not_enough_supply");
                return false;
            }

            // Without bucket : only small, medium and large are authorized
            if (!ModSave.Instance.Bucket && _amountToManufacture > 50)
            {
                NotificationHelper.ShowFail("manufacture_bucket_not_purchased");
                return false;
            }

            // Without chef, can cook near lab table only
            if (!ModSave.Instance.Chemist && Game.Player.Character.Position.DistanceTo(AnimationPosition) > 5)
            {
                NotificationHelper.ShowFail("manufacture_not_close_enough");
                return false;
            }

            return true;
        }

        protected override void UpdateComplement()
        {
            // Nothing
        }

        protected override List<string> EndComplement()
        {
            var product = CalculateProduct();

            ModSave.Instance.Supply -= _amountToManufacture;
            ModSave.Instance.Product += product;
            ModSave.Instance.Save();

            var values = new List<string> { product.ToString() };
            if (ModSave.Instance.Chemist)
            {
                NotificationHelper.ShowFromChemist("manufacture_finished_chemist", values);
            }
            else
            {
                NotificationHelper.ShowSuccess("manufacture_finished", values);
            }

            return null;
        }

        protected override void FailComplement()
        {
            // Nothing
        }

        private int CalculateProduct()
        {
            var multiplier = RandomHelper.Next(ModOptions.Instance.ManufactureMinGramsPerSupply,
                ModOptions.Instance.ManufactureMaxGramsPerSupply + 1);
            var substracter = RandomHelper.Next(ModOptions.Instance.ManufactureMinGramsPerSupply,
                ModOptions.Instance.ManufactureMaxGramsPerSupply + 1);

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
            AddStep(GetStepGoTo(), false);
            AddStep(GetStepCooking(), false);
            AddStep(GetStepGoBack(), false);
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
                    WPed wPed;
                    if (ModSave.Instance.Chemist)
                    {
                        NotificationHelper.ShowFromChemist("manufacture_started_chemist");
                        wPed = _chemist;
                        // it will wait for chef to go to position
                        _waitUntil = DateTime.Now.AddSeconds(SecondsToReachDestination);
                    }
                    else
                    {
                        wPed = new WPed(Game.Player.Character);
                        Screen.FadeOut(1000);
                        wPed.GtaPed.Weapons.Select(WeaponHash.Unarmed);
                        // it will wait 1 second for fade out
                        _waitUntil = DateTime.Now.AddSeconds(1);
                    }

                    wPed.Says("GENERIC_YES");
                    wPed.GtaPed.Task.GoTo(AnimationPosition);
                }
            };
        }

        private Step GetStepGoTo()
        {
            return new Step
            {
                Name = "Goto",
                MessageKey = null,
                // Nothing to do during this step
                Action = () => { },
                CompletionCondition = () =>
                    DateTime.Now > _waitUntil || ModSave.Instance.Chemist &&
                    _chemist.GtaPed.Position.DistanceTo(AnimationPosition) < 0.5f,
                CompletionAction = () =>
                {
                    var ped = ModSave.Instance.Chemist
                        ? _chemist
                        : new WPed(Game.Player.Character);

                    ped.GtaPed.PositionNoOffset = AnimationPosition;
                    ped.GtaPed.Rotation = AnimationRotation;
                    ped.GtaPed.IsPositionFrozen = true;
                    ped.PlayAnimation("anim@amb@business@meth@meth_monitoring_cooking@cooking@",
                        "chemical_pour_short_cooker");

                    if (ModSave.Instance.Chemist)
                    {
                        // For chef, it will take amount*1/2 seconds to cook, but minimum 10 seconds and maximum 5 minutes
                        _waitUntil = DateTime.Now.AddSeconds(Math.Min(Math.Max(10, _amountToManufacture / 2), 5 * 60));
                    }
                    else
                    {
                        var cameraPosition = new Vector3(1391.9039f, 3603.8345f, 39.8488f);
                        _playerCamera = World.CreateCamera(cameraPosition, Vector3.Zero, 50.0f);
                        _playerCamera.PointAt(AnimationPosition);
                        _playerCamera.IsActive = true;
                        World.RenderingCamera = _playerCamera;

                        // For player, it will always take 10 seconds to cook
                        _waitUntil = DateTime.Now.AddSeconds(10);
                        Screen.FadeIn(1000);
                    }

                    Logger.Debug("Should end at " + _waitUntil);
                }
            };
        }

        private Step GetStepCooking()
        {
            return new Step
            {
                Name = "Cooking",
                MessageKey = ModSave.Instance.Chemist ? null : "manufacture_cooking",
                // Nothing to do during this step
                Action = () => { },
                CompletionCondition = () => DateTime.Now > _waitUntil,
                CompletionAction = () =>
                {
                    var ped = ModSave.Instance.Chemist
                        ? _chemist
                        : new WPed(Game.Player.Character);

                    ped.Says("GENERIC_THANKS");
                    ped.GtaPed.IsPositionFrozen = false;

                    if (ModSave.Instance.Chemist)
                    {
                        ped.GtaPed.Task.ClearAllImmediately();
                        ped.GtaPed.Task.GoTo(IdlePosition);
                        // Wait chef to go to idle position
                        _waitUntil = DateTime.Now.AddSeconds(SecondsToReachDestination);
                    }
                    else
                    {
                        Screen.FadeOut(1000);
                        // Wait screen to fade out
                        _waitUntil = DateTime.Now.AddSeconds(1);
                    }
                }
            };
        }

        private Step GetStepGoBack()
        {
            return new Step
            {
                Name = "Idle",
                MessageKey = null,
                // Nothing to do during this step
                Action = () => { },
                CompletionCondition = () =>
                    DateTime.Now > _waitUntil ||
                    ModSave.Instance.Chemist && _chemist.GtaPed.Position.DistanceTo(IdlePosition) < 0.5f,
                CompletionAction = () =>
                {
                    var ped = ModSave.Instance.Chemist ? _chemist : new WPed(Game.Player.Character);
                    if (ModSave.Instance.Chemist)
                    {
                        ped.GtaPed.PositionNoOffset = IdlePosition;
                        ped.GtaPed.Rotation = IdleRotation;
                        ped.PlayScenario("WORLD_HUMAN_SMOKING");
                    }
                    else
                    {
                        ped.GtaPed.Task.ClearAllImmediately();
                        _playerCamera.IsActive = false;
                        World.RenderingCamera = null;
                        _playerCamera.Delete();
                        Screen.FadeIn(1000);
                    }
                }
            };
        }

        protected override void CreateScene()
        {
            if (!ModSave.Instance.Chemist) return;

            if (_chemist != null && _chemist.GtaPed != null && _chemist.GtaPed.IsDead)
            {
                _chemist.Remove();
            }

            //(move_m@generic)MP_M_Meth_01
            _chemist = WPed.GetPed((PedHash)3988008767, IdlePosition);
            _chemist.GtaPed.BlockPermanentEvents = true;
            _chemist.GtaPed.IsInvincible = true;
            _chemist.GtaPed.IsPersistent = true;
            _chemist.GtaPed.IsPositionFrozen = false;
            _chemist.GtaPed.PositionNoOffset = IdlePosition;
            _chemist.GtaPed.Rotation = IdleRotation;
            _chemist.PlayScenario("WORLD_HUMAN_SMOKING");
        }

        protected override void CleanScene()
        {
            if (!ModSave.Instance.Chemist) return;
            if (_chemist == null) return;
            if (_chemist.GtaPed == null) return;
            _chemist.GtaPed.BlockPermanentEvents = false;
            _chemist.GtaPed.IsInvincible = false;
            _chemist.GtaPed.IsPersistent = false;
            _chemist.GtaPed.MarkAsNoLongerNeeded();
        }
    }
}