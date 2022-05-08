#include "ModelImporter.h"
#include "Application.h"
#include "Globals.h"

#include "ModuleSceneManager.h"
#include "Scene.h"
#include "FileSystem.h"

#include "TransformComponent.h"
#include "MeshComponent.h"
#include "MaterialComponent.h"

#include "MeshImporter.h"
#include "TextureImporter.h"
#include "AnimationImporter.h"

#include "Model.h"
#include "ResourceManager.h"

#include "assimp/Importer.hpp"
#include "assimp/postProcess.h"
#include "assimp/scene.h"

#include <string>
#include "Profiling.h"

void ModelImporter::ReImport(std::string& assetsPath, std::string& library, ModelParameters& parameters)
{
	RG_PROFILING_FUNCTION("Reimporting Model");

	std::string p = assetsPath;

	Assimp::Importer import;
	unsigned int flags = aiProcess_CalcTangentSpace | aiProcess_FindInstances | aiProcess_ValidateDataStructure | aiProcess_SortByPType | aiProcess_ImproveCacheLocality | aiProcess_JoinIdenticalVertices | aiProcess_LimitBoneWeights | aiProcess_FindInvalidData | aiProcess_FindDegenerates;
	if (parameters.flippedUvs) flags |= aiProcess_FlipUVs;
	if (parameters.optimizedMesh) flags |= aiProcess_OptimizeMeshes;
	if (parameters.debone) flags |= aiProcess_Debone;
	if (parameters.genSmoothNormals) flags |= aiProcess_GenSmoothNormals;
	if (parameters.genUVCoords) flags |= aiProcess_GenUVCoords;
	if (parameters.optimizeGraph) flags |= aiProcess_OptimizeGraph;
	if (parameters.removeRedundantMaterials) flags |= aiProcess_RemoveRedundantMaterials;
	if (parameters.splitLargeMeshes) flags |= aiProcess_SplitLargeMeshes;

	const aiScene* scene = import.ReadFile(assetsPath, flags);

	if (!scene || scene->mFlags & AI_SCENE_FLAGS_INCOMPLETE || !scene->mRootNode)
	{
		DEBUG_LOG("ERROR ASSIMP %s", import.GetErrorString());
		return;
	}

	app->fs->GetFilenameWithoutExtension(p);

	JsonParsing json = JsonParsing();
	JsonParsing child = json.SetChild(json.GetRootValue(), "Model");
	child.SetNewJsonString(child.ValueToObject(child.GetRootValue()), "Name", p.c_str());
	std::string root = "Childs" + p;
	JSON_Array* array = child.SetNewJsonArray(child.GetRootValue(), root.c_str());

	ReProcessNode(scene->mRootNode, scene, child, array, assetsPath, parameters);

	char* buffer = nullptr;
	size_t size = json.Save(&buffer);

	app->fs->Save(library.c_str(), buffer, size);

	RELEASE_ARRAY(buffer);
}

void ModelImporter::ImportModel(std::string& path)
{
	RG_PROFILING_FUNCTION("Importing Model");

	std::string p = path;

	if (ResourceManager::GetInstance()->CheckResource(path))
	{
		return;
	}
	else
	{
		Assimp::Importer import;
		const aiScene* scene = import.ReadFile(path, aiProcess_Triangulate | aiProcess_FlipUVs | aiProcessPreset_TargetRealtime_MaxQuality);

		if (!scene || scene->mFlags & AI_SCENE_FLAGS_INCOMPLETE || !scene->mRootNode)
		{
			DEBUG_LOG("ERROR ASSIMP %s", import.GetErrorString());
			return;
		}

		app->fs->GetFilenameWithoutExtension(p);

		JsonParsing json = JsonParsing();
		JsonParsing child = json.SetChild(json.GetRootValue(), "Model");
		child.SetNewJsonString(child.ValueToObject(child.GetRootValue()), "Name", p.c_str());
		std::string root = "Childs" + p;
		JSON_Array* array = child.SetNewJsonArray(child.GetRootValue(), root.c_str());

		uint uid = ResourceManager::GetInstance()->CreateResource(ResourceType::MODEL, path, p);

		std::shared_ptr<Model> model = std::static_pointer_cast<Model>(ResourceManager::GetInstance()->GetResource(uid));
		std::vector<uint> uids;
		std::map<std::string, BoneInfo> bones;
		ProcessNode(scene->mRootNode, scene, child, array, path, uids, bones);
		if (scene->HasAnimations())	AnimationImporter::ImportAnimations2(path, scene, json, uids, bones);

		model->SetMeshes(uids);
		SaveModel(p, json);
	}
}

void ModelImporter::SaveModel(std::string& path, JsonParsing& json)
{
	char* buffer = nullptr;
	size_t size = json.Save(&buffer);

	app->fs->Save(path.c_str(), buffer, size);

	RELEASE_ARRAY(buffer);
}

