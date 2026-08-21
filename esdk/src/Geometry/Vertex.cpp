#include "StdAfx.h"
#include "GVertex.h"
#include "GMath.h"
#include <math.h>

#ifdef DOTNET
using namespace System;
#endif

BEGIN_NS(UnE)
BEGIN_NS(Geometry)

template <class Vertex, class Real>
static bool _Mirror(REF_CONST(Vertex) rVertex, REF_CONST(Vertex) vLineBegin, REF_CONST(Vertex) vLineEnd, CBR(INSTANCE(Vertex)) rResult)
{
#ifdef DOTNET
	rResult = nullptr;
#endif

	if (OF(vLineBegin, GetDistance(vLineEnd)) <= Math::HALF_TOLERANCE())
		return false;

	Real dLen = OF(rVertex, GetDistance(vLineBegin));
	Real dAngle = Math::GetAngle(rVertex, vLineBegin, vLineEnd);
	Real dH = dLen * cos(dAngle);

	INSTANCE(Vertex) vCenter = Math::GetLinearVertex(vLineBegin, vLineEnd, dH);
	rResult = vCenter * 2 - rVertex;
	return true;
}

Vertex3D::Vertex3D(void)
{
}

Vertex3D::Vertex3D(double x, double y, double z)
	: Vertex3<double>(x, y, z)
{
}

Vertex3D::Vertex3D(Vertex3DRefConst rhs)
{
	x = OF(rhs, x);
	y = OF(rhs, y);
	z = OF(rhs, z);
}

Vertex3D::~Vertex3D(void)
{
}

#ifdef DOTNET
/*bool Vertex3D::operator== (Vertex3D^ op1, Vertex3D^ op2)
{
	bool isNull1 = NullChecker::IsNull(op1);
	bool isNull2 = NullChecker::IsNull(op2);

	if (isNull1 && isNull2)
		return true;
	else if (isNull1 || isNull2)
		return false;

	return op1->GetDistance(op2) <= Math::HALF_TOLERANCE() ? true : false;
}

bool Vertex3D::operator!= (Vertex3D^ op1, Vertex3D^ op2)
{
	return !(op1 == op2);
}*/

Vertex3D^ Vertex3D::operator+ (Vertex3D^ op1, Vertex3D^ op2)
{
	return gcnew Vertex3D(op1->x + op2->x, op1->y + op2->y, op1->z + op2->z);
}

Vertex3D^ Vertex3D::operator- (Vertex3D^ op1, Vertex3D^ op2)
{
	return gcnew Vertex3D(op1->x - op2->x, op1->y - op2->y, op1->z - op2->z);
}

Vertex3D^ Vertex3D::operator* (Vertex3D^ op, double data)
{
	return gcnew Vertex3D(op->x * data, op->y * data, op->z * data);
}

Vertex3D^ Vertex3D::operator/ (Vertex3D^ op, double data)
{
	if (data <= Math::COORD_TOLERANCE()) throw gcnew System::DivideByZeroException;
	return gcnew Vertex3D(op->x / data, op->y / data, op->z / data);
}

#else
/*bool Vertex3D::operator== (const Vertex3D& rhs) const
{
	return GetDistance(rhs) <= Math::HALF_TOLERANCE() ? true : false;
}

bool Vertex3D::operator!= (const Vertex3D& rhs) const
{
	return !(*this == rhs);
}*/

Vertex3D Vertex3D::operator+ (const Vertex3D& rhs) const
{
	return Vertex3D(x + rhs.x, y + rhs.y, z + rhs.z);
}

Vertex3D Vertex3D::operator- (const Vertex3D& rhs) const
{
	return Vertex3D(x - rhs.x, y - rhs.y, z - rhs.z);
}

Vertex3D Vertex3D::operator* (double data) const
{
	return Vertex3D(x * data, y * data, z * data);
}

