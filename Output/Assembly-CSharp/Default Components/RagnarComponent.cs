using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace RagnarEngine
{
    static class Constants
    {
        public const double PI = 3.14159;
        public const float DEGTORAD = 0.0174532925199432957f;
        public const float RADTODEG = 57.295779513082320876f;
    }
    public enum ComponentType
    {
        NONE = -1,
        TRANSFORM,
        MESH_RENDERER,
        MATERIAL,
        CAMERA,
        SCRIPT,
        AUDIO_SOURCE,
        AUDIO_LISTENER,
        AUDIO_REVERB_ZONE,
        LIGHT,
        ANIMATION,
        BONE,
        RIGID_BODY, 
        UI_BUTTON,
        UI_IMAGE,
        UI_CHECKBOX,
        UI_SLIDER,
        UI_TEXT,
        UI_INPUTBOX,
        UI_DROPDOWN,
        TRANFORM2D,
        NAVAGENT,
        PARTICLE_SYSTEM,
        BILLBOARD,
        INPUT_ACTION,
    }

    public class RagnarComponent
    {
        public UIntPtr pointer;
        public ComponentType type;

        public static Dictionary<System.Type, ComponentType> componentTable = new Dictionary<Type, ComponentType>
        {
            { typeof(Transform), ComponentType.TRANSFORM },
            { typeof(Material), ComponentType.MATERIAL },
            { typeof(AudioSource), ComponentType.AUDIO_SOURCE },
            { typeof(Rigidbody), ComponentType.RIGID_BODY},
            { typeof(Animation), ComponentType.ANIMATION},
            { typeof(Camera), ComponentType.CAMERA},
            { typeof(AudioListener), ComponentType.AUDIO_LISTENER},
            { typeof(NavAgent), ComponentType.NAVAGENT },
            { typeof(ParticleSystem), ComponentType.PARTICLE_SYSTEM },
            { typeof(Transform2D), ComponentType.TRANFORM2D},
            { typeof(UIDropDown), ComponentType.UI_DROPDOWN},
            { typeof(UIInputBox), ComponentType.UI_INPUTBOX},
            { typeof(UIButton), ComponentType.UI_BUTTON},
            { typeof(UIImage), ComponentType.UI_IMAGE},
            { typeof(UICheckbox), ComponentType.UI_CHECKBOX},
            { typeof(UISlider), ComponentType.UI_SLIDER},
            { typeof(UIText), ComponentType.UI_TEXT},
            { typeof(Light), ComponentType.LIGHT},
            { typeof(InputAction), ComponentType.INPUT_ACTION},
        };

        public RagnarComponent()
        {
            type = ComponentType.SCRIPT;
        }

        public RagnarComponent(UIntPtr ptr)
        {
            pointer = ptr;
            type = ComponentType.SCRIPT;
        }

        public extern GameObject gameObject
        {
            [MethodImplAttribute(MethodImplOptions.InternalCall)]
            get;
        }

        public ComponentType GetComponentType()
        {
            return type;
        }
    }
}
