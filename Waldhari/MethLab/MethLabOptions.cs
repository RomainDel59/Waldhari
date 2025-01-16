using Waldhari.Common.Files;

namespace Waldhari.MethLab
{
    public class MethLabOptions : File<MethLabOptions>
    {
        public static MethLabOptions Instance;

        protected override void SetInstance(MethLabOptions instance) => Instance = instance;
        
        #region options

        // Price to buy
        public int Price;
        
        // Minimum and maximum time to make
        public int ManufactureMinTimeInMinutes;
        public int ManufactureMaxTimeInMinutes;
        
        // Minimum and maximum supply used per manufacture action
        public int ManufactureMinSupplyUsageInKg;
        public int ManufactureMaxSupplyUsageInKg;
        
        // Minimum and maximum meth gram cooked by supply
        public int ManufactureMinMadeGramsPerSupplyKg;
        public int ManufactureMaxMadeGramsPerSupplyKg;
        
        // Minimum and maximum amount of supply to get when supplying
        public int SupplyMinInKg;
        public int SupplyMaxInKg;
        
        // Price for one supply
        public int SupplyMinCostPerKg;
        public int SupplyMaxCostPerKg;
        
        // Minimum and maximum grams to sell when dealing
        public int DealMinGramsPerSale;
        public int DealMaxGramsPerSale;
        
        // Minimum and maximum price by gram when dealing
        public int DealMinPriceByGram;
        public int DealMaxPriceByGram;
        
        // Minimum and maximum price by gram when selling bulk stock
        public int BulkMinPriceByGram;
        public int BulkMaxPriceByGram;
        
        
        
        #endregion
        
        protected override void SetDefaults()
        {
            Price = 1000000;

            SupplyMinInKg = 100;
            SupplyMaxInKg = 1000;
            
            SupplyMinCostPerKg = 1500;
            SupplyMaxCostPerKg = 2000;

            ManufactureMinTimeInMinutes = 1;
            ManufactureMaxTimeInMinutes = 2;
            
            ManufactureMinSupplyUsageInKg = 15;
            ManufactureMaxSupplyUsageInKg = 30;

            ManufactureMinMadeGramsPerSupplyKg = 400;
            ManufactureMaxMadeGramsPerSupplyKg = 600;

            DealMinGramsPerSale = 10;
            DealMaxGramsPerSale = 50;
            
            DealMinPriceByGram = 50;
            DealMaxPriceByGram = 100;
            
            BulkMinPriceByGram = 10;
            BulkMaxPriceByGram = 50;
            
        }
        
    }
}