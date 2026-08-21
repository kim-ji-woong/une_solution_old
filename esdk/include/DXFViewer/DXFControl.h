#pragma once
#include "IPainter.h"

namespace DXFViewer {

	ref class Layer;
	ref class Block;
	ref class Shape;
	ref class LineType;
	ref class Viewport;
	ref class PlotSettings;
	ref class UPrintDocument;

	/// <summary>
	/// DXFControl에 대한 요약입니다.
	/// </summary>
	public ref class DXFControl : public System::Windows::Forms::UserControl, IPainter
	{
	public:
		delegate void BeginPaintEventHandler(bool);
		delegate void EndPaintEventHandler(bool);
		delegate void RefreshEventHandler(bool);

		delegate void BeginReadFileHandler(System::String^ szPath, System::String^ szType, int nEntity);
		delegate void ReadEntityHandler(System::String^ szEntityName, int nCount);
		delegate void EndReadFileHandler(System::String^ szPath, System::String^ szType);

	public:
		DXFControl(void)
		{
			InitializeComponent();
			//
			//TODO: 생성자 코드를 여기에 추가합니다.
			//
			Init();
		}

	protected:
		/// <summary>
		/// 사용 중인 모든 리소스를 정리합니다.
		/// </summary>
		~DXFControl()
		{
			if (components)
			{
				delete components;
			}

			DeleteDIB();
		}

		void SetDoubleBuffered(bool bEnabled);

		BeginPaintEventHandler^ PaintBegin;
		EndPaintEventHandler^ PaintEnd;
		RefreshEventHandler^ Repaint;

		BeginReadFileHandler^ ReadBegin;
		ReadEntityHandler^ ReadOne;
		EndReadFileHandler^ ReadEnd;

		bool m_bDrawText;
	public:

		void SendBeginRead(System::String^ szPath, System::String^ szType, int nEntity)
		{
			BeginRead::raise(szPath, szType, nEntity);
		}

		void SendEndRead(System::String^ szPath, System::String^ szType)
		{
			EndRead::raise(szPath, szType);
		}

		void SendReadEntity(System::String^ szPath, int nEntity)
		{
			ReadEntity::raise(szPath, nEntity);
		}

		event BeginReadFileHandler^ BeginRead
		{
			void add(BeginReadFileHandler^ paint)
			{
				ReadBegin = static_cast<BeginReadFileHandler^>(System::Delegate::Combine(ReadBegin, paint));
			}

			void remove(BeginReadFileHandler^ paint)
			{
				ReadBegin = static_cast<BeginReadFileHandler^>(System::Delegate::Remove(ReadBegin, paint));
			}

			void raise(System::String^ szPath, System::String^ szType, int nEntity)
			{
				if (ReadBegin != nullptr)
					ReadBegin->Invoke(szPath, szType, nEntity);
			}
		}

		event ReadEntityHandler^ ReadEntity
		{
			void add(ReadEntityHandler^ paint)
			{
				ReadOne = static_cast<ReadEntityHandler^>(System::Delegate::Combine(ReadOne, paint));
			}

			void remove(ReadEntityHandler^ paint)
			{
				ReadOne = static_cast<ReadEntityHandler^>(System::Delegate::Remove(ReadOne, paint));
			}

			void raise(System::String^ szPath, int nEntity)
			{
				if (ReadOne != nullptr)
					ReadOne->Invoke(szPath, nEntity);
			}
		}

		event EndReadFileHandler^ EndRead
		{
			void add(EndReadFileHandler^ paint)
			{
				ReadEnd = static_cast<EndReadFileHandler^>(System::Delegate::Combine(ReadEnd, paint));
			}

			void remove(EndReadFileHandler^ paint)
			{
				ReadEnd = static_cast<EndReadFileHandler^>(System::Delegate::Remove(ReadEnd, paint));
			}

			void raise(System::String^ szPath, System::String^ szType)
			{
				if (ReadEnd != nullptr)
					ReadEnd->Invoke(szPath, szType);
			}
		}

		event RefreshEventHandler^ RefreshEvent
		{
			void add(RefreshEventHandler^ paint)
			{
				Repaint = static_cast<RefreshEventHandler^>(System::Delegate::Combine(Repaint, paint));
			}

			void remove(RefreshEventHandler^ paint)
			{
				Repaint = static_cast<RefreshEventHandler^>(System::Delegate::Remove(Repaint, paint));
			}

			void raise(bool b)
			{
				if (Repaint != nullptr)
					Repaint->Invoke(b);
			}
		}

		event BeginPaintEventHandler^ BeginPaint
		{
			void add(BeginPaintEventHandler^ paint)
			{
				PaintBegin = static_cast<BeginPaintEventHandler^>(System::Delegate::Combine(PaintBegin, paint));
			}

			void remove(BeginPaintEventHandler^ paint)
			{
				PaintBegin = static_cast<BeginPaintEventHandler^>(System::Delegate::Remove(PaintBegin, paint));
			}

			void raise(bool b)
			{
				if (PaintBegin != nullptr)
					PaintBegin->Invoke(b);
			}
		}

		event EndPaintEventHandler^ EndPaint
		{
			void add(EndPaintEventHandler^ paint)
			{
				PaintEnd = static_cast<EndPaintEventHandler^>(System::Delegate::Combine(PaintEnd, paint));
			}

			void remove(EndPaintEventHandler^ paint)
			{
				PaintEnd = static_cast<EndPaintEventHandler^>(System::Delegate::Remove(PaintEnd, paint));
			}

			void raise(bool b)
			{
				if (PaintEnd != nullptr)
					PaintEnd->Invoke(b);
			}
		}

		bool OpenDXF(System::String^ strPath);
		void CloseDXF();

		virtual void SetCurrentLayer(Layer^ layer);
		virtual Layer^ GetCurrentLayer();

		virtual void SetCurrentBlock(Block^ block);
		virtual Block^ GetCurrentBlock();

		virtual void SetViewportCenter(UnE::Geometry::Vertex2D^ vCenter);
		virtual UnE::Geometry::Vertex2D^ GetViewportCenter();
		virtual double GetViewportWeight();
		void SetViewportWeight(double dWeight);
		virtual int GetScreenWidth();
		virtual int GetScreenHeight();

		virtual LineType^ GetSelectedLineType();

		virtual void Zoom(double dZoomValue, UnE::Geometry::Vertex2D^ vZoomCenter, bool refresh);

		// Y축이 화면 아래에서 위쪽으로 증가하는 방향인가?
		virtual bool DownToTop();

		virtual void _Refresh();

		virtual UnE::Geometry::Vertex2D^ ScreenToGlobal(int x, int y);
		virtual System::Drawing::Point GlobalToScreen(UnE::Geometry::Vertex2D^ vertex);

		virtual System::Drawing::Color GetBackColor();

		virtual property float EditBoxLength
		{
			float get() { return m_fEditBoxLength; }
		}

		virtual property System::Drawing::SolidBrush^ EditBoxBrush
		{
			System::Drawing::SolidBrush^ get() { return m_brushEditBox; }
		}

		virtual property System::Drawing::Pen^ EditBoxPen
		{
			System::Drawing::Pen^ get() { return m_penEditBox; }
		}

		virtual property System::Drawing::Color BackColor
		{
			System::Drawing::Color get() override { return System::Windows::Forms::UserControl::BackColor; }
			void set(System::Drawing::Color value) override
			{
				System::Windows::Forms::UserControl::BackColor = value;

				if (m_penSelectedBright2 != nullptr)
				{
					m_penSelectedBright2->Color = value;
				}

				if (m_penSelectedBright1 != nullptr)
				{
					m_penSelectedBright1->Color = System::Drawing::Color::FromArgb(255 - value.R, 255 - value.G, 255 - value.B);
				}
			}
		}

		virtual property System::Drawing::Pen^ SelectedBrightPen1
		{
			System::Drawing::Pen^ get()
			{
				if (m_penSelectedBright1 == nullptr)
				{
					m_penSelectedBright1 = gcnew System::Drawing::Pen(System::Drawing::Color::FromArgb(255 - this->BackColor.R, 255 - this->BackColor.G, 255 - this->BackColor.B));
				}

				return m_penSelectedBright1;
			}
		}

		virtual property System::Drawing::Pen^ SelectedBrightPen2
		{
			System::Drawing::Pen^ get()
			{
				if (m_penSelectedBright2 == nullptr)
				{
					m_penSelectedBright2 = gcnew System::Drawing::Pen(this->BackColor);
				}

				return m_penSelectedBright2;
			}
		}

		virtual property IPainter::RendererType Renderer
		{
			IPainter::RendererType get() { return m_renderType; }
			void set(IPainter::RendererType value) { m_renderType = value; }
		}

	public:
		System::TimeSpan^ mDrawSpan;
		virtual void Refresh() override;
		void Invalidate(bool);
		void InitSize();
		Shape^ SelectObject(double x, double y);
		Shape^ PickObject(double x, double y);
		void PickObject(Shape^ shape);
		// 모든 객체들을 현재의 위치로부터 (x, y) 만큼 이동시킨다.
		void MoveAll(double x, double y);

		// EditBox
		void SetEditBoxColor(System::Drawing::Color color, bool isFill);
		System::Drawing::Color GetColor(bool isFill);
		void SetEditBoxSize(int nLen);
		int GetEditBoxSize();

		void SaveHomeMatrix();
		void LoadHomeMatrix(bool refresh);
		Viewport^ GetViewport();
		void LoadViewport(Viewport^ viewport, bool refresh);

		void CalcShapeGroup();

		void OnMouseWheel(System::Object^ sender, System::Windows::Forms::MouseEventArgs^ e);
		virtual property bool SetExternalWheelEvent
		{
			void set(bool value)
			{
				if (value == m_bExternalWheel)
					return;

				if (value == true)
				{
					m_bExternalWheel = true;
					this->MouseWheel -= gcnew System::Windows::Forms::MouseEventHandler(this, &DXFControl::OnMouseWheel);
				}
				else
				{
					m_bExternalWheel = false;
					this->MouseWheel -= gcnew System::Windows::Forms::MouseEventHandler(this, &DXFControl::OnMouseWheel);
					this->MouseWheel += gcnew System::Windows::Forms::MouseEventHandler(this, &DXFControl::OnMouseWheel);
				}
			}
		}

	protected:
		bool m_bExternalWheel;
		void OnLoad(System::Object^ sender, System::EventArgs^ e);
		void OnPaint(System::Object^ sender, System::Windows::Forms::PaintEventArgs^ e);
		void OnSize(System::Object^ sender, System::EventArgs^ e);
		void OnMouseDown(System::Object^ sender, System::Windows::Forms::MouseEventArgs^ e);
		void OnMouseUp(System::Object^ sender, System::Windows::Forms::MouseEventArgs^ e);
		void OnMouseMove(System::Object^ sender, System::Windows::Forms::MouseEventArgs^ e);
		
		void timerMouseWheel_Tick(System::Object^ sender, System::EventArgs^ e);
		void OnWheelTimerTick(System::Object^ sender, System::EventArgs^ e);
		void Init();
		void Reshape(int nWidth, int nHeight);
		void ReshapeGDI(int nWidth, int nHeight);
		void PaintGDI(System::Windows::Forms::PaintEventArgs^ e);

		void OnPrintPage(System::Object^ sender, System::Drawing::Printing::PrintPageEventArgs^ e);
		void OnPrint(System::Object^ sender, System::Windows::Forms::PaintEventArgs^ e);
		void CreateImage(int nWidth, int nHeight);
		//System::Drawing::Image^ CreateScreenImage();

#pragma region OpenGL 함수들
		//bool SetImagePixelFormat(HDC hDC);
		//bool CreateImageGLContext(HDC hDC);
		void InitRC();
		void SizeImage(int width, int height);
		void ReshapeGL(int nWidth, int nHeight);
		void LoadOrtho();
		void PaintGL(System::Windows::Forms::PaintEventArgs^ e);
		void Display(System::IntPtr nHDC, System::IntPtr nHBitmap, System::Drawing::Graphics^ gPaint);
		void RenderImage(System::IntPtr nHDC);
		void DrawImage(System::IntPtr nHitmap, System::Drawing::Graphics^ gPaint);
		void DeleteDIB();
#pragma endregion

	public:
		property System::Windows::Forms::MouseButtons PanningMouseButton
		{
			System::Windows::Forms::MouseButtons get() { return m_btnPanning; }
			void set(System::Windows::Forms::MouseButtons value) { m_btnPanning = value; }
		}

		// MouseWheel을 이용하는 Zoom 기능을 사용할 것인가?
		property bool UseMouseWheel
		{
			bool get() { return m_useMouseWheel; }
			void set(bool value) { m_useMouseWheel = value; }
		}

		property System::Collections::ArrayList^ Layers
		{
			System::Collections::ArrayList^ get() { return m_arrLayer; }
		}

		property System::Collections::ArrayList^ Blocks
		{
			System::Collections::ArrayList^ get() { return m_arrBlock; }
		}

		property DXFViewer::UnitOfLength UnitOfLength
		{
			DXFViewer::UnitOfLength get() { return m_unitOfLength; }
			void set(DXFViewer::UnitOfLength value) { m_unitOfLength = value; }
		}

		property UnE::Geometry::Vertex2D^ MovedVertex
		{
			UnE::Geometry::Vertex2D^ get() { return m_vMove; }
			void set(UnE::Geometry::Vertex2D^ value) { m_vMove = value; }
		}

		property bool IsOpened
		{
			bool get() { return m_isOpened; }
		}

		property int GroupItemDistance
		{
			int get() { return m_nGroupItemDistance; }
			void set(int value) { m_nGroupItemDistance = value; }
		}

		property bool UseGroupItem
		{
			bool get() { return m_useGroupItem; }
			void set(bool value) { m_useGroupItem = value;}
		}

		// Group을 만들기 위해 필요한 최소 Item 개수
		property int GroupItemMinCount
		{
			int get() { return m_nGroupItemMinCount; }
			void set(int value) { m_nGroupItemMinCount = value; }
		}

		property bool Panning
		{
			bool get() { return m_isPanning; }
			void set(bool value) { m_isPanning = value; }
		}

		// 다른 Shape보다 Hatch를 먼저 그릴 것인가?
		property bool DrawHatchFirst
		{
			bool get() { return m_drawHatchFirst; }
			void set(bool value) { m_drawHatchFirst = value; }
		}

		// 도면을 열때 AutoCAD에서 마지막으로 기억된 Viewport를 사용할 것인가?
		property bool UseLastViewport
		{
			bool get() { return m_useLastViewport; }
			void set(bool value) { m_useLastViewport = value; }
		}

		property bool AntiAliasing
		{
			bool get() { return m_useAntialiasing; }
			void set(bool value) { m_useAntialiasing = value; }
		}

		property PlotSettings^ PlotSettings
		{
			DXFViewer::PlotSettings^ get() { return m_plotSettings; }
		}

		property DXFViewer::ExternalPainter^ ExternalPainter
		{
			DXFViewer::ExternalPainter^ get() { return m_externPainter; }
			void set(DXFViewer::ExternalPainter^ value) { m_externPainter = value; }
		}

		property System::Drawing::Printing::PrintDocument^ PrintDocument
		{
			System::Drawing::Printing::PrintDocument^ get() { return (System::Drawing::Printing::PrintDocument^)mPrintDocument; }
			void set(System::Drawing::Printing::PrintDocument^ value)
			{
				
				if (mPrintDocument != nullptr)
				{
					mPrintDocument->PrintPage -= gcnew System::Drawing::Printing::PrintPageEventHandler(this, &DXFControl::OnPrintPage);
				}
				mPrintDocument =value;

				if ( mPrintDocument != nullptr)
					mPrintDocument->PrintPage += gcnew System::Drawing::Printing::PrintPageEventHandler(this, &DXFControl::OnPrintPage);
			}
		}

		property bool OpenNRefresh
		{
			bool get() { return m_openNRefresh; }
			void set(bool value) { m_openNRefresh = value; }
		}

		property System::IntPtr CurrentHDC
		{
			System::IntPtr get() { return m_hdcCurrent; }
		}

		property UnE::Geometry::Vertex2D^ ObjectTL
		{
			UnE::Geometry::Vertex2D^ get() { return m_vObjectTL; }
			void set(UnE::Geometry::Vertex2D^ value) { m_vObjectTL = value; }
		}

		property UnE::Geometry::Vertex2D^ ObjectBR
		{
			UnE::Geometry::Vertex2D^ get() { return m_vObjectBR; }
			void set(UnE::Geometry::Vertex2D^ value) { m_vObjectBR = value; }
		}

		
		// LineType별 Pen
		// Key : Line Style(상위 4바이트) + Line Width(하위 4바이트)
		System::Collections::Generic::Dictionary<__int64, System::Drawing::Pen^>^ GetLineTypePen();
		System::Drawing::Bitmap^ GetCurrentBitmap();

	protected:

		System::Drawing::Printing::PrintDocument^ mPrintDocument;

		Layer^ m_pCurrentLayer;
		Block^ m_pCurrentBlock;

		// 원래의 위치에서 옮겨진 좌표값
		UnE::Geometry::Vertex2D^ m_vMove;
		UnE::Geometry::Vertex2D ^m_vViewportTL, ^m_vViewportBL, ^m_vViewportBR;
		double m_dViewportWeight;

		UnE::Geometry::Vertex2D^ m_vObjectTL;
		UnE::Geometry::Vertex2D^ m_vObjectBR;

		#pragma region OpenGL을 위한 변수들
		//System::Drawing::Bitmap^ m_bitmap;
		//DIBSection* m_pDIB;
		void* m_pDIB;
		bool m_isInitialized;
		//HGLRC m_hRC;
		int m_nRC;
		double m_dOrthoLeft, m_dOrthoTop, m_dOrthoNear;
		System::IntPtr m_hdcCurrent;
		#pragma endregion

		System::Collections::ArrayList^ m_arrLayer;
		System::Collections::ArrayList^ m_arrBlock;
		// Global 좌표를 화면좌표로 바꾸는 Matrix
		System::Drawing::Drawing2D::Matrix^ m_currentMatrix;
		// 화면좌표를 Global 좌표로 바꾸는 Matrix
		System::Drawing::Drawing2D::Matrix^ m_currentInverseMatrix;

		System::Windows::Forms::MouseButtons m_btnPanning;
		bool m_isPanning;
		System::Drawing::Point m_ptPanningOrigin;
		UnE::Geometry::Vertex2D^ m_vOriginCenter;

		bool m_useMouseWheel;
		DXFViewer::UnitOfLength m_unitOfLength;

		LineType^ m_lineTypeSelected;

		System::Drawing::Pen^ m_penEditBox;
		System::Drawing::SolidBrush^ m_brushEditBox;
		int m_nEditBoxSize;
		float m_fEditBoxLength;

		bool m_isOpened;

		float m_fHomem11, m_fHomem12, m_fHomem21, m_fHomem22;
		float m_fHomedx, m_fHomedy;
		UnE::Geometry::Vertex2D ^m_vHomeViewportTL, ^m_vHomeViewportBL, ^m_vHomeViewportBR;
		double m_dHomeViewportWeight;

		System::DateTime m_dtLastMouseWheel;
		// Group으로 묶여질 수 있는 최대 거리(화면좌표)
		int m_nGroupItemDistance;
		// 가까운 거리에 있는 Item들을 Group으로 묶을 것인가?
		bool m_useGroupItem;
		// Group을 만들기 위해 필요한 최소 Item 개수
		int m_nGroupItemMinCount;

		// 다른 Shape보다 Hatch를 먼저 그릴 것인가?
		bool m_drawHatchFirst;
		// 도면을 열때 AutoCAD에서 마지막으로 기억된 Viewport를 사용할 것인가?
		bool m_useLastViewport;
		bool m_useAntialiasing;

		System::Drawing::Pen^ m_penSelectedBright1;
		System::Drawing::Pen^ m_penSelectedBright2;

		DXFViewer::PlotSettings^ m_plotSettings;
		DXFViewer::ExternalPainter^ m_externPainter;

		// DXF 파일을 연후 Refresh를 수행할 것인가?
		// 이 값이 false이면 Thread에서 파일을 열 수 있다.
		bool m_openNRefresh;

		// LineType별 Pen
		// Key : Line Style(상위 4바이트) + Line Width(하위 4바이트)
		System::Collections::Generic::Dictionary<__int64, System::Drawing::Pen^>^ m_dicPens;

		System::Drawing::Image^ m_img;
		//bool m_makeImage;

	private:
		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		System::ComponentModel::Container ^components;
		System::Windows::Forms::Timer^ timerMouseWheel;
		System::Windows::Forms::Timer^ m_WheelTimer;
		IPainter::RendererType m_renderType;

		int m_nMoveX, m_nMoveY;
		bool m_bProcessWheel;

#pragma region Windows Form Designer generated code
		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다.
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
		/// </summary>
		void InitializeComponent(void)
		{
			this->components = (gcnew System::ComponentModel::Container());
			this->timerMouseWheel = gcnew System::Windows::Forms::Timer(this->components);
			this->SuspendLayout();
			// 
            // timerMouseWheel
            // 
			this->timerMouseWheel->Interval = 100;
            this->timerMouseWheel->Tick += gcnew System::EventHandler(this, &DXFControl::timerMouseWheel_Tick);
			//
			// DXFControl
			//
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->DoubleBuffered = true;
			this->Paint += gcnew System::Windows::Forms::PaintEventHandler(this, &DXFControl::OnPaint);
			this->Load += gcnew System::EventHandler(this, &DXFControl::OnLoad);
			this->Resize += gcnew System::EventHandler(this, &DXFControl::OnSize);
			this->MouseDown += gcnew System::Windows::Forms::MouseEventHandler(this, &DXFControl::OnMouseDown);
			this->MouseUp += gcnew System::Windows::Forms::MouseEventHandler(this, &DXFControl::OnMouseUp);
			this->MouseMove += gcnew System::Windows::Forms::MouseEventHandler(this, &DXFControl::OnMouseMove);
			this->MouseWheel += gcnew System::Windows::Forms::MouseEventHandler(this, &DXFControl::OnMouseWheel);
			this->ResumeLayout(false);

		}
#pragma endregion
	};

