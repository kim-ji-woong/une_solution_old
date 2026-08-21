#pragma once
#include "DXFEntity.h"

namespace DXF
{
	namespace ENTITIES
	{
		class RoundRect : public Entity
		{
		public:
			RoundRect(void);
			virtual ~RoundRect(void);

		public:
			void SetNormalVector(double dAxisX, double dAxisY, double dAxisZ);
			void GetCenterPoint(double* pX, double* pY, double* pZ);
			void GetNormalVector(double* pX, double* pY, double* pZ);

		protected:
			double m_dArrCoordCenter[3];
			Utility::Vertex3D m_vNormal;					// 객체가 존재하는 공간의 법선 벡터
		};
	}
}
