#include "StdAfx.h"
#include "XmlLoader.h"

BEGIN_NS(UnE)
BEGIN_NS(LX)

XmlLoader::XmlLoader(void)
{
}


XmlLoader::~XmlLoader(void)
{
}

bool XmlLoader::Open(wchar_t* strPath)
{
	m_cgPoints.RemoveAllPoint(true);
	m_parcels.RemoveAllParcel(true);

	if (!m_xml.OpenXMLFile(strPath, true))
		return false;

	if (!LoadUnits())
		return false;

	if (!LoadParcels())
		return false;

	if (!LoadCgPoints())
		return false;

	return true;
}

Units& XmlLoader::GetUnits()
{
	return m_units;
}

Parcels& XmlLoader::GetParcels()
{
	return m_parcels;
}

CgPoints& XmlLoader::GetCgPoints()
{
	return m_cgPoints;
}

bool XmlLoader::LoadUnits()
{
	DWORD_PTR dwNode = m_xml.FindNodeTree(m_xml.GetRootNode(), L"Units", EasyXML2::ELEMENT);
	if (dwNode == 0)
		return true;

	DWORD_PTR dwChild = m_xml.GetChildNode(dwNode);

	wchar_t strNodeName[256];
	wchar_t strNodeValue[256];
	EasyXML2::DataType nodeType;

	while (dwChild != 0)
	{
		if (!m_xml.GetNodeData(dwChild, strNodeName, &nodeType))
			return false;

		if (nodeType == EasyXML2::ATTRIBUTE)
		{
			if (!m_xml.GetChildNodeData(dwChild, strNodeValue))
				return false;

			m_units.SetAttrib(strNodeName, strNodeValue);
		}

		dwChild = m_xml.GetNextNode(dwChild);
	}

	return true;
}

bool XmlLoader::LoadParcels()
{
	DWORD_PTR dwNode = m_xml.FindNodeTree(m_xml.GetRootNode(), L"Parcels", EasyXML2::ELEMENT);
	if (dwNode == 0)
		return true;

	DWORD_PTR dwChild = m_xml.GetChildNode(dwNode);

	wchar_t strNodeName[256];
	EasyXML2::DataType nodeType;

	while (dwChild != 0)
	{
		if (!m_xml.GetNodeData(dwChild, strNodeName, &nodeType))
			return false;

		if (nodeType == EasyXML2::ELEMENT && !_wcsicmp(strNodeName, L"Parcel"))
		{
			if (!LoadParcel(dwChild))
				return false;
		}

		dwChild = m_xml.GetNextNode(dwChild);
	}

	return true;
}

bool XmlLoader::LoadParcel(DWORD_PTR dwNode)
{
	DWORD_PTR dwChild = m_xml.GetChildNode(dwNode);

	wchar_t strNodeName[256];
	wchar_t strNodeValue[256];
	EasyXML2::DataType nodeType;

	Parcel* pParcel = new Parcel();

	while (dwChild != 0)
	{
		if (!m_xml.GetNodeData(dwChild, strNodeName, &nodeType))
			goto FAILURE;

		if (nodeType == EasyXML2::ELEMENT && !_wcsicmp(strNodeName, L"Parcels"))
		{
			if (!LoadParcels(dwChild, pParcel))
				goto FAILURE;
		}
		else if (nodeType == EasyXML2::ATTRIBUTE)
		{
			if (!m_xml.GetChildNodeData(dwChild, strNodeValue))
				goto FAILURE;

			if (!_wcsicmp(strNodeName, L"name"))
				pParcel->SetParcelName(strNodeValue);

			pParcel->SetAttrib(strNodeName, strNodeValue);
		}

		dwChild = m_xml.GetNextNode(dwChild);
	}

	m_parcels.AddParcel(pParcel);
	return true;

FAILURE:
	delete pParcel;
	return false;
}

bool XmlLoader::LoadParcels(DWORD_PTR dwNode, Parcel* pParcel)
{
	DWORD_PTR dwChild = m_xml.GetChildNode(dwNode);

	wchar_t strNodeName[256];
	EasyXML2::DataType nodeType;

	while (dwChild != 0)
	{
		if (!m_xml.GetNodeData(dwChild, strNodeName, &nodeType))
			return false;

		if (nodeType == EasyXML2::ELEMENT && !_wcsicmp(strNodeName, L"Parcel"))
		{
			if (!LoadParcel(dwChild, pParcel))
				return false;
		}

		dwChild = m_xml.GetNextNode(dwChild);
	}

	return true;
}

bool XmlLoader::LoadParcel(DWORD_PTR dwNode, Parcel* pParcel)
{
	DWORD_PTR dwChild = m_xml.GetChildNode(dwNode);

	wchar_t strNodeName[256];
	EasyXML2::DataType nodeType;

	while (dwChild != 0)
	{
		if (!m_xml.GetNodeData(dwChild, strNodeName, &nodeType))
			return false;

		if (nodeType == EasyXML2::ELEMENT && !_wcsicmp(strNodeName, L"CoordGeom"))
		{
			CoordGeom* pCoordGeom = LoadCoordGeom(dwChild);

			if (pCoordGeom == 0)
				return false;

			pParcel->AddCoord(pCoordGeom);
			break;
		}

		dwChild = m_xml.GetNextNode(dwChild);
	}

	return true;
}

