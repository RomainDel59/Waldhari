using Waldhari.Common.Files;
using Waldhari.Common.Scripts;

namespace Waldhari.Common
{
    public static class OptionsHelper
    {
        public static void DefineGlobals()
        {
            PersistenceHandler.ModName = "Waldhari";
            
            new GlobalOptions().Load();
            
            Logger.Level = GlobalOptions.Instance.LoggerLevel;
            Logger.Clear();
            
            Localization.CurCulture = GlobalOptions.Instance.PreferredLanguage;
            Localization.Initialize();
            
            EnemyGroupBehaviorScript.AppearanceDistance = GlobalOptions.Instance.EnemiesAppearanceDistance;
            EnemyGroupBehaviorScript.DisappearanceDistance = GlobalOptions.Instance.EnemiesDisappearanceDistance;
            
            
        }
    }
}