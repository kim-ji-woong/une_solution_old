#include "stdafx.h"
#include "Door.h"
#include "POI.h"
#include "Wall.h"

using namespace VectorGraphics;

namespace FireSafetyManager
{
	static const double HALF_PI = 1.57079632679489661923;
	static const double _2PI = 6.28318530717958647692;

	Door::Door()
	{
		m_nID = -1;
		m_hasHinge1 = m_hasHinge2 = false;
		m_dThick = 10.0;
	}

	Door::DoorType Door::ToDoorType(int nDoorType)
	{
		if (nDoorType == (int)DoorType::Sliding)
			return DoorType::Sliding;
		else if (nDoorType == (int)DoorType::Hinged)
			return DoorType::Hinged;
		else if (nDoorType == (int)DoorType::Hinged2)
			return DoorType::Hinged2;
		else if (nDoorType == (int)DoorType::DualHinged)
			return DoorType::DualHinged;
		else if (nDoorType == (int)DoorType::DualHinged2)
			return DoorType::DualHinged2;

		return DoorType::Unknown;
	}

	int Door::GetID()
	{
		return m_nID;
	}

	const VectorGraphics::Vertex2D& Door::GetHinge1()
	{
		return m_vHinge1;
	}

	const VectorGraphics::Vertex2D& Door::GetHinge2()
	{
		return m_vHinge2;
	}

	const VectorGraphics::Vertex2D& Door::GetPosition()
	{
		return m_vPos;
	}

	double Door::GetWidth()
	{
		return m_dWidth;
	}

	double Door::GetHeight()
	{
		return m_dHeight;
	}

	double Door::GetThick()
	{
		return m_dThick;
	}

	double Door::GetElevation()
	{
		return m_dElevation;
	}

	Door::DoorType Door::GetDoorType()
	{
		return m_doorType;
	}

	Wall* Door::GetWall()
	{
		return m_pWall;
	}

	void Door::SetID(int nID)
	{
		m_nID = nID;
	}

	void Door::SetHinge1(const VectorGraphics::Vertex2D& vHinge)
	{
		m_vHinge1 = vHinge;
		m_hasHinge1 = true;
	}

	void Door::SetHinge2(const VectorGraphics::Vertex2D& vHinge)
	{
		m_vHinge2 = vHinge;
		m_hasHinge2 = true;
	}

	void Door::SetPosition(const VectorGraphics::Vertex2D& vPos)
	{
		m_vPos = vPos;
	}

	void Door::SetWidth(double dWidth)
	{
		m_dWidth = dWidth;
	}

	void Door::SetHeight(double dHeight)
	{
		m_dHeight = dHeight;
	}

	void Door::SetThick(double dThick)
	{
		m_dThick = dThick;
	}

	void Door::SetElevation(double dElevation)
	{
		m_dElevation = dElevation;
	}

	void Door::SetDoorType(Door::DoorType doorType)
	{
		m_doorType = doorType;
	}

	void Door::SetWall(Wall* pWall)
	{
		m_pWall = pWall;
	}

	int Door::CalcBoundary(std::vector<VertexList*>& edges)
	{
		if (m_pWall == 0 || m_dWidth <= 0.0)
			return 0;

		Vertex2D vEmptyLineBegin, vEmptyLineEnd;

		VertexList* vertices = MakeLineDoor(vEmptyLineBegin, vEmptyLineEnd);

		if (vertices == 0)
			return 0;

		edges.push_back(vertices);

		SetDoorHinge(vEmptyLineBegin, vEmptyLineEnd, edges);
		return (int)edges.size();
	}

