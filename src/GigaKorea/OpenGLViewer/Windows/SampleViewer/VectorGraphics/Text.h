#pragma once
#include "Vertex2D.h"
#include "Shape.h"
#include <string>

namespace VectorGraphics
{
	class __declspec(dllexport) Text : public Shape
	{
	public:
		Text();
		virtual ~Text();

	public:
		void Draw();
		void SetPosition(const Vertex2D& vPos);
		void SetContents(const std::wstring& str);
		// Degree
		void SetAngle(double dAngle);
		void SetFontName(const std::wstring& strFontName);
		void SetFontSize(double dFontSize);

		const Vertex2D& GetPosition();
		const std::wstring& GetContents();
		// Degree
		double GetAngle();
		const std::wstring& GetFontName();
		double GetFontSize();

	private:
		Vertex2D m_vPos;
		// Degree
		double m_dAngle;
		double m_dFontSize;
		std::wstring m_strFontName;
		std::wstring m_strContents;
	};
}
