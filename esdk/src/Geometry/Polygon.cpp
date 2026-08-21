#include "StdAfx.h"
#include "GPolygon.h"
#include "GLine.h"
#include "GMath.h"


#include <float.h>

BEGIN_NS(UnE)
BEGIN_NS(Geometry)

Polygon::Polygon(void)
{
#ifdef DOTNET
	m_arrVertices = gcnew System::Collections::Generic::List<Vertex2D^>();
#endif
}

Polygon::~Polygon(void)
{
}

int Polygon::GetVertexCount() CONSTF
{
#ifdef DOTNET
	return m_arrVertices->Count;
#else
	return (int)m_arrVertices.size();
#endif
}

// nIndex가 배열의 범위를 벗어나면 NULL을 리턴한다.
POINTER(Vertex2D) Polygon::GetVertex(int nIndex)
{
	if (nIndex >= GetVertexCount())
		return NULL_PTR;

#ifdef DOTNET
	return (Vertex2D^)m_arrVertices[nIndex];
#else
	return &m_arrVertices[nIndex];
#endif
}

void Polygon::AddVertex(REF_CONST(Vertex2D) vertex)
{
#ifdef DOTNET
	m_arrVertices->Add(vertex);
#else
	m_arrVertices.push_back(vertex);
#endif
}

bool Polygon::Insert(int nIndex, REF_CONST(Vertex2D) vertex)
{
	if (nIndex >= GetVertexCount())
		return false;

#ifdef DOTNET
	m_arrVertices->Insert(nIndex, vertex);
#else
	m_arrVertices.insert(m_arrVertices.begin() + nIndex, vertex);
#endif

	return true;
}

bool Polygon::UpdateVertex(int nIndex, REF_CONST(Vertex2D) vertex)
{
	if (nIndex >= GetVertexCount())
		return false;

	REF(Vertex2D) rVertex = (REF(Vertex2D))m_arrVertices[nIndex];
	
	OF(rVertex, x) = OF(vertex, x);
	OF(rVertex, y) = OF(vertex, y);

	return true;
}

bool Polygon::RemoveVertex(int nIndex)
{
	if (nIndex >= GetVertexCount())
		return false;

#ifdef DOTNET
	m_arrVertices->RemoveAt(nIndex);
#else
	m_arrVertices.erase(m_arrVertices.begin() + nIndex);
#endif

	return true;
}

void Polygon::Clear()
{
#ifdef DOTNET
	m_arrVertices->Clear();
#else
	m_arrVertices.clear();
#endif
}

// rLine에서 특정 좌표가 y값을 가지는 경우 x값을 알려준다.
// y값을 가질수 없거나 해가 무수히 많은 경우 false를 리턴한다.
bool Polygon::GetXFromLine(REF_CONST(Line2D) rLine, double y, double* pX)
{
	REF_CONST(Vertex2D) rBegin = OF(rLine, GetVertex(true));
	REF_CONST(Vertex2D) rEnd = OF(rLine, GetVertex(false));

	if (OF(rBegin, y) == OF(rEnd, y))
	{
		if (y == OF(rBegin, y))
		{
			*pX = (OF(rBegin, x) + OF(rEnd, x)) / 2;
			return true;
		}
		else
			return false;
	}

	*pX = (OF(rEnd, x) - OF(rBegin, x)) / (OF(rEnd, y) - OF(rBegin, y)) * (y - OF(rBegin, y)) + OF(rBegin, x);

#ifdef DOTNET
	Line2D::LineType noLimitLineType = Line2D::LineType::LINE;
#else
	Line2D::LineType noLimitLineType = Line2D::LINE;
#endif

	if (OF(rLine, GetLineType()) == noLimitLineType)
		return true;

	if (OF(rBegin, x) < OF(rEnd, x))
	{
		if (*pX < OF(rBegin, x) - Geometry::Math::HALF_TOLERANCE() || *pX > OF(rEnd, x) + Geometry::Math::HALF_TOLERANCE())
			return false;
	}
	else
	{
		if (*pX < OF(rEnd, x) - Geometry::Math::HALF_TOLERANCE() || *pX > OF(rBegin, x) + Geometry::Math::HALF_TOLERANCE())
			return false;
	}

	if (OF(rBegin, y) < OF(rEnd, y))
	{
		if (y < OF(rBegin, y) - Geometry::Math::HALF_TOLERANCE() || y > OF(rEnd, y) + Geometry::Math::HALF_TOLERANCE())
			return false;
	}
	else
	{
		if (y < OF(rEnd, y) - Geometry::Math::HALF_TOLERANCE() || y > OF(rBegin, y) + Geometry::Math::HALF_TOLERANCE())
			return false;
	}

	return true;
}

