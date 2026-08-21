#include "StdAfx.h"
#include "Parcel.h"
#include "UnEUtility/StringManager.h"

BEGIN_NS(UnE)
BEGIN_NS(LX)

CoordGeom::CoordGeom()
{
	m_strStartRef = L"";
	m_strEndRef = L"";
}

CoordGeom::~CoordGeom()
{
}

void CoordGeom::SetStartRef(std::wstring strRef)
{
	m_strStartRef = strRef;
}

void CoordGeom::SetEndRef(std::wstring strRef)
{
	m_strEndRef = strRef;
}

const std::wstring& CoordGeom::GetStartRef() const
{
	return m_strStartRef;
}

const std::wstring& CoordGeom::GetEndRef() const
{
	return m_strEndRef;
}

void CoordGeom::AddVertex(Geometry::Vertex3D vertex)
{
	m_vecVertices.push_back(vertex);
}

bool CoordGeom::InsertVertex(int nIndex, Geometry::Vertex3D vertex)
{
	if (nIndex < 0 || nIndex > GetVertexCount())
		return false;

	std::vector<Geometry::Vertex3D>::iterator iter = m_vecVertices.begin() + nIndex;
	m_vecVertices.insert(iter, vertex);
	return true;
}

int CoordGeom::GetVertexCount() const
{
	return (int)m_vecVertices.size();
}

const Geometry::Vertex3D* CoordGeom::GetVertex(int nIndex) const
{
	if (nIndex < 0 || nIndex >= GetVertexCount())
		return 0;

	return &m_vecVertices[nIndex];
}

bool CoordGeom::RemoveVertex(int nIndex)
{
	if (nIndex < 0 || nIndex >= GetVertexCount())
		return false;

	m_vecVertices.erase(m_vecVertices.begin() + nIndex);
	return true;
}

void CoordGeom::RemoveAllVertex()
{
	m_vecVertices.clear();
}

bool CoordGeom::ReadPointList3D(wchar_t* strPoints)
{
	unsigned int nBeginIndex = 0, nEndIndex;
	int nIndex = 0;
	double* pData = 0;

	Geometry::Vertex3D vertex;

	while (GetPoint3DNextIndex(strPoints, nBeginIndex, nEndIndex))
	{
		if (nIndex > 2)
			nIndex = 0;

		if (nIndex == 0)
			pData = &vertex.x;
		else if (nIndex == 1)
			pData = &vertex.y;
		else
			pData = &vertex.z;

		if (!Utility::StringManager::StrToDouble(strPoints, pData, nBeginIndex, nEndIndex))
			return false;

		nIndex++;

		if (nIndex > 2)
			AddVertex(vertex);
	}

	return nIndex == 3;
}

bool CoordGeom::GetPoint3DNextIndex(wchar_t* strPoints, unsigned int& rBeginIndex, unsigned int& rEndIndex)
{
	if (rBeginIndex != 0)
		rBeginIndex = rEndIndex + 1;

	bool findBegin = false;

	for (unsigned int i=rBeginIndex;strPoints[i];i++)
	{
		if (!findBegin)
		{
			if (strPoints[i] != ' ' && strPoints[i] != '\t' && strPoints[i] != '\r' && strPoints[i] != '\n')
			{
				rBeginIndex = i;
				findBegin = true;
			}
		}
		else
		{
			if (strPoints[i] == ' ' || strPoints[i] == '\t')
			{
				rEndIndex = i - 1;
				return true;
			}
		}
	}

	return false;
}

Parcel::Parcel(void)
{
	m_strParcelName = L"";

	m_pRefCount = new int;
	*m_pRefCount = 1;
}

Parcel::Parcel(const Parcel& rhs)
{
	Copy(rhs);
	*m_pRefCount += 1;
}

void Parcel::operator= (const Parcel& rhs)
{
	// 같은 메모리를 공유하고 있는지 검사
	bool bSame = false;
	if (m_pRefCount == rhs.m_pRefCount) bSame = true;

	if (!bSame)
	{
		*m_pRefCount -= 1;
		if (*m_pRefCount <= 0) 
		{
			FreeMemory();
			delete m_pRefCount;
		}
	}

	Copy(rhs);
	if (!bSame) *m_pRefCount += 1;
}

Parcel::~Parcel(void)
{
	*m_pRefCount -= 1;

	if (*m_pRefCount <= 0)
	{
		FreeMemory();
		delete m_pRefCount;
	}
}

void Parcel::Copy(const Parcel& rhs)
{
	m_vecCoords.insert(m_vecCoords.begin(), rhs.m_vecCoords.begin(), rhs.m_vecCoords.end());
	m_strParcelName = rhs.m_strParcelName;

	m_pRefCount = rhs.m_pRefCount;
}

void Parcel::FreeMemory()
{
	int nCoordCount = (int)m_vecCoords.size();

	for (int i=0;i<nCoordCount;i++)
	{
		delete m_vecCoords[i];
	}

	m_vecCoords.clear();
}

void Parcel::SetParcelName(std::wstring strParcelName)
{
	m_strParcelName = strParcelName;
}

