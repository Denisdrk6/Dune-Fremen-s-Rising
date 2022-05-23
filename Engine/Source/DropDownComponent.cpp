#include "Application.h"
#include "Globals.h"
#include "DropDownComponent.h"
#include "ModuleInput.h"
#include "ModuleSceneManager.h"
#include "Scene.h"
#include "ModuleEditor.h"
#include"GameView.h"
#include "ModuleWindow.h"
#include "Resource.h"
#include"Texture.h"
#include"GameObject.h"
#include "MaterialComponent.h"
#include "ResourceManager.h"
#include "GL/glew.h"
#include "freetype-2.10.0/include/ft2build.h"
#include FT_FREETYPE_H 
#include "Profiling.h"

DropDownComponent::DropDownComponent(GameObject* own)
{
	type = ComponentType::UI_DROPDOWN;
	own->isUI = true;
	active = true;
	DropDownText.setText("DropDown", 5, 5, 0.5, { 255,255,255 });
	fontPath = "Library/Fonts/Montserrat-Bold.ttf";

	transform = own->GetComponent<ComponentTransform2D>();
	if (!transform) // If comes from Load not enter
	{
		transform = (ComponentTransform2D*)own->CreateComponent(ComponentType::TRANFORM2D);
		normalMaterial = (MaterialComponent*)own->CreateComponent(ComponentType::MATERIAL);
		focusedMaterial = (MaterialComponent*)own->CreateComponent(ComponentType::MATERIAL);
		pressedMaterial = (MaterialComponent*)own->CreateComponent(ComponentType::MATERIAL);
		disabledMaterial = (MaterialComponent*)own->CreateComponent(ComponentType::MATERIAL);
		actual = normalMaterial;
		loadFont(fontPath.c_str());
		transform->SetButtonHeight(75);
	}
	//
	//app->userInterface->loadFont("Library/Fonts/Montserrat-Bold.ttf", &characters, shader, VAO, VBO);
	app->userInterface->UIGameObjects.push_back(own);
	planeToDraw = new MyPlane(float3{ 0,0,0 }, float3{ 1,1,1 });
	planeToDraw->own = own;
	app->userInterface->OrderButtons();
}
void DropDownComponent::loadFont(const char* path) {
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
DropDownComponent::~DropDownComponent()
{
	RELEASE(planeToDraw);
	RELEASE(shader);
	//for (auto& i : buttonsArray) RELEASE(i);//must die here?
	buttonsArray.clear();
}
std::string DropDownComponent::GetSelect() 
{
	ButtonComponent* aux = (ButtonComponent*)buttonsArray[selectedRaw]->GetComponent<ButtonComponent>();
	return aux->GetButtonText().textt;
}
bool DropDownComponent::Update(float dt)
{
	RG_PROFILING_FUNCTION("Button Update");
	DropDownText.SetOnlyPosition(float2(GetParentPosition().x + textPos.x, GetParentPosition().y + textPos.y));
	if (owner->active) {

		
			if (app->userInterface->focusedGameObject == owner)
			{
				state = State::FOCUSED;
				actual = focusedMaterial;
				if (app->input->GetMouseButton(SDL_BUTTON_LEFT) == KeyState::KEY_DOWN)
				{
					state = State::PRESSED;

					isDeployed = !isDeployed;


					actual = pressedMaterial;
				}
			}
			else
			{
				state = State::NORMAL;
				actual = normalMaterial;
			}
		

		if (isDeployed)
		{
			SetFocusedButtons();
			for (int a = 0; a < buttonsArray.size(); a++)
			{
				ButtonComponent* auxiliarButton = (ButtonComponent*)buttonsArray[a]->GetComponent<ButtonComponent>();
				ComponentTransform2D* auxiliarTransform = (ComponentTransform2D*)buttonsArray[a]->GetComponent<ComponentTransform2D>();

				if (auxiliarButton->GetState() == State::PRESSED)
				{
					changeSelected = true;
					selectedRaw = a;
					char* au = new char[30];
					strcpy(au, auxiliarButton->GetButtonText().textt.c_str());
					this->SetText(au);
					delete[] au;
				}
				auxiliarTransform->SetPosition({ transform->GetPosition().x,transform->GetPosition().y - (transform->GetButtonHeight() * (a + 1)),transform->GetPosition().z });
				auxiliarTransform->SetButtonHeight(transform->GetButtonHeight());
				auxiliarTransform->SetButtonWidth(transform->GetButtonWidth());

				for (int b = 0; b < buttonsArray[a]->components.size(); b++)
				{
					if (buttonsArray[a]->components[b]->type != ComponentType::UI_BUTTON)
					{
						buttonsArray[a]->components[b]->Update(dt);
					}

					//hacer comprobacion de colision y seteo de estado
				}
				UpdateButtons(buttonsArray[a]);
			}
		}
	}
	else 
	{
		state = State::DISABLED;
	}
	

	return true;
}

void DropDownComponent::Draw(CameraComponent* gameCam)
{
	glAlphaFunc(GL_GREATER, 0.5);
	glEnable(GL_ALPHA_TEST);

	planeToDraw->DrawPlane2D(actual->GetTexture().get());

	if (isDeployed)
	{
		for (int a = 0; a < buttonsArray.size(); a++)
		{
			ButtonComponent* auxiliar = (ButtonComponent*)buttonsArray[a]->GetComponent<ButtonComponent>();
			auxiliar->Draw();
			app->userInterface->RenderText(auxiliar->GetButtonText().textt, auxiliar->GetButtonText().X, auxiliar->GetButtonText().Y, auxiliar->GetButtonText().Scale, auxiliar->GetButtonText().Color, auxiliar->shader, &auxiliar->characters, auxiliar->VAO, auxiliar->VBO);
			
		}
	}
	glDisable(GL_ALPHA_TEST);
	glColor3f(255, 255, 255);
}
void DropDownComponent::SetActualColor(float Red, float Green, float Blue)
{
	generalColor.r = Red / 255;
	generalColor.g = Green / 255;
	generalColor.b = Blue / 255;
}
void DropDownComponent::setTextColor(float Red, float Green, float Blue)
{

	textColor.r = Red / 255;
	textColor.g = Green / 255;
	textColor.b = Blue / 255;
	DropDownText.setOnlyColor({ textColor.r, textColor.g, textColor.b });
}
float3 DropDownComponent::GetTextColor()
{
	return float3(textColor.r, textColor.g, textColor.b);
}
void DropDownComponent::OnEditor()
{
	if (ImGui::CollapsingHeader("DropDownComponent"))
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

		DropDownText.setOnlyColor({ textColor.r, textColor.g, textColor.b });

		ImGui::SliderFloat("Alpha", &alpha, 0.5f, 1.0f);

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
		if (ImGui::InputText("Text", text, IM_ARRAYSIZE(text)))
			DropDownText.setOnlyText(text);
		ImGui::DragFloat("Font Size", &DropDownText.Scale, 0.1, 0, 10);

		if (ImGui::Button("New Line"))
		{
			GameObject* object = new GameObject();
			ButtonComponent* button = new ButtonComponent(object, true);
			object->AddComponent(button);
			object->SetName("Button");
			buttonsArray.push_back(object);
			ComponentTransform2D* transformAux= object->GetComponent<ComponentTransform2D>();
			
			transformAux->SetPosition({ transform->GetPosition().x,transform->GetPosition().y - (transform->GetButtonHeight() * buttonsArray.size()),transform->GetPosition().z });
			transformAux->SetButtonHeight(transform->GetButtonHeight());
			transformAux->SetButtonWidth(transform->GetButtonWidth());
		}
		ImGui::Checkbox("isDeployed", &isDeployed);

		for(int a=0;a< buttonsArray.size();a++)
		{
			ButtonComponent* auxiliarButton = (ButtonComponent*)buttonsArray[a]->GetComponent<ButtonComponent>();
			auxiliarButton->OnEditor();
		}


		ComponentOptions(this);
		ImGui::Separator();
	}
}
void DropDownComponent::SetFocusedButtons()
{
	float2 fMousePos;
	float2 mPos = float2::zero;
	float4 viewport = float4::zero;
#ifndef DIST
	mPos = { ImGui::GetIO().MousePos.x, ImGui::GetIO().MousePos.y };
	viewport = app->editor->GetGameView()->GetBounds();
#else
	mPos = { (float)app->input->GetMouseX() ,(float)app->input->GetMouseY() };
	viewport = { 0,0, (float)*app->window->GetWindowWidth(), (float)*app->window->GetWindowHeight() };
#endif
	fMousePos = { mPos.x - viewport.x , mPos.y - viewport.y };

	for (int i = 0; i < buttonsArray.size(); i++)
	{
		GameObject* go = buttonsArray[i];
		ComponentTransform2D* transform2D = go->GetComponent<ComponentTransform2D>();

		float3 position = transform2D->GetPosition();

		ButtonComponent* buttonComp = (ButtonComponent*)go->GetComponent<ButtonComponent>();
		if (buttonComp)
		{
			ComponentTransform2D* button = (ComponentTransform2D*)go->GetComponent<ComponentTransform2D>();

			// Whats does this do?
			//DEBUG_LOG("POSITION X %f, POSITION Y %f viewport.z %f", fMousePos.x, fMousePos.y, viewport.z);
			float posXMin = ((viewport.z / 2) + (position.x)) - (button->GetButtonWidth() / 2);
			float posXMax = ((viewport.z / 2) + (position.x)) + (button->GetButtonWidth() / 2);

			float posYMin = ((viewport.w / 2) + (-(position.y))) - (button->GetButtonHeight() / 2);
			float posYMax = ((viewport.w / 2) + (-(position.y))) + (button->GetButtonHeight() / 2);

			//ImageComponent* image = go->GetComponent<ImageComponent>();
			if ((fMousePos.x > posXMin && fMousePos.x < posXMax && fMousePos.y > posYMin && fMousePos.y < posYMax))
			{
				hitObjs.push_back(go);
			}
		}
	}

	if (hitObjs.size() > 0)
	{
		std::vector<float> distance;
		float nearestDistance = 100000.0f;
		int nearObj = 0;
		for (int i = 0; i > hitObjs.size(); ++i)
		{
			ComponentTransform2D* transform2D = hitObjs[i]->GetComponent<ComponentTransform2D>();

			float3 position = transform2D->GetPosition();
			distance.push_back(position.z);
			if (distance[i] > nearestDistance)
			{
				nearestDistance = distance[i];
				nearObj = i;
			}
		}
		focusedGameObject = hitObjs[nearObj];

		hitObjs.clear();
	}
	else
		focusedGameObject = nullptr;
}

