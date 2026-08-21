#ifndef __UNE_CORE_VERTEX_H_INCLUDED__
#define __UNE_CORE_VERTEX_H_INCLUDED__

#pragma once

#include "CoreAPI.h"
#include "UEntity.h"


namespace UnE
{
	namespace Core
	{
		class UEntity;
		template<class Type> class UEntity3D : public UEntity
		{
		private:
			//UEntity3D<Type>(UEntity3D<Type>& rhs){};
			//UEntity3D<Type> operator=(UEntity3D<Type>& rhs){ return *this; }
		public:
			UEntity3D<Type>(){}
			UEntity3D<Type>(Type x, Type y, Type z){ m_X = x; m_Y = y; m_Z = z; }
			virtual ~UEntity3D<Type>(){}

			bool operator==(UEntity3D<Type>& rhs);
			bool operator!=(UEntity3D<Type>& rhs);

			UEntity3D<Type> operator-(UEntity3D<Type>& rhs);
			UEntity3D<Type> operator+(UEntity3D<Type>& rhs);

			void SetPosition( Type x, Type y , Type z);

			UEntity3D<Type> Offset ( Type x, Type y);
			UEntity3D<Type> Offset ( UEntity3D<Type>& rhs );

			void Distance(UEntity3D<Type>& rhs, Type* retValue);

		protected:
			Type m_X;
			Type m_Y;
			Type m_Z;
		};

		class CORE_API Vertex : public UEntity3D<float>
		{
		public:
			Vertex(float x = 0.0f, float y = 0.0f, float z = 0.0f, float nx = 0.0f, float ny = 0.0f, float nz = 0.0f);

		public:
			float x, y, z;
			float nx, ny, nz;
		};
		
		class CORE_API Vertices : public UEntity
		{
		public:
			Vertices();

		public:
			int m_nID;
			std::vector<Vertex> m_vecVertex;
		};


	}
}



#endif//__UNE_CORE_VERTEX_H_INCLUDED__