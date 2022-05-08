#include "MeshImporter.h"
#include "Application.h"
#include "Globals.h"

#include "FileSystem.h"
#include "ResourceManager.h"
#include "TextureImporter.h"
#include "AnimationImporter.h"

#include "Component.h"

#include "Mesh.h"
#include "Model.h"

#include "Profiling.h"

void MeshImporter::ReImportMesh(const aiMesh* mesh, const aiScene* scene, JsonParsing& json, std::string& library, std::string& path, ModelParameters& data)
{
	RG_PROFILING_FUNCTION("Importing mesh");

	std::vector<Vertex> vertices;
	std::vector<unsigned int> indices;
	//std::vector<float3> norms;
	//std::vector<float2> texCoords;

	unsigned int numBones = mesh->mNumBones;
	std::vector<unsigned int> bonesUid;

	int numVertices = mesh->mNumVertices;
	int numFaces = mesh->mNumFaces;

	//vertices.reserve(numVertices);
	indices.reserve(numFaces * 3);
	//texCoords.reserve(numVertices);

	for (unsigned int i = 0; i < numVertices; ++i)
	{
		Vertex vertex;
		vertex.position.x = mesh->mVertices[i].x;
		vertex.position.y = mesh->mVertices[i].y;
		vertex.position.z = mesh->mVertices[i].z;

		//float3 normals;
		if (data.normals && mesh->HasNormals())
		{
			vertex.normal.x = mesh->mNormals[i].x;
			vertex.normal.y = mesh->mNormals[i].y;
			vertex.normal.z = mesh->mNormals[i].z;
			//norms.push_back(normals);
		}

		//float2 coords;
		if (mesh->mTextureCoords[0])
		{
			vertex.texCoords.x = mesh->mTextureCoords[0][i].x;
			vertex.texCoords.y = mesh->mTextureCoords[0][i].y;
		}

		vertices.push_back(vertex);
		//texCoords.push_back(coords);
	}

	for (unsigned int i = 0; i < numFaces; ++i)
	{
		aiFace face = mesh->mFaces[i];
		for (unsigned int j = 0; j < face.mNumIndices; ++j)
		{
			indices.push_back(face.mIndices[j]);
		}
	}

	for (unsigned int i = 0; i < numBones; i++)
	{
		AnimationImporter::ReImportBones(path, mesh->mBones[i], json, library, bonesUid);
	}

	// TODO: Check the re-import when animation is completed
	//SaveMesh(library, vertices, indices/*, norms, texCoords*/, bonesUid);

	JSON_Array* array = json.SetNewJsonArray(json.GetRootValue(), "Components");
	JsonParsing parse = JsonParsing();
	parse.SetNewJsonNumber(parse.ValueToObject(parse.GetRootValue()), "Type", (int)ComponentType::MESH_RENDERER);
	parse.SetNewJsonString(parse.ValueToObject(parse.GetRootValue()), "Mesh Path", library.c_str());

	json.SetValueToArray(array, parse.GetRootValue());

	if (mesh->mMaterialIndex >= 0)
	{
		DEBUG_LOG("Processing material...");

		JsonParsing mat = JsonParsing();
		aiMaterial* material = scene->mMaterials[mesh->mMaterialIndex];
		TextureImporter::ImportTexture(material, aiTextureType_DIFFUSE, "texture_diffuse", mat, path);
		DEBUG_LOG("Material loading completed!");
		json.SetValueToArray(array, mat.GetRootValue());
	}
}

