using Waldhari.Common.Files;

namespace Waldhari.MethLab
{
    public class MethLabOptions : File<MethLabOptions>
    {
        public static MethLabOptions Instance;
        public int ManufactureMinGramsPerSupply { get; set; }
        public int ManufactureMaxGramsPerSupply { get; set; }

        protected override void SetInstance(MethLabOptions instance) => Instance = instance;
        
        #region options

        // Price to buy meth lab
        public int Price;
        
        // Minimum and maximum supply use per manufacture action
        public int ManufactureMin;
        public int ManufactureMax;
        
        // In game minutes
        public int DefenseCooldown;
        
        // Price for one supply
        //todo: change supply method
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
            
            
            ManufactureMin = 50;
            ManufactureMax = 100;

            DealMinGramsPerPack = 10;
            DealMaxGramsPerPack = 500;
            
            DealMinPrice = 50;
            DealMaxPrice = 100;
            
            BulkMinPrice = 25;
            BulkMaxPrice = 50;
            
        }
        
    }
}