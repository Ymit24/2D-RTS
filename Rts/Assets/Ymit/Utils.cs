using UnityEngine;
namespace Ymit
{
    public class Utils
    {
        public static bool AABB(float x1, float y1, float w1, float h1, float x2, float y2, float w2, float h2)
        {
            return x1 < x2 + w2 &&
                   x1 + w1 > x2 &&
                   y1 < y2 + h2 &&
                   y1 + h1 > y2;
        }
        public static Vector3 ZeroZ(Vector3 Vector)
        {
            return new Vector3(Vector.x, Vector.y, 0);
        }
        public static Vector2 Vector2D(Vector3 Vector)
        {
            return new Vector2(Vector.x, Vector.y);
        }
        public static Vector2 WorldMouse()
        {
            return ScreenToWorld(Input.mousePosition);
        }
        public static Vector2 ScreenToWorld(Vector2 WorldPoint)
        {
            return Vector2D(Camera.main.ScreenToWorldPoint(WorldPoint));
        }
    }
}
