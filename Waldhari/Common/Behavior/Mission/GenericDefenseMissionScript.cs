using System.Collections.Generic;
using GTA;
using Waldhari.Common.Behavior.Ped;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Exceptions;

namespace Waldhari.Common.Behavior.Mission
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public abstract class GenericDefenseMissionScript : AbstractMissionScript
    {
        protected abstract void ResetAll();
        protected abstract void AddCooldown();
        protected abstract string StepDefendingMessageKey { get; }
        
        protected GenericDefenseMissionScript(string name, string successKey) : 
            base(name, true, successKey) {}

        protected override void StartComplement()
        {
            // Nothing
            //todo: show started message ?
        }
        
        protected override bool OnTickComplement()
        {
            // Override dead condition in OnTick to show special message
            if (Game.Player.IsDead) throw new MissionException("defense_fail_dead");

            return true;
        }
        
        protected override List<string> EndComplement()
        {
            AddCooldown();
            
            return null;
        }
        
        protected override void FailComplement()
        {
            ResetAll();
            
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
                MessageKey = StepDefendingMessageKey,
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