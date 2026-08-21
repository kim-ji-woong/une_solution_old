#include "stdafx.h"
#include "Vertex2D.h"
#include <math.h>

namespace VectorGraphics
{
	Vertex2D::Vertex2D()
	{
		x = y = 0;
	}

	Vertex2D::Vertex2D(double x, double y)
	{
		this->x = x;
		this->y = y;
	}

	Vertex2D::Vertex2D(const Vertex2D& rhs)
	{
		this->x = rhs.x;
		this->y = rhs.y;
	}

	Vertex2D::~Vertex2D()
	{
	}

	Vertex2D Vertex2D::operator+ (const Vertex2D& rhs) const
	{
		return Vertex2D(this->x + rhs.x, this->y + rhs.y);
	}

	Vertex2D Vertex2D::operator- (const Vertex2D& rhs) const
	{
		return Vertex2D(this->x - rhs.x, this->y - rhs.y);
	}

	Vertex2D Vertex2D::operator* (double data) const
	{
		return Vertex2D(this->x * data, this->y * data);
	}

	Vertex2D Vertex2D::operator/ (double data) const
	{
		return Vertex2D(this->x / data, this->y / data);
	}

	double Vertex2D::GetDistance(const Vertex2D& vTarget) const
	{
		return sqrt((x - vTarget.x) * (x - vTarget.x) + (y - vTarget.y) * (y - vTarget.y));
	}

	Vertex2D Vertex2D::GetLinearVertex(const Vertex2D& vTarget, double len) const
	{
		double dLength = GetDistance(vTarget);

		if (dLength == 0.0)
			return vTarget;

		return *this + (vTarget - *this) * len / dLength;
	}

	Vertex2D Vertex2D::GetRightVertex(const Vertex2D& vTarget, double len) const
	{
		double len2 = GetDistance(vTarget);

		if (len2 < 0.001)
			return vTarget;

		Vertex2D vResult;
		vResult.x = len / len2 * (this->y - vTarget.y) + this->x;
		vResult.y = len / len2 * (vTarget.x - this->x) + this->y;
		return vResult;
	}
}