CoordGeom* XmlLoader::LoadCoordGeom(DWORD_PTR dwNode)
{
	DWORD_PTR dwChild = m_xml.GetChildNode(dwNode);

	wchar_t strNodeName[256];
	EasyXML2::DataType nodeType;

	while (dwChild != 0)
	{
		if (!m_xml.GetNodeData(dwChild, strNodeName, &nodeType))
			return 0;

		if (nodeType == EasyXML2::ELEMENT && !_wcsicmp(strNodeName, L"IrregularLine"))
		{
			return LoadIrregularLine(dwChild);
		}

		dwChild = m_xml.GetNextNode(dwChild);
	}

	return 0;
}

CoordGeom* XmlLoader::LoadIrregularLine(DWORD_PTR dwNode)
{
	DWORD_PTR dwChild = m_xml.GetChildNode(dwNode);

	wchar_t strNodeName[256];
	wchar_t strNodeValue[256];
	EasyXML2::DataType nodeType;

	CoordGeom* pCoord = new CoordGeom();

	while (dwChild != 0)
	{
		if (!m_xml.GetNodeData(dwChild, strNodeName, &nodeType))
			goto FAILURE;

		if (nodeType == EasyXML2::ELEMENT)
		{
			if (!_wcsicmp(strNodeName, L"Start"))
			{
				if (ReadPointRef(dwChild, strNodeValue))
					pCoord->SetStartRef(strNodeValue);
			}
			else if (!_wcsicmp(strNodeName, L"End"))
			{
				if (ReadPointRef(dwChild, strNodeValue))
					pCoord->SetEndRef(strNodeValue);
			}
			else if (!_wcsicmp(strNodeName, L"PntList3D"))
			{
				if (!ReadPointList3D(dwChild, pCoord))
					goto FAILURE;
			}
		}

		dwChild = m_xml.GetNextNode(dwChild);
	}

	return pCoord;

FAILURE:
	delete pCoord;
	return 0;
}

bool XmlLoader::ReadPointRef(DWORD_PTR dwNode, wchar_t* strValue)
{
	DWORD_PTR dwChild = m_xml.GetChildNode(dwNode);

	wchar_t strNodeName[256];
	EasyXML2::DataType nodeType;

	while (dwChild != 0)
	{
		if (!m_xml.GetNodeData(dwChild, strNodeName, &nodeType))
			return false;

		if (nodeType == EasyXML2::ATTRIBUTE && !_wcsicmp(strNodeName, L"pntRef"))
		{
			if (!m_xml.GetChildNodeData(dwChild, strValue))
				return false;
			else
				break;
		}

		dwChild = m_xml.GetNextNode(dwChild);
	}

	return true;
}

bool XmlLoader::ReadPointList3D(DWORD_PTR dwNode, CoordGeom* pCoord)
{
	DWORD_PTR dwChild = m_xml.GetChildNode(dwNode);

	wchar_t strNodeName[256];
	EasyXML2::DataType nodeType;

	while (dwChild != 0)
	{
		if (!m_xml.GetNodeData(dwChild, strNodeName, &nodeType))
			return false;

		if (nodeType == EasyXML2::TEXT)
		{
			if (!pCoord->ReadPointList3D(strNodeName))
				return false;

			break;
		}

		dwChild = m_xml.GetNextNode(dwChild);
	}

	return true;
}

bool XmlLoader::LoadCgPoints()
{
	DWORD_PTR dwNode = m_xml.FindNodeTree(m_xml.GetRootNode(), L"CgPoints", EasyXML2::ELEMENT);
	if (dwNode == 0)
		return true;

	DWORD_PTR dwChild = m_xml.GetChildNode(dwNode);

	wchar_t strNodeName[256];
	EasyXML2::DataType nodeType;

	while (dwChild != 0)
	{
		if (!m_xml.GetNodeData(dwChild, strNodeName, &nodeType))
			return false;

		if (nodeType == EasyXML2::ELEMENT && !_wcsicmp(strNodeName, L"CgPoint"))
		{
			CgPoint* pPoint = LoadCgPoint(dwChild);

			if (pPoint == 0)
				return false;

			m_cgPoints.AddPoint(pPoint);
		}

		dwChild = m_xml.GetNextNode(dwChild);
	}

	return true;
}

CgPoint* XmlLoader::LoadCgPoint(DWORD_PTR dwNode)
{
	DWORD_PTR dwChild = m_xml.GetChildNode(dwNode);

	wchar_t strNodeName[256];
	wchar_t strNodeValue[256];
	EasyXML2::DataType nodeType;

	CgPoint* pPoint = new CgPoint();

	while (dwChild != 0)
	{
		if (!m_xml.GetNodeData(dwChild, strNodeName, &nodeType))
			goto FAILURE;

		if (nodeType == EasyXML2::ATTRIBUTE)
		{
			if (!m_xml.GetChildNodeData(dwChild, strNodeValue))
				goto FAILURE;

			if (!_wcsicmp(L"name", strNodeName))
				pPoint->SetName(strNodeValue);
			else if (!_wcsicmp(L"pntSurv", strNodeName))
				pPoint->SetSurveyType(strNodeValue);
			else if (!_wcsicmp(L"state", strNodeName))
				pPoint->SetStateType(strNodeValue);
			else if (!_wcsicmp(L"oID", strNodeName))
				pPoint->SetOID(strNodeValue);
		}
		else if (nodeType == EasyXML2::TEXT)
		{
			if (!pPoint->ReadPoint3D(strNodeName))
				goto FAILURE;
		}

		dwChild = m_xml.GetNextNode(dwChild);
	}

	return pPoint;

FAILURE:
	delete pPoint;
	return 0;
}

END_NS
END_NS