void DropDownComponent::UpdateButtons(GameObject* auxiliar) 
{
	ButtonComponent* auxiliarButton = (ButtonComponent*)auxiliar->GetComponent<ButtonComponent>();
	/*auxiliarButton->GetButtonText().SetOnlyPosition(float2(auxiliarButton->GetParentPosition().x + auxiliarButton->GetTextPosition().x, auxiliarButton->GetParentPosition().y + auxiliarButton->GetTextPosition().y));*/
	auxiliarButton->setParent();
	if (auxiliarButton->GetState() != State::DISABLED)
	{
		if (focusedGameObject == auxiliar)
		{
			auxiliarButton->SetState((int)State::FOCUSED);
			auxiliarButton->SetActualMaterial(auxiliarButton->GetFocusedMaterial());
			if (app->input->GetMouseButton(SDL_BUTTON_LEFT) == KeyState::KEY_REPEAT)
			{
				auxiliarButton->SetState((int)State::PRESSED);
				auxiliarButton->SetActualMaterial(auxiliarButton->GetPressedMaterial());
			}
		}
		else
		{
			auxiliarButton->SetState((int)State::NORMAL);
			auxiliarButton->SetActualMaterial(auxiliarButton->GetNormalMaterial());
		}
	}
}
float2 DropDownComponent::GetParentPosition()
{
	float3 position = transform->GetPosition();

	return{ position.x / 2 ,position.y / 2 };
}
bool DropDownComponent::OnLoad(JsonParsing& node)
{
	std::string aux = node.GetJsonString("buttonText");
	strcpy(text, aux.c_str());
	DropDownText.textt = text;

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
	loadFont(fontPath.c_str());
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
		}else if (owner->components[a]->type == ComponentType::TRANFORM2D)		
		{
			transform= (ComponentTransform2D*)owner->components[a];
		}
	}
	int numOfButtons= node.GetJsonNumber("numOfButtons");
	std::string name;
	std::string number;
	for (int a = 0; a < numOfButtons; a++)
	{
		number = std::to_string(a);
		GameObject* object = new GameObject();
		ButtonComponent* button = new ButtonComponent(object, true);
		object->AddComponent(button);
		object->SetName("Button");
		buttonsArray.push_back(object);
		ComponentTransform2D* transformAux = object->GetComponent<ComponentTransform2D>();

		transformAux->SetPosition({ transform->GetPosition().x,transform->GetPosition().y - (transform->GetButtonHeight() * buttonsArray.size()),transform->GetPosition().z });
		transformAux->SetButtonHeight(transform->GetButtonHeight());
		transformAux->SetButtonWidth(transform->GetButtonWidth());

		name = "buttonText";
		name = name + number;
		std::string aux = node.GetJsonString(name.c_str());
		strcpy(button->GetText(), aux.c_str());
		button->SetText(button->GetText());

		name = "fontScale";
		name = name + number;
		button->SetFontScale(node.GetJsonNumber(name.c_str()));


		name = "textColor.r";
		name = name + number;
		float R = node.GetJsonNumber(name.c_str());

		name = "textColor.g";
		name = name + number;
		float G = node.GetJsonNumber(name.c_str());

		name = "textColor.b";
		name = name + number;
		float B = node.GetJsonNumber(name.c_str());	
		button->GetButtonText().setOnlyColor({R, G, B});

		float3 position;
		name = "textPositionX";
		name = name + number;
		
		position.x = node.GetJsonNumber(name.c_str());

		name = "textPositionY";
		name = name + number;
		position.y = node.GetJsonNumber(name.c_str());
		position.z = 0;
		button->SetTextPosition(position);

		name = "alpha";
		name = name + number;
		button->SetAlpha(node.GetJsonNumber(name.c_str()));

		name = "text";
		name = name + number;
		aux = node.GetJsonString(name.c_str());
		button->GetNormalMaterial()->SetTexture(ResourceManager::GetInstance()->LoadResource(aux));
		button->GetFocusedMaterial()->SetTexture(ResourceManager::GetInstance()->LoadResource(aux));
		button->GetPressedMaterial()->SetTexture(ResourceManager::GetInstance()->LoadResource(aux));
		button->GetActualMaterial()->SetTexture(ResourceManager::GetInstance()->LoadResource(aux));
	}


	return true;
}

