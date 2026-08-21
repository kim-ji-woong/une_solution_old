#pragma once
#include <string>

namespace PenWorld
{
	class LineType
	{
	public:
		LineType(void);
		virtual ~LineType(void);

	public:
		void SetStyle(unsigned short nStyle);
		int GetStyle();
		void SetThick(float fThick);
		float GetThick();
		void SetFactor(int nFactor);
		int GetFactor();
		void SetTypeName(wchar_t* strTypeName);
		wchar_t* GetTypeName();
		void AddRef();
		void SetZeroCount();
		int GetRefCount() const;

	public:
		float m_fThick;
		int m_nFactor;
		unsigned short m_nStyle;
		//COLORREF m_color;

	protected:
		//std::string m_strTypeName;
		wchar_t m_strTypeName[256];

	private:
		int m_nRefCount;
	};
}
