#include "stdafx.h"
#include "XMLManager.h"
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
#include "EasyXML/EasyXML2.h"
#include "UnEUtility/StringManager.h"
#include "Data/AlertArea.h"

using namespace FireSafetyManager;
using namespace VectorGraphics;
using namespace UnE;

std::map<std::wstring, int> g_mapPOITypes;
std::map<int, Project*> g_mapProjects;
std::map<std::wstring, Component*> g_mapComponents;
std::map<std::wstring, Level*> g_mapLevels;
std::map<std::wstring, Wall*> g_mapWalls;
std::map<std::wstring, Space*> g_mapSpaces;
std::map<std::wstring, AlertArea*> g_mapAlertAreas;

int g_nPOITypeID = 1;
int g_nProjectID = 1;
int g_nComponentID = 1;
int g_nLevelID = 1;
int g_nWallID = 1;
int g_nSpaceID = 1;
int g_nAlertAreaID = 1;

XMLManager::XMLManager()
{
	m_strError = L"";
}

XMLManager::~XMLManager()
{
}

bool XMLManager::LoadXML(std::wstring strPath, char* strPOIFolder)
{
	EasyXML2 xml;

	if (xml.OpenXMLFile(strPath.c_str(), true) == false)
	{
		wchar_t strError[256];
		xml.GetErrorMessage(strError);
		m_strError = strError;
		return false;
	}

	if (LoadPOITypes(&xml, strPOIFolder))
	{
		Project* project = LoadProjects(&xml);

		if (project != 0)
		{
			LoadLevels(&xml, project);
		}
	}

	return true;
}

static std::wstring UTF8ToANSI(const char *utf8str)
{
	std::wstring_convert<std::codecvt_utf8<wchar_t>> wconv;
	std::wstring wstr = wconv.from_bytes(utf8str);
	return wstr;
}

bool CompareWString(wchar_t* str1, wchar_t* str2, bool ignoreCase)
{
	int len1 = (int)wcslen(str1);
	int len2 = (int)wcslen(str2);

	if (len1 != len2)
		return false;

	for (int i = 0; i < len1; i++)
	{
		wchar_t ch1 = str1[i];
		wchar_t ch2 = str2[i];

		if (ignoreCase)
		{
			if (ch1 >= 'A' && ch1 <= 'Z')
				ch1 = (wchar_t)(ch1 + ('a' - 'A'));

			if (ch2 >= 'A' && ch2 <= 'Z')
				ch2 = (wchar_t)(ch2 + ('a' - 'A'));
		}

		if (ch1 != ch2)
			return false;
	}

	return true;
}

void ConvertWCtoC(wchar_t* src, char* trg)
{
	//입력받은 wchar_t 변수의 길이를 구함
	int strSize = WideCharToMultiByte(CP_ACP, 0, src, -1, NULL, 0, NULL, NULL);
	
	//형 변환 
	WideCharToMultiByte(CP_ACP, 0, src, -1, trg, strSize, 0, 0);
}

bool LoadPOIType(EasyXML2* pXML, char* strPOIFolder, DWORD_PTR node, bool isGroup, std::map<int, FireSafetyManager::POIType*>& rMapPOITypes)
{
	DWORD_PTR child = pXML->GetChildNode(node);
	wchar_t str[256];
	EasyXML2::DataType type;

	std::wstring strID = L"";
	std::wstring strName = L"";
	std::wstring strCode = L"";

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ATTRIBUTE)
		{
			if (CompareWString(str, L"id", true))
			{
				if (pXML->GetChildNodeData(child, str) == false)
					return false;

				strID = str;
			}
			else if (CompareWString(str, L"name", true))
			{
				if (pXML->GetChildNodeData(child, str) == false)
					return false;

				strName = str;
			}
			else if (CompareWString(str, L"code", true))
			{
				if (pXML->GetChildNodeData(child, str) == false)
					return false;

				strCode = str;
			}
		}
		else
			break;

		child = pXML->GetNextNode(child);
	} while (child != 0);

	if (strID == L"" || strName == L"")
		return false;

	g_mapPOITypes[strID] = g_nPOITypeID;

	POIType* poiType = new POIType(g_nPOITypeID++, strName, strCode, 0);

	if (strCode != L"")
	{
		char code[256];
		ConvertWCtoC((wchar_t*)strCode.c_str(), code);

		char strPOIPath[512];
		sprintf_s(strPOIPath, 512, "%s\\%s.poi", strPOIFolder, code);
		poiType->LoadPOIIcon(strPOIPath);
	}

	rMapPOITypes[poiType->GetID()] = poiType;
	return true;
}

