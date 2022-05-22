#include "ButtonComponent.h"
#include "Application.h"
#include "Globals.h"

#include "ModuleInput.h"


#include"GameObject.h"
#include "MaterialComponent.h"
#include "Transform2DComponent.h"
#include "GL/glew.h"
#include "freetype-2.10.0/include/ft2build.h"
#include FT_FREETYPE_H 
#include "Profiling.h"

ButtonComponent::ButtonComponent(GameObject* own)
{
	type = ComponentType::UI_BUTTON;
	own->isUI = true;
	active = true;
	buttonText.setText("Button", 5, 5, 0.5, { 255,255,255 });
	fontPath = "Library/Fonts/Montserrat-Bold.ttf";
	transform = own->GetComponent<ComponentTransform2D>();
	if (!transform) // If comes from Load not enter
	{
		own->CreateComponent(ComponentType::TRANFORM2D);
		transform = own->GetComponent<ComponentTransform2D>();
		normalMaterial = (MaterialComponent*)own->CreateComponent(ComponentType::MATERIAL);
		focusedMaterial = (MaterialComponent*)own->CreateComponent(ComponentType::MATERIAL);
		pressedMaterial = (MaterialComponent*)own->CreateComponent(ComponentType::MATERIAL);
		disabledMaterial = (MaterialComponent*)own->CreateComponent(ComponentType::MATERIAL);
		actual = normalMaterial;
		LoadFont(fontPath.c_str());
	}
	
	//app->userInterface->loadFont("Library/Fonts/Montserrat-Bold.ttf", &characters, shader, VAO, VBO);
	app->userInterface->UIGameObjects.push_back(own);
	planeToDraw = new MyPlane(float3{ 0,0,0 }, float3{ 1,1,1 });
	planeToDraw->own = own;
	app->userInterface->OrderButtons();
}
ButtonComponent::ButtonComponent(GameObject* own,bool isPart)
{
	type = ComponentType::UI_BUTTON;
	own->isUI = true;
	active = true;
	buttonText.setText("Button", 5, 5, 0.5, { 255,255,255 });
	fontPath = "Library/Fonts/Montserrat-Bold.ttf";
	
	own->CreateComponent(ComponentType::TRANFORM2D);
	normalMaterial = (MaterialComponent*)own->CreateComponent(ComponentType::MATERIAL);
	focusedMaterial = (MaterialComponent*)own->CreateComponent(ComponentType::MATERIAL);
	pressedMaterial = (MaterialComponent*)own->CreateComponent(ComponentType::MATERIAL);
	disabledMaterial = (MaterialComponent*)own->CreateComponent(ComponentType::MATERIAL);
	actual = normalMaterial;
	LoadFont(fontPath.c_str());
	

	planeToDraw = new MyPlane(float3{ 0,0,0 }, float3{ 1,1,1 });
	planeToDraw->own = own;

}
void ButtonComponent::LoadFont(const char* path) {
	FT_Library ft;
	if (FT_Init_FreeType(&ft))
	{
		DEBUG_LOG("ERROR::FREETYPE: Could not init FreeType Library");
		//return false;
	}

	FT_Face face;
	//"Library/Fonts/Montserrat-Bold.ttf"

	if (FT_New_Face(ft, path, 0, &face))
	{
		DEBUG_LOG("ERROR::FREETYPE: Failed to load font");
		//return false;
	}
	else {
		// set size to load glyphs as
		FT_Set_Pixel_Sizes(face, 0, 48);

		// disable byte-alignment restriction
		glPixelStorei(GL_UNPACK_ALIGNMENT, 1);

		// load first 128 characters of ASCII set
		for (unsigned char c = 0; c < 128; c++)
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

	shader = new Shadert("", "");
}
ButtonComponent::~ButtonComponent()
{
	RELEASE(planeToDraw);
	RELEASE(shader);
}

bool ButtonComponent::Update(float dt)
{
	
	RG_PROFILING_FUNCTION("Button Update");
	buttonText.SetOnlyPosition(float2(GetParentPosition().x + textPos.x, GetParentPosition().y + textPos.y));

	/*if (!active)
		state = State::DISABLED;
	else
		state = State::NORMAL;*/
	if (owner->active && !visualDisabled) {
		
			if (app->userInterface->focusedGameObject == owner)
			{
				state = State::FOCUSED;
				actual = focusedMaterial;
				if (app->input->GetMouseButton(SDL_BUTTON_LEFT) == KeyState::KEY_REPEAT)
				{
					state = State::PRESSED;
					actual = pressedMaterial;
				}
			}
			else
			{
				state = State::NORMAL;
				actual = normalMaterial;
			}
		
	}
	else 
	{
		state = State::DISABLED;
	}
	return true;
}

void ButtonComponent::Draw(CameraComponent* gameCam)
{

	glAlphaFunc(GL_GREATER, 0.5);
	glEnable(GL_ALPHA_TEST);

		planeToDraw->DrawPlane2D(actual->GetTexture().get());

	glDisable(GL_ALPHA_TEST);
	glColor3f(255, 255, 255);
	
}
void ButtonComponent::SetActualColor(float Red, float Green, float Blue)
{
	generalColor.r = Red / 255;
	generalColor.g = Green / 255;
	generalColor.b = Blue / 255;
}
void ButtonComponent::setTextColor(float Red, float Green, float Blue)
{

	textColor.r = Red/255;
	textColor.g = Green / 255;
	textColor.b = Blue / 255;
	buttonText.setOnlyColor({ textColor.r, textColor.g, textColor.b });
}
float3 ButtonComponent::GetTextColor()
{
	return float3(textColor.r, textColor.g, textColor.b);
}
void ButtonComponent::OnEditor()
{
	if (ImGui::CollapsingHeader("ButtonComponent"))
	{
		static float multiplier = 1;
		static float fadeDuration = 0.1f;

		// Manage if colors are being edited or not
		static bool textColorEditable = false;

		Checkbox(this, "Active", active);
		ImGui::Checkbox("Interactable", &active);

		ImGui::Separator();

		ImGui::DragFloat("Position X", &textPos.x, 0.2f);
		ImGui::DragFloat("Position Y", &textPos.y, 0.2f);

		ImGui::Text("Text Color"); ImGui::SameLine();
		if (ImGui::ColorButton("Text Color", ImVec4(textColor.r, textColor.g, textColor.b, textColor.a)))
			textColorEditable = !textColorEditable;

		if (textColorEditable)
			ImGui::ColorPicker3("Text Color", &textColor);

		buttonText.setOnlyColor({ textColor.r, textColor.g, textColor.b });

		ImGui::SliderFloat("Alpha", &alpha, 0.5f, 1.0f);

		ImGui::InputFloat("Fade Duration", &fadeDuration);

		char au[384];
		strcpy(au, fontPath.c_str());
		if (ImGui::InputText("Font Path", au, IM_ARRAYSIZE(au)))

		fontPath = au;
		if(ImGui::Button("Set Font"))
		{			
			RELEASE(shader);
			shader = nullptr;
			VAO = 0;
			VBO = 0;
			characters.clear();
			LoadFont(fontPath.c_str());
		}
		if (ImGui::InputText("Text", text, IM_ARRAYSIZE(text)))
			buttonText.setOnlyText(text);
		ImGui::DragFloat("Font Size", &buttonText.Scale, 0.1, 0, 10);

		ComponentOptions(this);
		ImGui::Separator();
	}
}

float2 ButtonComponent::GetParentPosition()
{
	float3 position = transform->GetPosition();

	return{ position.x / 2 ,position.y / 2 };
}
bool ButtonComponent::OnLoad(JsonParsing& node)
{
	std::string aux = node.GetJsonString("buttonText");
	strcpy(text, aux.c_str());
	buttonText.textt = text;

	fontScale = node.GetJsonNumber("fontScale");
	textColor.r = node.GetJsonNumber("textColor.r");
	textColor.g = node.GetJsonNumber("textColor.g");
	textColor.b = node.GetJsonNumber("textColor.b");

	textPos.x = node.GetJsonNumber("textPositionX");
	textPos.y = node.GetJsonNumber("textPositionY");
	alpha = node.GetJsonNumber("alpha");
	int cont = 0;
	if (node.GetJsonString("fontPath")) {
		fontPath = node.GetJsonString("fontPath");
	}
	
	LoadFont(fontPath.c_str());
	for (int a = 0; a < owner->components.size(); a++) {
		if (owner->components[a]->type == ComponentType::MATERIAL)
		{
			switch (cont)
			{
			case 0:
				normalMaterial = (MaterialComponent*)owner->components[a];
				break;
			case 1:
				focusedMaterial = (MaterialComponent*)owner->components[a];
				break;
			case 2:
				pressedMaterial = (MaterialComponent*)owner->components[a];
				break;
			case 3:
				disabledMaterial = (MaterialComponent*)owner->components[a];
				actual = normalMaterial;
				break;
			default:
				break;
			}
			cont++;
		}
	}
	return true;
}

bool ButtonComponent::OnSave(JsonParsing& node, JSON_Array* array)
{
	JsonParsing file = JsonParsing();

	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Type", (int)type); 
	file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), "buttonText", text);
	file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), "fontPath", fontPath.c_str());
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "fontScale", fontScale);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "textColor.r", textColor.r);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "textColor.g", textColor.g);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "textColor.b", textColor.b);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "textPositionX", textPos.x);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "textPositionY", textPos.y);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "alpha", alpha);
	node.SetValueToArray(array, file.GetRootValue());

	return true;
}
