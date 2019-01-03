namespace Game.Util
{
    public class Utils
    {
        public static float lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        public static float clamp(float x, float min, float max)
        {
            if (x < min)
                return min;
            else if (x > max)
                return max;
            else
                return x;
        }
    }
}