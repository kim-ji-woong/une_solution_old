#include "stdafx.h"
#include "VectorCtrl.h"
#include "Layer.h"
#include <algorithm>
#include <locale>

namespace VectorGraphics
{
	VectorCtrl::VectorCtrl()
	{
		m_fColLine[0] = 1.0f;
		m_fColLine[1] = 1.0f;
		m_fColLine[2] = 0.0f;

		m_colBk = RGB(0, 0, 0);

		m_nWidth = m_nHeight = 0;
		m_dViewportWeight = 1.0;

		m_isMButtonDown = false;
		m_nClickedX = m_nClickedY = 0;

		m_isInitialized = false;
	}


	VectorCtrl::~VectorCtrl()
	{
	}

	void VectorCtrl::OnCreate(int nWidth, int nHeight)
	{
		InitSize(nWidth, nHeight);
	}

	void VectorCtrl::InitSize(int nWidth, int nHeight)
	{
		CreateImage(nWidth, nHeight);

		m_vTL.y = nHeight;
		m_vBR.x = nWidth;
		m_nWidth = nWidth;
		m_nHeight = nHeight;
	}

	void VectorCtrl::CreateImage(int nWidth, int nHeight)
	{
		if ((nWidth > 0) && (nHeight > 0))
		{
			m_dib.Create(nWidth, nHeight, 24);

			if (!IsInitialized())
			{
				if (SetImagePixelFormat(m_dib.GetHDC()) && CreateImageGLContext(m_dib.GetHDC()))
				{
					m_isInitialized = true;
					InitRC();
				}
			}
		}
	}

	void VectorCtrl::InitRC()
	{
		glClearColor(GetRValue(m_colBk) / 255.0f, GetGValue(m_colBk) / 255.0f, GetBValue(m_colBk) / 255.0f, 1.0f);
		glEnable(GL_LINE_STIPPLE);
	}

	BOOL VectorCtrl::SetImagePixelFormat(HDC hdc)
	{
		PIXELFORMATDESCRIPTOR pixelDesc;

		pixelDesc.nSize = sizeof(PIXELFORMATDESCRIPTOR);
		pixelDesc.nVersion = 1;

		pixelDesc.dwFlags = PFD_DRAW_TO_BITMAP | PFD_SUPPORT_OPENGL |
			PFD_SUPPORT_GDI;

		pixelDesc.iPixelType = PFD_TYPE_RGBA;
		pixelDesc.cColorBits = 24;
		pixelDesc.cRedBits = 0;
		pixelDesc.cRedShift = 0;
		pixelDesc.cGreenBits = 0;
		pixelDesc.cGreenShift = 0;
		pixelDesc.cBlueBits = 0;
		pixelDesc.cBlueShift = 0;
		pixelDesc.cAlphaBits = 0;
		pixelDesc.cAlphaShift = 0;
		pixelDesc.cAccumBits = 0;
		pixelDesc.cAccumRedBits = 0;
		pixelDesc.cAccumGreenBits = 0;
		pixelDesc.cAccumBlueBits = 0;
		pixelDesc.cAccumAlphaBits = 0;
		pixelDesc.cDepthBits = 24;
		pixelDesc.cStencilBits = 0;
		pixelDesc.cAuxBuffers = 0;
		pixelDesc.iLayerType = PFD_MAIN_PLANE;
		pixelDesc.bReserved = 0;
		pixelDesc.dwLayerMask = 0;
		pixelDesc.dwVisibleMask = 0;
		pixelDesc.dwDamageMask = 0;

		int pix_index = ChoosePixelFormat(hdc, &pixelDesc);

		if (!SetPixelFormat(hdc, pix_index, &pixelDesc))
		{
			DWORD code = GetLastError();
			return FALSE;
		}

		return TRUE;
	}

	BOOL VectorCtrl::CreateImageGLContext(HDC hdc)
	{
		m_hRC = wglCreateContext(hdc);

		if (m_hRC == NULL)
			return FALSE;

		if (wglMakeCurrent(hdc, m_hRC) == FALSE)
		{
			SwapBuffers(hdc);
			wglMakeCurrent(hdc, NULL);
			return FALSE;
		}

		SwapBuffers(hdc);
		wglMakeCurrent(hdc, NULL);
		return TRUE;
	}

	void VectorCtrl::OnDraw(HDC hdc)
	{
		HDC hdc2 = GetHDC();

		wglMakeCurrent(hdc2, m_hRC);
		glClearColor(GetRValue(m_colBk) / 255.0f, GetGValue(m_colBk) / 255.0f, GetBValue(m_colBk) / 255.0f, 1.0f);

		Display(hdc);

		wglMakeCurrent(hdc2, NULL);
	}