bool DropDownComponent::OnSave(JsonParsing& node, JSON_Array* array)
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
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "numOfButtons", buttonsArray.size());
	std::string name;
	std::string number;

	for (int a = 0; a < buttonsArray.size(); a++) 
	{
		ButtonComponent* auxiliarButton = (ButtonComponent*)buttonsArray[a]->GetComponent<ButtonComponent>();
		
		number = std::to_string(a);
		
		name = "buttonText";
		name = name + number;
		file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), name.c_str(), auxiliarButton->GetText());

		name = "fontPath";
		name = name + number;
		file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), name.c_str(), fontPath.c_str());

		name = "fontScale";
		name = name + number;
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), name.c_str(), auxiliarButton->GetFontScale());

		name = "textColor.r";
		name = name + number;
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), name.c_str(), auxiliarButton->GetTextColor().x);

		name = "textColor.g";
		name = name + number;
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), name.c_str(), auxiliarButton->GetTextColor().y);

		name = "textColor.b";
		name = name + number;
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), name.c_str(), auxiliarButton->GetTextColor().z);

		name = "textPositionX";
		name = name + number;
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), name.c_str(), auxiliarButton->GetTextPosition().x);

		name = "textPositionY";
		name = name + number;
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), name.c_str(), auxiliarButton->GetTextPosition().y);

		name = "alpha";
		name = name + number;		
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), name.c_str(), auxiliarButton->GetAlpha());

		name = "text";
		name = name + number;
		std::shared_ptr<Texture> diff = nullptr;
		diff=buttonsArray[a]->GetComponent<MaterialComponent>()->GetTexture();		
		file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), name.c_str(), diff->GetAssetsPath().c_str());
	}
	node.SetValueToArray(array, file.GetRootValue());

	return true;
}