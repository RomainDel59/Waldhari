using System.Globalization;
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
        
        // In minutes
        public int RandomEventCooldown;
        
        public int WantedChance;
        public int WantedStars;
        
        public int RivalChance;
        public int RivalMembers;
        
        // In minutes
        public int DefenseCooldown;
        
        public int EnemiesAppearanceDistance;
        public int EnemiesDisappearanceDistance;
        
        
        
        #endregion
        
        protected override void SetDefaults()
        {
            PreferredLanguage = CultureInfo.CurrentCulture.Name;
            
            LoggerLevel = 3;
            ShowBlips = true;

            RandomEventCooldown = 2;
            
            WantedChance = 10;
            WantedStars = 2;
            
            RivalChance = 20;
            RivalMembers = 3;
            
            DefenseCooldown = 5;
            
            EnemiesAppearanceDistance = 50;
            EnemiesDisappearanceDistance = 1000;
            
        }
    }
}