void MeshImporter::ImportMesh(const aiMesh* mesh, const aiScene* scene, JsonParsing& json, std::string& path, std::vector<uint>& uids, std::map<std::string, BoneInfo>& bones)
{
	RG_PROFILING_FUNCTION("Importing mesh");

	std::vector<Vertex> vertices;
	std::vector<unsigned int> indices;
	unsigned int numBones = mesh->mNumBones;
	std::vector<unsigned int> bonesUid;

	int numVertices = mesh->mNumVertices;
	int numFaces = mesh->mNumFaces;

	for (unsigned int i = 0; i < numVertices; ++i)
	{
		Vertex vertex;
		vertex.position.x = mesh->mVertices[i].x;
		vertex.position.y = mesh->mVertices[i].y;
		vertex.position.z = mesh->mVertices[i].z;

		for (int i = 0; i < 4; ++i)
		{
			vertex.boneIDs[i] = -1;
			vertex.weights[i] = 0.0f;
		}

		if (mesh->HasNormals())
		{
			vertex.normal.x = mesh->mNormals[i].x;
			vertex.normal.y = mesh->mNormals[i].y;
			vertex.normal.z = mesh->mNormals[i].z;
		}

		if (mesh->mTextureCoords[0])
		{
			vertex.texCoords.x = mesh->mTextureCoords[0][i].x;
			vertex.texCoords.y = mesh->mTextureCoords[0][i].y;
		}

		if (mesh->HasTangentsAndBitangents())
		{
			vertex.tangents.x = mesh->mTangents[i].x;
			vertex.tangents.y = mesh->mTangents[i].y;
			vertex.tangents.z = mesh->mTangents[i].z;

			vertex.biTangents.x = mesh->mBitangents[i].x;
			vertex.biTangents.y = mesh->mBitangents[i].y;
			vertex.biTangents.z = mesh->mBitangents[i].z;
		}

		vertices.push_back(vertex);
	}

	for (unsigned int i = 0; i < numFaces; ++i)
	{
		aiFace face = mesh->mFaces[i];
		for (unsigned int j = 0; j < face.mNumIndices; ++j)
		{
			indices.push_back(face.mIndices[j]);
		}
	}

	ExtractBonesAndWeights(vertices, mesh, scene, bones);
	
	if (!mesh->HasTangentsAndBitangents())
		ComputeTangents(vertices, indices.size());

	std::string meshName;
	std::string assetsPath(path);
	std::string name("__");
	name += mesh->mName.C_Str();

	assetsPath.insert(assetsPath.find_last_of("."), name.c_str());

	uint uid = ResourceManager::GetInstance()->CreateResource(ResourceType::MESH, assetsPath, meshName);
	uids.push_back(uid);
	
	SaveMesh(meshName, vertices, indices/*, norms, texCoords*/, bones);

	JSON_Array* array = json.SetNewJsonArray(json.GetRootValue(), "Components");
	JsonParsing parse = JsonParsing();
	parse.SetNewJsonNumber(parse.ValueToObject(parse.GetRootValue()), "Type", (int)ComponentType::MESH_RENDERER);
	parse.SetNewJsonString(parse.ValueToObject(parse.GetRootValue()), "Mesh Path", meshName.c_str());

	json.SetValueToArray(array, parse.GetRootValue());

	if (mesh->mMaterialIndex >= 0)
	{
		DEBUG_LOG("Processing material...");

		JsonParsing mat = JsonParsing();
		aiMaterial* material = scene->mMaterials[mesh->mMaterialIndex];
		TextureImporter::ImportTexture(material, aiTextureType_DIFFUSE, "texture_diffuse", mat, path);
		DEBUG_LOG("Material loading completed!");
		json.SetValueToArray(array, mat.GetRootValue());
	}
}

