#pragma once

#include "GeometryAPI.h"
#include "GVertex.h"

// Bezier Curve를 표현한다.
// [2013/01/31] 김지웅

namespace UnE
{
	namespace Geometry
	{
		GEOMETRY_EXPORT_CLASS(BezierCurve2D)
		{
		public:
			BezierCurve2D();
			
		public:
			// nCurvePoint는 반드시 0보다 크고 33보다 작거나 같아야 한다.
#ifdef DOTNET
			bool Calc(array<Vertex2D^>^ arrCurvePoints, int nCurvePointCount, array<Vertex2D^>^ arrResultPoints, int nResultCount);
#else
			bool Calc(Vertex2D arrCurvePoints[], int nCurvePointCount, Vertex2D arrResultPoints[], int nResultCount);
#endif

		protected:
			double factorial(int n);
			void CreateFactorialTable();
			double Ni(int n, int i);
			double Bernstein(int n, int i, double t);

		private:
#ifdef DOTNET 
			array<double>^ FactorialLookup;
#else
			double FactorialLookup[33];		
#endif
		};
	}
}