	public ref class Viewport
	{
	private:
		float m_f11, m_f12, m_f21, m_f22, m_fdx, m_fdy;
		UnE::Geometry::Vertex2D^ m_vTL;
		UnE::Geometry::Vertex2D^ m_vBL;
		UnE::Geometry::Vertex2D^ m_vBR;
		double m_dWeight;

	public:
		property float F11
		{
			float get() { return m_f11; }
			void set(float value) { m_f11 = value; }
		}

		property float F12
		{
			float get() { return m_f12; }
			void set(float value) { m_f12 = value; }
		}

		property float F21
		{
			float get() { return m_f21; }
			void set(float value) { m_f21 = value; }
		}

		property float F22
		{
			float get() { return m_f22; }
			void set(float value) { m_f22 = value; }
		}

		property float FDx
		{
			float get() { return m_fdx; }
			void set(float value) { m_fdx = value; }
		}

		property float FDy
		{
			float get() { return m_fdy; }
			void set(float value) { m_fdy = value; }
		}

		property UnE::Geometry::Vertex2D^ TopLeft
		{
			UnE::Geometry::Vertex2D^ get() { return m_vTL; }
			void set(UnE::Geometry::Vertex2D^ value) { m_vTL = value; }
		}

		property UnE::Geometry::Vertex2D^ BottomLeft
		{
			UnE::Geometry::Vertex2D^ get() { return m_vBL; }
			void set(UnE::Geometry::Vertex2D^ value) { m_vBL = value; }
		}

		property UnE::Geometry::Vertex2D^ BottomRight
		{
			UnE::Geometry::Vertex2D^ get() { return m_vBR; }
			void set(UnE::Geometry::Vertex2D^ value) { m_vBR = value; }
		}

		property double Weight
		{
			double get() { return m_dWeight; }
			void set(double value) { m_dWeight = value; }
		}

	public:
		Viewport()
		{
			m_f11 = m_f12 = m_f21 = m_f22 = m_fdx = m_fdy = 0.0f;
			m_vTL = m_vBL = m_vBR = nullptr;
			m_dWeight = 0.0;
		}
	};
}
