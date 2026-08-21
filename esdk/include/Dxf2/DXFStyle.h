#pragma once
#include "DXFTable.h"

// 문자 Style 관련 클래스

namespace DXF
{
	namespace TABLES
	{
		class TableManager;

		class Style : public Table
		{
		public:
			class Entity
			{
			public:
				Entity(Style* pTable, const wchar_t* strStyleName, const wchar_t* strFontName = 0);

			public:
				void Write(Utility::FileManager* pMgr);
				wchar_t* GetStyleName();
				int GetHandle();
				const wchar_t* GetFontName();

			public:
				wchar_t m_strStyleName[256];
				Style* m_pParent;
				int m_nHandle;
				int m_nFlag;
				double m_dFixedHeight;
				double m_dWidthFactor;
				double m_dObliqueAngle;
				int m_nGenerationFlag;
				double m_dLastHeight;
				wchar_t m_strPrimaryFontFile[256];
				wchar_t m_strBigFontFile[256];
				wchar_t m_strFontName[256];
			};

		public:
			Style(TableManager* pMgr);
			virtual ~Style(void);
			friend class Entity;

		public:
			void Init();
			void Write(Utility::FileManager* pMgr);
			Entity* GetEntity(const wchar_t* strStyleName);
			void AddEntity(const wchar_t* strStyleName, const wchar_t* strFontName);

		public:
			virtual void ReadDatai(int nCode, int nData);
			virtual void ReadDatad(int nCode, double dData);
			virtual void ReadDatas(int nCode, wchar_t* strData);

		protected:
			std::list<Entity> m_list;
			Entity* m_pEntity;
		};
	}
}
