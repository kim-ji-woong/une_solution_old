#pragma once
#include "GeometryAPI.h"
#include "GVertex.h"

#ifndef DOTNET
#include <vector>
#endif

#ifndef GCE2D_MAKELINE
#define GCE2D_MAKELINE(Line2DObj) GCE2d_MakeLine(GP_POINT2D(OF(Line2DObj, GetVertex(true))), GP_POINT2D(OF(Line2DObj, GetVertex(false))))
#endif

// 직선과 반직선, 선분을 표현한다.
// 모든 Line class의 Default 속성은 선분이다.
// [2012/07/27] 김지웅

namespace UnE
{
	namespace Geometry
	{
		_DECLARE_CLASS(Line2D);
		_DECLARE_CLASS(Line2F);

		GEOMETRY_EXPORT_CLASS(Polygon)
		{
		public:
			Polygon(void);
			virtual ~Polygon(void);

		public:
			int GetVertexCount() CONSTF;
			// nIndex가 배열의 범위를 벗어나면 NULL을 리턴한다.
			POINTER(Vertex2D) GetVertex(int nIndex);

			void AddVertex(REF_CONST(Vertex2D) vertex);
			bool Insert(int nIndex, REF_CONST(Vertex2D) vertex);

			bool UpdateVertex(int nIndex, REF_CONST(Vertex2D) vertex);

			bool RemoveVertex(int nIndex);
			void Clear();

			// 점이 폴리곤 내부에 있는지 검색
			// 폴리곤의 시작점과 끝점이 다를 경우, 시작점과 끝점이 연결된 폐곡선으로 간주한다.
			// 물론 폴리곤의 시작점과 끝점이 같아도 상관없다.
			// Return : 1이면 vertex가 폴리곤의 내부에 위치한다.
			//          0이면 vertex가 폴리곤의 외부에 위치한다.
			//         -1이면 vertex가 폴리곤의 경계에 위치한다.
			int HitTest(REF_CONST(Vertex2D) vertex);

			// 폴리곤의 무게중심을 구한다.
			INSTANCE(Vertex2D) CalcWeightCenter();
			double GetArea() CONSTF;

			// Bounding Rect의 Min
			INSTANCE(Vertex2D) GetMin();
			// Bounding Rect의 Max
			INSTANCE(Vertex2D) GetMax();

			// vertex와 Polygon의 가장 가까운 외곽선과의 거리
			// vertex가 Polygon의 내부에 존재할 경우 음수값을 리턴한다.
			double GetDistance(REF_CONST(Vertex2D) vertex);
			// vertex와 Polygon의 가장 가까운 외곽선과의 거리 및 가장 가까운 점을 리턴한다.
			// vertex가 Polygon의 내부에 존재할 경우 음수값을 리턴한다.
			double GetDistanceNVertex(REF_CONST(Vertex2D) vertex, OUT CBR(INSTANCE(Vertex2D)) vResult);
			bool IsClockWise();

			// Polygon 연산을 수행하기 위하여 VertexList를 직접 사용할 수 있도록 한다.
#ifdef DOTNET
			System::Collections::Generic::List<Vertex2D^>^ GetVertexList();
#else
			std::vector<Vertex2D>& GetVertexList();
#endif
		protected:
			// rLine에서 특정 좌표가 y값을 가지는 경우 x값을 알려준다.
			// y값을 가질수 없거나 해가 무수히 많은 경우 false를 리턴한다.
			static bool GetXFromLine(REF_CONST(Line2D) rLine, double y, double* pX);
			static void CheckPointCount(REF_CONST(Vertex2D) rLineBegin, REF_CONST(Vertex2D) rLineEnd, double y, int& rCount);

		protected:
#ifdef DOTNET
			System::Collections::Generic::List<Vertex2D^>^ m_arrVertices;
#else
			std::vector<Vertex2D> m_arrVertices;
#endif
		};

		GEOMETRY_EXPORT_CLASS(PolygonF)
		{
		public:
			PolygonF(void);
			virtual ~PolygonF(void);

		public:
			int GetVertexCount() CONSTF;
			// nIndex가 배열의 범위를 벗어나면 NULL을 리턴한다.
			POINTER(Vertex2F) GetVertex(int nIndex);

			void AddVertex(REF_CONST(Vertex2F) vertex);
			bool Insert(int nIndex, REF_CONST(Vertex2F) vertex);

			bool UpdateVertex(int nIndex, REF_CONST(Vertex2F) vertex);

			bool RemoveVertex(int nIndex);
			void Clear();

			// 점이 폴리곤 내부에 있는지 검색
			// 폴리곤의 시작점과 끝점이 다를 경우, 시작점과 끝점이 연결된 폐곡선으로 간주한다.
			// 물론 폴리곤의 시작점과 끝점이 같아도 상관없다.
			// Return : 1이면 vertex가 폴리곤의 내부에 위치한다.
			//          0이면 vertex가 폴리곤의 외부에 위치한다.
			//         -1이면 vertex가 폴리곤의 경계에 위치한다.
			int HitTest(REF_CONST(Vertex2F) vertex);

			// 폴리곤의 무게중심을 구한다.
			INSTANCE(Vertex2F) CalcWeightCenter();
			float GetArea() CONSTF;

			// Bounding Rect의 Min
			INSTANCE(Vertex2F) GetMin();
			// Bounding Rect의 Max
			INSTANCE(Vertex2F) GetMax();

			// vertex와 Polygon의 가장 가까운 외곽선과의 거리
			// vertex가 Polygon의 내부에 존재할 경우 음수값을 리턴한다.
			float GetDistance(REF_CONST(Vertex2F) vertex);
			// vertex와 Polygon의 가장 가까운 외곽선과의 거리 및 가장 가까운 점을 리턴한다.
			// vertex가 Polygon의 내부에 존재할 경우 음수값을 리턴한다.
			float GetDistanceNVertex(REF_CONST(Vertex2F) vertex, OUT CBR(INSTANCE(Vertex2F)) vResult);
			bool IsClockWise();

			// Polygon 연산을 수행하기 위하여 VertexList를 직접 사용할 수 있도록 한다.
#ifdef DOTNET
			System::Collections::Generic::List<Vertex2F^>^ GetVertexList();
#else
			std::vector<Vertex2F>& GetVertexList();
#endif

		protected:
			// rLine에서 특정 좌표가 y값을 가지는 경우 x값을 알려준다.
			// y값을 가질수 없거나 해가 무수히 많은 경우 false를 리턴한다.
			static bool GetXFromLine(REF_CONST(Line2F) rLine, float y, float* pX);
			static void CheckPointCount(REF_CONST(Vertex2F) rLineBegin, REF_CONST(Vertex2F) rLineEnd, float y, int& rCount);

		protected:
#ifdef DOTNET
			System::Collections::Generic::List<Vertex2F^>^ m_arrVertices;
#else
			std::vector<Vertex2F> m_arrVertices;
#endif
		};
	}
}
