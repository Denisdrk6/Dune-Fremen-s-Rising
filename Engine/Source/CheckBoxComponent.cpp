#include "CheckboxComponent.h"
#include "Application.h"
#include "Globals.h"

#include"ModuleUI.h"
#include"ModuleInput.h"

#include "GameObject.h"
#include"MaterialComponent.h"
#include "Transform2DComponent.h"

#include "GL/glew.h"

#include "Profiling.h"

CheckboxComponent::CheckboxComponent(GameObject* own)
{
	type = ComponentType::UI_CHECKBOX;
	own->isUI = true;
	checkboxText.setText("check", 5, 5, 0.5, { 255,255,255 });	

	ownerTransform = own->GetComponent<ComponentTransform2D>();
	if (!ownerTransform) // If comes from Load not enter
	{
		own->CreateComponent(ComponentType::TRANFORM2D);
		ownerTransform = own->GetComponent<ComponentTransform2D>();
		selectedMaterial = (MaterialComponent*)own->CreateComponent(ComponentType::MATERIAL);
		noSelectedMaterial = (MaterialComponent*)own->CreateComponent(ComponentType::MATERIAL);
		actual = noSelectedMaterial;
	}
	
	app->userInterface->UIGameObjects.push_back(own);
	planeToDraw = new MyPlane(float3{ 0,0,0 }, float3{ 1,1,1 });
	planeToDraw->own = own;
	app->userInterface->OrderButtons();
}

CheckboxComponent::~CheckboxComponent()
{
	RELEASE(planeToDraw);
}

bool CheckboxComponent::Update(float dt)
{
	RG_PROFILING_FUNCTION("Checkbox Update");

	checkboxText.SetOnlyPosition(float2(GetParentPosition().x, GetParentPosition().y));
	isFirstTime = false;
	if (owner->active) {


		
			if (app->userInterface->focusedGameObject == owner)
			{
				state = State::FOCUSED;

				// If mouse button pressed -> Generate event!
				if (app->input->GetMouseButton(SDL_BUTTON_LEFT) == KeyState::KEY_UP)
				{
					if (checked == true) {
						actual = noSelectedMaterial;
						checked = false;
						isFirstTime = true;
					}
					else {
						actual = selectedMaterial;
						checked = true;
						isFirstTime = true;
					}


				}
			}
	}
	else 
	{
		state = State::DISABLED;
	}
	return true;
}

void CheckboxComponent::Draw(CameraComponent* gameCam)
{
	glAlphaFunc(GL_GREATER, 0.5);
	glEnable(GL_ALPHA_TEST);

	planeToDraw->DrawPlane2D(actual->GetTexture().get());

	glDisable(GL_ALPHA_TEST);
	glColor3f(255, 255, 255);
}

void CheckboxComponent::OnEditor()
{
	// General variables
	static float multiplier = 1;
	static float fadeDuration = 0.1f;

	// Manage if colors are being edited or not
	static bool normalEditable = false;
	static bool pressedEditable = false;
	static bool focusedEditable = false;
	static bool disabledEditable = false;
	static bool selectedEditable = false;
	static bool textColorEditable = false;
	if (ImGui::CollapsingHeader("CheckBox"))
	{
		ImGui::Checkbox("Interactable", &active);

		ImGui::Text("Normal Color"); ImGui::SameLine();
		if (ImGui::ColorButton("Normal Color", ImVec4(normalColor.r, normalColor.g, normalColor.b, normalColor.a)))
			normalEditable = !normalEditable;

		ImGui::Text("Pressed Color"); ImGui::SameLine();
		if (ImGui::ColorButton("Pressed Color", ImVec4(pressedColor.r, pressedColor.g, pressedColor.b, pressedColor.a)))
			pressedEditable = !pressedEditable;

		ImGui::Text("Focused Color"); ImGui::SameLine();
		if (ImGui::ColorButton("Focused Color", ImVec4(focusedColor.r, focusedColor.g, focusedColor.b, focusedColor.a)))
			focusedEditable = !focusedEditable;

		ImGui::Text("Disabled Color"); ImGui::SameLine();
		if (ImGui::ColorButton("Disabled Color", ImVec4(disabledColor.r, disabledColor.g, disabledColor.b, disabledColor.a)))
			disabledEditable = !disabledEditable;

		ImGui::Text("Selected Color"); ImGui::SameLine();
		if (ImGui::ColorButton("Selected Color", ImVec4(selectedColor.r, selectedColor.g, selectedColor.b, selectedColor.a)))
			selectedEditable = !selectedEditable;

		ImGui::Separator();

		if (normalEditable)
			ImGui::ColorPicker3("Normal Color", &normalColor);
		if (pressedEditable)
			ImGui::ColorPicker3("Pressed Color", &pressedColor);
		if (focusedEditable)
			ImGui::ColorPicker3("Focused Color", &focusedColor);
		if (disabledEditable)
			ImGui::ColorPicker3("Disabled Color", &disabledColor);
		if (selectedEditable)
			ImGui::ColorPicker3("Selected Color", &selectedColor);
		if (textColorEditable)
			ImGui::ColorPicker3("Text Color", &textColor);

		ImGui::SliderFloat("Alpha", &alpha, 0.5f, 1.0f);
		ImGui::InputFloat("Fade Duration", &fadeDuration);
		ImGui::InputText("Text", text, IM_ARRAYSIZE(text));
		ImGui::DragFloat("Font Size", &checkboxText.Scale, 0.1, 0, 10);
		checkboxText.setOnlyText(text);

		ComponentOptions(this);
		ImGui::Separator();
	}
}

float2 CheckboxComponent::GetParentPosition()
{
	return { ownerTransform->GetPosition().x - (strlen(text) * 12 * checkboxText.Scale) - (ownerTransform->GetScale().x / 4), ownerTransform->GetPosition().y - 5 };
}

bool CheckboxComponent::OnLoad(JsonParsing& node)
{
	std::string selected;
	selected = node.GetJsonString("selected");
	noSelectedMaterial = nullptr;
	selectedMaterial= nullptr;
	for (int a = 0; a < owner->components.size(); a++) {
		if (owner->components[a]->type == ComponentType::MATERIAL) 
		{
			if (selectedMaterial == nullptr) {
				selectedMaterial = (MaterialComponent*)owner->components[a];
			}
			else 
			{
				noSelectedMaterial = (MaterialComponent*)owner->components[a];
				break;
			}
		}
	}
	alpha = node.GetJsonNumber("alpha");
	const char* sel = new char[selected.size()];
	sel = selected.c_str();
	if (sel[0] == 'n')
	{
		actual = noSelectedMaterial;			
		checked = false;
	}
	else
	{
		actual = selectedMaterial;	
		checked = true;
	}
	
	return true;
}


bool CheckboxComponent::OnSave(JsonParsing& node, JSON_Array* array)
{
	JsonParsing file = JsonParsing();

	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Type", (int)type);
	if (actual == noSelectedMaterial) 
		file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), "selected", "noSelected");	
	else 
		file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), "selected", "Selected");
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "alpha", alpha);
	node.SetValueToArray(array, file.GetRootValue());

	return true;
}
