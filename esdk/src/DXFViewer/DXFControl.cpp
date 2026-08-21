#include "StdAfx.h"
#include "DXFControl.h"
#include "Layer.h"
#include "DXFLoader.h"
#include "Shape.h"
#include "LineType.h"
#include "PlotSettings.h"
#include "UPrintDocument.h"
//#include "dibsect.h"

//#include <GL/gl.h>
//#include <GL/glu.h>

#include "Block.h"
#include "Line.h"
#include "PolyLine.h"
#include "Text.h"
#include "ShapeGroup.h"
#include "Hatch.h"
#include "EArc.h"
#include "Arc.h"
#include "EditBox.h"

using namespace System::Drawing;
using namespace System::Collections;
using namespace UnE::Geometry;

namespace DXFViewer
{
	static bool SetImagePixelFormat(HDC hDC);
	static bool CreateImageGLContext(HDC hDC, int* pRC);
	static HDC GetHDC(Bitmap^ bmp);
	static HDC GetHDC(Bitmap^ bmp, [System::Runtime::InteropServices::OutAttribute] System::Drawing::Graphics^% g);

	bool DXFControl::OpenDXF(System::String^ strPath)
	{
		DXFLoader^ loader = gcnew DXFLoader(this, m_arrLayer);
		loader->UseLastViewport = this->UseLastViewport;
		m_isOpened = loader->Load(strPath);
		return m_isOpened;
	}

	void DXFControl::CloseDXF()
	{
		m_isOpened = false;

		m_pCurrentLayer = nullptr;
		m_pCurrentBlock = nullptr;

		m_arrLayer->Clear();
		m_arrBlock->Clear();

		m_currentMatrix = nullptr;
		m_isPanning = false;
	}

	void DXFControl::SetDoubleBuffered(bool bDoubleBuffered)
	{
		if (System::Windows::Forms::SystemInformation::TerminalServerSession)
			return;
		System::Type^ type = System::Windows::Forms::Control::typeid;

		System::Reflection::PropertyInfo^ aProp =
			type->GetProperty(
			"DoubleBuffered",
			System::Reflection::BindingFlags::NonPublic |
			System::Reflection::BindingFlags::Instance);

		aProp->SetValue(this, bDoubleBuffered, nullptr);
	}

	void DXFControl::Init()
	{
		SetDoubleBuffered(true);
		
		m_bDrawText = true;

		this->MinimumSize = System::Drawing::Size(100, 100);
		m_dicPens = gcnew System::Collections::Generic::Dictionary<__int64, System::Drawing::Pen^>();

		m_dViewportWeight = 1.0;

		m_vViewportTL = gcnew Vertex2D(0.0, 0.0);
		m_vViewportBL = gcnew Vertex2D(0.0, 0.0);
		m_vViewportBR = gcnew Vertex2D(0.0, 0.0);

		m_pCurrentLayer = nullptr;
		m_pCurrentBlock = nullptr;

		m_arrLayer = gcnew ArrayList();
		m_arrBlock = gcnew ArrayList();

		m_currentMatrix = nullptr;

		m_btnPanning = System::Windows::Forms::MouseButtons::Middle;
		m_isPanning = false;

		m_useMouseWheel = true;
		m_unitOfLength = DXFViewer::UnitOfLength::MILLIMETER;

		m_lineTypeSelected = gcnew LineType(this, System::Drawing::Drawing2D::DashStyle::Dash, 1);
		m_lineTypeSelected->GetPen()->Color = System::Drawing::Color::White;

		m_vMove = gcnew UnE::Geometry::Vertex2D();

		m_penEditBox = gcnew System::Drawing::Pen(System::Drawing::Color::Gray);
		m_brushEditBox = gcnew System::Drawing::SolidBrush(System::Drawing::Color::FromArgb(0, 127, 255));
		m_nEditBoxSize = 10;
		m_fEditBoxLength = 10.0f;

		m_isOpened = false;

		m_fHomem11 = m_fHomem12 = m_fHomem21 = m_fHomem22 = 0.0f;
		m_fHomedx = m_fHomedy = 0.0f;
		m_vHomeViewportTL = m_vHomeViewportBL = m_vHomeViewportBR = nullptr;
		m_dHomeViewportWeight = 0.0;

		m_nGroupItemDistance = 30;
		m_useGroupItem = false;
		m_nGroupItemMinCount = 3;

		m_drawHatchFirst = true;
		m_useLastViewport = false;
		m_useAntialiasing = true;

		//m_penSelectedBright1 = gcnew System::Drawing::Pen(System::Drawing::Color::FromArgb(255 - this->BackColor.R, 255 - this->BackColor.G, 255 - this->BackColor.B));
		//m_penSelectedBright2 = gcnew System::Drawing::Pen(this->BackColor);
		//m_penSelectedBright2->DashStyle = System::Drawing::Drawing2D::DashStyle::Dot;

		m_plotSettings = gcnew DXFViewer::PlotSettings();
		m_externPainter = nullptr;


		//m_plotSettings = gcnew DXFViewer::PlotSettings();
		m_openNRefresh = true;
		m_renderType = IPainter::RendererType::GDI_PLUS;

		//m_pDIB = new DIBSection();
		//m_bitmap = nullptr;
		m_isInitialized = false;
		m_nRC = 0;

		m_dOrthoLeft = m_dOrthoTop = m_dOrthoNear = 0.0;
		m_hdcCurrent = System::IntPtr::Zero;

		m_vObjectTL = nullptr;
		m_vObjectBR = nullptr;

		m_img = nullptr;
		//m_makeImage = true;
		m_nMoveX = m_nMoveY = 0;
	}

	void DXFControl::SetCurrentLayer(Layer^ layer)
	{
		m_pCurrentLayer = layer;
	}

	Layer^ DXFControl::GetCurrentLayer()
	{
		return m_pCurrentLayer;
	}

	void DXFControl::SetCurrentBlock(Block^ block)
	{
		m_pCurrentBlock = block;
	}

	Block^ DXFControl::GetCurrentBlock()
	{
		return m_pCurrentBlock;
	}

	void DXFControl::SetViewportCenter(Vertex2D^ vCenter)
	{
		//double dTrans[2] = { vCenter->x - (m_vViewportTL->x + m_vViewportBR->x) / 2, vCenter->y - (m_vViewportTL->y + m_vViewportBR->y) / 2 };

		m_dOrthoLeft = vCenter->x - this->Size.Width * m_dViewportWeight / 2;
		m_dOrthoTop = vCenter->y - this->Size.Height * m_dViewportWeight / 2;
		m_dOrthoNear = 0.0;

		double x = this->Size.Width / m_dViewportWeight / 2;
		double y = this->Size.Height / m_dViewportWeight / 2;
		
		m_vViewportTL->x = vCenter->x - x;
		m_vViewportTL->y = vCenter->y + y;
		m_vViewportBL->x = vCenter->x - x;
		m_vViewportBL->y = vCenter->y - y;
		m_vViewportBR->x = vCenter->x + x;
		m_vViewportBR->y = vCenter->y - y;

		/*m_vViewportTL->x += dTrans[0];
		m_vViewportBL->x += dTrans[0];
		m_vViewportBR->x += dTrans[0];

		m_vViewportTL->y += dTrans[1];
		m_vViewportBL->y += dTrans[1];
		m_vViewportBR->y += dTrans[1];*/

		/*if (m_renderType == IPainter::RendererType::OPEN_GL)
		{
			HDC hdc = ((DIBSection*)m_pDIB)->GetDC();
			wglMakeCurrent(hdc, (HGLRC)m_nRC);
			Reshape(this->Size.Width, this->Size.Height);
			wglMakeCurrent(0, 0);
		}
		else */
		if (m_renderType == IPainter::RendererType::GDI_PLUS)
			Reshape(this->Size.Width, this->Size.Height);
	}

	Vertex2D^ DXFControl::GetViewportCenter()
	{
		return (m_vViewportTL + m_vViewportBR) / 2;
	}