void MeshImporter::SaveMesh(std::string& name, std::vector<Vertex>& vertices, std::vector<unsigned int>& indices, std::map<std::string, BoneInfo>& bones)
{
	std::string json = name;

	json = json.substr(0, json.find_last_of('.'));
	json += ".json";

	unsigned int ranges[4] = { vertices.size(), indices.size(), bones.size(), json.size() };

	uint size = sizeof(ranges) + sizeof(Vertex) * vertices.size() + sizeof(unsigned int) * indices.size() + json.size();
	char* fileBuffer = new char[size];
	char* cursor = fileBuffer;

	unsigned int bytes = sizeof(ranges);
	memcpy(cursor, ranges, bytes);
	cursor += bytes;

	bytes = sizeof(Vertex) * vertices.size();
	memcpy(cursor, vertices.data(), bytes);
	cursor += bytes;

	bytes = sizeof(unsigned int) * indices.size();
	memcpy(cursor, indices.data(), bytes);
	cursor += bytes;

	bytes = json.size();
	memcpy(cursor, json.data(), bytes);
	cursor += bytes;

	if (ranges[2] > 0)
	{
		JsonParsing file = JsonParsing();

		int i = 0;
		for (std::map<std::string, BoneInfo>::iterator it = bones.begin(); it != bones.end(); ++i, ++it)
		{
			std::string name = (*it).first;
			BoneInfo info = (*it).second;

			std::string index = std::to_string(i);
			std::string aux;
			aux = "Name" + index;
			file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), aux.c_str(), name.c_str());

			aux = "Id" + index;
			file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), aux.c_str(), info.id);

			float3 position;
			Quat rotation;
			float3 scale;
			info.offset.Decompose(position, rotation, scale);

			aux = "Position" + index;
			file.SetNewJson3Number(file, aux.c_str(), position);

			aux = "Rotation" + index;
			file.SetNewJson4Number(file, aux.c_str(), rotation);

			aux = "Scale" + index;
			file.SetNewJson3Number(file, aux.c_str(), scale);
		}
		file.SaveFile(json.c_str());
	}

	if (app->fs->Save(name.c_str(), fileBuffer, size) > 0)
		DEBUG_LOG("Mesh %s saved succesfully", name.c_str());

	RELEASE_ARRAY(fileBuffer);


	//unsigned int header[2] = { vertices.size(), indices.size() };

	//uint size = sizeof(header) + sizeof(Vertex) * vertices.size() + sizeof(unsigned int) * indices.size();

	//char* buffer = new char[size];
	//char* cursor = buffer;

	//uint bytes = sizeof(header);
	//memcpy(cursor, header, bytes);
	//cursor += bytes;

	//bytes = sizeof(Vertex) * vertices.size();
	//memcpy(cursor, vertices.data(), bytes);
	//cursor += bytes;

	//bytes = sizeof(unsigned int) * indices.size();
	//memcpy(cursor, indices.data(), bytes);
	//cursor += bytes;

	////bytes = sizeof(float3) * normals.size();
	////memcpy(cursor, normals.data(), bytes);
	////cursor += bytes;
	////
	////bytes = sizeof(float2) * texCoords.size();
	////memcpy(cursor, texCoords.data(), bytes);
	////cursor += bytes;

	//if (app->fs->Save(name.c_str(), buffer, size) > 0)
	//	DEBUG_LOG("Mesh %s saved succesfully", name.c_str());

	// RELEASE_ARRAY(buffer);
}

void MeshImporter::LoadMesh(std::vector<Vertex>& vertices, std::vector<unsigned int>& indices, std::map<std::string, BoneInfo>& bones, std::string& path)
{
	char* buffer = nullptr;

	if (app->fs->Load(path.c_str(), &buffer) > 0)
	{
		char* cursor = buffer;
		unsigned int ranges[4];

		unsigned int bytes = sizeof(ranges);
		memcpy(ranges, cursor, bytes);
		cursor += bytes;

		vertices.resize(ranges[0]);
		indices.resize(ranges[1]);

		// Load vertices
		bytes = sizeof(Vertex) * vertices.size();
		memcpy(vertices.data(), cursor, bytes);
		cursor += bytes;

		// Load indices
		bytes = sizeof(unsigned int) * indices.size();
		memcpy(indices.data(), cursor, bytes);
		cursor += bytes;

		std::string jsonFile;
		jsonFile.resize(ranges[3]);
		bytes = ranges[3];
		memcpy(&jsonFile[0], cursor, bytes);
		cursor += bytes;

		JsonParsing file = JsonParsing();
		file.ParseFile(jsonFile.c_str());
		
		// Load bones
		for (int i = 0; i < ranges[2]; ++i)
		{
			unsigned int stringSize;
			std::string name;
			BoneInfo info;
			
			std::string index = std::to_string(i);
			std::string aux = "Name" + index;
			name = file.GetJsonString(aux.c_str());

			aux = "Id" + index;
			info.id = file.GetJsonNumber(aux.c_str());

			aux = "Position" + index;
			float3 position = file.GetJson3Number(file, aux.c_str());		

			aux = "Rotation" + index;
			float4 rotation = file.GetJson4Number(file, aux.c_str());

			aux = "Scale" + index;
			float3 scale = file.GetJson3Number(file, aux.c_str());

			Quat quat = { rotation.x, rotation.y, rotation.z, rotation.w };

			info.offset = info.offset.FromTRS(position, quat, scale);

			bones[name] = info;
		}
	}
	else
		DEBUG_LOG("Mesh file not found!");

	RELEASE_ARRAY(buffer);
}

void MeshImporter::CreateMetaMesh(std::string& library, std::string& assets, uint uid)
{
	JsonParsing metaFile;

	metaFile.SetNewJsonString(metaFile.ValueToObject(metaFile.GetRootValue()), "Assets Path", assets.c_str());
	metaFile.SetNewJsonNumber(metaFile.ValueToObject(metaFile.GetRootValue()), "Uuid", uid);

	char* buffer = nullptr;
	size_t size = metaFile.Save(&buffer);

	app->fs->Save(library.c_str(), buffer, size);

	RELEASE_ARRAY(buffer);
}

