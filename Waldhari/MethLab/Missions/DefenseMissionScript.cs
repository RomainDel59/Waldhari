﻿using System.Collections.Generic;
using GTA;
using Waldhari.Common;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Behavior.Ped;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Exceptions;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;

namespace Waldhari.MethLab.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class DefenseMissionScript : AbstractMissionScript
    {
        // first time try : wait 1 minute
        private static int _nextAttackTry = Game.GameTime + 60 * 1000;

        private static int _nextAutorizedAttack = -1;

        public DefenseMissionScript() : base("MethLabDefenseMission", true, "methlab_defense_success")
        {
        }

        private static void AddCooldown()
        {
            // DefenseCooldown => in game minutes
            _nextAttackTry = Game.GameTime + MethLabOptions.Instance.DefenseCooldown * 1000;
        }

        //todo: move this to methlab main script
        public static void TryToStart()
        {
            // If a mission is active (including defense)
            if (IsAnyMissionActive() || Game.IsMissionActive || Game.IsRandomEventActive)
            {
                AddCooldown();
                return;
            }

            // Is too far : 100 meters
            if (!WPositionHelper.IsNear(Game.Player.Character.Position, MethLabHelper.Positions.Property.Position, 100))
            {
                AddCooldown();
                return;
            }

            // Has not to try
            if (_nextAttackTry > Game.GameTime) return;
            
            // If no product : no attack
            if (MethLabSave.Instance.Product == 0) return;
            
            // Has been attacked in the last minutes : no new attack
            if(_nextAutorizedAttack > Game.GameTime) return;

            // Try to attack
            Logger.Info("Trying MethLabDefenseMission");
            if (RandomHelper.Try(GlobalOptions.Instance.RivalChance))
            {
                var script = InstantiateScript<DefenseMissionScript>();
                script.Start();
                
                // since an attack has been launched, the next will be at least 10 minutes later
                _nextAutorizedAttack = Game.GameTime + 10 * 60 * 1000;
            }

            AddCooldown();
        }

        protected override void StartComplement()
        {
            // Nothing
        }

        protected override bool OnTickComplement()
        {
            // Override dead condition in OnTick to show special message
            if (Game.Player.IsDead) throw new MissionException("methlab_defense_fail_dead");

            return true;
        }

        protected override List<string> EndComplement()
        {
            AddCooldown();
            
            return null;
        }

        protected override void FailComplement()
        {
            MethLabSave.Instance.Supply = 0;
            MethLabSave.Instance.Product = 0;
            MethLabSave.Instance.Save();
            
            AddCooldown();
        }

        protected override void SetupSteps()
        {
            AddStep(GetStepDefending(), false);
        }

        private Step GetStepDefending()
        {
            return new Step
            {
                Name = "Defending",
                MessageKey = "methlab_defense_defending",
                Action = () =>
                {
                    // No police during attack
                    Game.Player.WantedLevel = 0;
                },
                CompletionCondition = () =>
                    _rivalScript?.WGroup == null || _rivalScript.WGroup.AreDead()
            };
        }

        protected override void CreateScene()
        {
            _rivalScript = InstantiateScript<EnemyGroupScript>();
            _rivalScript.DefineGroup(WGroupHelper.CreateRivalMembers(RivalMembers * 2));
        }

        protected override void CleanScene()
        {
            // nothing
        }
    }
}