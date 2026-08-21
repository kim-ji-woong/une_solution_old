#pragma once

namespace VectorGraphics
{
	class Vertex2D;
	class Layer;

	class __declspec(dllexport) Shape
	{
	public:
		Shape();
		virtual ~Shape();

	public:
		virtual void Draw() = 0;
		virtual bool HitTest(const Vertex2D& vPos);
		virtual bool HitTestIfPOI(const Vertex2D& vPos);
		virtual bool HitTestIfNotPOI(const Vertex2D& vPos);

	public:
		Layer* GetLayer();

	protected:
		static void DrawVertex(const Vertex2D& rVertex);

	private:
		void SetLayer(Layer* pLayer);

	private:
		Layer* m_pLayer;
		friend class Layer;
	};
}
