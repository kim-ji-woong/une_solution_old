#include "stdafx.h"
#include "Project.h"
#include "Component.h"
#include "Level.h"
#include <string.h>

namespace FireSafetyManager
{
	DateTime::DateTime()
	{
		m_nYear = m_nMonth = m_nDay = m_nHour = m_nMin = m_nSec = 0;
	}

	DateTime::DateTime(int year, int month, int day, int hour, int min, int sec)
	{
		m_nYear = year;
		m_nMonth = month;
		m_nDay = day;
		m_nHour = hour;
		m_nMin = min;
		m_nSec = sec;
	}

	static bool StringToInt(const char* str, int begin, int end, int& rData)
	{
		rData = 0;

		for (int i = begin; i <= end; i++)
		{
			char ch = str[i];

			if (ch < '0' || ch > '9')
				return false;

			rData = rData * 10 + ch - '0';
		}

		return true;
	}

	bool DateTime::FromString(const char* strTime, DateTime& time)
	{
		int len = strlen(strTime);

		if (len < 19)
			return false;

		if (strTime[4] != '-' || strTime[7] != '-' || strTime[10] != ' ' ||
			strTime[13] != ':' || strTime[16] != ':')
			return false;

		if (!StringToInt(strTime, 0, 3, time.m_nYear))
			return false;
		if (!StringToInt(strTime, 5, 6, time.m_nMonth))
			return false;
		if (!StringToInt(strTime, 8, 9, time.m_nDay))
			return false;
		if (!StringToInt(strTime, 11, 12, time.m_nHour))
			return false;
		if (!StringToInt(strTime, 14, 15, time.m_nMin))
			return false;
		if (!StringToInt(strTime, 17, 18, time.m_nSec))
			return false;

		return true;
	}

	int DateTime::GetYear()
	{
		return m_nYear;
	}

	int DateTime::GetMonth()
	{
		return m_nMonth;
	}

	int DateTime::GetDay()
	{
		return m_nDay;
	}

	int DateTime::GetHour()
	{
		return m_nHour;
	}

	int DateTime::GetMinute()
	{
		return m_nMin;
	}

	int DateTime::GetSecond()
	{
		return m_nSec;
	}

	Project::Project()
	{
		m_nProjectID = -1;
		m_strProjectName = L"";
		m_unit = UnitOfLength::CM;
		m_pAnchorNode = 0;
	}

	Project::Project(int nID, const std::wstring& strProjectName, int nUnit, const DateTime& time)
	{
		m_nProjectID = nID;
		m_strProjectName = strProjectName;

		if (nUnit == (int)UnitOfLength::MM)
			m_unit = UnitOfLength::MM;
		else if (nUnit == (int)UnitOfLength::CM)
			m_unit = UnitOfLength::CM;
		else if (nUnit == (int)UnitOfLength::M)
			m_unit = UnitOfLength::M;
		else
			m_unit = UnitOfLength::CM;

		m_timeStamp = time;
	}

	Project::~Project()
	{
	}

	int Project::GetID()
	{
		return m_nProjectID;
	}

	void Project::AddComponent(Component* pComponent)
	{
		m_mapComponents[pComponent->GetID()] = pComponent;
	}

	void Project::AddLevel(Level* pLevel)
	{
		m_levels.push_back(pLevel);
	}

	Component* Project::FindComponent(int nComponentID)
	{
		std::map<int, Component*>::iterator iter = m_mapComponents.find(nComponentID);

		if (iter == m_mapComponents.end())
			return 0;

		return iter->second;
	}

	int Project::GetLevelCount()
	{
		return (int)m_levels.size();
	}

	Level* Project::GetLevel(int nIndex)
	{
		if (nIndex < 0 || nIndex >= GetLevelCount())
			return 0;

		std::list<Level*>::iterator iter = m_levels.begin();

		for (int i = 0; i < nIndex; i++)
			iter++;

		return *iter;
	}

