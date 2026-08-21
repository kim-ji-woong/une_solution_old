#pragma once

#include "GeometryAPI.h"
#include "GVertex.h"

namespace UnE
{
	namespace Geometry
	{
		GEOMETRY_DECLARE_EXPORT_CLASS(Vector)
		{
		public:
			// 벡터의 외적
			static Vertex3DInstance CrossProduct(Vertex3DRefConst v1, Vertex3DRefConst v2);
			//static Vertex3FInstance CrossProduct(Vertex3FRefConst v1, Vertex3FRefConst v2);

			// rVector를 길이 1의 단위 벡터로 만든다.
			static bool SetUnitVector(Vertex3DRef rVector);
			//static bool SetUnitVector(Vertex3FRef rVector);
			static bool SetUnitVector(Vertex2DRef rVector);
			//static bool SetUnitVector(Vertex2FRef rVector);
		};
	}
}
