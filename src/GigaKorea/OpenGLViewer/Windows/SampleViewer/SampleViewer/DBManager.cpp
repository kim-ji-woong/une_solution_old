#include "stdafx.h"
#include "DBManager.h"
#include "VectorCtrl.h"
#include "sqlite3.h"
#include <string.h>
#include <string>
#include <codecvt>
#include "Data/Project.h"
#include "Data/Level.h"
#include "Data/Component.h"
#include "Data/Wall.h"
#include "Data/Space.h"
#include "Data/Door.h"
#include "Data/Window.h"
#include "Data/_POI.h"
#include "Data/Line2D.h"
#include "Data/Column.h"
#include "Data/AlertArea.h"
#include <algorithm>
#include <vector>
#include "Line.h"
#include "Arc.h"
#include "EArc.h"

using namespace FireSafetyManager;
using namespace VectorGraphics;

DBManager::DBManager()
{
	strcpy_s(m_strError, 256, "");
}

DBManager::~DBManager()
{
}

bool DBManager::LoadDB(char* strPath, char* strPOIFolder)
{
	sqlite3* db;

	int rc = sqlite3_open(strPath, &db);

	if (rc)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(db));
		return false;
	}

	if (LoadPOITypes(db, strPOIFolder))
	{
		if (LoadProjects(db))
		{
			LoadLevels(db);
			LoadAlertAreas(db);
		}
	}

	sqlite3_close(db);
	return true;
}

static std::wstring UTF8ToANSI(const char *utf8str)
{
	std::wstring_convert<std::codecvt_utf8<wchar_t>> wconv;
	std::wstring wstr = wconv.from_bytes(utf8str);
	return wstr;
}

bool DBManager::LoadPOITypes(void* db, char* strPOIFolder)
{
	sqlite3* _db = (sqlite3*)db;
	char* strSQL = "Select ID, IsGroup, Name, Code from POIType";
	sqlite3_stmt *stmt;

	int rc = sqlite3_prepare_v2(_db, strSQL, -1, &stmt, NULL);
	if (rc != SQLITE_OK)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		return false;
	}

	char strPOIPath[512];

	while ((rc = sqlite3_step(stmt)) == SQLITE_ROW)
	{
		int nID = sqlite3_column_int(stmt, 0);
		bool isGroup = sqlite3_column_int(stmt, 1) == 1;
		std::wstring strName = UTF8ToANSI((const char*)sqlite3_column_text(stmt, 2));

		if (isGroup == false)
		{
			POIType* type = 0;
			const char* code = (const char*)sqlite3_column_text(stmt, 3);

			if (code == 0)
				type = new POIType(nID, strName, L"", 0);
			else
			{
				std::wstring strCode = UTF8ToANSI(code);
				type = new POIType(nID, strName, strCode, 0);

				sprintf_s(strPOIPath, 512, "%s\\%s.poi", strPOIFolder, code);
				type->LoadPOIIcon(strPOIPath);
			}

			m_mapPOITypes[nID] = type;
		}
	}

	bool result = true;

	if (rc != SQLITE_DONE)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		result = false;
	}

	sqlite3_finalize(stmt);

	for (std::list<Project*>::iterator iter = m_projects.begin(); iter != m_projects.end(); iter++)
	{
		if (LoadProjectComponent(db, *iter) == false)
			return false;
	}

	return result;
}

bool DBManager::LoadProjects(void* db)
{
	sqlite3* _db = (sqlite3*)db;
	char* strSQL = "Select ID, Name, UnitOfLength, TimeStamp, GlobalPosX, GlobalPosY, GlobalUnitOfLength, LocalPosX, LocalPosY, LocalAngle from Project";
	sqlite3_stmt *stmt;

	int rc = sqlite3_prepare_v2(_db, strSQL, -1, &stmt, NULL);
	if (rc != SQLITE_OK)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		return false;
	}

	while ((rc = sqlite3_step(stmt)) == SQLITE_ROW)
	{
		int nID = sqlite3_column_int(stmt, 0);
		std::wstring strName = UTF8ToANSI((const char*)sqlite3_column_text(stmt, 1));
		int nUnit = sqlite3_column_int(stmt, 2);
		const char* strTime = (const char*)sqlite3_column_text(stmt, 3);

		DateTime timeStamp;

		if (DateTime::FromString(strTime, timeStamp))
		{
			Project* project = new Project(nID, strName, nUnit, timeStamp);
			m_projects.push_back(project);

			if (sqlite3_column_text(stmt, 4) != 0 && sqlite3_column_text(stmt, 5) != 0 && sqlite3_column_text(stmt, 6) != 0 && sqlite3_column_text(stmt, 7) != 0 && sqlite3_column_text(stmt, 8) != 0 && sqlite3_column_text(stmt, 9) != 0)
			{
				double globalX = sqlite3_column_double(stmt, 4);
				double globalY = sqlite3_column_double(stmt, 5);
				int nGlobalUnit = sqlite3_column_int(stmt, 6);
				double localX = sqlite3_column_double(stmt, 7);
				double localY = sqlite3_column_double(stmt, 8);
				double localAngle = sqlite3_column_double(stmt, 9);

				AnchorNode* pAnchorNode = new AnchorNode(Vertex2D(globalX, globalY), Vertex2D(localX, localY), localAngle, (Project::UnitOfLength)nGlobalUnit, (Project::UnitOfLength)nUnit);
				project->SetAnchorNode(pAnchorNode);
			}
		}
	}

	bool result = true;

	if (rc != SQLITE_DONE)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		result = false;
	}

	sqlite3_finalize(stmt);

	for (std::list<Project*>::iterator iter = m_projects.begin(); iter != m_projects.end(); iter++)
	{
		if (LoadProjectComponent(db, *iter) == false)
			return false;
	}

	return result;
}

