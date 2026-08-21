#include "stdafx.h"

// 벡터곱
Utility::Vertex3D CrossProduct(const Utility::Vertex3D& v1, const Utility::Vertex3D& v2)
{
	// v1(a1,a2,a3) X v2(b1,b2,b3) = (a2 * b3 - a3 * b2,a3 * b1 - a1 * b3,a1 * b2 - a2 * b1)
	Utility::Vertex3D v;
	v.m_pt[0] = v1.m_pt[1] * v2.m_pt[2] - v1.m_pt[2] * v2.m_pt[1];
	v.m_pt[1] = v1.m_pt[2] * v2.m_pt[0] - v1.m_pt[0] * v2.m_pt[2];
	v.m_pt[2] = v1.m_pt[0] * v2.m_pt[1] - v1.m_pt[1] * v2.m_pt[0];
	return v;
}

// 판별식 : 이 값이 0이면 ppMat의 역행렬이 존재하지 않는다.
static double GetDeterminant(double** ppMat, int nSize)
{
	if (nSize == 1) return ppMat[0][0];
	else if (nSize == 2)
	{
		return ppMat[0][0] * ppMat[1][1] - ppMat[0][1] * ppMat[1][0];
	}

	double d = 0.0;
	double** ppTemp = new double*[nSize-1];
	int j, nColIndex;

	for (int i=0;i<nSize;i++)
	{
		for (j=0;j<nSize-1;j++)
		{
			ppTemp[j] = new double[nSize-1];

			for (int k=0;k<nSize-1;k++)
			{
				if (k < i) nColIndex = k;
				else
				{
					nColIndex = k + 1;
					if (nColIndex >= nSize) nColIndex -= nSize;
				}

				ppTemp[j][k] = ppMat[j+1][nColIndex];
			}
		}

		if (i % 2 == 0) d = d + ppMat[0][i] * GetDeterminant(ppTemp,nSize-1);
		else d = d - ppMat[0][i] * GetDeterminant(ppTemp,nSize-1);
		for (j=0;j<nSize-1;j++) delete [] ppTemp[j];
	}

	delete [] ppTemp;
	return d;
}

static double GetCofactor(double** ppMat, int nSize, int nRow, int nCol)
{
	if (nSize == 2)
	{
		if (nRow == 0)
		{
			if (nCol == 0) return ppMat[1][1];
			else return -ppMat[1][0];
		}
		else
		{
			if (nCol == 0) return -ppMat[0][1];
			else return ppMat[0][0];
		}
	}
	else if (nSize == 3)
	{
		int nRowIndex1 = nRow + 1 < nSize ? nRow + 1 : 0;
		int nRowIndex2 = nRow + 2 < nSize ? nRow + 2 : nRow + 2 - nSize;
		int nColIndex1 = nCol + 1 < nSize ? nCol + 1 : 0;
		int nColIndex2 = nCol + 2 < nSize ? nCol + 2 : nCol + 2 - nSize;

		return ppMat[nRowIndex1][nColIndex1] * ppMat[nRowIndex2][nColIndex2] - ppMat[nRowIndex1][nColIndex2] * ppMat[nRowIndex2][nColIndex1];
	}

	double** ppTemp = new double*[nSize-1];
	int i, nRowIndex, nColIndex;
	double dCofactor;

	for (i=0;i<nSize-1;i++)
	{
		ppTemp[i] = new double[nSize-1];

		if (i < nRow) nRowIndex = i;
		else nRowIndex = i + 1;

		for (int j=0;j<nSize-1;j++)
		{
			if (j < nCol) nColIndex = j;
			else nColIndex = j + 1;
			ppTemp[i][j] = ppMat[nRowIndex][nColIndex];
		}
	}

	if ((nRow + nCol) % 2 == 0) dCofactor = GetDeterminant(ppTemp,nSize-1);
	else dCofactor = -GetDeterminant(ppTemp,nSize-1);

	for (i=0;i<nSize-1;i++) delete [] ppTemp[i];
	delete [] ppTemp;
	return dCofactor;
}

