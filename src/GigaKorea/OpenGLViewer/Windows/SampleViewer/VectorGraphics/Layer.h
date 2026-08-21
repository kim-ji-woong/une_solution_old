#pragma once
#include <string>
#include <list>

namespace VectorGraphics
{
	class Shape;
	class Vertex2D;
	class VectorCtrl;

	class __declspec(dllexport) Layer
	{
	public:
		Layer();
		Layer(std::wstring name);
		virtual ~Layer();

	public:
		void AddShape(Shape* pShape);
		void RemoveShape(Shape* pShape);
		int GetShapeCount();
		Shape* GetShape(int nIndex);
		void Clear();

		void SetLayerName(std::wstring name);
		std::wstring GetLayerName();

		bool Draw();
		void SetColor(COLORREF color);
		COLORREF GetColor();
		void SetVisible(bool visible);
		bool GetVisible();
		void SetLineThick(float fLineThick);
		float GetLineThick();

		Shape* HitTest(const Vertex2D& vPos);
		Shape* HitTestPOI(const Vertex2D& vPos);
		Shape* HitTestExceptPOI(const Vertex2D& vPos);

		void SetControl(VectorCtrl* pCtrl);
		VectorCtrl* GetControl();

	private:
		std::wstring m_strLayerName;
		std::list <Shape*> m_listShapes;
		COLORREF m_color;
		bool m_visible;
		VectorCtrl* m_pCtrl;
		float m_fLineThick;
	};
}
