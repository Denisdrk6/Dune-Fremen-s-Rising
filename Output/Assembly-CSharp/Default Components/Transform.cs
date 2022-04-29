using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RagnarEngine
{
    public class Transform : RagnarComponent
    {
        public Transform()
        {
            type = ComponentType.TRANSFORM;
        }

        public extern Vector3 localPosition
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;

            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            set;
        }

        public extern Vector3 globalPosition
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;
        }

        public extern Quaternion localRotation
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;

            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            set;
        }

        public extern Quaternion globalRotation
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;

            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            set;
        }
        
        public extern Vector3 scale
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            set;
        }

        public extern Vector3 forward
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;
        }

        public extern Vector3 right
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;
        }
        public extern Vector3 up
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern Vector3 RotateY(Vector3 vector, int anglesDegrees);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern float GetAngleBetween(Vector3 vec1, Vector3 vec2);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern float GetDistanceBetween(Vector3 vec1, Vector3 vec2);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern float ForceUpdateTransform();
    }
}