using System.Collections.Generic;
using GTA;
using GTA.Math;
using Waldhari.Common.Entities;
using Waldhari.Common.Misc;
using Waldhari.Common.UI;

namespace Waldhari.MethLab
{
    public static class MethLabHelper
    {
        
        public static Vector3 PropertyPosition = new Vector3(1389.134f, 3605.236f, 38.94193f);

        public static WPosition LaboratoryPosition = new WPosition
        {
            Position = new Vector3(1389.134f, 3605.236f, 38.94193f),
            //todo: add rotation and heading
        };
        
        public static PedHash Chemist = (PedHash)3988008767;
        
        public static void ShowFromChemist(string messageKey, List<string> messageValues = null)
        {
            NotificationHelper.ShowFromDefault(messageKey, "chemist_sender", messageValues);
        }
        
        public static Vector3 LabParking = new Vector3(1381.139f, 3597.028f, 34.87445f);

        //todo: make MethLabPositions returns WPosition
        private static readonly List<Vector3> Positions = new List<Vector3>
        {
            new Vector3(-10.379f, 6479.785f, 31.450f),
            new Vector3(-263.226f, 6337.349f, 32.426f),
            new Vector3(-1094.664f, 4947.784f, 218.354f),
            new Vector3(1541.439f, 6335.468f, 24.075f),
            new Vector3(2195.705f, 5606.610f, 53.543f),
            new Vector3(1980.264f, 5171.993f, 47.639f),
            new Vector3(1904.219f, 4923.188f, 48.855f),
            new Vector3(1644.699f, 4840.306f, 42.029f),
            new Vector3(2489.476f, 4961.729f, 44.761f),
            new Vector3(2564.898f, 4640.567f, 34.077f),
            new Vector3(3803.763f, 4442.306f, 4.069f),
            new Vector3(2705.055f, 4134.968f, 43.916f),
            new Vector3(2466.705f, 4101.600f, 38.065f),
            new Vector3(1343.784f, 4311.199f, 37.986f),
            new Vector3(711.609f, 4180.271f, 40.709f),
            new Vector3(-2175.321f, 4273.497f, 49.029f),
            new Vector3(84.905f, 3732.156f, 39.572f),
            new Vector3(348.255f, 3392.751f, 36.403f),
            new Vector3(897.093f, 3577.524f, 33.389f),
            new Vector3(1396.033f, 3626.845f, 35.012f),
            new Vector3(1946.009f, 3823.136f, 31.964f),
            new Vector3(1961.695f, 3755.230f, 32.238f),
            new Vector3(1731.594f, 3313.742f, 41.224f),
            new Vector3(1756.024f, 3325.939f, 41.139f),
            new Vector3(2134.934f, 4781.117f, 40.970f),
            new Vector3(1951.139f, 4651.256f, 40.589f),
            new Vector3(1706.954f, 4825.075f, 42.020f),
            new Vector3(3334.109f, 5161.359f, 18.305f),
            new Vector3(2170.505f, 3359.104f, 45.481f),
            new Vector3(2397.484f, 3321.691f, 47.652f),
            new Vector3(2354.154f, 3134.378f, 48.209f),
            new Vector3(2340.720f, 3052.575f, 48.152f),
            new Vector3(2677.911f, 3513.246f, 52.712f),
            new Vector3(2660.747f, 3278.581f, 55.241f),
            new Vector3(708.716f, 607.638f, 128.911f),
            new Vector3(-2596.849f, 1929.607f, 167.308f),
            new Vector3(-3171.017f, 1098.924f, 20.784f),
            new Vector3(-1793.863f, 407.060f, 113.403f),
            new Vector3(-160.960f, 926.678f, 235.656f),
            new Vector3(-1565.433f, -578.797f, 25.708f),
            new Vector3(970.576f, -1538.099f, 30.693f),
            new Vector3(136.513f, -3197.850f, 5.858f),
            new Vector3(-1171.568f, -2028.023f, 13.207f),
            new Vector3(-519.258f, -1707.941f, 19.265f),
            new Vector3(294.698f, -1718.453f, 29.246f),
            new Vector3(-460.664f, -62.074f, 44.513f),
        };

        public static Vector3 GetRandomPosition()
        {
            //todo: make MethLabPositions returns WPosition
            return Positions[RandomHelper.Next(0, Positions.Count)];
        }
    }
}