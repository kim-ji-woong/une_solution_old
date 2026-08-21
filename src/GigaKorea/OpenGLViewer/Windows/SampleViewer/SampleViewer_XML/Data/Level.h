#pragma once
#include <map>
#include <list>
#include <string>

namespace FireSafetyManager
{
	class Wall;
	class Space;
	class POI;
	class Column;
	class AlertArea;

	class Level
	{
	public:
		Level();
		Level(int nID, int nFloorIndex, const std::wstring& strFloorName, double dElevation);
		virtual ~Level();

	public:
		static bool CompareLevel(const Level* level1, const Level* level2);

	public:
		int GetID();
		int GetFloorIndex();
		double GetElevation();

		void SetFloorIndex(int nFloorIndex);

		void AddWall(Wall* pWall);
		void AddSapce(Space* pSpace);
		void AddPOI(POI* poi);
		void AddColumn(Column* pColumn);
		void AddAlertArea(AlertArea* pArea);

		Wall* FindWall(int nWallID);
		Space* FindSpace(int nSpaceID);
		POI* FindPOI(int nPOIID);
		Column* FindColumn(int nColumnID);
		AlertArea* FindAlertArea(int nAreaID);

		int GetWallCount();
		Wall* GetWall(int nIndex);
		int GetSpaceCount();
		Space* GetSpace(int nIndex);
		int GetPOICount();
		POI* GetPOI(int nIndex);
		int GetColumnCount();
		Column* GetColumn(int nIndex);
		int GetAlertAreaCount();
		AlertArea* GetAlertArea(int nIndex);

	private:
		int m_nID;
		int m_nFloorIndex;
		std::wstring m_strName;
		double m_dElevation;

		std::map<int, Wall*> m_mapWalls;
		std::map<int, Space*> m_mapSpaces;
		std::map<int, POI*> m_mapPOIs;
		std::map<int, Column*> m_mapColumns;
		std::map<int, AlertArea*> m_mapAlertAreas;
	};
}