// 정방행렬의 역행렬을 구한다.
// 역행렬이 존재하지 않으면 0을 리턴한다.
static double** InverseMatrix(double** ppMat, int nSize)
{
	if (nSize < 1) return 0;

	double det = GetDeterminant(ppMat,nSize);
	if (det == 0.0) return 0;

	double** ppInverse = new double*[nSize];

	for (int i=0;i<nSize;i++)
	{
		ppInverse[i] = new double[nSize];
		for (int j=0;j<nSize;j++)
		{
			ppInverse[i][j] = GetCofactor(ppMat,nSize,j,i) / det;
		}
	}

	return ppInverse;
}

BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

Entity::Entity(void)
{
	m_strDefaultSubClassName = L"AcDbEntity";
	m_bNotSupported = false;
	m_fLineWeight	= 0.0f;
	m_fLineWidth	= 0.0f;
	m_nColorIndex	= 256;
	m_nHandle		= 0;
	//m_nHandle		= Get32BitHandle();
	m_pLineType		= 0;
	m_pMgr			= 0;

	// X축 방향 벡터
	m_vecAxis[0].m_pt[0] = 1.0;
	m_vecAxis[0].m_pt[1] = 0.0;
	m_vecAxis[0].m_pt[2] = 0.0;
	// Y축 방향 벡터
	m_vecAxis[1].m_pt[0] = 0.0;
	m_vecAxis[1].m_pt[1] = 1.0;
	m_vecAxis[1].m_pt[2] = 0.0;
	// Z축 방향 벡터
	m_vecAxis[2].m_pt[0] = 0.0;
	m_vecAxis[2].m_pt[1] = 0.0;
	m_vecAxis[2].m_pt[2] = 1.0;
}

Entity::~Entity(void)
{
}

wchar_t* Entity::GetEntityType()
{
	return (wchar_t*)m_strEntityType.data();
}

void Entity::SetEntityType(wchar_t* strEntityType)
{
	m_strEntityType = strEntityType;
}

void Entity::SetHandle(int nHandle)
{
	m_nHandle = nHandle;
}

int Entity::GetHandle()
{
	return m_nHandle;
}

void Entity::SetSoftPointer(int nPointer)
{
	m_nSoftPointer = nPointer;
}

void Entity::SetSubClass(wchar_t* strClassName)
{
	m_strSubClassName = strClassName;
}

void Entity::SetOwnLayer(wchar_t* strLayer)
{
	m_strOwnLayer = strLayer;
}

void Entity::SetLineWeight(float fLineWeight)
{
	m_fLineWeight = fLineWeight;
}

void Entity::SetLineWidth(float fLineWidth)
{
	m_fLineWidth = fLineWidth;
}

void Entity::SetLineType(TABLES::LType::Entity* pLineType)
{
	m_pLineType = pLineType;
}

// nIndex : X축(0), Y축(1), Z축(2)
void Entity::SetAxisVector(int nIndex, Utility::Vertex3D& rVector)
{
	if (nIndex < 0 || nIndex > 2) return;

	m_vecAxis[nIndex] = rVector;
}

// nIndex : X축(0), Y축(1), Z축(2)
void Entity::SetAxisVector(int nIndex, double x, double y, double z)
{
	if (nIndex < 0 || nIndex > 2) return;

	m_vecAxis[nIndex].m_pt[0] = x;
	m_vecAxis[nIndex].m_pt[1] = y;
	m_vecAxis[nIndex].m_pt[2] = z;
}

// nIndex : X축(0), Y축(1), Z축(2)
bool Entity::GetAxisVector(int nIndex, double& x, double& y, double& z)
{
	if (nIndex < 0 || nIndex > 2)
		return false;

	x = m_vecAxis[nIndex].m_pt[0];
	y = m_vecAxis[nIndex].m_pt[1];
	z = m_vecAxis[nIndex].m_pt[2];

	return true;
}

