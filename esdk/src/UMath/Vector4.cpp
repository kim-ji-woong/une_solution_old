#include "stdafx.h"


#include "UVector4.h"
#include "UMath.h"

using namespace UnE::Math;

namespace UnE
{
	namespace Math
	{

#ifndef DOTNET		
		const Vector4 Vector4::ZERO( 0, 0, 0, 0 );				
#else

		bool Vector4::operator == ( REF_CONST(Vector4) lhs, REF_CONST(Vector4) rkVector ) CONSTF
		{
			return ( OF(lhs, x) == OF(rkVector,x) &&
				OF(lhs, y) ==  OF(rkVector,y) &&
				OF(lhs, z) ==  OF(rkVector,z) &&
				OF(lhs, w) ==  OF(rkVector,w) );
		}

		bool Vector4::operator != ( REF_CONST(Vector4) lhs, REF_CONST(Vector4) rkVector ) CONSTF
		{
			return ( OF(lhs, x) != OF(rkVector,x) ||
				OF(lhs, y) != OF(rkVector,y) ||
				OF(lhs, z) != OF(rkVector,z) ||
				OF(lhs, w) != OF(rkVector,w) );
		}


		// arithmetic operations
		INSTANCE(Vector4) Vector4::operator + ( REF_CONST(Vector4) lhs, REF_CONST(Vector4) rkVector ) CONSTF
		{
			return dnonlynew Vector4(
				OF(lhs, x) + OF(rkVector,x),
				OF(lhs, y) + OF(rkVector,y),
				OF(lhs, z) + OF(rkVector,z),
				OF(lhs, w) + OF(rkVector,w));
		}

		INSTANCE(Vector4) Vector4::operator - ( REF_CONST(Vector4) lhs, REF_CONST(Vector4) rkVector ) CONSTF
		{
			return dnonlynew Vector4(
				OF(lhs, x) - OF(rkVector,x),
				OF(lhs, y) - OF(rkVector,y),
				OF(lhs, z) - OF(rkVector,z),
				OF(lhs, w) - OF(rkVector,w));
		}

		INSTANCE(Vector4) Vector4::operator * ( REF_CONST(Vector4) lhs, SYS_CONST(Real) fScalar ) CONSTF
		{
			return dnonlynew Vector4(
				OF(lhs, x) * fScalar,
				OF(lhs, y) * fScalar,
				OF(lhs, z) * fScalar,
				OF(lhs, w) * fScalar);
		}

		INSTANCE(Vector4) Vector4::operator * ( REF_CONST(Vector4) lhs, REF_CONST(Vector4) rhs) CONSTF
		{
			return dnonlynew Vector4(
				OF(rhs,x) * OF(lhs, x),
				OF(rhs,y) * OF(lhs, y),
				OF(rhs,z) * OF(lhs, z),
				OF(rhs,w) * OF(lhs, w));
		}

		INSTANCE(Vector4) Vector4::operator / ( REF_CONST(Vector4) lhs, const Real fScalar ) CONSTF
		{
			assert( fScalar != 0.0 );

			Real fInv = 1.0f / fScalar;

			return dnonlynew Vector4(
				OF(lhs, x) * fInv,
				OF(lhs, y) * fInv,
				OF(lhs, z) * fInv,
				OF(lhs, w) * fInv);
		}

		INSTANCE(Vector4) Vector4::operator / ( REF_CONST(Vector4) lhs, REF_CONST(Vector4) rhs) CONSTF
		{
			return dnonlynew Vector4(
				OF(lhs, x) / OF(rhs,x),
				OF(lhs, y) / OF(rhs,y),
				OF(lhs, z) / OF(rhs,z),
				OF(lhs, w) / OF(rhs,w));
		}

		REF_CONST(Vector4) Vector4::operator + (REF_CONST(Vector4) lhs) CONSTF
		{
			return lhs;
		}

		INSTANCE(Vector4) Vector4::operator - (REF_CONST(Vector4) lhs) CONSTF
		{
			return dnonlynew Vector4(- (OF(lhs,x)), - (OF(lhs,y)), - (OF(lhs,z)), - (OF(lhs,w)));
		}

		INSTANCE(Vector4) Vector4::operator * ( SYS_CONST(Real)fScalar, REF_CONST(Vector4) rkVector )
		{
			return dnonlynew Vector4(
				fScalar * OF(rkVector,x),
				fScalar * OF(rkVector,y),
				fScalar * OF(rkVector,z),
				fScalar * OF(rkVector,w));
		}

