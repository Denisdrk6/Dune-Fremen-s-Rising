﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RagnarEngine
{

    public sealed class GameObject
    {
        public string name;
        public UIntPtr pointer;
        public Transform transform;

        public GameObject()
        {
            name = "Empty";
            pointer = UIntPtr.Zero;
            //InternalCalls.CreateGameObject(name, Vector3.zero);
        }
        public GameObject(string _name, UIntPtr ptr, UIntPtr transPTR)
        {
            name = _name;
            pointer = ptr;

            transform = new Transform();
            transform.pointer = transPTR;
        }


        public T GetComponent<T>() where T : RagnarComponent
        {
            //ComponentType type = T.get;
            ComponentType retValue = ComponentType.SCRIPT;
            if (RagnarComponent.componentTable.ContainsKey(typeof(T)))
            {
                retValue = RagnarComponent.componentTable[typeof(T)];
            }
            return TryGetComponent<T>(typeof(T).ToString(), (int)retValue - 1); // TODO: Very temporary. FIND A BETTER WAY TO DO THIS
        }


        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern internal T TryGetComponent<T>(string type, int inputType = 0);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void AddComponent(int componentType);


        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void Instantiate(string name, Vector3 position, Quaternion rotation);

    }
}
