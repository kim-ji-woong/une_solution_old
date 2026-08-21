#include "StdAfx.h"
#include "Vertex.h"

BEGIN_NS(Utility)

Vertex3D::Vertex3D()
{
	m_pt[0] = m_pt[1] = m_pt[2] = 0.0;
}

Vertex3D::Vertex3D(double x, double y, double z)
{
	SetVertex(x, y, z);
}

void Vertex3D::SetVertex(double x, double y, double z)
{
	m_pt[0] = x;
	m_pt[1] = y;
	m_pt[2] = z;
}

Vertex2D::Vertex2D()
{
	m_pt[0] = m_pt[1] = 0.0;
}

Vertex2D::Vertex2D(double x, double y)
{
	SetVertex(x, y);
}

void Vertex2D::SetVertex(double x, double y)
{
	m_pt[0] = x;
	m_pt[1] = y;
}

END_NS
