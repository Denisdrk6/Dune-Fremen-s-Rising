using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using System.Collections;

namespace RagnarEngine
{
    public class Mathf
    {
        public static int dir = 0;
        public static float Lerp(float a, float b, float rate) => a + (b - a) * rate;
        public static Vector3 Lerp(Vector3 a, Vector3 b, float rate) => a + (b - a) * rate;

        public static float PingPong(float min, float max, float value)
        {
            float range = max - min;

            float multiple = value / range;

            bool ascending = multiple % 2 == 0;
            float modulus = value % range;

            return ascending ? modulus + min : max - modulus;
        }
        public static float PingPongFloat(float value, float speed, float max, float min, bool ascendingFirst)
        {
            if (dir == 0)
                dir = ascendingFirst ? 1 : -1;

            value += speed * dir;

            if (value > max)
            {
                value = max;
                dir = -1;
            }
            else if (value < min)
            {
                value = min;
                dir = 1;
            }
            return value;
        }
    }

}
