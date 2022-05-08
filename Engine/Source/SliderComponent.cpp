#include "SliderComponent.h"
#include "Application.h"
#include "Globals.h"
#include "ModuleWindow.h"
#include "ModuleInput.h"
#include "ModuleUI.h"
#include "ModuleEditor.h"
#include "ModuleCamera3D.h"

#include "GameObject.h"
#include "CameraComponent.h"
#include "MaterialComponent.h"
#include "Transform2DComponent.h"

#include "GL/glew.h"
#include "GameView.h"

#include "Profiling.h"

SliderComponent::SliderComponent(GameObject* own)
{
	type = ComponentType::UI_SLIDER;
	own->isUI = true;
	sliderText.setText("Slider", 5, 5, 0.5, { 255,255,255 });

	value = 0;
	minValue = 70;
	maxValue = 120;

	state = State::NORMAL;
	barProgres = 0.0f;
	completed = false;
	drawRect = false;

	if (!own->GetComponent<ComponentTransform2D>()) // If comes from Load not enter
	{
		own->CreateComponent(ComponentType::TRANFORM2D);
		
		smallCuad =(ComponentTransform2D*)own->CreateComponent(ComponentType::TRANFORM2D);
		smallCuad->SetButtonWidth(20);
		smallCuad->SetShowEdit(false);
		own->CreateComponent(ComponentType::MATERIAL);
		secondMaterial = (MaterialComponent*)own->CreateComponent(ComponentType::MATERIAL);
	}
	
	app->userInterface->UIGameObjects.push_back(own);
	planeToDraw = new MyPlane(float3{ 0,0,0 }, float3{ 1,1,1 });
	planeToDraw->own = own;
	frontPlaneToDraw = new MyPlane(float3{ 0,0,0 }, float3{ 1,1,1 });
	frontPlaneToDraw->own = own;
	app->userInterface->OrderButtons();
}

SliderComponent::~SliderComponent()
{
	RELEASE(planeToDraw);
	RELEASE(frontPlaneToDraw);
}

bool SliderComponent::Update(float dt)
{
	RG_PROFILING_FUNCTION("Slider Update");
	if (owner->active) {
		int cont = 0;
		ComponentTransform2D* q;
		ComponentTransform2D* r;
		for (int a = 0; a < owner->components.size(); a++) {
			if (owner->components[a]->type == ComponentType::TRANFORM2D)
			{
				cont++;
				if (cont == 1) {
					q = (ComponentTransform2D*)owner->components[a];
				}
				else
				{
					r = (ComponentTransform2D*)owner->components[a];


					break;
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		float2 mPos = float2::zero;
		float4 viewport = float4::zero;
#ifndef DIST
		mPos = { ImGui::GetIO().MousePos.x, ImGui::GetIO().MousePos.y };
		viewport = app->editor->GetGameView()->GetBounds();
#else
		mPos = { (float)app->input->GetMouseX() ,(float)app->input->GetMouseY() };
		viewport = { 0,0, (float)*app->window->GetWindowWidth(), (float)*app->window->GetWindowHeight() };
#endif

		float2 mousePos = { (float)app->input->GetMouseX() ,(float)app->input->GetMouseY() };
		float2 fMousePos = { mPos.x - viewport.x , mPos.y - viewport.y };

		ComponentTransform2D* transform2D = owner->GetComponent<ComponentTransform2D>();
		float posXMin = ((viewport.z / 2) + (transform2D->GetPosition().x)) - (transform2D->GetButtonWidth() / 2);
		float posXMax = ((viewport.z / 2) + (transform2D->GetPosition().x)) + (transform2D->GetButtonWidth() / 2);
		float total = posXMax - posXMin;
		float thePos = total * barProgres;

		r->SetPosition(float3(thePos + q->GetPosition().x - (q->GetButtonWidth() / 2), q->GetPosition().y, r->GetPosition().z));
		r->SetButtonHeight(q->GetButtonHeight()+20);
		r->Update(0);

		if (app->userInterface->focusedGameObject == owner)
		{
			state = State::FOCUSED;

			if (app->input->GetMouseButton(SDL_BUTTON_LEFT) == KeyState::KEY_REPEAT) {
				isHolding = true;
				state = State::PRESSED;
			}
		}
		else state = State::NORMAL;

		if (app->userInterface->UIGameObjectSelected == owner)
		{
			state = State::SELECTED;
		}
		bool enter = false;
		if (isHolding && app->input->GetMouseButton(SDL_BUTTON_LEFT) == KeyState::KEY_REPEAT) 
		{
			enter = true;
		}
		else {
			isHolding = false;
		}
		if (state == State::PRESSED|| enter) {

			if (fMousePos.x > posXMin && fMousePos.x < posXMax)
			{
				thePos = fMousePos.x - posXMin;
				barProgres = thePos / total;

				r->SetPosition(float3(thePos + q->GetPosition().x - (q->GetButtonWidth() / 2), q->GetPosition().y, r->GetPosition().z));

				r->Update(0);
			}
		}
	}
	else 
	{
		state = State::DISABLED;
	}
	return true;

}

void SliderComponent::Draw(CameraComponent* gameCam)
{
	glAlphaFunc(GL_GREATER, 0.5);
	glEnable(GL_ALPHA_TEST);

	

	firstDraw = false;
	planeToDraw->DrawPlane2D(owner->GetComponent<MaterialComponent>()->GetTexture().get());
	firstDraw = true;

	frontPlaneToDraw->DrawPlane2D(secondMaterial->GetTexture().get());

	

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

	if (ImGui::CollapsingHeader("SliderComponent"))
	{
		ImGui::Checkbox("Interactable", &active);

		

		ImGui::Separator();

				
		ImGui::SliderFloat("Alpha", &alpha, 0.5f, 1.0f);
		ImGui::InputFloat("Min Value", &minValue);
		ImGui::InputFloat("Max Value", &maxValue);
		ImGui::SliderFloat("Value", &value, minValue, maxValue);

		ImGui::InputText("Text", text, IM_ARRAYSIZE(text));
		ImGui::DragFloat("Font Size", &sliderText.Scale, 0.1, 0, 10);
		sliderText.setOnlyText(text);

		ComponentOptions(this);
		ImGui::Separator();
	}	
}

float2 SliderComponent::GetParentPosition()
{
	ComponentTransform2D* transform2D = owner->GetComponent<ComponentTransform2D>();
	float3 position = transform2D->GetPosition();
	return { position.x - (strlen(text) * 12 * sliderText.Scale), position.y - 5 };
}

bool SliderComponent::OnLoad(JsonParsing& node)
{
	int contm = 0;
	int contt = 0;
	for (int a = 0; a < owner->components.size(); a++) 
	{
		if (owner->components[a]->type == ComponentType::MATERIAL) 
		{
			if (contm != 0)
			{
				secondMaterial = (MaterialComponent*)owner->components[a];
				
			}
			contm++;
		}
		else if (owner->components[a]->type == ComponentType::TRANFORM2D) {
			if (contt != 0)
			{
				smallCuad =(ComponentTransform2D*)owner->components[a];
				smallCuad->SetButtonWidth(20);
				smallCuad->SetShowEdit(false);
				
			}
			contt++;
		}
	}
	barProgres = node.GetJsonNumber("barProgres");
	alpha = node.GetJsonNumber("alpha");
	return true;
}

bool SliderComponent::OnSave(JsonParsing& node, JSON_Array* array)
{
	JsonParsing file = JsonParsing();

	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Type", (int)type);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "barProgres", barProgres);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "alpha", alpha);
	node.SetValueToArray(array, file.GetRootValue());

	return true;
}
