#type vertex
#version 430

layout(location = 0) in vec3 position;
layout(location = 1) in vec4 color;
layout(location = 2) in float detectionTime;

uniform mat4 view;
uniform mat4 projection;

out vec4 col;
out float detection;

void main()
{
	gl_Position = projection * view * vec4(position, 1.0f);
	col = color;
	detection = detectionTime;
}

#type fragment
#version 430 core

out vec4 fragColor;
in vec4 col;
in float detection;

void main()
{
	if (detection < 0)
	{
		fragColor = col;
		return;
	}
	
	fragColor = step(detection, vec4(1,0,0,1));
}