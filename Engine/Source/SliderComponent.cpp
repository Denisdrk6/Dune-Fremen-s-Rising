#include "Application.h"
#include "SDL.h"
#include "ModuleInput.h"
#include "SliderComponent.h"
#include "ModuleCamera3D.h"
#include "CameraComponent.h"
#include "MaterialComponent.h"
#include "glew/include/GL/glew.h"
#include "ModuleUI.h"
#include "ModuleEditor.h"
#include "GameView.h"
SliderComponent::SliderComponent(GameObject* own)
{
	//name = "Slider Component";
	type = ComponentType::UI_SLIDER;
	value = 0;
	minValue = 70;
	maxValue = 120;
	drawRect = false;
	state = State::NORMAL;
	barProgres = 0.0f;
	completed = false;
	owner = own;
	actualColor = normalColor;
	//thePlane = App->editor->planes[App->editor->planes.size() - 1];
	sliderText.setText("Slider", 5, 5, 0.5, { 255,255,255 });
}

SliderComponent::~SliderComponent()
{
	
}

bool SliderComponent::Update(float dt)
{
	/*sliderText.SetOnlyPosition(float2(GetParentPosition().x, GetParentPosition().y));*/

	

	if (!active)
		state = State::DISABLED;
	else
		state = State::NORMAL;

	if (state != State::DISABLED)
	{

		if (app->userInterface->focusedGameObject == owner)
		{
			state = State::FOCUSED;

			if (state != State::FOCUSED && state != State::PRESSED)
			{
			}

			if (app->input->GetMouseButton(SDL_BUTTON_LEFT) == KeyState::KEY_REPEAT)
			{
				state = State::PRESSED;
			}

			// If mouse button pressed -> Generate event!
			if (app->input->GetMouseButton(SDL_BUTTON_LEFT) == KeyState::KEY_UP)
			{

			}
		}
		else state = State::NORMAL;

		if (app->userInterface->UIGameObjectSelected == owner)
		{
			state = State::SELECTED;
			if (app->input->GetKey(SDL_SCANCODE_RETURN) == KeyState::KEY_DOWN)
			{

			}

		}
	}

	//barProgres += 0.001;
	if (state == State::PRESSED) {
		float2 mousePos = { (float)app->input->GetMouseX() ,(float)app->input->GetMouseY() };
		float2 mPos = { ImGui::GetIO().MousePos.x, ImGui::GetIO().MousePos.y };
		float4 viewport = app->editor->gameView->bounds;
		float2 fMousePos = { mPos.x - viewport.x , mPos.y - viewport.y };


		ComponentTransform2D* transform2D = owner->GetComponent<ComponentTransform2D>();
		float posXMin = ((viewport.z / 2) + (transform2D->position.x)) - (transform2D->buttonWidth / 2);
		float posXMax = ((viewport.z / 2) + (transform2D->position.x)) + (transform2D->buttonWidth / 2);

		if (fMousePos.x > posXMin && fMousePos.x < posXMax)
		{
			float thePos = fMousePos.x - posXMin;
			float total = posXMax - posXMin;


			barProgres = thePos / total;

			if (state == State::PRESSED)
			{
				
			}


			if (barProgres < 0.5f)
			{
				/*thePlane->texCoords[0] = 1;
				thePlane->texCoords[6] = 1;*/
				thePlane->texCoords[0] = (0.5 - barProgres);
				thePlane->texCoords[4] = (0.5 - barProgres);
			}
			else if (barProgres >= 0.5f) {

				float aux = barProgres - 0.5;
				thePlane->texCoords[2] = (1 - aux);
				thePlane->texCoords[5] = (1 - aux);
			}
			glDeleteBuffers(thePlane->texCoords.size() * sizeof(GLfloat), &thePlane->TBO);

			glGenBuffers(1, &thePlane->TBO);
			glBindBuffer(GL_ARRAY_BUFFER, thePlane->TBO);
			glBufferData(GL_ARRAY_BUFFER, thePlane->texCoords.size() * sizeof(GLfloat), thePlane->texCoords.data(), GL_STATIC_DRAW);

			glBindBuffer(GL_ARRAY_BUFFER, 0);
		}
	}
	return true;

}

