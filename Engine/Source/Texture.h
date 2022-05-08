#pragma once
#include "Resource.h"

typedef unsigned char GLubyte;

enum class TextureType
{
	NONE = -1,
	DIFFUSE,
	NORMAL = 6, // Because of assimp textures
};

struct TextureParameters
{
	bool alienify = false;
	float blurAvg = 0.0f;
	float blurGaussian = 0.0f;
	bool edgeDetectP = false;
	bool edgeDetectS = false;
	bool emboss = false;
	float gammaCorrectCurve = 0.0f;
	bool negative = false;
	float noise = 0.0f;
	int pixelization = 0;
};

class Texture : public Resource
{
public:
	Texture(uint uid, std::string& assets, std::string& library);
	Texture(uint uid, std::string& library);

	~Texture();

	void Load() override;
	void UnLoad() override;

	void DrawOnEditor() override;

	void Bind(unsigned int index = 0);
	void Unbind();

	inline const std::string& GetPath() const { return path; }
	inline const int& GetWidth() const { return width; }
	inline const int& GetId() const { return id; }
	inline const int& GetHeight() const { return height; }

private:
	void Reimport();

private:
	unsigned int id;
	int width;
	int height;

	std::string path;
	GLubyte* data;
	TextureParameters parameters;
};