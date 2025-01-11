using Waldhari.Common.Behavior.Ped;
using Waldhari.Common.Files;

namespace Waldhari.Common
{
    public static class OptionsHelper
    {
        public static void GlobalLoad()
        {
            PersistenceHandler.ModName = "Waldhari";
            
            new GlobalOptions().Load();
            
            Logger.Level = GlobalOptions.Instance.LoggerLevel;
            Logger.Clear();
            
            Localization.CurCulture = GlobalOptions.Instance.PreferredLanguage;
            Localization.Initialize();
            
            EnemyGroupScript.AppearanceDistance = GlobalOptions.Instance.EnemiesAppearanceDistance;
            EnemyGroupScript.DisappearanceDistance = GlobalOptions.Instance.EnemiesDisappearanceDistance;
            
            
        }
    }
}