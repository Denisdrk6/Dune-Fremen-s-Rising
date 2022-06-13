#type vertex
#version 430 core

layout(location = 0) in vec3 position;
layout(location = 1) in vec2 texCoords;

out vec2 vTexCoords;
out vec3 vPosition;

void main()
{
	gl_Position = vec4(position, 1.0f);
	vTexCoords = texCoords;
	vPosition = position;
}

#type fragment
#version 430 core

in vec2 vTexCoords;
in vec3 vPosition;

out vec4 fragColor;
uniform sampler2D colorTexture;
uniform sampler2D normalTexture;
uniform sampler2D depthTexture;

void main()
{
	vec2 texSize = textureSize(colorTexture, 0).xy;
	vec2 texCoord = gl_FragCoord.xy / texSize;

	float minDistance = 0.5;
	float maxDistance = 2.0;

	vec4 realPixelDepth = texture(depthTexture, vTexCoords);
	vec4 realPixelNormal = texture(normalTexture, vTexCoords);

	float dx = dFdx(vTexCoords.s);
	float dy = dFdy(vTexCoords.t);
	if (realPixelNormal.a < 1)
	{
		dx /= 8;
		dy /= 8;
	}

	vec4 result = texture(colorTexture, vTexCoords);

	if (realPixelNormal.a < 0)
	{
		fragColor = result;
		//discard;
		return;
	}

	if (vTexCoords.y > 0.002 && vTexCoords.y < 0.998 && vTexCoords.x > 0.002 && vTexCoords.x < 0.998)
	{
		float maxDepth = 0;
		float maxNormal = 0;
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				texCoord += vec2(dx * i, dy * j);
				vec4 currentPixelDepth = texture(depthTexture, texCoord);
				vec4 currentPixelNormal = texture(normalTexture, texCoord);

				maxDepth = max(maxDepth, length(currentPixelDepth - realPixelDepth));
				maxNormal = max(maxNormal, length(currentPixelNormal - realPixelNormal));
			}
		}

		float normalThreshold = 0.2f;
		float depthThreshold = 0.005f;

		if (maxNormal > normalThreshold || maxDepth > depthThreshold)
		{
			result = vec4(0, 0, 0, 1);
			if (realPixelNormal.a == -1)
				result.a = 0;
		}
	}

	fragColor = result;
}