	void Door::SetDoorHinge(Vertex2D& vBegin, Vertex2D& vEnd, std::vector<VertexList*>& edges)
	{
		// 한방향 외여닫이문
		if (m_doorType == DoorType::Hinged)
		{
			if (m_hasHinge1)
			{
				VertexList* path = SetOneWayDoorHinge(m_vHinge1, &vBegin, &vEnd);

				if (path != 0)
					edges.push_back(path);
			}
		}
		// 양방향 외여닫이문
		else if (m_doorType == DoorType::Hinged2)
		{
			if (m_hasHinge1)
			{
				VertexList* path = SetTwoWayDoorHinge(m_vHinge1, &vBegin, &vEnd);

				if (path != 0)
					edges.push_back(path);
			}
		}
		// 한방향 쌍여닫이문
		else if (m_doorType == DoorType::DualHinged)
		{
			if (m_hasHinge1 == false || m_hasHinge2 == false)
				return;

			double len = vBegin.GetDistance(m_vHinge1) - m_dThick / 2;
			Vertex2D vMiddle = vBegin.GetLinearVertex(vEnd, len);

			VertexList* path2 = SetOneWayDoorHinge(m_vHinge1, &vBegin, &vMiddle);
			VertexList* path3 = SetOneWayDoorHinge(m_vHinge2, &vEnd, &vMiddle);

			if (path2 != 0 && path3 != 0)
			{
				edges.push_back(path2);
				edges.push_back(path3);
			}
		}
		// 양방향 쌍여닫이문
		else if (m_doorType == DoorType::DualHinged2)
		{
			if (m_hasHinge1 == false || m_hasHinge2 == false)
				return;

			double len = vBegin.GetDistance(m_vHinge1) - m_dThick / 2;
			Vertex2D vMiddle = vBegin.GetLinearVertex(vEnd, len);

			VertexList* path2 = SetTwoWayDoorHinge(m_vHinge1, &vBegin, &vMiddle);
			VertexList* path3 = SetTwoWayDoorHinge(m_vHinge2, &vEnd, &vMiddle);

			if (path2 != 0 && path3 != 0)
			{
				edges.push_back(path2);
				edges.push_back(path3);
			}
		}
	}

	bool Door::IsBeginSide(const Vertex2D& vHinge, const Vertex2D& vBegin, const Vertex2D& vEnd)
	{
		double len1 = vHinge.GetDistance(vBegin);
		double len2 = vHinge.GetDistance(vEnd);
		return len1 < len2;
	}

	VertexList* Door::SetTwoWayDoorHinge(Vertex2D& vHinge, Vertex2D* vBegin, Vertex2D* vEnd)
	{
		if (IsBeginSide(vHinge, *vBegin, *vEnd) == false)
		{
			Vertex2D* vTemporary = vBegin;
			vBegin = vEnd;
			vEnd = vTemporary;
		}

		vHinge = vBegin->GetLinearVertex(vHinge, vBegin->GetDistance(vHinge) + m_dThick / 2);

		Vertex2D vHinge1 = vHinge;
		Vertex2D vHinge2 = *vBegin * 2 - vHinge1;
		Vertex2D vB1 = vBegin->GetLinearVertex(vHinge1, m_dThick / 2);
		Vertex2D vB2 = *vBegin * 2 - vB1;
		Vertex2D vE2 = vB2 - *vBegin + *vEnd;
		Vertex2D vE1 = vB1 - *vBegin + *vEnd;

		VertexList* path = new VertexList();

		path->Vertices.push_back(vE1);
		GetHingeDatas(vHinge1, vE1, vB1, false, path);
		
		path->Vertices.push_back(vHinge2);
		GetHingeDatas(vHinge2, vE2, vB2, true, path);
		path->Vertices.push_back(vE1);

		return path;
	}

	VertexList* Door::SetOneWayDoorHinge(Vertex2D& vHinge, Vertex2D* vBegin, Vertex2D* vEnd)
	{
		if (IsBeginSide(vHinge, *vBegin, *vEnd) == false)
		{
			Vertex2D* vTemporary = vBegin;
			vBegin = vEnd;
			vEnd = vTemporary;
		}

		vHinge = vBegin->GetLinearVertex(vHinge, vBegin->GetDistance(vHinge) + m_dThick / 2);

		Vertex2D vB = vBegin->GetLinearVertex(vHinge, m_dThick / 2);
		Vertex2D vE = vB - *vBegin + *vEnd;

		VertexList* path = new VertexList();

		path->Vertices.push_back(vE);
		path->Vertices.push_back(vB);
		path->Vertices.push_back(vHinge);
		
		GetHingeDatas(vHinge, vE, vB, true, path);
		return path;
	}

