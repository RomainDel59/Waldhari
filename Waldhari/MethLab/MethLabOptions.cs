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
        
        // Minimum and maximum supply used per manufacture action
        public int ManufactureMin;
        public int ManufactureMax;
        
        // Minimum and maximum meth gram cooked by supply
        public int ManufactureMinGramsPerSupply;
        public int ManufactureMaxGramsPerSupply;
        
        // In minutes
        public int DefenseCooldown;
        
        // Minimum and maximum amount of supply to get when supplying
        public int SupplyMin;
        public int SupplyMax;
        
        // Price for one supply
        public int SupplyMinCost;
        public int SupplyMaxCost;
        
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
            
            DefenseCooldown = 15;

            SupplyMinCost = 20;
            SupplyMaxCost = 40;
            
            ManufactureMin = 50;
            ManufactureMax = 100;

            ManufactureMinGramsPerSupply = 1;
            ManufactureMaxGramsPerSupply = 5;

            DealMinGramsPerPack = 10;
            DealMaxGramsPerPack = 500;
            
            DealMinPrice = 50;
            DealMaxPrice = 100;
            
            BulkMinPrice = 25;
            BulkMaxPrice = 50;
            
        }
        
    }
}