	double DXFControl::GetViewportWeight()
	{
		return m_dViewportWeight;
	}

	void DXFControl::SetViewportWeight(double dWeight)
	{
		if (dWeight <= UnE::Geometry::Math::HALF_TOLERANCE())
			return;

		m_dViewportWeight = dWeight;

		if (this->Size.Width > 0 && this->Size.Height > 0)
		{
			Vertex2D^ vCenter = GetViewportCenter();
			double x = this->Size.Width / dWeight / 2;
			double y = this->Size.Height / dWeight / 2;

			m_vViewportTL->x = vCenter->x - x;
			m_vViewportTL->y = vCenter->y + y;
			m_vViewportBL->x = vCenter->x - x;
			m_vViewportBL->y = vCenter->y - y;
			m_vViewportBR->x = vCenter->x + x;
			m_vViewportBR->y = vCenter->y - y;

			Reshape(this->Size.Width, this->Size.Height);
		}
	}

	int DXFControl::GetScreenWidth()
	{
		return this->Size.Width;
	}

	int DXFControl::GetScreenHeight()
	{
		return this->Size.Height;
	}

	LineType^ DXFControl::GetSelectedLineType()
	{
		return m_lineTypeSelected;
	}

	void DXFControl::OnLoad(System::Object^ sender, System::EventArgs^ e)
	{
		InitSize();
	}

	void DXFControl::InitSize()
	{
		int nWidth = this->Size.Width;
		int nHeight = this->Size.Height;
		CreateImage(nWidth, nHeight);

		if (m_renderType == IPainter::RendererType::OPEN_GL)
		{
			m_vViewportTL->x = m_vViewportBL->x = 0.0;
			m_vViewportTL->y = nHeight;
			m_vViewportBL->y = 0.0;
			m_vViewportBR->x = nWidth;
			m_vViewportBR->y = m_vViewportBL->y;
		}
		else
		{
			m_vViewportTL->x = m_vViewportBL->x = 0.0;
			m_vViewportTL->y = 0.0;
			m_vViewportBL->y = nHeight;
			m_vViewportBR->x = nWidth;
			m_vViewportBR->y = m_vViewportBL->y;
		}

		m_dViewportWeight = 1.0;

		Reshape(nWidth, nHeight);
	}

	void DXFControl::CreateImage(int nWidth, int nHeight)
	{
		if (nWidth > 0 && nHeight > 0)
		{
			//((DIBSection*)m_pDIB)->Create(nWidth, nHeight, 24);
			//m_bitmap = gcnew Bitmap(nWidth, nHeight);

			//if (!m_isInitialized && m_renderType == IPainter::RendererType::OPEN_GL)
			//{
			//	//HDC hdc = ((DIBSection*)m_pDIB)->GetDC();
			//	//HDC hdc = GetHDC(m_bitmap);
			//	
			//	int nRC;

			//	if (SetImagePixelFormat(hdc) && CreateImageGLContext(hdc, &nRC))
			//	{
			//		m_nRC = nRC;
			//		InitRC();
			//	}
			//}

			m_isInitialized = true;
		}
	}

	void DXFControl::InitRC()
	{
		// VText 사용시만...
		//InitFont();

		//glClearColor(this->BackColor.R / 255.0f, this->BackColor.G / 255.0f, this->BackColor.B / 255.0f, 1.0f);
		//glEnable(GL_LINE_STIPPLE);
		//SetAntiAliasing(true);
	}

	static HDC GetHDC(Bitmap^ bmp)
	{
		System::Drawing::Graphics^ g;
		return GetHDC(bmp, g);
	}

	static HDC GetHDC(Bitmap^ bmp, [System::Runtime::InteropServices::OutAttribute] System::Drawing::Graphics^% g)
	{
		g = System::Drawing::Graphics::FromImage(bmp);
		int nHDC = g->GetHdc().ToInt32();
		return (HDC)nHDC;
		//return (HDC)g->GetHdc().ToInt32();
	}

	static bool CreateImageGLContext(HDC hDC, int* pRC)
	{
		HGLRC hrc = wglCreateContext(hDC);

		if (hrc == NULL)
			return FALSE;

		*pRC = (int)hrc;

		if (wglMakeCurrent(hDC, hrc) == FALSE)
		{
			SwapBuffers(hDC);
			wglMakeCurrent(hDC, NULL);
			return FALSE;
		}

		SwapBuffers(hDC);
		wglMakeCurrent(hDC, NULL);
		return TRUE;
	}

	static bool SetImagePixelFormat(HDC hDC)
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

		int pix_index = ChoosePixelFormat(hDC, &pixelDesc);

		if (!SetPixelFormat(hDC, pix_index, &pixelDesc))
		{
			//DWORD code = GetLastError();
			return false;
		}

