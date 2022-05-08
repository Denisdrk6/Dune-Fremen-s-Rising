#include "ImageComponent.h"
#include "Application.h"
#include "Globals.h"

#include "ModuleInput.h"
#include "ModuleUI.h"
#include "Texture.h"
#include "GameObject.h"
#include "MaterialComponent.h"
#include "Transform2DComponent.h"
#include"ResourceManager.h"
#include "GL/glew.h"
void UIAnimation::Update(float dt) 
{
	if (!isStatic) {
		currentDt += dt;
		for (int a = 0; a < images.size(); a++) {

			if (animatonState == a && currentDt >= timeBetwen) {
				currentDt = 0;
				if (images.size() == a + 1) {
					if (loop)
						animatonState = 0;
					else
						animatonState = -1;
				}
				else {
					animatonState++;
				}
			}
		}
		if (animatonState == -1) {
			StopAnim();
		}
	}
}
void UIAnimation::SetStaticimage(int image) {

	animatonState= image;
	isPlayng = true;
	isStatic = true;
}
MaterialComponent* UIAnimation::Draw() 
{
	return images[animatonState];
}
void UIAnimation::StartAnim()
{
	isPlayng = true;
	animatonState = 0;
	currentDt = 0;
}
void UIAnimation::StopAnim() {
	isPlayng = false;
	animatonState = -1;
	currentDt = 0;
}
void UIAnimation::ChageVelocity(float velocity) {
	timeBetwen = velocity;
}
// IMAGE
ImageComponent::ImageComponent(GameObject* own)
{
	type = ComponentType::UI_IMAGE;
	own->isUI = true;
	this->text = "Image Component";

	if (!own->GetComponent<ComponentTransform2D>()) // If comes from Load not enter
	{
		own->CreateComponent(ComponentType::TRANFORM2D);
		principal=(MaterialComponent*)own->CreateComponent(ComponentType::MATERIAL);
	}

	app->userInterface->UIGameObjects.push_back(own);
	planeToDraw = new MyPlane(float3{ 0,0,0 }, float3{ 1,1,1 });
	planeToDraw->own = own;
	app->userInterface->OrderButtons();
}

ImageComponent::~ImageComponent()
{
	text.clear();
	RELEASE(planeToDraw);
}

bool ImageComponent::Update(float dt)
{
	for (int a = 0;a < animations.size();a++) {
		if(animations[a]->isPlayng)
			animations[a]->Update(dt);
	}
	return true;
}
void ImageComponent::SetActualColor(float Red, float Green, float Blue)
{
	actualColor.r = Red / 255;
	actualColor.g = Green / 255;
	actualColor.b = Blue / 255;
}
void ImageComponent::Draw(CameraComponent* gameCam)
{
	glAlphaFunc(GL_GREATER, 0.5);
	glEnable(GL_ALPHA_TEST);
	MaterialComponent* auxiliar;
	auxiliar = principal;
	for (int a = 0;a < animations.size();a++) {
		if (animations[a]->isPlayng) {
			auxiliar =animations[a]->Draw();
		}
			
	}
	glColor4f(actualColor.r, actualColor.g, actualColor.b, actualColor.a);
	planeToDraw->DrawPlane2D(auxiliar->GetTexture().get());

	glDisable(GL_ALPHA_TEST);
	glColor3f(255, 255, 255);
}
int ImageComponent::LoadTexture(std::string newTexture) 
{
	MaterialComponent* auxiliar = (MaterialComponent*)owner->CreateComponent(ComponentType::MATERIAL);
	ResourceManager* aux =ResourceManager::GetInstance();
	auxiliar->SetTexture(aux->LoadResource(std::string(newTexture)));
	materialList.push_back(auxiliar);

	
	return materialList.size();
}
void ImageComponent::UseTexture(int ID) 
{
	principal = materialList[ID-1];
}
void ImageComponent::OnEditor()
{
	if (ImGui::CollapsingHeader("ImageComponent"))
	{
		ImGui::SliderFloat("Alpha", &alpha, 0.5f, 1.0f);
		if (ImGui::Button("create animation")) {
			UIAnimation* aux = new UIAnimation();
			animations.push_back(aux);
		}
		for (int b = 0;b < animations.size();b++) {
			ImGui::Checkbox("isLoop", &animations[b]->loop);
			ImGui::InputFloat("time Betwen (seconds)", &animations[b]->timeBetwen);
			if (ImGui::Button("set image")) {
				MaterialComponent* matAux = (MaterialComponent*)owner->CreateComponent(ComponentType::MATERIAL);
				animations[b]->images.push_back(matAux);
			}
			for (int c = 0;c < animations[b]->images.size();c++) {
				animations[b]->images[c]->OnEditor();
			}
		}
		ComponentOptions(this);
		ImGui::Separator();
	}
}
bool ImageComponent::OnLoad(JsonParsing& node)
{
	alpha = node.GetJsonNumber("alpha");
	
	for (int a = 0; a < owner->components.size(); a++) {
		if (owner->components[a]->type == ComponentType::MATERIAL) 
		{
			principal = (MaterialComponent*)owner->components[a];
			break;
		}
		
	}
	

	int num=node.GetJsonNumber("numOfAnimations");

	std::string name;
	std::string number;
	std::string texture;
	for (int a = 0; a < num; a++) {
		UIAnimation* aux = new UIAnimation();
		animations.push_back(aux);
		number = std::to_string(a);


		name = "loop";
		name = name + number;
		aux->loop = node.GetJsonBool(name.c_str());
		
		name = "timeBetwen";
		name = name + number;
		aux->timeBetwen=node.GetJsonNumber(name.c_str());

		name = "numOfImages";
		name = name + number;
		int Images = node.GetJsonNumber(name.c_str());
		for (int b = 0; b < Images; b++)
		{
			name = "text";
			number = std::to_string(a);
			number = number + std::to_string(b);
			name = name + number;

			//MaterialComponent* matAux = (MaterialComponent*)owner->CreateComponent(ComponentType::MATERIAL);
			MaterialComponent* matAux = new MaterialComponent(owner, false);
			texture=node.GetJsonString(name.c_str());
			matAux->SetTexture(ResourceManager::GetInstance()->LoadResource(texture));
			animations[a]->images.push_back(matAux);
		}
	}
	return true;
}

bool ImageComponent::OnSave(JsonParsing& node, JSON_Array* array)
{
	JsonParsing file = JsonParsing();

	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Type", (int)type);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "alpha", alpha);

	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "numOfAnimations", animations.size());
	
	std::string name;
	std::string number;
	for (int a = 0; a < animations.size(); a++) 
	{
		number = std::to_string(a);
		name = "numOfImages";
		name = name + number;
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), name.c_str(), animations[a]->images.size());

		name = "loop";
		name = name + number;
		file.SetNewJsonBool(file.ValueToObject(file.GetRootValue()), name.c_str(), animations[a]->loop);

		name = "timeBetwen";
		name = name + number;
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), name.c_str(), animations[a]->timeBetwen);
		for (int b = 0; b < animations[a]->images.size(); b++) 
		{
			number = std::to_string(a);
			name = "text";			
			number = number + std::to_string(b);

			name = name + number;
			std::shared_ptr<Texture> diff = nullptr;
			diff = animations[a]->images[b]->GetTexture();
			file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), name.c_str(), diff->GetAssetsPath().c_str());
		}
	}

	node.SetValueToArray(array, file.GetRootValue());

	return true;
}