void ModelImporter::LoadModel(std::string& path)
{
	RG_PROFILING_FUNCTION("Loading Model");
	char* buffer = nullptr;

	app->fs->Load(path.c_str(), &buffer);

	if (buffer != nullptr)
	{
		JsonParsing json = JsonParsing(buffer);

		json = json.GetChild(json.GetRootValue(), "Model");

		//GameObject* go = new GameObject();
		std::string name = json.GetJsonString("Name");
		//go->SetName(name.c_str());

		name = "Childs" + name;
		CreatingModel(json, json.GetJsonArray(json.ValueToObject(json.GetRootValue()), name.c_str()), app->sceneManager->GetCurrentScene()->GetRoot());
	}

	RELEASE_ARRAY(buffer);
}

void ModelImporter::ProcessNode(aiNode* node, const aiScene* scene, JsonParsing& nodeJ, JSON_Array* json, std::string& path, std::vector<uint>& uids, std::map<std::string, BoneInfo>& bones)
{
	if (node == scene->mRootNode || node->mNumMeshes > 0)
	{
		JsonParsing jsonValue = JsonParsing();
		
		jsonValue.SetNewJsonString(jsonValue.ValueToObject(jsonValue.GetRootValue()), "Name", node->mName.C_Str());

		aiVector3D pos;
		aiQuaternion quat;
		aiVector3D sca;
		node->mTransformation.Decompose(sca, quat, pos);
		float3 position(pos.x, pos.y, pos.z);
		Quat quaternion(quat.x, quat.y, quat.z, quat.w);
		float3 scale(sca.x, sca.y, sca.z);

		jsonValue.SetNewJson3Number(jsonValue, "Position", position);
		jsonValue.SetNewJson4Number(jsonValue, "Rotation", quaternion);
		jsonValue.SetNewJson3Number(jsonValue, "Scale", scale);

		for (unsigned int i = 0; i < node->mNumMeshes; ++i)
		{
			aiMesh* mesh = scene->mMeshes[node->mMeshes[i]];
			MeshImporter::ImportMesh(mesh, scene, jsonValue, path, uids, bones);
		}

		std::string name = "Childs" + std::string(node->mName.C_Str());
		JSON_Array* array = jsonValue.SetNewJsonArray(jsonValue.GetRootValue(), name.c_str());
		// Repeat the process until there's no more children
		for (unsigned int i = 0; i < node->mNumChildren; ++i)
		{
			ProcessNode(node->mChildren[i], scene, jsonValue, array, path, uids, bones);
		}
		nodeJ.SetValueToArray(json, jsonValue.GetRootValue());
	}
	else
	{
		for (unsigned int i = 0; i < node->mNumChildren; ++i)
		{
			ProcessNode(node->mChildren[i], scene, nodeJ, json, path, uids, bones);
		}
	}
}

void ModelImporter::ReProcessNode(aiNode* node, const aiScene* scene, JsonParsing& nodeJ, JSON_Array* json, std::string& path, ModelParameters& data)
{
	if (node == scene->mRootNode || node->mNumMeshes > 0)
	{
		JsonParsing jsonValue = JsonParsing();
		jsonValue.SetNewJsonString(jsonValue.ValueToObject(jsonValue.GetRootValue()), "Name", node->mName.C_Str());

		aiVector3D pos;
		aiQuaternion quat;
		aiVector3D sca;
		node->mTransformation.Decompose(sca, quat, pos);
		float3 position(pos.x, pos.y, pos.z);
		Quat quaternion(quat.x, quat.y, quat.z, quat.w);
		float3 scale(sca.x, sca.y, sca.z);

		jsonValue.SetNewJson3Number(jsonValue, "Position", position);
		jsonValue.SetNewJson4Number(jsonValue, "Rotation", quaternion);
		jsonValue.SetNewJson3Number(jsonValue, "Scale", scale);

		for (unsigned int i = 0; i < node->mNumMeshes; ++i)
		{
			aiMesh* mesh = scene->mMeshes[node->mMeshes[i]];
			std::string assetsMesh = path;
			std::string meshName = "__";
			meshName += +mesh->mName.C_Str();
			assetsMesh.insert(assetsMesh.find_last_of("."), meshName);
			std::string library = ResourceManager::GetInstance()->GetResource(assetsMesh)->GetLibraryPath();
			MeshImporter::ReImportMesh(mesh, scene, jsonValue, library, path, data);
		}

		std::string name = "Childs" + std::string(node->mName.C_Str());
		JSON_Array* array = jsonValue.SetNewJsonArray(jsonValue.GetRootValue(), name.c_str());
		// Repeat the process until there's no more children
		for (unsigned int i = 0; i < node->mNumChildren; ++i)
		{
			ReProcessNode(node->mChildren[i], scene, jsonValue, array, path, data);
		}
		nodeJ.SetValueToArray(json, jsonValue.GetRootValue());
	}
	else
	{
		for (unsigned int i = 0; i < node->mNumChildren; ++i)
		{
			ReProcessNode(node->mChildren[i], scene, nodeJ, json, path, data);
		}
	}
}

