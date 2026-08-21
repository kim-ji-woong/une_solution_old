#pragma once

namespace DXF
{
	namespace ENTITIES
	{
		class Line : public Entity
		{
		public:
			Line(void);
			Line(double dArrCoordBegin[3], double dArrCoordEnd[3]);
			virtual ~Line(void);

		public:
			void SetLine(double dArrCoordBegin[3], double dArrCoordEnd[3]);
			void GetCoord(double dArrCoordBegin[3], double dArrCoordEnd[3]);

		public:
			virtual void Write(Utility::FileManager* pMgr);
			virtual void Init();
			virtual bool ReadDatai(int nCode, int nData);
			virtual bool ReadDatad(int nCode, double dData);
			virtual bool ReadDatas(int nCode, wchar_t* strData);

		protected:
			double m_dArrCoordBegin[3];
			double m_dArrCoordEnd[3];
		};
	}
}