void Polygon::CheckPointCount(REF_CONST(Vertex2D) rLineBegin, REF_CONST(Vertex2D) rLineEnd, double y, int& rCount)
{
	double dMaxY, dMinY;
	
	if (OF(rLineBegin, y) < OF(rLineEnd, y))
	{
		dMinY = OF(rLineBegin, y);
		dMaxY = OF(rLineEnd, y);
	}
	else
	{
		dMinY = OF(rLineEnd, y);
		dMaxY = OF(rLineBegin, y);
	}

	// y가 rLineBegin과 rLineEnd 사이에 있거나, y가 둘 중 최소점과 일치하는 경우 rCount를 증가시킨다.
	// y가 둘 중 최대점과 일치하는 경우 rCount를 증가시키지 않는다.
	if (y < dMaxY - Geometry::Math::HALF_TOLERANCE() && y >= dMinY - Geometry::Math::HALF_TOLERANCE())
	{
		rCount++;
	}
}

// 점이 폴리곤 내부에 있는지 검색
// 폴리곤의 시작점과 끝점이 다를 경우, 시작점과 끝점이 연결된 폐곡선으로 간주한다.
// 물론 폴리곤의 시작점과 끝점이 같아도 상관없다.
// Return : 1이면 vertex가 폴리곤의 내부에 위치한다.
//          0이면 vertex가 폴리곤의 외부에 위치한다.
//         -1이면 vertex가 폴리곤의 경계에 위치한다.
int Polygon::HitTest(REF_CONST(Vertex2D) vertex)
{
	int nVertexCount = GetVertexCount();
	if (nVertexCount < 3)
		return 0;

	// 시작점과 끝점이 같은지 검사한다.
	REF_CONST(Vertex2D) vFirst = (REF_CONST(Vertex2D))m_arrVertices[0];
	REF_CONST(Vertex2D) vLast = (REF_CONST(Vertex2D))m_arrVertices[nVertexCount - 1];
	bool isFirstLastSame = OF(vFirst, GetDistance(vLast)) <= Math::HALF_TOLERANCE();
	//bool isFirstLastSame = (REF_CONST(Vertex2D))m_arrVertices[0] == (REF_CONST(Vertex2D))m_arrVertices[nVertexCount - 1];
	if (isFirstLastSame) nVertexCount--;

	REF_CONST(Vertex2D) rBeginVertex = (REF_CONST(Vertex2D))m_arrVertices[0];
	REF_CONST(Vertex2D) rEndVertex = (REF_CONST(Vertex2D))m_arrVertices[nVertexCount - 1];

	double x;
	int nCount = 0;
	PTR_CONST(Vertex2D) pPrev = POINTER_ADDR(rEndVertex);
	
	for (int i=0;i<nVertexCount;i++)
	{
		REF_CONST(Vertex2D) rVertex = (REF_CONST(Vertex2D))m_arrVertices[i];
		INSTANCE(Line2D) line = dnonlynew Line2D(POINTER_VALUE(pPrev), rVertex);

		if (OF(line, IsInclude(vertex)))
			return -1;
		else
		{
			double diff = pPrev->y - OF(rVertex, y);
			if (diff < 0) diff = -diff;

			// X축과 평행한 선분은 계산하지 않는다.
			if (diff > Geometry::Math::HALF_TOLERANCE())
			{
				if (GetXFromLine(line, OF(vertex, y), &x))
				{
					diff = x - OF(vertex, x);
					if (diff < 0.0) diff = -diff;

					if (diff <= Geometry::Math::GetTolerance(x))
						return -1;
					else if (x > OF(vertex, x))
					{
						CheckPointCount(POINTER_VALUE(pPrev), rVertex, OF(vertex, y), nCount);
					}
				}
			}
		}

		pPrev = POINTER_ADDR(rVertex);
	}

	if (nCount % 2 == 0)
		return 0;

	return 1;
}

