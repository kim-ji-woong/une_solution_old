#pragma once

namespace DXF
{
	namespace ENTITIES
	{
		class Ellipse :	public RoundRect
		{
		public:
			Ellipse(void);
			virtual ~Ellipse(void);

		public:
			virtual void Write(Utility::FileManager* pMgr);
			virtual void Init();
			virtual bool ReadDatai(int nCode, int nData);
			virtual bool ReadDatad(int nCode, double dData);
			virtual bool ReadDatas(int nCode, wchar_t* strData);

		public:
			// dArrCoordLongAxis : 타원의 장축 끝점이 타원의 중점으로 부터 얼만큼 떨어져 있나를 나타냄
			// dRatio : 단축대 장축의 비율
			void SetEllipse(double dArrCoordCenter[3], double dArrCoordLongAxis[3], double dRatio);
			// 시작각도와 끝 각도를 설정(Radian)
			void SetAngle(double dBeginAngle, double dEndAngle);
			void GetLongAxisCoord(double* pX, double* pY, double* pZ);
			// 단축대 장축의 비율(short / long)
			double GetRatio();
			// Radian
			void GetParameter(double* pBeginParameter, double* pEndMeter);

		protected:
			double m_dArrCoordLongAxis[3];	// 장축의 끝점
			double m_dRatio;				// 단축대 장축의 비율(short / long)
			// 타원의 시작 Parameter(Radian)
			// 타원이 완전한 원일 경우 시작 Parameter, 끝 Parameter는 시작각과 끝각을 의미한다.
			// 그외의 경우 시작각과 끝각은 다음과 같이 구한다.
			// Tan(시작각) = Tan(m_dParameterBegin) * m_dRatio;
			// Tan(끝각) = Tan(m_dParameterEnd) * m_dRatio;
			// Tan 값은 180도를 기준으로 반복하기 때문에, 다음과 같은 보정이 필요하다.
			// 1) Parameter가 PI/2보다 크고 PI * 1.5보다 작을때
			//    : 시작각 또는 끝각 += 180도
			// 2) Parameter가 PI * 1.5보다 클때
			//    : 시작각 또는 끝각 += 360도
			double m_dParameterBegin;
			double m_dParameterEnd;
		};
	}
}
