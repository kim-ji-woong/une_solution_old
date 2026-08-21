#pragma once

namespace Utility
{
	class Vertex3D
	{
	public:
		Vertex3D();
		Vertex3D(double x, double y, double z);
		
	public:
		void SetVertex(double x, double y, double z);
		
	public:
		double m_pt[3];
	};

	class Vertex2D
	{
	public:
		Vertex2D();
		Vertex2D(double x, double y);
		
	public:
		void SetVertex(double x, double y);
		
	public:
		double m_pt[2];
	};
}
