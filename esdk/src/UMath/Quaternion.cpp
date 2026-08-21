#include "stdafx.h"

#include "UQuaternion.h"
#include "UMath.h"
#include "UMatrix3.h"
#include "UVector3.h"


namespace UnE
{
	namespace Math
	{

	#ifndef DOTNET
		const Real Quaternion::msEpsilon = Real(1e-03);
		const Quaternion Quaternion::ZERO(0,0,0,0);
		const Quaternion Quaternion::IDENTITY(1,0,0,0);
	#endif

		//-----------------------------------------------------------------------
		void Quaternion::FromRotationMatrix (REF_CONST(Matrix3) kRot)
		{
			// Algorithm in Ken Shoemake's article in 1987 SIGGRAPH course notes
			// article "Quaternion Calculus and Fast Animation".

			Real fTrace = IDX( kRot,0,0 ) + IDX( kRot, 1,1) + IDX( kRot,2,2);
			Real fRoot;

			if ( fTrace > 0.0 )
			{
				// |w| > 1/2, may as well choose w > 1/2
				fRoot = UMath::Sqrt(fTrace + 1.0f);  // 2w
				w = 0.5f*fRoot;
				fRoot = 0.5f/fRoot;  // 1/(4w)
				x = (IDX(kRot,2,1)-IDX(kRot,1,2))*fRoot;
				y = (IDX(kRot,0,2)-IDX(kRot,2,0))*fRoot;
				z = (IDX(kRot,1,0)-IDX(kRot,0,1))*fRoot;
			}
			else
			{
				// |w| <= 1/2
				static size_t s_iNext[3] = { 1, 2, 0 };
				size_t i = 0;
				if ( IDX(kRot,1,1) > IDX(kRot,0,0) )
				{
					i = 1;
				}
				if ( IDX(kRot,2,2) > IDX( kRot,i,i) )
				{
					i = 2;
				}
				size_t j = s_iNext[i];
				size_t k = s_iNext[j];

				fRoot = UMath::Sqrt(IDX( kRot,i,i)-IDX( kRot,j,j)-IDX( kRot,k,k) + 1.0f);
#ifdef DOTNET
				switch(i)
				{
				case 0:
					x = 0.5f*fRoot;
					fRoot = 0.5f/fRoot;
					w = (IDX( kRot,k,j)-IDX( kRot,j,k))*fRoot;
					y = (IDX( kRot,j,i)+IDX( kRot,i,j))*fRoot;
					z = (IDX( kRot,k,i)+IDX( kRot,i,k))*fRoot;
					break;
				case 1:
					y = 0.5f*fRoot;
					fRoot = 0.5f/fRoot;
					w = (IDX( kRot,k,j)-IDX( kRot,j,k))*fRoot;
					z = (IDX( kRot,j,i)+IDX( kRot,i,j))*fRoot;
					x = (IDX( kRot,k,i)+IDX( kRot,i,k))*fRoot;
					break;
				case 2:
					z = 0.5f*fRoot;
					fRoot = 0.5f/fRoot;
					w = (IDX( kRot,k,j)-IDX( kRot,j,k))*fRoot;
					x = (IDX( kRot,j,i)+IDX( kRot,i,j))*fRoot;
					y = (IDX( kRot,k,i)+IDX( kRot,i,k))*fRoot;
					break;
				}
#else
				Real* apkQuat[3] = { &x, &y, &z };
				*apkQuat[i] = 0.5f*fRoot;
				fRoot = 0.5f/fRoot;
				w = (IDX( kRot,k,j)-IDX( kRot,j,k))*fRoot;
				*apkQuat[j] = (IDX( kRot,j,i)+IDX( kRot,i,j))*fRoot;
				*apkQuat[k] = (IDX( kRot,k,i)+IDX( kRot,i,k))*fRoot;
#endif
			}
		}
		//-----------------------------------------------------------------------
		void Quaternion::ToRotationMatrix (REF(Matrix3) kRot) CONSTF
		{
			Real fTx  = x+x;
			Real fTy  = y+y;
			Real fTz  = z+z;
			Real fTwx = fTx*w;
			Real fTwy = fTy*w;
			Real fTwz = fTz*w;
			Real fTxx = fTx*x;
			Real fTxy = fTy*x;
			Real fTxz = fTz*x;
			Real fTyy = fTy*y;
			Real fTyz = fTz*y;
			Real fTzz = fTz*z;

			IDX(kRot,0,0) = 1.0f-(fTyy+fTzz);
			IDX(kRot,0,1) = fTxy-fTwz;
			IDX(kRot,0,2) = fTxz+fTwy;
			IDX(kRot,1,0) = fTxy+fTwz;
			IDX(kRot,1,1) = 1.0f-(fTxx+fTzz);
			IDX(kRot,1,2) = fTyz-fTwx;
			IDX(kRot,2,0) = fTxz-fTwy;
			IDX(kRot,2,1) = fTyz+fTwx;
			IDX(kRot,2,2) = 1.0f-(fTxx+fTyy);
		}
		//-----------------------------------------------------------------------
		void Quaternion::FromAngleAxis (REF_CONST(Radian) rfAngle, REF_CONST(Vector3)  rkAxis)
		{
			// assert:  axis[] is unit length
			//
			// The quaternion representing the rotation is
			//   q = cos(A/2)+sin(A/2)*(x*i+y*j+z*k)

			INSTANCE(Radian) fHalfAngle = dnonlynew Radian( 0.5*rfAngle );
			Real fSin = UMath::Sin(fHalfAngle);
			w = UMath::Cos(fHalfAngle);
			x = fSin* OF( rkAxis,x);
			y = fSin* OF( rkAxis,y);
			z = fSin* OF( rkAxis,z);
		}
		//-----------------------------------------------------------------------
		void Quaternion::ToAngleAxis (REF(Degree) dAngle, REF(Vector3) rkAxis) CONSTF
		{
			INSTANCE(Radian) rAngle = dnonlynew Radian();
			ToAngleAxis ( rAngle, rkAxis );
			dAngle = dnonlynew Degree(rAngle);
		}
		//-----------------------------------------------------------------------
		void Quaternion::ToAngleAxis (REF(Radian) rfAngle, REF(Vector3)  rkAxis) CONSTF
		{
			// The quaternion representing the rotation is
			//   q = cos(A/2)+sin(A/2)*(x*i+y*j+z*k)

			Real fSqrLength = x*x+y*y+z*z;
			if ( fSqrLength > 0.0 )
			{
				rfAngle = 2.0*UMath::ACos(w);
				Real fInvLength = UMath::InvSqrt(fSqrLength);
				OF(rkAxis,x) = x*fInvLength;
				OF(rkAxis,y) = y*fInvLength;
				OF(rkAxis,z) = z*fInvLength;
			}
			else
			{
				// angle is 0 (mod 2*pi), so any axis will do
				rfAngle = dnonlynew Radian(0.0);
				OF(rkAxis,x) = 1.0;
				OF(rkAxis,y) = 0.0;
				OF(rkAxis,z) = 0.0;
			}
		}
		//-----------------------------------------------------------------------
#ifdef DOTNET
		void Quaternion::FromAxes (array<Vector3^>^ akAxis) CONSTF
		{
			INSTANCE(Matrix3) kRot = dnonlynew Matrix3();

			for (size_t iCol = 0; iCol < 3; iCol++)
			{
				kRot[0,iCol] = akAxis[iCol]->x;
				kRot[1,iCol] = akAxis[iCol]->y;
				kRot[2,iCol] = akAxis[iCol]->z;
			}

			FromRotationMatrix(kRot);
		}
#else
		void Quaternion::FromAxes (const Vector3* akAxis)
		{
			Matrix3 kRot;

			for (size_t iCol = 0; iCol < 3; iCol++)
			{
				kRot[0][iCol] = akAxis[iCol].x;
				kRot[1][iCol] = akAxis[iCol].y;
				kRot[2][iCol] = akAxis[iCol].z;
			}

			FromRotationMatrix(kRot);
		}
#endif
		//-----------------------------------------------------------------------
		void Quaternion::FromAxes (REF_CONST(Vector3)  xaxis, REF_CONST(Vector3)  yaxis, REF_CONST(Vector3)  zaxis)
		{
			INSTANCE(Matrix3) kRot = dnonlynew Matrix3();

			IDX(kRot,0,0) = OF(xaxis,x);
			IDX(kRot,1,0) = OF(xaxis,y);
			IDX(kRot,2,0) = OF(xaxis,z);

			IDX(kRot,0,1) = OF(yaxis,x);
			IDX(kRot,1,1) = OF(yaxis,y);
			IDX(kRot,2,1) = OF(yaxis,z);

			IDX(kRot,0,2) = OF(zaxis,x);
			IDX(kRot,1,2) = OF(zaxis,y);
			IDX(kRot,2,2) = OF(zaxis,z);

			FromRotationMatrix(kRot);

		}
		//-----------------------------------------------------------------------
#ifdef DOTNET
		void Quaternion::ToAxes (array<Vector3^>^ akAxis) CONSTF
		{
			INSTANCE(Matrix3) kRot = dnonlynew Matrix3();

			ToRotationMatrix(kRot);

			for (size_t iCol = 0; iCol < 3; iCol++)
			{
				(akAxis[iCol])->x = (kRot[0,iCol]);
				(akAxis[iCol])->y = (kRot[1,iCol]);
				(akAxis[iCol])->z = (kRot[2,iCol]);
			}
		}
#else
		void Quaternion::ToAxes (Vector3* akAxis) CONSTF
		{
			Matrix3 kRot;

			ToRotationMatrix(kRot);

			for (size_t iCol = 0; iCol < 3; iCol++)
			{
				akAxis[iCol].x = kRot[0][iCol];
				akAxis[iCol].y = kRot[1][iCol];
				akAxis[iCol].z = kRot[2][iCol];
			}
		}
#endif
		//-----------------------------------------------------------------------
		INSTANCE(Vector3) Quaternion::xAxis(void) CONSTF
		{
			//Real fTx  = 2.0*x;
			Real fTy  = 2.0f*y;
			Real fTz  = 2.0f*z;
			Real fTwy = fTy*w;
			Real fTwz = fTz*w;
			Real fTxy = fTy*x;
			Real fTxz = fTz*x;
			Real fTyy = fTy*y;
			Real fTzz = fTz*z;

			return dnonlynew Vector3(1.0f-(fTyy+fTzz), fTxy+fTwz, fTxz-fTwy);
		}
		//-----------------------------------------------------------------------
		INSTANCE(Vector3) Quaternion::yAxis(void) CONSTF
		{
			Real fTx  = 2.0f*x;
			Real fTy  = 2.0f*y;
			Real fTz  = 2.0f*z;
			Real fTwx = fTx*w;
			Real fTwz = fTz*w;
			Real fTxx = fTx*x;
			Real fTxy = fTy*x;
			Real fTyz = fTz*y;
			Real fTzz = fTz*z;

			return dnonlynew Vector3(fTxy-fTwz, 1.0f-(fTxx+fTzz), fTyz+fTwx);
		}
		//-----------------------------------------------------------------------
		INSTANCE(Vector3) Quaternion::zAxis(void) CONSTF
		{
			Real fTx  = 2.0f*x;
			Real fTy  = 2.0f*y;
			Real fTz  = 2.0f*z;
			Real fTwx = fTx*w;
			Real fTwy = fTy*w;
			Real fTxx = fTx*x;
			Real fTxz = fTz*x;
			Real fTyy = fTy*y;
			Real fTyz = fTz*y;

			return dnonlynew Vector3(fTxz+fTwy, fTyz-fTwx, 1.0f-(fTxx+fTyy));
		}
		//-----------------------------------------------------------------------
		void Quaternion::ToAxes (REF(Vector3)  xaxis, REF(Vector3)  yaxis, REF(Vector3)  zaxis) CONSTF
		{
			INSTANCE(Matrix3) kRot = dnonlynew Matrix3();

			ToRotationMatrix(kRot);

			OF(xaxis,x) = IDX(kRot,0,0);
			OF(xaxis,y) = IDX(kRot,1,0);
			OF(xaxis,z) = IDX(kRot,2,0);

			OF(yaxis,x) = IDX(kRot,0,1);
			OF(yaxis,y) = IDX(kRot,1,1);
			OF(yaxis,z) = IDX(kRot,2,1);

			OF(zaxis,x) = IDX(kRot,0,2);
			OF(zaxis,y) = IDX(kRot,1,2);
			OF(zaxis,z) = IDX(kRot,2,2);
		}

		
		//-----------------------------------------------------------------------
		Real Quaternion::Dot (REF_CONST(Quaternion) rkQ) CONSTF
		{
			return w*OF(rkQ,w)+x*OF(rkQ,x)+y*OF(rkQ,y)+z*OF(rkQ,z);
		}
		//-----------------------------------------------------------------------
		Real Quaternion::Norm () CONSTF
		{
			return w*w+x*x+y*y+z*z;
		}
		//-----------------------------------------------------------------------
		INSTANCE(Quaternion) Quaternion::Inverse () CONSTF
		{
			Real fNorm = w*w+x*x+y*y+z*z;
			if ( fNorm > 0.0 )
			{
				Real fInvNorm = 1.0f/fNorm;
				return dnonlynew Quaternion(w*fInvNorm,-x*fInvNorm,-y*fInvNorm,-z*fInvNorm);
			}
			else
			{
				// return an invalid result to flag the error
				return dnonlynew Quaternion(Quaternion::ZERO);
			}
		}
		//-----------------------------------------------------------------------
		INSTANCE(Quaternion) Quaternion::UnitInverse () CONSTF
		{
			// assert:  'this' is unit length
			return dnonlynew Quaternion(w,-x,-y,-z);
		}
		//-----------------------------------------------------------------------
		INSTANCE(Quaternion) Quaternion::Exp () CONSTF
		{
			// If q = A*(x*i+y*j+z*k) where (x,y,z) is unit length, then
			// exp(q) = cos(A)+sin(A)*(x*i+y*j+z*k).  If sin(A) is near zero,
			// use exp(q) = cos(A)+A*(x*i+y*j+z*k) since A/sin(A) has limit 1.

			INSTANCE(Radian) fAngle = dnonlynew Radian( UMath::Sqrt(x*x+y*y+z*z) );
			Real fSin = UMath::Sin(fAngle);

			INSTANCE(Quaternion) kResult;
			OF(kResult,w) = UMath::Cos(fAngle);

			if ( UMath::Abs(fSin) >= msEpsilon )
			{
				Real fCoeff = fSin/(OF(fAngle,valueRadians()));
				OF(kResult,x) = fCoeff*x;
				OF(kResult,y) = fCoeff*y;
				OF(kResult,z) = fCoeff*z;
			}
			else
			{
				OF(kResult,x) = x;
				OF(kResult,y) = y;
				OF(kResult,z) = z;
			}

			return kResult;
		}
		//-----------------------------------------------------------------------
		INSTANCE(Quaternion) Quaternion::Log () CONSTF
		{
			// If q = cos(A)+sin(A)*(x*i+y*j+z*k) where (x,y,z) is unit length, then
			// log(q) = A*(x*i+y*j+z*k).  If sin(A) is near zero, use log(q) =
			// sin(A)*(x*i+y*j+z*k) since sin(A)/A has limit 1.

			INSTANCE(Quaternion) kResult = dnonlynew Quaternion();
			OF(kResult,w) = 0.0;

			if ( UMath::Abs(w) < 1.0 )
			{
				INSTANCE(Radian) fAngle = dnonlynew Radian( UMath::ACos(w) );
				Real fSin = UMath::Sin(fAngle);
				if ( UMath::Abs(fSin) >= msEpsilon )
				{
					Real fCoeff = OF(fAngle,valueRadians())/fSin;
					OF(kResult,x) = fCoeff*x;
					OF(kResult,y) = fCoeff*y;
					OF(kResult,z) = fCoeff*z;
					return kResult;
				}
			}

			OF(kResult,x) = x;
			OF(kResult,y) = y;
			OF(kResult,z) = z;

			return kResult;
		}
		//-----------------------------------------------------------------------
		INSTANCE(Vector3) Quaternion::operator* (REF_CONST(Vector3)  v) CONSTF
		{
			// nVidia SDK implementation
			INSTANCE(Vector3) uv = dnonlynew Vector3();
			INSTANCE(Vector3) uuv = dnonlynew Vector3();
			INSTANCE(Vector3) qvec = dnonlynew Vector3(x, y, z);
			uv =  OF( qvec, crossProduct(v));
			uuv = OF( qvec,crossProduct(uv));
			uv *= (2.0f * w);
			uuv *= 2.0f;

			return v + uv + uuv;

		}
		//-----------------------------------------------------------------------
		bool Quaternion::equals(REF_CONST(Quaternion) rhs, REF_CONST(Radian) tolerance) CONSTF
		{
			Real fCos = Dot(rhs);
			INSTANCE(Radian) angle = UMath::ACos(fCos);

			return (UMath::Abs(OF(angle,valueRadians())) <= OF(tolerance,valueRadians()))
				|| UMath::RealEqual(OF(angle,valueRadians()), UMath::PI, OF(tolerance,valueRadians()));


		}
		//-----------------------------------------------------------------------
		INSTANCE(Quaternion) Quaternion::Slerp (Real fT, REF_CONST(Quaternion) rkP, REF_CONST(Quaternion) rkQ, bool shortestPath)
		{
			Real fCos = OF( rkP, Dot(rkQ));
			INSTANCE(Quaternion) rkT = dnonlynew Quaternion();

			// Do we need to invert rotation?
			if (fCos < 0.0f && shortestPath)
			{
				fCos = -fCos;				rkT = -rkQ;
			}
			else
			{
				rkT = rkQ;
			}

			if (UMath::Abs(fCos) < 1 - msEpsilon)
			{
				// Standard case (slerp)
				Real fSin = UMath::Sqrt(1 - UMath::Sqr(fCos));
				INSTANCE(Radian) fAngle = UMath::ATan2(fSin, fCos);
				Real fInvSin = 1.0f / fSin;
				Real fCoeff0 = fInvSin * UMath::Sin( fAngle * (1.0f - fT));
				Real fCoeff1 = fInvSin * UMath::Sin(fT * fAngle);
				return ( fCoeff0 * rkP ) + ( fCoeff1 * rkT );
			}
			else
			{
				// There are two situations:
				// 1. "rkP" and "rkQ" are very close (fCos ~= +1), so we can do a linear
				//    interpolation safely.
				// 2. "rkP" and "rkQ" are almost inverse of each other (fCos ~= -1), there
				//    are an infinite number of possibilities interpolation. but we haven't
				//    have method to fix this case, so just use linear interpolation here.
				INSTANCE(Quaternion) t = ((1.0f - fT) * rkP) + ( rkT * fT  );
				// taking the complement requires renormalisation
				OF( t, normalise() );
				return t;
			}
		}
		//-----------------------------------------------------------------------
		INSTANCE(Quaternion) Quaternion::SlerpExtraSpins (Real fT,
			REF_CONST(Quaternion) rkP, REF_CONST(Quaternion) rkQ, int iExtraSpins)
		{
			Real fCos = OF(rkP,Dot(rkQ));
			INSTANCE(Radian) fAngle = dnonlynew Radian ( UMath::ACos(fCos) );

			if ( UMath::Abs(OF(fAngle,valueRadians())) < msEpsilon )
				return rkP;

			Real fSin = UMath::Sin(fAngle);
			INSTANCE(Radian)  fPhase = dnonlynew Radian ( UMath::PI*iExtraSpins*fT );
			Real fInvSin = 1.0f/fSin;
			Real fCoeff0 = UMath::Sin((1.0f-fT)*fAngle - fPhase)*fInvSin;
			Real fCoeff1 = UMath::Sin(fAngle*fT + fPhase)*fInvSin;
			return fCoeff0*rkP + fCoeff1*rkQ;
		}
		//-----------------------------------------------------------------------
		void Quaternion::Intermediate (REF_CONST(Quaternion) rkQ0,
			REF_CONST(Quaternion) rkQ1, REF_CONST(Quaternion) rkQ2,
			REF(Quaternion) rkA, REF(Quaternion) rkB)
		{
			// assert:  q0, q1, q2 are unit quaternions

			INSTANCE(Quaternion) kQ0inv = OF( rkQ0, UnitInverse());
			INSTANCE(Quaternion) kQ1inv = OF( rkQ1, UnitInverse());
			INSTANCE(Quaternion) rkP0 = kQ0inv*rkQ1;
			INSTANCE(Quaternion) rkP1 = kQ1inv*rkQ2;
			INSTANCE(Quaternion) kArg = 0.25*( OF( rkP0, Log()) - OF( rkP1,Log()));
			INSTANCE(Quaternion) kMinusArg = -kArg;

			rkA = rkQ1*OF(kArg,Exp());
			rkB = rkQ1*OF(kMinusArg,Exp());
		}
		//-----------------------------------------------------------------------
		INSTANCE(Quaternion) Quaternion::Squad (Real fT,
			REF_CONST(Quaternion) rkP, REF_CONST(Quaternion) rkA,
			REF_CONST(Quaternion) rkB, REF_CONST(Quaternion) rkQ, bool shortestPath)
		{
			Real fSlerpT = 2.0f*fT*(1.0f-fT);
			INSTANCE(Quaternion) kSlerpP = Slerp(fT, rkP, rkQ, shortestPath);
			INSTANCE(Quaternion) kSlerpQ = Slerp(fT, rkA, rkB);
			return Slerp(fSlerpT, kSlerpP ,kSlerpQ);
		}
		//-----------------------------------------------------------------------
		Real Quaternion::normalise(void)
		{
			Real len = Norm();
			Real factor = 1.0f / UMath::Sqrt(len);

			operator=(THIS_OBJ * factor);
			return len;
		}
		//-----------------------------------------------------------------------
		INSTANCE(Radian) Quaternion::getRoll(bool reprojectAxis) CONSTF
		{
			if (reprojectAxis)
			{
				// roll = atan2(localx.y, localx.x)
				// pick parts of xAxis() implementation that we need
	//			Real fTx  = 2.0*x;
				Real fTy  = 2.0f*y;
				Real fTz  = 2.0f*z;
				Real fTwz = fTz*w;
				Real fTxy = fTy*x;
				Real fTyy = fTy*y;
				Real fTzz = fTz*z;

				// Vector3(1.0-(fTyy+fTzz), fTxy+fTwz, fTxz-fTwy);

				return dnonlynew Radian(UMath::ATan2(fTxy+fTwz, 1.0f-(fTyy+fTzz)));

			}
			else
			{
				return dnonlynew Radian(UMath::ATan2(2*(x*y + w*z), w*w + x*x - y*y - z*z));
			}
		}
		//-----------------------------------------------------------------------
		INSTANCE(Radian) Quaternion::getPitch(bool reprojectAxis) CONSTF
		{
			if (reprojectAxis)
			{
				// pitch = atan2(localy.z, localy.y)
				// pick parts of yAxis() implementation that we need
				Real fTx  = 2.0f*x;
	//			Real fTy  = 2.0f*y;
				Real fTz  = 2.0f*z;
				Real fTwx = fTx*w;
				Real fTxx = fTx*x;
				Real fTyz = fTz*y;
				Real fTzz = fTz*z;

				// Vector3(fTxy-fTwz, 1.0-(fTxx+fTzz), fTyz+fTwx);
				return dnonlynew Radian(UMath::ATan2(fTyz+fTwx, 1.0f-(fTxx+fTzz)));
			}
			else
			{
				// internal version
				return dnonlynew Radian(UMath::ATan2(2*(y*z + w*x), w*w - x*x - y*y + z*z));
			}
		}
		//-----------------------------------------------------------------------
		INSTANCE(Radian) Quaternion::getYaw(bool reprojectAxis) CONSTF
		{
			if (reprojectAxis)
			{
				// yaw = atan2(localz.x, localz.z)
				// pick parts of zAxis() implementation that we need
				Real fTx  = 2.0f*x;
				Real fTy  = 2.0f*y;
				Real fTz  = 2.0f*z;
				Real fTwy = fTy*w;
				Real fTxx = fTx*x;
				Real fTxz = fTz*x;
				Real fTyy = fTy*y;

				// Vector3(fTxz+fTwy, fTyz-fTwx, 1.0-(fTxx+fTyy));

				return dnonlynew Radian(UMath::ATan2(fTxz+fTwy, 1.0f-(fTxx+fTyy)));

			}
			else
			{
				// internal version
				return dnonlynew Radian(UMath::ASin(-2*(x*z - w*y)));
			}
		}
		//-----------------------------------------------------------------------
		INSTANCE(Quaternion) Quaternion::nlerp(Real fT, REF_CONST(Quaternion) rkP,
			REF_CONST(Quaternion) rkQ, bool shortestPath)
		{
			INSTANCE(Quaternion) result = dnonlynew Quaternion();
			Real fCos = OF(rkP,Dot(rkQ));
			if (fCos < 0.0f && shortestPath)
			{
				result = rkP + fT * ((-rkQ) - rkP);
			}
			else
			{
				result = rkP + fT * (rkQ - rkP);
			}
			OF(result,normalise());
			return result;
		}
		
		
		//////////////////////////////////////////////////////////////////////////
#ifdef DOTNET
		//-----------------------------------------------------------------------
		INSTANCE(Quaternion) Quaternion::operator+ (REF_CONST(Quaternion) lkQ, REF_CONST(Quaternion) rkQ) CONSTF
		{
			return dnonlynew Quaternion(OF(lkQ,w)+OF(rkQ,w) ,OF(lkQ,x)+OF(rkQ,x) ,OF(lkQ,y)+OF(rkQ,y) ,OF(lkQ,z) + OF(rkQ,z));
		}
		//-----------------------------------------------------------------------
		INSTANCE(Quaternion) Quaternion::operator- (REF_CONST(Quaternion) lkQ, REF_CONST(Quaternion) rkQ) CONSTF
		{
			return dnonlynew Quaternion(OF(lkQ,w)-OF(rkQ,w) ,OF(lkQ,x)-OF(rkQ,x) ,OF(lkQ,y)-OF(rkQ,y) ,OF(lkQ,z)-OF(rkQ,z));
		}
		//-----------------------------------------------------------------------
		INSTANCE(Quaternion) Quaternion::operator* (REF_CONST(Quaternion) lkQ, REF_CONST(Quaternion) rkQ) CONSTF
		{
			// NOTE:  Multiplication is not generally commutative, so in most
			// cases p*q != q*p.

			return dnonlynew Quaternion
				(
				OF(lkQ,w) * OF(rkQ,w) - OF(lkQ,x) * OF(rkQ,x) - OF(lkQ,y) * OF(rkQ,y) - OF(lkQ,z) * OF(rkQ,z),
				OF(lkQ,w) * OF(rkQ,x) + OF(lkQ,x) * OF(rkQ,w) + OF(lkQ,y) * OF(rkQ,z) - OF(lkQ,z) * OF(rkQ,y),
				OF(lkQ,w) * OF(rkQ,y) + OF(lkQ,y) * OF(rkQ,w) + OF(lkQ,z) * OF(rkQ,x) - OF(lkQ,x) * OF(rkQ,z),
				OF(lkQ,w) * OF(rkQ,z) + OF(lkQ,z) * OF(rkQ,w) + OF(lkQ,x) * OF(rkQ,y) - OF(lkQ,y) * OF(rkQ,x)
				);
		}
		//-----------------------------------------------------------------------
		INSTANCE(Quaternion) Quaternion::operator* (REF_CONST(Quaternion) lkQ, Real fScalar) CONSTF
		{
			return dnonlynew Quaternion(fScalar*OF(lkQ,w) ,fScalar*OF(lkQ,x) ,fScalar*OF(lkQ,y), fScalar*OF(lkQ,z));
		}
		//-----------------------------------------------------------------------
		INSTANCE(Quaternion) Quaternion::operator* (Real fScalar, REF_CONST(Quaternion) rkQ)
		{
			return dnonlynew Quaternion(fScalar*OF(rkQ,w) ,fScalar*OF(rkQ,x) ,fScalar*OF(rkQ,y), fScalar*OF(rkQ,z));
		}
		//-----------------------------------------------------------------------
		INSTANCE(Quaternion) Quaternion::operator- (REF_CONST(Quaternion) lkQ) CONSTF
		{
			return dnonlynew Quaternion(-OF(lkQ,w),-OF(lkQ,x),-OF(lkQ,y),-OF(lkQ,z));
		}
		//-----------------------------------------------------------------------
		bool Quaternion::operator!=( REF_CONST(Quaternion) lkQ, REF_CONST(Quaternion) rhs ) CONSTF
		{
			return !operator==(lkQ, rhs);
		}
		//-----------------------------------------------------------------------
		bool Quaternion::operator==( REF_CONST(Quaternion) lkQ, REF_CONST(Quaternion) rhs ) CONSTF
		{
			return (OF(rhs,x) == OF(lkQ,x)) && (OF(rhs,y) == OF(lkQ,y)) && (OF(rhs,z) == OF(lkQ,z)) && (OF(rhs,w) == OF(lkQ,w));
		}
#else
		//-----------------------------------------------------------------------
		INSTANCE(Quaternion) Quaternion::operator+ (REF_CONST(Quaternion) rkQ) CONSTF
		{
			return dnonlynew Quaternion(w+OF(rkQ,w) ,x+OF(rkQ,x) ,y+OF(rkQ,y) ,z + OF(rkQ,z));
		}
		//-----------------------------------------------------------------------
		INSTANCE(Quaternion) Quaternion::operator- (REF_CONST(Quaternion) rkQ) CONSTF
		{
			return dnonlynew Quaternion(w-OF(rkQ,w) , x-OF(rkQ,x) ,y-OF(rkQ,y) ,z-OF(rkQ,z));
		}
		//-----------------------------------------------------------------------
		INSTANCE(Quaternion) Quaternion::operator* (REF_CONST(Quaternion) rkQ) CONSTF
		{
			// NOTE:  Multiplication is not generally commutative, so in most
			// cases p*q != q*p.

			return dnonlynew Quaternion
				(
				w * OF(rkQ,w) - x * OF(rkQ,x) - y * OF(rkQ,y) - z * OF(rkQ,z),
				w * OF(rkQ,x) + x * OF(rkQ,w) + y * OF(rkQ,z) - z * OF(rkQ,y),
				w * OF(rkQ,y) + y * OF(rkQ,w) + z * OF(rkQ,x) - x * OF(rkQ,z),
				w * OF(rkQ,z) + z * OF(rkQ,w) + x * OF(rkQ,y) - y * OF(rkQ,x)
				);
		}
		//-----------------------------------------------------------------------
		INSTANCE(Quaternion) Quaternion::operator* (Real fScalar) CONSTF
		{
			return dnonlynew Quaternion(fScalar*w,fScalar*x,fScalar*y,fScalar*z);
		}
		//-----------------------------------------------------------------------
		INSTANCE(Quaternion) operator* (Real fScalar, REF_CONST(Quaternion) rkQ)
		{
			return dnonlynew Quaternion(fScalar*OF(rkQ,w) ,fScalar*OF(rkQ,x) ,fScalar*OF(rkQ,y),
				fScalar*OF(rkQ,z));
		}
		//-----------------------------------------------------------------------
		INSTANCE(Quaternion) Quaternion::operator- () CONSTF
		{
			return dnonlynew Quaternion(-w,-x,-y,-z);
		}
		//-----------------------------------------------------------------------
		bool Quaternion::operator!=( REF_CONST(Quaternion) rhs ) CONSTF
		{
			return !operator==(rhs);
		}
		//-----------------------------------------------------------------------
		bool Quaternion::operator==( REF_CONST(Quaternion) rhs ) CONSTF
		{
			return (OF(rhs,x) == x) && (OF(rhs,y) == y) && (OF(rhs,z) == z) && (OF(rhs,w) == w);
		}

#endif

	}
}