bool LoadPOITypes(EasyXML2* pXML, char* strPOIFolder, DWORD_PTR node, std::map<int, FireSafetyManager::POIType*>& rMapPOITypes)
{
	DWORD_PTR child = pXML->GetChildNode(node);
	wchar_t str[256];
	EasyXML2::DataType type;

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ELEMENT)
		{
			if (CompareWString(str, L"POITypeGroup", true))
			{
				LoadPOIType(pXML, strPOIFolder, child, true, rMapPOITypes);
			}
			else if (CompareWString(str, L"POIType", true))
			{
				LoadPOIType(pXML, strPOIFolder, child, false, rMapPOITypes);
			}
		}

		LoadPOITypes(pXML, strPOIFolder, child, rMapPOITypes);
		child = pXML->GetNextNode(child);
	} while (child != 0);

	return true;
}

bool XMLManager::LoadPOITypes(void* xml, char* strPOIFolder)
{
	EasyXML2* pXML = (EasyXML2*)xml;

	DWORD_PTR root = pXML->GetRootNode();

	if (root == 0)
		return false;

	DWORD_PTR child = pXML->GetChildNode(root);
	wchar_t str[256];
	EasyXML2::DataType type;
	DWORD_PTR commonNode = 0, poiTypesNode = 0;

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ELEMENT && CompareWString(str, L"Common", true))
		{
			commonNode = child;
			break;
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);

	if (commonNode == 0)
		return false;

	child = pXML->GetChildNode(commonNode);

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ELEMENT && CompareWString(str, L"POITypes", true))
		{
			poiTypesNode = child;
			break;
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);

	if (poiTypesNode == 0)
		return false;

	return ::LoadPOITypes(pXML, strPOIFolder, poiTypesNode, m_mapPOITypes);
}

bool StringToVertex2D(wchar_t* str, Vertex2D& rVertex)
{
	int len = (int)wcslen(str);
	int endIndex = -1, beginIndex = -1, commaIndex = -1;

	for (int i = 0; i < len; i++)
	{
		if (str[i] == ',')
		{
			commaIndex = i;
			break;
		}
	}

	if (commaIndex < 0)
		return false;

	for (int i = commaIndex - 1; i >= 0; i--)
	{
		if (str[i] >= '0' && str[i] <= '9')
		{
			endIndex = i + 1;
			break;
		}
	}

	for (int i = commaIndex + 1; i < len; i++)
	{
		if ((str[i] >= '0' && str[i] <= '9') || str[i] == '-' || str[i] == '+')
		{
			beginIndex = i;
			break;
		}
	}

	if (endIndex < 0 || beginIndex < 0)
		return false;

	double x, y;

	if (UnE::Utility::StringManager::StrToDouble(str, &x, 0, (unsigned int)endIndex) == false)
		return false;
	if (UnE::Utility::StringManager::StrToDouble(&str[beginIndex], &y) == false)
		return false;

	rVertex.x = x;
	rVertex.y = y;
	return true;
}

bool LoadPos(EasyXML2* pXML, DWORD_PTR node, Vertex2D& rVertex, std::map<std::wstring, std::wstring>* pMap)
{
	DWORD_PTR child = pXML->GetChildNode(node);

	if (child == 0)
		return false;

	wchar_t str[256];
	EasyXML2::DataType type;

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ATTRIBUTE)
		{
			if (pMap != 0)
			{
				std::wstring strAttrName = str;

				if (pXML->GetChildNodeData(child, str) == false)
					return false;

				std::wstring strAttrValue = str;
				(*pMap)[strAttrName] = strAttrValue;
			}
		}
		else if (type == EasyXML2::DataType::ELEMENT && CompareWString(str, L"Pos", true))
		{
			if (pXML->GetChildNodeData(child, str) == false)
				return false;

			if (StringToVertex2D(str, rVertex) == false)
				return false;
			else
				return true;
		}
		else
			break;

		child = pXML->GetNextNode(child);
	} while (child != 0);

	return false;
}