	void VectorCtrl::Display(HDC hdc)
	{
		RenderImage();
		glFlush();
		DrawImage(hdc);
	}

	void DrawLayer(Layer* pLayer)
	{
		pLayer->Draw();
	}

	void VectorCtrl::RenderImage()
	{
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
		glEnable(GL_LINE_STIPPLE);

		glPushMatrix();

		std::for_each(m_listLayers.begin(), m_listLayers.end(), DrawLayer);

		glPopMatrix();
	}

	void VectorCtrl::DrawImage(HDC hdc)
	{
		m_dib.Draw(hdc, 0, 0);
	}

	void VectorCtrl::AddLayer(Layer* pLayer)
	{
		m_listLayers.push_back(pLayer);
		pLayer->SetControl(this);
	}

	void VectorCtrl::RemoveLayer(Layer* pLayer)
	{
		m_listLayers.remove(pLayer);
	}

	int VectorCtrl::GetLayerCount()
	{
		return (int)m_listLayers.size();
	}

	Layer* VectorCtrl::GetLayer(int nIndex)
	{
		if (nIndex < 0 || nIndex >= GetLayerCount())
			return 0;

		std::list<Layer*>::iterator iter = m_listLayers.begin();

		for (int i = 0; i < nIndex; i++)
		{
			iter++;
		}

		return *iter;
	}

	void VectorCtrl::Clear()
	{
		m_listLayers.clear();
	}

	void VectorCtrl::OnSize(int nWidth, int nHeight)
	{
		m_nWidth = nWidth;
		m_nHeight = nHeight;

		if (nWidth > 0 && nHeight > 0)
		{
			SizeImage(nWidth, nHeight);

			HDC hdc = GetHDC();
			wglMakeCurrent(hdc, m_hRC);
			Reshape(nWidth, nHeight);
			wglMakeCurrent(NULL, NULL);
		}
	}

	void VectorCtrl::GetScreenSize(int& rWidth, int& rHeight)
	{
		rWidth = m_nWidth;
		rHeight = m_nHeight;
	}

	HDC VectorCtrl::GetHDC()
	{
		return m_dib.GetHDC();
	}

	void VectorCtrl::Reshape(int nWidth, int nHeight)
	{
		GLfloat fRange = 1000.0f;

		// Prevent a divide by zero
		if (nHeight == 0)
			nHeight = 1;

		// Set Viewport to window dimensions
		glViewport(0, 0, nWidth, nHeight);

		// Reset projection matrix stack
		glMatrixMode(GL_PROJECTION);
		glLoadIdentity();

		m_nWidth = nWidth;
		m_nHeight = nHeight;

		m_vBR = m_vBL.GetLinearVertex(m_vBR, nWidth * m_dViewportWeight);
		m_vTL = m_vBL.GetLinearVertex(m_vTL, nHeight * m_dViewportWeight);
		m_vCenter = (m_vTL + m_vBR) / 2;

		glOrtho(m_vCenter.x - m_nWidth * m_dViewportWeight / 2.0, m_vCenter.x + m_nWidth * m_dViewportWeight / 2.0,
			m_vCenter.y - m_nHeight * m_dViewportWeight / 2.0, m_vCenter.y + m_nHeight * m_dViewportWeight / 2.0,
			-fRange, fRange);

		// Reset Model view matrix stack
		glMatrixMode(GL_MODELVIEW);
		glLoadIdentity();
	}

	void VectorCtrl::SizeImage(int nWidth, int nHeight)
	{
		if (nWidth > 0 && nHeight > 0)
		{
			if (!IsInitialized())
			{
				CreateImage(nWidth, nHeight);
			}
			else
			{
				if (!m_dib.IsCreated())
				{
					m_dib.Create(nWidth, nHeight, 24);

					if (SetImagePixelFormat(GetHDC()))
					{
						CreateImageGLContext(GetHDC());
					}
				}
				else if ((nWidth != m_dib.Width()) || (nHeight != m_dib.Height()))
				{
					m_dib.Create(nWidth, nHeight, 24);
					SetImagePixelFormat(GetHDC());
					CreateImageGLContext(GetHDC());
				}
			}
		}
	}

	void VectorCtrl::SetViewportCenter(double x, double y)
	{
		double dMoveX = x - m_vCenter.x;
		double dMoveY = y - m_vCenter.y;

		m_vTL.x += dMoveX;
		m_vBL.x += dMoveX;
		m_vBR.x += dMoveX;
		m_vCenter.x += dMoveX;

		m_vTL.y += dMoveY;
		m_vBL.y += dMoveY;
		m_vBR.y += dMoveY;
		m_vCenter.y += dMoveY;

		HDC hdc = GetHDC();

		wglMakeCurrent(hdc, m_hRC);
		Reshape(m_nWidth, m_nHeight);
		wglMakeCurrent(NULL, NULL);
	}

