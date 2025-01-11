using System.Collections.Generic;
using GTA;
using Waldhari.Behavior.Mission;
using Waldhari.Behavior.Ped;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Exceptions;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;

namespace Waldhari.MethLab.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class DefenseMissionScript : AbstractMissionScript
    {
        private int _nextAttackTry;
        private EnemyGroupScript _attackingScript;

        public DefenseMissionScript() : base("MethLabDefenseMission", true, "methlab_defense_success")
        {
            AddCooldown();
        }

        private void AddCooldown()
        {
            // DefenseCooldown => in game minutes
            _nextAttackTry = Game.GameTime + MethLabOptions.Instance.DefenseCooldown * 1000;
        }

        //todo: move this to methlab main script
        public void TryToStart()
        {
            // If another mission is active
            if (IsAnyMissionActive() || Game.IsMissionActive || Game.IsRandomEventActive)
            {
                AddCooldown();
                return;
            }

            // Is already defending
            if (_attackingScript != null && !_attackingScript.WGroup.AreDead())
            {
                AddCooldown();
                return;
            }

            // Is too far
            if (!WPositionHelper.IsNear(Game.Player.Character.Position, MethLabHelper.LaboratoryPosition.Position, 50))
            {
                AddCooldown();
                return;
            }

            // Has not to try
            if (_nextAttackTry > Game.GameTime) return;

            // Try to attack
            Logger.Info("Trying MethLabDefenseMission");
            if (RandomHelper.Try(RivalChance))
            {
                _attackingScript = InstantiateScript<EnemyGroupScript>();
                _attackingScript.DefineGroup(WGroupHelper.CreateRivalMembers(RivalMembers * 2, false));
                Start();
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
            _attackingScript.MarkAsNoLongerNeeded();
            _attackingScript.Abort();
            
            AddCooldown();
            
            return null;
        }

        protected override void FailComplement()
        {
            _attackingScript.MarkAsNoLongerNeeded();
            _attackingScript.Abort();
            
            MethLabSave.Instance.Supply = 0;
            MethLabSave.Instance.Product = 0;
            MethLabSave.Instance.Save();
            
            AddCooldown();
        }

        protected override void SetupSteps()
        {
            AddStep(GetStepDefending());
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
                    _attackingScript.WGroup.AreDead()
            };
        }

        protected override void CreateScene()
        {
            //No scene
        }

        protected override void CleanScene()
        {
            //No scene
        }
    }
}