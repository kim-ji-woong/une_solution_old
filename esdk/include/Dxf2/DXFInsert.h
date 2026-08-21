#pragma once

namespace Utility
{
	class Vertex3D;
}

namespace DXF
{
	namespace ENTITIES
	{
		class Insert : public Entity
		{
		public:
			Insert(void);
			virtual ~Insert(void);

		public:
			virtual void Init();
			virtual bool ReadDatai(int nCode, int nData);
			virtual bool ReadDatad(int nCode, double dData);
			virtual bool ReadDatas(int nCode, wchar_t* strData);
			virtual void Write(Utility::FileManager* pMgr);

		public:
			const wchar_t* GetBlockName() const;
			const Utility::Vertex3D& GetInsertPoint() const;
			void SetBlockName(const wchar_t* strBlockName);
			double GetAngle() const;	// Degree

		protected:
			std::wstring m_strBlockName;
			Utility::Vertex3D m_ptInsert;
			double m_dAngle;	// Degree
		};
	}
}
