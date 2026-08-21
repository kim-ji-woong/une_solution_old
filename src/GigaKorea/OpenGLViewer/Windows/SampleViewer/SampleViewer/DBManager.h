#pragma once
#include <list>
#include <map>

namespace FireSafetyManager
{
	class Project;
	class Level;
	class POIType;
}

class DBManager
{
public:
	DBManager();
	virtual ~DBManager();

public:
	bool LoadDB(char* strPath, char* strPOIFolder);
	char* GetErrorMessage();

	int GetProjectCount();
	FireSafetyManager::Project* GetProject(int nIndex);
	std::map<int, FireSafetyManager::POIType*>& GetPOITypes();

private:
	bool LoadPOITypes(void* db, char* strPOIFolder);
	bool LoadProjects(void* db);
	bool LoadProjectComponent(void* db, FireSafetyManager::Project* project);
	bool LoadLevels(void* db);
	bool LoadAlertAreas(void* db);
	bool LoadWalls(void* db, FireSafetyManager::Level* pLevel, FireSafetyManager::Project* project);
	bool LoadSpaces(void* db, FireSafetyManager::Level* pLevel, FireSafetyManager::Project* project);
	bool LoadDoors(void* db, FireSafetyManager::Level* pLevel, FireSafetyManager::Project* project);
	bool LoadWindows(void* db, FireSafetyManager::Level* pLevel, FireSafetyManager::Project* project);
	bool LoadPOIs(void* db, FireSafetyManager::Level* pLevel, FireSafetyManager::Project* project);
	bool LoadColumns(void* db, FireSafetyManager::Level* pLevel, FireSafetyManager::Project* project);
	bool LoadBoundary(void* db, int nBoundaryID, std::list<VectorGraphics::Vertex2D>& vertices);

	FireSafetyManager::Project* FindProject(int nID);

private:
	char m_strError[256];
	std::list<FireSafetyManager::Project*> m_projects;
	std::map<int, FireSafetyManager::POIType*> m_mapPOITypes;
};