		return true;
	}

	void DXFControl::OnSize(System::Object^ sender, System::EventArgs^ e)
	{
		int nWidth = this->Size.Width;
		int nHeight = this->Size.Height;

		if (nWidth == 0 || nHeight == 0)
			return;

		SizeImage(nWidth, nHeight);

		//if (m_renderType == IPainter::RendererType::OPEN_GL)
		//{
		//	HDC hdc = ((DIBSection*)m_pDIB)->GetDC();
		//	//HDC hdc = GetHDC(m_bitmap);
		//	wglMakeCurrent(hdc, (HGLRC)m_nRC);
		//	Reshape(nWidth, nHeight);
		//	wglMakeCurrent(NULL, NULL);
		//}
		//else
			Reshape(nWidth, nHeight);

		UPrintDocument ^ doc = (UPrintDocument^)mPrintDocument;
		if (doc != nullptr)
			doc->DrawingSize = gcnew System::Drawing::Size(nWidth, nHeight);

		m_img = nullptr;
		//m_makeImage = true;

		Refresh();
	}

	void DXFControl::SizeImage(int width, int height)
	{
		if ((width > 0) && (height > 0))
		{
			if (!m_isInitialized)
			{
				CreateImage(width, height);
			}
			else
			{
				//if (((DIBSection*)m_pDIB)->IsCreated())
				////if (m_bitmap == nullptr)
				//{
				//	((DIBSection*)m_pDIB)->Create(width, height, 24);
				//	HDC hdc = ((DIBSection*)m_pDIB)->GetDC();
				//	//m_bitmap = gcnew Bitmap(width, height);
				//	//HDC hdc = GetHDC(m_bitmap);

				//	if (SetImagePixelFormat(hdc))
				//	{
				//		int nRC;
				//		
				//		if (CreateImageGLContext(hdc, &nRC))
				//			m_nRC = nRC;
				//	}
				//}
				//else if ((width != ((DIBSection*)m_pDIB)->Width()) || (height != ((DIBSection*)m_pDIB)->Height()))
				//{
				//	((DIBSection*)m_pDIB)->Create(width, height, 24);
				//	HDC hdc = ((DIBSection*)m_pDIB)->GetDC();
				//	//m_bitmap = gcnew Bitmap(width, height);
				//	//HDC hdc = GetHDC(m_bitmap);

				//	SetImagePixelFormat(hdc);

				//	int nRC;

				//	if (CreateImageGLContext(hdc, &nRC))
				//		m_nRC = nRC;
				//}
			}
		}
	}

	void DXFControl::ReshapeGL(int nWidth, int nHeight)
	{
		//GLfloat fRange = 1000.0f;

		//// Prevent a divide by zero
		//if (nHeight == 0)
		//	nHeight = 1;

		//// Set Viewport to window dimensions
		//glViewport(0, 0, nWidth, nHeight);

		//// Reset projection matrix stack
		//glMatrixMode(GL_PROJECTION);
		//glLoadIdentity();

		//// Establish clipping volume (left, right, bottom, top, near, far)
		///*glOrtho(m_dArrViewportBL[0], nWidth * m_dViewportWeight + m_dArrViewportBL[0],
		//m_dArrViewportBL[1], nHeight * m_dViewportWeight + m_dArrViewportBL[1],
		//-fRange,fRange);*/

		//m_dOrthoLeft = this->m_vViewportBL->x;
		//m_dOrthoTop = this->m_vViewportBL->y;

		///*int nDepth = nWidth > nHeight ? nWidth : nHeight;
		//glOrtho(m_dOrthoLeft, nWidth * m_dViewportWeight + m_dOrthoLeft,
		//m_dOrthoTop, nHeight * m_dViewportWeight + m_dOrthoTop,
		//-fRange,fRange);*/

		//Vertex2D^ vTL = gcnew Vertex2D(m_vViewportTL->x, m_vViewportTL->y);
		//Vertex2D^ vBL = gcnew Vertex2D(m_vViewportBL->x, m_vViewportBL->y);

		//m_vViewportBR = UnE::Geometry::Math::GetLinearVertex(m_vViewportBL, m_vViewportBR, nWidth * m_dViewportWeight);
		//m_vViewportTL = UnE::Geometry::Math::GetLinearVertex(m_vViewportBL, m_vViewportTL, nHeight * m_dViewportWeight);

		//LoadOrtho();
	}

	void DXFControl::LoadOrtho()
	{
		//GLfloat fRange = 1000.0f;

		//Vertex2D^ vCenter = (m_vViewportTL + m_vViewportBL) / 2;

		//glOrtho(vCenter->x - this->Size.Width * m_dViewportWeight / 2.0, vCenter->x + this->Size.Width * m_dViewportWeight / 2.0,
		//	vCenter->y - this->Size.Height * m_dViewportWeight / 2.0, vCenter->y + this->Size.Height * m_dViewportWeight / 2.0,
		//	-fRange, fRange);

		//// Reset Model view matrix stack
		//glMatrixMode(GL_MODELVIEW);
		//glLoadIdentity();
	}

	void DXFControl::ReshapeGDI(int nWidth, int nHeight)
	{
		if (nWidth <= 0 || nHeight <= 0)
			return;

		float m11 = (float)((m_vViewportBR->x - m_vViewportBL->x) / nWidth);
		float m21 = (float)((m_vViewportBL->x - m_vViewportTL->x) / nHeight);
		float dx = (float)m_vViewportTL->x;
		float m12 = (float)((m_vViewportBR->y - m_vViewportBL->y) / nWidth);
		float m22 = (float)((m_vViewportBL->y - m_vViewportTL->y) / nHeight);
		float dy = (float)m_vViewportTL->y;

		m_currentInverseMatrix = gcnew Drawing2D::Matrix(m11, m12, m21, m22, dx, dy);
		m_currentMatrix = m_currentInverseMatrix->Clone();

		try
		{
			if (m_currentMatrix->IsInvertible)
				m_currentMatrix->Invert();
			else
				m_currentMatrix = gcnew Drawing2D::Matrix(1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f);
		}
		catch (System::ArgumentException^)
		{
			m_currentMatrix = gcnew Drawing2D::Matrix(1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f);
		}
		/*Vertex2D^ vTR = m_vViewportTL + m_vViewportBR - m_vViewportBL;
		vTR = UnE::Geometry::Math::GetLinearVertex(m_vViewportTL, vTR, nWidth / m_dViewportWeight);
		m_vViewportBL = UnE::Geometry::Math::GetLinearVertex(m_vViewportTL, m_vViewportBL, nHeight / m_dViewportWeight);
		m_vViewportBR = m_vViewportBL + vTR - m_vViewportTL;

		// Matrix
		//                        ┌──────┐
		//                        │m11  m12  0│
		// [x'  y'  1] = [x  y  1]│m21  m22  0│
		//                        │dx   dy   1│
		//                        └──────┘
		double dWeight = m_dViewportWeight * m_dViewportWeight;
		float m11 = (float)((m_vViewportBR->x - m_vViewportBL->x) / this->Size.Width * dWeight);
		// 회전을 고려하지 않으면 m12와 m21은 0이 되면 되지만, 회전을 고려하면 값을 가져야 한다.
		float m12 = (float)((m_vViewportBR->y - m_vViewportBL->y) / this->Size.Width * dWeight);
		float m21 = (float)((m_vViewportBL->x - m_vViewportTL->x) / this->Size.Height * dWeight);
		//float m12 = 0.0f;
		//float m21 = 0.0f;
		float m22 = (float)((m_vViewportBL->y - m_vViewportTL->y) / this->Size.Height * dWeight);
		float dx = (float)m_vViewportTL->x;
		float dy = (float)m_vViewportTL->y;

		if (DownToTop())
		{
			// DXF 파일은 Y축이 아래에서 위쪽으로 향하므로 윈도우 좌표계와 세로축 방향이 다르다.
			// 세로축 방향으로 뒤집기 위하여 m12와 m22에 음수값을 준다.
			m_currentMatrix = gcnew Drawing2D::Matrix(m11, -m12, m21, -m22, -dx, dy);
		}
		else
		{
			m_currentMatrix = gcnew Drawing2D::Matrix(m11, m12, m21, m22, -dx, dy);
		}

		m_currentInverseMatrix = m_currentMatrix->Clone();

		try
		{
			m_currentInverseMatrix->Invert();
		}
		catch (System::ArgumentException^)
		{
			m_currentInverseMatrix = nullptr;
		}*/
	}

	void DXFControl::Reshape(int nWidth, int nHeight)
	{
		if (m_renderType == IPainter::RendererType::OPEN_GL)
			ReshapeGL(nWidth, nHeight);
		else if (m_renderType == IPainter::RendererType::GDI_PLUS)
			ReshapeGDI(nWidth, nHeight);
	}

	void DXFControl::SaveHomeMatrix()
	{
		if (m_currentMatrix == nullptr)
			return;

		m_fHomem11 = m_currentMatrix->Elements[0];
		m_fHomem12 = m_currentMatrix->Elements[1];
		m_fHomem21 = m_currentMatrix->Elements[2];
		m_fHomem22 = m_currentMatrix->Elements[3];
		m_fHomedx = m_currentMatrix->Elements[4];
		m_fHomedy = m_currentMatrix->Elements[5];

		m_vHomeViewportTL = gcnew Vertex2D(m_vViewportTL->x, m_vViewportTL->y);
		m_vHomeViewportBL = gcnew Vertex2D(m_vViewportBL->x, m_vViewportBL->y);
		m_vHomeViewportBR = gcnew Vertex2D(m_vViewportBR->x, m_vViewportBR->y);

		m_dHomeViewportWeight = m_dViewportWeight;
	}

	void DXFControl::LoadHomeMatrix(bool refresh)
	{
		if (m_vHomeViewportTL == nullptr)
			return;

		m_vViewportTL->x = m_vHomeViewportTL->x;
		m_vViewportTL->y = m_vHomeViewportTL->y;
		m_vViewportBL->x = m_vHomeViewportBL->x;
		m_vViewportBL->y = m_vHomeViewportBL->y;
		m_vViewportBR->x = m_vHomeViewportBR->x;
		m_vViewportBR->y = m_vHomeViewportBR->y;

		m_dViewportWeight = m_dHomeViewportWeight;

		m_currentMatrix = gcnew Drawing2D::Matrix(m_fHomem11, m_fHomem12, m_fHomem21, m_fHomem22, m_fHomedx, m_fHomedy);

		m_currentInverseMatrix = m_currentMatrix->Clone();

		try
		{
			m_currentInverseMatrix->Invert();
		}
		catch (System::ArgumentException^)
		{
			m_currentInverseMatrix = nullptr;
		}

		if (refresh)
			Refresh();
	}

	Viewport^ DXFControl::GetViewport()
	{
		if (m_currentMatrix == nullptr)
			return nullptr;

		Viewport^ viewport = gcnew Viewport();

		viewport->F11 = m_currentMatrix->Elements[0];
		viewport->F12 = m_currentMatrix->Elements[1];
		viewport->F21 = m_currentMatrix->Elements[2];
		viewport->F22 = m_currentMatrix->Elements[3];
		viewport->FDx = m_currentMatrix->Elements[4];
		viewport->FDy = m_currentMatrix->Elements[5];

		viewport->TopLeft = gcnew Vertex2D(m_vViewportTL->x, m_vViewportTL->y);
		viewport->BottomLeft = gcnew Vertex2D(m_vViewportBL->x, m_vViewportBL->y);
		viewport->BottomRight = gcnew Vertex2D(m_vViewportBR->x, m_vViewportBR->y);

		viewport->Weight = m_dViewportWeight;
		return viewport;
	}

	void DXFControl::LoadViewport(Viewport^ viewport, bool refresh)
	{
		if (viewport == nullptr)
			return;

		m_vViewportTL->x = viewport->TopLeft->x;
		m_vViewportTL->y = viewport->TopLeft->y;
		m_vViewportBL->x = viewport->BottomLeft->x;
		m_vViewportBL->y = viewport->BottomLeft->y;
		m_vViewportBR->x = viewport->BottomRight->x;
		m_vViewportBR->y = viewport->BottomRight->y;

		m_dViewportWeight = viewport->Weight;

		if (m_renderType == IPainter::RendererType::GDI_PLUS)
		{
			m_currentMatrix = gcnew Drawing2D::Matrix(viewport->F11, viewport->F12, viewport->F21, viewport->F22, viewport->FDx, viewport->FDy);

			m_currentInverseMatrix = m_currentMatrix->Clone();

			try
			{
				m_currentInverseMatrix->Invert();
			}
			catch (System::ArgumentException^)
			{
				m_currentInverseMatrix = nullptr;
			}
		}

		if (refresh)
			Refresh();
	}


	void DXFControl::OnPrintPage(System::Object^ sender, System::Drawing::Printing::PrintPageEventArgs^ e)
	{
		int width = this->Size.Width;
		int height = this->Size.Height;
		if (width == 0 || height == 0)
			return;

		DXFViewer::UPrintDocument^ document = (DXFViewer::UPrintDocument^)sender;
		if (document == nullptr)
			return;

		// Get Document Scale
		double a = document->Length;
		double b = document->UnitValue;
		double t = 25.4;
		if (document->LengthOfUnit == LengthUnit::mm)
		{
			t = 1.0;
		}

		b = System::Math::Round(b * t, 5);
		if (b == 0)
			return;

		float m_fScale = (float)(a / b);
		System::Drawing::Rectangle^ page = e->MarginBounds;


		// Create Back Image
		System::Drawing::Rectangle^ clipRect = gcnew System::Drawing::Rectangle(0, 0, width, height);
		Bitmap^ backImage = gcnew Bitmap(width, height, System::Drawing::Imaging::PixelFormat::Format32bppPArgb);
		Graphics^ gBack = Graphics::FromImage(backImage);


		// Draw Content on Image
		bool bTemp = m_useAntialiasing;
		m_useAntialiasing = true;
		OnPrint(this, gcnew System::Windows::Forms::PaintEventArgs(gBack, *clipRect));
		m_useAntialiasing = bTemp;

		Bitmap^ rectImage = nullptr;
		if (document->WindowPrintMode == true)
		{
			
			rectImage = backImage->Clone(*(document->DrawingRectSize), backImage->PixelFormat);
		}

		if (rectImage != nullptr)
			backImage = rectImage;
#ifdef DEBUG
		//backImage->Save("C:\\Users\\skkim\\\Desktop\\Print.bmp", System::Drawing::Imaging::ImageFormat::Bmp);
#endif
		// Draw Image		

		e->Graphics->ResetTransform();
		
		float offsetX = (float)document->OffsetX;
		float offsetY = (float)document->OffsetY;

		bool bUpsideDown = document->UpsideDown;
		if (bUpsideDown == true)
		{
			float fWidth = e->PageBounds.Width * 0.5f;
			float fHeight = e->PageBounds.Height * 0.5f;
			e->Graphics->TranslateTransform(fWidth, fHeight);
			e->Graphics->RotateTransform(180.0f);
			e->Graphics->TranslateTransform(-fWidth, -fHeight);

		}


		// 문서의 Margin을 Draw영역에서 제외한다.
		System::Drawing::Region^ region = gcnew System::Drawing::Region(e->MarginBounds);
		e->Graphics->Clip = region;
		// 이미지의 사이즈를 구한다.
		System::Drawing::Size^ imageSize = gcnew System::Drawing::Size(width, height);

		if (document->PrintOnCenter == true)
		{
			// 화면 스케일을 적용
			e->Graphics->ScaleTransform(m_fScale, m_fScale);

			// Scale에 따른 크기 변화랑을 Position에 적용한다.
			float dx = (imageSize->Width / m_fScale - imageSize->Width) * 0.5f;
			float dy = (imageSize->Height / m_fScale - imageSize->Height) * 0.5f;

			// 이미지가 중심에 오도록 Scale이 적용된 Image의 크기를 고려하여 Position을 구한다.
			float transX = dx + ((page->Width - imageSize->Width) * 0.5f + page->Location.X) / m_fScale;
			float transY = dy + ((page->Height - imageSize->Height) * 0.5f + page->Location.Y) / m_fScale;

			RectangleF^ imgRect = gcnew RectangleF(transX, transY, (float)imageSize->Width, (float)imageSize->Height);
			e->Graphics->DrawImage(backImage, *imgRect);
			//e->Graphics->TranslateTransform(transX, transY);
		}
		else
		{
			// 화면 스케일을 적용
			e->Graphics->ScaleTransform(m_fScale, m_fScale);

			// Scale에 따른 크기 변화랑을 Position에 적용한다.
			float dx = (offsetX / m_fScale - offsetX) * 0.5f;
			float dy = (offsetY / m_fScale - offsetY) * 0.5f;

			// 이미지가 Offset 위치에 오도록 Scale을 고려하여 Position을 구한다.
			float transX = dx + (offsetX + page->Location.X) / m_fScale;
			float transY = dy + (offsetY + page->Location.Y) / m_fScale;

			RectangleF^ imgRect = gcnew RectangleF(transX, transY, (float)imageSize->Width, (float)imageSize->Height);
			e->Graphics->DrawImage(backImage, *imgRect);

			//e->Graphics->TranslateTransform(transX, transY);			
		}

		//OnPaint(this, gcnew System::Windows::Forms::PaintEventArgs(e->Graphics, *clipRect));
	}

	void DXFControl::OnPrint(System::Object^ sender,System::Windows::Forms::PaintEventArgs^ e)
	{
		e->Graphics->ResetTransform();

		if (m_useAntialiasing)
			e->Graphics->SmoothingMode = System::Drawing::Drawing2D::SmoothingMode::AntiAlias;
		else
			e->Graphics->SmoothingMode = System::Drawing::Drawing2D::SmoothingMode::Default;

		if (m_currentMatrix != nullptr)
			e->Graphics->Transform = m_currentMatrix;

		try
		{
			Vertex2D^ v1 = ScreenToGlobal(0, 0);
			Vertex2D^ v2 = ScreenToGlobal(m_nEditBoxSize, 0);
			m_fEditBoxLength = (float)v1->GetDistance(v2);
			/*for each (Layer^ pLayer in m_arrLayer)
			{
			pLayer->Draw(e->Graphics);
			}*/

			if (m_externPainter != nullptr)
			{
				//m_externPainter->OnPrevPrint(e);
				m_externPainter->OnPrevPrint(e->Graphics);
			}

			if (m_drawHatchFirst)
			{
				for each (Layer^ pLayer in m_arrLayer)
				{
					for each (Layer^ pLayer in m_arrLayer)
					{
						pLayer->DrawHatch(e->Graphics,true);
					}

					for each (Layer^ pLayer in m_arrLayer)
					{
						pLayer->DrawExceptHatch(e->Graphics, true);
					}
				}
			}
			else
			{
				for each (Layer^ pLayer in m_arrLayer)
				{
					pLayer->Draw(e->Graphics, true);
				}
			}

			for each (Layer^ pLayer in m_arrLayer)
			{
				pLayer->DrawGroup(e->Graphics, true);
			}

		}
		catch (System::Runtime::InteropServices::ExternalException^ /*e*/)
		{
			//System::Diagnostics::Trace::WriteLine(e->Message);
		}

		if (m_externPainter != nullptr)
		{
			//m_externPainter->OnPostPrint(e);
			m_externPainter->OnPostPrint(e->Graphics);
		}
	}

	//void DXFControl::PaintGDI(System::Windows::Forms::PaintEventArgs^ e)
	//{
	//	e->Graphics->ResetTransform();

	//	if (m_useAntialiasing)
	//		e->Graphics->SmoothingMode = System::Drawing::Drawing2D::SmoothingMode::AntiAlias;
	//	else
	//		e->Graphics->SmoothingMode = System::Drawing::Drawing2D::SmoothingMode::Default;

	//	if (m_currentMatrix != nullptr)
	//		e->Graphics->Transform = m_currentMatrix->Clone();
	//			
	//	try
	//	{
	//		Vertex2D^ v1 = ScreenToGlobal(0, 0);
	//		Vertex2D^ v2 = ScreenToGlobal(m_nEditBoxSize, 0);
	//		m_fEditBoxLength = (float)v1->GetDistance(v2);
	//		/*for each (Layer^ pLayer in m_arrLayer)
	//		{
	//		pLayer->Draw(e->Graphics);
	//		}*/

	//		if (m_externPainter != nullptr)
	//			m_externPainter->OnPrevPaint(e->Graphics);

	//		if (m_drawHatchFirst)
	//		{
	//			for each (Layer^ pLayer in m_arrLayer)
	//			{
	//				for each (Layer^ pLayer in m_arrLayer)
	//				{
	//					pLayer->DrawHatch(e->Graphics);
	//				}

	//				for each (Layer^ pLayer in m_arrLayer)
	//				{
	//					pLayer->DrawExceptHatch(e->Graphics);
	//				}
	//			}
	//		}
	//		else
	//		{
	//			for each (Layer^ pLayer in m_arrLayer)
	//			{
	//				pLayer->Draw(e->Graphics);
	//			}
	//		}

	//		for each (Layer^ pLayer in m_arrLayer)
	//		{
	//			pLayer->DrawGroup(e->Graphics);
	//		}

	//	}
	//	catch (System::Runtime::InteropServices::ExternalException^ /*e*/)
	//	{
	//		//System::Diagnostics::Trace::WriteLine(e->Message);
	//	}

	//	if (m_externPainter != nullptr)
	//		m_externPainter->OnPostPaint(e->Graphics);
	//}

	void DXFControl::PaintGDI(System::Windows::Forms::PaintEventArgs^ e)
	{
		System::DateTime^ dt = System::DateTime::Now;

		this->BeginPaint::raise(m_img != nullptr);

		e->Graphics->ResetTransform();

		if (m_useAntialiasing)
			e->Graphics->SmoothingMode = System::Drawing::Drawing2D::SmoothingMode::AntiAlias;
		else
			e->Graphics->SmoothingMode = System::Drawing::Drawing2D::SmoothingMode::Default;

		if (m_img == nullptr)
		{
			if (m_currentMatrix != nullptr)
				e->Graphics->Transform = m_currentMatrix;
		}
		

		// Clip영역 지정. Add By skkim 2015.02.25
		// 현재 화면의 TL, BL을 구한다.
		UnE::Geometry::Vertex2D^ v1 = ScreenToGlobal(0, 0);
		UnE::Geometry::Vertex2D^ v2 = ScreenToGlobal(this->Size.Width, this->Size.Height);

		// Global에서의 현재화면의 Rect를 구한다.
		

		float fMaxX = (float)System::Math::Max(v1->x, v2->x);
		float fMinX = (float)System::Math::Min(v1->x, v2->x);
		float fMaxY = (float)System::Math::Max(v1->y, v2->y);
		float fMinY = (float)System::Math::Min(v1->y, v2->y);
		float fWidth = fMaxX - fMinX;
		float fHeight = fMaxY - fMinY;

		System::Drawing::RectangleF^ rect = gcnew System::Drawing::RectangleF(
			 fMinX, fMinY, fWidth, fHeight
			);

		// 현재화면의 Rect를 Clip영역으로 지정한다.
		System::Drawing::Region^ region = gcnew System::Drawing::Region(*rect);

		//m_penEditBox->Color = System::Drawing::Color::White;
		//e->Graphics->DrawRectangle(m_penEditBox,
		//	fMinX + 20, fMinY + 50, fWidth - 20.0f , fHeight - 40.0f);

		try
		{
			Vertex2D^ v1 = ScreenToGlobal(0, 0);
			Vertex2D^ v2 = ScreenToGlobal(m_nEditBoxSize, 0);
			m_fEditBoxLength = (float)v1->GetDistance(v2);
			/*for each (Layer^ pLayer in m_arrLayer)
			{
			pLayer->Draw(e->Graphics);
			}*/

			if (m_img != nullptr)
			{
				System::Drawing::Drawing2D::Matrix^ oldMatrix = e->Graphics->Transform->Clone();
				e->Graphics->DrawImage(m_img, m_nMoveX, m_nMoveY);
				e->Graphics->Transform = oldMatrix;

				if (m_externPainter != nullptr)
				{
					System::Drawing::Drawing2D::Matrix^ oldMatrix2 = e->Graphics->Transform->Clone();
					e->Graphics->Transform = m_currentMatrix;
					m_externPainter->OnOverlayPaint(e->Graphics, m_bDrawText);
					e->Graphics->Transform = oldMatrix2;
				}
			}
			else
			{
				m_img = gcnew System::Drawing::Bitmap(this->Size.Width, this->Size.Height, Imaging::PixelFormat::Format32bppPArgb);

				System::Drawing::Graphics^ g = System::Drawing::Graphics::FromImage(m_img);
				g->Transform = e->Graphics->Transform;
				g->Clear(this->BackColor);

				g->Clip = region;
			
				/*if (m_makeImage)
				{
					m_img = CreateScreenImage();
					PaintGDI(e);
					return;
				}
				else*/
				{
					if (m_externPainter != nullptr)
					{
						//m_externPainter->OnPrevPaint(e);
						m_externPainter->OnPrevPaint(g, m_bDrawText);
					}

					if (m_drawHatchFirst)
					{
						//for each (Layer^ pLayer in m_arrLayer)
						{
							for each (Layer^ pLayer in m_arrLayer)
							{
								//pLayer->DrawHatch(e->Graphics);
								pLayer->DrawHatch(g, m_bDrawText);
							}

							for each (Layer^ pLayer in m_arrLayer)
							{
								//pLayer->DrawExceptHatch(e->Graphics);
								pLayer->DrawExceptHatch(g, m_bDrawText);
							}
						}
					}
					else
					{
						for each (Layer^ pLayer in m_arrLayer)
						{
							//pLayer->Draw(e->Graphics);
							pLayer->Draw(g, m_bDrawText);
						}
					}

					//for each (Layer^ pLayer in m_arrLayer)
					//{
					//	
					//	//pLayer->DrawGroup(e->Graphics);
					//	pLayer->Draw(g, m_bDrawText);
					//}
				}
				
				/*if (m_externPainter != nullptr)
				{
					m_externPainter->OnPostPaint(g, m_bDrawText);
				}*/

				System::Drawing::Drawing2D::Matrix^ oldMatrix2 = e->Graphics->Transform->Clone();
				e->Graphics->ResetTransform();
				e->Graphics->DrawImage(m_img, 0, 0);

				e->Graphics->Transform = oldMatrix2;
				
				if (m_externPainter != nullptr)
				{
					m_externPainter->OnPostPaint(e->Graphics, m_bDrawText);
				}
			}
		}
		catch (System::Runtime::InteropServices::ExternalException^/* e*/)
		{
			//System::Diagnostics::Trace::WriteLine(e->Message);
			//System::Diagnostics::Trace::WriteLine(e->StackTrace);
		}
		catch (System::OutOfMemoryException^)
		{
			m_img = nullptr;
			return;
		}
		catch (System::OverflowException^ ex)
		{
			m_img = nullptr;
			System::Diagnostics::Trace::WriteLine(ex->Message);
			System::Diagnostics::Trace::WriteLine(ex->StackTrace);
			return;
		}
		catch (System::Exception^)
		{
			m_img = nullptr;
			return;
		}
		
		
		this->EndPaint::raise(m_img != nullptr);

		System::DateTime^ dt2 = System::DateTime::Now;
		mDrawSpan = *dt2 - *dt;
	}


	void DXFControl::PaintGL(System::Windows::Forms::PaintEventArgs^ e)
	{
		//if (m_externPainter != nullptr)
		//{
		//	//m_externPainter->OnPrevPaint(e);
		//	m_externPainter->OnPrevPaint(e->Graphics, true);
		//}

		//HDC hdc = ((DIBSection*)m_pDIB)->GetDC();
		////System::Drawing::Graphics^ g;
		////HDC hdc = GetHDC(m_bitmap, g);

		//wglMakeCurrent(hdc, (HGLRC)m_nRC);

		//glClearColor(this->BackColor.R / 255.0f, this->BackColor.G / 255.0f, this->BackColor.B / 255.0f, 1.0f);

		//if (m_useAntialiasing)
		//{
		//	glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
		//	glEnable(GL_BLEND);
		//	glEnable(GL_POINT_SMOOTH);
		//	glHint(GL_POINT_SMOOTH_HINT, GL_NICEST);
		//	glEnable(GL_LINE_SMOOTH);
		//	glHint(GL_LINE_SMOOTH_HINT, GL_NICEST);
		//	glEnable(GL_POLYGON_SMOOTH);
		//	glHint(GL_POLYGON_SMOOTH_HINT, GL_NICEST);
		//}
		//else
		//{
		//	glDisable(GL_BLEND);
		//	glDisable(GL_LINE_SMOOTH);
		//	glDisable(GL_POINT_SMOOTH);
		//	glDisable(GL_POLYGON_SMOOTH);
		//}

		//Display((System::IntPtr)hdc, (System::IntPtr)((DIBSection*)m_pDIB)->GetHandle(), e->Graphics);
		////SwapBuffers(hdc);
		//wglMakeCurrent(hdc, NULL);

		//if (m_externPainter != nullptr)
		//{
		//	//m_externPainter->OnPostPaint(e);
		//	m_externPainter->OnPostPaint(e->Graphics, true);
		//}
	}

	void DXFControl::Display(System::IntPtr nHDC, System::IntPtr nHBitmap, System::Drawing::Graphics^ gPaint)
	{
		//RenderImage(nHDC);
		//glFlush();
		//DrawImage(nHBitmap, gPaint);
	}

	void DXFControl::RenderImage(System::IntPtr nHDC)
	{
		/*glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
		glEnable(GL_LINE_STIPPLE);

		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

		glEnableClientState(GL_VERTEX_ARRAY);

		glPushMatrix();
		
		HDC hdc = (HDC)nHDC.ToInt32();

		if (hdc == 0)
			m_hdcCurrent = System::IntPtr::Zero;
		else
			m_hdcCurrent = nHDC;

		if (m_drawHatchFirst)
		{
			for each (Layer^ pLayer in m_arrLayer)
			{
				for each (Layer^ pLayer in m_arrLayer)
				{
					pLayer->DrawHatch(nullptr, true);
				}

				for each (Layer^ pLayer in m_arrLayer)
				{
					pLayer->DrawExceptHatch(nullptr, true);
				}
			}
		}
		else
		{
			for each (Layer^ pLayer in m_arrLayer)
			{
				pLayer->Draw(nullptr, true);
			}
		}

		for each (Layer^ pLayer in m_arrLayer)
		{
			pLayer->DrawGroup(nullptr, true);
		}

		glPopMatrix();

		glDisable(GL_BLEND);*/
	}

	int m_nCount = 0;

	void DXFControl::DrawImage(System::IntPtr nHBitmap, System::Drawing::Graphics^ gPaint)
	{
		System::Drawing::Bitmap^ bitmap = System::Drawing::Bitmap::FromHbitmap(nHBitmap);
		//gBitmap->ReleaseHdc();
		gPaint->DrawImage(bitmap, System::Drawing::Point(0, 0));
		//gBitmap->ReleaseHdc();
	}

	void DXFControl::OnPaint(System::Object^ sender, System::Windows::Forms::PaintEventArgs^ e)
	{
		//if (m_renderType == IPainter::RendererType::OPEN_GL)
		//	PaintGL(e);
		//else if (m_renderType == IPainter::RendererType::GDI_PLUS)
			PaintGDI(e);
	}

	Vertex2D^ DXFControl::ScreenToGlobal(int x, int y)
	{
		Vertex2D^ vResult = nullptr;

		if (m_renderType == IPainter::RendererType::GDI_PLUS)
		{
			if (m_currentInverseMatrix == nullptr)
				return nullptr;

			vResult = gcnew Vertex2D();
			vResult->x = m_currentInverseMatrix->Elements[0] * x + m_currentInverseMatrix->Elements[2] * y + m_currentInverseMatrix->Elements[4];
			vResult->y = m_currentInverseMatrix->Elements[1] * x + m_currentInverseMatrix->Elements[3] * y + m_currentInverseMatrix->Elements[5];
		}
		else if (m_renderType == IPainter::RendererType::OPEN_GL)
		{
			vResult = m_vViewportBL + (m_vViewportBR - m_vViewportBL) * x / this->Size.Width;
			vResult = vResult + (m_vViewportTL - m_vViewportBL) * (this->Size.Height - y) / this->Size.Height;
		}

		return vResult;
	}

	System::Drawing::Point DXFControl::GlobalToScreen(Vertex2D^ vertex)
	{
		System::Drawing::Point ptResult;

		if (m_renderType == IPainter::RendererType::GDI_PLUS)
		{
			if (m_currentMatrix == nullptr)
				return ptResult;

			ptResult.X = (int)(m_currentMatrix->Elements[0] * vertex->x + m_currentMatrix->Elements[2] * vertex->y + m_currentMatrix->Elements[4]);
			ptResult.Y = (int)(m_currentMatrix->Elements[1] * vertex->x + m_currentMatrix->Elements[3] * vertex->y + m_currentMatrix->Elements[5]);
		}
		else if (m_renderType == IPainter::RendererType::OPEN_GL)
		{
			double dLenHorz = m_vViewportBL->GetDistance(m_vViewportBR);
			double dLenVert = m_vViewportBL->GetDistance(m_vViewportTL);

			Vertex2D^ vTR = m_vViewportTL + m_vViewportBR - m_vViewportBL;

			Line2D^ lineLeft = gcnew UnE::Geometry::Line2D(m_vViewportTL, m_vViewportBL);
			Line2D^ lineTop = gcnew UnE::Geometry::Line2D(vTR, m_vViewportTL);

			double w = lineLeft->GetDistance(vertex, true);
			double h = lineTop->GetDistance(vertex, true);

			if (UnE::Geometry::Math::IsRightSideFromLine(vertex, m_vViewportTL, m_vViewportBL) < 0)
				w = -w;

			if (UnE::Geometry::Math::IsRightSideFromLine(vertex, vTR, m_vViewportTL) < 0)
				h = -h;

			ptResult.X = (int)(this->Size.Width * w / dLenHorz);
			ptResult.Y = (int)(this->Size.Height * h / dLenVert);
		}

		return ptResult;
	}

	void DXFControl::OnMouseDown(System::Object^ sender, System::Windows::Forms::MouseEventArgs^ e)
	{
		if (e->Button == m_btnPanning)
		{
			m_isPanning = true;
			m_ptPanningOrigin.X = e->X;
			m_ptPanningOrigin.Y = e->Y;
			m_nMoveX = m_nMoveY = 0;
			m_vOriginCenter = GetViewportCenter();
		}
	}

	void DXFControl::OnMouseUp(System::Object^ sender, System::Windows::Forms::MouseEventArgs^ e)
	{
		if (e->Button == m_btnPanning)
		{
			m_isPanning = false;

			// 화면이 이동되었으니 ScreenImage를 새로 만든다.
			m_nMoveX = m_nMoveY = 0;
			_Refresh();
		}
	}

	void DXFControl::OnMouseMove(System::Object^ sender, System::Windows::Forms::MouseEventArgs^ e)
	{
		if (m_isPanning)
		{
			m_nMoveX = e->X - m_ptPanningOrigin.X;
			m_nMoveY = e->Y - m_ptPanningOrigin.Y;

			Vertex2D^ vNewCenter = nullptr;
			
			vNewCenter = gcnew Vertex2D(m_vOriginCenter->x - m_nMoveX / m_dViewportWeight, m_vOriginCenter->y + m_nMoveY / m_dViewportWeight);
			//if (m_renderType == IPainter::RendererType::GDI_PLUS)
			//	vNewCenter = gcnew Vertex2D(m_vOriginCenter->x - m_nMoveX/* * m_dViewportWeight*/, m_vOriginCenter->y - m_nMoveY/* * m_dViewportWeight*/);
			//else if (m_renderType == IPainter::RendererType::OPEN_GL)
			//	vNewCenter = gcnew Vertex2D(m_vOriginCenter->x - m_nMoveX * m_dViewportWeight, m_vOriginCenter->y + m_nMoveY * m_dViewportWeight);

			SetViewportCenter(vNewCenter);

			Refresh();
		}
	}

	void DXFControl::OnMouseWheel(System::Object^ sender, System::Windows::Forms::MouseEventArgs^ e)
	{
		if (m_isPanning == true)
			return;

		if (!m_useMouseWheel)
			return;

		Vertex2D^ vCurrent = ScreenToGlobal(e->X, e->Y);
		if (vCurrent == nullptr)
			return;

		/*m_bProcessWheel = true;

		m_dtLastMouseWheel = System::DateTime::Now;

		
		timerMouseWheel->Enabled = true;
		timerMouseWheel->Start();*/
		

		m_img = nullptr;
		//m_makeImage = false;

		double dZoomValue = m_dViewportWeight;

		if (m_renderType == IPainter::RendererType::GDI_PLUS)
		{
			if (e->Delta < 0)
			{
				dZoomValue *= 0.9;
				if (dZoomValue < 0.0001)
					dZoomValue = 0.0001;
			}
			else
				dZoomValue /= 0.9;
		}
		else if (m_renderType == IPainter::RendererType::OPEN_GL)
		{
			if (e->Delta > 0)
			{
				dZoomValue *= 0.9;
				if (dZoomValue < 0.0001)
					dZoomValue = 0.0001;
			}
			else
				dZoomValue /= 0.9;
		}

		/*if (mDrawSpan != nullptr && mDrawSpan->TotalMilliseconds > 150)
		{

			if (m_WheelTimer == nullptr || m_WheelTimer->Enabled == false)
			{
				if (m_WheelTimer == nullptr)
				{
					m_WheelTimer = gcnew System::Windows::Forms::Timer();
					m_WheelTimer->Interval = 600;
					m_WheelTimer->Tick += gcnew System::EventHandler(this, &DXFControl::OnWheelTimerTick);
				}
				m_WheelTimer->Enabled = true;
				m_WheelTimer->Start();
			}

			m_bDrawText = false;

			Zoom(dZoomValue, vCurrent, false);
		}
		else*/
		{			
			Zoom(dZoomValue, vCurrent, true);
		}
		
		//m_bProcessWheel = false;
	}

	void DXFControl::OnWheelTimerTick(System::Object^ sender, System::EventArgs^ e)
	{
		if (m_bProcessWheel == false)
		{
			m_bDrawText = true;
			Refresh();
		}
		m_WheelTimer->Enabled = false;
		m_WheelTimer->Stop();
	}

	void DXFControl::timerMouseWheel_Tick(System::Object^ sender, System::EventArgs^ e)
	{
		using namespace System;
				
		if (!m_isOpened || !m_useGroupItem)
		{
			timerMouseWheel->Stop();
			//m_makeImage = true;
			//m_bProcessWheel = false;
			return;
		}

		DateTime dtNow = DateTime::Now;
		TimeSpan span = dtNow - m_dtLastMouseWheel;
		if (span.TotalMilliseconds > 300)
		{
			timerMouseWheel->Stop();
			//m_makeImage = true;

			for each (Layer^ layer in m_arrLayer)
			{
				layer->CalcGroup(m_nGroupItemMinCount, m_nGroupItemDistance);
			}			
			timerMouseWheel->Enabled = false;
		}
	}

	void DXFControl::Zoom(double dZoomValue, Vertex2D^ vZoomCenter, bool refresh)
	{
	
		//System::Diagnostics::Debug::WriteLine(dZoomValue);

		// 이 이상 넘어가면... 죽는다.
		if (dZoomValue > 7.0 || dZoomValue <= UnE::Geometry::Math::HALF_TOLERANCE())
			return;

		// vZoomCenter에 해당하는 화면좌표1 얻어오기
		System::Drawing::Point ptZoomCenter = GlobalToScreen(vZoomCenter);

		double left = ptZoomCenter.X / dZoomValue;
		double top = ptZoomCenter.Y / dZoomValue;
		double right = (ptZoomCenter.X - this->Size.Width) / dZoomValue;
		double bottom = (ptZoomCenter.Y - this->Size.Height) / dZoomValue;

		m_vViewportTL->x = vZoomCenter->x - left;
		m_vViewportTL->y = vZoomCenter->y + top;
		m_vViewportBL->x = vZoomCenter->x - left;
		m_vViewportBL->y = vZoomCenter->y + bottom;
		m_vViewportBR->x = vZoomCenter->x - right;
		m_vViewportBR->y = vZoomCenter->y + bottom;

		m_dViewportWeight = dZoomValue;

		Reshape(this->Size.Width, this->Size.Height);

		/*// m_vViewportTL을 기준으로 Zoom
		m_dViewportWeight = dZoomValue;
		Reshape(this->Size.Width, this->Size.Height);

		// Zoom 시킨 이후에 ZoomCenter에 해당하는 화면좌표2 얻어오기
		Point pt2 = GlobalToScreen(vZoomCenter);

		// 화면좌표2와 화면좌표1 간의 이동거리 얻기
		int nMoveX = pt2.X - ptZoomCenter.X;
		int nMoveY = pt2.Y - ptZoomCenter.Y;

		// vZoomCenter를 화면상에 고정시키기 위하여 Viewport 관련 좌표들 이동
		m_vViewportTL->x += nMoveX;// * m_dViewportWeight;
		m_vViewportTL->y += nMoveY;// * m_dViewportWeight;
		m_vViewportBL->x += nMoveX;// * m_dViewportWeight;
		m_vViewportBL->y += nMoveY;// * m_dViewportWeight;
		m_vViewportBR->x += nMoveX;// * m_dViewportWeight;
		m_vViewportBR->y += nMoveY;// * m_dViewportWeight;

		// Matrix 재구성
		Reshape(this->Size.Width, this->Size.Height);*/

		/*m_vViewportTL = vZoomCenter + (m_vViewportTL - vZoomCenter) * dZoomValue / m_dViewportWeight;
		m_vViewportBL = vZoomCenter + (m_vViewportBL - vZoomCenter) * dZoomValue / m_dViewportWeight;
		m_vViewportBR = vZoomCenter + (m_vViewportBR - vZoomCenter) * dZoomValue / m_dViewportWeight;

		m_dViewportWeight = dZoomValue;

		Reshape(this->Size.Width, this->Size.Height);*/

		if (refresh)
		{
			Refresh();
			//System::Diagnostics::Trace::WriteLine(L"Zoom Refresh");
		}
	}
	void DXFControl::Invalidate(bool)
	{
		//System::DateTime^ dt = System::DateTime::Now;

		__super::Invalidate(false);

		//System::DateTime^ dt2 = System::DateTime::Now;

		//System::TimeSpan^ span = *dt2 - *dt;
		//System::Diagnostics::Trace::WriteLine("REDRAW : " + span->TotalMilliseconds);
	}

	void DXFControl::Refresh()
	{
		__super::Refresh();// Invalidate(true);
		//RefreshEvent(true);	
	}

	void DXFControl::_Refresh()
	{
		m_img = nullptr;
		//m_makeImage = true;
		//Reshape(this->Size.Width, this->Size.Height);
		SetViewportCenter(this->GetViewportCenter());
		Refresh();
	}

	// Y축이 화면 아래에서 위쪽으로 증가하는 방향인가?
	bool DXFControl::DownToTop()
	{
		return true;
	}

	Shape^ DXFControl::SelectObject(double x, double y)
	{
		for each (Layer^ pLayer in m_arrLayer)
		{
			if (pLayer->Hidden)
				continue;

			Shape^ shape = pLayer->SelectObject(x, y);
			if (shape != nullptr)
				return shape;
		}

		return nullptr;
	}

	Shape^ DXFControl::PickObject(double x, double y)
	{
		Shape^ shape = SelectObject(x, y);

		if (shape != nullptr)
			shape->Selected = true;

		return shape;
	}

	void DXFControl::PickObject(Shape^ shape)
	{
		if (shape == nullptr)
			return;

		shape->Selected = true;
	}

	// 모든 객체들을 현재의 위치로부터 (x, y) 만큼 이동시킨다.
	void DXFControl::MoveAll(double x, double y)
	{
		for each (Layer^ pLayer in m_arrLayer)
		{
			pLayer->MoveAll(x, y);
		}

		m_vMove->x = x;
		m_vMove->y = y;
	}

	void DXFControl::SetEditBoxColor(System::Drawing::Color color, bool isFill)
	{
		if (isFill)
			m_brushEditBox->Color = color;
		else
			m_penEditBox->Color = color;
	}

	System::Drawing::Color DXFControl::GetColor(bool isFill)
	{
		return isFill ? m_brushEditBox->Color : m_penEditBox->Color;
	}

	void DXFControl::SetEditBoxSize(int nLen)
	{
		m_nEditBoxSize = nLen;
	}

	int DXFControl::GetEditBoxSize()
	{
		return m_nEditBoxSize;
	}

	

	void DXFControl::CalcShapeGroup()
	{
		for each (Layer^ layer in m_arrLayer)
		{
			layer->CalcGroup(m_nGroupItemMinCount, m_nGroupItemDistance);
		}
	}

	System::Drawing::Color DXFControl::GetBackColor()
	{
		return this->BackColor;
	}

	// LineType별 Pen
	// Key : Line Style(상위 4바이트) + Line Width(하위 4바이트)
	System::Collections::Generic::Dictionary<__int64, System::Drawing::Pen^>^ DXFControl::GetLineTypePen()
	{
		return m_dicPens;
	}

	void DXFControl::DeleteDIB()
	{
		//delete (DIBSection*)m_pDIB;
		//m_pDIB = 0;
	}

	System::Drawing::Bitmap^ DXFControl::GetCurrentBitmap()
	{
		return nullptr;
		//return System::Drawing::Bitmap::FromHbitmap((System::IntPtr)((DIBSection*)m_pDIB)->GetHandle());
	}

	void Layer::DrawObjectGL(Shape^ obj)
	{
		/*glPushMatrix();
		UnE::Geometry::Vertex2D^ vOrigin = obj->GetBlock()->OriginVertex;
		glTranslatef((float)vOrigin->x, (float)vOrigin->y, 0.0f);

		obj->Draw(nullptr, true);
		
		glTranslatef((float)-vOrigin->x, (float)-vOrigin->y, 0.0f);
		glPopMatrix();*/
	}

	bool Line::DrawGL()
	{
		/*System::Drawing::Color color = GetColor();
		
		glColor3f(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);
		glLineWidth(m_lineType->GetLineWidth());

		glVertexPointer(2, GL_FLOAT, 0, m_arrPointGL);
		glDrawArrays(GL_LINES, 0, 2);*/

		return true;
	}

	bool PolyLine::DrawGL()
	{
		/*System::Drawing::Color color = GetColor();

		glColor3f(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);
		glLineWidth(m_lineType->GetLineWidth());

		glVertexPointer(2, GL_FLOAT, 0, m_arrPointGL);
		glDrawArrays(GL_LINES, 0, m_arrPoint->Length);*/

		return true;
	}

	bool Text::DrawGL()
	{
		/*if (m_pOwner == nullptr)
			return false;

		if (m_strTextGL == 0)
			return true;

		DXFControl^ ctrl = (DXFControl^)m_pOwner;
		HDC hdc = (HDC)ctrl->CurrentHDC.ToInt32();

		int nEscapement = (int)(m_dTextAngle * 10);
		HFONT hFont = CreateFont((int)m_font->Size, 0, nEscapement,
			0, 0, 0, 0, 0, HANGUL_CHARSET, 3, 2, 1, VARIABLE_PITCH | FF_ROMAN, m_strFontName);
		HGDIOBJ hOldFont = SelectObject(hdc, hFont);

		System::Drawing::Color color = GetColor();

		SetTextColor(hdc, RGB(color.R, color.G, color.B));
		SetBkMode(hdc, TRANSPARENT);
		TextOut(hdc, (int)m_ptPos.X, (int)m_ptPos.Y, m_strTextGL, m_strText->Length);

		SelectObject(hdc, hOldFont);
		DeleteObject(hFont);*/

		return true;
	}

	bool ShapeGroup::DrawGL()
	{
		/*if (m_drawType == DrawType::IMAGE)
		{
			if (m_img != nullptr)
			{
				if (m_pOwner == nullptr)
					return false;

				DXFControl^ ctrl = (DXFControl^)m_pOwner;
				System::Drawing::Graphics^ g = System::Drawing::Graphics::FromHdc(ctrl->CurrentHDC);

				float fWidth = 0, fHeight = 0;
				GetImageSize(fWidth, fHeight);

				g->DrawImage(m_img, (float)m_vPos->x, (float)m_vPos->y, fWidth, fHeight);

				if (Selectable && Selected)
					m_editBox->Draw(g, (float)(m_vPos->x + fWidth / 2), (float)(m_vPos->y + fHeight / 2));

				return true;
			}
		}
		else if (m_drawType == DrawType::SHAPE)
		{
			if (m_shape != nullptr)
			{
				m_shape->Selectable = this->Selectable;
				m_shape->Selected = this->Selected;
				m_shape->Draw(nullptr, true );

				return true;
			}
		}*/

		return false;
	}

	bool Hatch::DrawGL()
	{
		/*if (m_arrPointGL == 0 || m_arrIndex == 0 || m_nIndexCount == 0)
			return false;

		System::Drawing::Color color = GetColor();

		glColor3f(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);
		glPolygonMode(GL_BACK, GL_FILL);

		glVertexPointer(2, GL_FLOAT, 0, m_arrPointGL);
		glDrawElements(GL_TRIANGLES, m_nIndexCount, GL_UNSIGNED_INT, m_arrIndex);
		glPolygonMode(GL_BACK, GL_LINE);*/

		return true;
	}

	bool EArc::DrawGL()
	{
		return true;
	}

	bool Arc::DrawGL()
	{
		return true;
	}
}