void SliderComponent::Draw(CameraComponent* gameCam)
{
	/*MyPlane* planeToDraw = nullptr;
	int auxId = owner->id;

	for (int i = 0; i < app->editor->planes.size(); i++)
		if (App->editor->planes[i]->id == auxId) planeToDraw = App->editor->planes[i];*/

	glAlphaFunc(GL_GREATER, 0.5);
	glEnable(GL_ALPHA_TEST);

	switch (state)
	{
	case State::DISABLED:
		glColor4f(disabledColor.r, disabledColor.g, disabledColor.b, disabledColor.a);
		actualColor = disabledColor;
		break;
	case State::NORMAL:
		glColor4f(normalColor.r, normalColor.g, normalColor.b, normalColor.a);
		actualColor = disabledColor;
		break;
	case State::FOCUSED:
		glColor4f(focusedColor.r, focusedColor.g, focusedColor.b, focusedColor.a);
		actualColor = disabledColor;
		break;
	case State::PRESSED:
		glColor4f(pressedColor.r, pressedColor.g, pressedColor.b, pressedColor.a);
		actualColor = disabledColor;
		break;
	case State::SELECTED:
		glColor4f(selectedColor.r, selectedColor.g, selectedColor.b, selectedColor.a);
		actualColor = disabledColor;
		break;
	default:
		break;
	}

	MaterialComponent* mat = owner->GetComponent<MaterialComponent>();
	thePlane->DrawPlane2D(mat->GetTexture().get());

	glDisable(GL_ALPHA_TEST);
	glColor3f(255, 255, 255);
}

void SliderComponent::OnEditor()
{
	// Colors
	static bool normalEditable = false;
	static bool pressedEditable = false;
	static bool focusedEditable = false;
	static bool disabledEditable = false;
	static bool selectedEditable = false;
	static bool textColorEditable = false;

	if (ImGui::CollapsingHeader("Slider"))
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

		ImGui::Text("Text Color"); ImGui::SameLine();
		if (ImGui::ColorButton("Text Color", ImVec4(textColor.r, textColor.g, textColor.b, textColor.a)))
			textColorEditable = !textColorEditable;

		sliderText.setOnlyColor({ textColor.r, textColor.g, textColor.b });

		if (normalEditable)
		{
			ImGui::ColorPicker3("Normal Color", &normalColor);
		}
		if (pressedEditable)
		{
			ImGui::ColorPicker3("Pressed Color", &pressedColor);
		}
		if (focusedEditable)
		{
			ImGui::ColorPicker3("Focused Color", &focusedColor);
		}
		if (disabledEditable)
		{
			ImGui::ColorPicker3("Disabled Color", &disabledColor);
		}
		if (selectedEditable)
		{
			ImGui::ColorPicker3("Selected Color", &selectedColor);
		}
		if (textColorEditable)
		{
			ImGui::ColorPicker3("Text Color", &textColor);
		}

		ImGui::InputFloat("Min Value", &minValue);
		ImGui::InputFloat("Max Value", &maxValue);
		ImGui::SliderFloat("Value", &value, minValue, maxValue);

		ImGui::InputText("Text", text, IM_ARRAYSIZE(text));
		ImGui::DragFloat("Font Size", &sliderText.Scale, 0.1, 0, 10);
		sliderText.setOnlyText(text);

	}
	
}

float2 SliderComponent::GetParentPosition()
{
	ComponentTransform2D* transform2D = gen->GetComponent<ComponentTransform2D>();
	float3 position = transform2D->position;
	return { position.x - (strlen(text) * 12 * sliderText.Scale), position.y - 5 };

	
}