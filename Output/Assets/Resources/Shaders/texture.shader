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
uniform sampler2D tex;
uniform float alpha;

void main()
{
	fragColor = texture2D(tex, vTexCoords) * alpha;
}
