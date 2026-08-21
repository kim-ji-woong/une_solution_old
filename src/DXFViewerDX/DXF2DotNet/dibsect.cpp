#include "stdafx.h"
#include "dibsect.h"


void DIBSection::InitObject(void)
{
	m_hbmp = 0;
	m_pBits = 0 ;
	m_hdc = 0;
	//m_pdc = 0 ;
	m_total_width = 0;
	m_bitcount = 0;
	m_nWidth = m_nHeight = m_total_width = 0;
}

DIBSection::DIBSection()
{
	InitObject();
}

DIBSection::DIBSection(const DIBSection& dib)
{
	InitObject();
	Copy(dib);
}

DIBSection::~DIBSection()
{
	Close();
}

void DIBSection::Copy(const DIBSection& dib)
{
	Close();
	if (dib.IsCreated())
	{
		int w = dib.Width();
		int h = dib.Height();
		int bitcount = dib.GetBitCount();

		Create(w,h,bitcount);

		if (IsCreated())
		{
			memcpy(m_pBits,dib.GetConstBits(),(h * m_total_width * bitcount / 8));
		}
	}
}

DIBSection& DIBSection::operator = (const DIBSection& dib)
{
	Copy(dib);
	return *this;
}

void DIBSection::Close(void)
{
	if (m_hbmOld)
	{
		::SelectObject(m_hdc, m_hbmOld);
	}

	if (m_hbmp)
		DeleteObject(m_hbmp);

	//if (m_pdc) delete m_pdc;
	if (m_hdc)
	{
		DeleteDC(m_hdc);
		m_hdc = 0;
	}

	InitObject();
}

void DIBSection::Create(int cx, int cy, int nbits) 
{
	if (cx <= 0 || cy <= 0)
		return;

	nbits = 32;
	
	if ((nbits != 8) && (nbits != 16) &&
		(nbits != 24) && (nbits != 32))
		return;

	Close();

	 // Save size for drawing later.
	m_nWidth = cx ;
	m_nHeight = cy ;

	 // Initialize the bitmapinfo header
	int size = sizeof(BITMAPINFOHEADER) ;
	memset(&m_bih, 0, size);

	 // Populate bitmapinfo header
	m_bih.biSize = size;
	m_bih.biWidth = ((((int) cx * 8) + 31) & ~31) >> 3;
	m_bih.biHeight = cy;
	m_bih.biPlanes = 1;
	m_bih.biBitCount = nbits;
	m_bih.biCompression = BI_RGB;
	m_total_width = m_bih.biWidth;
	m_bitcount = nbits;

	  // Create a new DC.
	m_hdc = CreateCompatibleDC(0);
	//m_pdc = new CDC ;
	//m_pdc->CreateCompatibleDC(NULL);

	  // Create the DIB section.
	m_hbmp = CreateDIBSection( m_hdc,
							(BITMAPINFO*)&m_bih,
							DIB_PAL_COLORS,
							&m_pBits,
							0,
							0);

	if (m_hbmp == 0)
		return;

	if (m_pBits == 0)
		return;

	  // Select the bitmap into the buffer
	if (m_hbmp)
	{
		m_hbmOld = (HBITMAP)::SelectObject(m_hdc, m_hbmp);
	} 
}

void DIBSection::Draw(HDC hdcDest, int x, int y) 
{
	BitBlt(hdcDest, 0, 0,
		m_nWidth, m_nHeight,
		m_hdc,
		x, y,
		SRCCOPY);
}

void DIBSection::SetPixel(int x, int y, unsigned char r, unsigned char g, unsigned char b)
{
	if ((x < Width()) && (y < Height()))
	{
		unsigned char * bits = (unsigned char *)GetBits();
		int offset = x * GetBitCount()/8 + (Height() - y - 1)*GetTotalWidth()*GetBitCount()/8;
		bits[offset++] = b;
		bits[offset++] = g;
		bits[offset] = r;
	}
}

void DIBSection::GetPixel(int x, int y, unsigned char& r, unsigned char& g, unsigned char& b)
{
	if ((x < Width()) && (y < Height()))
	{
		unsigned char * src = (unsigned char *)GetBits();

		int offset = x * GetBitCount()/8 + (Height() - y - 1)*GetTotalWidth()*GetBitCount()/8;
		b = src[offset++];
		g = src[offset++];
		r = src[offset];
	}
}

void DIBSection::ResizeImage(DIBSection& dst_dib, int w, int h)
{
	if ((w > 0) && (h > 0))
	{
		dst_dib.Create(w,h,GetBitCount());

		if (dst_dib.IsCreated())
		{
			int width = dst_dib.Width();
			int height = dst_dib.Height();
			int src_width = Width();
			int src_height = Height();

			if ((dst_dib.IsCreated() && (width > 0) && (height > 0)) &&
				(IsCreated() && (src_width > 0) && (src_height > 0)))
			{
				double horizontal_scale = (double)src_width/(double)dst_dib.Width();
				double vertical_scale = (double)src_height/(double)dst_dib.Height();

				unsigned char * src_ptr = (unsigned char *)GetBits();
				unsigned char * dst_ptr = (unsigned char *)dst_dib.GetBits();

				int src_bytecount = GetBitCount() / 8;
				int src_dibwidth = GetTotalWidth();

				int dst_bytecount = dst_dib.GetBitCount() / 8;
				int dst_dibwidth = dst_dib.GetTotalWidth();
				int dst_bitcount = dst_dib.GetBitCount();

				int src_row, src_col;
				int src_index, dst_index;

				for (int row = 0; row<height; row++){
					src_row = (int)(row * vertical_scale + 0.50);
					if (src_row >= src_height) src_row = src_height - 1;

					for (int col = 0; col<width; col++){
						src_col = (int)(col * horizontal_scale + 0.50);
						if (src_col >= src_width) src_col = src_width-1;
						src_index = src_row * src_dibwidth * src_bytecount + src_col*src_bytecount;
						dst_index = row*dst_dibwidth*dst_bytecount+col*dst_bytecount;
						memcpy(&dst_ptr[dst_index],
							&src_ptr[src_index],
							dst_bytecount);
					}
				}
			}
		}
	}
}

void DIBSection::PatBlt(DWORD pattern)
{
	if (IsCreated())
	{
		if ((Width() > 0) && (Height() > 0))
		{
			::PatBlt(m_hdc, 0, 0, Width(), Height(), pattern);
		}
	}
}
