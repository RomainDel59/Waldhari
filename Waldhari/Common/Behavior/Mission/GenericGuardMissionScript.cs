using System.Collections.Generic;
using System.Linq;
using GTA;
using Waldhari.Common.Behavior.Ped;
using Waldhari.Common.Entities;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Files;
using Waldhari.WeedFarm;

namespace Waldhari.Common.Behavior.Mission
{
    [ScriptAttributes(NoDefaultInstance = true)]
    //todo: make it abstract
    public class GenericGuardMissionScript : AbstractMissionScript
    {
        // Parameters //todo: should be buyable by phone
        public List<WPositionHelper.GuardPositions> guardPositionsList = WeedFarmHelper.Positions.GuardPositionsList;
        public int numberOfGuards = 2;
        
        private List<PedActingScript> guardScriptList;
        private WGroup wGroup;
        
        //todo: name comes from class implemented this
        public GenericGuardMissionScript() : base("GenericGuardMissionScript", false, null)
        {
            CheckIfAnotherMissionIsActiveToLaunch = false;
        }

        protected override void StartComplement()
        {
            guardScriptList = new List<PedActingScript>();
            
            for (int i = 0; i < numberOfGuards; i++)
            {
                AddGuard(i);
            }
        }

        public void AddGuard(int index)
        {
            if (numberOfGuards > guardPositionsList.Count)
            {
                Logger.Warning($"Can't add guard to {Name}, the number of positions {guardPositionsList.Count} exceeded");
                return;
            }
            
            var wPed = new WPed
            {
                PedHash = PedHash.SecuroGuardMale01,
                InitialPosition = guardPositionsList[index].Position[0],
                Scenario = WPed.PedScenario.Guard
            };
            wPed.Create();
            wPed.Ped.RelationshipGroup = Game.Player.Character.RelationshipGroup;
            wPed.AddWeapon(WeaponsHelper.GetRandomGangVehicleWeapon());
            
            var script = InstantiateScript<PedActingScript>();
            script.PedIsRunning = false;
            script.WPed = wPed;
            
            guardScriptList.Add(script);
        }

        protected override bool OnTickComplement()
        {
            // If there is no guard, continue to wait
            if (numberOfGuards == 0) _waitEndTime = Game.GameTime + Minutes * 60 * 1000;
            return true;
        }

        protected override List<string> EndComplement()
        {
            // should not happend
            return null;
        }

        protected override void FailComplement()
        {
            // should not happend
        }

        protected override void SetupSteps()
        {
            AddStep(GetStepWait(), false);
            AddStep(GetChangingPosition(), false);
        }

        private const int Minutes = 1;//todo: change to 5 minutes
        private int _waitEndTime = Game.GameTime + Minutes * 60 * 1000;
        
        private Step GetStepWait()
        {
            return new Step
            {
                Name = "Wait",
                MessageKey = null,
                Action = () =>
                {
                    // While one of the guard is in combat, will wait again from begining
                    if (guardScriptList.All(guard => !guard.IsInCombat()))
                    {
                        _waitEndTime = Game.GameTime + Minutes * 60 * 1000;
                    }
                },
                CompletionCondition =
                    () => _waitEndTime < Game.GameTime,
                CompletionAction = () =>
                {
                    Logger.Debug($"Guards change position started");

                    for (int i = 0; i < numberOfGuards; i++)
                    {
                        guardPositionsList[i].ActualPosition++;
                        if (guardPositionsList[i].ActualPosition >= guardPositionsList[i].Position.Count)
                        {
                            guardPositionsList[i].ActualPosition = 0;
                        }
                        guardScriptList[i].WPed.InitialPosition = guardPositionsList[i].Position[guardPositionsList[i].ActualPosition];
                    }
                }
            };
        }
        
        private Step GetChangingPosition()
        {
            return new Step
            {
                Name = "ChangingPosition",
                MessageKey = null,
                Action = () => { },
                CompletionCondition =
                    () => guardScriptList.All(guard => !guard.HasMoved),
                CompletionAction = () =>
                {
                    Logger.Debug($"Guards will wait {Minutes} minutes before change position");
                    _waitEndTime = Game.GameTime + Minutes * 60 * 1000;
                    // return to step one
                    CurrentStep = GetFirstStep()-1;
                }
            };
        }
        
        
        
        
        protected override void CreateScene()
        {
            // nothing
        }

        protected override void CleanScene()
        {
            // nothing
        }
        
    }
}