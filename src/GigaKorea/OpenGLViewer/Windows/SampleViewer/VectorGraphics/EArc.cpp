#include "stdafx.h"
#include "EArc.h"
#include <math.h>

namespace VectorGraphics
{
	static const double HALF_PI = 1.57079632679489661923;
	static const double _PI = 3.14159265358979323846;
	static const double _3HALF_PI = 4.71238898038468985769;
	static const double _2PI = 6.28318530717958647692;

	EArc::EArc()
	{
		m_dBeginAngle = 0.0;
		m_dEArcAngle = 0.0;
		m_isClockwise = true;
	}

	EArc::EArc(const Vertex2D& vTL, const Vertex2D& vBL, const Vertex2D& vBR, double dBeginAngle, double dEArcAngle, bool isClockwise)
	{
		SetEArc(vTL, vBL, vBR, dBeginAngle, dEArcAngle, isClockwise);
	}

	EArc::~EArc()
	{
	}

	static Vertex2D GetEArcVertex(double angle, double a, double b, const VectorGraphics::Vertex2D& vTL, const VectorGraphics::Vertex2D& vBL, const VectorGraphics::Vertex2D& vBR, const VectorGraphics::Vertex2D& vT, const VectorGraphics::Vertex2D& vB, const VectorGraphics::Vertex2D& vL, const VectorGraphics::Vertex2D& vR)
	{
		if (angle < 0.0)
		{
			int nCount = (int)(-angle / _2PI);
			angle += _2PI * (nCount + 1);
		}
		else if (angle > _2PI)
		{
			int nCount = (int)(angle / _2PI);
			angle -= _2PI * nCount;
		}

		double tolerance = 0.001;
		Vertex2D vResult;

		if (angle <= tolerance || angle >= (_2PI - tolerance))
		{
			vResult = vR;
		}
		else if (angle >= (HALF_PI - tolerance) &&
			angle <= (HALF_PI + tolerance))
		{
			vResult = vT;
		}
		else if (angle >= (_PI - tolerance) &&
			angle <= (_PI + tolerance))
		{
			vResult = vL;
		}
		else if (angle >= (_3HALF_PI - tolerance) &&
			angle <= (_3HALF_PI + tolerance))
		{
			vResult = vB;
		}
		else
		{
			double dLengthX, dLengthY;

			if (angle < HALF_PI)
			{
				double dTanData = tan(angle);

				dLengthX = sqrt(1.0 / (1.0 / a / a + dTanData * dTanData / b / b));
				dLengthY = sqrt(1.0 / (1.0 / a / a / dTanData / dTanData + 1.0 / b / b));
			}
			else if (angle < _PI)
			{
				double dTanData = tan(_PI - angle);

				dLengthX = -sqrt(1.0 / (1.0 / a / a + dTanData * dTanData / b / b));
				dLengthY = sqrt(1.0 / (1.0 / a / a / dTanData / dTanData + 1.0 / b / b));
			}
			else if (angle < _3HALF_PI)
			{
				double dTanData = tan(angle - _PI);

				dLengthX = -sqrt(1.0 / (1.0 / a / a + dTanData * dTanData / b / b));
				dLengthY = -sqrt(1.0 / (1.0 / a / a / dTanData / dTanData + 1.0 / b / b));
			}
			else
			{
				double dTanData = tan(_2PI - angle);

				dLengthX = sqrt(1.0 / (1.0 / a / a + dTanData * dTanData / b / b));
				dLengthY = -sqrt(1.0 / (1.0 / a / a / dTanData / dTanData + 1.0 / b / b));
			}

			Vertex2D vCenter = (vTL + vBR) / 2;
			vResult = vCenter + (vR - vCenter) * dLengthX / a;
			vResult = vResult + (vT - vCenter) * dLengthY / b;
		}

		return vResult;
	}

	void EArc::SetEArc(const Vertex2D& vTL, const Vertex2D& vBL, const Vertex2D& vBR, double dBeginAngle, double dEArcAngle, bool isClockwise)
	{
		m_vTL = vTL;
		m_vBL = vBL;
		m_vBR = vBR;
		m_dBeginAngle = dBeginAngle;
		m_dEArcAngle = dEArcAngle;
		m_isClockwise = isClockwise;

		Vertex2D vL = (m_vTL + m_vBL) / 2;
		Vertex2D vB = (m_vBL + m_vBR) / 2;
		Vertex2D vT = m_vTL - m_vBL + vB;
		Vertex2D vR = m_vBR - m_vBL + vL;

		double a = vBL.GetDistance(m_vBR) / 2;
		double b = vTL.GetDistance(m_vBL) / 2;

		double endAngle = isClockwise ? m_dBeginAngle - m_dEArcAngle : m_dBeginAngle + m_dEArcAngle;

		VectorGraphics::Vertex2D vBegin = GetEArcVertex(m_dBeginAngle, a, b, m_vTL, m_vBL, m_vBR, vT, vB, vL, vR);
		VectorGraphics::Vertex2D vEnd = GetEArcVertex(endAngle, a, b, m_vTL, m_vBL, m_vBR, vT, vB, vL, vR);

		int nSlice = 100;
		double theta = m_dEArcAngle / nSlice;

		m_vertices.clear();

		m_vertices.push_back(vBegin);
		
		for (int i = 1; i < nSlice; i++)
		{
			double angle = isClockwise ? m_dBeginAngle - theta * i : m_dBeginAngle + theta * i;
			Vertex2D vertex = GetEArcVertex(angle, a, b, vTL, vBL, vBR, vT, vB, vL, vR);

			m_vertices.push_back(vertex);
		}

		m_vertices.push_back(vEnd);
	}

	const std::list<Vertex2D>& EArc::GetVertices()
	{
		return m_vertices;
	}

	void EArc::Draw()
	{
		glBegin(GL_LINES);
		
		for (std::list<Vertex2D>::iterator iter = m_vertices.begin(); iter != m_vertices.end(); iter++)
		{
			glVertex2f((float)iter->x, (float)iter->y);
		}

		glEnd();
	}
}
