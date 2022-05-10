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

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void ScriptRotation(float x, float y, float z);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void ScriptRotationAngle(float angle);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern float GetAngle();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void LockControlls(bool locked);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern float GetZoom();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void ScriptZoom(float zoom);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SwitchPresets(bool preset1);
    }
}
