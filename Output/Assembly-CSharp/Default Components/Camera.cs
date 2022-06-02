using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RagnarEngine
{
    public class Camera : RagnarComponent
    {
        public Camera(UIntPtr ptr) : base(ptr)
        {
            type = ComponentType.CAMERA;
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void LookAt(Vector3 transform);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void ChangeFov(float fov);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void ScriptMovement(float x, float y, float z);

        public extern bool lockCam
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;

            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            set;
        }

        public extern float horizontalAngle
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;

            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            set;
        }
    }
}
