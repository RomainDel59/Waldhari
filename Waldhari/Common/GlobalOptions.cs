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
        
        public int WantedChance;
        public int RivalChance;
        public int RivalMembers;
        
        public int EnemiesAppearanceDistance;
        public int EnemiesDisappearanceDistance;
        
        
        
        #endregion
        
        protected override void SetDefaults()
        {
            PreferredLanguage = "fr-FR";
            //todo: set to 3
            LoggerLevel = 5;
            ShowBlips = true;

            WantedChance = 25;
            RivalChance = 25;
            RivalMembers = 3;
            
            EnemiesAppearanceDistance = 200;
            EnemiesDisappearanceDistance = 1000;
            
        }
    }
}