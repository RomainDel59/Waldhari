using Waldhari.Common.Files;

namespace Waldhari.MethLab
{
    public class MethLabSave : File<MethLabSave>
    {
        public static MethLabSave Instance;

        protected override void SetInstance(MethLabSave instance) => Instance = instance;
        
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