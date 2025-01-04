using Waldhari.Common.Files;

namespace Waldhari.MethLab
{
    public class MethLabOptions : File<MethLabOptions>
    {
        public static MethLabOptions Instance;
        protected override void SetInstance(MethLabOptions instance) => Instance = instance;
        
        #region options

        public int Price;
        
        
        
        #endregion
        
        protected override void SetDefaults()
        {
            Price = 1000000;
            
        }
        
    }
}