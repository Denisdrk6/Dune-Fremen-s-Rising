﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RagnarEngine
{
    public class Camera : RagnarComponent
    {
        public Camera(UIntPtr ptr) : base(ptr)
        {
            pointer = ptr;
            type = ComponentType.CAMERA;
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void LookAt(Vector3 transform);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void ChangeFov(float fov);
    }
}
