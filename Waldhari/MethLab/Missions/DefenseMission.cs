using System;
using System.Collections.Generic;
using GTA;

namespace Waldhari.MethLab.Missions
{
    public class DefenseMission : AbstractMission
    {
        private DateTime _nextAttackTry;
        private GroupHelper.Group _attackingGroup;
        
        public DefenseMission() : base("DefenseMission", true, "defense_success")
        {
            AddCooldown();
        }

        private void AddCooldown()
        {
            _nextAttackTry = DateTime.Now.AddSeconds(ModOptions.Instance.DefenseCooldown);
        }
        
        public void TryToStart(bool hasAnotherMissionActive)
        {
            // If another mission is active
            if (hasAnotherMissionActive || Game.IsMissionActive || Game.IsRandomEventActive)
            {
                AddCooldown();
                return;
            }
            
            // Is already defending
            if (_attackingGroup != null && !_attackingGroup.IsDead())
            {
                AddCooldown();
                return;
            }
            
            // Is too far
            if (!(Game.Player.Character.Position.DistanceTo(ManufactureMission.AnimationPosition) < 50))
            {
                AddCooldown();
                return;
            }
            
            // Has not to try
            if (DateTime.Now < _nextAttackTry) return;
            
            // Try to attack
            var chance = ModOptions.Instance.ChanceRivalPercent;
            if (ModSave.Instance.Intel)
            {
                chance -= ModOptions.Instance.IntelPercent;
            }
            var trying = RandomHelper.Next(0, 100 + 1);
            
            Logger.Debug($"Trying DefenseMission chance={chance}, trying={trying}");
            if (chance > trying)
            {
                _attackingGroup = GroupHelper.CreateBikersRival(ModOptions.Instance.RivalNumber * 5, false);
                Start(string.Empty);
            }
            
            AddCooldown();
        }

        protected override bool StartComplement(string arg)
        {
            // Nothing
            return true;
        }

        protected override void UpdateComplement()
        {
            // Override dead condition in Update() to show special message
            if (Game.Player.IsDead) throw new MissionException("defense_fail_dead");
        }

        protected override List<string> EndComplement()
        {
            Logger.Debug($"DefenseMission end complement");
            _attackingGroup.Update();
            _attackingGroup = null;
            AddCooldown();
            return null;
        }

        protected override void FailComplement()
        {
            _attackingGroup.Update(true);
            _attackingGroup = null;
            ModSave.Instance.Supply = 0;
            ModSave.Instance.Product = 0;
            ModSave.Instance.Save();
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
                MessageKey = "defense_defending",
                Action = () =>
                {
                    _attackingGroup.Update();
                    
                    // No police during attack
                    Game.Player.WantedLevel = 0;
                },
                CompletionCondition = () =>
                    _attackingGroup.IsDead()
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