	Level* Project::FindLevel(int nID)
	{
		std::list<Level*>::iterator iter = m_levels.begin();

		for (; iter != m_levels.end(); iter++)
		{
			Level* pLevel = *iter;

			if (pLevel->GetID() == nID)
				return pLevel;
		}

		return 0;
	}

	void Project::SetAnchorNode(AnchorNode* pAnchorNode)
	{
		m_pAnchorNode = pAnchorNode;
	}

	AnchorNode* Project::GetAnchorNode()
	{
		return m_pAnchorNode;
	}

	static const double _PI = 3.14159265358979323846;

	AnchorNode::AnchorNode(const Vertex2D& vGlobal, const Vertex2D& vLocal, double dLocalAngle, Project::UnitOfLength globalUnitOfLength, Project::UnitOfLength localUnitOfLength)
	{
		m_vGlobal = vGlobal;
		m_vLocal = vLocal;
		m_dDegree = dLocalAngle;
		m_dRadian = m_dDegree * _PI / 180.0;
		m_globalUnitOfLength = globalUnitOfLength;
		m_localUnitOfLength = localUnitOfLength;
		m_dScale = GetScale();
	}

	static double GetAngle(const Vertex2D& v1, Vertex2D& vCenter, Vertex2D& v2)
	{
		// 코사인 제2법칙
		// C²= A²+ B²- 2ABcosΘ
		double a = vCenter.GetDistance(v1);
		double b = v2.GetDistance(vCenter);
		double c = v2.GetDistance(v1);

		double cosData = (a * a + b * b - c * c) / 2 / a / b;
		if (cosData < -1.0) cosData = -1.0;
		else if (cosData > 1.0) cosData = 1.0;

		return acos(cosData);
	}

	Vertex2D AnchorNode::LocalToGlobal(double x, double y)
	{
		double xMove = x - m_vLocal.x;
		double yMove = y - m_vLocal.y;

		double globalX = m_vGlobal.x + xMove * m_dScale;
		double globalY = m_vGlobal.y + yMove * m_dScale;
		Vertex2D vTemp(globalX, globalY);

		double radius = m_vGlobal.GetDistance(vTemp);

		if (radius < 0.001)
			return m_vGlobal;

		Vertex2D vNorth(m_vGlobal.x, m_vGlobal.y + 100.0);
		double angle = GetAngle(vNorth, m_vGlobal, vTemp);

		if (xMove < 0.0)
			angle = _PI * 2 - angle;

		angle += m_dRadian;

		globalX = m_vGlobal.x + radius * sin(angle);
		globalY = m_vGlobal.y + radius * cos(angle);
		return Vertex2D(globalX, globalY);
	}

	double AnchorNode::GetScale()
	{
		if (m_localUnitOfLength == Project::UnitOfLength::MM)
		{
			if (m_globalUnitOfLength == Project::UnitOfLength::MM)
				return 1.0;
			else if (m_globalUnitOfLength == Project::UnitOfLength::CM)
				return 0.1;
			else if (m_globalUnitOfLength == Project::UnitOfLength::M)
				return 0.001;
		}
		else if (m_localUnitOfLength == Project::UnitOfLength::CM)
		{
			if (m_globalUnitOfLength == Project::UnitOfLength::MM)
				return 10.0;
			else if (m_globalUnitOfLength == Project::UnitOfLength::CM)
				return 1.0;
			else if (m_globalUnitOfLength == Project::UnitOfLength::M)
				return 0.01;
		}
		else if (m_localUnitOfLength == Project::UnitOfLength::M)
		{
			if (m_globalUnitOfLength == Project::UnitOfLength::MM)
				return 1000.0;
			else if (m_globalUnitOfLength == Project::UnitOfLength::CM)
				return 100.0;
			else if (m_globalUnitOfLength == Project::UnitOfLength::M)
				return 1.0;
		}

		return 1.0;
	}
}