bool LoadDouble(EasyXML2* pXML, DWORD_PTR node, std::wstring strElementName, double& rData)
{
	DWORD_PTR child = pXML->GetChildNode(node);

	if (child == 0)
		return false;

	wchar_t str[256];
	EasyXML2::DataType type;

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ELEMENT && CompareWString(str, (wchar_t*)strElementName.c_str(), true))
		{
			if (pXML->GetChildNodeData(child, str) == false)
				return false;

			if (UnE::Utility::StringManager::StrToDouble(str, &rData) == false)
				return false;
			else
				return true;
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);

	return false;
}

bool StringToUnit(std::wstring str, Project::UnitOfLength& unit)
{
	if (str == L"cm")
		unit = Project::UnitOfLength::CM;
	else if (str == L"mm")
		unit = Project::UnitOfLength::MM;
	else if (str == L"meter")
		unit = Project::UnitOfLength::M;
	else
		return false;

	return true;
}

bool LoadAnchorNode(EasyXML2* pXML, DWORD_PTR node, Vertex2D& vGlobal, Vertex2D& vLocal, double& dAngle, Project::UnitOfLength& globalUnit)
{
	DWORD_PTR child = pXML->GetChildNode(node);

	if (child == 0)
		return false;

	wchar_t str[256];
	EasyXML2::DataType type;
	bool readGlobal = false, readLocal = false;

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ELEMENT && CompareWString(str, L"Global", true))
		{
			std::map<std::wstring, std::wstring> mapAttrs;

			if (LoadPos(pXML, child, vGlobal, &mapAttrs) == false)
				return false;

			std::map<std::wstring, std::wstring>::iterator iter = mapAttrs.find(L"unit");

			if (iter == mapAttrs.end())
				return false;

			if (StringToUnit(iter->second, globalUnit) == false)
				return false;

			readGlobal = true;
		}
		else if (type == EasyXML2::DataType::ELEMENT && CompareWString(str, L"Local", true))
		{
			if (LoadPos(pXML, child, vLocal, 0) == false)
				return false;

			if (LoadDouble(pXML, child, L"Angle", dAngle) == false)
				return false;

			readLocal = true;
		}
		else
			break;

		child = pXML->GetNextNode(child);
	} while (child != 0);

	return readGlobal && readLocal;
}

Project* XMLManager::LoadProjects(void* xml)
{
	EasyXML2* pXML = (EasyXML2*)xml;

	DWORD_PTR root = pXML->GetRootNode();

	if (root == 0)
		return 0;

	DWORD_PTR child = pXML->GetChildNode(root);
	wchar_t str[256];
	EasyXML2::DataType type;
	DWORD_PTR projectNode = 0;

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return 0;

		if (type == EasyXML2::DataType::ELEMENT && CompareWString(str, L"ProjectInfo", true))
		{
			projectNode = child;
			break;
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);

	if (projectNode == 0)
		return 0;

	std::wstring strName = L"";
	std::wstring strUnit = L"";
	std::wstring strDateTime = L"";

	child = pXML->GetChildNode(projectNode);

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return 0;

		if (type == EasyXML2::DataType::ATTRIBUTE && CompareWString(str, L"name", true))
		{
			if (pXML->GetChildNodeData(child, str) == false)
				return 0;

			strName = str;
		}
		else if (type == EasyXML2::DataType::ATTRIBUTE && CompareWString(str, L"unit", true))
		{
			if (pXML->GetChildNodeData(child, str) == false)
				return 0;

			strUnit = str;
		}
		else if (type == EasyXML2::DataType::ATTRIBUTE && CompareWString(str, L"datetime", true))
		{
			if (pXML->GetChildNodeData(child, str) == false)
				return 0;

			strDateTime = str;
		}
		else if (type == EasyXML2::DataType::ELEMENT)
		{
			break;
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);

	if (strName == L"" || strUnit == L"" || strDateTime == L"")
		return 0;

	Vertex2D vGlobal, vLocal;
	double dAngle;
	bool readAnchorNode = false;
	Project::UnitOfLength globalUnit, localUnit;

	if (StringToUnit(strUnit, localUnit) == false)
		return 0;

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			break;

		if (type == EasyXML2::DataType::ELEMENT && CompareWString(str, L"AnchorNode", true))
		{
			readAnchorNode = LoadAnchorNode(pXML, child, vGlobal, vLocal, dAngle, globalUnit);
		}
		else if (type == EasyXML2::DataType::ELEMENT && CompareWString(str, L"ProjectProperties", true))
		{
			child = pXML->GetNextNode(child);
			continue;
		}
		else
			break;

		child = pXML->GetNextNode(child);
	} while (child != 0);

	char strTime[256];
	::ConvertWCtoC((wchar_t*)strDateTime.c_str(), strTime);
	
	DateTime timeStamp;
	Project* project = 0;

	if (DateTime::FromString(strTime, timeStamp))
	{
		project = new Project(g_nProjectID++, strName, (int)localUnit, timeStamp);
		m_projects.push_back(project);

		g_mapProjects[project->GetID()] = project;

		if (readAnchorNode)
		{
			AnchorNode* pAnchorNode = new AnchorNode(vGlobal, vLocal, dAngle, globalUnit, localUnit);
			project->SetAnchorNode(pAnchorNode);
		}
	}


	for (std::list<Project*>::iterator iter = m_projects.begin(); iter != m_projects.end(); iter++)
	{
		if (LoadProjectComponent(pXML, *iter) == false)
			return 0;
	}

	return project;
}

