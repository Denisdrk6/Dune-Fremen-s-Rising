using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RagnarEngine
{

    public class SceneManager
    {
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        static public extern void NextScene();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        static public extern void LoadScene(string name);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        static public extern void SaveCheckpoint(string name);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        static public extern void LoadCheckpoint(string name);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        static public extern void SaveTest(int deadCount, string playerName, Vector3 playerPos, float time);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        static public extern void Exit();

        static public extern String lastSceneName
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;
        }

        static public extern String currentSceneName
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;
        }
    }
}