void ModelImporter::CreatingModel(JsonParsing& json, JSON_Array* array, GameObject* go)
{
	size_t size = json.GetJsonArrayCount(array);
	for (int i = 0; i < size; ++i)
	{
		GameObject* newGo = new GameObject();
		TransformComponent* transform = (TransformComponent*)newGo->CreateComponent(ComponentType::TRANSFORM);
		newGo->SetParent(go);
		go->AddChild(newGo);

		JsonParsing parsing = json.GetJsonArrayValue(array, i);
		std::string name = parsing.GetJsonString("Name");
		
		float4 quat = parsing.GetJson4Number(parsing, "Rotation");
		Quat rot(quat.x, quat.y, quat.z, quat.w);
		float3 pos = parsing.GetJson3Number(parsing, "Position");
		float3 scale = parsing.GetJson3Number(parsing, "Scale");
		
		transform->SetTransform(pos, rot, scale);

		newGo->SetName(name.c_str());
		JSON_Array* arr = parsing.GetJsonArray(parsing.ValueToObject(parsing.GetRootValue()), "Components");
		size_t s = parsing.GetJsonArrayCount(arr);

		for (int j = 0; j < s; ++j)
		{
			JsonParsing component = parsing.GetJsonArrayValue(arr, j);
			switch ((ComponentType)(int)component.GetJsonNumber("Type"))
			{
			case ComponentType::MESH_RENDERER:
			{
				MeshComponent* mesh = (MeshComponent*)newGo->CreateComponent(ComponentType::MESH_RENDERER);
				//LoadMesh(component.GetJsonString("Mesh Path"), mesh);
				std::string path = component.GetJsonString("Mesh Path");
				app->fs->GetFilenameWithoutExtension(path);
				path = path.substr(path.find_last_of("_") + 1, path.length());
				mesh->SetMesh(ResourceManager::GetInstance()->LoadResource(std::stoll(path)));
				break;
			}
			case ComponentType::MATERIAL:
			{
				MaterialComponent* material = (MaterialComponent*)newGo->CreateComponent(ComponentType::MATERIAL);
				std::string path = component.GetJsonString("Texture Path");
				app->fs->GetFilenameWithoutExtension(path);
				path = path.substr(path.find_last_of("_") + 1, path.length());
				material->SetTexture(ResourceManager::GetInstance()->LoadResource(std::stoll(path)));
				break;
			}
			}
		}

		name = "Childs" + name;
		CreatingModel(parsing, parsing.GetJsonArray(parsing.ValueToObject(parsing.GetRootValue()), name.c_str()), newGo);
	}
}

void ModelImporter::CreateMetaModel(std::string& path, ModelParameters& data, std::string& assets, uint uid)
{
	JsonParsing metaModel;

	metaModel.SetNewJsonBool(metaModel.ValueToObject(metaModel.GetRootValue()), "Debone", data.debone);
	metaModel.SetNewJsonBool(metaModel.ValueToObject(metaModel.GetRootValue()), "FlippedUvs", data.flippedUvs);
	metaModel.SetNewJsonBool(metaModel.ValueToObject(metaModel.GetRootValue()), "GenSmoothNormals", data.genSmoothNormals);
	metaModel.SetNewJsonBool(metaModel.ValueToObject(metaModel.GetRootValue()), "GenUVCoords", data.genUVCoords);
	metaModel.SetNewJsonBool(metaModel.ValueToObject(metaModel.GetRootValue()), "HasNormals", data.normals);
	metaModel.SetNewJsonBool(metaModel.ValueToObject(metaModel.GetRootValue()), "OptimizedMesh", data.optimizedMesh);
	metaModel.SetNewJsonBool(metaModel.ValueToObject(metaModel.GetRootValue()), "OptimizeGraph", data.optimizeGraph);
	metaModel.SetNewJsonBool(metaModel.ValueToObject(metaModel.GetRootValue()), "RemoveRedundantMaterials", data.removeRedundantMaterials);
	metaModel.SetNewJsonBool(metaModel.ValueToObject(metaModel.GetRootValue()), "SplitLargeMeshes", data.splitLargeMeshes);

	metaModel.SetNewJsonNumber(metaModel.ValueToObject(metaModel.GetRootValue()), "Uuid", uid);
	metaModel.SetNewJsonString(metaModel.ValueToObject(metaModel.GetRootValue()), "Assets Path", assets.c_str());

	char* buffer = nullptr;
	size_t size = metaModel.Save(&buffer);

	app->fs->Save(path.c_str(), buffer, size);

	RELEASE_ARRAY(buffer);
}