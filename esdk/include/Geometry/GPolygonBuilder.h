#pragma once
#include "GeometryAPI.h"
#include "GVertexLink.h"

#ifndef DOTNET
#include <vector>
#endif

// 직선의 조합을 이용하여 Polygon List를 계산한다.
// [2018/11/09] 김지웅
namespace UnE
{
	namespace Geometry
	{
		_DECLARE_CLASS(Line2D);
		_DECLARE_CLASS(Polygon);

		GEOMETRY_EXPORT_CLASS(PolygonBuilder)
		{
		public:
			PolygonBuilder(void);
			virtual ~PolygonBuilder(void);

		public:
			void AddLine(REF_CONST(Vertex2D) vBegin, REF_CONST(Vertex2D) vEnd);

#ifdef DOTNET
			// Polygon 생성 과정에서 기존에 입력된 Line들이 쪼개어지게 되는데
			// 최종적으로 쪼개어진 모든 Line들의 집합을 lines에 담는다.
			System::Collections::Generic::List<Polygon^>^ MakePolygon(OUT System::Collections::Generic::List<Line2D^>^% lines);
#else
			// Polygon 생성 과정에서 기존에 입력된 Line들이 쪼개어지게 되는데
			// 최종적으로 쪼개어진 모든 Line들의 집합을 lines에 담는다.
			void MakePolygon(int& rPolygonCount, Polygon*& polygons, int& rLineCount, Line2D*& lines);
#endif

		private:
			POINTER(VertexLink::Node) GetNextNode(POINTER(VertexLink::Node) node, POINTER(VertexLink::Node) prev, OUT CBR(double) dAngle);
			void MakePolygon(REF(STD_VECTOR(POINTER(VertexLink::Node))) nodes, REF(STD_VECTOR(INSTANCE(Polygon))) polygons);
			void MakePolygon(POINTER(VertexLink::Node) node, REF(STD_VECTOR(POINTER(STD_VECTOR(POINTER(VertexLink::Node))))) polygonNodesList);
			bool CheckDuplicate(REF(STD_VECTOR(POINTER(VertexLink::Node))) polygonNodes, REF(STD_VECTOR(POINTER(STD_VECTOR(POINTER(VertexLink::Node))))) polygonNodesList);
			bool IsSamePolygonNodeList(REF(STD_VECTOR(POINTER(VertexLink::Node))) nodes1, REF(STD_VECTOR(POINTER(VertexLink::Node))) nodes2);
			POINTER(STD_VECTOR(POINTER(VertexLink::Node))) MakePolygon(POINTER(VertexLink::Node) begin, POINTER(VertexLink::Node) next);

		private:
#ifdef DOTNET
			System::Collections::Generic::List<Line2D^>^ m_lines = gcnew System::Collections::Generic::List<Line2D^>();
#else
			std::vector<Line2D> m_lines;
#endif
		};
	}
}
