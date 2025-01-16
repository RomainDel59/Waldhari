using Waldhari.Common.Files;

namespace Waldhari.CokeWork
{
    public class CokeWorkSave : File<CokeWorkSave>
    {
        public static CokeWorkSave Instance;

        protected override void SetInstance(CokeWorkSave instance) => Instance = instance;
        
        #region save

        public int Owner;
        public bool Worker;
        
        public int Supply;
        public int Product;
        
        #endregion
        
        protected override void SetDefaults()
        {
            Owner = 0;
            Worker = false;
            Supply = 0;
            Product = 0;
        }

    }
}