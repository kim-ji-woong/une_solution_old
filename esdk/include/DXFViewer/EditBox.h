#pragma once

namespace DXFViewer
{
	interface class IPainter;

	public ref class EditBox
	{
	public:
		EditBox(IPainter^ owner);
		virtual ~EditBox(void);

	public:
		bool Draw(System::Drawing::Graphics^ g, float x, float y);
		void SetOwner(IPainter^ owner);

	private:
		array<System::Drawing::PointF>^ m_arrPoint;
		IPainter^ m_pOwner;
	};
}
