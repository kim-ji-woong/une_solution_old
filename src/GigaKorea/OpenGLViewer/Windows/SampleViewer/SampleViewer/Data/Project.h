#pragma once
#include <map>
#include <list>
#include <string>
#include "Vertex2D.h"

using namespace VectorGraphics;

namespace FireSafetyManager
{
	class Component;
	class Level;

	class DateTime
	{
	public:
		DateTime();
		DateTime(int year, int month, int day, int hour, int min, int sec);

	public:
		static bool FromString(const char* strTime, DateTime& time);

	public:
		int GetYear();
		int GetMonth();
		int GetDay();
		int GetHour();
		int GetMinute();
		int GetSecond();

	private:
		int m_nYear, m_nMonth, m_nDay, m_nHour, m_nMin, m_nSec;
	};

	class AnchorNode;

	class Project
	{
	public:
		enum UnitOfLength { MM = 0, CM, M };

	public:
		Project();
		Project(int nID, const std::wstring& strProjectName, int nUnit, const DateTime& time);
		virtual ~Project();

	public:
		int GetID();
		void AddComponent(Component* pComponent);
		void AddLevel(Level* pLevel);

		int GetLevelCount();
		Level* GetLevel(int nIndex);
		Level* FindLevel(int nID);

		Component* FindComponent(int nComponentID);

		void SetAnchorNode(AnchorNode* pAnchorNode);
		AnchorNode* GetAnchorNode();

	private:
		int m_nProjectID;
		std::wstring m_strProjectName;
		UnitOfLength m_unit;
		DateTime m_timeStamp;
		// 층정보
		std::list<Level*> m_levels;
		// 벽체 재질
		// Key : Component ID
		std::map<int, Component*> m_mapComponents;
		AnchorNode* m_pAnchorNode;
	};

	class AnchorNode
	{
	public:
		AnchorNode(const Vertex2D& vGlobal, const Vertex2D& vLocal, double dLocalAngle, Project::UnitOfLength globalUnitOfLength, Project::UnitOfLength localUnitOfLength);

	public:
		Vertex2D LocalToGlobal(double x, double y);

	private:
		double GetScale();

	private:
		Vertex2D m_vGlobal;
		Vertex2D m_vLocal;
		// 방위각(Degree)
		double m_dDegree;
		double m_dRadian;
		Project::UnitOfLength m_globalUnitOfLength = Project::UnitOfLength::M;
		Project::UnitOfLength m_localUnitOfLength = Project::UnitOfLength::MM;
		double m_dScale;
	};
}
