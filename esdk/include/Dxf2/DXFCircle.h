#pragma once

namespace DXF
{
	namespace ENTITIES
	{
		class Circle : public RoundRect
		{
		public:
			Circle(void);
			virtual ~Circle(void);

		public:
			virtual void Write(Utility::FileManager* pMgr);
			virtual void Init();
			virtual bool ReadDatai(int nCode, int nData);
			virtual bool ReadDatad(int nCode, double dData);
			virtual bool ReadDatas(int nCode, wchar_t* strData);

		public:
			void SetCircle(double dArrCoordCenter[3], double dRadius);
			double GetRadius();

		protected:
			double m_dRadius;
		};
	}
}