void LoadAttribs(EasyXML2* pXML, DWORD_PTR node, std::map<std::wstring, std::wstring>* pMap)
{
	DWORD_PTR child = pXML->GetChildNode(node);

	if (child == 0)
		return;

	wchar_t str[256];
	EasyXML2::DataType type;

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return;

		if (type == EasyXML2::DataType::ATTRIBUTE)
		{
			if (pMap != 0)
			{
				std::wstring strAttrName = str;

				if (pXML->GetChildNodeData(child, str) == false)
					return;

				std::wstring strAttrValue = str;
				(*pMap)[strAttrName] = strAttrValue;
			}
		}
		else
			break;

		child = pXML->GetNextNode(child);
	} while (child != 0);
}

void LoadText(EasyXML2* pXML, DWORD_PTR node, std::wstring& strValue)
{
	DWORD_PTR child = pXML->GetChildNode(node);

	if (child == 0)
		return;

	wchar_t str[256];
	EasyXML2::DataType type;

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return;

		if (type == EasyXML2::DataType::TEXT)
		{
			strValue = str;
			return;
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);
}

bool XMLManager::LoadProjectComponent(void* xml, Project* project)
{
	EasyXML2* pXML = (EasyXML2*)xml;

	DWORD_PTR root = pXML->GetRootNode();

	if (root == 0)
		return false;

	DWORD_PTR child = pXML->GetChildNode(root);
	wchar_t str[256];
	EasyXML2::DataType type;
	DWORD_PTR commonNode = 0, poiTypesNode = 0;

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ELEMENT && CompareWString(str, L"Common", true))
		{
			commonNode = child;
			break;
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);

	if (commonNode == 0)
		return false;

	child = pXML->GetChildNode(commonNode);
	child = pXML->GetChildNode(child);

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ELEMENT && CompareWString(str, L"Component", true))
		{
			std::map<std::wstring, std::wstring> mapAttrs;
			LoadAttribs(pXML, child, &mapAttrs);

			std::wstring strComponentName = L"";
			LoadText(pXML, child, strComponentName);
			
			std::map<std::wstring, std::wstring>::iterator iterID = mapAttrs.find(L"id");
			std::map<std::wstring, std::wstring>::iterator iterType = mapAttrs.find(L"type");

			Component* pComponent = new Component(g_nComponentID++, iterType->second, strComponentName);
			project->AddComponent(pComponent);
			g_mapComponents[iterID->second] = pComponent;
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);

	return true;
}

