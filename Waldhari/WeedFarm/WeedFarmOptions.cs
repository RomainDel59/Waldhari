using Waldhari.Common.Files;

namespace Waldhari.WeedFarm
{
    public class WeedFarmOptions : File<WeedFarmOptions>
    {
        public static WeedFarmOptions Instance;

        protected override void SetInstance(WeedFarmOptions instance) => Instance = instance;
        
        #region options

        // Price to buy property
        public int Price;
        
        // Minimum and maximum amount of supply to get when supplying
        public int SupplyMinInKg;
        public int SupplyMaxInKg;
        
        // Price for one kg of supply
        public int SupplyMinCostPerKg;
        public int SupplyMaxCostPerKg;
        
        // Minimum and maximum time to make
        public int ManufactureMinTimeInMinutes;
        public int ManufactureMaxTimeInMinutes;
        
        // Minimum and maximum supply used per manufacture action
        public int ManufactureMinSupplyUsageInKg;
        public int ManufactureMaxSupplyUsageInKg;
        
        // Minimum and maximum meth gram cooked by supply kg
        public int ManufactureMinMadeGramsPerSupplyKg;
        public int ManufactureMaxMadeGramsPerSupplyKg;
        
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

            SupplyMinInKg = 1000;
            SupplyMaxInKg = 2000;
            
            SupplyMinCostPerKg = 5;
            SupplyMaxCostPerKg = 10;

            ManufactureMinTimeInMinutes = 1;
            ManufactureMaxTimeInMinutes = 2;
            
            ManufactureMinSupplyUsageInKg = 30;
            ManufactureMaxSupplyUsageInKg = 50;

            ManufactureMinMadeGramsPerSupplyKg = 500;
            ManufactureMaxMadeGramsPerSupplyKg = 1000;

            DealMinGramsPerSale = 20;
            DealMaxGramsPerSale = 100;
            
            DealMinPriceByGram = 20;
            DealMaxPriceByGram = 50;
            
            BulkMinPriceByGram = 5;
            BulkMaxPriceByGram = 20;
            
        }
        
    }
}