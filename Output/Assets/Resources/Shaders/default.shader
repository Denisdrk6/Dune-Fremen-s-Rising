#type vertex
#version 430 core

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 texCoords;
layout(location = 3) in vec4 boneIds;
layout(location = 4) in vec4 weights;
layout(location = 5) in vec3 tangents;
layout(location = 6) in vec3 biTangents;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat3 normalMatrix;
uniform float textureAlpha;
uniform vec3 ambientColor;
uniform vec3 camPos;
uniform mat4 lightSpaceMatrix;

const int MAX_BONES = 100;
const int MAX_BONE_INFLUENCE = 4;
uniform mat4 finalBonesMatrices[MAX_BONES];

out vec3 vPosition;
out vec3 vAmbientColor;
out vec2 vTexCoords;
out vec3 vCamPos;
out vec3 vNormal;
out float vTextureAlpha;
out vec4 fragPosLightSpace;
out mat3 tbnMatrix;

out vec3 tangentLightPos;
out vec3 tangentViewPos;
out vec3 tangentFragPos;

void main()
{
	vec4 totalPosition = vec4(0.0f);
    for (int i = 0; i < MAX_BONE_INFLUENCE; i++)
    {
        int currentBoneId = int(boneIds[i]);

        if (currentBoneId == -1)
            continue;
        if (currentBoneId >= MAX_BONES)
        {
            totalPosition = vec4(position, 1.0f);
            break;
        }
        vec4 localPosition = finalBonesMatrices[currentBoneId] * vec4(position, 1.0f);
        totalPosition += localPosition * weights[i];
        vec3 localNormal = mat3(finalBonesMatrices[currentBoneId]) * normal;
    }

    if (totalPosition == vec4(0.0f))
        totalPosition = vec4(position, 1.0f);

    gl_Position = projection * view * model * totalPosition;

	vTexCoords = texCoords;
	vPosition = vec3(model * vec4(position, 1));
	vNormal = normalize((model * vec4(normal, 0.0)).xyz);
	vAmbientColor = ambientColor;
	fragPosLightSpace = lightSpaceMatrix * model * totalPosition;
	vCamPos = camPos;
	vTextureAlpha = 1.0f;

	vec3 T = normalize(vec3(model * vec4(tangents, 0.0)));
	vec3 B = normalize(vec3(model * vec4(biTangents, 0.0)));
	vec3 N = normalize(vec3(model * vec4(normal, 0.0)));
	tbnMatrix = mat3(T, B, N);
}


#type fragment
#version 430 core

in vec3 vPosition;
in vec3 vNormal;
in vec2 vTexCoords;
in vec3 vCamPos;
in vec3 vAmbientColor;
in float vTextureAlpha;
in vec4 fragPosLightSpace;
in mat3 tbnMatrix;

layout(location = 0) out vec4 fragColor;
layout(location = 1) out vec4 fragNormals;

layout(location = 0) uniform sampler2D tex;
layout(location = 1) uniform sampler2D depthTexture;
layout(location = 2) uniform sampler2D normalMap;
uniform float normalsThickness;

struct Material
{
	vec3 ambient;
	vec3 diffuse;
	vec3 specular;
	float shininess;
	bool gammaCorrection;
	float gammaCorrectionAmount;
};
uniform Material material;

struct DirLight
{
	vec3 direction;

	vec3 ambient;
	vec3 diffuse;
	vec3 specular;

	float intensity;
	bool genShadows;
};
uniform DirLight dirLight;

struct PointLight
{
	vec3 position;

	vec3 ambient;
	vec3 diffuse;
	vec3 specular;

	float intensity;
	float constant;
	float lin;
	float quadratic;
};
#define MAX_POINT_LIGHTS 4
uniform PointLight pointLights[MAX_POINT_LIGHTS];

struct SpotLight
{
	vec3 position;
	vec3 direction;
	float cutOff;
	float outerCutOff;
	float intensity;

	vec3 ambient;
	vec3 diffuse;
	vec3 specular;
};
#define MAX_SPOT_LIGHTS 4
uniform SpotLight spotLights[MAX_SPOT_LIGHTS];


struct CelShadingProps
{
	float a;
	float b;
	float c;
	float d;
};
const CelShadingProps csp = {0.1f, 0.3f, 0.6f, 1.0f};


