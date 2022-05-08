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
    }

}