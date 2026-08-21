#pragma once

namespace Utility
{
	class Vertex3D;
}

namespace DXF
{
	namespace ENTITIES
	{
		class Image : public Entity
		{
		public:
			Image(void);
			virtual ~Image(void);
			Image(const Image& rhs);
			void operator= (const Image& rhs);

		public:
			virtual void Write(Utility::FileManager* pMgr);
			virtual void Init();

		public:
			void SetClassVersion(int nVersion);
			void SetImageSize(double dWidth, double dHeight);
			// 원래의 이미지가 공간상에 위치한 모습을 세 좌표로 나타낸다.
			// dArrLT : 원래 이미지의 좌측 상단의 모서리가 위치한 좌표
			// dArrLB : 원래 이미지의 좌측 하단의 모서리가 위치한 좌표
			// dArrRB : 원래 이미지의 우측 하단의 모서리가 위치한 좌표
			void SetPosition(double dArrLT[3], double dArrLB[3], double dArrRB[3]);
			void SetImageDef(int nImageDef);
			void SetDisplayProperty(int nProperty);
			void SetImageState(int nClippingState, int nBrightness, int nContrast, int nFade);
			void SetImageDefReactor(int nImageDefReactor);
			void SetBoundaryType(int nBoundaryType);
			void SetBoundaryPoint(double* pBoundaryPointX, double* pBoundaryPointY, int nArrSize);

		protected:
			int m_nClassVersion;
			double m_dImageWidth;
			double m_dImageHeight;
			double m_dArrPos[3];
			double m_dArrLB[3];
			double m_dArrRB[3];
			bool m_bPositionFlag;
			Utility::Vertex3D m_vU;			// 이미지 가로 방향의 축척 및 회전에 관한 단위 벡터
			Utility::Vertex3D m_vV;			// 이미지 세로 방향의 축척 및 회전에 관한 단위 벡터
			int m_nImageDef;
			int m_nDisplayProperty;
			int m_nClippingState;
			int m_nBrightness;
			int m_nContrast;
			int m_nFade;
			int m_nImageDefReactor;
			int m_nBoundaryType;
			int m_nBoundaryPointSize;
			double* m_pBoundaryPointX;
			double* m_pBoundaryPointY;

		protected:
			int* m_pRefCount;
		};
	}
}