void MeshImporter::ExtractBonesAndWeights(std::vector<Vertex>& vertices, const aiMesh* mesh, const aiScene* scene, std::map<std::string, BoneInfo>& bones)
{
	int numBones = 0;
	for (int boneIndex = 0; boneIndex < mesh->mNumBones; ++boneIndex)
	{
		int boneID = -1;
		std::string boneName = mesh->mBones[boneIndex]->mName.C_Str();

		if (bones.find(boneName) == bones.end())
		{
			BoneInfo newBoneInfo;
			newBoneInfo.id = numBones;
			newBoneInfo.offset = FromAssimpMatrixToMathGeoLib(mesh->mBones[boneIndex]->mOffsetMatrix);
			bones[boneName] = newBoneInfo;
			boneID = numBones;
			numBones++;
		}
		else
		{
			boneID = bones[boneName].id;
		}

		aiVertexWeight* weights = mesh->mBones[boneIndex]->mWeights;
		int numWeights = mesh->mBones[boneIndex]->mNumWeights;

		for (int weightIndex = 0; weightIndex < numWeights; ++weightIndex)
		{
			int vertexId = weights[weightIndex].mVertexId;
			float weight = weights[weightIndex].mWeight;
			SetBoneData(vertices[vertexId], boneID, weight);
		}
	}
}

void MeshImporter::ComputeTangents(std::vector<Vertex>& vertices, unsigned int indicesSize)
{
	for (int i = 0; i < indicesSize; i += 3)
	{
		float2 uv1 = { vertices[i * 2].texCoords };
		float2 uv2 = { vertices[i * 2 + 1].texCoords };
		float2 uv3 = { vertices[i * 2 + 2].texCoords };

		float2 deltaUv1 = uv2 - uv1;
		float2 deltaUv2 = uv3 - uv1;

		float3 edge1 = vertices[i + 1].position - vertices[i].position;
		float3 edge2 = vertices[i + 2].position - vertices[i].position;

		float f = 1.0f / (deltaUv1.x * deltaUv2.y - deltaUv2.x * deltaUv1.y);

		vertices[i].tangents.x = f * (deltaUv2.y * edge1.x - deltaUv1.y * edge2.x);
		vertices[i].tangents.y = f * (deltaUv2.y * edge1.y - deltaUv1.y * edge2.y);
		vertices[i].tangents.z = f * (deltaUv2.y * edge1.z - deltaUv1.y * edge2.z);
		vertices[i].tangents.Normalize();

		if (i + 3 >= indicesSize)
			break;
	}
}


float4x4 MeshImporter::FromAssimpMatrixToMathGeoLib(aiMatrix4x4& matrix)
{
	float4x4 mathMatrix;
	aiQuaternion quat;
	aiVector3D pos;
	aiVector3D sca;
	matrix.Decompose(sca, quat, pos);
	
	float3 position = { pos.x, pos.y, pos.z };
	float3 scale = { sca.x, sca.y, sca.z };
	Quat quaternion = { quat.x, quat.y, quat.z, quat.w };

	return mathMatrix.FromTRS(position, quaternion, scale);
}

void MeshImporter::SetBoneData(Vertex& vertex, int boneID, float weight)
{
	for (int i = 0; i < 4; ++i)
	{
		if (vertex.boneIDs[i] < 0)
		{
			vertex.weights[i] = weight;
			vertex.boneIDs[i] = boneID;
			break;
		}
	}
}

void MeshImporter::SerializeBoneData(BoneInfo& info, JsonParsing& file)
{
	/*unsigned int bytes = sizeof(info.id);
	memcpy(*buffer, &info.id, bytes);
	*buffer += bytes;

	float3 position;
	Quat rotation;
	float3 scale;
	info.offset.Decompose(position, rotation, scale);

	bytes = sizeof(float3);
	memcpy(*buffer, &position, bytes);
	*buffer += bytes;

	bytes = sizeof(Quat);
	memcpy(*buffer, &rotation, bytes);
	*buffer += bytes;

	bytes = sizeof(float3);
	memcpy(*buffer, &scale, bytes);
	*buffer += bytes;*/
}