// vertex와 Polygon의 가장 가까운 외곽선과의 거리
// vertex가 Polygon의 내부에 존재할 경우 음수값을 리턴한다.
double Polygon::GetDistance(REF_CONST(Vertex2D) vertex)
{
	INSTANCE(Vertex2D) vResult;
	return GetDistanceNVertex(vertex, vResult);
	/*int nVertexCount = GetVertexCount();
	if (nVertexCount < 3)
		return 0.0;

	// 시작점과 끝점이 같은지 검사한다.
	bool isFirstLastSame = (REF_CONST(Vertex2D))m_arrVertices[0] == (REF_CONST(Vertex2D))m_arrVertices[nVertexCount - 1];
	if (isFirstLastSame) nVertexCount--;

	REF_CONST(Vertex2D) rBeginVertex = (REF_CONST(Vertex2D))m_arrVertices[0];
	REF_CONST(Vertex2D) rEndVertex = (REF_CONST(Vertex2D))m_arrVertices[nVertexCount - 1];

	double x, minDistance = -1.0;
	int nCount = 0;
	PTR_CONST(Vertex2D) pPrev = POINTER_ADDR(rEndVertex);
	
	for (int i=0;i<nVertexCount;i++)
	{
		REF_CONST(Vertex2D) rVertex = (REF_CONST(Vertex2D))m_arrVertices[i];
		INSTANCE(Line2D) line = dnonlynew Line2D(POINTER_VALUE(pPrev), rVertex);

		double distance = OF(line, GetDistance(vertex, false));

		if (minDistance < 0.0 || minDistance > distance)
			minDistance = distance;
		
		if (OF(line, IsInclude(vertex)))
		{
			// vertex가 외곽선내에 포함되어 있다.
			return 0.0;
		}
		else
		{
			double diff = pPrev->y - OF(rVertex, y);
			if (diff < 0) diff = -diff;

			// X축과 평행한 선분은 계산하지 않는다.
			if (diff > Geometry::Math::HALF_TOLERANCE())
			{
				if (GetXFromLine(line, OF(vertex, y), &x))
				{
					diff = x - OF(vertex, x);
					if (diff < 0.0) diff = -diff;

					if (diff <= Geometry::Math::GetTolerance(x))
						return -1;
					else if (x > OF(vertex, x))
					{
						CheckPointCount(POINTER_VALUE(pPrev), rVertex, OF(vertex, y), nCount);
					}
				}
			}
		}

		pPrev = POINTER_ADDR(rVertex);
	}

	if (nCount % 2 == 0)
		return minDistance;

	// vertex가 폴리곤의 내부에 위치한다.
	return -minDistance;*/
}

// vertex와 Polygon의 가장 가까운 외곽선과의 거리 및 가장 가까운 점을 리턴한다.
// vertex가 Polygon의 내부에 존재할 경우 음수값을 리턴한다.
double Polygon::GetDistanceNVertex(REF_CONST(Vertex2D) vertex, OUT CBR(INSTANCE(Vertex2D)) vResult)
{
	vResult = dnonlynew Vertex2D();

	int nVertexCount = GetVertexCount();
	if (nVertexCount < 3)
		return 0.0;

	// 시작점과 끝점이 같은지 검사한다.
	REF_CONST(Vertex2D) vFirst = (REF_CONST(Vertex2D))m_arrVertices[0];
	REF_CONST(Vertex2D) vLast = (REF_CONST(Vertex2D))m_arrVertices[nVertexCount - 1];
	bool isFirstLastSame = OF(vFirst, GetDistance(vLast)) <= Math::HALF_TOLERANCE();
	//bool isFirstLastSame = (REF_CONST(Vertex2D))m_arrVertices[0] == (REF_CONST(Vertex2D))m_arrVertices[nVertexCount - 1];
	if (isFirstLastSame) nVertexCount--;

	REF_CONST(Vertex2D) rBeginVertex = (REF_CONST(Vertex2D))m_arrVertices[0];
	REF_CONST(Vertex2D) rEndVertex = (REF_CONST(Vertex2D))m_arrVertices[nVertexCount - 1];

	double x, minDistance = -1.0;
	int nCount = 0;
	PTR_CONST(Vertex2D) pPrev = POINTER_ADDR(rEndVertex);

	for (int i = 0; i<nVertexCount; i++)
	{
		REF_CONST(Vertex2D) rVertex = (REF_CONST(Vertex2D))m_arrVertices[i];
		INSTANCE(Line2D) line = dnonlynew Line2D(POINTER_VALUE(pPrev), rVertex);

		//double distance = OF(line, GetDistance(vertex, false));
		INSTANCE(Vertex2D) vNear = Geometry::Math::GetNearestVertex(vertex, POINTER_VALUE(pPrev), rVertex, false);
		double distance = OF(vertex, GetDistance(vNear));

		if (minDistance < 0.0 || minDistance > distance)
		{
			OF(vResult, SetVertex(OF(vNear, x), OF(vNear, y)));
			minDistance = distance;
		}

		if (OF(line, IsInclude(vertex)))
		{
			// vertex가 외곽선내에 포함되어 있다.
			return 0.0;
		}
		else
		{
			double diff = pPrev->y - OF(rVertex, y);
			if (diff < 0) diff = -diff;

			// X축과 평행한 선분은 계산하지 않는다.
			if (diff > Geometry::Math::HALF_TOLERANCE())
			{
				if (GetXFromLine(line, OF(vertex, y), &x))
				{
					diff = x - OF(vertex, x);
					if (diff < 0.0) diff = -diff;

					if (diff <= Geometry::Math::GetTolerance(x))
						return -1;
					else if (x > OF(vertex, x))
					{
						CheckPointCount(POINTER_VALUE(pPrev), rVertex, OF(vertex, y), nCount);
					}
				}
			}
		}

		pPrev = POINTER_ADDR(rVertex);
	}

	if (nCount % 2 == 0)
		return minDistance;

	// vertex가 폴리곤의 내부에 위치한다.
	return -minDistance;
}

