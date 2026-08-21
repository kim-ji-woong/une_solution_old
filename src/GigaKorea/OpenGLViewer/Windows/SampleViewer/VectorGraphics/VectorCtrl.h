#pragma once
#include "DIBSection.h"
#include <list>
#include "Vertex2D.h"

namespace VectorGraphics
{
	class Layer;
	class Shape;

	class __declspec(dllexport) VectorCtrl
	{
	public:
		enum MouseType { LBUTTON, MBUTTON, RBUTTON };

	public:
		VectorCtrl();
		virtual ~VectorCtrl();

	public:
		void OnCreate(int nWidth, int nHeight);
		void OnDraw(HDC hdc);
		void OnSize(int nWidth, int nHeight);
		void GetScreenSize(int& rWidth, int& rHeight);

		void AddLayer(Layer* pLayer);
		void RemoveLayer(Layer* pLayer);
		int GetLayerCount();
		Layer* GetLayer(int nIndex);
		void Clear();

		void MouseDown(int x, int y, MouseType type);
		void MouseUp(int x, int y, MouseType type);
		bool MouseMove(int x, int y);

		void SetViewportWeight(double dWeight);
		double GetViewportWeight();
		void Zoom(int x, int y, double dWeight);

		void SetViewportCenter(double x, double y);
		Vertex2D GetViewportCenter();

		void ScreenToGlobal(int x, int y, Vertex2D* pVertex);
		void GlobalToScreen(const Vertex2D& rVertex, int* x, int* y);

		Shape* HitTest(const Vertex2D& vPos);
		Shape* HitTestPOI(const Vertex2D& vPos);
		Shape* HitTestExceptPOI(const Vertex2D& vPos);

		HDC GetHDC();

		void SetBackgroundColor(COLORREF colBk);
		COLORREF GetBackgroundColor();

	private:
		void Display(HDC hdc);
		void InitSize(int nWidth, int nHeight);
		void CreateImage(int nWidth, int nHeight);
		BOOL SetImagePixelFormat(HDC hdc);
		BOOL CreateImageGLContext(HDC hdc);
		void InitRC();

		bool IsInitialized() const { return m_isInitialized; };

		void RenderImage();
		void DrawImage(HDC hdc);
		void SizeImage(int nWidth, int nHeight);
		void Reshape(int nWidth, int nHeight);

	private:
		HGLRC m_hRC;
		DIBSection m_dib;

		float m_fColLine[3];
		COLORREF m_colBk;

		bool m_isInitialized;

		int m_nWidth, m_nHeight;
		Vertex2D m_vTL, m_vBL, m_vBR, m_vCenter, m_vClicked;
		double m_dViewportWeight;

		std::list<Layer*> m_listLayers;

		bool m_isMButtonDown;
		int m_nClickedX, m_nClickedY;
	};
}
