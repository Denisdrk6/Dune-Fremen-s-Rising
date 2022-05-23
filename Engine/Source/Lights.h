#pragma once
#include "Math/float4x4.h"
#include "Geometry/Frustum.h"

enum class LightType
{
    NONE = 0,
    DIRECTIONAL,
    POINT,
    SPOT
};

class Light
{
public:
    LightType type = LightType::NONE;

    float intensity = 10.0f;
    float3 ambient = { 0.25f, 0.25f, 0.25f };
    float3 diffuse = { 0.75f, 0.75f, 0.75f };
    float3 specular = { 0,0,0 };
};

class DirectionalLight : public Light
{
public:
    DirectionalLight()
    {
        dir = { 1,0,0 };
        ambient = { 0.25f, 0.25f, 0.25f };
        diffuse = { 0.75f, 0.75f, 0.75f };
        specular = { 0.0f,0.0f,0.0f };
        
        intensity = 5.0f;
        
        type = LightType::DIRECTIONAL;
        generateShadows = true;

        lightSpace = float4x4::identity;

        //frustum.SetKind(FrustumProjectiveSpace::FrustumSpaceGL, FrustumHandedness::FrustumRightHanded);

        Light();
    }

public:
    float3 dir;
    bool generateShadows;
    float4x4 lightSpace;
    //Frustum frustum;
};

class PointLight : public Light
{
public:
    PointLight()
    {
        position = { 0,0,0 };
        type = LightType::POINT;
        intensity = 10.0f;
        Light();
    }

public:
    float3 position = { 0,0,0 };

    float constant = 1.0f;
    float lin = 0.09f;
    float quadratic = 0.032f;
    bool toDelete = false;
};

class SpotLight : public Light
{
public:
    SpotLight()
    {
        position = { 0,0,0 };
        direction = { 0,0,-1 };
        cutOff = math::DegToRad(12.5f);
        outerCutOff = math::DegToRad(15.0f);
        intensity = 1.0f;

        type = LightType::SPOT;

        Light();
    }

public:
    float3  position;
    float3  direction;
    // Must be in radians since it's an angle!!
    float cutOff;
    float outerCutOff;
    float intensity;
    bool toDelete = false;
};