// 폴리곤의 무게중심을 구한다.
INSTANCE(Vertex2D) Polygon::CalcWeightCenter()
{
	INSTANCE(Vertex2D) vCenter = dnonlynew Vertex2D();
	int nVertexCount = GetVertexCount();
	if (nVertexCount < 3)
		return vCenter;

	double dArea = 0.0;

	// For all vertices
	int i = 0;
	for (i = 0; i < nVertexCount; ++i)
	{
		int nIndex2 = (i + 1) % nVertexCount;

		REF_CONST(Vertex2D) v1 = (REF_CONST(Vertex2D))m_arrVertices[i];
		REF_CONST(Vertex2D) v2 = (REF_CONST(Vertex2D))m_arrVertices[nIndex2];

		double a = OF(v1, x) * OF(v2, y) - OF(v2, x) * OF(v1, y);
		dArea += a;
		OF(vCenter, x) += (OF(v1, x) + OF(v2, x)) * a;
		OF(vCenter, y) += (OF(v1, y) + OF(v2, y)) * a;
	}

	dArea *= 0.5;
	OF(vCenter, x) /= (6.0 * dArea);
	OF(vCenter, y) /= (6.0 * dArea);

	return vCenter;
	/*INSTANCE(Vertex2D) vCenter = dnonlynew Vertex2D();

	int nVertexCount = GetVertexCount();
	if (nVertexCount < 3)
		return vCenter;

	for (int i=0;i<nVertexCount;i++)
	{
		REF_CONST(Vertex2D) rVertex = (REF_CONST(Vertex2D))m_arrVertices[i];

		OF(vCenter, x) += OF(rVertex, x);
		OF(vCenter, y) += OF(rVertex, y);
	}

	OF(vCenter, x) /= nVertexCount;
	OF(vCenter, y) /= nVertexCount;

	return vCenter;*/
}

INSTANCE(Vertex2D) Polygon::GetMin()
{
	INSTANCE(Vertex2D) vCenter = dnonlynew Vertex2D();

	int nVertexCount = GetVertexCount();
	if (nVertexCount < 3)
		return vCenter;

	double max_x = DBL_MAX;
	double max_y = DBL_MAX;
	for (int i=0;i<nVertexCount;i++)
	{
		REF_CONST(Vertex2D) rVertex = (REF_CONST(Vertex2D))m_arrVertices[i];

		if( max_x > OF(rVertex, x))
		{
			max_x = OF(rVertex, x);
		}

		if( max_y > OF(rVertex, y))
		{
			max_y = OF(rVertex, y);
		}
	}

	OF(vCenter, x) = max_x;
	OF(vCenter, y) = max_y;

	return vCenter;
}
// Bounding Rect의 Max
INSTANCE(Vertex2D) Polygon::GetMax()
{
	INSTANCE(Vertex2D) vCenter = dnonlynew Vertex2D();

	int nVertexCount = GetVertexCount();
	if (nVertexCount < 3)
		return vCenter;

	double max_x = -DBL_MIN;
	double max_y = -DBL_MIN;
	for (int i=0;i<nVertexCount;i++)
	{
		REF_CONST(Vertex2D) rVertex = (REF_CONST(Vertex2D))m_arrVertices[i];

		if( max_x < OF(rVertex, x))
		{
			max_x = OF(rVertex, x);
		}

		if( max_y < OF(rVertex, y))
		{
			max_y = OF(rVertex, y);
		}
	}

	OF(vCenter, x) = max_x;
	OF(vCenter, y) = max_y;

	return vCenter;
}

