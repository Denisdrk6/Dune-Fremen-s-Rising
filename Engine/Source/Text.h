#pragma once

#include <string>
#include "Math/float2.h"

class Text {
public:
	std::string textt;
	float X, Y, Scale;
	float3 Color;

	void setText(std::string text, float x, float y, float scale, float3 color)
	{
		textt = text;
		X = x;
		Y = y;
		Scale = scale;
		Color = color;
	}
	void setOnlyText(std::string text)
	{
		textt = text;
	}
	void setOnlyColor(float3 color)
	{
		Color = color;
	}
	void SetOnlyPosition(float2 pos)
	{
		X = pos.x;
		Y = pos.y;
	}
	void SetPositionX(float x ) {
		X = x;
	}
	void SetPositionY(float y) {
		Y = y;
	}
};