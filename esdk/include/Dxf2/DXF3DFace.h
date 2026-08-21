#pragma once

namespace DXF
{
	namespace ENTITIES
	{
		class _3DFace :	public Entity
		{
		public:
			_3DFace(void);
			_3DFace(double dArrCoord1[3], double dArrCoord2[3], double dArrCoord3[3]);
			_3DFace(double dArrCoord1[3], double dArrCoord2[3], double dArrCoord3[3], double dArrCoord4[3]);
			virtual ~_3DFace(void);

		public:
			void Set3DFace(double dArrCoord1[3], double dArrCoord2[3], double dArrCoord3[3], double dArrCoord4[3]);

		public:
			virtual void Init();

		protected:
			double m_dArrCoord[4][3];
		};
	}
}