Line2D* LoadLine(EasyXML2* pXML, DWORD_PTR node)
{
	wchar_t str[256];
	EasyXML2::DataType type;
	DWORD_PTR child = pXML->GetChildNode(node);

	std::vector<Vertex2D> vertices;

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ELEMENT)
		{
			if (CompareWString(str, L"Pos", true))
			{
				std::wstring strVertex;
				LoadText(pXML, child, strVertex);

				Vertex2D vertex;

				if (StringToVertex2D((wchar_t*)strVertex.c_str(), vertex) == false)
					return 0;
				else
					vertices.push_back(vertex);
			}
			else
				break;
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);

	if (vertices.size() != 2)
		return 0;

	Line2D* pLine = new Line2D(vertices[0], vertices[1]);
	return pLine;
}

Line2D* LoadGrid(EasyXML2* pXML, DWORD_PTR node, std::map<std::wstring, Line2D*>& rMapGrids)
{
	wchar_t str[256];
	EasyXML2::DataType type;

	std::map<std::wstring, std::wstring> mapAttrs;
	LoadAttribs(pXML, node, &mapAttrs);

	std::map<std::wstring, std::wstring>::iterator iter = mapAttrs.find(L"id");

	if (iter == mapAttrs.end())
		return 0;

	DWORD_PTR child = pXML->GetChildNode(node);
	Line2D* pLine = 0;

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ELEMENT)
		{
			if (CompareWString(str, L"Line", true))
			{
				pLine = LoadLine(pXML, child);
			}
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);

	if (pLine == 0)
		return 0;

	rMapGrids[iter->second] = pLine;
	return pLine;
}

std::map<std::wstring, Line2D*>* LoadGrids(EasyXML2* pXML, DWORD_PTR node)
{
	std::map<std::wstring, Line2D*>* pMapGrid = new std::map<std::wstring, Line2D*>();

	wchar_t str[256];
	EasyXML2::DataType type;
	DWORD_PTR child = pXML->GetChildNode(node);

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ELEMENT)
		{
			if (CompareWString(str, L"Grid", true))
			{
				if (LoadGrid(pXML, child, *pMapGrid) == 0)
					return 0;
			}
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);

	return pMapGrid;
}

bool XMLManager::LoadLevel(void* xml, DWORD_PTR node, Project* project)
{
	EasyXML2* pXML = (EasyXML2*)xml;

	std::map<std::wstring, std::wstring> mapAttrs;
	LoadAttribs(pXML, node, &mapAttrs);

	std::map<std::wstring, std::wstring>::iterator iter = mapAttrs.find(L"id");

	if (iter == mapAttrs.end())
		return false;

	DWORD_PTR child = pXML->GetChildNode(node);
	wchar_t str[256];
	EasyXML2::DataType type;

	std::wstring strName = L"";
	std::wstring strElevation = L"";

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ELEMENT)
		{
			if (CompareWString(str, L"Name", true))
			{
				LoadText(pXML, child, strName);

				if (strElevation.length() > 0)
					break;
			}
			else if (CompareWString(str, L"Elevation", true))
			{
				LoadText(pXML, child, strElevation);

				if (strName.length() > 0)
					break;
			}
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);

	if (strName.length() == 0 || strElevation.length() == 0)
		return false;

	double dElevation;

	if (UnE::Utility::StringManager::StrToDouble(strElevation.c_str(), &dElevation) == false)
		return false;

	Level* pLevel = new Level(g_nLevelID++, -1, strName, dElevation);
	g_mapLevels[iter->second] = pLevel;
	project->AddLevel(pLevel);

	child = pXML->GetChildNode(node);
	std::map<std::wstring, Line2D*>* pMapGrids = 0;

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ELEMENT)
		{
			if (CompareWString(str, L"GridCollection", true))
			{
				pMapGrids = LoadGrids(pXML, child);

				if (pMapGrids == 0)
					return false;
			}
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);

	if (pMapGrids == 0)
		return false;

	child = pXML->GetChildNode(node);
	DWORD_PTR elementCollectionNode = 0;

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ELEMENT)
		{
			if (CompareWString(str, L"ElementCollection", true))
			{
				elementCollectionNode = child;
				break;
			}
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);

	if (elementCollectionNode == 0)
		return false;

	child = pXML->GetChildNode(elementCollectionNode);

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ELEMENT)
		{
			if (CompareWString(str, L"Wall", true))
			{
				LoadWall(pXML, child, pLevel, project, *pMapGrids);
			}
			else if (CompareWString(str, L"Space", true))
			{
				LoadSpace(pXML, child, pLevel, project);
			}
			else if (CompareWString(str, L"AlertArea", true))
			{
				LoadAlertAreas(pXML, child, pLevel);
			}
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);
}

