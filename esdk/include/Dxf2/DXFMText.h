#pragma once
#include "DXFEntity.h"

namespace DXF
{
	namespace ENTITIES
	{
		class MText : public Entity
		{
		public:
			MText(void);
			virtual ~MText(void);

		public:
			virtual void Write(Utility::FileManager* pMgr);
			virtual void Init();
			virtual bool ReadDatai(int nCode, int nData);
			virtual bool ReadDatad(int nCode, double dData);
			virtual bool ReadDatas(int nCode, wchar_t* strData);

		public:
			void SetAttachment(int nAttachmentPoint);
			void GetAttachment(int* pHorizon, int* pVertical);
			void SetDrawingDirection(int nDrawingDirection);
			int GetDrawingDirection();
			void SetInsertionPoint(double x, double y, double z);
			void GetInsertionPoint(double* pX, double* pY, double* pZ);
			void SetArea(double dAreaWidth, double dAreaHeight);
			void GetArea(double* pAreaWidth, double* pAreaHeight);
			void SetHeight(double dHeight);
			double GetHeight();
			void SetNormalVector(double dAxisX, double dAxisY, double dAxisZ);
			void GetNormalVector(double* pX, double* pY, double* pZ);
			wchar_t* GetString();
			void SetString(wchar_t* strData);
			void SetLineSpace(double dLineSpace);
			double GetLineSpace();
			// Radian
			double GetTextAngle() const;
			void SetStyleName(const wchar_t* strStyleName);
			const wchar_t* GetStyleName();

		protected:
			// X축방향 벡터의 세 좌표값
			void SetXAxisVector(double x, double y, double z);
			void SetZAxisVector();
			void SetText(wchar_t* strData);

		protected:
			int m_nAttachmentPoint;
			int m_nDrawingDirection;
			double m_dInsertionPoint[3];
			double m_dHeight;
			double m_dAreaWidth;
			double m_dAreaHeight;
			Utility::Vertex3D m_vNormal;				// 객체가 존재하는 공간의 법선 벡터
			std::wstring m_strStyleName;
			std::wstring m_strData;
			double m_dLineSpace;
			// 각도(Radian), ReadDXF시에만 쓰인다.
			// DXF를 제작할 경우에는 쓰이지 않는다.
			double m_dAngle;

		private:
			double m_dArrTemp[3];
		};
	}
}
