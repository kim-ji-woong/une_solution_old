#pragma once

namespace Utility
{
	class Vertex3D;
}

namespace DXF
{
	namespace ENTITIES
	{
		class Solid : public Entity
		{
		public:
			Solid(void);
			virtual ~Solid(void);

		public:
			virtual void Write(Utility::FileManager* pMgr);
			virtual void Init();
			virtual bool ReadDatai(int nCode, int nData);
			virtual bool ReadDatad(int nCode, double dData);
			virtual bool ReadDatas(int nCode, wchar_t* strData);

		public:
			void SetPoint(int nIndex, double x, double y, double z);
			void SetPoint(int nIndex, const Utility::Vertex3D& rPt);
			bool GetPoint(int nIndex, Utility::Vertex3D* pt);

		protected:
			Utility::Vertex3D m_ptCorner[4];
		};
	}
}