bool XMLManager::LoadLevels(void* xml, Project* project)
{
	EasyXML2* pXML = (EasyXML2*)xml;

	DWORD_PTR root = pXML->GetRootNode();

	if (root == 0)
		return false;

	DWORD_PTR child = pXML->GetChildNode(root);
	wchar_t str[256];
	EasyXML2::DataType type;
	DWORD_PTR levelsNode = 0;

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ELEMENT && CompareWString(str, L"Levels", true))
		{
			levelsNode = child;
			break;
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);

	if (levelsNode == 0)
		return false;

	child = pXML->GetChildNode(levelsNode);

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ELEMENT && CompareWString(str, L"Level", true))
		{
			if (LoadLevel(pXML, child, project) == false)
				return false;
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);

	return true;
}

bool LoadBoundary(EasyXML2* pXML, DWORD_PTR node, std::vector<Vertex2D>& vertices)
{
	DWORD_PTR child = pXML->GetChildNode(node);

	wchar_t str[256];
	EasyXML2::DataType type;
	Vertex2D vBegin;

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ELEMENT)
		{
			if (CompareWString(str, L"Line", true))
			{
				Line2D* pLine = LoadLine(pXML, child);

				if (pLine == 0)
					return false;

				int nVertexCount = (int)vertices.size();

				if (nVertexCount == 0)
				{
					vBegin = pLine->GetVertex(true);
					vertices.push_back(pLine->GetVertex(true));
					vertices.push_back(pLine->GetVertex(false));
				}
				else
				{
					Vertex2D& rPrev = vertices[nVertexCount - 1];

					const Vertex2D& rBegin = pLine->GetVertex(true);
					const Vertex2D& rEnd = pLine->GetVertex(false);

					double len1 = rPrev.GetDistance(rBegin);
					double len2 = rPrev.GetDistance(rEnd);

					Vertex2D vertex = len1 < len2 ? rEnd : rBegin;

					if (vBegin.GetDistance(vertex) > 0.1)
						vertices.push_back(vertex);
				}

				delete pLine;
			}
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);

	return vertices.size() >= 3;
}

bool XMLManager::LoadWall(void* xml, DWORD_PTR node, Level* pLevel, Project* project, std::map<std::wstring, Line2D*>& rMapGrid)
{
	EasyXML2* pXML = (EasyXML2*)xml;

	DWORD_PTR child = pXML->GetChildNode(node);

	if (child == 0)
		return false;

	wchar_t str[256];
	EasyXML2::DataType type;

	std::wstring strWallID = L"";
	std::wstring strGridID = L"";
	std::wstring strComponentID = L"";
	std::wstring strThick = L"";
	std::wstring strHeight =  L"";
	std::vector<Vertex2D> boundaryVertex;

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ATTRIBUTE)
		{
			if (CompareWString(str, L"id", true))
			{
				if (pXML->GetChildNodeData(child, str) == false)
					return false;

				strWallID = str;
			}
			else if (CompareWString(str, L"grid", true))
			{
				if (pXML->GetChildNodeData(child, str) == false)
					return false;

				strGridID = str;
			}
			else if (CompareWString(str, L"component", true))
			{
				if (pXML->GetChildNodeData(child, str) == false)
					return false;

				strComponentID = str;
			}
		}
		else if (type == EasyXML2::DataType::ELEMENT)
		{
			if (CompareWString(str, L"Thickness", true))
			{
				if (pXML->GetChildNodeData(child, str) == false)
					return false;

				strThick = str;
			}
			else if (CompareWString(str, L"Height", true))
			{
				if (pXML->GetChildNodeData(child, str) == false)
					return false;

				strHeight = str;
			}
			else if (CompareWString(str, L"Boundary", true))
			{
				if (LoadBoundary(pXML, child, boundaryVertex) == false)
					boundaryVertex.clear();
			}
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);

	if (strWallID.length() == 0 || strGridID.length() == 0 || strComponentID.length() == 0 ||
		strThick.length() == 0 || strHeight.length() == 0)
		return false;

	std::map<std::wstring, Line2D*>::iterator iterGrid = rMapGrid.find(strGridID);

	if (iterGrid == rMapGrid.end())
		return false;

	std::map<std::wstring, Component*>::iterator iterComponent = g_mapComponents.find(strComponentID);

	if (iterComponent == g_mapComponents.end())
		return false;

	double dThick, dHeight;

	if (UnE::Utility::StringManager::StrToDouble(strThick.c_str(), &dThick) == false ||
		UnE::Utility::StringManager::StrToDouble(strHeight.c_str(), &dHeight) == false)
		return false;

	Wall* pWall = new Wall(g_nWallID++, dThick, dHeight, iterComponent->second, iterGrid->second);
	pLevel->AddWall(pWall);
	g_mapWalls[strWallID] = pWall;

	for (std::vector<Vertex2D>::iterator _iter = boundaryVertex.begin(); _iter != boundaryVertex.end(); _iter++)
	{
		pWall->AddBoundaryVertex(*_iter);
	}

	return true;
}

