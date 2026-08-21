#pragma once
#include <list>
#include <map>
#include <string>

namespace FireSafetyManager
{
	class Project;
	class Level;
	class POIType;
	class Line2D;
}

namespace VectorGraphics
{
	class Vertex2D;
}

class XMLManager
{
public:
	XMLManager();
	virtual ~XMLManager();

public:
	bool LoadXML(std::wstring strPath, char* strPOIFolder);
	std::wstring GetErrorMessage();

	int GetProjectCount();
	FireSafetyManager::Project* GetProject(int nIndex);
	std::map<int, FireSafetyManager::POIType*>& GetPOITypes();

private:
	bool LoadPOITypes(void* xml, char* strPOIFolder);
	FireSafetyManager::Project* LoadProjects(void* xml);
	bool LoadProjectComponent(void* xml, FireSafetyManager::Project* project);
	bool LoadLevels(void* xml, FireSafetyManager::Project* project);
	bool LoadLevel(void* xml, DWORD_PTR node, FireSafetyManager::Project* project);
	bool LoadAlertAreas(void* xml, DWORD_PTR node, FireSafetyManager::Level* pLevel);
	bool LoadWall(void* xml, DWORD_PTR node, FireSafetyManager::Level* pLevel, FireSafetyManager::Project* project, std::map<std::wstring, FireSafetyManager::Line2D*>& rMapGrid);
	bool LoadSpace(void* xml, DWORD_PTR node, FireSafetyManager::Level* pLevel, FireSafetyManager::Project* project);
	bool LoadDoors(void* xml, FireSafetyManager::Level* pLevel, FireSafetyManager::Project* project);
	bool LoadWindows(void* xml, FireSafetyManager::Level* pLevel, FireSafetyManager::Project* project);
	bool LoadPOIs(void* xml, FireSafetyManager::Level* pLevel, FireSafetyManager::Project* project);
	bool LoadColumns(void* xml, FireSafetyManager::Level* pLevel, FireSafetyManager::Project* project);

	FireSafetyManager::Project* FindProject(int nID);

private:
	std::wstring m_strError;
	std::list<FireSafetyManager::Project*> m_projects;
	std::map<int, FireSafetyManager::POIType*> m_mapPOITypes;
};

