using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RagnarEngine
{
    public class NavAgent : RagnarComponent
    {
        NavAgent(UIntPtr ptr) : base(ptr)
        {
            type = ComponentType.NAVAGENT;
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern Vector3[] CalculatePath(Vector3 destination);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern bool ValidDestination(Vector3 destination);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void ClearPath();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern int PathSize();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern bool MovePath();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern bool MoveTo(Vector3 destination);

        public extern Vector3 hitPosition
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;

            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            set;
        }

        public extern Vector3 rayCastA
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;
        }        

        public extern Vector3 rayCastB
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;
        }

        public extern Vector3 path
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            set;
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;
        }

        public extern float speed
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            set;

            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;
        }
    }
}
