#pragma once
#include <vector>

namespace UnE
{
	namespace UBML
	{
		// 부호가 unsigned인 type은 모두 짝수이다.
		// (type / 100) % 10의 결과는 Type Number이다.
		// 1Byte 정수(1), 2Byte 정수(2), 4Byte 정수(3), 8Byte 정수(4), 4Byte 실수(5), 8Byte 실수(6), boolean(7), 1Byte 문자(8), 2Byte 문자(9)
		enum DataType
		{
			__BYTE = 1 + 100,
			__UBYTE = 2 + 100,
			__SHORT = 3 + 200,
			__USHORT = 4 + 200,			
			__LONG = 5 + 300,
			__ULONG = 6 + 300,
			__LONGLONG = 7 + 400,
			__ULONGLONG = 8 + 400,
			__FLOAT = 9 + 500,
			__DOUBLE = 11 + 600,
			__BOOL = 13 + 700,
			__CHAR = 15 + 800,
			__WCHAR = 17 + 900,
			__ELEMENT = 19 + 1000,
			__BYTE_ARR = 31 + 1100,
			__UBYTE_ARR = 32 + 1100,
			__SHORT_ARR = 33 + 1200,
			__USHORT_ARR = 34 + 1200,
			__LONG_ARR = 35 + 1300,
			__ULONG_ARR = 36 + 1300,
			__LONGLONG_ARR = 37 + 1400,
			__ULONGLONG_ARR = 38 + 1400,
			__FLOAT_ARR = 39 + 1500,
			__DOUBLE_ARR = 41 + 1600,
			__BOOL_ARR = 43 + 1700,
			__CHAR_ARR = 45 + 1800,
			__WCHAR_ARR = 47 + 1900,
			__ELEMENT_ARR = 49 + 2000,
			__BYTE_ARR_FIXED = 61 + 2100,
			__UBYTE_ARR_FIXED = 62 + 2100,
			__SHORT_ARR_FIXED = 63 + 2200,
			__USHORT_ARR_FIXED = 64 + 2200,
			__LONG_ARR_FIXED = 65 + 2300,
			__ULONG_ARR_FIXED = 66 + 2300,
			__LONGLONG_ARR_FIXED = 67 + 2400,
			__ULONGLONG_ARR_FIXED = 68 + 2400,
			__FLOAT_ARR_FIXED = 69 + 2500,
			__DOUBLE_ARR_FIXED = 71 + 2600,
			__BOOL_ARR_FIXED = 73 + 2700,
			__CHAR_ARR_FIXED = 75 + 2800,
			__WCHAR_ARR_FIXED = 77 + 2900,
			__ELEMENT_ARR_FIXED = 79 + 3000
		};

		class __declspec(dllexport) UData
		{
		public:
			enum ClassType {SEGMENT = 0, ELEMENT};

		public:
			virtual ClassType GetClassType() const = 0;
		};

		class __declspec(dllexport) Segment : public UData
		{
		public:
			Segment();
			Segment(DataType type);

			void SetType(DataType type);
			bool SetTypeTag(unsigned char typeTag, bool& isArray, size_t& rDataByteSize);

			unsigned char GetTypeTag(bool& isArray, size_t& rDataByteSize) const;
			std::wstring GetTypeTagString(bool& isArray) const;
			inline DataType GetType() const {return m_type;}

			void AddData(bool data);
			void AddData(char data);
			void AddData(wchar_t data);
			void AddData(short data);
			void AddData(int data);
			void AddData(__int64 data);
			void AddData(float data);
			void AddData(double data);

			void SetTagDescription(std::wstring strDesc);
			const std::wstring& GetTagDescription() const;
			
			void SetTagName(std::wstring strName);
			const std::wstring& GetTagName() const;



			inline unsigned int GetDataCount() const {return (unsigned int)m_vecData.size();}
			inline const void* GetData(unsigned int nIndex) const {return &m_vecData[nIndex];}

		public:
			virtual ClassType GetClassType() const;

		protected:
			// 데이터 크기가 8바이트 보다 큰 것은 없으므로 가장 큰 __int64로 설정
			std::vector<__int64> m_vecData;
			std::wstring m_strDescription;
			std::wstring m_strName;
	
			DataType m_type;
		};

		class __declspec(dllexport) Element : public UData
		{
		public:
			Element(int nTag = 0);
			virtual ~Element(void);

		public:
			// strTag의 길이는 반드시 두 글자 이어야 한다.
			// 처음 두 글자는 ASCII, 나머지는 숫자로 인식된다.
			bool MakeTag(const char* strTag);
			bool MakeTag(const wchar_t* strTag);
			bool MakeTag(const char* strHeader, int num);
			bool MakeTag(const wchar_t* strHeader, int num);

			void SetTag(int nTag);
			int GetTag() const;
			std::wstring GetTagString() const;

			void SetDescription(std::wstring strDesc);
			const std::wstring& GetDescription() const;

			void AddData(UData* pData);
			unsigned int GetDataCount() const;
			const UData* GetData(unsigned int nIndex) const;

			void RemoveData(unsigned int nBeginIndex, unsigned int nEndIndex, bool freeMemory = true);
			void RemoveData(unsigned int nIndex, bool freeMemory = true);
			void RemoveFirstData(bool freeMemory = true);
			void RemoveLastData(bool freeMemory = true);
			void RemoveAll(bool freeMemory = true);

		public:
			virtual ClassType GetClassType() const;

		private:
			int m_nTag;
			std::vector<UData*> m_vecData;
			// XML 변환시 사용된다.
			std::wstring m_strDescription;
		};
	}
}
