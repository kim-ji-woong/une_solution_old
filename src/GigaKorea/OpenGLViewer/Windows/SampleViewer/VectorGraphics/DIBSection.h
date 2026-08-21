#pragma once

namespace VectorGraphics
{
	class __declspec(dllexport) DIBSection
	{
		void InitObject(void);
	protected:
		HBITMAP m_hbmp;
		HBITMAP m_hbmOld;
		void* m_pBits;
		SIZE m_size;
		HDC m_hdc;
		int m_total_width;
		BITMAPINFOHEADER m_bih;
		unsigned long m_bitcount;
	public:
		DIBSection();
		DIBSection(const DIBSection& dib);
		virtual ~DIBSection();

		void Copy(const DIBSection& dib);
		DIBSection& operator=(const DIBSection& dib);

		void Close(void);

		int IsCreated(void)const{ return (m_pBits ? 1 : 0); }

		// Create a DIBSection
		void Create(int cx, int cy, int nbits);

		// Draw method
		void Draw(HDC hdcDest, int x, int y);

		// Resize method
		void ResizeImage(DIBSection& dst_dib, int w, int h);

		// patblt method
		void PatBlt(DWORD pattern);

		HDC GetHDC() { return m_hdc; }
		HBITMAP GetHandle() { return m_hbmp; }

		unsigned long Width(void)const{ return m_size.cx; }
		unsigned long Height(void)const{ return m_size.cy; }
		unsigned long GetTotalWidth(void)const{ return m_total_width; }
		void * GetBits(void){ return m_pBits; }
		void * GetConstBits(void)const{ return m_pBits; }
		BITMAPINFOHEADER& GetBIH(void){ return m_bih; }
		unsigned long GetBitCount(void)const{ return m_bitcount; }
		void SetPixel(UINT32 x, UINT32 y, COLORREF cr);
		void GetPixel(UINT32 x, UINT32 y, COLORREF& cr);
	};
}
