using Waldhari.Common.Files;

namespace Waldhari.MethLab
{
    public class MethLabOptions : File<MethLabOptions>
    {
        public static MethLabOptions Instance;

        protected override void SetInstance(MethLabOptions instance) => Instance = instance;
        
        #region options

        // Price to buy meth lab
        public int Price;
        
        // In game minutes
        public int DefenseCooldown;
        
        // Price for one supply
        public int SupplyCost;
        
        // Minimum and maximum grams to sell when dealing
        public int DealMinGramsPerPack;
        public int DealMaxGramsPerPack;
        
        // Minimum and maximum price by gram when dealing
        public int DealMinPrice;
        public int DealMaxPrice;
        
        // Minimum and maximum price by gram when selling bulk stock
        public int BulkMinPrice;
        public int BulkMaxPrice;
        
        
        
        #endregion
        
        protected override void SetDefaults()
        {
            Price = 1000000;
            
            DefenseCooldown = 60;

            SupplyCost = 30;

            DealMinGramsPerPack = 10;
            DealMaxGramsPerPack = 500;
            
            DealMinPrice = 50;
            DealMaxPrice = 100;
            
            BulkMinPrice = 25;
            BulkMaxPrice = 50;
            
        }
        
    }
}