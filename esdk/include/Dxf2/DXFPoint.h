#pragma once

namespace DXF
{
	namespace ENTITIES
	{
		class Point : public Entity
		{
		public:
			Point(void);
			Point(double x, double y, double z);
			virtual ~Point(void);

		public:
			void SetCoord(double x, double y, double z);
			double X();
			double Y();
			double Z();

		public:
			virtual void Write(Utility::FileManager* pMgr);
			virtual void Init();
			virtual bool ReadDatai(int nCode, int nData);
			virtual bool ReadDatad(int nCode, double dData);
			virtual bool ReadDatas(int nCode, wchar_t* strData);

		protected:
			double m_dX, m_dY, m_dZ;
		};
	}
}