Vertex3D Vertex3D::operator/ (double data) const
{
	if (data <= Math::COORD_TOLERANCE()) throw L"0으로 나누기를 시도하고 있습니다.";
	return Vertex3D(x / data, y / data, z / data);
}
#endif

double Vertex3D::GetDistance(REF_CONST(Vertex3<double>) rVertex) CONST
{
	double _x = this->x - OF(rVertex, x);
	double _y = this->y - OF(rVertex, y);
	double _z = this->z - OF(rVertex, z);

	return sqrt(_x * _x + _y * _y + _z * _z);
}

// v1과 v2를 지나는 직선을 기준으로 현재의 버텍스와 대칭되는 객체를 만들어 리턴한다.
// v1과 v2가 동일한 좌표일 경우 false를 리턴한다.
bool Vertex3D::Mirror(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2, OUT CBR(INSTANCE(Vertex3D)) rResult) CONST
{
	return _Mirror<Vertex3D, double>(THIS_OBJ, v1, v2, rResult);
}

// v1, v2, v3를 지나는 평면을 기준으로 현재의 버텍스와 대칭되는 객체를 만들어 리턴한다.
// v1, v2, v3가 평면을 구성하지 못할 경우 false를 리턴한다.
bool Vertex3D::Mirror(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2, REF_CONST(Vertex3D) v3, OUT CBR(INSTANCE(Vertex3D)) rResult) CONST
{
#ifdef DOTNET
	rResult = nullptr;
#endif

	double a, b, c, d;	// ax + by + cz + d = 0
	if (!Math::MakePlane(v1, v2, v3, a, b, c, d))
		return false;

	INSTANCE(Vertex3D) vTarget = Math::GetNearestVertex(THIS_OBJ, a, b, c, d);
	rResult = vTarget * 2 - THIS_OBJ;
	return true;
}

Vertex3F::Vertex3F(void)
{
}

Vertex3F::Vertex3F(float x, float y, float z)
	: Vertex3<float>(x, y, z)
{
}

Vertex3F::Vertex3F(Vertex3FRefConst rhs)
{
	x = OF(rhs, x);
	y = OF(rhs, y);
	z = OF(rhs, z);
}

Vertex3F::~Vertex3F(void)
{
}

#ifdef DOTNET
/*bool Vertex3F::operator== (Vertex3F^ op1, Vertex3F^ op2)
{
	bool isNull1 = NullChecker::IsNull(op1);
	bool isNull2 = NullChecker::IsNull(op2);

	if (isNull1 && isNull2)
		return true;
	else if (isNull1 || isNull2)
		return false;

	return op1->GetDistance(op2) <= Math::HALF_TOLERANCE() ? true : false;
}

bool Vertex3F::operator!= (Vertex3F^ op1, Vertex3F^ op2)
{
	return !(op1 == op2);
}*/

Vertex3F^ Vertex3F::operator+ (Vertex3F^ op1, Vertex3F^ op2)
{
	return gcnew Vertex3F(op1->x + op2->x, op1->y + op2->y, op1->z + op2->z);
}

Vertex3F^ Vertex3F::operator- (Vertex3F^ op1, Vertex3F^ op2)
{
	return gcnew Vertex3F(op1->x - op2->x, op1->y - op2->y, op1->z - op2->z);
}

Vertex3F^ Vertex3F::operator* (Vertex3F^ op, float data)
{
	return gcnew Vertex3F(op->x * data, op->y * data, op->z * data);
}

Vertex3F^ Vertex3F::operator/ (Vertex3F^ op, float data)
{
	if (data <= Math::HALF_TOLERANCE()) throw gcnew System::DivideByZeroException;
	return gcnew Vertex3F(op->x / data, op->y / data, op->z / data);
}

#else
/*bool Vertex3F::operator== (const Vertex3F& rhs) const
{
	return GetDistance(rhs) <= Math::HALF_TOLERANCE() ? true : false;
}

bool Vertex3F::operator!= (const Vertex3F& rhs) const
{
	return !(*this == rhs);
}*/