vec4 CalculateShadow(vec4 fragPosLightSpace, vec3 normal, vec3 lightDir)
{
	vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
	projCoords = projCoords * 0.5 + 0.5;
	
	float closestDepth = texture(depthTexture, projCoords.xy).x;
	float currentDepth = projCoords.z;

	vec2 texSize = textureSize(tex, 0).xy;
	vec2 depthTexSize = textureSize(depthTexture, 0).xy;

	// Change the color and apply blur so it is not full black
	// ========================
	vec4 colorSum = vec4(0);
	vec2 texCoord = gl_FragCoord.xy / texSize;
	float shadow = 0;
	float dx = dFdx(texCoord.s);
	float dy = dFdy(texCoord.t);
	for (int i = 0; i < 3; ++i)
	{
		for (int j = 0; j < 3; ++j)
		{
			texCoord += vec2(dx * i, dy * j);
			colorSum += texture(tex, texCoord);

			float pcfDepth = texture(depthTexture, projCoords.xy + vec2(i * 0.5, j * 0.5) * depthTexSize).x;
			shadow += currentDepth > pcfDepth - 0.00005 ? 1 : 0;
		}
	}
	
	colorSum = colorSum / 9;
	shadow = shadow / 9;
	shadow = smoothstep(0, 2, shadow);
	// ========================

	vec4 result = mix(vec4(1), normalize(colorSum), shadow);

	return result;	
}

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
	vec3 lightDir = normalize(-light.direction);

	// Diffuse shading
	float diff = max(dot(normal, lightDir), 0.0);
	
	if (diff < csp.a) diff = 0.25f;
	else if (diff < csp.b) diff = csp.b;
	else if (diff < csp.c) diff = csp.c;
	else diff = csp.d;

	// Specular shading
	vec3 reflectDir = reflect(-lightDir, normal);
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
	spec = step(0.5f, spec);
	
	vec3 ambient = light.ambient * material.diffuse * vAmbientColor;
	vec3 diffuse = light.diffuse * diff * material.diffuse;
	vec3 specular = light.specular * spec * material.specular;

	vec4 shadow = light.genShadows ? CalculateShadow(fragPosLightSpace, normal, lightDir) : vec4(1);
	
	return ((ambient + shadow.rgb * shadow.a) * (diffuse + specular)) * light.intensity;
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
	vec3 lightDir = normalize(light.position - fragPos);

	// Diffuse shading
	float diff = max(dot(normal, lightDir), 0.0);

	if (diff < csp.a) diff = 0.25f;
	else if (diff < csp.b) diff = csp.b;
	else if (diff < csp.c) diff = csp.c;
	else diff = csp.d;

	// Specular shading
	vec3 reflectDir = reflect(-lightDir, normal);
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
	spec = step(0.5f, spec);

	// Attenuation
	float distance = length(fragPos - light.position);
	float attenuation = 1.0 / (1 + light.constant + light.lin * distance + light.quadratic * (distance * distance));
	
	attenuation *= light.intensity;

	vec3 ambient = light.ambient * material.diffuse * vAmbientColor;
	vec3 diffuse = light.diffuse * diff * material.diffuse;
	vec3 specular = light.specular * spec * material.specular;

	ambient *= attenuation;
	diffuse *= attenuation;
	specular *= attenuation;

	return (ambient + diffuse + specular);
}

vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{	
	vec3 lightDir = normalize(light.position - fragPos);
	
	float diff = max(dot(normal, lightDir), 0.0);
	
	if (diff < csp.a) diff = 0.25f;
	else if (diff < csp.b) diff = csp.b;
	else if (diff < csp.c) diff = csp.c;
	else diff = csp.d;
	
	vec3 reflectDir = reflect(-lightDir, normal);
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
	spec = step(0.5f, spec);

	//float distance = length(light.position - fragPos);
	
	float theta = dot(lightDir, normalize(-light.direction));
	float epsilon = light.cutOff - light.outerCutOff;
	float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);
	
	//vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));
	//vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));
	//vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));

	vec3 ambient = light.ambient * material.diffuse * material.diffuse;
	vec3 diffuse = light.diffuse * diff * material.diffuse;
	vec3 specular = light.specular * spec * material.specular;


	ambient *=  /*attenuation **/ intensity * light.intensity;
	diffuse  *= /*attenuation **/ intensity * light.intensity;
	specular *= /*attenuation **/ intensity * light.intensity;
	
	return (ambient + diffuse + specular);
}

void main()
{
	//vec3 norm = normalize(vNormal);
	vec3 norm = texture(normalMap, vTexCoords).rgb;
	norm = normalize(norm * 2.0 - 1.0);
	
	vec3 viewDir = normalize(vCamPos - vPosition);
	
	vec3 result = CalcDirLight(dirLight, norm, viewDir);
	
	for (int i = 0; i < MAX_POINT_LIGHTS; ++i)
		result += CalcPointLight(pointLights[i], norm, vPosition, viewDir);
	
	for (int i = 0; i < MAX_SPOT_LIGHTS; ++i)
		result += CalcSpotLight(spotLights[i], norm, vPosition, viewDir);

	vec3 finalColor = result;
	if (material.gammaCorrection)
	{
		finalColor = pow(result, vec3(1.0 / material.gammaCorrectionAmount));
	}

	fragColor = texture(tex , vTexCoords) * vTextureAlpha * vec4(finalColor, 1);
	fragNormals = vec4(vNormal, normalsThickness);

	if (fragColor.a > 0 && fragColor.a <= 0.1)
	{
		fragColor = vec4(vec3(0.5), 0);
		fragNormals.a = -1;
	}
	else if (fragColor.a == 0)
	{
		discard;
	}
}


