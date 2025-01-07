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
        
        // In game minutes
        public int RandomEventCooldown;
        
        public int WantedChance;
        public int WantedStars;
        
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

            RandomEventCooldown = 60;
            
            WantedChance = 10;
            WantedStars = 2;
            
            RivalChance = 20;
            RivalMembers = 3;
            
            EnemiesAppearanceDistance = 200;
            EnemiesDisappearanceDistance = 1000;
            
        }
    }
}