Vertex3F Vertex3F::operator+ (const Vertex3F& rhs) const
{
	return Vertex3F(x + rhs.x, y + rhs.y, z + rhs.z);
}

Vertex3F Vertex3F::operator- (const Vertex3F& rhs) const
{
	return Vertex3F(x - rhs.x, y - rhs.y, z - rhs.z);
}

Vertex3F Vertex3F::operator* (float data) const
{
	return Vertex3F(x * data, y * data, z * data);
}

Vertex3F Vertex3F::operator/ (float data) const
{
	if (data <= Math::HALF_TOLERANCE()) throw L"0으로 나누기를 시도하고 있습니다.";
	return Vertex3F(x / data, y / data, z / data);
}
#endif

float Vertex3F::GetDistance(REF_CONST(Vertex3<float>) rVertex) CONST
{
	double _x = this->x - OF(rVertex, x);
	double _y = this->y - OF(rVertex, y);
	double _z = this->z - OF(rVertex, z);

	return (float)sqrt(_x * _x + _y * _y + _z * _z);
}

// v1과 v2를 지나는 직선을 기준으로 현재의 버텍스와 대칭되는 객체를 만들어 리턴한다.
// v1과 v2가 동일한 좌표일 경우 false를 리턴한다.
bool Vertex3F::Mirror(REF_CONST(Vertex3F) v1, REF_CONST(Vertex3F) v2, OUT CBR(INSTANCE(Vertex3F)) rResult) CONST
{
	INSTANCE(Vertex3D) _THIS = dnonlynew Vertex3D(this->x, this->y, this->z);

	INSTANCE(Vertex3D) _v1 = dnonlynew Vertex3D(OF(v1, x), OF(v1, y), OF(v1, z));
	INSTANCE(Vertex3D) _v2 = dnonlynew Vertex3D(OF(v2, x), OF(v2, y), OF(v2, z));
	INSTANCE(Vertex3D) _vResult = dnonlynew Vertex3D();

	bool isSuccess = _Mirror<Vertex3D, double>(_THIS, _v1, _v2, _vResult);
	if (!isSuccess) return false;

	OF(rResult, x) = (float)OF(_vResult, x);
	OF(rResult, y) = (float)OF(_vResult, y);
	OF(rResult, z) = (float)OF(_vResult, z);

	return true;

	//return _Mirror<Vertex3F, float>(THIS_OBJ, v1, v2, rResult);
}

// v1, v2, v3를 지나는 평면을 기준으로 현재의 버텍스와 대칭되는 객체를 만들어 리턴한다.
// v1, v2, v3가 평면을 구성하지 못할 경우 false를 리턴한다.
bool Vertex3F::Mirror(REF_CONST(Vertex3F) v1, REF_CONST(Vertex3F) v2, REF_CONST(Vertex3F) v3, OUT CBR(INSTANCE(Vertex3F)) rResult) CONST
{
#ifdef DOTNET
	rResult = nullptr;
#endif

	double a, b, c, d;	// ax + by + cz + d = 0
	INSTANCE(Vertex3D) _v1 = dnonlynew Vertex3D(OF(v1, x), OF(v1, y), OF(v1, z));
	INSTANCE(Vertex3D) _v2 = dnonlynew Vertex3D(OF(v2, x), OF(v2, y), OF(v2, z));
	INSTANCE(Vertex3D) _v3 = dnonlynew Vertex3D(OF(v3, x), OF(v3, y), OF(v3, z));

	if (!Math::MakePlane(_v1, _v2, _v3, a, b, c, d))
		return false;

	INSTANCE(Vertex3D) _THIS = dnonlynew Vertex3D(this->x, this->y, this->z);

	INSTANCE(Vertex3D) vTarget = Math::GetNearestVertex(_THIS, a, b, c, d);
	INSTANCE(Vertex3D) vResult = vTarget * 2 - _THIS;

	OF(rResult, x) = (float)OF(vResult, x);
	OF(rResult, y) = (float)OF(vResult, y);
	OF(rResult, z) = (float)OF(vResult, z);
	return true;
}