bool DBManager::LoadProjectComponent(void* db, Project* project)
{
	sqlite3* _db = (sqlite3*)db;
	char strSQL[256];
	sprintf_s(strSQL, 256, "Select ID, TypeName, ComponentName from Component where ProjectID = %d", project->GetID());
	sqlite3_stmt *stmt;

	int rc = sqlite3_prepare_v2(_db, strSQL, -1, &stmt, NULL);
	if (rc != SQLITE_OK)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		return false;
	}

	while ((rc = sqlite3_step(stmt)) == SQLITE_ROW)
	{
		int nID = sqlite3_column_int(stmt, 0);
		std::wstring strTypeName = UTF8ToANSI((const char*)sqlite3_column_text(stmt, 1));
		std::wstring strComponentName = UTF8ToANSI((const char*)sqlite3_column_text(stmt, 2));

		Component* pComponent = new Component(nID, strTypeName, strComponentName);
		project->AddComponent(pComponent);
	}

	bool result = true;

	if (rc != SQLITE_DONE)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		result = false;
	}

	sqlite3_finalize(stmt);
	return result;
}

bool DBManager::LoadLevels(void* db)
{
	sqlite3* _db = (sqlite3*)db;
	char strSQL[256];

	for (std::list<Project*>::iterator iter = m_projects.begin(); iter != m_projects.end(); iter++)
	{
		Project* project = *iter;

		sprintf_s(strSQL, 256, "Select ID, Name, Elevation from Level where ProjectID = %d", project->GetID());
		sqlite3_stmt *stmt;

		int rc = sqlite3_prepare_v2(_db, strSQL, -1, &stmt, NULL);
		if (rc != SQLITE_OK)
		{
			strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
			return false;
		}

		std::vector<Level*> levels;

		while ((rc = sqlite3_step(stmt)) == SQLITE_ROW)
		{
			int nID = sqlite3_column_int(stmt, 0);
			std::wstring strName = UTF8ToANSI((const char*)sqlite3_column_text(stmt, 1));
			double dElevation = sqlite3_column_double(stmt, 2);

			Level* pLevel = new Level(nID, -1, strName, dElevation);
			levels.push_back(pLevel);
		}

		bool result = true;

		if (rc != SQLITE_DONE)
		{
			strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
			result = false;
		}

		sqlite3_finalize(stmt);

		// Ãþ ¹øÈ£ ºÎ¿©
		std::sort(levels.begin(), levels.end(), Level::CompareLevel);
		int nFirstFloorIndex = -1;
		int i = 0;

		for (std::vector<Level*>::iterator _iter = levels.begin(); _iter != levels.end(); _iter++, i++)
		{
			Level* pLevel = *_iter;

			if (nFirstFloorIndex >= 0)
			{
				pLevel->SetFloorIndex(i - nFirstFloorIndex);
			}
			else
			{
				if (pLevel->GetElevation() >= 0.0)
				{
					nFirstFloorIndex = i;
					pLevel->SetFloorIndex(0);
				}
			}
		}

		i = 0;

		for (std::vector<Level*>::iterator _iter = levels.begin(); i < nFirstFloorIndex; _iter++, i++)
		{
			Level* pLevel = *_iter;
			pLevel->SetFloorIndex(i - nFirstFloorIndex);
		}

		for (std::vector<Level*>::iterator _iter = levels.begin(); _iter != levels.end(); _iter++)
		{
			Level* pLevel = *_iter;

			if (LoadWalls(db, pLevel, project) == false)
				return false;
			if (LoadSpaces(db, pLevel, project) == false)
				return false;
			if (LoadDoors(db, pLevel, project) == false)
				return false;
			if (LoadWindows(db, pLevel, project) == false)
				return false;
			if (LoadPOIs(db, pLevel, project) == false)
				return false;
			if (LoadColumns(db, pLevel, project) == false)
				return false;

			project->AddLevel(*_iter);
		}

		levels.clear();
	}

	return true;
}

