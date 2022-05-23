#include "TextComponent.h"
#include "Application.h"

#include "ModuleInput.h"

#include"FileSystem.h"
#include "GameObject.h"

#include "ModuleSceneManager.h"
#include "TransformComponent.h"
#include "MaterialComponent.h"
#include "Transform2DComponent.h"
#include "Globals.h"
#include "GL/glew.h"
#include "freetype-2.10.0/include/ft2build.h"
#include FT_FREETYPE_H 
#include "Profiling.h"

TextComponent::TextComponent(GameObject* own)
{
	owner = own;
	type = ComponentType::UI_TEXT;
	own->isUI = true;
	active = true;
	textToShow.setText("Text", 5, 5, 0.5, { 255,255,255 });
	fontPath = "Library/Fonts/Montserrat-Bold.ttf";
	if (!own->GetComponent<ComponentTransform2D>()) // If comes from Load not enter
	{
		own->CreateComponent(ComponentType::TRANFORM2D);
		own->CreateComponent(ComponentType::MATERIAL);
		loadFont(fontPath);	
	}
	//app->userInterface->loadFont("Library/Fonts/Montserrat-Bold.ttf", &characters, shader, VAO, VBO);
	//RELEASE(shader); not needed at start
	shader = new Shadert("", "");
	app->userInterface->UIGameObjects.push_back(own);
	app->userInterface->OrderButtons();
	//planeToDraw = new MyPlane(float3{ 0,0,0 }, float3{ 1,1,1 });
	//planeToDraw->own = own;
	savetext();
	planeToDraw = new MyPlane(float3{ 0,0,0 }, float3{ 1,1,1 });
	planeToDraw->own = own;

	// Get components
	if (owner)
	{
		ownerTransform2DComponent = owner->GetComponent<ComponentTransform2D>();
		ownerMaterialComponent = owner->GetComponent<MaterialComponent>();
	}
}

TextComponent::~TextComponent()
{
	RELEASE(shader);
	RELEASE(planeToDraw);
}

bool TextComponent::Update(float dt)
{
	RG_PROFILING_FUNCTION("Text Update");

	textToShow.SetOnlyPosition(float2(GetParentPosition().x, GetParentPosition().y));
	
	//textToShow.setOnlyText(text);
	textToShow.Scale = fontScale;
	return true;
}

void TextComponent::Draw(CameraComponent* gameCam)
{
	if (owner->active) {
		glAlphaFunc(GL_GREATER, 0.5);
		glEnable(GL_ALPHA_TEST);

		planeToDraw->DrawPlane2D(ownerMaterialComponent->GetTexture().get());

		glDisable(GL_ALPHA_TEST);
		glColor3f(255, 255, 255);
	}
}
void TextComponent::setTextColor(float Red, float Green, float Blue)
{
	textColor.r = Red;
	textColor.g = Green;
	textColor.b = Blue;
	textToShow.setOnlyColor({ textColor.r, textColor.g, textColor.b });
}
float3 TextComponent::GetTextColor() 
{
	return float3(textColor.r, textColor.g, textColor.b);
}
void TextComponent::OnEditor()
{
	if (ImGui::CollapsingHeader("Text"))
	{
		static float multiplier = 1;
		static float fadeDuration = 0.1f;
		static bool textColorEditable = false;

		Checkbox(this, "Active", active);
		char* aux=new char[fileText.size()];
		strcpy(aux, fileText.c_str());
		ImGui::InputText("Text Path", aux, 50);
		fileText = aux;
		delete[] aux;
		ImGui::Text("Text Color"); ImGui::SameLine();
		if (ImGui::ColorButton("Text Color", ImVec4(textColor.r, textColor.g, textColor.b, textColor.a)))
			textColorEditable = !textColorEditable;

		if (textColorEditable)
			ImGui::ColorPicker3("Text Color", &textColor);

		textToShow.setOnlyColor({ textColor.r, textColor.g, textColor.b });

		ImGui::SliderFloat("Color Multiplier", &multiplier, 1, 5);
		ImGui::InputFloat("Fade Duration", &fadeDuration);

		char au[384];
		strcpy(au, fontPath.c_str());
		if (ImGui::InputText("Font Path", au, IM_ARRAYSIZE(au)))

			fontPath = au;
		if (ImGui::Button("Set Font"))
		{

			RELEASE(shader);
			shader = nullptr;
			VAO = 0;
			VBO = 0;
			characters.clear();
			loadFont(fontPath.c_str());
		}


		char text[500];
		strcpy(text, textToShow.textt.c_str());
		if(ImGui::InputText("Texte", text, IM_ARRAYSIZE(text)))
			textToShow.setOnlyText(text);
		textToShow.Scale = fontScale;
		ImGui::DragFloat("Font Size", &textToShow.Scale, 0.1, 0, 10);

		ComponentOptions(this);
		ImGui::Separator();
	}
}

float2 TextComponent::GetParentPosition()
{
	float3 position = ownerTransform2DComponent->GetPosition();
	return { position.x/2, position.y/2 };
	
}
bool TextComponent::OnLoad(JsonParsing& node)
{
	std::string aux = node.GetJsonString("buttonText");
	//strcpy(text, aux.c_str());
	

	fontScale = node.GetJsonNumber("fontScale");
	textColor.r = node.GetJsonNumber("textColor.r");
	textColor.g = node.GetJsonNumber("textColor.g");
	textColor.b = node.GetJsonNumber("textColor.b");
	fileText = node.GetJsonString("TextFile");
	if (fileText == "-")
	{
		textToShow.textt = aux;
	}
	else 
	{
		loadtext(fileText);
	}

	if (node.GetJsonString("fontPath")) {
		
	}
	fontPath = node.GetJsonString("fontPath");

	loadFont(fontPath);


	return true;
}