Vertex2D::Vertex2D(void)
{
}

Vertex2D::Vertex2D(double x, double y)
	: Vertex2<double>(x, y)
{
}

Vertex2D::Vertex2D(Vertex2DRefConst rhs)
{
	x = OF(rhs, x);
	y = OF(rhs, y);
}

Vertex2D::~Vertex2D(void)
{
}

#ifdef DOTNET
/*bool Vertex2D::operator== (Vertex2D^ op1, Vertex2D^ op2)
{
	bool isNull1 = NullChecker::IsNull(op1);
	bool isNull2 = NullChecker::IsNull(op2);

	if (isNull1 && isNull2)
		return true;
	else if (isNull1 || isNull2)
		return false;

	return op1->GetDistance(op2) <= Math::HALF_TOLERANCE() ? true : false;
}

bool Vertex2D::operator!= (Vertex2D^ op1, Vertex2D^ op2)
{
	return !(op1 == op2);
}*/

Vertex2D^ Vertex2D::operator+ (Vertex2D^ op1, Vertex2D^ op2)
{
	return gcnew Vertex2D(op1->x + op2->x, op1->y + op2->y);
}

Vertex2D^ Vertex2D::operator- (Vertex2D^ op1, Vertex2D^ op2)
{
	return gcnew Vertex2D(op1->x - op2->x, op1->y - op2->y);
}

Vertex2D^ Vertex2D::operator* (Vertex2D^ op, double data)
{
	return gcnew Vertex2D(op->x * data, op->y * data);
}

Vertex2D^ Vertex2D::operator/ (Vertex2D^ op, double data)
{
	if (data <= Math::COORD_TOLERANCE()) throw gcnew System::DivideByZeroException;
	return gcnew Vertex2D(op->x / data, op->y / data);
}

#else
/*bool Vertex2D::operator== (const Vertex2D& rhs) const
{
	return GetDistance(rhs) <= Math::HALF_TOLERANCE() ? true : false;
}

bool Vertex2D::operator!= (const Vertex2D& rhs) const
{
	return !(*this == rhs);
}*/

Vertex2D Vertex2D::operator+ (const Vertex2D& rhs) const
{
	return Vertex2D(x + rhs.x, y + rhs.y);
}

Vertex2D Vertex2D::operator- (const Vertex2D& rhs) const
{
	return Vertex2D(x - rhs.x, y - rhs.y);
}

Vertex2D Vertex2D::operator* (double data) const
{
	return Vertex2D(x * data, y * data);
}

Vertex2D Vertex2D::operator/ (double data) const
{
	if (data <= Math::COORD_TOLERANCE()) throw L"0으로 나누기를 시도하고 있습니다.";
	return Vertex2D(x / data, y / data);
}
#endif

double Vertex2D::GetDistance(REF_CONST(Vertex2<double>) rVertex) CONST
{
	double _x = this->x - OF(rVertex, x);
	double _y = this->y - OF(rVertex, y);

	return sqrt(_x * _x + _y * _y);
}

// v1과 v2를 지나는 직선을 기준으로 현재의 버텍스와 대칭되는 객체를 만들어 리턴한다.
// v1과 v2가 동일한 좌표일 경우 false를 리턴한다.
bool Vertex2D::Mirror(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2, OUT CBR(INSTANCE(Vertex2D)) rResult) CONST
{
	return _Mirror<Vertex2D, double>(THIS_OBJ, v1, v2, rResult);
}

Vertex2F::Vertex2F(void)
{
}

Vertex2F::Vertex2F(float x, float y)
	: Vertex2<float>(x, y)
{
}

Vertex2F::Vertex2F(Vertex2FRefConst rhs)
{
	x = OF(rhs, x);
	y = OF(rhs, y);
}

Vertex2F::~Vertex2F(void)
{
}

