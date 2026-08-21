#pragma once

namespace UnE
{
	namespace Geometry
	{
		ref class Vertex2D;
	}
}

namespace DXFViewer
{
	ref class Layer;
	ref class Block;
	ref class LineType;

	public interface class IPainter
	{
		enum class RendererType
		{
			GDI_PLUS = 0, OPEN_GL
		};

		void SetCurrentLayer(Layer^ layer);
		Layer^ GetCurrentLayer();

		void SetCurrentBlock(Block^ block);
		Block^ GetCurrentBlock();

		void SetViewportCenter(UnE::Geometry::Vertex2D^ vCenter);
		UnE::Geometry::Vertex2D^ GetViewportCenter();
		double GetViewportWeight();
		int GetScreenWidth();
		int GetScreenHeight();

		LineType^ GetSelectedLineType();

		void Zoom(double dZoomValue, UnE::Geometry::Vertex2D^ vZoomCenter, bool refresh);

		// Y축이 화면 아래에서 위쪽으로 증가하는 방향인가?
		bool DownToTop();

		void _Refresh();

		UnE::Geometry::Vertex2D^ ScreenToGlobal(int x, int y);
		System::Drawing::Point GlobalToScreen(UnE::Geometry::Vertex2D^ vertex);

		System::Drawing::Color GetBackColor();

		property float EditBoxLength
		{
			float get();
		}

		property System::Drawing::SolidBrush^ EditBoxBrush
		{
			System::Drawing::SolidBrush^ get();
		}

		property System::Drawing::Pen^ EditBoxPen
		{
			System::Drawing::Pen^ get();
		}

		property System::Drawing::Pen^ SelectedBrightPen1
		{
			System::Drawing::Pen^ get();
		}

		property System::Drawing::Pen^ SelectedBrightPen2
		{
			System::Drawing::Pen^ get();
		}

		property RendererType Renderer
		{
			RendererType get();
			void set(RendererType value);
		}
	};

	public ref class ExternalPainter abstract
	{
	public:
		// OnPaint() 호출되기 직전에 호출된다.
		//virtual void OnPrevPaint(System::Windows::Forms::PaintEventArgs^ e) abstract;
		virtual void OnPrevPaint(System::Drawing::Graphics^ g, bool bDrawText) abstract;
		// OnPaint() 호출된 직후에 호출된다.
		//virtual void OnPostPaint(System::Windows::Forms::PaintEventArgs^ e) abstract;
		virtual void OnPostPaint(System::Drawing::Graphics^ g, bool bDrawText) abstract;
		
		// 모든 Drawing이 끝난 직후 호출된다.
		virtual void OnOverlayPaint(System::Drawing::Graphics^ g, bool bDrawText) abstract;

		// OnPrintPage() 호출되기 직전에 호출된다.
		//virtual void OnPrevPrint(System::Windows::Forms::PaintEventArgs^ e) abstract;
		virtual void OnPrevPrint(System::Drawing::Graphics^ g) abstract;
		// OnPrintPage() 호출된 직후에 호출된다.
		//virtual void OnPostPrint(System::Windows::Forms::PaintEventArgs^ e) abstract;
		virtual void OnPostPrint(System::Drawing::Graphics^ g) abstract;
	};
}
