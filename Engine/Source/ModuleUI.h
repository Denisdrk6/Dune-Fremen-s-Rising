#pragma once
#include "Module.h"

#include "Geometry/LineSegment.h"
#include "Math/float2.h"
#include "Color.h"
#include <vector>
#include <string>
#include <map>

class Texture;
class GameObject;
typedef unsigned short GLushort;
typedef unsigned int GLuint;
typedef float GLfloat;
typedef unsigned int uint;
#define CONVERSION_FACTOR 600
class Shadert
{
public:
	unsigned int ID;

	Shadert(const std::string& vertex, const std::string& fragment);
	// activate the shader
	// ------------------------------------------------------------------------
	void Use();
	void StopUse();

private:
	// utility function for checking shader compilation/linking errors.
	// ------------------------------------------------------------------------
	void CheckCompileErrors(uint shader, std::string type);
};


class MyPlane
{
public:
	MyPlane(float3 pos, float3 sca);
	~MyPlane();
    void DrawPlane2D(Texture* texture);

public:
    GLuint imageID;
	float3 position;
	float3 scale;
	unsigned int VBO;
	unsigned int VAO;
	//unsigned int EBO;
	unsigned int TBO;
	GLuint aTextureId;
    GameObject* own;
public:
    std::vector<GLushort> indices;
    std::vector<GLfloat> normals;
    std::vector<GLfloat> texCoords;
    std::vector<float3> vertices;

    Shadert* shader;
};
struct IVec2 {
    IVec2() = default;
    IVec2(int x, int y) : x(x), y(y) {}

    int x = 0, y = 0;
};

/// Holds all state information relevant to a character as loaded using FreeType
struct Character {
    unsigned int TextureID; // ID handle of the glyph texture
    IVec2  Size;      // Size of glyph
    IVec2  Bearing;   // Offset from baseline to left/top of glyph
    unsigned int Advance;   // Horizontal offset to advance to next glyph
};

class ModuleUI : public Module
{
public:
	ModuleUI(bool startEnabled = true);
	~ModuleUI();

	bool Start()override;
    bool PreUpdate(float dt)override;
    bool Update(float dt)override;
    void Draw();
	bool CleanUp();

	void SetFocusedObject();
	void HitPosibleFocusedObjects(const math::float4& viewport);

	void RenderText(std::string text, float x, float y, float scale, float3 color, Shadert* shader, std::map<char, Character>* characters, uint VAO, uint VBO);
	void CleanText(std::string& text);
	void DrawCharacters(std::string& text, float& x, float scale, float y, std::map<char, Character>* characters, uint VAO, uint VBO);

    void DeleteUIGameObjects(GameObject* ui);
	inline std::vector<GameObject*>::const_iterator FindUI(GameObject* child) { return std::find(std::begin(UIGameObjects), std::end(UIGameObjects), child); };
	void updateText();

	void OrderButtons();
	void loadFont(std::string path,std::map<char, Character>* chara, Shadert* shade, uint VAO, uint VBO);

	void ImportToLibrary();

public:
    
    
	LineSegment myRay;
	float3 corners[8];
	std::vector<GameObject*> hitObjs;
	GameObject* focusedGameObject;
    std::string textExample = "Default";

    Color color = { 255,255,255 };
    float scale = 1;
    std::vector<GameObject*> UIGameObjects;
    GameObject* UIGameObjectSelected;
    uint nextUIGameObject = 1;
    float2 fMousePos;
};