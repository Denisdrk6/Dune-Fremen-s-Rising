using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RagnarEngine
{ 
    public class Animation : RagnarComponent
    {
        public Animation(UIntPtr ptr) : base(ptr)
        {
            type = ComponentType.ANIMATION;
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void PlayAnimation(string v);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern bool HasFinished();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern float GetDuration();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern float GetLoopTime();

    }
}
