#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

RoundRect::RoundRect(void)
{
}

RoundRect::~RoundRect(void)
{
}

void RoundRect::SetNormalVector(double dAxisX, double dAxisY, double dAxisZ)
{
	m_vNormal.m_pt[0] = dAxisX;
	m_vNormal.m_pt[1] = dAxisY;
	m_vNormal.m_pt[2] = dAxisZ;
}

void RoundRect::GetCenterPoint(double* pX, double* pY, double* pZ)
{
	*pX = m_dArrCoordCenter[0];
	*pY = m_dArrCoordCenter[1];
	*pZ = m_dArrCoordCenter[2];
}

void RoundRect::GetNormalVector(double* pX, double* pY, double* pZ)
{
	*pX = m_vNormal.m_pt[0];
	*pY = m_vNormal.m_pt[1];
	*pZ = m_vNormal.m_pt[2];
}

END_NS
END_NS
