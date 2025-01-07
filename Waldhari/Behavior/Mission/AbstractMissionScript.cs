using System;
using System.Collections.Generic;
using GTA;
using LemonUI;
using Waldhari.Behavior.Animation;
using Waldhari.Behavior.Ped;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Exceptions;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;
using Waldhari.Common.Mission;
using Waldhari.Common.UI;

namespace Waldhari.Behavior.Mission
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public abstract class AbstractMissionScript : Script
    {
        private int _nextExecution = Game.GameTime;
        
        // Parameters MenuPool, WantedChance, RivalChance and RivalMembers are mandatory
        
        /// <summary>
        /// Pool for managing menus.
        /// </summary>
        public ObjectPool MenuPool;
        
        /// <summary>
        /// Chance of triggering a "wanted" random event.
        /// </summary>
        public int WantedChance = -1;
        
        /// <summary>
        /// Chance of triggering a "rival gang" random event.
        /// </summary>
        public int RivalChance = -1;
        
        /// <summary>
        /// Number of rival gang members to spawn.
        /// </summary>
        public int RivalMembers = -1;
        
        /// <summary>
        /// Name of mission (for logs only).
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// Determines if the mission ends with an animation or a simple notification.
        /// </summary>
        private readonly bool _finishWithAnimation;

        /// <summary>
        /// Key for the success message to display at the end of the mission.
        /// </summary>
        private readonly string _successMessageKey;

        /// <summary>
        /// List of steps defining the mission flow.
        /// </summary>
        private readonly List<Step> _steps = new List<Step>();

        /// <summary>
        /// Index of the current active step.
        /// </summary>
        private int _currentStep;
        
        /// <summary>
        /// Indicates whether the mission is currently active.
        /// </summary>
        public bool IsActive;
        
        /// <summary>
        /// Index of the "wanted" step, if applicable. -1 means it is not defined.
        /// </summary>
        private int _wantedStepIndex = -1;
        private bool HasWantedStep() => _wantedStepIndex > -1;
        
        /// <summary>
        /// Index of the "rival" step, if applicable. -1 means it is not defined.
        /// </summary>
        private int _rivalStepIndex = -1;
        private bool HasRivalStep() => _rivalStepIndex > -1;
        
        /// <summary>
        /// Permit to manage rival behavior.
        /// </summary>
        private EnemyGroupScript _rivalScript;
        
        /// <summary>
        /// Ensures random events (e.g., rival gang or police chase) are triggered only once during a mission.
        /// </summary>
        private bool _randomEventAlreadyLaunchedOnce;

        /// <summary>
        /// Determines the time when next random event trigger is tried.
        /// </summary>
        private int _nextRandomEventTry = Game.GameTime + 60 * 1000;
        
        /// <summary>
        /// Determines the index of the first "real" step, excluding optional random events like wanted or rival steps.
        /// </summary>
        private int GetFirstStep()
        {
            if (HasWantedStep() && HasRivalStep())
            {
                return 2; // Skip both "wanted" and "rival" steps
            }
            if (HasWantedStep() || HasRivalStep())
            {
                return 1; // Skip one of the random event steps
            } 
            return 0; // Default to the first step
        }

        /// <summary>
        /// Constructor for the abstract mission.
        /// </summary>
        /// <param name="name">Name of the mission (used for logs).</param>
        /// <param name="finishWithAnimation">True if the mission ends with an animation.</param>
        /// <param name="successMessageKey">Key for the success message.</param>
        protected AbstractMissionScript(string name, bool finishWithAnimation, string successMessageKey)
        {
            Logger.Debug($"Instantiate mission {name}");
            
            _name = name;
            _finishWithAnimation = finishWithAnimation;
            _successMessageKey = successMessageKey;
            
            Tick += OnTick;
        }
        
        /// <summary>
        /// Uses to start a mission, made to be attached to a menu
        /// so it is launched when player select it.
        /// Do not override this method and use StartComplement to add
        /// start conditions and/or processes before mission is launched.
        /// At the end of Start method, the first step (after Wanted/Rival step, if applicable)
        /// is automatically launched.
        /// </summary>
        /// <param name="arg">Arg from menu</param>
        public void Start(string arg)
        {
            Logger.Debug($"Starting mission {_name}");

            try
            {
                if (IsActive || Game.IsMissionActive || Game.IsRandomEventActive)
                {
                    NotificationHelper.ShowFailure("already_in_mission");
                    Abort();
                }

                if (!StartComplement(arg)) Abort();;

                CreateScene();

                IsActive = true;
                HideMenu();

                SetupSteps();

                SetStep(GetFirstStep());
            }
            catch (TechnicalException e)
            {
                Logger.Error($"TechnicalException for {_name} : {e.Message}");
                NotificationHelper.ShowFailure(e.Message);
                Abort();
            }
        }
        
        /// <summary>
        /// Uses as a complement of starting method.
        /// This is where to add start conditions
        /// and/or processes before mission is launched.
        /// </summary>
        /// <param name="arg">Arg from menu</param>
        /// <returns>If mission can start.</returns>
        protected abstract bool StartComplement(string arg);

        /// <summary>
        /// If mission isn't launched : do nothing.
        /// Do not override this method and
        /// use OnTickComplement to add fail conditions.
        /// Fail conditions must throw MissionException.
        /// </summary>
        private void OnTick(object sender, EventArgs e)
        {
            // Wait for mission to be activated
            if (!IsActive) return;
            
            // To lower material usage :
            // runs this script every 1/2 second only
            if (_nextExecution > Game.GameTime) return;
            _nextExecution = Game.GameTime + 500;
            
            try
            {
                TickComplement();

                if (Game.Player.IsDead) throw new MissionException("player_dead");

                // No police when fighting rival gang
                if (IsFightingRival()) Game.Player.WantedLevel = 0;

                // Launch rival step if defined (launched one time)
                if (HasRivalStep() && IsFightingRival() && _currentStep != _rivalStepIndex)
                {
                    Logger.Info($"CurrentStep = {_currentStep}, but is fighting rival so : rival step to be launched");

                    _steps[_rivalStepIndex].PreviousIndex = _currentStep;
                    SetStep(_rivalStepIndex);

                    return;
                }

                // Launch wanted step if defined (launched one time)
                if (HasWantedStep() && Game.Player.WantedLevel > 0 && _currentStep != _wantedStepIndex)
                {
                    Logger.Debug($"CurrentStep = {_currentStep}, but wanted level = {Game.Player.WantedLevel} so : wanted step to be launched");

                    _steps[_wantedStepIndex].PreviousIndex = _currentStep;
                    SetStep(_wantedStepIndex);

                    return;
                }

                switch (_steps[_currentStep].Execute())
                {
                    case Step.ExecutionResult.GoPrevious:
                        SetStep(_steps[_currentStep].PreviousIndex);
                        break;
                    case Step.ExecutionResult.GoNext:
                        var nextIndex = _currentStep + 1;
                        if (nextIndex >= _steps.Count)
                        {
                            End();
                            return;
                        }

                        SetStep(nextIndex);
                        break;
                    case Step.ExecutionResult.Continue:
                        if (!_randomEventAlreadyLaunchedOnce && _nextRandomEventTry < Game.GameTime)
                        {
                            _nextRandomEventTry = Game.GameTime + 60*1000;
                            TryLaunchRandomEvent();
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (MissionException exception)
            {
                ManageMissionException(exception);
            }
            catch (TechnicalException exception)
            {
                ManageTechnicalException(exception);
            }
            
        }
        
        /// <summary>
        /// Uses as a complement of OnTick method.
        /// This is where to add fail conditions
        /// during tick script of the mission.
        /// </summary>
        protected abstract void TickComplement();

        /// <summary>
        /// Ends the mission successfully and cleans up properties.
        /// </summary>
        private void End()
        {
            Logger.Debug($"Mission {_name} success");

            ClearProperties();

            var values = EndComplement();

            if (_successMessageKey == null) return;

            if (_finishWithAnimation)
            {
                AnimationHelper.MissionSucceed(_successMessageKey, values);
            }
            else
            {
                NotificationHelper.ShowSuccess(_successMessageKey, values);
            }
            
            Abort();
        }

        /// <summary>
        /// Handles post-mission success logic.
        /// </summary>
        /// <returns>List of values for the success message.</returns>
        protected abstract List<string> EndComplement();


        /// <summary>
        /// Ends the mission as a failure and cleans up properties.
        /// </summary>
        private void Fail(string messageKey)
        {
            Logger.Debug($"Mission {_name} fail");
            
            ClearProperties();

            FailComplement();

            if (_finishWithAnimation)
            {
                AnimationHelper.MissionFailed(messageKey);
            }
            else
            {
                NotificationHelper.ShowFailure(messageKey);
            }
            
            Abort();
        }

        /// <summary>
        /// Handles post-mission failure logic.
        /// </summary>
        protected abstract void FailComplement();

        /// <summary>
        /// Clears mission-specific properties.
        /// </summary>
        private void ClearProperties()
        {
            NotificationHelper.HideSubtitle();
            CleanScene();
            _steps.Clear();
            IsActive = false;
            _randomEventAlreadyLaunchedOnce = false;
            _rivalScript.MarkAsNoLongerNeeded();
            _rivalScript.Abort();
            _wantedStepIndex = -1;
        }

        /// <summary>
        /// Defines mission steps.
        /// </summary>
        protected abstract void SetupSteps();

        /// <summary>
        /// Creates the mission scene.
        /// </summary>
        protected abstract void CreateScene();

        /// <summary>
        /// Cleans up the mission scene.
        /// </summary>
        protected abstract void CleanScene();

        /// <summary>
        /// Adds the "Wanted" step to the mission steps list and updates the index.
        /// </summary>
        protected void AddWantedStep()
        {
            AddStep(GetWantedStep());
            _wantedStepIndex = _steps.Count - 1;
        }

        /// <summary>
        /// Permits to use an "escape the police" step.
        /// Developer have to define a previous step index when wanted step happens,
        /// so if player escapes the police, it returns to this previous step.
        /// This step have to be the first one, and the next one setup
        /// as it can't go back (previous = itself).
        /// </summary>
        /// <returns>Wanted step</returns>
        private static Step GetWantedStep()
        {
            return new Step
            {
                Name = "Wanted",
                MessageKey = "wanted_during_mission",
                // Nothing to do during pursuit
                Action = () => { },
                // Previous step is always itself by default until it's define manually
                PreviousIndex = 0,
                // Can return to previous step if escapes police (see signature comment)
                AccessCondition = () => Game.Player.WantedLevel > 0,
                // Can't go to a next step
                CompletionCondition = () => false
            };
        }

        /// <summary>
        /// Adds the "Rival" step to the mission steps list and updates the index.
        /// </summary>
        protected void AddRivalStep()
        {
            AddStep(GetRivalStep());
            _rivalStepIndex = _steps.Count - 1;
        }

        /// <summary>
        /// Permits to use an "escape the rival gang" step.
        /// Developer have to define a previous step index when rival step happens,
        /// so if player escapes or kills rival members, it returns to this previous step.
        /// This step have to be the first or second (if wanted is set up) one, and the next one setup
        /// as it can't go back (previous = itself).
        /// </summary>
        /// <returns>Rival step</returns>
        private Step GetRivalStep()
        {
            return new Step
            {
                Name = "Rival",
                MessageKey = "rival_during_mission",
                // Nothing, this is a script
                Action = () => {},
                // Previous step is always itself by default until it's define manually
                PreviousIndex = 0,
                // Can return to previous step if kill all rival gang members
                AccessCondition = () => !IsFightingRival(),
                // Can't go to a next step
                CompletionCondition = () => false
            };
        }

        /// <summary>
        /// Adds a step to the list. If it can't move to previous step,
        /// it will define previous step as itself and access condition to true,
        /// so it will never go back to previous step.
        /// </summary>
        /// <param name="step">Step to add</param>
        /// <param name="canMoveToPrevious">If it can go back to previous step</param>
        protected void AddStep(Step step, bool canMoveToPrevious = true)
        {
            // If it can't go back from this step
            if (!canMoveToPrevious)
            {
                // The count will be itself once added
                step.PreviousIndex = _steps.Count;
            }
            // Else, the previous index is always the previous step
            // if it's not manually defined.
            // Except the first one which is the first step
            else if (step.PreviousIndex == -1)
            {
                step.PreviousIndex = _steps.Count == 0 ? 0 : _steps.Count - 1;
            }

            // If it can't go back from this step
            if (!canMoveToPrevious)
            {
                step.AccessCondition = () => true;
            }
            // The access condition is always the completion condition of the previous step
            // if it's not manually defined,
            // except the first one which is always true
            else if (step.AccessCondition == null)
            {
                step.AccessCondition = _steps.Count == 0 ? () => true : _steps[_steps.Count - 1].CompletionCondition;
            }

            _steps.Add(step);
        }

        /// <summary>
        /// Sets a step.
        /// </summary>
        /// <param name="step"></param>
        /// <exception cref="TechnicalException"></exception>
        private void SetStep(int step)
        {
            if (step > _steps.Count)
            {
                throw new TechnicalException($"Invalid step for mission {_name} : set step {step} > max step {_steps.Count}.");
            }

            Logger.Info($"{_name} step is set to {step} '{_steps[step].Name}'.");
            _currentStep = step;
            _steps[_currentStep].ShowMessage();
        }

        /// <summary>
        /// Try to launch a random step if possible.
        /// </summary>
        /// <exception cref="TechnicalException">If there is a param missing</exception>
        private void TryLaunchRandomEvent()
        {
            //If random step possible only
            if (!IsRandomStepPossible()) return;

            // If random event not already activated only
            if (_randomEventAlreadyLaunchedOnce) return;

            // If it isn't the first step currently running only
            if (_currentStep == GetFirstStep()) return;

            // If not already wanted only
            if (IsWanted()) return;

            // If not already fighting only
            if (IsFightingRival()) return;

            // Random event : Police try to catch player
            if (HasWantedStep())
            {
                if(WantedChance == -1) throw new TechnicalException("Wanted chance is not defined.");
                Logger.Info($"Trying WantedStep chance={WantedChance}");
                if (RandomHelper.Try(WantedChance))
                {
                    Game.Player.WantedLevel = 2;
                    // no other random event will be processed during this mission
                    _randomEventAlreadyLaunchedOnce = true;
                    return; //to avoid Rival step to trigger also
                }
            }

            // Random event : Rival gang try to catch player
            if (HasRivalStep())
            {
                if(RivalChance == -1) throw new TechnicalException("Rival chance is not defined.");
                if(RivalMembers == -1) throw new TechnicalException("Rival members is not defined.");
                Logger.Info($"Trying WantedStep chance={RivalChance}");
                if (RandomHelper.Try(RivalChance))
                {
                    _rivalScript = InstantiateScript<EnemyGroupScript>();
                    _rivalScript.DefineGroup(WGroupHelper.CreateRivalMembers(RivalMembers));
                    // no other random event will be processed during this mission
                    _randomEventAlreadyLaunchedOnce = true;
                }
            }
        }

        /// <summary>
        /// Checks if the player has a wanted level.
        /// </summary>
        /// <returns>The player has a wanted level</returns>
        private static bool IsWanted()
        {
            return Game.Player.WantedLevel > 0;
        }

        /// <summary>
        /// Checks if the rival group exists and has at least one member still alive.
        /// </summary>
        /// <returns>The rival group exists and has at least one member still alive</returns>
        private bool IsFightingRival()
        {
            return _rivalScript != null && !_rivalScript.WGroup.AreDead();
        }

        /// <summary>
        /// Checks if at least one random step is defined.
        /// </summary>
        /// <returns>At least one random step is defined</returns>
        private bool IsRandomStepPossible()
        {
            return HasWantedStep() || HasRivalStep();
        }

        /// <summary>
        /// Hides the menu if it exists, otherwise throws an exception.
        /// </summary>
        /// <exception cref="TechnicalException">The menu is not defined</exception>
        private void HideMenu()
        {
            if (MenuPool == null)
            {
                throw new TechnicalException("No menu pool has been set.");
            }
            MenuPool.HideAll();
        }

        /// <summary>
        /// Ends the mission as a failure.
        /// This is when player fails, but it is a managed mechanist of gameplay:
        /// for example, if player dies.
        /// </summary>
        /// <param name="e"></param>
        private void ManageMissionException(MissionException e)
        {
            Logger.Debug($"MissionException for {_name} : {e.Message}");
            Fail(e.Message);
        }

        /// <summary>
        /// Ends the mission as a failure.
        /// This is when a technical problem appears.
        /// Should not be trigger! Developer will have to fix the issue if it happens. 
        /// </summary>
        /// <param name="e"></param>
        private void ManageTechnicalException(TechnicalException e)
        {
            Logger.Error($"TechnicalException for {_name} : {e.Message}");
            Fail(e.Message);
        }
        
        
        
        
    }
}