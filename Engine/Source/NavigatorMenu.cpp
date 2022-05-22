#include "NavigatorMenu.h"
#include "Application.h"
#include "Globals.h"

#include "NavMeshBuilder.h"

#include "Profiling.h"

NavigatorMenu::NavigatorMenu() : Menu(true, "Navigator")
{
}

NavigatorMenu::~NavigatorMenu()
{
}

bool NavigatorMenu::Start()
{
    buildSettings = app->navMesh->GetBuildSettings();

    DEBUG_LOG("Linking the ModuleNavMesh Settings");
    bool ret = true;

    return ret;
}

bool NavigatorMenu::Update(float dt)
{
    RG_PROFILING_FUNCTION("Navigator Menu");

    ImGui::Begin(ICON_FA_WALKING" Navigator", &active);
    ImGui::Dummy({ 0,10 });
    ImGui::SetCursorPosX((ImGui::GetWindowContentRegionMax().x - 180) / 2);
    if (ImGui::Button("Bake NavMesh", ImVec2(180.0f, 20.0f)))
    {
        app->navMesh->BakeNavMesh();
    }
    ImGui::Dummy({ 0,10 });

    ImGui::Text("Agent Properties");
    ImGui::Separator();
    ImGui::Spacing();
    ImGui::PushItemWidth(180);
    ImGui::DragFloat("Agent radius", &buildSettings->agentRadius, 0.1f,0.f);
    ImGui::DragFloat("Agent height", &buildSettings->agentHeight, 0.1f, 0.f);
    ImGui::DragFloat("Agent max climb", &buildSettings->agentMaxClimb, 0.1f,0.f);
    ImGui::DragFloat("Agent max slope", &buildSettings->agentMaxSlope, 0.1f,0.f);
    ImGui::PopItemWidth();
    ImGui::Dummy({ 0,10 });

    ImGui::Text("NavMesh Properties");
    ImGui::Separator();
    ImGui::Spacing();
    ImGui::PushItemWidth(180);
    ImGui::DragFloat("Cell size", &buildSettings->cellSize, 0.1f, 0.f);
    ImGui::DragFloat("Cell height", &buildSettings->cellHeight, 0.1f, 0.f);
    ImGui::DragFloat("Region minimum size", &buildSettings->regionMinSize, 0.1f,0.f);
    ImGui::DragFloat("Region merge size", &buildSettings->regionMergeSize, 0.1f,0.f);
    ImGui::DragFloat("Edge max lenght", &buildSettings->edgeMaxLen, 0.1f, 0.f);
    ImGui::DragFloat("Edge max error", &buildSettings->edgeMaxError, 0.1f, 0.f);
    ImGui::DragFloat("Detail sample distance", &buildSettings->detailSampleDist, 0.1f, 0.f);
    ImGui::DragFloat("Detail sample max error", &buildSettings->detailSampleMaxError, 0.1f, 0.f);
    ImGui::DragFloat("Tile Size", &buildSettings->tileSize, 0.1f, 0.f);
    ImGui::DragFloat("Verts per Poly", &buildSettings->vertsPerPoly, 0.1f, 0.f);
    ImGui::DragInt("Partition Type", &buildSettings->partitionType, 1, 0);
    ImGui::PopItemWidth();
    ImGui::Dummy({ 0,10 });

    if (ImGui::CollapsingHeader("NavMeshBuilder"))
    {
        ImGui::Text("NaveMesh Geometry");
        ImGui::Separator();
        ImGui::Spacing();
        
        const SimpleMesh* mesh = app->navMesh->GetInputGeom()->getMesh();
        const NavMeshBuilder* builder = app->navMesh->GetNavMeshBuilder();
        if (mesh->vertices.size() <= 0 && mesh->indices.size() <= 0)
            ImGui::Text("No Geometry added");
        else 
        {
            ImGui::Text("Verts: %d", mesh->vertices.size());
            ImGui::Text("Indices: %d", mesh->indices.size());

            ImGui::Spacing();
            ImGui::Spacing();

            ImGui::Text("Max Tiles: %i", builder->GetMaxTyles());
            ImGui::Text("Max Polys per tiles: %i", builder->GetMaxPolyTile());
            ImGui::Text("Cell size: %.4f", buildSettings->cellSize);
            ImGui::Text("Cell Height: %.4f", buildSettings->cellHeight);

            if (builder->GetPolyMesh() != nullptr)
            {
                ImGui::Spacing();
                ImGui::Separator();
                ImGui::Separator();
                ImGui::Spacing();
                ImGui::Text("PolyMesh");

                ImGui::Text("Verts: %i", builder->GetPolyMesh()->nverts);
                ImGui::Text("Polys: %i", builder->GetPolyMesh()->npolys);
            }
        }
    }

    ImGui::End();
    return true;
}