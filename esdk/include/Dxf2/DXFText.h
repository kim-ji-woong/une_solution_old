#pragma once
#include "DXFEntity.h"

namespace DXF
{
	namespace ENTITIES
	{
		class Text : public Entity
		{
		public:
			Text(void);
			virtual ~Text(void);

		public:
			virtual void Write(Utility::FileManager* pMgr);
			virtual void Init();
			virtual bool ReadDatai(int nCode, int nData);
			virtual bool ReadDatad(int nCode, double dData);
			virtual bool ReadDatas(int nCode, wchar_t* strData);

		public:
			wchar_t* GetString();
			void GetJustification(int* pHorizon, int* pVertical);
			wchar_t* GetStyleName();
			void SetString(wchar_t* strData);
			void SetStyleName(wchar_t* strStyleName);
			void SetHorizontalJustification(int nHorizon);
			void SetVerticalJustification(int nVertical);
			void SetHeight(double dHeight);
			double GetHeight();
			void SetFirstAlignPoint(double dX, double dY, double dZ);
			void GetFirstAlignPoint(double* pX, double* pY, double* pZ);
			void SetSecondAlignPoint(double dX, double dY, double dZ);
			void GetSecondAlignPoint(double* pX, double* pY, double* pZ);
			void SetNormalVector(double dAxisX, double dAxisY, double dAxisZ);
			void GetNormalVector(double* pX, double* pY, double* pZ);
			// Degree
			double GetTextAngle() const;

		protected:
			double m_dFirstAlignPoint[3];
			double m_dSecondAlignPoint[3];	// m_nHorizonJust와 m_nVerticalJust 적어도 둘 중 하나가 0이 아닐때에만 사용됨
			double m_dHeight;
			std::wstring m_strStyleName;
			int m_nHorizonJust;				// 수평 정렬
			// 수직정렬의 기본값 : Baseline
			// Baseline = baseline of text font
			// Bottom = lowest text font value
			int m_nVerticalJust;			// 수직 정렬
			Utility::Vertex3D m_vNormal;			// 객체가 존재하는 공간의 법선 벡터
			std::wstring m_strData;
			// 각도(Degree), ReadDXF시에만 쓰인다.
			// DXF를 제작할 경우에는 쓰이지 않는다.
			double m_dAngle;

		private:
			double m_dArrTemp[3];
		};
	}
}
