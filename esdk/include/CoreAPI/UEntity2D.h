#ifndef __UNE_CORE_POINT2D_H_INCLUDED__
#define __UNE_CORE_POINT2D_H_INCLUDED__

#pragma once

#include "CoreAPI.h"
#include "UEntity.h"

namespace UnE
{

	namespace Core
	{
		//////////////////////////////////////////////////////////////////////////
		// POINT 2D TEMPLATE		

		template <class Type> class UEntity2D : public UEntity
		{
		private:
			UEntity2D<Type>(UEntity2D<Type>& rhs){};
			UEntity2D<Type> operator=(UEntity2D<Type>& rhs){ return *this; }
		public:
			UEntity2D<Type>(){}
			UEntity2D<Type>(Type x, Type y){ m_X = x; m_Y = y; }
			virtual ~UEntity2D<Type>(){}

			bool operator==(UEntity2D<Type>& point);
			bool operator!=(UEntity2D<Type>& point);

			UEntity2D<Type> operator-(UEntity2D<Type>& rhs);
			UEntity2D<Type> operator+(UEntity2D<Type>& rhs);
			
			void SetPosition( Type x, Type y );

			UEntity2D<Type> Offset ( Type x, Type y);
			UEntity2D<Type> Offset ( UEntity2D<Type>& rhs );

			void Distance(UEntity2D<Type>& rhs, Type* retValue);

		protected:
			Type m_X;
			Type m_Y;
		};

		//////////////////////////////////////////////////////////////////////////
		// Template Function
		template<class Type>
		UnE::Core::UEntity2D<Type> UnE::Core::UEntity2D<Type>::operator-( UEntity2D<Type>& rhs )
		{
			return UEntity2D<Type>( m_X - rhs.m_X, m_Y - rhs.m_Y);
		}

		template<class Type>
		bool UnE::Core::UEntity2D<Type>::operator==( UEntity2D<Type>& pt )
		{
			if( m_X == pt.m_X && m_Y == pt.m_Y)
				return true;
			return false;
		}

		template<class Type>
		bool UnE::Core::UEntity2D<Type>::operator!=( UEntity2D<Type>& pt )
		{
			return !(operator==(pt));
		}

		template<class Type>
		UnE::Core::UEntity2D<Type> UnE::Core::UEntity2D<Type>::operator+( UEntity2D<Type>& rhs )
		{
			return UEntity2D<Type>(m_X + rhs.m_X, m_Y + rhs.m_Y);
		}

		template<class Type>
		void UnE::Core::UEntity2D<Type>::SetPosition( Type x,  Type y )
		{
			m_X = x;  m_Y = y;
		}

		template<class Type>
		UnE::Core::UEntity2D<Type> UnE::Core::UEntity2D<Type>::Offset( Type x, Type y )
		{
			return UEntity2D<Type>(m_X + x , m_Y + y);
		}

		template<class Type>
		UnE::Core::UEntity2D<Type> UnE::Core::UEntity2D<Type>::Offset( UEntity2D<Type>& rhs )
		{
			return UEntity2D<Type>(m_X + rhs.m_X, m_Y + rhs.m_Y);
		}

		template <class Type>
		void UnE::Core::UEntity2D<Type>::Distance( UEntity2D<Type>& rhs, Type* retValue )
		{
			if( retValue == NULL) return;
		}

		//////////////////////////////////////////////////////////////////////////
		// EXPORT CLASS 
		class CORE_API Point2Di : public UEntity2D<int>{};
		class CORE_API Point2Df : public UEntity2D<float>{};
		class CORE_API Point2Dd : public UEntity2D<double>{};
		class CORE_API Point2Dl : public UEntity2D<long>{};
		

	}//Core
}//UnE

#endif//__UNE_CORE_POINT2D_H_INCLUDED__