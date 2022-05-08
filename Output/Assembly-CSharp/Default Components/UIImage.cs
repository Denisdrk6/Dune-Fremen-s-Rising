using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RagnarEngine
{
    internal class UIImage : RagnarComponent
    {
        public UIImage(UIntPtr ptr) : base(ptr)
        {
            type = ComponentType.UI_IMAGE;
        }
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern int LoadTexture(string path);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void UseTexture(int ID);


        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetImageGeneralColor(float Red, float Green, float Blue);
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern Vector3 GetImageGeneralColor();


        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetImageAlpha(float NewAlpha);
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern float GetImageAlpha();


        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void StartAnimation(int animation);
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void StopAnimation(int animation);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void ChangeAnimationVelocity(int animation, float velocity);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetStaticImage(int animation, int image);
    }
}
