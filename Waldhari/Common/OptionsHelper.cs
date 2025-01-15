using Waldhari.Common.Behavior.Ped;
using Waldhari.Common.Files;

namespace Waldhari.Common
{
    public static class OptionsHelper
    {
        private static bool _loaded; 
        public static void GlobalLoad()
        {
            if(_loaded) return;
            _loaded = true;
            
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