using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RagnarEngine
{
    public class Material : RagnarComponent
    {
        public Material(UIntPtr ptr) : base(ptr)
        {
            type = ComponentType.MATERIAL;
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetTexturePath(string path);

        public extern float diffuseAlpha
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;

            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            set;
        }

        public extern bool emissiveEnabled
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;

            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            set;
        }

        public extern Vector3 emissiveColor
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;

            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            set;
        }
    }
}