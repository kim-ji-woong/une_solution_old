#include "stdafx.h"
#include "UMath.h"
#include "UVector2.h"


namespace UnE
{
	namespace Math
	{

#ifndef DOTNET
		const Vector2 Vector2::ZERO( 0, 0);

		const Vector2 Vector2::UNIT_X( 1, 0);
		const Vector2 Vector2::UNIT_Y( 0, 1);
		const Vector2 Vector2::NEGATIVE_UNIT_X( -1,  0);
		const Vector2 Vector2::NEGATIVE_UNIT_Y(  0, -1);
		const Vector2 Vector2::UNIT_SCALE(1, 1);

#else  // DOTNET ONLY

		//------------------------------------------------------------
		bool Vector2::operator==( REF_CONST(Vector2) lhs, REF_CONST(Vector2) rkVector ) CONSTF
		{
			return ( OF(lhs,x) == OF(rkVector,x) && OF(lhs,y) == OF(rkVector,y) );
		}
		//------------------------------------------------------------------------------
		bool Vector2::operator!=( REF_CONST(Vector2) lhs, REF_CONST(Vector2) rkVector ) CONSTF
		{
			return ( OF(lhs,x) != OF(rkVector,x) || OF(lhs,y) != OF(rkVector,y)  );
		}
		//------------------------------------------------------------------------------
		INSTANCE(Vector2) Vector2::operator+( REF_CONST(Vector2) lhs, REF_CONST(Vector2) rkVector ) CONSTF
		{
			return dnonlynew Vector2(OF(lhs,x) + OF(rkVector,x) , OF(lhs,y) + OF( rkVector,y) );
		}
		//------------------------------------------------------------------------------
		REF_CONST(Vector2) Vector2::operator+( REF_CONST(Vector2) lhs) CONSTF
		{
			return lhs;
		}
		//------------------------------------------------------------------------------
		INSTANCE(Vector2) Vector2::operator+( REF_CONST(Vector2) lhs, const Real rhs)
		{
			return dnonlynew Vector2( OF(lhs,x) + rhs, OF(lhs,y) + rhs);
		}
		//------------------------------------------------------------------------------
		INSTANCE(Vector2) Vector2::operator+( const Real lhs, REF_CONST(Vector2) rhs)
		{
			return dnonlynew Vector2(	lhs + OF(rhs,x), lhs + OF(rhs,y));
		}
		//------------------------------------------------------------------------------
		INSTANCE(Vector2) Vector2::operator-( REF_CONST(Vector2) lhs, REF_CONST(Vector2) rkVector ) CONSTF
		{
			return dnonlynew Vector2( OF(lhs,x) - OF( rkVector, x ), OF(lhs,y) - OF( rkVector, y) );
		}
		//------------------------------------------------------------------------------
		INSTANCE(Vector2) Vector2::operator*( REF_CONST(Vector2) lhs, const Real fScalar ) CONSTF
		{
			return dnonlynew Vector2(OF(lhs,x) * fScalar, OF(lhs,y) * fScalar);
		}
		//------------------------------------------------------------------------------
		INSTANCE(Vector2) Vector2::operator*( REF_CONST(Vector2) lhs, REF_CONST(Vector2) rhs) CONSTF
		{
			return dnonlynew Vector2( OF(lhs,x) * OF( rhs, x) , OF(lhs,y) * OF( rhs, y ));
		}
		//------------------------------------------------------------------------------
		INSTANCE(Vector2) Vector2::operator*( const Real fScalar, REF_CONST(Vector2) rkVector )
		{
			return dnonlynew Vector2(fScalar * OF(rkVector,x), fScalar * OF(rkVector,y));
		}
		//------------------------------------------------------------------------------
		INSTANCE(Vector2) Vector2::operator/( REF_CONST(Vector2) lhs, const Real fScalar ) CONSTF
		{
			assert( fScalar != 0.0 );

			Real fInv = 1.0f / fScalar;

			return dnonlynew Vector2(  OF(lhs,x) * fInv,OF(lhs,y) * fInv);
		}
		//------------------------------------------------------------------------------
		INSTANCE(Vector2) Vector2::operator/( REF_CONST(Vector2) lhs, REF_CONST(Vector2) rhs) CONSTF
		{
			return dnonlynew Vector2( OF(lhs,x) / OF(rhs,x), OF(lhs,y) / OF(rhs,y));
		}
		//------------------------------------------------------------------------------
		INSTANCE(Vector2) Vector2::operator/(const Real fScalar, REF_CONST(Vector2) rkVector )
		{
			return dnonlynew Vector2(fScalar / OF(rkVector,x), fScalar / OF(rkVector,y));
		}
		//------------------------------------------------------------------------------
		INSTANCE(Vector2) Vector2::operator-( REF_CONST(Vector2) lhs) CONSTF
		{
			return dnonlynew Vector2(-OF(lhs,x), -OF(lhs,y));
		}
		//------------------------------------------------------------------------------
		INSTANCE(Vector2) Vector2::operator-( REF_CONST(Vector2) lhs, const Real rhs)
		{
			return dnonlynew Vector2( OF(lhs,x) - rhs, OF(lhs,y) - rhs);
		}
		//------------------------------------------------------------------------------
		INSTANCE(Vector2) Vector2::operator-( const Real lhs, REF_CONST(Vector2) rhs)
		{
			return dnonlynew Vector2( lhs - OF(rhs,x), lhs - OF(rhs,y));
		}
		//------------------------------------------------------------------------------
		REF(Vector2) Vector2::operator += ( REF_CONST(Vector2) lhs, REF_CONST(Vector2) rkVector )		
		{
			OF(lhs,x) += OF(rkVector,x);
			OF(lhs,y) += OF(rkVector,y);

			return lhs;
		}
		//------------------------------------------------------------------------------
		REF(Vector2) Vector2::operator += ( REF_CONST(Vector2) lhs, const Real fScaler )
		{
			OF(lhs,x) += fScaler;
			OF(lhs,y) += fScaler;

			return lhs;
		}
		//------------------------------------------------------------------------------
		REF(Vector2) Vector2::operator -= ( REF_CONST(Vector2) lhs, REF_CONST(Vector2) rkVector )
		{
			OF(lhs,x) -= OF(rkVector,x);
			OF(lhs,y) -= OF(rkVector,y);

			return lhs;
		}
		//------------------------------------------------------------------------------
		REF(Vector2) Vector2::operator -= ( REF_CONST(Vector2) lhs, const Real fScaler )
		{
			OF(lhs,x) -= fScaler;
			OF(lhs,y) -= fScaler;

			return lhs;
		}
		//------------------------------------------------------------------------------
		REF(Vector2) Vector2::operator *= ( REF_CONST(Vector2) lhs, const Real fScalar )
		{
			OF(lhs,x) *= fScalar;
			OF(lhs,y) *= fScalar;

			return lhs;
		}
		//------------------------------------------------------------------------------
		REF(Vector2) Vector2::operator *= ( REF_CONST(Vector2) lhs, REF_CONST(Vector2) rkVector )
		{
			OF(lhs,x) *= OF(rkVector,x);
			OF(lhs,y) *= OF(rkVector,y);

			return lhs;
		}
		//------------------------------------------------------------------------------
		REF(Vector2) Vector2::operator /= ( REF_CONST(Vector2) lhs, const Real fScalar )
		{
			assert( fScalar != 0.0 );

			Real fInv = 1.0f / fScalar;

			OF(lhs,x) *= fInv;
			OF(lhs,y) *= fInv;

			return lhs;
		}
		//------------------------------------------------------------------------------
		REF(Vector2) Vector2::operator /= ( REF_CONST(Vector2) lhs, REF_CONST(Vector2) rkVector )
		{
			OF(lhs,x) /= OF(rkVector,x);
			OF(lhs,y) /= OF(rkVector,y);
			return lhs;
		}

#endif // dotnet

	}
}
