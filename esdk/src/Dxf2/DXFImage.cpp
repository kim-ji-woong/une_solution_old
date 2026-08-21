#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

Image::Image(void)
{
	Init();
}

Image::~Image(void)
{
	*m_pRefCount -= 1;
	if (*m_pRefCount <= 0)
	{
		delete m_pRefCount;
		delete [] m_pBoundaryPointX;
		delete [] m_pBoundaryPointY;
	}
}

Image::Image(const Image& rhs)
{
	memcpy(this,&rhs,sizeof(Image));
	*m_pRefCount += 1;
}

void Image::operator= (const Image& rhs)
{
	// 같은 메모리를 공유하고 있는지 검사
	bool bSame = false;
	if (m_pRefCount == rhs.m_pRefCount) bSame = true;

	if (!bSame)
	{
		*m_pRefCount -= 1;
		if (*m_pRefCount <= 0) 
		{
			delete m_pRefCount;
			delete [] m_pBoundaryPointX;
			delete [] m_pBoundaryPointY;
		}
	}

	memcpy(this,&rhs,sizeof(Image));
	if (!bSame) *m_pRefCount += 1;
}

void Image::Write(Utility::FileManager* pMgr)
{
	Entity::Write(pMgr);
	int i;

	AddLine(pMgr,100,L"%s",m_strSubClassName.data());
	AddLine(pMgr,90,L"%d",m_nClassVersion);
	for (i=1;i<=3;i++) AddLine(pMgr,10*i,L"%lf",m_dArrPos[i-1]);
	for (i=1;i<=3;i++) AddLine(pMgr,10*i+1,L"%lf",m_vU.m_pt[i-1]);
	for (i=1;i<=3;i++) AddLine(pMgr,10*i+2,L"%lf",m_vV.m_pt[i-1]);
	AddLine(pMgr,13,L"%lf",m_dImageWidth);
	AddLine(pMgr,23,L"%lf",m_dImageHeight);
	AddLine(pMgr,340,L"%X",m_nImageDef);
	AddLine(pMgr,70,L"%d",m_nDisplayProperty);
	AddLine(pMgr,280,L"%d",m_nClippingState);
	AddLine(pMgr,281,L"%d",m_nBrightness);
	AddLine(pMgr,282,L"%d",m_nContrast);
	AddLine(pMgr,283,L"%d",m_nFade);
	AddLine(pMgr,360,L"%X",m_nImageDefReactor);
	AddLine(pMgr,71,L"%d",m_nBoundaryType);
	AddLine(pMgr,91,L"%d",m_nBoundaryPointSize);
	for (i=0;i<m_nBoundaryPointSize;i++)
	{
		AddLine(pMgr,14,L"%lf",m_pBoundaryPointX[i]);
		AddLine(pMgr,24,L"%lf",m_pBoundaryPointY[i]);
	}
}

void Image::Init()
{
	m_strSubClassName = L"AcDbRasterImage";
	m_strEntityType	  = L"IMAGE";
	m_nClassVersion	  = 0;
	m_bPositionFlag	  = false;
	m_dImageWidth	  = -1;
	m_nDisplayProperty= 7;
	m_nClippingState  = 0;
	m_nBrightness	  = 50;
	m_nContrast		  = 50;
	m_nFade			  = 0;
	m_nBoundaryType	  = 1;
	m_pBoundaryPointX = 0;
	m_pBoundaryPointY = 0;
	m_nBoundaryPointSize = 0;
}

void Image::SetClassVersion(int nVersion)
{
	m_nClassVersion = nVersion;
}