bool DBManager::LoadAlertAreas(void* db)
{
	char str[256];
	std::string strProjectIDs = "";
	std::list<FireSafetyManager::Project*>::iterator iter = m_projects.begin();

	for (; iter != m_projects.end(); iter++)
	{
		FireSafetyManager::Project* project = *iter;
		sprintf_s(str, 256, "%d", project->GetID());

		if (strProjectIDs.length() == 0)
			strProjectIDs = str;
		else
		{
			strProjectIDs += ", ";
			strProjectIDs += str;
		}
	}

	if (strProjectIDs.length() == 0)
		return true;

	char strSQL[256];
	sprintf_s(strSQL, 256, "Select ID, Name, BoundaryID, LevelIDs, ProjectID from AlertArea where ProjectID in (%s)", strProjectIDs.c_str());

	sqlite3* _db = (sqlite3*)db;
	sqlite3_stmt *stmt;

	int rc = sqlite3_prepare_v2(_db, strSQL, -1, &stmt, NULL);
	if (rc != SQLITE_OK)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		return false;
	}

	std::map<AlertArea*, int> mapAreaBoundaries;

	while ((rc = sqlite3_step(stmt)) == SQLITE_ROW)
	{
		int nID = sqlite3_column_int(stmt, 0);
		std::wstring strName = UTF8ToANSI((const char*)sqlite3_column_text(stmt, 1));
		int nBoundaryID = sqlite3_column_int(stmt, 2);
		std::string strLevelIDs = (const char*)sqlite3_column_text(stmt, 3);
		int nProjectID = sqlite3_column_int(stmt, 4);

		Project* project = FindProject(nProjectID);

		if (project == 0)
			continue;

		AlertArea* pArea = new AlertArea();
		pArea->SetID(nID);
		pArea->SetName(strName);

		mapAreaBoundaries[pArea] = nBoundaryID;

		int nBeginIndex = -1;

		for (int i = 0; i < strLevelIDs.length(); i++)
		{
			char ch = strLevelIDs.at(i);

			if (nBeginIndex < 0)
			{
				if (ch >= '0' && ch <= '9')
				{
					nBeginIndex = i;
				}
			}
			else
			{
				if (ch < '0' || ch > '9')
				{
					std::string strLevelID = strLevelIDs.substr(nBeginIndex, i - nBeginIndex);
					int nLevelID = atoi(strLevelID.c_str());

					Level* pLevel = project->FindLevel(nLevelID);

					if (pLevel != 0)
					{
						pLevel->AddAlertArea(pArea);
					}

					nBeginIndex = -1;
				}
			}
		}

		if (nBeginIndex >= 0)
		{
			std::string strLevelID = strLevelIDs.substr(nBeginIndex, strLevelIDs.length() - nBeginIndex);
			int nLevelID = atoi(strLevelID.c_str());

			Level* pLevel = project->FindLevel(nLevelID);

			if (pLevel != 0)
			{
				pLevel->AddAlertArea(pArea);
			}
		}
	}

	bool result = true;

	if (rc != SQLITE_DONE)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		result = false;
	}

	sqlite3_finalize(stmt);

	for (std::map<AlertArea*, int>::iterator iter = mapAreaBoundaries.begin(); iter != mapAreaBoundaries.end(); iter++)
	{
		std::list<Vertex2D> vertices;

		if (LoadBoundary(db, iter->second, vertices) == false)
			return false;

		AlertArea* pArea = iter->first;

		for (std::list<Vertex2D>::iterator _iter = vertices.begin(); _iter != vertices.end(); _iter++)
		{
			pArea->AddBoundaryVertex(*_iter);
		}
	}

	return result;
}

