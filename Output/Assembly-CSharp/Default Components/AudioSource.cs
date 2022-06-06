using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RagnarEngine
{
    public class AudioSource : RagnarComponent
    {
        public AudioSource(UIntPtr ptr) : base(ptr)
        {
            type = ComponentType.AUDIO_SOURCE;
        }
        
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void PlayClip(string clipName);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void PlayClip2(string clipName);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void StopCurrentClip(string clipName);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetClipVolume(float vol);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern float GetClipVolume();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetState(string group, string state);

    }
}
