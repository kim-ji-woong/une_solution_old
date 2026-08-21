#pragma once

namespace DXF
{
	namespace CLASSES
	{
		class ClassManager : public SectionManager
		{
		public:
			class DefaultClass
			{
			public:
				DefaultClass(wchar_t* strDXFRecord, wchar_t* strCPPClass, int n90, int n280, int n281);
				wchar_t m_strDXFRecord[64];
				wchar_t m_strCPPClass[64];
				int m_n90;
				int m_n280;
				int m_n281;
			};

		public:
			ClassManager(void);
			virtual ~ClassManager(void);

		public:
			virtual void ReadDatai(int nCode, int nData) {}
			virtual void ReadDatad(int nCode, double dData) {}
			virtual void ReadDatas(int nCode, wchar_t* strData) {}

		public:
			void Init();
			void Write(Utility::FileManager* pMgr);
			void AddClassType(wchar_t* strClassName);

		protected:
			std::list<DefaultClass> m_list;
		};

	}
}
