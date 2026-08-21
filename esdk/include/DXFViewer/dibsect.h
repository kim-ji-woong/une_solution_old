#ifndef __DIB_SECTION_CLASS_NCLUDE_H__
#define __DIB_SECTION_CLASS_NCLUDE_H__
#include <Windows.h>

//
// DIBSection -
//
// Encapsulates a DIB section and a DC.
//
class DIBSection
{
	void InitObject(void);
protected:
	HBITMAP m_hbmp;
	HBITMAP m_hbmOld;
	void* m_pBits;
	int m_nWidth, m_nHeight;
	HDC m_hdc;
	//CDC* m_pdc;
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

	int IsCreated(void)const{ return (m_pBits?1:0);}

	  // Create a DIBSection
	void Create(int cx, int cy, int nbits );

	  // Draw method
	void Draw(HDC hdcDest, int x, int y);

	  // Resize method
	void ResizeImage(DIBSection& dst_dib, int w, int h);

	  // patblt method
	void PatBlt(DWORD pattern);

	HDC GetDC() {return m_hdc;}
	HBITMAP GetHandle() {return m_hbmp;}

	int Width(void)const{ return m_nWidth;}
	int Height(void)const{ return m_nHeight;}
	unsigned long GetTotalWidth(void)const{ return m_total_width;}
	void * GetBits(void){ return m_pBits;}
	void * GetConstBits(void)const{ return m_pBits;}
	BITMAPINFOHEADER& GetBIH(void){ return m_bih;}
	unsigned long GetBitCount(void)const{ return m_bitcount;}
	void SetPixel(int x, int y, unsigned char r, unsigned char g, unsigned char b);
	void GetPixel(int x, int y, unsigned char& r, unsigned char& g, unsigned char& b);
};
#endif 
