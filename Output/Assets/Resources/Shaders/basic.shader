#type vertex
#version 430

layout(location = 0) in vec3 position;
layout(location = 1) in vec4 color;

uniform mat4 view;
uniform mat4 projection;

out vec4 col;

void main()
{
	gl_Position = projection * view * vec4(position, 1.0f);
	col = color;
}

#type fragment
#version 430 core

out vec4 fragColor;
in vec4 col;

void main()
{
	//fragColor = vec4(0, 1, 0, 0.3);
	fragColor = col;
}