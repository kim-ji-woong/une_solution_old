#pragma once

namespace UnE
{
	namespace Geometry
	{
		ref class Vertex2D;
	}
}

namespace DXFDotNet
{
	ref class Layer;
	ref class Block;
	ref class LineType;
	ref class EntityFactory;

	public interface class IShapeOwner
	{
		void SetCurrentLayer(Layer^ layer);
		Layer^ GetCurrentLayer();

		void SetCurrentBlock(Block^ block);
		Block^ GetCurrentBlock();
		
		//LineType^ GetSelectedLineType();
		UnE::Geometry::Vertex2D^ ScreenToGlobal(int x, int y);
		System::Drawing::Point GlobalToScreen(UnE::Geometry::Vertex2D^ vertex);


		void _Refresh();

		EntityFactory^ GetShapeFactory();
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