Wall* LoadLinkedWall(EasyXML2* pXML, DWORD_PTR node)
{
	DWORD_PTR child = pXML->GetChildNode(node);

	if (child == 0)
		return false;

	wchar_t str[256];
	EasyXML2::DataType type;

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ATTRIBUTE)
		{
			if (CompareWString(str, L"link", true))
			{
				if (pXML->GetChildNodeData(child, str) == false)
					return false;

				std::wstring strWallID = str;

				std::map<std::wstring, Wall*>::iterator iter = g_mapWalls.find(strWallID);

				if (iter == g_mapWalls.end())
					return 0;

				return iter->second;
			}
		}
		else
			break;

		child = pXML->GetNextNode(child);
	} while (child != 0);

	return 0;
}

bool XMLManager::LoadAlertAreas(void* xml, DWORD_PTR node, FireSafetyManager::Level* pLevel)
{
	EasyXML2* pXML = (EasyXML2*)xml;

	DWORD_PTR child = pXML->GetChildNode(node);

	if (child == 0)
		return false;

	wchar_t str[256];
	EasyXML2::DataType type;

	std::wstring strAreaID = L"";
	std::wstring strAreaName = L"";
	std::vector<Vertex2D> boundaryVertex;

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ATTRIBUTE)
		{
			if (CompareWString(str, L"id", true))
			{
				if (pXML->GetChildNodeData(child, str) == false)
					return false;

				strAreaID = str;
			}
			else if (CompareWString(str, L"name", true))
			{
				if (pXML->GetChildNodeData(child, str) == false)
					return false;

				strAreaName = str;
			}
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);

	AlertArea* area = new AlertArea();
	g_mapAlertAreas[strAreaID] = area;
	area->SetID(g_nAlertAreaID++);
	area->SetName(strAreaName);
	pLevel->AddAlertArea(area);

	child = pXML->GetChildNode(node);

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ELEMENT)
		{
			if (CompareWString(str, L"Boundary", true))
			{
				if (LoadBoundary(pXML, child, boundaryVertex) == false)
					return false;
			}
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);

	if (boundaryVertex.size() < 3)
		return false;

	for (std::vector<Vertex2D>::iterator _iter = boundaryVertex.begin(); _iter != boundaryVertex.end(); _iter++)
	{
		area->AddBoundaryVertex(*_iter);
	}

	return true;
}

