#pragma once

namespace DXF
{
	namespace HEADER
	{
		using namespace std;

		class CData
		{
		public:
			CData(void);
			~CData(void);

			// Type1(Data가 정수 또는 실수인 헤더변수)를 추가
			void SetData(wstring strVariable, int nCode1, double fValue);
			// Type2(Data가 문자열인 헤더변수)를 추가
			void SetData(wstring strVariable, int nCode1, wstring strValue);
			// Type3(code와 Data가 3개씩 존재하는 헤더변수)를 추가
			void SetData(wstring strVariable, int nCode1, double fValue1, int nCode2, double fValue2, int nCode3, double fValue3);
			void SetData(wstring strVariable, int nCode1, double fValue1, int nCode2, double fValue2);

			int		GetType();	// Type를 반환
			// Type1을 반환
			bool	GetData_Type1(wstring& strVariable, int& nCode, double& fValue);
			// Type2를 반환
			bool	GetData_Type2(wstring& strVariable, int& nCode, wstring& strValue);
			// Type3을 반환
			bool	GetData_Type3(wstring& strVariable, int& nCode, int& nCode1, double& fValue1, int& nCode2, double& fValue2, int& nCode3, double& fValue3);
			bool	GetData_Type4(wstring& strVariable, int& nCode, int& nCode1, double& fValue1, int& nCode2, double& fValue2);
			int GetIntValue();

			// Update Data
			bool	UpdateData(double fValue);
			bool	UpdateData(wstring strValue);
			bool	UpdateData(double fValue1, double fValue2);
			bool	UpdateData(double fValue1, double fValue2, double fValue3);

			void	Write(Utility::FileManager* pMgr);

		private:
			wstring	m_strVariable;
			int		m_nType;		// Type1, Type2, Type3
			int		m_nCode;
			int		m_nCode1;	
			int		m_nCode2;
			int		m_nCode3;
			int		m_nValue1;
			double	m_fValue1;
			double	m_fValue2;
			double	m_fValue3;
			wstring	m_strValue;
		};
	}
}
