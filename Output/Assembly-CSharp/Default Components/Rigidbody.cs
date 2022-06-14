using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RagnarEngine
{
    public enum CollisionType
    {
        BOX = 0,
        SPHERE = 1,
        CAPSULE = 2,
        CYLINDER = 3,
        CONE = 4,
        STATIC_PLANE = 5,
    }

    public class Rigidbody : RagnarComponent
    {
        
        public Rigidbody(UIntPtr ptr) : base(ptr)
        {
            type = ComponentType.RIGID_BODY;
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void ApplyCentralForce(Vector3 force);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void ApplyVelocity(Vector3 force);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void ApplyCentralImpulse(Vector3 impulse);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void ApplyTorque(Vector3 torque);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void ClearForces();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetAsTrigger();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetAsStatic();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetCollisionSphere(float radius, float x, float y, float z);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetHeight(float height);
        
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetBodyPosition(Vector3 position);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern Vector3 GetBodyPosition();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetBodyRotation(Quaternion rotation);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern Quaternion GetBodyRotation();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetRadiusSphere(float rad);

        public void IgnoreCollision(GameObject other, bool value)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
                SetIgnoreCollision(rb, value);
        }


        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern private void SetIgnoreCollision(Rigidbody other, bool value);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void EraseTrigger(Rigidbody owner, Rigidbody other);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void SetCollisionType(CollisionType type);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern void IsActive(Rigidbody owner, bool active);
        public extern Vector3 linearVelocity
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            set;
        }
        public extern Vector3 totalForce
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;
        }
    }
}
