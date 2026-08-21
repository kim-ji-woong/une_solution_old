#include "StdAfx.h"
#include "UEntity3D.h"


namespace UnE
{
	namespace Core
	{

		Vertex::Vertex(float x, float y, float z, float nx, float ny, float nz)
		{
			this->x = x;
			this->y = y;
			this->z = z;
			this->nx = nx;
			this->ny = ny;
			this->nz = nz;
		}

		Vertices::Vertices()
		{
			m_nID = -1;
		}


	}
}