#pragma once
#include "Vertex2D.h"
#include <vector>

namespace VectorGraphics
{
	class VertexList;
}

namespace FireSafetyManager
{
	class Wall;

	class Door
	{
	public:
		//  πÃ¥›¿ÃπÆ, ø‹ø©¥›¿ÃπÆ, æÁ¬  ø‹ø©¥›¿ÃπÆ, Ω÷ø©¥›¿ÃπÆ, æÁ¬  Ω÷ø©¥›¿ÃπÆ
		enum DoorType { Unknown = -1, Sliding = 0, Hinged, Hinged2, DualHinged, DualHinged2 };

	public:
		Door();

	public:
		static DoorType ToDoorType(int nDoorType);

	public:
		int GetID();
		const VectorGraphics::Vertex2D& GetHinge1();
		const VectorGraphics::Vertex2D& GetHinge2();
		const VectorGraphics::Vertex2D& GetPosition();
		double GetWidth();
		double GetHeight();
		double GetThick();
		double GetElevation();
		DoorType GetDoorType();
		Wall* GetWall();

		void SetID(int nID);
		void SetHinge1(const VectorGraphics::Vertex2D& vHinge);
		void SetHinge2(const VectorGraphics::Vertex2D& vHinge);
		void SetPosition(const VectorGraphics::Vertex2D& vPos);
		void SetWidth(double dWidth);
		void SetHeight(double dHeight);
		void SetThick(double dThick);
		void SetElevation(double dElevation);
		void SetDoorType(DoorType doorType);
		void SetWall(Wall* pWall);

		int CalcBoundary(std::vector<VectorGraphics::VertexList*>& edges);

	private:
		VectorGraphics::VertexList* MakeLineDoor(VectorGraphics::Vertex2D& vEmptyLineBegin, VectorGraphics::Vertex2D& vEmptyLineEnd);
		void SetDoorHinge(VectorGraphics::Vertex2D& vBegin, VectorGraphics::Vertex2D& vEnd, std::vector<VectorGraphics::VertexList*>& edges);
		VectorGraphics::VertexList* SetOneWayDoorHinge(VectorGraphics::Vertex2D& vHinge, VectorGraphics::Vertex2D* vBegin, VectorGraphics::Vertex2D* vEnd);
		void GetHingeDatas(const VectorGraphics::Vertex2D& vTop, const VectorGraphics::Vertex2D& vArcEnd, const VectorGraphics::Vertex2D& vCenter, bool topToEnd, VectorGraphics::VertexList* path);
		VectorGraphics::VertexList* SetTwoWayDoorHinge(VectorGraphics::Vertex2D& vHinge, VectorGraphics::Vertex2D* vBegin, VectorGraphics::Vertex2D* vEnd);

		bool IsBeginSide(const VectorGraphics::Vertex2D& vHinge, const VectorGraphics::Vertex2D& vBegin, const VectorGraphics::Vertex2D& vEnd);
		double GetArcAngle(const VectorGraphics::Vertex2D& vertex, const VectorGraphics::Vertex2D& vCenter, const VectorGraphics::Vertex2D& vRight);
		double InflateAngle(double dAngle);
		void AddArcVertex(const VectorGraphics::Vertex2D& vCenter, double dRadius, double dBeginAngle, bool isClockwise, VectorGraphics::VertexList* path);
		
	private:
		int m_nID;
		VectorGraphics::Vertex2D m_vHinge1;
		VectorGraphics::Vertex2D m_vHinge2;
		bool m_hasHinge1, m_hasHinge2;
		VectorGraphics::Vertex2D m_vPos;
		double m_dWidth;
		double m_dHeight;
		double m_dElevation;
		double m_dThick;
		DoorType m_doorType;
		Wall* m_pWall;
	};
}
