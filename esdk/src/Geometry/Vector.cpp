#include "StdAfx.h"
#include "GeometryAPI.h"
#include "GVector.h"
#include "GMath.h"
#include <Math.h>

BEGIN_NS(UnE)
BEGIN_NS(Geometry)

template <class Vertex>
static INSTANCE(Vertex) _CrossProduct(REF_CONST(Vertex) v1, REF_CONST(Vertex) v2)
{
	INSTANCE(Vertex) vResult = dnonlynew Vertex(OF(v1, y) * OF(v2, z) - OF(v1, z) * OF(v2, y), OF(v1, z) * OF(v2, x) - OF(v1, x) * OF(v2, z), OF(v1, x) * OF(v2, y) - OF(v1, y) * OF(v2, x));
	return vResult;
}

Vertex3DInstance Vector::CrossProduct(Vertex3DRefConst v1, Vertex3DRefConst v2)
{
	return _CrossProduct<Vertex3D>(v1, v2);
}

/*Vertex3FInstance Vector::CrossProduct(Vertex3FRefConst v1, Vertex3FRefConst v2)
{
	return _CrossProduct<Vertex3F>(v1, v2);
}*/

template <class Vertex, class Real>
static bool _SetUnitVector(REF(Vertex) rVector)
{
	// 인자가 없을 경우 원점에 위치한다.
	INSTANCE(Vertex) vOrigin = dnonlynew Vertex();

	Real dLen = OF(rVector, GetDistance(vOrigin));
	if (dLen <= Math::HALF_TOLERANCE())
		return false;

	INSTANCE(Vertex) vResult = rVector / dLen;
	OF(rVector, CopyFrom(vResult));
	return true;
}

// rVector를 길이 1의 단위 벡터로 만든다.
bool Vector::SetUnitVector(Vertex3DRef rVector)
{
	return _SetUnitVector<Vertex3D, double>(rVector);
}

/*bool Vector::SetUnitVector(Vertex3FRef rVector)
{
	return _SetUnitVector<Vertex3F, float>(rVector);
}*/

bool Vector::SetUnitVector(Vertex2DRef rVector)
{
	return _SetUnitVector<Vertex2D, double>(rVector);
}

/*bool Vector::SetUnitVector(Vertex2FRef rVector)
{
	return _SetUnitVector<Vertex2F, float>(rVector);
}*/

END_NS
END_NS
