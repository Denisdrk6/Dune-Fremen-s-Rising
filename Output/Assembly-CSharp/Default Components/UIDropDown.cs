using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RagnarEngine
{
    class UIDropDown : RagnarComponent
    {
        public UIDropDown(UIntPtr ptr) : base(ptr)
        {
            type = ComponentType.UI_DROPDOWN;
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern int GetDropState();
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetDropState(int newState);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern Vector3 GetTextPosition();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetTextPosition(float posx,float posy);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetDropAlpha(float NewAlpha);
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern float GetDropAlpha();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetDropTextColor(float Red, float Green,float Blue);
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern Vector3 GetDropTextColor();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetDropGeneralColor(float Red, float Green, float Blue);
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern Vector3 GetDropGeneralColor();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern String GetSelected();
        public extern string text
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;

            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            set;
        }
       
    }
}
