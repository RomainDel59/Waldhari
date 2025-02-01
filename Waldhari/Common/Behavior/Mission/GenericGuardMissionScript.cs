﻿using System.Collections.Generic;
using System.Linq;
using GTA;
using LemonUI.Menus;
using Waldhari.Common.Behavior.Ped;
using Waldhari.Common.Entities;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Files;
using Waldhari.Common.UI;

namespace Waldhari.Common.Behavior.Mission
{
    [ScriptAttributes(NoDefaultInstance = true)]
    //todo: make it abstract
    public class GenericGuardMissionScript : AbstractMissionScript
    {
        // Parameters
        public List<WPositionHelper.GuardPositions> GuardPositionsList;
        public int NumberOfGuards;
        
        private List<PedActingScript> _guardScriptList;
        
        public GenericGuardMissionScript(string name) : base(name, false, null)
        {
            IsPlayerMission = false;
        }

        protected override void StartComplement()
        {
            _guardScriptList = new List<PedActingScript>();
            
            for (int i = 0; i < NumberOfGuards; i++)
            {
                AddGuard(i);
            }
        }

        private void AddGuard(int index)
        {
            var wPed = new WPed
            {
                PedHash = PedHash.SecuroGuardMale01,
                InitialPosition = GuardPositionsList[index].Position[0],
                Scenario = WPed.PedScenario.Guard
            };
            wPed.Create();
            wPed.Ped.RelationshipGroup = Game.Player.Character.RelationshipGroup;
            wPed.AddWeapon(WeaponsHelper.GetRandomGangVehicleWeapon());
            
            var script = InstantiateScript<PedActingScript>();
            script.PedIsRunning = false;
            script.WPed = wPed;
            
            _guardScriptList.Add(script);
        }

        protected override bool OnTickComplement()
        {
            // If there is no guard, continue to wait
            if (NumberOfGuards == 0) _waitEndTime = Game.GameTime + Minutes * 60 * 1000;
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
                    if (_guardScriptList.All(guard => !guard.IsInCombat()))
                    {
                        _waitEndTime = Game.GameTime + Minutes * 60 * 1000;
                    }
                },
                CompletionCondition =
                    () => _waitEndTime < Game.GameTime,
                CompletionAction = () =>
                {
                    Logger.Debug($"Guards change position started");

                    for (int i = 0; i < NumberOfGuards; i++)
                    {
                        GuardPositionsList[i].ActualPosition++;
                        if (GuardPositionsList[i].ActualPosition >= GuardPositionsList[i].Position.Count)
                        {
                            GuardPositionsList[i].ActualPosition = 0;
                        }
                        _guardScriptList[i].WPed.InitialPosition = GuardPositionsList[i].Position[GuardPositionsList[i].ActualPosition];
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
                    () => _guardScriptList.All(guard => !guard.HasMoved),
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


        public void AddMenuItem(NativeMenu menu)
        {
            MenuHelper.CreateActionItem(
                title: "rentguard_menu_title",
                description: "rentguard_menu_description",
                menu: menu,
                action: () =>
                {
                    if (NumberOfGuards+1 > GuardPositionsList.Count)
                    {
                        Logger.Info($"Can't add guard to {Name}, the number of positions {GuardPositionsList.Count} exceeded");
                        return;
                    }

                    if (GlobalOptions.Instance.GuardPrice > Game.Player.Money)
                    {
                        NotificationHelper.ShowFailure("not_enough_money");
                        return;
                    }

                    NumberOfGuards++;
                    AddGuard(NumberOfGuards-1);
                    NotificationHelper.ShowFromSecuroServ("rentguard_done");
                }
            );
        }
        
    }
}