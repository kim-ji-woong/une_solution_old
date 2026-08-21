#pragma once

namespace DXF
{
	class DXFManager;
	namespace HEADER
	{
		class CHeader : public SectionManager
		{
		public:
			CHeader();
			~CHeader(void);

			void AddVariable(wstring strVariable, int nCode1, double fValue);
			void AddVariable(wstring strVariable, int nCode1, wstring strValue);
			void AddVariable(wstring strVariable, int nCode1, double fValue1, int nCode2, double fValue2);
			void AddVariable(wstring strVariable, int nCode1, double fValue1, int nCode2, double fValue2, int nCode3, double fValue3);

			bool UpdateVariable(wstring strVariable, double fValue);
			bool UpdateVariable(wstring strVariable, wstring strValue);
			bool UpdateVariable(wstring strVariable, double fValue1, double fValue2);
			bool UpdateVariable(wstring strVariable, double fValue1, double fValue2, double fValue3);

			void Write(Utility::FileManager* pMgr);

			std::map<std::wstring, CData>& GetHeader();

		public:
			virtual void Clear();
			virtual void ReadDatai(int nCode, int nData);
			virtual void ReadDatad(int nCode, double dData);
			virtual void ReadDatas(int nCode, wchar_t* strData);

		public:
			double GetAngBase();
			int GetAngDir();
			int GetAunits();
			wchar_t* GetCurrentLayer();
			void UpdateNextHandle();

		protected:
			void Init();

		private:
			std::map<std::wstring, CData> m_mapHeader;

		protected:
			double m_dAngBase;		// 각도 0 방향
			int m_nAngDir;			// 1 : 시계 방향, 0 : 시계 반대 방향
			int m_nAunits;			// 각도 단위(0 : Degree, 1 : 도분초, 2 : 그라디안, 3 : 라디안, 4 : 측량사 단위)
			int m_nInsUnits;		// 길이 단위계
			std::wstring m_strCLayer;		// 현재 Layer
			void* m_pVariable;
		};
	}
}
