#type vertex
#version 430

layout(location = 0) in vec3 position;

uniform mat4 view;
uniform mat4 projection;


void main()
{
	gl_Position = projection * view * vec4(position, 1.0f);
}

#type fragment
#version 430 core

out vec4 fragColor;
uniform float rcolor;
uniform float gcolor;
uniform float bcolor;

void main()
{
	//fragColor = vec4(0, 1, 0, 0.3);
	fragColor = vec4(rcolor, gcolor, bcolor, 0.3);
}