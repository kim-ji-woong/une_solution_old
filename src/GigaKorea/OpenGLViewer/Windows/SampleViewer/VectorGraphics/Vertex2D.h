#pragma once

namespace VectorGraphics
{
	class __declspec(dllexport) Vertex2D
	{
	public:
		Vertex2D();
		Vertex2D(double x, double y);
		Vertex2D(const Vertex2D& rhs);
		virtual ~Vertex2D();

	public:
		Vertex2D operator+ (const Vertex2D& rhs) const;
		Vertex2D operator- (const Vertex2D& rhs) const;
		Vertex2D operator* (double data) const;
		Vertex2D operator/ (double data) const;

	public:
		double GetDistance(const Vertex2D& vTarget) const;
		Vertex2D GetLinearVertex(const Vertex2D& vTarget, double len) const;
		Vertex2D GetRightVertex(const Vertex2D& vTarget, double len) const;

	public:
		double x, y;
	};
}