bool TextComponent::OnSave(JsonParsing& node, JSON_Array* array)
{
	JsonParsing file = JsonParsing();

	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Type", (int)type);
	file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), "buttonText", textToShow.textt.c_str());
	file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), "fontPath", fontPath.c_str());
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "fontScale", fontScale);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "textColor.r", textColor.r);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "textColor.g", textColor.g);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "textColor.b", textColor.b);
	file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), "TextFile", fileText.c_str());
	node.SetValueToArray(array, file.GetRootValue());
	

	return true;
}

void TextComponent::loadtext(std::string path)
{
	JsonParsing dialogFile = JsonParsing();
	if (dialogFile.ParseFile(path.c_str()) > 0)
	{
		std::string total;

		JSON_Array* jsonArray = dialogFile.GetJsonArray(dialogFile.ValueToObject(dialogFile.GetRootValue()), "AllText");
		JsonParsing c = dialogFile.GetJsonArrayValue(jsonArray, 0);
		std::string aux = c.GetJsonString("Text");


		total = aux;
		textToShow.textt = total;
	}

	

}

void TextComponent::loadFont(std::string path) {
	FT_Library ft;
	if (FT_Init_FreeType(&ft))
	{
		//LOG("ERROR::FREETYPE: Could not init FreeType Library");
		//return false;
	}

	FT_Face face;
	//"Library/Fonts/Montserrat-Bold.ttf"

	if (FT_New_Face(ft, path.c_str(), 0, &face))
	{
		//LOG("ERROR::FREETYPE: Failed to load font");
		//return false;
	}
	else {
		// set size to load glyphs as
		FT_Set_Pixel_Sizes(face, 0, 48);

		// disable byte-alignment restriction
		glPixelStorei(GL_UNPACK_ALIGNMENT, 1);

		// load first 128 characters of ASCII set
		for (unsigned char c = 0; c < 255; c++)
		{
			// Load character glyph 
			if (FT_Load_Char(face, c, FT_LOAD_RENDER))
			{
				DEBUG_LOG("ERROR::FREETYTPE: Failed to load Glyph");
				continue;
			}
			// generate texture
			unsigned int texture;
			glGenTextures(1, &texture);
			glBindTexture(GL_TEXTURE_2D, texture);
			glTexImage2D(
				GL_TEXTURE_2D,
				0,
				GL_RED,
				face->glyph->bitmap.width,
				face->glyph->bitmap.rows,
				0,
				GL_RED,
				GL_UNSIGNED_BYTE,
				face->glyph->bitmap.buffer
			);
			// set texture options
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
			// now store character for later use
			Character character = {
				texture,
				IVec2(face->glyph->bitmap.width, face->glyph->bitmap.rows),
				IVec2(face->glyph->bitmap_left, face->glyph->bitmap_top),
				static_cast<unsigned int>(face->glyph->advance.x)
			};
			characters.insert(std::pair<char, Character>(c, character));
		}
		glBindTexture(GL_TEXTURE_2D, 0);
	}
	// destroy FreeType once we're finished
	FT_Done_Face(face);
	FT_Done_FreeType(ft);


	// configure VAO/VBO for texture quads
	// -----------------------------------
	glGenVertexArrays(1, &VAO);
	glGenBuffers(1, &VBO);
	glBindVertexArray(VAO);
	glBindBuffer(GL_ARRAY_BUFFER, VBO);
	glBufferData(GL_ARRAY_BUFFER, sizeof(float) * 6 * 4, NULL, GL_DYNAMIC_DRAW);
	glEnableVertexAttribArray(0);
	glVertexAttribPointer(0, 4, GL_FLOAT, GL_FALSE, 4 * sizeof(float), 0);
	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glBindVertexArray(0);

	RELEASE(shader);
	shader = new Shadert("", "");
}
void TextComponent::savetext()
{
	JsonParsing TextFile;
	JSON_Array* arrayDialogue = TextFile.SetNewJsonArray(TextFile.GetRootValue(), "AllText");


	JsonParsing file2 = JsonParsing();

	file2.SetNewJsonString(file2.ValueToObject(file2.GetRootValue()), "Text", "vvvvvvvvv");
	file2.SetNewJsonString(file2.ValueToObject(file2.GetRootValue()), "Text1", "oooo");
	file2.SetNewJsonString(file2.ValueToObject(file2.GetRootValue()), "Text2", "eeeeeeeeoo");
	TextFile.SetValueToArray(arrayDialogue, file2.GetRootValue());


	char* buf;
	uint size = TextFile.Save(&buf);

	std::string savePath = "Assets/Dialogues/";
	savePath += "Credits";
	savePath += ".rgdialogue";

	if (app->fs->Save(savePath.c_str(), buf, size) > 0)
		//DEBUG_LOG("Dialogue saved succesfully");
	
		//DEBUG_LOG("Dialogue couldn't be saved");

	RELEASE_ARRAY(buf);


}