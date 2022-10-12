using UnityEngine;

namespace Excalibur
{
    public static partial class Utility
    {
        public static float Clamp(float value, float min, float max)
        {
            return Mathf.Clamp(value, min, max);
        }

        public static float Clamp01(float value)
        {
            return Mathf.Clamp01(value);
        }

        public static int RandomInt(int minimum, int maximum)
        {
            return Random.Range(minimum, maximum);
        }

        public static int RandomIntInclude(int minimum, int maximum)
        {
            return Random.Range(minimum, maximum + 1);
        }
        public static float RandomSingle(float minimum, float maximum)
        {
            return Random.Range(minimum, maximum);
        }

        public static float RandomSingleInclude(float minimum, float maximum)
        {
            return Random.Range(minimum, maximum + 1);
        }
    }
}
