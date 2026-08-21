#pragma once
#include "IShapeOwner.h"

using namespace System::ComponentModel;

namespace DXFDotNet {

	ref class Layer;
	ref class Block;
	ref class Shape;
	ref class LineType;
	ref class Viewport;
	ref class PlotSettings;
	ref class UPrintDocument;
	ref class EntityFactory;
	
	/// <summary>
	/// DXFControl에 대한 요약입니다::
	/// </summary>
	public ref class DXFControl : System::Windows::Forms::UserControl, IShapeOwner
	{
	protected:
		DXFControl(void)
		{		
			Init();
		}

	protected:
		/// <summary>
		/// 사용 중인 모든 리소스를 정리합니다::
		/// </summary>
		~DXFControl()
		{
					
		}


		EntityFactory^ mFactory;
	public:

		void Init();
		//bool OpenDXF(System::String^ strPath);
		//void CloseDXF();

		virtual void SetCurrentLayer(Layer^ layer);
		virtual Layer^ GetCurrentLayer();

		virtual void SetCurrentBlock(Block^ block);
		virtual Block^ GetCurrentBlock();

		////virtual void SetViewportCenter(UnE::Geometry::Vertex2D^ vCenter);
		////virtual UnE::Geometry::Vertex2D^ GetViewportCenter();
		//virtual double GetViewportWeight();
		//void SetViewportWeight(double dWeight);
		//virtual int GetScreenWidth();
		//virtual int GetScreenHeight();

		//virtual LineType^ GetSelectedLineType();

		//virtual void Zoom(double dZoomValue, UnE::Geometry::Vertex2D^ vZoomCenter, bool refresh);

		//// Y축이 화면 아래에서 위쪽으로 증가하는 방향인가?
		virtual bool DownToTop();

		virtual void _Refresh();

		//virtual UnE::Geometry::Vertex2D^ ScreenToGlobal(int x, int y);
		//virtual System::Drawing::Point GlobalToScreen(UnE::Geometry::Vertex2D^ vertex);

		//virtual System::Drawing::Color GetBackColor();

		virtual UnE::Geometry::Vertex2D^ ScreenToGlobal(int x, int y);

		virtual System::Drawing::Point GlobalToScreen(UnE::Geometry::Vertex2D^ vertex);

		virtual EntityFactory^ GetShapeFactory();
		virtual void SetShapeFactory(EntityFactory^ factory);

		//Shape^ SelectObject(double x, double y);
		//Shape^ PickObject(double x, double y);
		//void PickObject(Shape^ shape);
		//// 모든 객체들을 현재의 위치로부터 (x, y) 만큼 이동시킨다::
		//void MoveAll(double x, double y);

		//// EditBox
		//void SetEditBoxColor(System::Drawing::Color color, bool isFill);
		//System::Drawing::Color GetColor(bool isFill);
		//void SetEditBoxSize(int nLen);
		//int GetEditBoxSize();

		//void SaveHomeMatrix();
		//void LoadHomeMatrix(bool refresh);
		//Viewport^ GetViewport();
		//void LoadViewport(Viewport^ viewport, bool refresh);

		//void CalcShapeGroup();

		//void OnMouseWheel(System::Object^ sender, System::Windows::Forms::MouseEventArgs^ e);
		/*virtual property bool SetExternalWheelEvent
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
		}*/

	public:

		/*bool m_bExternalWheel;
		void OnLoad(System::Object^ sender, System::EventArgs^ e);
	    void OnSize(System::Object^ sender, System::EventArgs^ e);
		void OnMouseDown(System::Object^ sender, System::Windows::Forms::MouseEventArgs^ e);
		void OnMouseUp(System::Object^ sender, System::Windows::Forms::MouseEventArgs^ e);
		void OnMouseMove(System::Object^ sender, System::Windows::Forms::MouseEventArgs^ e);
		
		void timerMouseWheel_Tick(System::Object^ sender, System::EventArgs^ e);
		void OnWheelTimerTick(System::Object^ sender, System::EventArgs^ e);
		void Init();
		void Reshape(int nWidth, int nHeight);
		void ReshapeGDI(int nWidth, int nHeight);*/
	

		//void OnPrintPage(System::Object^ sender, System::Drawing::Printing::PrintPageEventArgs^ e);
		//void OnPrint(System::Object^ sender, System::Windows::Forms::PaintEventArgs^ e);
		//void CreateImage(int nWidth, int nHeight);
		//System::Drawing::Image^ CreateScreenImage();

		property System::Collections::ArrayList^ Layers
		{
			System::Collections::ArrayList^ get() { return m_arrLayer; }
		}

		property System::Collections::ArrayList^ Blocks
		{
			System::Collections::ArrayList^ get() { return m_arrBlock; }
		}

		property DXFDotNet::UnitOfLength UnitOfLength
		{
			DXFDotNet::UnitOfLength get() { return m_unitOfLength; }
			void set(DXFDotNet::UnitOfLength value) { m_unitOfLength = value; }
		}

		[Browsable(false)]
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

	
		[Browsable(false)]
		property UnE::Geometry::Vertex2D^ ObjectTL
		{
			UnE::Geometry::Vertex2D^ get() { return m_vObjectTL; }
			void set(UnE::Geometry::Vertex2D^ value) { m_vObjectTL = value; }
		}
		[Browsable(false)]
		property UnE::Geometry::Vertex2D^ ObjectBR
		{
			UnE::Geometry::Vertex2D^ get() { return m_vObjectBR; }
			void set(UnE::Geometry::Vertex2D^ value) { m_vObjectBR = value; }
		}
		[Browsable(false)]
		property UnE::Geometry::Vertex2D^ ObjectCenter
		{
			UnE::Geometry::Vertex2D^ get() { return m_vObjectCenter; }
			void set(UnE::Geometry::Vertex2D^ value) { m_vObjectCenter = value; }
		}
		[Browsable(false)]
		System::Collections::Generic::Dictionary<__int64, System::Drawing::Pen^>^ GetLineTypePen();
		
	protected:		
		// LineType별 Pen
		// Key : Line Style(상위 4바이트) + Line Width(하위 4바이트)
		System::Collections::Generic::Dictionary<__int64, System::Drawing::Pen^>^ m_dicPens;

		Layer^ m_pCurrentLayer;
		Block^ m_pCurrentBlock;
		
		UnE::Geometry::Vertex2D^ m_vObjectTL;
		UnE::Geometry::Vertex2D^ m_vObjectBR;
		UnE::Geometry::Vertex2D^ m_vObjectCenter;

		System::Collections::ArrayList^ m_arrLayer;
		System::Collections::ArrayList^ m_arrBlock;

		//UnE::Geometry::Vertex2D^ m_vOriginCenter;
		
		DXFDotNet::UnitOfLength m_unitOfLength;

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

		// 도면을 열때 AutoCAD에서 마지막으로 기억된 Viewport를 사용할 것인가?
		bool m_useLastViewport;	
	};

	public ref class Viewport
	{
	protected:
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
			m_f11 = m_f12 = m_f21 = m_f22 = m_fdx = m_fdy = 0.f;
			m_vTL = m_vBL = m_vBR = nullptr;
			m_dWeight = 0.0;
		}
	};
}