bool DBManager::LoadWalls(void* db, Level* pLevel, Project* project)
{
	char strSQL[256];
	sprintf_s(strSQL, 256, "Select wall.ID, Thick, Height, ComponentID, grid.GridType, grid.BeginX, grid.BeginY, grid.EndX, grid.EndY, grid.ThirdX, grid.ThirdY, grid.BeginAngle, grid.Angle, grid.ClockWise, BoundaryID from wall, grid where wall.GridID = grid.ID and wall.LevelID = %d", pLevel->GetID());

	sqlite3* _db = (sqlite3*)db;
	sqlite3_stmt *stmt;

	int rc = sqlite3_prepare_v2(_db, strSQL, -1, &stmt, NULL);
	if (rc != SQLITE_OK)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		return false;
	}

	std::map<Wall*, int> mapWallBoundaries;

	while ((rc = sqlite3_step(stmt)) == SQLITE_ROW)
	{
		int nID = sqlite3_column_int(stmt, 0);
		double dThick = sqlite3_column_double(stmt, 1);
		double dHeight = sqlite3_column_double(stmt, 2);
		int nComponentID = sqlite3_column_int(stmt, 3);
		int nGridType = sqlite3_column_int(stmt, 4);
		int nBoundaryID = sqlite3_column_int(stmt, 14);

		Component* pComponent = project->FindComponent(nComponentID);

		if (pComponent == 0)
			continue;

		Line2D* pLine = 0;
		VectorGraphics::Arc* pArc = 0;
		EArc* pEArc = 0;

		Wall::GridType gridType = Wall::ToGridType(nGridType);

		if (gridType == Wall::GridType::Line)
		{
			double x1 = sqlite3_column_double(stmt, 5);
			double y1 = sqlite3_column_double(stmt, 6);
			double x2 = sqlite3_column_double(stmt, 7);
			double y2 = sqlite3_column_double(stmt, 8);

			pLine = new Line2D(Vertex2D(x1, y1), Vertex2D(x2, y2));
		}
		else if (gridType == Wall::GridType::Arc)
		{
			if (sqlite3_column_text(stmt, 9) == 0 || sqlite3_column_text(stmt, 11) == 0 ||
				sqlite3_column_text(stmt, 12) == 0 || sqlite3_column_text(stmt, 13) == 0)
				continue;

			double centerX = sqlite3_column_double(stmt, 5);
			double centerY = sqlite3_column_double(stmt, 6);
			double radius = sqlite3_column_double(stmt, 9);
			double beginAngle = sqlite3_column_double(stmt, 11);
			double arcAngle = sqlite3_column_double(stmt, 12);
			bool isClockWise = sqlite3_column_int(stmt, 13) == 1;

			pArc = new VectorGraphics::Arc(Vertex2D(centerX, centerY), radius, beginAngle, arcAngle, isClockWise);
		}
		else if (gridType == Wall::GridType::EArc)
		{
			if (sqlite3_column_text(stmt, 9) == 0 || sqlite3_column_text(stmt, 10) == 0 ||
				sqlite3_column_text(stmt, 11) == 0 || sqlite3_column_text(stmt, 12) == 0 ||
				sqlite3_column_text(stmt, 13) == 0)
				continue;

			double tlX = sqlite3_column_double(stmt, 5);
			double tlY = sqlite3_column_double(stmt, 6);
			double blX = sqlite3_column_double(stmt, 7);
			double blY = sqlite3_column_double(stmt, 8);
			double brX = sqlite3_column_double(stmt, 9);
			double brY = sqlite3_column_double(stmt, 10);
			double beginAngle = sqlite3_column_double(stmt, 11);
			double earcAngle = sqlite3_column_double(stmt, 12);
			bool isClockWise = sqlite3_column_int(stmt, 13) == 1;

			pEArc = new EArc(Vertex2D(tlX, tlY), Vertex2D(blX, blY), Vertex2D(brX, brY), beginAngle, earcAngle, isClockWise);
		}
		
		Wall* pWall = 0;

		if (pLine != 0)
			pWall = new Wall(nID, dThick, dHeight, pComponent, pLine);
		else if (pArc != 0)
			pWall = new Wall(nID, dThick, dHeight, pComponent, pArc);
		else if (pEArc != 0)
			pWall = new Wall(nID, dThick, dHeight, pComponent, pEArc);
		else
			continue;

		pLevel->AddWall(pWall);
		mapWallBoundaries[pWall] = nBoundaryID;
	}

	bool result = true;

	if (rc != SQLITE_DONE)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		result = false;
	}

	sqlite3_finalize(stmt);

	for (std::map<Wall*, int>::iterator iter = mapWallBoundaries.begin(); iter != mapWallBoundaries.end(); iter++)
	{
		std::list<Vertex2D> vertices;
		
		if (LoadBoundary(db, iter->second, vertices) == false)
			return false;

		Wall* pWall = iter->first;

		for (std::list<Vertex2D>::iterator _iter = vertices.begin(); _iter != vertices.end(); _iter++)
		{
			pWall->AddBoundaryVertex(*_iter);
		}
	}

	return true;
}

