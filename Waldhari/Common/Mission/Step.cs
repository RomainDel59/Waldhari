using System;
using Waldhari.Common.Exceptions;
using Waldhari.Common.Files;
using Waldhari.Common.UI;

namespace Waldhari.Common.Mission
{
    public class Step
    {
        /// <summary>
        /// Possible result during execution of step.
        /// </summary>
        public enum ExecutionResult
        {
            GoPrevious,
            GoNext,
            Continue
        }

        /// <summary>
        /// Name for log only.
        /// </summary>
        public string Name;

        /// <summary>
        /// Message displayed to guide player to complete the step. If null, no subtitle displayed.
        /// </summary>
        public string MessageKey;

        /// <summary>
        /// Action during the step, executed to each Tick.
        /// </summary>
        public Action Action;

        /// <summary>
        /// Condition to enter the step, if player doesn't fit condition anymore, preview step is relaunched.
        /// If it's undefined when added to step list, will take the value of previous step complete condition,
        /// otherwise it will keep the manual definition.
        /// </summary>
        public Func<bool> AccessCondition;

        /// <summary>
        /// Condition to enter the next step, if player fit condition, next step is launched.
        /// </summary>
        public Func<bool> CompletionCondition;

        /// <summary>
        /// Action to do after completion of step, executed only at completion of step.
        /// </summary>
        public Action CompletionAction;

        /// <summary>
        /// Index of the previous step if access condition doesn't fit anymore.
        /// Default = -1, so it can be defined with previous index when added to step list,
        /// otherwise it will keep the manual definition.
        /// </summary>
        public int PreviousIndex = -1;

        /// <summary>
        /// Show the message during one hour if exists, else hide previous subtitle.
        /// </summary>
        public void ShowMessage()
        {
            Logger.Debug($"ShowMessage for step {Name}.");

            if (MessageKey == null)
            {
                NotificationHelper.HideSubtitle();
                return;
            }

            NotificationHelper.Subtitle(MessageKey, 3600);
        }

        /// <summary>
        /// Execute step action if setup.
        /// </summary>
        public ExecutionResult Execute()
        {
            try
            {
                if (!AccessCondition())
                {
                    Logger.Info($"Access condition doesn't fit anymore for step {Name}.");
                    return ExecutionResult.GoPrevious;
                }

                if (CompletionCondition())
                {
                    Logger.Info($"Completion condition completed for step {Name}.");
                    CompletionAction?.Invoke();
                    return ExecutionResult.GoNext;
                }

                Action?.Invoke();

                return ExecutionResult.Continue;
            }
            catch (Exception e)
            {
                throw new TechnicalException($"Error during execution of step {Name}: {e.Message}");
            }
        }
    }
}