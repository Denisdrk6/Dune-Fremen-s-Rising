using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RagnarEngine
{
    public class InputAction : RagnarComponent
    {
        public InputAction(UIntPtr ptr) : base(ptr)
        {
            type = ComponentType.INPUT_ACTION;
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetActionMap(int index);

    }
}