bool Entity::IsSupoorted()
{
	return !m_bNotSupported;
}

// WCS 좌표계에서의 좌표(rCoordX,rCoordY,rCoordZ)를 OCS 좌표계의 좌표로 바꾼다.
// vNormal은 WCS에서의 좌표가 위치한 평면의 법선 벡터이다.
void Entity::WCSToOCS(double& rCoordX, double& rCoordY, double& rCoordZ, const Utility::Vertex3D& vNormal)
{
	// OCS 좌표계에서의 가상 X, Y, Z축
	Utility::Vertex3D vX, vY, vZ;

	// 1/64 = 0.015625
	if (fabs(vNormal.m_pt[0]) < 0.015625 && fabs(vNormal.m_pt[1]) < 0.015625)
	{
		Utility::Vertex3D v(0.0,1.0,0.0);
		vX = ::CrossProduct(v,vNormal);
	}
	else
	{
		Utility::Vertex3D v(0.0,0.0,1.0);
		vX = ::CrossProduct(v,vNormal);
	}

	vY = ::CrossProduct(vNormal,vX);
	vZ = ::CrossProduct(vX,vY);

	double x = rCoordX * vX.m_pt[0] + rCoordY * vX.m_pt[1] + rCoordZ * vX.m_pt[2];
	double y = rCoordX * vY.m_pt[0] + rCoordY * vY.m_pt[1] + rCoordZ * vY.m_pt[2];
	double z = rCoordX * vZ.m_pt[0] + rCoordY * vZ.m_pt[1] + rCoordZ * vZ.m_pt[2];

	rCoordX = x;
	rCoordY = y;
	rCoordZ = z;
}

// OCS 좌표계에서의 좌표(rCoordX,rCoordY,rCoordZ)를 WCS 좌표계의 좌표로 바꾼다.
// vNormal은 WCS에서의 좌표가 위치한 평면의 법선 벡터이다.
void Entity::OCSToWCS(double& rCoordX, double& rCoordY, double& rCoordZ, const Utility::Vertex3D& vNormal)
{
	// WCS 좌표계에서의 X, Y, Z축
	Utility::Vertex3D vX, vY, vZ;
	int i;

	// 1/64 = 0.015625
	if (fabs(vNormal.m_pt[0]) < 0.015625 && fabs(vNormal.m_pt[1]) < 0.015625)
	{
		vX = ::CrossProduct(Utility::Vertex3D(0.0,1.0,0.0),vNormal);
	}
	else
	{
		vX = ::CrossProduct(Utility::Vertex3D(0.0,0.0,1.0),vNormal);
	}

	vY = ::CrossProduct(vNormal,vX);
	vZ = ::CrossProduct(vX,vY);

	double** ppArr = new double*[3];
	for (i=0;i<3;i++) ppArr[i] = new double[3];
	memcpy(ppArr[0],vX.m_pt,sizeof(double)*3);
	memcpy(ppArr[1],vY.m_pt,sizeof(double)*3);
	memcpy(ppArr[2],vZ.m_pt,sizeof(double)*3);

	double** ppInverse = ::InverseMatrix(ppArr,3);
	if (ppInverse == 0)
	{
		for (i=0;i<3;i++) delete [] ppArr[i];
		delete [] ppArr;
		return;
	}

	double x = rCoordX * ppInverse[0][0] + rCoordY * ppInverse[0][1] + rCoordZ * ppInverse[0][2];
	double y = rCoordX * ppInverse[1][0] + rCoordY * ppInverse[1][1] + rCoordZ * ppInverse[1][2];
	double z = rCoordX * ppInverse[2][0] + rCoordY * ppInverse[2][1] + rCoordZ * ppInverse[2][2];

	for (i=0;i<3;i++)
	{
		delete [] ppArr[i];
		delete [] ppInverse[i];
	}
	delete [] ppArr;
	delete [] ppInverse;

	rCoordX = x;
	rCoordY = y;
	rCoordZ = z;
}