double Polygon::GetArea() CONSTF
{
	double dArea = 0.0;
	int nVertexCount = GetVertexCount();

	for (int i = 0; i < nVertexCount; i++)
	{
		int nSecondIndex = (i + 1) % nVertexCount;

		REF_CONST(Vertex2D) v1 = (REF_CONST(Vertex2D))m_arrVertices[i];
		REF_CONST(Vertex2D) v2 = (REF_CONST(Vertex2D))m_arrVertices[nSecondIndex];

		dArea += OF(v1, x) * OF(v2, y) - OF(v2, x) * OF(v1, y);
	}

	if (dArea < 0.0)
		return -dArea / 2;
	return dArea / 2;
}

// Polygon 연산을 수행하기 위하여 VertexList를 직접 사용할 수 있도록 한다.
#ifdef DOTNET
System::Collections::Generic::List<Vertex2D^>^ Polygon::GetVertexList()
{
	return m_arrVertices;
}
#else
std::vector<Vertex2D>& Polygon::GetVertexList()
{
	return m_arrVertices;
}
#endif

bool Polygon::IsClockWise()
{
	double sum = 0.0;
	int nVertexCount = GetVertexCount();

	for (int i = 0; i < nVertexCount; i++)
	{
		POINTER(Vertex2D) v1 = GetVertex(i);
		POINTER(Vertex2D) v2 = GetVertex((i + 1) % nVertexCount);
		sum += (v2->x - v1->x) * (v2->y + v1->y);
	}

	return sum > 0.0;
}

PolygonF::PolygonF(void)
{
#ifdef DOTNET
	m_arrVertices = gcnew System::Collections::Generic::List<Vertex2F^>();
#endif
}

PolygonF::~PolygonF(void)
{
}

int PolygonF::GetVertexCount() CONSTF
{
#ifdef DOTNET
	return m_arrVertices->Count;
#else
	return (int)m_arrVertices.size();
#endif
}

// nIndex가 배열의 범위를 벗어나면 NULL을 리턴한다.
POINTER(Vertex2F) PolygonF::GetVertex(int nIndex)
{
	if (nIndex >= GetVertexCount())
		return NULL_PTR;

#ifdef DOTNET
	return (Vertex2F^)m_arrVertices[nIndex];
#else
	return &m_arrVertices[nIndex];
#endif
}

void PolygonF::AddVertex(REF_CONST(Vertex2F) vertex)
{
#ifdef DOTNET
	m_arrVertices->Add(vertex);
#else
	m_arrVertices.push_back(vertex);
#endif
}

bool PolygonF::Insert(int nIndex, REF_CONST(Vertex2F) vertex)
{
	if (nIndex >= GetVertexCount())
		return false;

#ifdef DOTNET
	m_arrVertices->Insert(nIndex, vertex);
#else
	m_arrVertices.insert(m_arrVertices.begin() + nIndex, vertex);
#endif

	return true;
}

bool PolygonF::UpdateVertex(int nIndex, REF_CONST(Vertex2F) vertex)
{
	if (nIndex >= GetVertexCount())
		return false;

	REF(Vertex2F) rVertex = (REF(Vertex2F))m_arrVertices[nIndex];

	OF(rVertex, x) = OF(vertex, x);
	OF(rVertex, y) = OF(vertex, y);

	return true;
}

bool PolygonF::RemoveVertex(int nIndex)
{
	if (nIndex >= GetVertexCount())
		return false;

#ifdef DOTNET
	m_arrVertices->RemoveAt(nIndex);
#else
	m_arrVertices.erase(m_arrVertices.begin() + nIndex);
#endif

	return true;
}

void PolygonF::Clear()
{
#ifdef DOTNET
	m_arrVertices->Clear();
#else
	m_arrVertices.clear();
#endif
}

// rLine에서 특정 좌표가 y값을 가지는 경우 x값을 알려준다.
// y값을 가질수 없거나 해가 무수히 많은 경우 false를 리턴한다.
bool PolygonF::GetXFromLine(REF_CONST(Line2F) rLine, float y, float* pX)
{
	REF_CONST(Vertex2F) rBegin = OF(rLine, GetVertex(true));
	REF_CONST(Vertex2F) rEnd = OF(rLine, GetVertex(false));

	if (OF(rBegin, y) == OF(rEnd, y))
	{
		if (y == OF(rBegin, y))
		{
			*pX = (OF(rBegin, x) + OF(rEnd, x)) / 2;
			return true;
		}
		else
			return false;
	}

	*pX = (OF(rEnd, x) - OF(rBegin, x)) / (OF(rEnd, y) - OF(rBegin, y)) * (y - OF(rBegin, y)) + OF(rBegin, x);

#ifdef DOTNET
	Line2F::LineType noLimitLineType = Line2F::LineType::LINE;
#else
	Line2F::LineType noLimitLineType = Line2F::LINE;
#endif

	if (OF(rLine, GetLineType()) == noLimitLineType)
		return true;

	if (OF(rBegin, x) < OF(rEnd, x))
	{
		if (*pX < OF(rBegin, x) - Geometry::Math::HALF_TOLERANCE() || *pX > OF(rEnd, x) + Geometry::Math::HALF_TOLERANCE())
			return false;
	}
	else
	{
		if (*pX < OF(rEnd, x) - Geometry::Math::HALF_TOLERANCE() || *pX > OF(rBegin, x) + Geometry::Math::HALF_TOLERANCE())
			return false;
	}

	if (OF(rBegin, y) < OF(rEnd, y))
	{
		if (y < OF(rBegin, y) - Geometry::Math::HALF_TOLERANCE() || y > OF(rEnd, y) + Geometry::Math::HALF_TOLERANCE())
			return false;
	}
	else
	{
		if (y < OF(rEnd, y) - Geometry::Math::HALF_TOLERANCE() || y > OF(rBegin, y) + Geometry::Math::HALF_TOLERANCE())
			return false;
	}

	return true;
}