#ifdef DOTNET
/*bool Vertex2F::operator== (Vertex2F^ op1, Vertex2F^ op2)
{
	bool isNull1 = NullChecker::IsNull(op1);
	bool isNull2 = NullChecker::IsNull(op2);

	if (isNull1 && isNull2)
		return true;
	else if (isNull1 || isNull2)
		return false;

	return op1->GetDistance(op2) <= Math::HALF_TOLERANCE() ? true : false;
}

bool Vertex2F::operator!= (Vertex2F^ op1, Vertex2F^ op2)
{
	return !(op1 == op2);
}*/

Vertex2F^ Vertex2F::operator+ (Vertex2F^ op1, Vertex2F^ op2)
{
	return gcnew Vertex2F(op1->x + op2->x, op1->y + op2->y);
}

Vertex2F^ Vertex2F::operator- (Vertex2F^ op1, Vertex2F^ op2)
{
	return gcnew Vertex2F(op1->x - op2->x, op1->y - op2->y);
}

Vertex2F^ Vertex2F::operator* (Vertex2F^ op, float data)
{
	return gcnew Vertex2F(op->x * data, op->y * data);
}

Vertex2F^ Vertex2F::operator/ (Vertex2F^ op, float data)
{
	if (data <= Math::HALF_TOLERANCE()) throw gcnew System::DivideByZeroException;
	return gcnew Vertex2F(op->x / data, op->y / data);
}

#else
/*bool Vertex2F::operator== (const Vertex2F& rhs) const
{
	return GetDistance(rhs) <= Math::HALF_TOLERANCE() ? true : false;
}

bool Vertex2F::operator!= (const Vertex2F& rhs) const
{
	return !(*this == rhs);
}*/

Vertex2F Vertex2F::operator+ (const Vertex2F& rhs) const
{
	return Vertex2F(x + rhs.x, y + rhs.y);
}

Vertex2F Vertex2F::operator- (const Vertex2F& rhs) const
{
	return Vertex2F(x - rhs.x, y - rhs.y);
}

Vertex2F Vertex2F::operator* (float data) const
{
	return Vertex2F(x * data, y * data);
}

Vertex2F Vertex2F::operator/ (float data) const
{
	if (data <= Math::COORD_TOLERANCE()) throw L"0으로 나누기를 시도하고 있습니다.";
	return Vertex2F(x / data, y / data);
}
#endif

float Vertex2F::GetDistance(REF_CONST(Vertex2<float>) rVertex) CONST
{
	double _x = this->x - OF(rVertex, x);
	double _y = this->y - OF(rVertex, y);

	return (float)sqrt(_x * _x + _y * _y);
}

// v1과 v2를 지나는 직선을 기준으로 현재의 버텍스와 대칭되는 객체를 만들어 리턴한다.
// v1과 v2가 동일한 좌표일 경우 false를 리턴한다.
bool Vertex2F::Mirror(REF_CONST(Vertex2F) v1, REF_CONST(Vertex2F) v2, OUT CBR(INSTANCE(Vertex2F)) rResult) CONST
{
	INSTANCE(Vertex2D) _THIS = dnonlynew Vertex2D(this->x, this->y);

	INSTANCE(Vertex2D) _v1 = dnonlynew Vertex2D(OF(v1, x), OF(v1, y));
	INSTANCE(Vertex2D) _v2 = dnonlynew Vertex2D(OF(v2, x), OF(v2, y));
	INSTANCE(Vertex2D) _vResult = dnonlynew Vertex2D();
	
	bool isSuccess = _Mirror<Vertex2D, double>(_THIS, _v1, _v2, _vResult);
	if (!isSuccess) return false;

	OF(rResult, x) = (float)OF(_vResult, x);
	OF(rResult, y) = (float)OF(_vResult, y);

	return true;

	//return _Mirror<Vertex2D, double>(THIS_OBJ, v1, v2, rResult);
}

END_NS
END_NS
