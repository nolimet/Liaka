using UnityEngine;
using System.Collections;

namespace util
{
    public static class MathHelper
    {

        public static Vector2 AngleToVector(float angle)
        {
            Vector2 output;
            float radians = angle * Mathf.Deg2Rad;

            output = new Vector2((float)Mathf.Cos(radians), (float)Mathf.Sin(radians));
            return output;
        }

        public static float VectorToAngle(Vector2 v2)
        {
            return Mathf.Atan2(v2.y,v2.x) * 180f / Mathf.PI;
        }

        public static int toInt(this bool b)
        {
            if (b)
                return 1;
            return 0;
        }

        public static float v(this Vector2 v2)
        {
            return Mathf.Sqrt(Mathf.Pow(v2.x,2) + Mathf.Pow(v2.y,2));
        }
    }
}