using System;
using UnityEngine;

namespace Game
{
    public class TileCoord
    {
        public float x;
        public float y;

        public TileCoord(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return "<" + x + "," + y + ">";
        }

        public TileCoord Rounded()
        {
            return new TileCoord(Mathf.Floor(x), Mathf.Floor(y));
        }
        
        public static TileCoord operator +(TileCoord a, TileCoord b) { return new TileCoord(a.x + b.x, a.y + b.y); }
        public static TileCoord operator -(TileCoord a, TileCoord b) { return new TileCoord(a.x - b.x, a.y - b.y); }
        public static TileCoord operator *(TileCoord a, float scale) { return new TileCoord(a.x * scale, a.y * scale); }
        public static TileCoord operator /(TileCoord a, float scale) { return new TileCoord(a.x / scale, a.y / scale); }

        public static bool operator ==(TileCoord a, TileCoord b)
        {
            bool aNull = object.ReferenceEquals(a, null);
            bool bNull = object.ReferenceEquals(b, null);
            if (aNull || bNull)
            {
                return aNull == bNull;
            }

            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(TileCoord a, TileCoord b)
        {
            return !(a == b);
        }
        
        public static float Magnitude(TileCoord a)
        {
            return (float) Mathf.Sqrt(a.x * a.x + a.y * a.y);
        }

        public static TileCoord Normalize(TileCoord a)
        {
            float magnitude = Magnitude(a);
            return new TileCoord(a.x / magnitude, a.y / magnitude);
        }
        
        public static float Distance(TileCoord a, TileCoord b)
        {
            //UnityEngine.Debug.Log(a.x + " " + a.y + " " + b.x + " " + b.y);
            return (float) Math.Sqrt(((b.x - a.x) * (b.x - a.x)) + ((b.y - a.y) * (b.y - a.y)));
        }
    }
}