bool DBManager::LoadSpaces(void* db, FireSafetyManager::Level* pLevel, FireSafetyManager::Project* project)
{
	char strSQL[512];
	sprintf_s(strSQL, 512, "Select Space.ID, Space.Name, swl.WallID, swl.WallIndex, BoundaryID from Space, SpaceWallLink as swl where Space.ID = swl.SpaceID and Space.LevelID = %d order by Space.ID, swl.WallIndex", pLevel->GetID());

	sqlite3* _db = (sqlite3*)db;
	sqlite3_stmt *stmt;

	int rc = sqlite3_prepare_v2(_db, strSQL, -1, &stmt, NULL);
	if (rc != SQLITE_OK)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		return false;
	}

	std::map<Space*, int> mapSpaceBoundary;

	while ((rc = sqlite3_step(stmt)) == SQLITE_ROW)
	{
		int nID = sqlite3_column_int(stmt, 0);
		std::wstring strName = UTF8ToANSI((const char*)sqlite3_column_text(stmt, 1));
		int nWallID = sqlite3_column_int(stmt, 2);
		int nBoundaryID = sqlite3_column_int(stmt, 4);

		Wall* pWall = pLevel->FindWall(nWallID);

		if (pWall == 0)
			continue;

		Space* pSpace = pLevel->FindSpace(nID);

		if (pSpace == 0)
		{
			pSpace = new Space();
			pSpace->SetID(nID);
			pSpace->SetName(strName);

			pLevel->AddSapce(pSpace);
		}

		pSpace->AddWall(pWall);
		mapSpaceBoundary[pSpace] = nBoundaryID;
	}

	bool result = true;

	if (rc != SQLITE_DONE)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		result = false;
	}

	sqlite3_finalize(stmt);

	for (std::map<Space*, int>::iterator iter = mapSpaceBoundary.begin(); iter != mapSpaceBoundary.end(); iter++)
	{
		std::list<Vertex2D> vertices;

		if (LoadBoundary(db, iter->second, vertices) == false)
			return false;

		Space* pSpace = iter->first;

		for (std::list<Vertex2D>::iterator _iter = vertices.begin(); _iter != vertices.end(); _iter++)
		{
			pSpace->AddBoundaryVertex(*_iter);
		}
	}

	return true;
}

bool DBManager::LoadDoors(void* db, Level* pLevel, Project* project)
{
	char strSQL[256];
	sprintf_s(strSQL, 256, "Select ID, WallID, X, Y, Width, Height, Elevation, DoorType, Hinge1X, Hinge1Y, Hinge2X, Hinge2Y from Door where LevelID = %d", pLevel->GetID());

	sqlite3* _db = (sqlite3*)db;
	sqlite3_stmt *stmt;

	int rc = sqlite3_prepare_v2(_db, strSQL, -1, &stmt, NULL);
	if (rc != SQLITE_OK)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		return false;
	}

	while ((rc = sqlite3_step(stmt)) == SQLITE_ROW)
	{
		int nID = sqlite3_column_int(stmt, 0);
		int nWallID = sqlite3_column_int(stmt, 1);
		
		Wall* pWall = pLevel->FindWall(nWallID);

		if (pWall == 0)
			continue;

		Door* pDoor = new Door();
		pDoor->SetID(nID);

		double x = sqlite3_column_double(stmt, 2);
		double y = sqlite3_column_double(stmt, 3);
		pDoor->SetPosition(Vertex2D(x, y));

		pDoor->SetWidth(sqlite3_column_double(stmt, 4));
		pDoor->SetHeight(sqlite3_column_double(stmt, 5));
		pDoor->SetElevation(sqlite3_column_double(stmt, 6));
		pDoor->SetDoorType(Door::ToDoorType(sqlite3_column_int(stmt, 7)));

		if (sqlite3_column_text(stmt, 8) != 0 && sqlite3_column_text(stmt, 9) != 0)
		{
			double x1 = sqlite3_column_double(stmt, 8);
			double y1 = sqlite3_column_double(stmt, 9);
			pDoor->SetHinge1(Vertex2D(x1, y1));
		}

		if (sqlite3_column_text(stmt, 10) != 0 && sqlite3_column_text(stmt, 11) != 0)
		{
			double x2 = sqlite3_column_double(stmt, 10);
			double y2 = sqlite3_column_double(stmt, 11);
			pDoor->SetHinge2(Vertex2D(x2, y2));
		}

		pWall->AddDoor(pDoor);
	}

	bool result = true;

	if (rc != SQLITE_DONE)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		result = false;
	}

	sqlite3_finalize(stmt);
	return true;
}

