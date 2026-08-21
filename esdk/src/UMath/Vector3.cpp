#include "stdafx.h"
#include "UMath.h"
#include "UVector3.h"


namespace UnE
{
	namespace Math
	{

#ifndef DOTNET
		const Vector3 Vector3::ZERO( 0, 0, 0 );

		const Vector3 Vector3::UNIT_X( 1, 0, 0 );
		const Vector3 Vector3::UNIT_Y( 0, 1, 0 );
		const Vector3 Vector3::UNIT_Z( 0, 0, 1 );
		const Vector3 Vector3::NEGATIVE_UNIT_X( -1,  0,  0 );
		const Vector3 Vector3::NEGATIVE_UNIT_Y(  0, -1,  0 );
		const Vector3 Vector3::NEGATIVE_UNIT_Z(  0,  0, -1 );
		const Vector3 Vector3::UNIT_SCALE(1, 1, 1);
#else // DOTNET ONLY

		bool Vector3::operator==( REF_CONST(Vector3) lhs, REF_CONST(Vector3) rkVector ) CONSTF
		{
			return ( OF(lhs,x) == OF(rkVector,x) && OF(lhs,y) == OF(rkVector,y) && OF(lhs,z) == OF(rkVector,z) );
		}

		bool Vector3::operator!=( REF_CONST(Vector3) lhs , REF_CONST(Vector3) rkVector ) CONSTF
		{
			return ( OF(lhs,x) != OF(rkVector,x) || OF(lhs,y) != OF(rkVector,y) || OF(lhs,z) != OF(rkVector,z) );
		}

		INSTANCE(Vector3) Vector3::operator+( REF_CONST(Vector3) lhs, REF_CONST(Vector3) rkVector ) CONSTF
		{
			return dnonlynew Vector3(
				OF(lhs,x) + OF(rkVector,x),
				OF(lhs,y) + OF(rkVector,y),
				OF(lhs,z) + OF(rkVector,z));
		}

		REF_CONST(Vector3) Vector3::operator+( REF_CONST(Vector3) lhs ) CONSTF
		{
			return lhs;
		}

		INSTANCE(Vector3) Vector3::operator+( REF_CONST(Vector3) lhs, const Real rhs)
		{
			return dnonlynew Vector3(
				OF(lhs,x) + rhs,
				OF(lhs,y) + rhs,
				OF(lhs,z) + rhs);
		}

		INSTANCE(Vector3) Vector3::operator+( const Real lhs, REF_CONST(Vector3) rhs)
		{
			return dnonlynew Vector3(
				lhs + OF(rhs,x),
				lhs + OF(rhs,y),
				lhs + OF(rhs,z));
		}

		INSTANCE(Vector3) Vector3::operator-( REF_CONST(Vector3) lhs, REF_CONST(Vector3) rkVector ) CONSTF
		{
			return dnonlynew Vector3(
				OF(lhs,x) - OF(rkVector,x),
				OF(lhs,y) - OF(rkVector,y),
				OF(lhs,z) - OF(rkVector,z));
		}

		INSTANCE(Vector3) Vector3::operator*( REF_CONST(Vector3) lhs, const Real fScalar ) CONSTF
		{
			return dnonlynew Vector3(
				OF(lhs,x) * fScalar,
				OF(lhs,y) * fScalar,
				OF(lhs,z) * fScalar);
		}

		INSTANCE(Vector3) Vector3::operator*( REF_CONST(Vector3) lhs, REF_CONST(Vector3) rhs) CONSTF
		{
			return dnonlynew Vector3(
				OF(lhs,x) * OF(rhs,x),
				OF(lhs,y) * OF(rhs,y),
				OF(lhs,z) * OF(rhs,z));
		}

		INSTANCE(Vector3) Vector3::operator*( const Real fScalar, REF_CONST(Vector3) rkVector )
		{
			return dnonlynew Vector3(
				fScalar * OF(rkVector,x),
				fScalar * OF(rkVector,y),
				fScalar * OF(rkVector,z));
		}

		INSTANCE(Vector3) Vector3::operator/( REF_CONST(Vector3) lhs, const Real fScalar ) CONSTF
		{
			assert( fScalar != 0.0 );

			Real fInv = 1.0f / fScalar;

			return dnonlynew Vector3(
				OF(lhs,x) * fInv,
				OF(lhs,y) * fInv,
				OF(lhs,z) * fInv);
		}

		INSTANCE(Vector3) Vector3::operator/( REF_CONST(Vector3) lhs, REF_CONST(Vector3) rhs) CONSTF
		{
			return dnonlynew Vector3(
				OF(lhs,x) / OF(rhs,x),
				OF(lhs,y) / OF(rhs,y),
				OF(lhs,z) / OF(rhs,z));
		}