void Image::SetImageSize(double dWidth, double dHeight)
{
	m_dImageWidth  = dWidth;
	m_dImageHeight = dHeight;

	m_nBoundaryPointSize = 2;
	m_pBoundaryPointX = new double[m_nBoundaryPointSize];
	m_pBoundaryPointY = new double[m_nBoundaryPointSize];

	m_pBoundaryPointX[0] = -0.5;
	m_pBoundaryPointX[1] = dWidth - 0.5;
	m_pBoundaryPointY[0] = -0.5;
	m_pBoundaryPointY[1] = dHeight - 0.5;

	if (m_bPositionFlag)
	{
		double d1 = sqrt((m_dArrLB[0] - m_dArrRB[0]) * (m_dArrLB[0] - m_dArrRB[0]) + (m_dArrLB[1] - m_dArrRB[1]) * (m_dArrLB[1] - m_dArrRB[1]) + (m_dArrLB[2] - m_dArrRB[2]) * (m_dArrLB[2] - m_dArrRB[2]));
		double d2 = sqrt((m_dArrLB[0] - m_dArrPos[0]) * (m_dArrLB[0] - m_dArrPos[0]) + (m_dArrLB[1] - m_dArrPos[1]) * (m_dArrLB[1] - m_dArrPos[1]) + (m_dArrLB[2] - m_dArrPos[2]) * (m_dArrLB[2] - m_dArrPos[2]));
		double dScaleU = d1 / m_dImageWidth;
		double dScaleV = d2 / m_dImageHeight;

		for (int i=0;i<3;i++)
		{
			m_vU.m_pt[i] = (m_dArrRB[i] - m_dArrLB[i]) * dScaleU;
			m_vV.m_pt[i] = (m_dArrPos[i] - m_dArrLB[i]) * dScaleV;
		}
	}
}

// 원래의 이미지가 공간상에 위치한 모습을 세 좌표로 나타낸다.
// dArrLT : 원래 이미지의 좌측 상단의 모서리가 위치한 좌표
// dArrLB : 원래 이미지의 좌측 하단의 모서리가 위치한 좌표
// dArrRB : 원래 이미지의 우측 하단의 모서리가 위치한 좌표
void Image::SetPosition(double dArrLT[3], double dArrLB[3], double dArrRB[3])
{
	int nSize = sizeof(double) * 3;
	memcpy(m_dArrPos,dArrLT,nSize);
	memcpy(m_dArrLB,dArrLB,nSize);
	memcpy(m_dArrRB,dArrRB,nSize);

	if (m_dImageWidth > 0.0)
	{
		double d1 = sqrt((m_dArrLB[0] - m_dArrRB[0]) * (m_dArrLB[0] - m_dArrRB[0]) + (m_dArrLB[1] - m_dArrRB[1]) * (m_dArrLB[1] - m_dArrRB[1]) + (m_dArrLB[2] - m_dArrRB[2]) * (m_dArrLB[2] - m_dArrRB[2]));
		double d2 = sqrt((m_dArrLB[0] - m_dArrPos[0]) * (m_dArrLB[0] - m_dArrPos[0]) + (m_dArrLB[1] - m_dArrPos[1]) * (m_dArrLB[1] - m_dArrPos[1]) + (m_dArrLB[2] - m_dArrPos[2]) * (m_dArrLB[2] - m_dArrPos[2]));
		double dScaleU = d1 / m_dImageWidth;
		double dScaleV = d2 / m_dImageHeight;

		for (int i=0;i<3;i++)
		{
			m_vU.m_pt[i] = (m_dArrRB[i] - m_dArrLB[i]) * dScaleU;
			m_vV.m_pt[i] = (m_dArrPos[i] - m_dArrLB[i]) * dScaleV;
		}
	}

	m_bPositionFlag = true;
}

void Image::SetImageDef(int nImageDef)
{
	m_nImageDef = nImageDef;
}

void Image::SetDisplayProperty(int nProperty)
{
	m_nDisplayProperty = nProperty;
}

void Image::SetImageState(int nClippingState, int nBrightness, int nContrast, int nFade)
{
	m_nClippingState = nClippingState;
	m_nBrightness	 = nBrightness;
	m_nContrast		 = nContrast;
	m_nFade			 = nFade;
}

void Image::SetImageDefReactor(int nImageDefReactor)
{
	m_nImageDefReactor = nImageDefReactor;
}

void Image::SetBoundaryType(int nBoundaryType)
{
	m_nBoundaryType = nBoundaryType;
}

void Image::SetBoundaryPoint(double* pBoundaryPointX, double* pBoundaryPointY, int nArrSize)
{
	if (nArrSize <= 0) return;

	delete [] pBoundaryPointX;
	delete [] pBoundaryPointY;

	m_pBoundaryPointX = new double[nArrSize];
	m_pBoundaryPointY = new double[nArrSize];
	memcpy(m_pBoundaryPointX,pBoundaryPointX,sizeof(double)*nArrSize);
	memcpy(m_pBoundaryPointY,pBoundaryPointY,sizeof(double)*nArrSize);
	m_nBoundaryPointSize = nArrSize;
}

END_NS
END_NS