void PolygonF::CheckPointCount(REF_CONST(Vertex2F) rLineBegin, REF_CONST(Vertex2F) rLineEnd, float y, int& rCount)
{
	float dMaxY, dMinY;

	if (OF(rLineBegin, y) < OF(rLineEnd, y))
	{
		dMinY = OF(rLineBegin, y);
		dMaxY = OF(rLineEnd, y);
	}
	else
	{
		dMinY = OF(rLineEnd, y);
		dMaxY = OF(rLineBegin, y);
	}

	// y가 rLineBegin과 rLineEnd 사이에 있거나, y가 둘 중 최소점과 일치하는 경우 rCount를 증가시킨다.
	// y가 둘 중 최대점과 일치하는 경우 rCount를 증가시키지 않는다.
	if (y < dMaxY - Geometry::Math::HALF_TOLERANCE() && y >= dMinY - Geometry::Math::HALF_TOLERANCE())
	{
		rCount++;
	}
}

// 점이 폴리곤 내부에 있는지 검색
// 폴리곤의 시작점과 끝점이 다를 경우, 시작점과 끝점이 연결된 폐곡선으로 간주한다.
// 물론 폴리곤의 시작점과 끝점이 같아도 상관없다.
// Return : 1이면 vertex가 폴리곤의 내부에 위치한다.
//          0이면 vertex가 폴리곤의 외부에 위치한다.
//         -1이면 vertex가 폴리곤의 경계에 위치한다.
int PolygonF::HitTest(REF_CONST(Vertex2F) vertex)
{
	int nVertexCount = GetVertexCount();
	if (nVertexCount < 3)
		return 0;

	// 시작점과 끝점이 같은지 검사한다.
	REF_CONST(Vertex2F) vFirst = (REF_CONST(Vertex2F))m_arrVertices[0];
	REF_CONST(Vertex2F) vLast = (REF_CONST(Vertex2F))m_arrVertices[nVertexCount - 1];
	bool isFirstLastSame = OF(vFirst, GetDistance(vLast)) <= Math::HALF_TOLERANCE();
	//bool isFirstLastSame = (REF_CONST(Vertex2F))m_arrVertices[0] == (REF_CONST(Vertex2F))m_arrVertices[nVertexCount - 1];
	if (isFirstLastSame) nVertexCount--;

	REF_CONST(Vertex2F) rBeginVertex = (REF_CONST(Vertex2F))m_arrVertices[0];
	REF_CONST(Vertex2F) rEndVertex = (REF_CONST(Vertex2F))m_arrVertices[nVertexCount - 1];

	float x;
	int nCount = 0;
	PTR_CONST(Vertex2F) pPrev = POINTER_ADDR(rEndVertex);

	for (int i = 0; i<nVertexCount; i++)
	{
		REF_CONST(Vertex2F) rVertex = (REF_CONST(Vertex2F))m_arrVertices[i];
		INSTANCE(Line2F) line = dnonlynew Line2F(POINTER_VALUE(pPrev), rVertex);

		if (OF(line, IsInclude(vertex)))
			return -1;
		else
		{
			float diff = pPrev->y - OF(rVertex, y);
			if (diff < 0) diff = -diff;

			// X축과 평행한 선분은 계산하지 않는다.
			if (diff > Geometry::Math::HALF_TOLERANCE())
			{
				if (GetXFromLine(line, OF(vertex, y), &x))
				{
					diff = x - OF(vertex, x);
					if (diff < 0.0) diff = -diff;

					if (diff <= Geometry::Math::GetTolerance(x))
						return -1;
					else if (x > OF(vertex, x))
					{
						CheckPointCount(POINTER_VALUE(pPrev), rVertex, OF(vertex, y), nCount);
					}
				}
			}
		}

		pPrev = POINTER_ADDR(rVertex);
	}

	if (nCount % 2 == 0)
		return 0;

	return 1;
}