const std::wstring& Parcel::GetParcelName() const
{
	return m_strParcelName;
}

void Parcel::AddCoord(CoordGeom* pCoord)
{
	if (pCoord)
		m_vecCoords.push_back(pCoord);
}

bool Parcel::InsertCoord(int nIndex, CoordGeom* pCoord)
{
	if (nIndex < 0 || nIndex > GetCoordCount() || pCoord == 0)
		return false;

	std::vector<CoordGeom*>::iterator iter = m_vecCoords.begin() + nIndex;
	m_vecCoords.insert(iter, pCoord);
	return true;
}

int Parcel::GetCoordCount() const
{
	return (int)m_vecCoords.size();
}

const CoordGeom* Parcel::GetCoord(int nIndex) const
{
	if (nIndex < 0 || nIndex >= GetCoordCount())
		return 0;

	return m_vecCoords[nIndex];
}

bool Parcel::RemoveCoord(int nIndex)
{
	if (nIndex < 0 || nIndex >= GetCoordCount())
		return false;

	m_vecCoords.erase(m_vecCoords.begin() + nIndex);
	return true;
}

void Parcel::RemoveAllCoord()
{
	m_vecCoords.clear();
}

int Parcel::GetAttribCount() const
{
	return (int)m_mapAttr.size();
}

void Parcel::SetAttrib(std::wstring strAttrName, std::wstring strAttrValue)
{
	m_mapAttr[strAttrName] = strAttrValue;
}

bool Parcel::GetAttrib(int nIndex, std::wstring& strAttrName, std::wstring& strAttrValue)
{
	if (nIndex < 0 || nIndex >= GetAttribCount())
		return false;

	std::map<std::wstring, std::wstring>::iterator iter = m_mapAttr.begin();

	for (int i=0;i<nIndex;i++)
		iter++;

	strAttrName = iter->first;
	strAttrValue = iter->second;

	return true;
}

bool Parcel::RemoveAttrib(int nIndex)
{
	if (nIndex < 0 || nIndex >= GetAttribCount())
		return false;

	std::map<std::wstring, std::wstring>::iterator iter = m_mapAttr.begin();

	for (int i=0;i<nIndex;i++)
		iter++;

	m_mapAttr.erase(iter);
	return true;
}

bool Parcel::RemoveAttrib(std::wstring strAttrName)
{
	std::map<std::wstring, std::wstring>::iterator iter = m_mapAttr.find(strAttrName);

	if (iter == m_mapAttr.end())
		return false;

	m_mapAttr.erase(iter);
	return true;
}

void Parcel::RemoveAllAttrib()
{
	m_mapAttr.clear();
}

Parcels::Parcels()
{
	m_pRefCount = new int;
	*m_pRefCount = 1;
}

Parcels::~Parcels()
{
	*m_pRefCount -= 1;

	if (*m_pRefCount <= 0)
	{
		FreeMemory();
		delete m_pRefCount;
	}
}

Parcels::Parcels(const Parcels& rhs)
{
	Copy(rhs);
	*m_pRefCount += 1;
}

void Parcels::operator= (const Parcels& rhs)
{
	// 같은 메모리를 공유하고 있는지 검사
	bool bSame = false;
	if (m_pRefCount == rhs.m_pRefCount) bSame = true;

	if (!bSame)
	{
		*m_pRefCount -= 1;
		if (*m_pRefCount <= 0) 
		{
			FreeMemory();
			delete m_pRefCount;
		}
	}

	Copy(rhs);
	if (!bSame) *m_pRefCount += 1;
}

void Parcels::AddParcel(Parcel* pParcel)
{
	if (pParcel)
		m_vecParcels.push_back(pParcel);
}

bool Parcels::InsertParcel(int nIndex, Parcel* pParcel)
{
	if (nIndex < 0 || nIndex > GetParcelCount() || pParcel == 0)
		return false;

	m_vecParcels.insert(m_vecParcels.begin() + nIndex, pParcel);
	return true;
}

int Parcels::GetParcelCount() const
{
	return (int)m_vecParcels.size();
}

const Parcel* Parcels::GetParcel(int nIndex) const
{
	if (nIndex < 0 || nIndex >= GetParcelCount())
		return false;

	return m_vecParcels[nIndex];
}

bool Parcels::RemoveParcel(int nIndex)
{
	if (nIndex < 0 || nIndex >= GetParcelCount())
		return false;

	m_vecParcels.erase(m_vecParcels.begin() + nIndex);
	return true;
}

void Parcels::RemoveAllParcel(bool freeMemory)
{
	if (freeMemory)
		FreeMemory();
	else
		m_vecParcels.clear();
}

void Parcels::Copy(const Parcels& rhs)
{
	m_vecParcels.insert(m_vecParcels.begin(), rhs.m_vecParcels.begin(), rhs.m_vecParcels.end());

	m_pRefCount = rhs.m_pRefCount;
}

void Parcels::FreeMemory()
{
	int nParcelCount = GetParcelCount();

	for (int i=0;i<nParcelCount;i++)
	{
		delete m_vecParcels[i];
	}

	m_vecParcels.clear();
}

END_NS
END_NS