bool DBManager::LoadWindows(void* db, Level* pLevel, Project* project)
{
	char strSQL[256];
	sprintf_s(strSQL, 256, "Select ID, WallID, X, Y, Width, Height, Elevation from Window where LevelID = %d", pLevel->GetID());

	sqlite3* _db = (sqlite3*)db;
	sqlite3_stmt *stmt;

	int rc = sqlite3_prepare_v2(_db, strSQL, -1, &stmt, NULL);
	if (rc != SQLITE_OK)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		return false;
	}

	while ((rc = sqlite3_step(stmt)) == SQLITE_ROW)
	{
		int nID = sqlite3_column_int(stmt, 0);
		int nWallID = sqlite3_column_int(stmt, 1);

		Wall* pWall = pLevel->FindWall(nWallID);

		if (pWall == 0)
			continue;

		Window* pWindow = new Window();
		pWindow->SetID(nID);

		double x = sqlite3_column_double(stmt, 2);
		double y = sqlite3_column_double(stmt, 3);
		pWindow->SetPosition(Vertex2D(x, y));

		pWindow->SetWidth(sqlite3_column_double(stmt, 4));
		pWindow->SetHeight(sqlite3_column_double(stmt, 5));
		pWindow->SetElevation(sqlite3_column_double(stmt, 6));
		
		pWall->AddWindow(pWindow);
	}

	bool result = true;

	if (rc != SQLITE_DONE)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		result = false;
	}

	sqlite3_finalize(stmt);
	return true;
}

bool DBManager::LoadPOIs(void* db, Level* pLevel, Project* project)
{
	char strSQL[256];
	sprintf_s(strSQL, 256, "Select ID, TypeID, Name, X, Y from POI where LevelID = %d", pLevel->GetID());

	sqlite3* _db = (sqlite3*)db;
	sqlite3_stmt *stmt;

	int rc = sqlite3_prepare_v2(_db, strSQL, -1, &stmt, NULL);
	if (rc != SQLITE_OK)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		return false;
	}

	while ((rc = sqlite3_step(stmt)) == SQLITE_ROW)
	{
		int nID = sqlite3_column_int(stmt, 0);
		int nTypeID = sqlite3_column_int(stmt, 1);
		std::wstring strName = UTF8ToANSI((const char*)sqlite3_column_text(stmt, 2));
		double x = sqlite3_column_double(stmt, 3);
		double y = sqlite3_column_double(stmt, 4);

		std::map<int, POIType*>::iterator iter = m_mapPOITypes.find(nTypeID);

		if (iter != m_mapPOITypes.end())
		{
			POIType* poiType = iter->second;

			POI* poi = new POI(nID, strName, Vertex2D(x, y), 0.0, 0.0, poiType);
			pLevel->AddPOI(poi);
		}
	}

	bool result = true;

	if (rc != SQLITE_DONE)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		result = false;
	}

	sqlite3_finalize(stmt);
	return true;
}

bool DBManager::LoadColumns(void* db, Level* pLevel, Project* project)
{
	char strSQL[256];
	sprintf_s(strSQL, 256, "Select ID, ColumnType, TLx, TLy, BLx, BLy, BRx, BRy from Column where LevelID = %d", pLevel->GetID());

	sqlite3* _db = (sqlite3*)db;
	sqlite3_stmt *stmt;

	int rc = sqlite3_prepare_v2(_db, strSQL, -1, &stmt, NULL);
	if (rc != SQLITE_OK)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		return false;
	}

	while ((rc = sqlite3_step(stmt)) == SQLITE_ROW)
	{
		int nID = sqlite3_column_int(stmt, 0);
		int nColumnType = sqlite3_column_int(stmt, 1);
		double dTLx = sqlite3_column_double(stmt, 2);
		double dTLy = sqlite3_column_double(stmt, 3);
		double dBLx = sqlite3_column_double(stmt, 4);

		Column::ColumnType columnType = (Column::ColumnType)nColumnType;
		Column* pColumn = 0;

		if (columnType == Column::ColumnType::RectType)
		{
			double dBLy = sqlite3_column_double(stmt, 5);
			double dBRx = sqlite3_column_double(stmt, 6);
			double dBRy = sqlite3_column_double(stmt, 7);

			pColumn = new Column();
			pColumn->SetRectType(Vertex2D(dTLx, dTLy), Vertex2D(dBLx, dBLy), Vertex2D(dBRx, dBRy));
		}
		else if (columnType == Column::ColumnType::CircleType)
		{
			pColumn = new Column();
			pColumn->SetCircleType(Vertex2D(dTLx, dTLy), dBLx);
		}

		if (pColumn == 0)
			continue;

		pColumn->SetID(nID);
		pLevel->AddColumn(pColumn);
	}

	bool result = true;

	if (rc != SQLITE_DONE)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		result = false;
	}

	sqlite3_finalize(stmt);
	return true;
}