// vertex와 PolygonF의 가장 가까운 외곽선과의 거리
// vertex가 PolygonF의 내부에 존재할 경우 음수값을 리턴한다.
float PolygonF::GetDistance(REF_CONST(Vertex2F) vertex)
{
	INSTANCE(Vertex2F) vResult;
	return GetDistanceNVertex(vertex, vResult);
}

// vertex와 PolygonF의 가장 가까운 외곽선과의 거리 및 가장 가까운 점을 리턴한다.
// vertex가 PolygonF의 내부에 존재할 경우 음수값을 리턴한다.
float PolygonF::GetDistanceNVertex(REF_CONST(Vertex2F) vertex, OUT CBR(INSTANCE(Vertex2F)) vResult)
{
	vResult = dnonlynew Vertex2F();

	int nVertexCount = GetVertexCount();
	if (nVertexCount < 3)
		return 0.0f;

	// 시작점과 끝점이 같은지 검사한다.
	REF_CONST(Vertex2F) vFirst = (REF_CONST(Vertex2F))m_arrVertices[0];
	REF_CONST(Vertex2F) vLast = (REF_CONST(Vertex2F))m_arrVertices[nVertexCount - 1];
	bool isFirstLastSame = OF(vFirst, GetDistance(vLast)) <= Math::HALF_TOLERANCE();
	//bool isFirstLastSame = (REF_CONST(Vertex2F))m_arrVertices[0] == (REF_CONST(Vertex2F))m_arrVertices[nVertexCount - 1];
	if (isFirstLastSame) nVertexCount--;

	REF_CONST(Vertex2F) rBeginVertex = (REF_CONST(Vertex2F))m_arrVertices[0];
	REF_CONST(Vertex2F) rEndVertex = (REF_CONST(Vertex2F))m_arrVertices[nVertexCount - 1];

	float x, minDistance = -1.0f;
	int nCount = 0;
	PTR_CONST(Vertex2F) pPrev = POINTER_ADDR(rEndVertex);

	for (int i = 0; i<nVertexCount; i++)
	{
		REF_CONST(Vertex2F) rVertex = (REF_CONST(Vertex2F))m_arrVertices[i];
		INSTANCE(Line2F) line = dnonlynew Line2F(POINTER_VALUE(pPrev), rVertex);

		//double distance = OF(line, GetDistance(vertex, false));
		INSTANCE(Vertex2F) vNear = Geometry::Math::GetNearestVertex(vertex, POINTER_VALUE(pPrev), rVertex, false);
		float distance = OF(vertex, GetDistance(vNear));

		if (minDistance < 0.0f || minDistance > distance)
		{
			OF(vResult, SetVertex(OF(vNear, x), OF(vNear, y)));
			minDistance = distance;
		}

		if (OF(line, IsInclude(vertex)))
		{
			// vertex가 외곽선내에 포함되어 있다.
			return 0.0f;
		}
		else
		{
			double diff = pPrev->y - OF(rVertex, y);
			if (diff < 0) diff = -diff;

			// X축과 평행한 선분은 계산하지 않는다.
			if (diff > Geometry::Math::HALF_TOLERANCE())
			{
				if (GetXFromLine(line, OF(vertex, y), &x))
				{
					diff = x - OF(vertex, x);
					if (diff < 0.0) diff = -diff;

					if (diff <= Geometry::Math::GetTolerance(x))
						return -1;
					else if (x > OF(vertex, x))
					{
						CheckPointCount(POINTER_VALUE(pPrev), rVertex, OF(vertex, y), nCount);
					}
				}
			}
		}

		pPrev = POINTER_ADDR(rVertex);
	}

	if (nCount % 2 == 0)
		return minDistance;

	// vertex가 폴리곤의 내부에 위치한다.
	return -minDistance;
}