bool XMLManager::LoadSpace(void* xml, DWORD_PTR node, FireSafetyManager::Level* pLevel, FireSafetyManager::Project* project)
{
	EasyXML2* pXML = (EasyXML2*)xml;

	DWORD_PTR child = pXML->GetChildNode(node);

	if (child == 0)
		return false;

	wchar_t str[256];
	EasyXML2::DataType type;

	std::wstring strSpaceID = L"";
	std::wstring strSpaceName = L"";
	std::vector<Vertex2D> boundaryVertex;

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ATTRIBUTE)
		{
			if (CompareWString(str, L"id", true))
			{
				if (pXML->GetChildNodeData(child, str) == false)
					return false;

				strSpaceID = str;
			}
			else if (CompareWString(str, L"name", true))
			{
				if (pXML->GetChildNodeData(child, str) == false)
					return false;

				strSpaceName = str;
			}
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);

	Space* pSpace = new Space(g_nSpaceID++, strSpaceName);
	g_mapSpaces[strSpaceID] = pSpace;
	pLevel->AddSapce(pSpace);

	child = pXML->GetChildNode(node);

	do
	{
		if (pXML->GetNodeData(child, str, &type) == false)
			return false;

		if (type == EasyXML2::DataType::ELEMENT)
		{
			if (CompareWString(str, L"LinkedWall", true))
			{
				Wall* pWall = LoadLinkedWall(pXML, child);

				if (pWall == 0)
					return false;
				else
					pSpace->AddWall(pWall);
			}
			else if (CompareWString(str, L"Boundary", true))
			{
				if (LoadBoundary(pXML, child, boundaryVertex) == false)
					return false;
			}
		}

		child = pXML->GetNextNode(child);
	} while (child != 0);

	if (boundaryVertex.size() < 3)
		return false;

	for (std::vector<Vertex2D>::iterator _iter = boundaryVertex.begin(); _iter != boundaryVertex.end(); _iter++)
	{
		pSpace->AddBoundaryVertex(*_iter);
	}

	return true;
}

bool XMLManager::LoadDoors(void* db, Level* pLevel, Project* project)
{
	char strSQL[256];
	sprintf_s(strSQL, 256, "Select ID, WallID, X, Y, Width, Height, Elevation, DoorType, Hinge1X, Hinge1Y, Hinge2X, Hinge2Y from Door where LevelID = %d", pLevel->GetID());

	sqlite3* _db = (sqlite3*)db;
	sqlite3_stmt *stmt;

	int rc = sqlite3_prepare_v2(_db, strSQL, -1, &stmt, NULL);
	if (rc != SQLITE_OK)
	{
		//strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
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
		//strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		result = false;
	}

	sqlite3_finalize(stmt);
	return true;
}

bool XMLManager::LoadWindows(void* db, Level* pLevel, Project* project)
{
	char strSQL[256];
	sprintf_s(strSQL, 256, "Select ID, WallID, X, Y, Width, Height, Elevation from Window where LevelID = %d", pLevel->GetID());

	sqlite3* _db = (sqlite3*)db;
	sqlite3_stmt *stmt;

	int rc = sqlite3_prepare_v2(_db, strSQL, -1, &stmt, NULL);
	if (rc != SQLITE_OK)
	{
		//strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
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
		//strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		result = false;
	}

	sqlite3_finalize(stmt);
	return true;
}

bool XMLManager::LoadPOIs(void* db, Level* pLevel, Project* project)
{
	char strSQL[256];
	sprintf_s(strSQL, 256, "Select ID, TypeID, Name, X, Y from POI where LevelID = %d", pLevel->GetID());

	sqlite3* _db = (sqlite3*)db;
	sqlite3_stmt *stmt;

	int rc = sqlite3_prepare_v2(_db, strSQL, -1, &stmt, NULL);
	if (rc != SQLITE_OK)
	{
		//strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
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
		//strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
		result = false;
	}

	sqlite3_finalize(stmt);
	return true;
}

bool XMLManager::LoadColumns(void* db, Level* pLevel, Project* project)
{
	char strSQL[256];
	sprintf_s(strSQL, 256, "Select ID, ColumnType, TLx, TLy, BLx, BLy, BRx, BRy from Column where LevelID = %d", pLevel->GetID());

	sqlite3* _db = (sqlite3*)db;
	sqlite3_stmt *stmt;

	int rc = sqlite3_prepare_v2(_db, strSQL, -1, &stmt, NULL);
	if (rc != SQLITE_OK)
	{
		//strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
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
		//strcpy_s(m_strError, 256, sqlite3_errmsg(_db));
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

int XMLManager::GetProjectCount()
{
	return (int)m_projects.size();
}

Project* XMLManager::GetProject(int nIndex)
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

std::map<int, FireSafetyManager::POIType*>& XMLManager::GetPOITypes()
{
	return m_mapPOITypes;
}

Project* XMLManager::FindProject(int nID)
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

std::wstring XMLManager::GetErrorMessage()
{
	return m_strError;
}