	void Door::AddArcVertex(const Vertex2D& vCenter, double dRadius, double dBeginAngle, bool isClockwise, VertexList* path)
	{
		int nVertexCount = 25;
		double dAngle = HALF_PI / (nVertexCount - 1);

		for (int i = 1; i < nVertexCount; i++)
		{
			double dTheta = isClockwise ? dBeginAngle - dAngle * i : dBeginAngle + dAngle * i;
			double x = vCenter.x + cos(dTheta) * dRadius;
			double y = vCenter.y + sin(dTheta) * dRadius;

			path->Vertices.push_back(Vertex2D(x, y));
		}
	}

	bool IsSameAngle(double angle1, double angle2)
	{
		double dCos1 = cos(angle1);
		double dSin1 = sin(angle1);
		double dCos2 = cos(angle2);
		double dSin2 = sin(angle2);

		if (fabs(dCos1 - dCos2) < 0.001 && fabs(dSin1 - dSin2) < 0.001)
			return true;

		return false;
	}

	void Door::GetHingeDatas(const Vertex2D& vTop, const Vertex2D& vArcEnd, const Vertex2D& vCenter, bool topToEnd, VertexList* path)
	{
		Vertex2D vR(vCenter.x + 100.0, vCenter.y);

		double dBeginAngle = GetArcAngle(vTop, vCenter, vR);
		double dEndAngle = GetArcAngle(vArcEnd, vCenter, vR);
		bool isClockWise = true;

		/*double dTarget1 = InflateAngle(dBeginAngle + HALF_PI);
		double dTarget2 = InflateAngle(dBeginAngle - HALF_PI);

		if (fabs(dTarget1 - dEndAngle) < fabs(dTarget2 - dEndAngle))
		{
			isClockWise = false;
		}*/

		double dRadius = vTop.GetDistance(vCenter);

		if (topToEnd)
		{
			if (IsSameAngle(dBeginAngle - HALF_PI, dEndAngle) == false)
				isClockWise = false;

			AddArcVertex(vCenter, dRadius, dBeginAngle, isClockWise, path);
		}
		else
		{
			if (IsSameAngle(dEndAngle - HALF_PI, dBeginAngle) == false)
				isClockWise = false;

			AddArcVertex(vCenter, dRadius, dEndAngle, isClockWise, path);
		}
	}

	static double GetAngle(const Vertex2D& v1, const Vertex2D& vCenter, const Vertex2D& v2)
	{
		double a = vCenter.GetDistance(v1);
		double b = v2.GetDistance(vCenter);
		double c = v2.GetDistance(v1);

		double cosData = (a * a + b * b - c * c) / 2 / a / b;
		if (cosData < -1.0) cosData = -1.0;
		else if (cosData > 1.0) cosData = 1.0;

		return acos(cosData);
	}

	double Door::InflateAngle(double dAngle)
	{
		while (dAngle >= _2PI)
		{
			dAngle -= _2PI;
		}

		while (dAngle < 0.0)
		{
			dAngle += _2PI;
		}

		return dAngle;
	}

	double Door::GetArcAngle(const Vertex2D& vertex, const Vertex2D& vCenter, const Vertex2D& vRight)
	{
		double dAngle = GetAngle(vertex, vCenter, vRight);

		if (vertex.y < vRight.y)
			dAngle = _2PI - dAngle;

		return dAngle;
	}

	VertexList* Door::MakeLineDoor(Vertex2D& vEmptyLineBegin, Vertex2D& vEmptyLineEnd)
	{
		vEmptyLineBegin = m_vPos.GetLinearVertex(m_pWall->GetBegin(), m_dWidth / 2);
		vEmptyLineEnd = m_vPos * 2 - vEmptyLineBegin;

		Vertex2D v1 = vEmptyLineBegin.GetRightVertex(vEmptyLineEnd, m_dThick / 2);
		Vertex2D v2 = vEmptyLineBegin * 2 - v1;
		Vertex2D v3 = m_vPos * 2 - v1;
		Vertex2D v4 = m_vPos * 2 - v2;

		VertexList* vertices = new VertexList();

		vertices->Vertices.push_back(v1);
		vertices->Vertices.push_back(v2);
		vertices->Vertices.push_back(v3);
		vertices->Vertices.push_back(v4);
		vertices->Vertices.push_back(v1);

		return vertices;
	}
}
