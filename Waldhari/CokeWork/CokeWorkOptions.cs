using Waldhari.Common.Files;

namespace Waldhari.CokeWork
{
    public class CokeWorkOptions : File<CokeWorkOptions>
    {
        public static CokeWorkOptions Instance;

        protected override void SetInstance(CokeWorkOptions instance) => Instance = instance;
        
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

            SupplyMinInKg = 100;
            SupplyMaxInKg = 500;
            
            SupplyMinCostPerKg = 800;
            SupplyMaxCostPerKg = 1200;

            ManufactureMinTimeInMinutes = 1;
            ManufactureMaxTimeInMinutes = 2;

            ManufactureMinSupplyUsageInKg = 5;
            ManufactureMaxSupplyUsageInKg = 10;

            ManufactureMinMadeGramsPerSupplyKg = 800;
            ManufactureMaxMadeGramsPerSupplyKg = 1200;

            DealMinGramsPerSale = 5;
            DealMaxGramsPerSale = 30;

            DealMinPriceByGram = 100;
            DealMaxPriceByGram = 200;

            BulkMinPriceByGram = 50;
            BulkMaxPriceByGram = 100;
            
        }
        
    }
}