// 폴리곤의 무게중심을 구한다.
INSTANCE(Vertex2F) PolygonF::CalcWeightCenter()
{
	INSTANCE(Vertex2F) vCenter = dnonlynew Vertex2F();
	int nVertexCount = GetVertexCount();
	if (nVertexCount < 3)
		return vCenter;

	float fArea = 0.0f;

	// For all vertices
	int i = 0;
	for (i = 0; i < nVertexCount; ++i)
	{
		int nIndex2 = (i + 1) % nVertexCount;

		REF_CONST(Vertex2F) v1 = (REF_CONST(Vertex2F))m_arrVertices[i];
		REF_CONST(Vertex2F) v2 = (REF_CONST(Vertex2F))m_arrVertices[nIndex2];

		float a = OF(v1, x) * OF(v2, y) - OF(v2, x) * OF(v1, y);
		fArea += a;
		OF(vCenter, x) += (OF(v1, x) + OF(v2, x)) * a;
		OF(vCenter, y) += (OF(v1, y) + OF(v2, y)) * a;
	}

	fArea *= 0.5f;
	OF(vCenter, x) /= (6.0f * fArea);
	OF(vCenter, y) /= (6.0f * fArea);

	return vCenter;
	/*INSTANCE(Vertex2F) vCenter = dnonlynew Vertex2F();

	int nVertexCount = GetVertexCount();
	if (nVertexCount < 3)
		return vCenter;

	for (int i = 0; i<nVertexCount; i++)
	{
		REF_CONST(Vertex2F) rVertex = (REF_CONST(Vertex2F))m_arrVertices[i];

		OF(vCenter, x) += OF(rVertex, x);
		OF(vCenter, y) += OF(rVertex, y);
	}

	OF(vCenter, x) /= nVertexCount;
	OF(vCenter, y) /= nVertexCount;

	return vCenter;*/
}

INSTANCE(Vertex2F) PolygonF::GetMin()
{
	INSTANCE(Vertex2F) vCenter = dnonlynew Vertex2F();

	int nVertexCount = GetVertexCount();
	if (nVertexCount < 3)
		return vCenter;

	float max_x = FLT_MAX;
	float max_y = FLT_MAX;
	for (int i = 0; i<nVertexCount; i++)
	{
		REF_CONST(Vertex2F) rVertex = (REF_CONST(Vertex2F))m_arrVertices[i];

		if (max_x > OF(rVertex, x))
		{
			max_x = OF(rVertex, x);
		}

		if (max_y > OF(rVertex, y))
		{
			max_y = OF(rVertex, y);
		}
	}

	OF(vCenter, x) = max_x;
	OF(vCenter, y) = max_y;

	return vCenter;
}
// Bounding Rect의 Max
INSTANCE(Vertex2F) PolygonF::GetMax()
{
	INSTANCE(Vertex2F) vCenter = dnonlynew Vertex2F();

	int nVertexCount = GetVertexCount();
	if (nVertexCount < 3)
		return vCenter;

	float max_x = -FLT_MIN;
	float max_y = -FLT_MIN;
	for (int i = 0; i<nVertexCount; i++)
	{
		REF_CONST(Vertex2F) rVertex = (REF_CONST(Vertex2F))m_arrVertices[i];

		if (max_x < OF(rVertex, x))
		{
			max_x = OF(rVertex, x);
		}

		if (max_y < OF(rVertex, y))
		{
			max_y = OF(rVertex, y);
		}
	}

	OF(vCenter, x) = max_x;
	OF(vCenter, y) = max_y;

	return vCenter;
}

float PolygonF::GetArea() CONSTF
{
	float fArea = 0.0f;
	int nVertexCount = GetVertexCount();

	for (int i = 0; i < nVertexCount; i++)
	{
		int nSecondIndex = (i + 1) % nVertexCount;

		REF_CONST(Vertex2F) v1 = (REF_CONST(Vertex2F))m_arrVertices[i];
		REF_CONST(Vertex2F) v2 = (REF_CONST(Vertex2F))m_arrVertices[nSecondIndex];

		fArea += OF(v1, x) * OF(v2, y) - OF(v2, x) * OF(v1, y);
	}

	if (fArea < 0.0f)
		return -fArea / 2;
	return fArea / 2;
}

// Polygon 연산을 수행하기 위하여 VertexList를 직접 사용할 수 있도록 한다.
#ifdef DOTNET
System::Collections::Generic::List<Vertex2F^>^ PolygonF::GetVertexList()
{
	return m_arrVertices;
}
#else
std::vector<Vertex2F>& PolygonF::GetVertexList()
{
	return m_arrVertices;
}
#endif

bool PolygonF::IsClockWise()
{
	float sum = 0.0f;
	int nVertexCount = GetVertexCount();

	for (int i = 0; i < nVertexCount; i++)
	{
		POINTER(Vertex2F) v1 = GetVertex(i);
		POINTER(Vertex2F) v2 = GetVertex((i + 1) % nVertexCount);
		sum += (v2->x - v1->x) * (v2->y + v1->y);
	}

	return sum > 0.0f;
}

END_NS
END_NS
