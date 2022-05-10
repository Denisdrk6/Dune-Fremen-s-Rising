using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using System.Collections;

namespace RagnarEngine
{
    public class Mathf
    {
        public static float Lerp(float a, float b, float rate) => a + (b - a) * rate;
        public static Vector3 Lerp(Vector3 a, Vector3 b, float rate) => a + (b - a) * rate;
    }

}
