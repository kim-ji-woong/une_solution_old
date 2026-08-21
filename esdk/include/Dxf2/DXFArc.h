#pragma once

// 3차원 공간에서 원을 그리기 위해서는 원이 지나는 세 점이 필요하다.
// 그러나, CAD에서는 이러한 방식으로 3차원 공간상에 원을 그리는 방식을
// 지원하지 않고, 원이 존재하는 평면을 가상의 XY 평면(OCS 좌표계)로 놓은 다음
// 그 평면에서의 좌표값을 사용하여 원을 그린다.(중점과 반지름)

// DXF에는 Arc의 BeginAngle과 EndAngle만 존재하며, Arc의 방향은 기입하지 않는다.
// Arc는 항상 반시계 방향으로만 그려진다.

namespace DXF
{
	namespace ENTITIES
	{
		class Arc :	public Circle
		{
		public:
			Arc(void);
			virtual ~Arc(void);

		public:
			virtual void Write(Utility::FileManager* pMgr);
			virtual void Init();
			virtual bool ReadDatai(int nCode, int nData);
			virtual bool ReadDatad(int nCode, double dData);
			virtual bool ReadDatas(int nCode, wchar_t* strData);

		public:
			void SetArc(double dArrCoordCenter[3], double dRadius, double dAngleBegin, double dAngleEnd);
			// Degree
			void GetAngle(double* pBeginAngle, double* pEndAngle);

		protected:
			double m_dAngleBegin;	// Degree
			double m_dAngleEnd;		// Degree
		};
	}
}