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

        public static Bounds getChildBounds(this Transform t)
        {
            // First find a center for your bounds.
            Vector3 center = Vector3.zero;

            foreach (Transform child in t.transform)
            {
                center += child.gameObject.GetComponent<SpriteRenderer>().bounds.center;
            }
            center /= t.transform.childCount; //center is average center of children

            //Now you have a center, calculate the bounds by creating a zero sized 'Bounds', 
            Bounds bounds = new Bounds(center, Vector3.zero);

            foreach (Transform child in t.transform)
            {
                bounds.Encapsulate(child.gameObject.GetComponent<SpriteRenderer>().bounds);
            }

            return bounds;
        }
    }
}