bool LoadLineTypeBoundary(double beginX, double beginY, double endX, double endY, std::list<VectorGraphics::Vertex2D>& vertices, int nCount, VectorGraphics::Vertex2D& prev)
{
	VectorGraphics::Vertex2D v1(beginX, beginY);
	VectorGraphics::Vertex2D v2(endX, endY);

	if (nCount == 0)
	{
		vertices.push_back(v1);
		vertices.push_back(v2);
		prev = v2;
	}
	else
	{
		if (prev.GetDistance(v1) < 0.001)
		{
			vertices.push_back(v2);
			prev = v2;
		}
		else if (prev.GetDistance(v2) < 0.001)
		{
			vertices.push_back(v1);
			prev = v1;
		}
		else
		{
			if (nCount == 1)
			{
				vertices.reverse();
				prev = vertices.back();

				if (prev.GetDistance(v1) < 0.001)
				{
					vertices.push_back(v2);
					prev = v2;
				}
				else if (prev.GetDistance(v2) < 0.001)
				{
					vertices.push_back(v1);
					prev = v1;
				}
				else
					return false;
			}
			else
				return false;
		}
	}

	return true;
}

bool LoadArcTypeBoundary(double centerX, double centerY, double radius, double beginAngle, double arcAngle, bool isClockwise, std::list<VectorGraphics::Vertex2D>& vertices, int nCount, VectorGraphics::Vertex2D& prev)
{
	VectorGraphics::Arc arc(Vertex2D(centerX, centerY), radius, beginAngle, arcAngle, isClockwise);
	const std::list<Vertex2D>& arcVertices = arc.GetVertices();

	if (arcVertices.size() == 0)
		return false;

	VectorGraphics::Vertex2D vBegin = *arcVertices.begin();
	VectorGraphics::Vertex2D vEnd = arcVertices.back();

	bool positive = true;

	if (nCount == 0)
	{
		vertices.push_back(vBegin);
	}
	else
	{
		if (prev.GetDistance(vBegin) < 0.001)
		{
		}
		else if (prev.GetDistance(vEnd) < 0.001)
		{
			positive = false;
		}
		else
		{
			if (nCount == 1)
			{
				vertices.reverse();
				prev = vertices.back();

				if (prev.GetDistance(vBegin) < 0.001)
				{
				}
				else if (prev.GetDistance(vEnd) < 0.001)
				{
					positive = false;
				}
				else
					return false;
			}
			else
				return false;
		}
	}

	if (positive)
	{
		std::list<Vertex2D>::const_iterator iter = arcVertices.begin();
		iter++;

		for (; iter != arcVertices.end(); iter++)
		{
			vertices.push_back(*iter);
		}

		prev = vEnd;
	}
	else
	{
		std::list<Vertex2D>::const_reverse_iterator iter = arcVertices.rbegin();
		iter++;

		for (; iter != arcVertices.rend(); iter++)
		{
			vertices.push_back(*iter);
		}

		prev = vBegin;
	}

	return true;
}

bool LoadEArcTypeBoundary(double tlX, double tlY, double blX, double blY, double brX, double brY, double beginAngle, double earcAngle, bool isClockwise, std::list<VectorGraphics::Vertex2D>& vertices, int nCount, VectorGraphics::Vertex2D& prev)
{
	Vertex2D vTL(tlX, tlY);
	Vertex2D vBL(blX, blY);
	Vertex2D vBR(brX, brY);

	EArc earc(vTL, vBL, vBR, beginAngle, earcAngle, isClockwise);
	const std::list<Vertex2D>& earcVertices = earc.GetVertices();

	Vertex2D vBegin = *earcVertices.begin();
	Vertex2D vEnd = earcVertices.back();

	bool positive = true;

	if (nCount == 0)
	{
		vertices.push_back(vBegin);
	}
	else
	{
		if (prev.GetDistance(vBegin) < 0.001)
		{
		}
		else if (prev.GetDistance(vEnd) < 0.001)
		{
			positive = false;
		}
		else
		{
			if (nCount == 1)
			{
				vertices.reverse();
				prev = vertices.back();

				if (prev.GetDistance(vBegin) < 0.001)
				{
				}
				else if (prev.GetDistance(vEnd) < 0.001)
				{
					positive = false;
				}
				else
					return false;
			}
			else
				return false;
		}
	}

	if (positive)
	{
		std::list<Vertex2D>::const_iterator iter = earcVertices.begin();
		iter++;

		for (; iter != earcVertices.end(); iter++)
		{
			vertices.push_back(*iter);
		}

		prev = vEnd;
	}
	else
	{
		std::list<Vertex2D>::const_reverse_iterator iter = earcVertices.rbegin();
		iter++;

		for (; iter != earcVertices.rend(); iter++)
		{
			vertices.push_back(*iter);
		}

		prev = vBegin;
	}

	return true;
}