		INSTANCE(Vector3) Vector3::operator/( const Real fScalar, REF_CONST(Vector3) rkVector )
		{
			return dnonlynew Vector3(
				fScalar / OF(rkVector,x),
				fScalar / OF(rkVector,y),
				fScalar / OF(rkVector,z));
		}

		INSTANCE(Vector3) Vector3::operator-( REF_CONST(Vector3) lhs ) CONSTF
		{
			return dnonlynew Vector3(-OF(lhs,x), -OF(lhs,y), -OF(lhs,x));
		}

		INSTANCE(Vector3) Vector3::operator-( REF_CONST(Vector3) lhs, const Real rhs)
		{
			return dnonlynew Vector3(
				OF(lhs,x) - rhs,
				OF(lhs,y) - rhs,
				OF(lhs,z) - rhs);
		}

		INSTANCE(Vector3) Vector3::operator-( const Real lhs, REF_CONST(Vector3) rhs)
		{
			return dnonlynew Vector3(
				lhs - OF(rhs,x),
				lhs - OF(rhs,y),
				lhs - OF(rhs,z));
		}
		//-----------------------------------------------------------------------------
		// arithmetic updates
		//-----------------------------------------------------------------------------
		REF(Vector3) Vector3::operator+=( REF_CONST(Vector3) lhs, REF_CONST(Vector3) rkVector )
		{
			OF(lhs,x) += OF(rkVector,x);
			OF(lhs,y) += OF(rkVector,y);
			OF(lhs,z) += OF(rkVector,z);
			return lhs;
		}
		//-----------------------------------------------------------------------------
		REF(Vector3) Vector3::operator+=( REF_CONST(Vector3) lhs, const Real fScalar )
		{
			OF(lhs,x) += fScalar;
			OF(lhs,y) += fScalar;
			OF(lhs,z) += fScalar;
			return lhs;
		}
		//-----------------------------------------------------------------------------
		REF(Vector3) Vector3::operator-=( REF_CONST(Vector3) lhs,  REF_CONST(Vector3) rkVector )
		{
			OF(lhs,x) -= OF(rkVector,x);
			OF(lhs,y) -= OF(rkVector,y);
			OF(lhs,z) -= OF(rkVector,z);

			return lhs;
		}
		//-----------------------------------------------------------------------------
		REF(Vector3) Vector3::operator-=( REF_CONST(Vector3) lhs, const Real fScalar )
		{
			OF(lhs,x) -= fScalar;
			OF(lhs,y) -= fScalar;
			OF(lhs,z) -= fScalar;
			return lhs;
		}
		//-----------------------------------------------------------------------------
		REF(Vector3) Vector3::operator*=( REF_CONST(Vector3) lhs, const Real fScalar )
		{
			OF(lhs,x) *= fScalar;
			OF(lhs,y) *= fScalar;
			OF(lhs,z) *= fScalar;
			return lhs;
		}
		//-----------------------------------------------------------------------------
		REF(Vector3) Vector3::operator*=( REF_CONST(Vector3) lhs,  REF_CONST(Vector3) rkVector )
		{
			OF(lhs,x) *= OF(rkVector,x);
			OF(lhs,y) *= OF(rkVector,y);
			OF(lhs,z) *= OF(rkVector,z);
			return lhs;
		}
		//-----------------------------------------------------------------------------
		REF(Vector3) Vector3::operator/=( REF_CONST(Vector3) lhs, const Real fScalar )
		{
			assert( fScalar != 0.0 );

			Real fInv = 1.0f / fScalar;
			OF(lhs,x) *= fInv;
			OF(lhs,y) *= fInv;
			OF(lhs,z) *= fInv;
			return lhs;
		}
		//-----------------------------------------------------------------------------
		REF(Vector3) Vector3::operator/=( REF_CONST(Vector3) lhs,  REF_CONST(Vector3) rkVector )
		{
			OF(lhs,x) /= OF(rkVector,x);
			OF(lhs,y) /= OF(rkVector,y);
			OF(lhs,z) /= OF(rkVector,z);
			return lhs;
		}
		//-----------------------------------------------------------------------------
		bool Vector3::operator<( REF_CONST(Vector3) lhs, REF_CONST(Vector3) rhs ) CONSTF
		{
			if(OF(lhs,x) < OF(rhs,x) && OF(lhs,y) < OF(rhs,y) && OF(lhs,z) < OF(rhs,z) )
				return true;
			return false;
		}
		//-----------------------------------------------------------------------------
		bool Vector3::operator>( REF_CONST(Vector3) lhs, REF_CONST(Vector3) rhs ) CONSTF
		{
			if( OF(lhs,x) > OF(rhs,x) && OF(lhs,y) > OF(rhs,y) && OF(lhs,z) > OF(rhs,z) )
				return true;
			return false;
		}
#endif
	}
}
