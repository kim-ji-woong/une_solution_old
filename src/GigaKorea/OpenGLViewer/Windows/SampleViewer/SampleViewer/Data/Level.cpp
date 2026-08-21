#include "stdafx.h"
#include "Level.h"
#include "Wall.h"
#include "Space.h"
#include "_POI.h"
#include <string.h>
#include "Column.h"
#include "AlertArea.h"

namespace FireSafetyManager
{
	Level::Level()
	{
		m_nID = -1;
		m_strName = L"";
		m_nFloorIndex = 0;
		m_dElevation = 0.0;
	}

	Level::Level(int nID, int nFloorIndex, const std::wstring& strFloorName, double dElevation)
	{
		m_nID = nID;
		m_strName = strFloorName;
		m_nFloorIndex = nFloorIndex;
		m_dElevation = dElevation;
	}

	Level::~Level()
	{
	}

	bool Level::CompareLevel(const Level* level1, const Level* level2)
	{
		return level1->m_dElevation < level2->m_dElevation;
	}

	int Level::GetID()
	{
		return m_nID;
	}

	int Level::GetFloorIndex()
	{
		return m_nFloorIndex;
	}

	double Level::GetElevation()
	{
		return m_dElevation;
	}

	void Level::SetFloorIndex(int nFloorIndex)
	{
		m_nFloorIndex = nFloorIndex;
	}

	void Level::AddWall(Wall* pWall)
	{
		m_mapWalls[pWall->GetID()] = pWall;
	}

	void Level::AddSapce(Space* pSpace)
	{
		m_mapSpaces[pSpace->GetID()] = pSpace;
	}

	void Level::AddPOI(POI* poi)
	{
		m_mapPOIs[poi->GetID()] = poi;
	}

	void Level::AddColumn(Column* pColumn)
	{
		m_mapColumns[pColumn->GetID()] = pColumn;
	}

	void Level::AddAlertArea(AlertArea* pArea)
	{
		m_mapAlertAreas[pArea->GetID()] = pArea;
	}

	Wall* Level::FindWall(int nWallID)
	{
		std::map<int, Wall*>::iterator iter = m_mapWalls.find(nWallID);

		if (iter == m_mapWalls.end())
			return 0;

		return iter->second;
	}

	Space* Level::FindSpace(int nSpaceID)
	{
		std::map<int, Space*>::iterator iter = m_mapSpaces.find(nSpaceID);

		if (iter == m_mapSpaces.end())
			return 0;

		return iter->second;
	}

	POI* Level::FindPOI(int nPOIID)
	{
		std::map<int, POI*>::iterator iter = m_mapPOIs.find(nPOIID);

		if (iter == m_mapPOIs.end())
			return 0;

		return iter->second;
	}

	Column* Level::FindColumn(int nColumnID)
	{
		std::map<int, Column*>::iterator iter = m_mapColumns.find(nColumnID);

		if (iter == m_mapColumns.end())
			return 0;

		return iter->second;
	}

	AlertArea* Level::FindAlertArea(int nAreaID)
	{
		std::map<int, AlertArea*>::iterator iter = m_mapAlertAreas.find(nAreaID);

		if (iter == m_mapAlertAreas.end())
			return 0;

		return iter->second;
	}

	int Level::GetWallCount()
	{
		return (int)m_mapWalls.size();
	}

	Wall* Level::GetWall(int nIndex)
	{
		if (nIndex < 0 || nIndex >= GetWallCount())
			return 0;

		std::map<int, Wall*>::iterator iter = m_mapWalls.begin();

		for (int i = 0; i < nIndex; i++)
		{
			iter++;
		}

		return iter->second;
	}

	int Level::GetSpaceCount()
	{
		return (int)m_mapSpaces.size();
	}

	Space* Level::GetSpace(int nIndex)
	{
		if (nIndex < 0 || nIndex >= GetSpaceCount())
			return 0;

		std::map<int, Space*>::iterator iter = m_mapSpaces.begin();

		for (int i = 0; i < nIndex; i++)
		{
			iter++;
		}

		return iter->second;
	}

	int Level::GetPOICount()
	{
		return (int)m_mapPOIs.size();
	}

	POI* Level::GetPOI(int nIndex)
	{
		if (nIndex < 0 || nIndex >= GetPOICount())
			return 0;

		std::map<int, POI*>::iterator iter = m_mapPOIs.begin();

		for (int i = 0; i < nIndex; i++)
		{
			iter++;
		}

		return iter->second;
	}

	int Level::GetColumnCount()
	{
		return (int)m_mapColumns.size();
	}

	Column* Level::GetColumn(int nIndex)
	{
		if (nIndex < 0 || nIndex >= GetColumnCount())
			return 0;

		std::map<int, Column*>::iterator iter = m_mapColumns.begin();

		for (int i = 0; i < nIndex; i++)
		{
			iter++;
		}

		return iter->second;
	}

	int Level::GetAlertAreaCount()
	{
		return (int)m_mapAlertAreas.size();
	}

	AlertArea* Level::GetAlertArea(int nIndex)
	{
		if (nIndex < 0 || nIndex >= GetAlertAreaCount())
			return 0;

		std::map<int, AlertArea*>::iterator iter = m_mapAlertAreas.begin();

		for (int i = 0; i < nIndex; i++)
		{
			iter++;
		}

		return iter->second;
	}
}