	Vertex2D VectorCtrl::GetViewportCenter()
	{
		return m_vCenter;
	}

	void VectorCtrl::MouseDown(int x, int y, VectorCtrl::MouseType type)
	{
		if (type == MouseType::MBUTTON)
			m_isMButtonDown = true;

		m_nClickedX = x;
		m_nClickedY = y;
		m_vClicked = m_vCenter;
	}

	void VectorCtrl::MouseUp(int x, int y, VectorCtrl::MouseType type)
	{
		if (type == MouseType::MBUTTON)
			m_isMButtonDown = false;
	}

	bool VectorCtrl::MouseMove(int x, int y)
	{
		if (m_isMButtonDown)
		{
			int nMoveX = x - m_nClickedX;
			int nMoveY = y - m_nClickedY;
			double dMoveX = (m_vBR.x - m_vBL.x) * nMoveX / m_nWidth;
			double dMoveY = (m_vBL.y - m_vTL.y) * nMoveY / m_nHeight;
			
			SetViewportCenter(m_vClicked.x - dMoveX, m_vClicked.y - dMoveY);
			return true;
		}

		return false;
	}

	void VectorCtrl::ScreenToGlobal(int x, int y, Vertex2D* pVertex)
	{
		pVertex->x = (m_vBR.x - m_vBL.x) * x / m_nWidth + m_vTL.x;
		pVertex->y = m_vTL.y - (m_vTL.y - m_vBL.y) * y / m_nHeight;
	}

	void VectorCtrl::GlobalToScreen(const Vertex2D& rVertex, int* x, int* y)
	{
		*x = (int)((rVertex.x - m_vTL.x) * m_nWidth / (m_vBR.x - m_vBL.x));
		*y = (int)((m_vTL.y - rVertex.y) * m_nHeight / (m_vTL.y - m_vBL.y));
	}

	void VectorCtrl::SetViewportWeight(double dWeight)
	{
		if (dWeight < 0.0)
			return;

		m_dViewportWeight = dWeight;

		HDC hdc = GetHDC();
		wglMakeCurrent(hdc, m_hRC);
		Reshape(m_nWidth, m_nHeight);
		wglMakeCurrent(NULL, NULL);
	}

	double VectorCtrl::GetViewportWeight()
	{
		return m_dViewportWeight;
	}

	void VectorCtrl::Zoom(int x, int y, double dWeight)
	{
		if (dWeight < 0.0)
			return;

		Vertex2D vZoom;
		ScreenToGlobal(x, y, &vZoom);

		m_vTL = vZoom + (m_vTL - vZoom) * dWeight / m_dViewportWeight;
		m_vBL = vZoom + (m_vBL - vZoom) * dWeight / m_dViewportWeight;
		m_vBR = vZoom + (m_vBR - vZoom) * dWeight / m_dViewportWeight;
		m_vCenter = (m_vTL + m_vBR) / 2;

		m_dViewportWeight = dWeight;

		HDC hdc = GetHDC();
		wglMakeCurrent(hdc, m_hRC);
		Reshape(m_nWidth, m_nHeight);
		wglMakeCurrent(NULL, NULL);
	}

	Shape* VectorCtrl::HitTest(const Vertex2D& vPos)
	{
		for (std::list<Layer*>::iterator iter = m_listLayers.begin(); iter != m_listLayers.end(); iter++)
		{
			Layer* pLayer = *iter;
			Shape* pShape = pLayer->HitTest(vPos);

			if (pShape)
				return pShape;
		}

		return 0;
	}

	Shape* VectorCtrl::HitTestPOI(const Vertex2D& vPos)
	{
		for (std::list<Layer*>::iterator iter = m_listLayers.begin(); iter != m_listLayers.end(); iter++)
		{
			Layer* pLayer = *iter;
			Shape* pShape = pLayer->HitTestPOI(vPos);

			if (pShape)
				return pShape;
		}

		return 0;
	}

	Shape* VectorCtrl::HitTestExceptPOI(const Vertex2D& vPos)
	{
		for (std::list<Layer*>::iterator iter = m_listLayers.begin(); iter != m_listLayers.end(); iter++)
		{
			Layer* pLayer = *iter;
			Shape* pShape = pLayer->HitTestExceptPOI(vPos);

			if (pShape)
				return pShape;
		}

		return 0;
	}

	void VectorCtrl::SetBackgroundColor(COLORREF colBk)
	{
		m_colBk = colBk;
	}

	COLORREF VectorCtrl::GetBackgroundColor()
	{
		return m_colBk;
	}
}
