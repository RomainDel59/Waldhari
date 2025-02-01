using Waldhari.Common.Files;

namespace Waldhari.WeedFarm
{
    public class WeedFarmSave : File<WeedFarmSave>
    {
        public static WeedFarmSave Instance;

        protected override void SetInstance(WeedFarmSave instance) => Instance = instance;
        
        #region save

        public int Owner;
        public bool Worker;
        
        public int Supply;
        public int Product;
        
        public int Guards;
        
        #endregion
        
        protected override void SetDefaults()
        {
            Owner = 0;
            Worker = false;
            Supply = 0;
            Product = 0;
            Guards = 0;
        }

    }
}