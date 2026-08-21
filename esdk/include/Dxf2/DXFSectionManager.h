#pragma once

namespace DXF
{
	class SectionManager
	{
	public:
		SectionManager(void);
		virtual ~SectionManager(void);

	public:
		virtual void ReadDatai(int nCode, int nData) = 0;
		virtual void ReadDatad(int nCode, double dData) = 0;
		virtual void ReadDatas(int nCode, wchar_t* strData) = 0;
		virtual void Clear() {}
		// Handle Code(5)가 정수가 아닌 문자열일 경우에도 읽을수 있는가?
		virtual bool ReadStringHandle();

	public:
		void SetOwner(DXFManager* pOwner);
		DXFManager* GetOwner();

	protected:
		DXFManager* m_pOwner;
		bool m_bDeleted;
	};
}