void Entity::Write(Utility::FileManager* pMgr)
{
	wchar_t strDefault[256];
	swprintf_s(strDefault,L"0\r\n%s\r\n5\r\n%X\r\n",m_strEntityType.data(),m_nHandle);
	pMgr->Write(strDefault,0,FILE_CURRENT);

	std::list<Group102>::const_iterator p = m_list102.begin();

	while (p != m_list102.end())
	{
		AddLine(pMgr,102,L"{%s",p->strName.data());
		AddLine(pMgr,p->nHandleCode,L"%X",p->nHandle);
		AddLine(pMgr,102,L"}");
		
		p++;
	}

	swprintf_s(strDefault,L"330\r\n%X\r\n100\r\n%s\r\n8\r\n%s\r\n",
		m_nSoftPointer,m_strDefaultSubClassName.data(),m_strOwnLayer.data());
	pMgr->Write(strDefault,0,FILE_CURRENT);

	if (m_pLineType)
	{
		AddLine(pMgr,6,L"%s",m_pLineType->GetTypeName());
	}

	if (m_fLineWeight != 0.0f) AddLine(pMgr,370,L"%d",(int)m_fLineWeight);
	if (m_nColorIndex != 256)	// 256 : ByLayer
	{
		AddLine(pMgr,62,L"%d",m_nColorIndex);
	}
}

void Entity::Add102Group(wchar_t* strGroupName, int nHandleCode, int nHandle)
{
	Group102 group;
	group.strName = strGroupName;
	group.nHandleCode = nHandleCode;
	group.nHandle = nHandle;
	m_list102.push_back(group);
}

void Entity::Remove102Group(wchar_t* strGroupName)
{
	std::list<Group102>::iterator p = m_list102.begin();

	while (p != m_list102.end())
	{
		if (!p->strName.compare(strGroupName))
		{
			m_list102.erase(p);
			break;
		}
		
		p++;
	}
}

void Entity::Set102Handle(wchar_t* strGroupName, int nHandle)
{
	std::list<Group102>::iterator p = m_list102.begin();

	while (p != m_list102.end())
	{
		if (!p->strName.compare(strGroupName))
		{
			p->nHandle = nHandle;
			break;
		}
		
		p++;
	}
}

void Entity::SetColorIndex(int nColorIndex)
{
	m_nColorIndex = nColorIndex;
}

wchar_t* Entity::GetOwnLayer()
{
	return (wchar_t*)m_strOwnLayer.data();
}

TABLES::LType::Entity* Entity::GetLineType()
{
	return m_pLineType;
}

bool Entity::ReadDatai(int nCode, int nData) 
{
	switch (nCode)
	{
	case 5:
		//m_nHandle = nData;
		SetHandle(nData);
		return true;

	case 62:
		m_nColorIndex = nData;
		return true;
	}

	return false;
}

bool Entity::ReadDatad(int nCode, double dData)
{
	return false;
}

bool Entity::ReadDatas(int nCode, wchar_t* strData)
{
	switch (nCode)
	{
	case 6:
		{
			//TABLES::TableManager* pTblMgr = DXFManager::GetDXFManager()->GetTableManager();
			TABLES::TableManager* pTblMgr = m_pMgr->GetOwner()->GetTableManager();
			if (pTblMgr == 0) return false;
			TABLES::LType* pLType = pTblMgr->GetLType();
			TABLES::LType::Entity* pEntity = pLType->GetEntity(strData);
			m_pLineType = pEntity;
		}
		return true;

	case 8:
		m_strOwnLayer = strData;
		return true;
	}

	return false;
}

int Entity::GetColorIndex()
{
	return m_nColorIndex;
}

void Entity::SetManager(EntityManager* pMgr)
{
	m_pMgr = pMgr;

	if (m_nHandle == 0 && m_pMgr != 0)
	{
		DXFManager* pDXFMgr = m_pMgr->GetOwner();

		if (pDXFMgr != 0)
		{
			m_nHandle = pDXFMgr->Get32BitHandle();
		}
	}
}

END_NS
END_NS
