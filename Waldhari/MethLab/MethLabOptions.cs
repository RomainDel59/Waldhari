using Waldhari.Common.Files;

namespace Waldhari.MethLab
{
    public class MethLabOptions : File<MethLabOptions>
    {
        public static MethLabOptions Instance;

        protected override void SetInstance(MethLabOptions instance) => Instance = instance;
        
        #region options

        public int Price;
        
        public int SupplyCost;
        
        public int DealMaxGramsPerPack;
        public int DealMinPrice;
        public int DealMaxPrice;
        
        public int BulkMinPrice;
        public int BulkMaxPrice;
        
        
        
        #endregion
        
        protected override void SetDefaults()
        {
            Price = 1000000;

            SupplyCost = 30;
            
            DealMaxGramsPerPack = 500;
            DealMinPrice = 50;
            DealMaxPrice = 100;
            
            BulkMinPrice = 25;
            BulkMaxPrice = 50;
        }
        
    }
}