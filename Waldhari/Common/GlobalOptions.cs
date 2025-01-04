using Waldhari.Common.Files;

namespace Waldhari.Common
{
    public class GlobalOptions : File<GlobalOptions>
    {
        public static GlobalOptions Instance;
        protected override void SetInstance(GlobalOptions instance) => Instance = instance;
        
        #region options

        public string PreferredLanguage;
        public int LoggerLevel;
        public bool ShowBlips;
        
        public int EnemiesAppearanceDistance;
        public int EnemiesDisappearanceDistance;
        
        
        
        #endregion
        
        protected override void SetDefaults()
        {
            PreferredLanguage = "fr-FR";
            //todo: set to 3
            LoggerLevel = 5;
            ShowBlips = true;
            
            EnemiesAppearanceDistance = 200;
            EnemiesDisappearanceDistance = 1000;
            
        }
    }
}