bool DBManager::LoadBoundary(void* db, int nBoundaryID, std::list<VectorGraphics::Vertex2D>& vertices)
{
	vertices.clear();

	char strSQL[256];
	sprintf_s(strSQL, 256, "Select OrderIndex, LineType, BeginX, BeginY, EndX, EndY, ThirdX, ThirdY, BeginAngle, Angle, ClockWise from Boundary where ID = %d order by OrderIndex", nBoundaryID);

	sqlite3* _db = (sqlite3*)db;
	sqlite3_stmt *stmt;

	int rc = sqlite3_prepare_v2(_db, strSQL, -1, &stmt, NULL);
	if (rc != SQLITE_OK)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		return false;
	}

	VectorGraphics::Vertex2D prev;
	int nCount = 0;

	while ((rc = sqlite3_step(stmt)) == SQLITE_ROW)
	{
		int nIndex = sqlite3_column_int(stmt, 0);
		int nTypeID = sqlite3_column_int(stmt, 1);
		double beginX = sqlite3_column_double(stmt, 2);
		double beginY = sqlite3_column_double(stmt, 3);
		double endX = sqlite3_column_double(stmt, 4);
		double endY = sqlite3_column_double(stmt, 5);

		if (nTypeID == (int)Wall::GridType::Line)
		{
			if (LoadLineTypeBoundary(beginX, beginY, endX, endY, vertices, nCount, prev) == false)
			{
				sqlite3_finalize(stmt);
				return false;
			}
		}
		else if (nTypeID == (int)Wall::GridType::Arc)
		{
			if (sqlite3_column_text(stmt, 6) != 0 && sqlite3_column_text(stmt, 8) != 0 && sqlite3_column_text(stmt, 9) != 0 && sqlite3_column_text(stmt, 10) != 0)
			{
				double radius = sqlite3_column_double(stmt, 6);
				double beginAngle = sqlite3_column_double(stmt, 8);
				double arcAngle = sqlite3_column_double(stmt, 9);
				bool isClockwise = sqlite3_column_int(stmt, 10) == 1;

				if (LoadArcTypeBoundary(beginX, beginY, radius, beginAngle, arcAngle, isClockwise, vertices, nCount, prev) == false)
				{
					sqlite3_finalize(stmt);
					return false;
				}
			}
			else
			{
				sqlite3_finalize(stmt);
				return false;
			}
		}
		else if (nTypeID == (int)Wall::GridType::EArc)
		{
			if (sqlite3_column_text(stmt, 6) != 0 && sqlite3_column_text(stmt, 7) != 0 && sqlite3_column_text(stmt, 8) != 0 && sqlite3_column_text(stmt, 9) != 0 && sqlite3_column_text(stmt, 10) != 0)
			{
				double brX = sqlite3_column_double(stmt, 6);
				double brY = sqlite3_column_double(stmt, 7);
				double beginAngle = sqlite3_column_double(stmt, 8);
				double earcAngle = sqlite3_column_double(stmt, 9);
				bool isClockwise = sqlite3_column_int(stmt, 10) == 1;

				if (LoadEArcTypeBoundary(beginX, beginY, endX, endY, brX, brY, beginAngle, earcAngle, isClockwise, vertices, nCount, prev) == false)
				{
					sqlite3_finalize(stmt);
					return false;
				}
			}
			else
			{
				sqlite3_finalize(stmt);
				return false;
			}
		}
		else
		{
			sqlite3_finalize(stmt);
			return false;
		}

		nCount++;
	}

	bool result = true;

	if (rc != SQLITE_DONE)
	{
		strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		result = false;
	}

	sqlite3_finalize(stmt);
	return result;
}

int DBManager::GetProjectCount()
{
	return (int)m_projects.size();
}

Project* DBManager::GetProject(int nIndex)
{
	if (nIndex < 0 || nIndex >= GetProjectCount())
		return 0;

	std::list<Project*>::iterator iter = m_projects.begin();

	for (int i = 0; i < nIndex; i++)
	{
		iter++;
	}

	return *iter;
}

std::map<int, FireSafetyManager::POIType*>& DBManager::GetPOITypes()
{
	return m_mapPOITypes;
}

Project* DBManager::FindProject(int nID)
{
	std::list<Project*>::iterator iter = m_projects.begin();

	for (; iter != m_projects.end(); iter++)
	{
		Project* project = *iter;

		if (project->GetID() == nID)
			return project;
	}

	return 0;
}