		INSTANCE(Vector4) Vector4::operator / ( SYS_CONST(Real) fScalar, REF_CONST(Vector4) rkVector )
		{
			return dnonlynew Vector4(
				fScalar / OF(rkVector,x),
				fScalar / OF(rkVector,y),
				fScalar / OF(rkVector,z),
				fScalar / OF(rkVector,w));
		}

		INSTANCE(Vector4) Vector4::operator + (REF_CONST(Vector4) lhs, SYS_CONST(Real) rhs)
		{
			return dnonlynew Vector4(
				OF(lhs,x) + rhs,
				OF(lhs,y) + rhs,
				OF(lhs,z) + rhs,
				OF(lhs,w) + rhs);
		}

		INSTANCE(Vector4) Vector4::operator + (SYS_CONST(Real) lhs, REF_CONST(Vector4) rhs)
		{
			return dnonlynew Vector4(
				lhs + OF(rhs,x),
				lhs + OF(rhs,y),
				lhs + OF(rhs,z),
				lhs + OF(rhs,w));
		}

		INSTANCE(Vector4) Vector4::operator - (REF_CONST(Vector4) lhs, SYS_CONST(Real) rhs)
		{
			return dnonlynew Vector4(
				OF(lhs,x) - rhs,
				OF(lhs,y) - rhs,
				OF(lhs,z) - rhs,
				OF(lhs,w) - rhs);
		}

		INSTANCE(Vector4) Vector4::operator - (SYS_CONST(Real) lhs, REF_CONST(Vector4)rhs)
		{
			return dnonlynew Vector4(
				lhs - OF(rhs,x),
				lhs - OF(rhs,y),
				lhs - OF(rhs,z),
				lhs - OF(rhs,w));
		}

		// arithmetic updates
		REF(Vector4) Vector4::operator += ( REF_CONST(Vector4) lhs, REF_CONST(Vector4) rkVector )
		{
			OF(lhs,x) += OF(rkVector,x);
			OF(lhs,y) += OF(rkVector,y);
			OF(lhs,z) += OF(rkVector,z);
			OF(lhs,w) += OF(rkVector,w);

			return lhs;
		}

		REF(Vector4) Vector4::operator -= ( REF_CONST(Vector4) lhs, REF_CONST(Vector4) rkVector )
		{
			OF(lhs,x) -= OF(rkVector,x);
			OF(lhs,y) -= OF(rkVector,y);
			OF(lhs,z) -= OF(rkVector,z);
			OF(lhs,w) -= OF(rkVector,w);

			return lhs;
		}

		REF(Vector4) Vector4::operator *= ( REF_CONST(Vector4) lhs, const Real fScalar )
		{
			OF(lhs,x) *= fScalar;
			OF(lhs,y) *= fScalar;
			OF(lhs,z) *= fScalar;
			OF(lhs,w) *= fScalar;
			return lhs;
		}

		REF(Vector4) Vector4::operator += ( REF_CONST(Vector4) lhs, const Real fScalar )
		{
			OF(lhs,x) += fScalar;
			OF(lhs,y) += fScalar;
			OF(lhs,z) += fScalar;
			OF(lhs,w) += fScalar;
			return lhs;
		}

		REF(Vector4) Vector4::operator -= ( REF_CONST(Vector4) lhs, const Real fScalar )
		{
			OF(lhs,x) -= fScalar;
			OF(lhs,y) -= fScalar;
			OF(lhs,z) -= fScalar;
			OF(lhs,w) -= fScalar;
			return lhs;
		}

		REF(Vector4) Vector4::operator *= ( REF_CONST(Vector4) lhs, REF_CONST(Vector4) rkVector )
		{
			OF(lhs,x) *= OF(rkVector,x);
			OF(lhs,y) *= OF(rkVector,y);
			OF(lhs,z) *= OF(rkVector,z);
			OF(lhs,w) *= OF(rkVector,w);

			return lhs;
		}

		REF(Vector4) Vector4::operator /= ( REF_CONST(Vector4) lhs, const Real fScalar )
		{
			assert( fScalar != 0.0 );

			Real fInv = 1.0f / fScalar;

			OF(lhs,x) *= fInv;
			OF(lhs,y) *= fInv;
			OF(lhs,z) *= fInv;
			OF(lhs,w) *= fInv;

			return lhs;
		}

		REF(Vector4) Vector4::operator /= ( REF_CONST(Vector4) lhs, REF_CONST(Vector4) rkVector )
		{
			OF(lhs,x) /= OF(rkVector,x);
			OF(lhs,y) /= OF(rkVector,y);
			OF(lhs,z) /= OF(rkVector,z);
			OF(lhs,w) /= OF(rkVector,w);

			return lhs;
		}
#endif

	}
}
