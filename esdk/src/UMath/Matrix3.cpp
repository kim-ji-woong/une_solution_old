
#include "stdafx.h"

#include "UMatrix3.h"
#include "UMath.h"

// Adapted from Matrix math by Wild Magic http://www.geometrictools.com/

namespace UnE
{
	namespace Math
	{
#ifndef DOTNET
	const Real Matrix3::EPSILON = Real(1e-06);
	const Matrix3 Matrix3::ZERO(0,0,0,0,0,0,0,0,0);
	const Matrix3 Matrix3::IDENTITY(1,0,0,0,1,0,0,0,1);
	const Real Matrix3::msSvdEpsilon = Real(1e-04);
	const unsigned int Matrix3::msSvdMaxIterations = 32;

#endif
	//-----------------------------------------------------------------------
	INSTANCE(Vector3) Matrix3::GetColumn (size_t iCol) CONSTF
	{
		assert( iCol < 3 );
		return dnonlynew Vector3(IDX(m, 0, (int)iCol), IDX(m ,1, (int)iCol), IDX( m, 2, (int)iCol));
	}

	//-----------------------------------------------------------------------
	void Matrix3::SetColumn(size_t iCol, REF_CONST(Vector3) vec)
	{
		assert( iCol < 3 );
		IDX(m,0,iCol) = OF(vec,x);
		IDX(m,1,iCol) = OF(vec,y);
		IDX(m,2,iCol) = OF(vec,z);
	}

	//-----------------------------------------------------------------------
	void Matrix3::FromAxes(REF_CONST(Vector3) xAxis, REF_CONST(Vector3) yAxis, REF_CONST(Vector3) zAxis)
	{
		SetColumn(0,xAxis);
		SetColumn(1,yAxis);
		SetColumn(2,zAxis);
	}



#ifdef DOTNET
	bool Matrix3::operator== (REF_CONST(Matrix3) lMatrix,System::Object^ obj) CONSTF
	{
		return obj == lMatrix;

	}

	//-----------------------------------------------------------------------
	bool Matrix3::operator== (REF_CONST(Matrix3) lMatrix,REF_CONST(Matrix3) rkMatrix) CONSTF
	{
		System::Object^ obj1 = (System::Object^)nullptr;
		System::Object^ obj2 = (System::Object^)nullptr;
		if( lMatrix == obj1 )
			return false;
		if( rkMatrix == obj2)
			return false;


		for (size_t iRow = 0; iRow < 3; iRow++)
		{
			for (size_t iCol = 0; iCol < 3; iCol++)
			{
				if ( IDX( lMatrix, iRow,iCol) != IDX( OF( rkMatrix,m), iRow,iCol) )
					return false;
			}
		}

		return true;
	}
	//-----------------------------------------------------------------------
	INSTANCE(Matrix3) Matrix3::operator+ (REF_CONST(Matrix3) lMatrix,REF_CONST(Matrix3) rkMatrix) CONSTF
	{
		INSTANCE(Matrix3) kSum = dnonlynew Matrix3();
		for (size_t iRow = 0; iRow < 3; iRow++)
		{
			for (size_t iCol = 0; iCol < 3; iCol++)
			{
				IDX( kSum , iRow,iCol) = IDX( lMatrix, iRow,iCol) + IDX( rkMatrix, iRow,iCol);
			}
		}
		return kSum;
	}
	//-----------------------------------------------------------------------
	INSTANCE(Matrix3) Matrix3::operator- (REF_CONST(Matrix3) lMatrix,REF_CONST(Matrix3) rkMatrix) CONSTF
	{
		INSTANCE(Matrix3) kDiff = dnonlynew Matrix3();

		for (size_t iRow = 0; iRow < 3; iRow++)
		{
			for (size_t iCol = 0; iCol < 3; iCol++)
			{
				IDX( OF(kDiff,m), iRow,iCol) = IDX( lMatrix , iRow, iCol) - IDX( OF(rkMatrix,m), iRow, iCol);
			}
		}
		return kDiff;
	}
	//-----------------------------------------------------------------------
	INSTANCE(Matrix3) Matrix3::operator* (REF_CONST(Matrix3) lMatrix,REF_CONST(Matrix3) rkMatrix) CONSTF
	{
		INSTANCE(Matrix3) kProd = dnonlynew Matrix3();
		for (size_t iRow = 0; iRow < 3; iRow++)
		{
			for (size_t iCol = 0; iCol < 3; iCol++)
			{
				IDX( OF( kProd, m), iRow,iCol) =
					IDX(lMatrix, iRow,0) * IDX( OF(rkMatrix,m),0,iCol) +
					IDX(lMatrix, iRow,1) * IDX( OF(rkMatrix,m),1,iCol) +
					IDX(lMatrix, iRow,2) * IDX( OF(rkMatrix,m),2,iCol);
			}
		}
		return kProd;
	}
	//-----------------------------------------------------------------------
	INSTANCE(Vector3) Matrix3::operator* (REF_CONST(Matrix3) rMatrix, REF_CONST(Vector3) rkPoint) CONSTF
	{
		INSTANCE(Vector3) kProd = dnonlynew Vector3();

		for (size_t iRow = 0; iRow < 3; iRow++)
		{
			kProd[iRow] =
				IDX(rMatrix,iRow,0) * rkPoint[0] +
				IDX(rMatrix,iRow,1) * rkPoint[1] +
				IDX(rMatrix,iRow,2) * rkPoint[2];
		}
		return kProd;
	}
	//-----------------------------------------------------------------------
	INSTANCE(Vector3) Matrix3::operator* (REF_CONST(Vector3) rkPoint, REF_CONST(Matrix3) rkMatrix)
	{
		INSTANCE(Vector3) kProd = dnonlynew Vector3();
		for (size_t iRow = 0; iRow < 3; iRow++)
		{
			kProd[iRow] =
				rkPoint[0]* IDX( rkMatrix, 0,iRow) +
				rkPoint[1]* IDX( rkMatrix, 1,iRow) +
				rkPoint[2]* IDX( rkMatrix, 2,iRow);
		}
		return kProd;
	}
	//-----------------------------------------------------------------------
	INSTANCE(Matrix3) Matrix3::operator- (REF(Matrix3) rMatrix) CONSTF
	{
		INSTANCE(Matrix3) kNeg = dnonlynew Matrix3();
		for (size_t iRow = 0; iRow < 3; iRow++)
		{
			for (size_t iCol = 0; iCol < 3; iCol++)
				IDX( kNeg, iRow, iCol) = - IDX(rMatrix,iRow,iCol);
		} 
		return rMatrix;
	}
	//-----------------------------------------------------------------------
	INSTANCE(Matrix3) Matrix3::operator* (REF_CONST(Matrix3) rMatrix, Real fScalar) CONSTF
	{
		INSTANCE(Matrix3) kProd = dnonlynew Matrix3();
		for (size_t iRow = 0; iRow < 3; iRow++)
		{
			for (size_t iCol = 0; iCol < 3; iCol++)
				IDX( kProd, iRow, iCol) = fScalar * IDX(rMatrix,iRow,iCol);
		}
		return kProd;
	}
	//-----------------------------------------------------------------------
	INSTANCE(Matrix3) Matrix3::operator* (Real fScalar, REF_CONST(Matrix3) rkMatrix)
	{
		INSTANCE(Matrix3) kProd = dnonlynew Matrix3();
		for (size_t iRow = 0; iRow < 3; iRow++)
		{
			for (size_t iCol = 0; iCol < 3; iCol++)
				IDX( kProd, iRow, iCol) = fScalar * IDX(rkMatrix, iRow,iCol);
		}
		return kProd;
	}
#else
	
	//-----------------------------------------------------------------------
	bool Matrix3::operator== (REF_CONST(Matrix3) rkMatrix) CONSTF
	{
		for (size_t iRow = 0; iRow < 3; iRow++)
		{
			for (size_t iCol = 0; iCol < 3; iCol++)
			{
				if ( IDX( m, iRow,iCol) != IDX( OF( rkMatrix,m), iRow,iCol) )
					return false;
			}
		}

		return true;
	}
	//-----------------------------------------------------------------------
	INSTANCE(Matrix3) Matrix3::operator+ (REF_CONST(Matrix3) rkMatrix) CONSTF
	{
		INSTANCE(Matrix3) kSum = dnonlynew Matrix3();
		for (size_t iRow = 0; iRow < 3; iRow++)
		{
			for (size_t iCol = 0; iCol < 3; iCol++)
			{
				IDX( kSum , iRow,iCol) = IDX( m, iRow,iCol) + IDX( rkMatrix, iRow,iCol);
			}
		}
		return kSum;
	}
	//-----------------------------------------------------------------------
	INSTANCE(Matrix3) Matrix3::operator- (REF_CONST(Matrix3) rkMatrix) CONSTF
	{
		INSTANCE(Matrix3) kDiff = dnonlynew Matrix3();

		for (size_t iRow = 0; iRow < 3; iRow++)
		{
			for (size_t iCol = 0; iCol < 3; iCol++)
			{
				IDX( OF(kDiff,m), iRow,iCol) = IDX( m , iRow, iCol) - IDX( OF(rkMatrix,m), iRow, iCol);
			}
		}
		return kDiff;
	}
	//-----------------------------------------------------------------------
	INSTANCE(Matrix3) Matrix3::operator* (REF_CONST(Matrix3) rkMatrix) CONSTF
	{
		INSTANCE(Matrix3) kProd = dnonlynew Matrix3();
		for (size_t iRow = 0; iRow < 3; iRow++)
		{
			for (size_t iCol = 0; iCol < 3; iCol++)
			{
				IDX( OF( kProd, m), iRow,iCol) =
					IDX(m, iRow,0) * IDX( OF(rkMatrix,m),0,iCol) +
					IDX(m, iRow,1) * IDX( OF(rkMatrix,m),1,iCol) +
					IDX(m, iRow,2) * IDX( OF(rkMatrix,m),2,iCol);
			}
		}
		return kProd;
	}
	//-----------------------------------------------------------------------
	INSTANCE(Vector3) Matrix3::operator* (REF_CONST(Vector3) rkPoint) CONSTF
	{
		INSTANCE(Vector3) kProd = dnonlynew Vector3();

		for (size_t iRow = 0; iRow < 3; iRow++)
		{
			kProd[iRow] =
				IDX(m,iRow,0) * rkPoint[0] +
				IDX(m,iRow,1) * rkPoint[1] +
				IDX(m,iRow,2) * rkPoint[2];
		}
		return kProd;
	}
	//-----------------------------------------------------------------------
	INSTANCE(Vector3) operator* (REF_CONST(Vector3) rkPoint, REF_CONST(Matrix3) rkMatrix)
	{
		INSTANCE(Vector3) kProd = dnonlynew Vector3();
		for (size_t iRow = 0; iRow < 3; iRow++)
		{
			kProd[iRow] =
				rkPoint[0]* IDX( rkMatrix, 0,iRow) +
				rkPoint[1]* IDX( rkMatrix, 1,iRow) +
				rkPoint[2]* IDX( rkMatrix, 2,iRow);
		}
		return kProd;
	}
	//-----------------------------------------------------------------------
	INSTANCE(Matrix3) Matrix3::operator- () CONSTF
	{
		INSTANCE(Matrix3) kNeg = dnonlynew Matrix3();
		for (size_t iRow = 0; iRow < 3; iRow++)
		{
			for (size_t iCol = 0; iCol < 3; iCol++)
				IDX( kNeg, iRow, iCol) = - IDX(m,iRow,iCol);
		} 
		return kNeg;
	}
	//-----------------------------------------------------------------------
	INSTANCE(Matrix3) Matrix3::operator* (Real fScalar) CONSTF
	{
		INSTANCE(Matrix3) kProd = dnonlynew Matrix3();
		for (size_t iRow = 0; iRow < 3; iRow++)
		{
			for (size_t iCol = 0; iCol < 3; iCol++)
				IDX( kProd, iRow, iCol) = fScalar * IDX(m,iRow,iCol);
		}
		return kProd;
	}
	//-----------------------------------------------------------------------
	INSTANCE(Matrix3) operator* (Real fScalar, REF_CONST(Matrix3) rkMatrix)
	{
		INSTANCE(Matrix3) kProd = dnonlynew Matrix3();
		for (size_t iRow = 0; iRow < 3; iRow++)
		{
			for (size_t iCol = 0; iCol < 3; iCol++)
				IDX( kProd, iRow, iCol) = fScalar * IDX(rkMatrix, iRow,iCol);
		}
		return kProd;
	}
	
#endif

	//-----------------------------------------------------------------------
	INSTANCE(Matrix3) Matrix3::Transpose () CONSTF
	{
		INSTANCE(Matrix3) kTranspose = dnonlynew Matrix3();
		for (size_t iRow = 0; iRow < 3; iRow++)
		{
			for (size_t iCol = 0; iCol < 3; iCol++)
				IDX( kTranspose, iRow, iCol) = IDX( m, iCol, iRow);
		}
		return kTranspose;
	}
	//-----------------------------------------------------------------------
	bool Matrix3::Inverse (REF(Matrix3) rkInverse, Real fTolerance) CONSTF
	{
		// Invert a 3x3 using cofactors.  This is about 8 times faster than
		// the Numerical Recipes code which uses Gaussian elimination.

		IDX(rkInverse,0,0) = IDX(m,1,1)*IDX(m,2,2)-IDX(m,1,2)* IDX(m,2,1);
		IDX(rkInverse,0,1) = IDX(m,0,2)*IDX(m,2,1)-IDX(m,0,1)* IDX(m,2,2);
		IDX(rkInverse,0,2) = IDX(m,0,1)*IDX(m,1,2)-IDX(m,0,2)* IDX(m,1,1);

		IDX(rkInverse,1,0) = IDX(m,1,2)*IDX(m,2,0)-IDX(m,1,0)* IDX(m,2,2);
		IDX(rkInverse,1,1) = IDX(m,0,0)*IDX(m,2,2)-IDX(m,0,2)* IDX(m,2,0);
		IDX(rkInverse,1,2) = IDX(m,0,2)*IDX(m,1,0)-IDX(m,0,0)* IDX(m,1,2);

		IDX(rkInverse,2,0) = IDX(m,1,0)*IDX(m,2,1)-IDX(m,1,1)* IDX(m,2,0);
		IDX(rkInverse,2,1) = IDX(m,0,1)*IDX(m,2,0)-IDX(m,0,0)* IDX(m,2,1);
		IDX(rkInverse,2,2) = IDX(m,0,0)*IDX(m,1,1)-IDX(m,0,1)* IDX(m,1,0);

		Real fDet =
			IDX( m, 0, 0) * IDX( rkInverse,0,0) +
			IDX( m, 0, 1) * IDX( rkInverse,1,0) +
			IDX( m, 0, 2) * IDX( rkInverse,2,0);

		if ( UMath::Abs(fDet) <= fTolerance )
			return false;

		Real fInvDet = 1.0f/fDet;
		for (size_t iRow = 0; iRow < 3; iRow++)
		{
			for (size_t iCol = 0; iCol < 3; iCol++)
				IDX( rkInverse, iRow,iCol) *= fInvDet;
		}

		return true;
	}
	//-----------------------------------------------------------------------
	INSTANCE(Matrix3) Matrix3::Inverse (Real fTolerance) CONSTF
	{
		INSTANCE(Matrix3) kInverse = dnonlynew Matrix3(Matrix3::ZERO);
		Inverse(kInverse,fTolerance);
		return kInverse;
	}
	//-----------------------------------------------------------------------
	Real Matrix3::Determinant () CONSTF
	{
		Real fCofactor00 = IDX(m,1,1)*IDX(m,2,2) -
			IDX(m,1,2)*IDX(m,2,1);
		Real fCofactor10 = IDX(m,1,2)*IDX(m,2,0) -
			IDX(m,1,0)*IDX(m,2,2);
		Real fCofactor20 = IDX(m,1,0)*IDX(m,2,1) -
			IDX(m,1,1)*IDX(m,2,0);

		Real fDet =
			IDX(m,0,0)*fCofactor00 +
			IDX(m,0,1)*fCofactor10 +
			IDX(m,0,2)*fCofactor20;

		return fDet;
	}
	//-----------------------------------------------------------------------
	void Matrix3::Bidiagonalize (REF(Matrix3) kA, REF(Matrix3) kL, REF(Matrix3) kR)
	{
		Real afV[3], afW[3];
		Real fLength, fSign, fT1, fInvT1, fT2;
		bool bIdentity;

		// map first column to (*,0,0)
		fLength = UMath::Sqrt(IDX(kA,0,0)*IDX(kA,0,0) + IDX(kA,1,0)*IDX(kA,1,0) +
			IDX(kA,2,0)*IDX(kA,2,0));
		if ( fLength > 0.0 )
		{
			fSign = (IDX(kA,0,0) > 0.0f ? 1.0f : -1.0f);
			fT1 = IDX(kA,0,0) + fSign*fLength;
			fInvT1 = 1.0f/fT1;
			afV[1] = IDX(kA,1,0)*fInvT1;
			afV[2] = IDX(kA,2,0)*fInvT1;

			fT2 = -2.0f/(1.0f+afV[1]*afV[1]+afV[2]*afV[2]);
			afW[0] = fT2*(IDX(kA,0,0)+IDX(kA,1,0)*afV[1]+IDX(kA,2,0)*afV[2]);
			afW[1] = fT2*(IDX(kA,0,1)+IDX(kA,1,1)*afV[1]+IDX(kA,2,1)*afV[2]);
			afW[2] = fT2*(IDX(kA,0,2)+IDX(kA,1,2)*afV[1]+IDX(kA,2,2)*afV[2]);
			IDX(kA,0,0) += afW[0];
			IDX(kA,0,1) += afW[1];
			IDX(kA,0,2) += afW[2];
			IDX(kA,1,1) += afV[1]*afW[1];
			IDX(kA,1,2) += afV[1]*afW[2];
			IDX(kA,2,1) += afV[2]*afW[1];
			IDX(kA,2,2) += afV[2]*afW[2];

			IDX(kL,0,0) = 1.0f+fT2;
			IDX(kL,0,1) = fT2*afV[1];
			IDX(kL,1,0) = fT2*afV[1];
			IDX(kL,0,2) = fT2*afV[2];
			IDX(kL,2,0) = fT2*afV[2];
			IDX(kL,1,1) = 1.0f+fT2*afV[1]*afV[1];
			IDX(kL,1,2) = fT2*afV[1]*afV[2];
			IDX(kL,2,1) = fT2*afV[1]*afV[2];
			IDX(kL,2,2) = 1.0f+fT2*afV[2]*afV[2];
			bIdentity = false;
		}
		else
		{
			kL = dnonlynew Matrix3(Matrix3::IDENTITY);
			bIdentity = true;
		}

		// map first row to (*,*,0)
		fLength = UMath::Sqrt(IDX(kA,0,1)*IDX(kA,0,1)+IDX(kA,0,2)*IDX(kA,0,2));
		if ( fLength > 0.0 )
		{
			fSign = (IDX(kA,0,1) > 0.0f ? 1.0f : -1.0f);
			fT1 = IDX(kA,0,1) + fSign*fLength;
			afV[2] = IDX(kA,0,2)/fT1;

			fT2 = -2.0f/(1.0f+afV[2]*afV[2]);
			afW[0] = fT2*(IDX(kA,0,1)+IDX(kA,0,2)*afV[2]);
			afW[1] = fT2*(IDX(kA,1,1)+IDX(kA,1,2)*afV[2]);
			afW[2] = fT2*(IDX(kA,2,1)+IDX(kA,2,2)*afV[2]);
			IDX(kA,0,1) += afW[0];
			IDX(kA,1,1) += afW[1];
			IDX(kA,1,2) += afW[1]*afV[2];
			IDX(kA,2,1) += afW[2];
			IDX(kA,2,2) += afW[2]*afV[2];

			IDX(kR,0,0) = 1.0;
			IDX(kR,0,1) = 0.0;
			IDX(kR,1,0) = 0.0;
			IDX(kR,0,2) = 0.0;
			IDX(kR,2,0) = 0.0;
			IDX(kR,1,1) = 1.0f+fT2;
			IDX(kR,1,2) = fT2*afV[2];
			IDX(kR,2,1) = fT2*afV[2];
			IDX(kR,2,2) = 1.0f+fT2*afV[2]*afV[2];
		}
		else
		{
			kR = dnonlynew Matrix3(Matrix3::IDENTITY);
		}

		// map second column to (*,*,0)
		fLength = UMath::Sqrt(IDX(kA,1,1)*IDX(kA,1,1)+IDX(kA,2,1)*IDX(kA,2,1));
		if ( fLength > 0.0 )
		{
			fSign = (IDX(kA,1,1) > 0.0f ? 1.0f : -1.0f);
			fT1 = IDX(kA,1,1) + fSign*fLength;
			afV[2] = IDX(kA,2,1)/fT1;

			fT2 = -2.0f/(1.0f+afV[2]*afV[2]);
			afW[1] = fT2*(IDX(kA,1,1)+IDX(kA,2,1)*afV[2]);
			afW[2] = fT2*(IDX(kA,1,2)+IDX(kA,2,2)*afV[2]);
			IDX(kA,1,1) += afW[1];
			IDX(kA,1,2) += afW[2];
			IDX(kA,2,2) += afV[2]*afW[2];

			Real fA = 1.0f+fT2;
			Real fB = fT2*afV[2];
			Real fC = 1.0f+fB*afV[2];

			if ( bIdentity )
			{
				IDX(kL,0,0) = 1.0;
				IDX(kL,0,1) = 0.0;
				IDX(kL,1,0) = 0.0;
				IDX(kL,0,2) = 0.0;
				IDX(kL,2,0) = 0.0;
				IDX(kL,1,1) = fA;
				IDX(kL,1,2) = fB;
				IDX(kL,2,1) = fB;
				IDX(kL,2,2) = fC;
			}
			else
			{
				for (int iRow = 0; iRow < 3; iRow++)
				{
					Real fTmp0 = IDX(kL,iRow,1);
					Real fTmp1 = IDX(kL,iRow,2);
					IDX(kL,iRow,1) = fA*fTmp0+fB*fTmp1;
					IDX(kL,iRow,2) = fB*fTmp0+fC*fTmp1;
				}
			}
		}
	}
	//-----------------------------------------------------------------------
	void Matrix3::GolubKahanStep (REF(Matrix3) kA, REF(Matrix3) kL, REF(Matrix3) kR)
	{
		Real fT11 = IDX(kA,0,1)*IDX(kA,0,1)+IDX(kA,1,1)*IDX(kA,1,1);
		Real fT22 = IDX(kA,1,2)*IDX(kA,1,2)+IDX(kA,2,2)*IDX(kA,2,2);
		Real fT12 = IDX(kA,1,1)*IDX(kA,1,2);
		Real fTrace = fT11+fT22;
		Real fDiff = fT11-fT22;
		Real fDiscr = UMath::Sqrt(fDiff*fDiff+4.0f*fT12*fT12);
		Real fRoot1 = 0.5f*(fTrace+fDiscr);
		Real fRoot2 = 0.5f*(fTrace-fDiscr);

		// adjust right
		Real fY = IDX(kA,0,0) - (UMath::Abs(fRoot1-fT22) <=
			UMath::Abs(fRoot2-fT22) ? fRoot1 : fRoot2);
		Real fZ = IDX(kA,0,1);
		Real fInvLength = UMath::InvSqrt(fY*fY+fZ*fZ);
		Real fSin = fZ*fInvLength;
		Real fCos = -fY*fInvLength;

		Real fTmp0 = IDX(kA,0,0);
		Real fTmp1 = IDX(kA,0,1);
		IDX(kA,0,0) = fCos*fTmp0-fSin*fTmp1;
		IDX(kA,0,1) = fSin*fTmp0+fCos*fTmp1;
		IDX(kA,1,0) = -fSin*IDX(kA,1,1);
		IDX(kA,1,1) *= fCos;

		size_t iRow;
		for (iRow = 0; iRow < 3; iRow++)
		{
			fTmp0 = IDX(kR,0,iRow);
			fTmp1 = IDX(kR,1,iRow);
			IDX(kR,0,iRow) = fCos*fTmp0-fSin*fTmp1;
			IDX(kR,1,iRow) = fSin*fTmp0+fCos*fTmp1;
		}

		// adjust left
		fY = IDX(kA,0,0);
		fZ = IDX(kA,1,0);
		fInvLength = UMath::InvSqrt(fY*fY+fZ*fZ);
		fSin = fZ*fInvLength;
		fCos = -fY*fInvLength;

		IDX(kA,0,0) = fCos*IDX(kA,0,0)-fSin*IDX(kA,1,0);
		fTmp0 = IDX(kA,0,1);
		fTmp1 = IDX(kA,1,1);
		IDX(kA,0,1) = fCos*fTmp0-fSin*fTmp1;
		IDX(kA,1,1) = fSin*fTmp0+fCos*fTmp1;
		IDX(kA,0,2) = -fSin*IDX(kA,1,2);
		IDX(kA,1,2) *= fCos;

		size_t iCol;
		for (iCol = 0; iCol < 3; iCol++)
		{
			fTmp0 = IDX(kL,iCol,0);
			fTmp1 = IDX(kL,iCol,1);
			IDX(kL,iCol,0) = fCos*fTmp0-fSin*fTmp1;
			IDX(kL,iCol,1) = fSin*fTmp0+fCos*fTmp1;
		}

		// adjust right
		fY = IDX(kA,0,1);
		fZ = IDX(kA,0,2);
		fInvLength = UMath::InvSqrt(fY*fY+fZ*fZ);
		fSin = fZ*fInvLength;
		fCos = -fY*fInvLength;

		IDX(kA,0,1) = fCos*IDX(kA,0,1)-fSin*IDX(kA,0,2);
		fTmp0 = IDX(kA,1,1);
		fTmp1 = IDX(kA,1,2);
		IDX(kA,1,1) = fCos*fTmp0-fSin*fTmp1;
		IDX(kA,1,2) = fSin*fTmp0+fCos*fTmp1;
		IDX(kA,2,1) = -fSin*IDX(kA,2,2);
		IDX(kA,2,2) *= fCos;

		for (iRow = 0; iRow < 3; iRow++)
		{
			fTmp0 = IDX(kR,1,iRow);
			fTmp1 = IDX(kR,2,iRow);
			IDX(kR,1,iRow) = fCos*fTmp0-fSin*fTmp1;
			IDX(kR,2,iRow) = fSin*fTmp0+fCos*fTmp1;
		}

		// adjust left
		fY = IDX(kA,1,1);
		fZ = IDX(kA,2,1);
		fInvLength = UMath::InvSqrt(fY*fY+fZ*fZ);
		fSin = fZ*fInvLength;
		fCos = -fY*fInvLength;

		IDX(kA,1,1) = fCos*IDX(kA,1,1)-fSin*IDX(kA,2,1);
		fTmp0 = IDX(kA,1,2);
		fTmp1 = IDX(kA,2,2);
		IDX(kA,1,2) = fCos*fTmp0-fSin*fTmp1;
		IDX(kA,2,2) = fSin*fTmp0+fCos*fTmp1;

		for (iCol = 0; iCol < 3; iCol++)
		{
			fTmp0 = IDX(kL,iCol,1);
			fTmp1 = IDX(kL,iCol,2);
			IDX(kL,iCol,1) = fCos*fTmp0-fSin*fTmp1;
			IDX(kL,iCol,2) = fSin*fTmp0+fCos*fTmp1;
		}
	}
	//-----------------------------------------------------------------------
	void Matrix3::SingularValueDecomposition (REF(Matrix3) kL, REF(Vector3) kS, REF(Matrix3) kR) CONSTF
	{
		// temas: currently unused
		//const int iMax = 16;
		size_t iRow, iCol;

		INSTANCE(Matrix3) kA = dnonlynew Matrix3(THIS_OBJ);
		Bidiagonalize(kA,kL,kR);

		for (unsigned int i = 0; i < msSvdMaxIterations; i++)
		{
			Real fTmp, fTmp0, fTmp1;
			Real fSin0, fCos0, fTan0;
			Real fSin1, fCos1, fTan1;

			bool bTest1 = (UMath::Abs(IDX(kA,0,1)) <=
				msSvdEpsilon*(UMath::Abs(IDX(kA,0,0))+UMath::Abs(IDX(kA,1,1))));
			bool bTest2 = (UMath::Abs(IDX(kA,1,2)) <=
				msSvdEpsilon*(UMath::Abs(IDX(kA,1,1))+UMath::Abs(IDX(kA,2,2))));
			if ( bTest1 )
			{
				if ( bTest2 )
				{
					kS[0] = IDX(kA,0,0);
					kS[1] = IDX(kA,1,1);
					kS[2] = IDX(kA,2,2);
					break;
				}
				else
				{
					// 2x2 closed form factorization
					fTmp = (IDX(kA,1,1)*IDX(kA,1,1) - IDX(kA,2,2)*IDX(kA,2,2) +
						    IDX(kA,1,2)*IDX(kA,1,2))/(IDX(kA,1,2)*IDX(kA,2,2));
					fTan0 = 0.5f*(fTmp+UMath::Sqrt(fTmp*fTmp + 4.0f));
					fCos0 = UMath::InvSqrt(1.0f+fTan0*fTan0);
					fSin0 = fTan0*fCos0;

					for (iCol = 0; iCol < 3; iCol++)
					{
						fTmp0 = IDX(kL,iCol,1);
						fTmp1 = IDX(kL,iCol,2);
						IDX(kL,iCol,1) = fCos0*fTmp0-fSin0*fTmp1;
						IDX(kL,iCol,2) = fSin0*fTmp0+fCos0*fTmp1;
					}

					fTan1 = (IDX(kA,1,2)-IDX(kA,2,2)*fTan0)/IDX(kA,1,1);
					fCos1 = UMath::InvSqrt(1.0f+fTan1*fTan1);
					fSin1 = -fTan1*fCos1;

					for (iRow = 0; iRow < 3; iRow++)
					{
						fTmp0 = IDX(kR,1,iRow);
						fTmp1 = IDX(kR,2,iRow);
						IDX(kR,1,iRow) = fCos1*fTmp0-fSin1*fTmp1;
						IDX(kR,2,iRow) = fSin1*fTmp0+fCos1*fTmp1;
					}

					kS[0] = IDX(kA,0,0);
					kS[1] = fCos0*fCos1*IDX(kA,1,1) -
						fSin1*(fCos0*IDX(kA,1,2)-fSin0*IDX(kA,2,2));
					kS[2] = fSin0*fSin1*IDX(kA,1,1) +
						fCos1*(fSin0*IDX(kA,1,2)+fCos0*IDX(kA,2,2));
					break;
				}
			}
			else
			{
				if ( bTest2 )
				{
					// 2x2 closed form factorization
					fTmp = (IDX(kA,0,0)*IDX(kA,0,0) + IDX(kA,1,1)*IDX(kA,1,1) -
						IDX(kA,0,1)*IDX(kA,0,1))/(IDX(kA,0,1)*IDX(kA,1,1));
					fTan0 = 0.5f*(-fTmp+UMath::Sqrt(fTmp*fTmp + 4.0f));
					fCos0 = UMath::InvSqrt(1.0f+fTan0*fTan0);
					fSin0 = fTan0*fCos0;

					for (iCol = 0; iCol < 3; iCol++)
					{
						fTmp0 = IDX(kL,iCol,0);
						fTmp1 = IDX(kL,iCol,1);
						IDX(kL,iCol,0) = fCos0*fTmp0-fSin0*fTmp1;
						IDX(kL,iCol,1) = fSin0*fTmp0+fCos0*fTmp1;
					}

					fTan1 = (IDX(kA,0,1)-IDX(kA,1,1)*fTan0)/IDX(kA,0,0);
					fCos1 = UMath::InvSqrt(1.0f+fTan1*fTan1);
					fSin1 = -fTan1*fCos1;

					for (iRow = 0; iRow < 3; iRow++)
					{
						fTmp0 = IDX(kR,0,iRow);
						fTmp1 = IDX(kR,1,iRow);
						IDX(kR,0,iRow) = fCos1*fTmp0-fSin1*fTmp1;
						IDX(kR,1,iRow) = fSin1*fTmp0+fCos1*fTmp1;
					}

					kS[0] = fCos0*fCos1*IDX(kA,0,0) -
						fSin1*(fCos0*IDX(kA,0,1)-fSin0*IDX(kA,1,1));
					kS[1] = fSin0*fSin1*IDX(kA,0,0) +
						fCos1*(fSin0*IDX(kA,0,1)+fCos0*IDX(kA,1,1));
					kS[2] = IDX(kA,2,2);
					break;
				}
				else
				{
					GolubKahanStep(kA,kL,kR);
				}
			}
		}

		// positize diagonal
		for (iRow = 0; iRow < 3; iRow++)
		{
			if ( kS[iRow] < 0.0 )
			{
				kS[iRow] = -kS[iRow];
				for (iCol = 0; iCol < 3; iCol++)
					IDX(kR,iRow,iCol) = -IDX(kR,iRow,iCol);
			}
		}
	}
	//-----------------------------------------------------------------------
	void Matrix3::SingularValueComposition (REF_CONST(Matrix3) kL, REF_CONST(Vector3) kS, REF_CONST(Matrix3) kR)
	{
		size_t iRow, iCol;
		INSTANCE(Matrix3) kTmp = dnonlynew Matrix3();

		// product S*R
		for (iRow = 0; iRow < 3; iRow++)
		{
			for (iCol = 0; iCol < 3; iCol++)
				IDX(kTmp,iRow,iCol) = kS[iRow]*IDX(kR,iRow,iCol);
		}

		// product L*S*R
		for (iRow = 0; iRow < 3; iRow++)
		{
			for (iCol = 0; iCol < 3; iCol++)
			{
				IDX(m,iRow,iCol) = 0.0;
				for (int iMid = 0; iMid < 3; iMid++)
					IDX(m,iRow,iCol) += IDX(kL,iRow,iMid)*IDX(kTmp,iMid,iCol);
			}
		}
	}
	//-----------------------------------------------------------------------
	void Matrix3::Orthonormalize ()
	{
		// Algorithm uses Gram-Schmidt orthogonalization.  If 'this' matrix is
		// M = [m0|m1|m2], then orthonormal output matrix is Q = [q0|q1|q2],
		//
		//   q0 = m0/|m0|
		//   q1 = (m1-(q0*m1)q0)/|m1-(q0*m1)q0|
		//   q2 = (m2-(q0*m2)q0-(q1*m2)q1)/|m2-(q0*m2)q0-(q1*m2)q1|
		//
		// where |V| indicates length of vector V and A*B indicates dot
		// product of vectors A and B.

		// compute q0
		Real fInvLength = UMath::InvSqrt(IDX(m,0,0)*IDX(m,0,0)
			+ IDX(m,1,0)*IDX(m,1,0) +
			IDX(m,2,0)*IDX(m,2,0));

		IDX(m,0,0) *= fInvLength;
		IDX(m,1,0) *= fInvLength;
		IDX(m,2,0) *= fInvLength;

		// compute q1
		Real fDot0 =
			IDX(m,0,0)*IDX(m,0,1) +
			IDX(m,1,0)*IDX(m,1,1) +
			IDX(m,2,0)*IDX(m,2,1);

		IDX(m,0,1) -= fDot0*IDX(m,0,0);
		IDX(m,1,1) -= fDot0*IDX(m,1,0);
		IDX(m,2,1) -= fDot0*IDX(m,2,0);

		fInvLength = UMath::InvSqrt(IDX(m,0,1)*IDX(m,0,1) +
			IDX(m,1,1)*IDX(m,1,1) +
			IDX(m,2,1)*IDX(m,2,1));

		IDX(m,0,1) *= fInvLength;
		IDX(m,1,1) *= fInvLength;
		IDX(m,2,1) *= fInvLength;

		// compute q2
		Real fDot1 =
			IDX(m,0,1)*IDX(m,0,2) +
			IDX(m,1,1)*IDX(m,1,2) +
			IDX(m,2,1)*IDX(m,2,2);

		fDot0 =
			IDX(m,0,0)*IDX(m,0,2) +
			IDX(m,1,0)*IDX(m,1,2) +
			IDX(m,2,0)*IDX(m,2,2);

		IDX(m,0,2) -= fDot0*IDX(m,0,0) + fDot1*IDX(m,0,1);
		IDX(m,1,2) -= fDot0*IDX(m,1,0) + fDot1*IDX(m,1,1);
		IDX(m,2,2) -= fDot0*IDX(m,2,0) + fDot1*IDX(m,2,1);

		fInvLength = UMath::InvSqrt(IDX(m,0,2)*IDX(m,0,2) +
			IDX(m,1,2)*IDX(m,1,2) +
			IDX(m,2,2)*IDX(m,2,2));

		IDX(m,0,2) *= fInvLength;
		IDX(m,1,2) *= fInvLength;
		IDX(m,2,2) *= fInvLength;
	}
	//-----------------------------------------------------------------------
	void Matrix3::QDUDecomposition (REF(Matrix3) kQ, REF(Vector3) kD, REF(Vector3) kU) CONSTF
	{
		// Factor M = QR = QDU where Q is orthogonal, D is diagonal,
		// and U is upper triangular with ones on its diagonal.  Algorithm uses
		// Gram-Schmidt orthogonalization (the QR algorithm).
		//
		// If M = [ m0 | m1 | m2 ] and Q = [ q0 | q1 | q2 ], then
		//
		//   q0 = m0/|m0|
		//   q1 = (m1-(q0*m1)q0)/|m1-(q0*m1)q0|
		//   q2 = (m2-(q0*m2)q0-(q1*m2)q1)/|m2-(q0*m2)q0-(q1*m2)q1|
		//
		// where |V| indicates length of vector V and A*B indicates dot
		// product of vectors A and B.  The matrix R has entries
		//
		//   r00 = q0*m0  r01 = q0*m1  r02 = q0*m2
		//   r10 = 0      r11 = q1*m1  r12 = q1*m2
		//   r20 = 0      r21 = 0      r22 = q2*m2
		//
		// so D = diag(r00,r11,r22) and U has entries u01 = r01/r00,
		// u02 = r02/r00, and u12 = r12/r11.

		// Q = rotation
		// D = scaling
		// U = shear

		// D stores the three diagonal entries r00, r11, r22
		// U stores the entries U[0] = u01, U[1] = u02, U[2] = u12

		// build orthogonal matrix Q
		Real fInvLength = IDX(m,0,0)*IDX(m,0,0) + IDX(m,1,0)*IDX(m,1,0) + IDX(m,2,0)*IDX(m,2,0);

		if (!UMath::RealEqual(fInvLength,0))
			fInvLength = UMath::InvSqrt(fInvLength);

		IDX(kQ,0,0) = IDX(m,0,0)*fInvLength;
		IDX(kQ,1,0) = IDX(m,1,0)*fInvLength;
		IDX(kQ,2,0) = IDX(m,2,0)*fInvLength;

		Real fDot = IDX(kQ,0,0)*IDX(m,0,1) + IDX(kQ,1,0)*IDX(m,1,1) +
			IDX(kQ,2,0)*IDX(m,2,1);
		IDX(kQ,0,1) = IDX(m,0,1)-fDot*IDX(kQ,0,0);
		IDX(kQ,1,1) = IDX(m,1,1)-fDot*IDX(kQ,1,0);
		IDX(kQ,2,1) = IDX(m,2,1)-fDot*IDX(kQ,2,0);
		fInvLength = IDX(kQ,0,1)*IDX(kQ,0,1) + IDX(kQ,1,1)*IDX(kQ,1,1) + IDX(kQ,2,1)*IDX(kQ,2,1);
		if (!UMath::RealEqual(fInvLength,0))
			fInvLength = UMath::InvSqrt(fInvLength);
		
		IDX(kQ,0,1) *= fInvLength;
		IDX(kQ,1,1) *= fInvLength;
		IDX(kQ,2,1) *= fInvLength;

		fDot = IDX(kQ,0,0)*IDX(m,0,2) + IDX(kQ,1,0)*IDX(m,1,2) +
			IDX(kQ,2,0)*IDX(m,2,2);
		IDX(kQ,0,2) = IDX(m,0,2)-fDot*IDX(kQ,0,0);
		IDX(kQ,1,2) = IDX(m,1,2)-fDot*IDX(kQ,1,0);
		IDX(kQ,2,2) = IDX(m,2,2)-fDot*IDX(kQ,2,0);
		fDot = IDX(kQ,0,1)*IDX(m,0,2) + IDX(kQ,1,1)*IDX(m,1,2) +
			IDX(kQ,2,1)*IDX(m,2,2);
		IDX(kQ,0,2) -= fDot*IDX(kQ,0,1);
		IDX(kQ,1,2) -= fDot*IDX(kQ,1,1);
		IDX(kQ,2,2) -= fDot*IDX(kQ,2,1);
		fInvLength = IDX(kQ,0,2)*IDX(kQ,0,2) + IDX(kQ,1,2)*IDX(kQ,1,2) + IDX(kQ,2,2)*IDX(kQ,2,2);
		if (!UMath::RealEqual(fInvLength,0))
			fInvLength = UMath::InvSqrt(fInvLength);

		IDX(kQ,0,2) *= fInvLength;
		IDX(kQ,1,2) *= fInvLength;
		IDX(kQ,2,2) *= fInvLength;

		// guarantee that orthogonal matrix has determinant 1 (no reflections)
		Real fDet = IDX(kQ,0,0)*IDX(kQ,1,1)*IDX(kQ,2,2) + IDX(kQ,0,1)*IDX(kQ,1,2)*IDX(kQ,2,0) +
			IDX(kQ,0,2)*IDX(kQ,1,0)*IDX(kQ,2,1) - IDX(kQ,0,2)*IDX(kQ,1,1)*IDX(kQ,2,0) -
			IDX(kQ,0,1)*IDX(kQ,1,0)*IDX(kQ,2,2) - IDX(kQ,0,0)*IDX(kQ,1,2)*IDX(kQ,2,1);

		if ( fDet < 0.0 )
		{
			for (size_t iRow = 0; iRow < 3; iRow++)
				for (size_t iCol = 0; iCol < 3; iCol++)
					IDX(kQ,iRow,iCol) = -IDX(kQ,iRow,iCol);
		}

		// build "right" matrix R
		Matrix3 kR;
		IDX(kR,0,0) = IDX(kQ,0,0)*IDX(m,0,0) + IDX(kQ,1,0)*IDX(m,1,0) +
			IDX(kQ,2,0)*IDX(m,2,0);
		IDX(kR,0,1) = IDX(kQ,0,0)*IDX(m,0,1) + IDX(kQ,1,0)*IDX(m,1,1) +
			IDX(kQ,2,0)*IDX(m,2,1);
		IDX(kR,1,1) = IDX(kQ,0,1)*IDX(m,0,1) + IDX(kQ,1,1)*IDX(m,1,1) +
			IDX(kQ,2,1)*IDX(m,2,1);
		IDX(kR,0,2) = IDX(kQ,0,0)*IDX(m,0,2) + IDX(kQ,1,0)*IDX(m,1,2) +
			IDX(kQ,2,0)*IDX(m,2,2);
		IDX(kR,1,2) = IDX(kQ,0,1)*IDX(m,0,2) + IDX(kQ,1,1)*IDX(m,1,2) +
			IDX(kQ,2,1)*IDX(m,2,2);
		IDX(kR,2,2) = IDX(kQ,0,2)*IDX(m,0,2) + IDX(kQ,1,2)*IDX(m,1,2) +
			IDX(kQ,2,2)*IDX(m,2,2);

		// the scaling component
		kD[0] = IDX(kR,0,0);
		kD[1] = IDX(kR,1,1);
		kD[2] = IDX(kR,2,2);

		// the shear component
		Real fInvD0 = 1.0f/kD[0];
		kU[0] = IDX(kR,0,1)*fInvD0;
		kU[1] = IDX(kR,0,2)*fInvD0;
		kU[2] = IDX(kR,1,2)/kD[1];
	}
	//-----------------------------------------------------------------------
	Real Matrix3::MaxCubicRoot (Real afCoeff[3])
	{
		// Spectral norm is for A^T*A, so characteristic polynomial
		// P(x) = c[0]+c[1]*x+c[2]*x^2+x^3 has three positive real roots.
		// This yields the assertions c[0] < 0 and c[2]*c[2] >= 3*c[1].

		// quick out for uniform scale (triple root)
		const Real fOneThird = Real(1.0/3.0);
		const Real fEpsilon = Real(1e-06);
		Real fDiscr = afCoeff[2]*afCoeff[2] - 3.0f*afCoeff[1];
		if ( fDiscr <= fEpsilon )
			return -fOneThird*afCoeff[2];

		// Compute an upper bound on roots of P(x).  This assumes that A^T*A
		// has been scaled by its largest entry.
		Real fX = 1.0;
		Real fPoly = afCoeff[0]+fX*(afCoeff[1]+fX*(afCoeff[2]+fX));
		if ( fPoly < 0.0 )
		{
			// uses a matrix norm to find an upper bound on maximum root
			fX = UMath::Abs(afCoeff[0]);
			Real fTmp = 1.0f+UMath::Abs(afCoeff[1]);
			if ( fTmp > fX )
				fX = fTmp;
			fTmp = 1.0f+UMath::Abs(afCoeff[2]);
			if ( fTmp > fX )
				fX = fTmp;
		}

		// Newton's method to find root
		Real fTwoC2 = 2.0f*afCoeff[2];
		for (int i = 0; i < 16; i++)
		{
			fPoly = afCoeff[0]+fX*(afCoeff[1]+fX*(afCoeff[2]+fX));
			if ( UMath::Abs(fPoly) <= fEpsilon )
				return fX;

			Real fDeriv = afCoeff[1]+fX*(fTwoC2+3.0f*fX);
			fX -= fPoly/fDeriv;
		}

		return fX;
	}
	//-----------------------------------------------------------------------
	Real Matrix3::SpectralNorm () CONSTF
	{
		Matrix3 kP;
		size_t iRow, iCol;
		Real fPmax = 0.0;
		for (iRow = 0; iRow < 3; iRow++)
		{
			for (iCol = 0; iCol < 3; iCol++)
			{
				IDX(kP,iRow,iCol) = 0.0;
				for (int iMid = 0; iMid < 3; iMid++)
				{
					IDX(kP,iRow,iCol) +=
						IDX(m,iMid,iRow)*IDX(m,iMid,iCol);
				}
				if ( IDX(kP,iRow,iCol) > fPmax )
					fPmax = IDX(kP,iRow,iCol);
			}
		}

		Real fInvPmax = 1.0f/fPmax;
		for (iRow = 0; iRow < 3; iRow++)
		{
			for (iCol = 0; iCol < 3; iCol++)
				IDX(kP,iRow,iCol) *= fInvPmax;
		}

		Real afCoeff[3];
		afCoeff[0] = -(IDX(kP,0,0)*(IDX(kP,1,1)*IDX(kP,2,2)-IDX(kP,1,2)*IDX(kP,2,1)) +
			IDX(kP,0,1)*(IDX(kP,2,0)*IDX(kP,1,2)-IDX(kP,1,0)*IDX(kP,2,2)) +
			IDX(kP,0,2)*(IDX(kP,1,0)*IDX(kP,2,1)-IDX(kP,2,0)*IDX(kP,1,1)));
		afCoeff[1] = IDX(kP,0,0)*IDX(kP,1,1)-IDX(kP,0,1)*IDX(kP,1,0) +
			IDX(kP,0,0)*IDX(kP,2,2)-IDX(kP,0,2)*IDX(kP,2,0) +
			IDX(kP,1,1)*IDX(kP,2,2)-IDX(kP,1,2)*IDX(kP,2,1);
		afCoeff[2] = -(IDX(kP,0,0)+IDX(kP,1,1)+IDX(kP,2,2));

		Real fRoot = MaxCubicRoot(afCoeff);
		Real fNorm = UMath::Sqrt(fPmax*fRoot);
		return fNorm;
	}
	//-----------------------------------------------------------------------
	void Matrix3::ToAngleAxis (REF(Vector3) rkAxis, REF(Radian) rfRadians) CONSTF
	{
		// Let (x,y,z) be the unit-length axis and let A be an angle of rotation.
		// The rotation matrix is R = I + sin(A)*P + (1-cos(A))*P^2 where
		// I is the identity and
		//
		//       +-        -+
		//   P = |  0 -z +y |
		//       | +z  0 -x |
		//       | -y +x  0 |
		//       +-        -+
		//
		// If A > 0, R represents a counterclockwise rotation about the axis in
		// the sense of looking from the tip of the axis vector towards the
		// origin.  Some algebra will show that
		//
		//   cos(A) = (trace(R)-1)/2  and  R - R^t = 2*sin(A)*P
		//
		// In the event that A = pi, R-R^t = 0 which prevents us from extracting
		// the axis through P.  Instead note that R = I+2*P^2 when A = pi, so
		// P^2 = (R-I)/2.  The diagonal entries of P^2 are x^2-1, y^2-1, and
		// z^2-1.  We can solve these for axis (x,y,z).  Because the angle is pi,
		// it does not matter which sign you choose on the square roots.

		Real fTrace = IDX(m,0,0) + IDX(m,1,1) + IDX(m,2,2);
		Real fCos = 0.5f*(fTrace-1.0f);
		rfRadians = UMath::ACos(fCos);  // in [0,PI]

		if ( rfRadians > dnonlynew Radian(0.0) )
		{
			if ( rfRadians < dnonlynew Radian(UMath::PI) )
			{
				OF(rkAxis,x) = IDX(m,2,1)-IDX(m,1,2);
				OF(rkAxis,y) = IDX(m,0,2)-IDX(m,2,0);
				OF(rkAxis,z) = IDX(m,1,0)-IDX(m,0,1);
				OF(rkAxis,normalise());
			}
			else
			{
				// angle is PI
				float fHalfInverse;
				if ( IDX(m,0,0) >= IDX(m,1,1) )
				{
					// r00 >= r11
					if ( IDX(m,0,0) >= IDX(m,2,2) )
					{
						// r00 is maximum diagonal term
						OF(rkAxis,x) = 0.5f * UMath::Sqrt(IDX(m,0,0) -
							IDX(m,1,1) - IDX(m,2,2) + 1.0f);
						fHalfInverse = 0.5f/OF(rkAxis,x);
						OF(rkAxis,y) = fHalfInverse*IDX(m,0,1);
						OF(rkAxis,z) = fHalfInverse*IDX(m,0,2);
					}
					else
					{
						// r22 is maximum diagonal term
						OF(rkAxis,z) = 0.5f*UMath::Sqrt(IDX(m,2,2) -
							IDX(m,0,0) - IDX(m,1,1) + 1.0f);
						fHalfInverse = 0.5f/OF(rkAxis,z);
						OF(rkAxis,x) = fHalfInverse*IDX(m,0,2);
						OF(rkAxis,y) = fHalfInverse*IDX(m,1,2);
					}
				}
				else
				{
					// r11 > r00
					if ( IDX(m,1,1) >= IDX(m,2,2) )
					{
						// r11 is maximum diagonal term
						OF(rkAxis,y) = 0.5f*UMath::Sqrt(IDX(m,1,1) -
							IDX(m,0,0) - IDX(m,2,2) + 1.0f);
						fHalfInverse  = 0.5f/OF(rkAxis,y);
						OF(rkAxis,x) = fHalfInverse*IDX(m,0,1);
						OF(rkAxis,z) = fHalfInverse*IDX(m,1,2);
					}
					else
					{
						// r22 is maximum diagonal term
						OF(rkAxis,z) = 0.5f*UMath::Sqrt(IDX(m,2,2) -
							IDX(m,0,0) - IDX(m,1,1) + 1.0f);
						fHalfInverse = 0.5f/OF(rkAxis,z);
						OF(rkAxis,x) = fHalfInverse*IDX(m,0,2);
						OF(rkAxis,y) = fHalfInverse*IDX(m,1,2);
					}
				}
			}
		}
		else
		{
			// The angle is 0 and the matrix is the identity.  Any axis will
			// work, so just use the x-axis.
			OF(rkAxis,x) = 1.0;
			OF(rkAxis,y) = 0.0;
			OF(rkAxis,z) = 0.0;
		}
	}
	//-----------------------------------------------------------------------
	void Matrix3::FromAngleAxis (REF_CONST(Vector3) rkAxis, REF_CONST(Radian) fRadians)
	{
		Real fCos = UMath::Cos(fRadians);
		Real fSin = UMath::Sin(fRadians);
		Real fOneMinusCos = 1.0f-fCos;
		Real fX2 = OF(rkAxis,x)*OF(rkAxis,x);
		Real fY2 = OF(rkAxis,y)*OF(rkAxis,y);
		Real fZ2 = OF(rkAxis,z)*OF(rkAxis,z);
		Real fXYM = OF(rkAxis,x)*OF(rkAxis,y)*fOneMinusCos;
		Real fXZM = OF(rkAxis,x)*OF(rkAxis,z)*fOneMinusCos;
		Real fYZM = OF(rkAxis,y)*OF(rkAxis,z)*fOneMinusCos;
		Real fXSin = OF(rkAxis,x)*fSin;
		Real fYSin = OF(rkAxis,y)*fSin;
		Real fZSin = OF(rkAxis,z)*fSin;

		IDX(m,0,0) = fX2*fOneMinusCos+fCos;
		IDX(m,0,1) = fXYM-fZSin;
		IDX(m,0,2) = fXZM+fYSin;
		IDX(m,1,0) = fXYM+fZSin;
		IDX(m,1,1) = fY2*fOneMinusCos+fCos;
		IDX(m,1,2) = fYZM-fXSin;
		IDX(m,2,0) = fXZM-fYSin;
		IDX(m,2,1) = fYZM+fXSin;
		IDX(m,2,2) = fZ2*fOneMinusCos+fCos;
	}
	//-----------------------------------------------------------------------
	bool Matrix3::ToEulerAnglesXYZ (REF(Radian) rfYAngle, REF(Radian) rfPAngle, REF(Radian) rfRAngle) CONSTF
	{
		// rot =  cy*cz          -cy*sz           sy
		//        cz*sx*sy+cx*sz  cx*cz-sx*sy*sz -cy*sx
		//       -cx*cz*sy+sx*sz  cz*sx+cx*sy*sz  cx*cy

		rfPAngle = dnonlynew Radian(UMath::ASin(IDX(m,0,2)));
		if ( rfPAngle < dnonlynew  Radian(UMath::HALF_PI) )
		{
			if ( rfPAngle > dnonlynew Radian(-UMath::HALF_PI) )
			{
				rfYAngle = UMath::ATan2(-IDX(m,1,2),IDX(m,2,2));
				rfRAngle = UMath::ATan2(-IDX(m,0,1),IDX(m,0,0));
				return true;
			}
			else
			{
				// WARNING.  Not a unique solution.
				INSTANCE(Radian) fRmY = UMath::ATan2(IDX(m,1,0),IDX(m,1,1));
				rfRAngle = dnonlynew Radian(0.0);  // any angle works
				rfYAngle = rfRAngle - fRmY;
				return false;
			}
		}
		else
		{
			// WARNING.  Not a unique solution.
			INSTANCE(Radian) fRpY = UMath::ATan2(IDX(m,1,0),IDX(m,1,1));
			rfRAngle = dnonlynew Radian(0.0);  // any angle works
			rfYAngle = fRpY - rfRAngle;
			return false;
		}
	}
	//-----------------------------------------------------------------------
	bool Matrix3::ToEulerAnglesXZY (REF(Radian) rfYAngle, REF(Radian) rfPAngle, REF(Radian) rfRAngle) CONSTF
	{
		// rot =  cy*cz          -sz              cz*sy
		//        sx*sy+cx*cy*sz  cx*cz          -cy*sx+cx*sy*sz
		//       -cx*sy+cy*sx*sz  cz*sx           cx*cy+sx*sy*sz

		rfPAngle = UMath::ASin(-IDX(m,0,1));
		if ( rfPAngle < dnonlynew Radian(UMath::HALF_PI) )
		{
			if ( rfPAngle > dnonlynew Radian(-UMath::HALF_PI) )
			{
				rfYAngle = UMath::ATan2(IDX(m,2,1),IDX(m,1,1));
				rfRAngle = UMath::ATan2(IDX(m,0,2),IDX(m,0,0));
				return true;
			}
			else
			{
				// WARNING.  Not a unique solution.
				INSTANCE(Radian) fRmY = UMath::ATan2(-IDX(m,2,0),IDX(m,2,2));
				rfRAngle = dnonlynew Radian(0.0);  // any angle works
				rfYAngle = rfRAngle - fRmY;
				return false;
			}
		}
		else
		{
			// WARNING.  Not a unique solution.
			INSTANCE(Radian) fRpY = UMath::ATan2(-IDX(m,2,0),IDX(m,2,2));
			rfRAngle = dnonlynew Radian(0.0);  // any angle works
			rfYAngle = fRpY - rfRAngle;
			return false;
		}
	}
	//-----------------------------------------------------------------------
	bool Matrix3::ToEulerAnglesYXZ (REF(Radian) rfYAngle,REF(Radian) rfPAngle, REF(Radian) rfRAngle) CONSTF
	{
		// rot =  cy*cz+sx*sy*sz  cz*sx*sy-cy*sz  cx*sy
		//        cx*sz           cx*cz          -sx
		//       -cz*sy+cy*sx*sz  cy*cz*sx+sy*sz  cx*cy

		rfPAngle = UMath::ASin(-IDX(m,1,2));
		if ( rfPAngle < dnonlynew Radian(UMath::HALF_PI) )
		{
			if ( rfPAngle > dnonlynew Radian(-UMath::HALF_PI) )
			{
				rfYAngle = UMath::ATan2(IDX(m,0,2),IDX(m,2,2));
				rfRAngle = UMath::ATan2(IDX(m,1,0),IDX(m,1,1));
				return true;
			}
			else
			{
				// WARNING.  Not a unique solution.
				INSTANCE(Radian) fRmY = UMath::ATan2(-IDX(m,0,1),IDX(m,0,0));
				rfRAngle = dnonlynew Radian(0.0);  // any angle works
				rfYAngle = rfRAngle - fRmY;
				return false;
			}
		}
		else
		{
			// WARNING.  Not a unique solution.
			INSTANCE(Radian) fRpY = UMath::ATan2(-IDX(m,0,1),IDX(m,0,0));
			rfRAngle = dnonlynew Radian(0.0);  // any angle works
			rfYAngle = fRpY - rfRAngle;
			return false;
		}
	}
	//-----------------------------------------------------------------------
	bool Matrix3::ToEulerAnglesYZX (REF(Radian) rfYAngle,REF(Radian) rfPAngle, REF(Radian) rfRAngle) CONSTF
	{
		// rot =  cy*cz           sx*sy-cx*cy*sz  cx*sy+cy*sx*sz
		//        sz              cx*cz          -cz*sx
		//       -cz*sy           cy*sx+cx*sy*sz  cx*cy-sx*sy*sz

		rfPAngle = UMath::ASin(IDX(m,1,0));
		if ( rfPAngle < dnonlynew Radian(UMath::HALF_PI) )
		{
			if ( rfPAngle > dnonlynew Radian(-UMath::HALF_PI) )
			{
				rfYAngle = UMath::ATan2(-IDX(m,2,0),IDX(m,0,0));
				rfRAngle = UMath::ATan2(-IDX(m,1,2),IDX(m,1,1));
				return true;
			}
			else
			{
				// WARNING.  Not a unique solution.
				INSTANCE(Radian) fRmY = UMath::ATan2(IDX(m,2,1),IDX(m,2,2));
				rfRAngle = dnonlynew Radian(0.0);  // any angle works
				rfYAngle = rfRAngle - fRmY;
				return false;
			}
		}
		else
		{
			// WARNING.  Not a unique solution.
			INSTANCE(Radian) fRpY = UMath::ATan2(IDX(m,2,1),IDX(m,2,2));
			rfRAngle = dnonlynew Radian(0.0);  // any angle works
			rfYAngle = fRpY - rfRAngle;
			return false;
		}
	}
	//-----------------------------------------------------------------------
	bool Matrix3::ToEulerAnglesZXY (REF(Radian) rfYAngle, REF(Radian) rfPAngle, REF(Radian) rfRAngle) CONSTF
	{
		// rot =  cy*cz-sx*sy*sz -cx*sz           cz*sy+cy*sx*sz
		//        cz*sx*sy+cy*sz  cx*cz          -cy*cz*sx+sy*sz
		//       -cx*sy           sx              cx*cy

		rfPAngle = UMath::ASin(IDX(m,2,1));
		if ( rfPAngle < dnonlynew Radian(UMath::HALF_PI) )
		{
			if ( rfPAngle > dnonlynew Radian(-UMath::HALF_PI) )
			{
				rfYAngle = UMath::ATan2(-IDX(m,0,1),IDX(m,1,1));
				rfRAngle = UMath::ATan2(-IDX(m,2,0),IDX(m,2,2));
				return true;
			}
			else
			{
				// WARNING.  Not a unique solution.
				INSTANCE(Radian) fRmY = UMath::ATan2(IDX(m,0,2),IDX(m,0,0));
				rfRAngle = dnonlynew Radian(0.0);  // any angle works
				rfYAngle = rfRAngle - fRmY;
				return false;
			}
		}
		else
		{
			// WARNING.  Not a unique solution.
			INSTANCE(Radian) fRpY = UMath::ATan2(IDX(m,0,2),IDX(m,0,0));
			rfRAngle = dnonlynew Radian(0.0);  // any angle works
			rfYAngle = fRpY - rfRAngle;
			return false;
		}
	}
	//-----------------------------------------------------------------------
	bool Matrix3::ToEulerAnglesZYX (REF(Radian) rfYAngle, REF(Radian) rfPAngle, REF(Radian) rfRAngle) CONSTF
	{
		// rot =  cy*cz           cz*sx*sy-cx*sz  cx*cz*sy+sx*sz
		//        cy*sz           cx*cz+sx*sy*sz -cz*sx+cx*sy*sz
		//       -sy              cy*sx           cx*cy

		rfPAngle = UMath::ASin(-IDX(m,2,0));
		if ( rfPAngle < dnonlynew Radian(UMath::HALF_PI) )
		{
			if ( rfPAngle > dnonlynew Radian(-UMath::HALF_PI) )
			{
				rfYAngle = UMath::ATan2(IDX(m,1,0),IDX(m,0,0));
				rfRAngle = UMath::ATan2(IDX(m,2,1),IDX(m,2,2));
				return true;
			}
			else
			{
				// WARNING.  Not a unique solution.
				INSTANCE(Radian) fRmY = UMath::ATan2(-IDX(m,0,1),IDX(m,0,2));
				rfRAngle = dnonlynew Radian(0.0);  // any angle works
				rfYAngle = rfRAngle - fRmY;
				return false;
			}
		}
		else
		{
			// WARNING.  Not a unique solution.
			INSTANCE(Radian) fRpY = UMath::ATan2(-IDX(m,0,1),IDX(m,0,2));
			rfRAngle = dnonlynew Radian(0.0);  // any angle works
			rfYAngle = fRpY - rfRAngle;
			return false;
		}
	}
	//-----------------------------------------------------------------------
	void Matrix3::FromEulerAnglesXYZ (REF_CONST(Radian) fYAngle, REF_CONST(Radian) fPAngle, REF_CONST(Radian) fRAngle)
	{
		Real fCos, fSin;

		fCos = UMath::Cos(fYAngle);
		fSin = UMath::Sin(fYAngle);
		INSTANCE(Matrix3) kXMat = dnonlynew Matrix3(1.0,0.0,0.0,0.0,fCos,-fSin,0.0,fSin,fCos);

		fCos = UMath::Cos(fPAngle);
		fSin = UMath::Sin(fPAngle);
		INSTANCE(Matrix3) kYMat = dnonlynew Matrix3(fCos,0.0,fSin,0.0,1.0,0.0,-fSin,0.0,fCos);

		fCos = UMath::Cos(fRAngle);
		fSin = UMath::Sin(fRAngle);
		INSTANCE(Matrix3) kZMat = dnonlynew Matrix3(fCos,-fSin,0.0,fSin,fCos,0.0,0.0,0.0,1.0);

		operator=(kXMat*(kYMat*kZMat));
	}
	//-----------------------------------------------------------------------
	void Matrix3::FromEulerAnglesXZY (REF_CONST(Radian) fYAngle, REF_CONST(Radian) fPAngle, REF_CONST(Radian) fRAngle)
	{
		Real fCos, fSin;

		fCos = UMath::Cos(fYAngle);
		fSin = UMath::Sin(fYAngle);
		INSTANCE(Matrix3) kXMat = dnonlynew Matrix3(1.0,0.0,0.0,0.0,fCos,-fSin,0.0,fSin,fCos);

		fCos = UMath::Cos(fPAngle);
		fSin = UMath::Sin(fPAngle);
		INSTANCE(Matrix3) kZMat = dnonlynew Matrix3(fCos,-fSin,0.0,fSin,fCos,0.0,0.0,0.0,1.0);

		fCos = UMath::Cos(fRAngle);
		fSin = UMath::Sin(fRAngle);
		INSTANCE(Matrix3) kYMat = dnonlynew Matrix3(fCos,0.0,fSin,0.0,1.0,0.0,-fSin,0.0,fCos);

		INSTANCE(Matrix3) kTemp = (kZMat*kYMat);
		operator=(kXMat*kTemp);
	}
	//-----------------------------------------------------------------------
	void Matrix3::FromEulerAnglesYXZ (REF_CONST(Radian) fYAngle, REF_CONST(Radian) fPAngle, REF_CONST(Radian) fRAngle)
	{
		Real fCos, fSin;

		fCos = UMath::Cos(fYAngle);
		fSin = UMath::Sin(fYAngle);
		INSTANCE(Matrix3) kYMat = dnonlynew Matrix3(fCos,0.0,fSin,0.0,1.0,0.0,-fSin,0.0,fCos);

		fCos = UMath::Cos(fPAngle);
		fSin = UMath::Sin(fPAngle);
		INSTANCE(Matrix3) kXMat = dnonlynew Matrix3(1.0,0.0,0.0,0.0,fCos,-fSin,0.0,fSin,fCos);

		fCos = UMath::Cos(fRAngle);
		fSin = UMath::Sin(fRAngle);
		INSTANCE(Matrix3) kZMat = dnonlynew Matrix3(fCos,-fSin,0.0,fSin,fCos,0.0,0.0,0.0,1.0);

		INSTANCE(Matrix3) kTemp = (kZMat*kYMat);
		operator=(kXMat*kTemp);
	}
	//-----------------------------------------------------------------------
	void Matrix3::FromEulerAnglesYZX (REF_CONST(Radian) fYAngle, REF_CONST(Radian) fPAngle, REF_CONST(Radian) fRAngle)
	{
		Real fCos, fSin;

		fCos = UMath::Cos(fYAngle);
		fSin = UMath::Sin(fYAngle);
		INSTANCE(Matrix3) kYMat = dnonlynew Matrix3(fCos,0.0,fSin,0.0,1.0,0.0,-fSin,0.0,fCos);

		fCos = UMath::Cos(fPAngle);
		fSin = UMath::Sin(fPAngle);
		INSTANCE(Matrix3) kZMat = dnonlynew Matrix3(fCos,-fSin,0.0,fSin,fCos,0.0,0.0,0.0,1.0);

		fCos = UMath::Cos(fRAngle);
		fSin = UMath::Sin(fRAngle);
		INSTANCE(Matrix3) kXMat = dnonlynew Matrix3(1.0,0.0,0.0,0.0,fCos,-fSin,0.0,fSin,fCos);

		INSTANCE(Matrix3) kTemp = (kZMat*kYMat);
		operator=(kXMat*kTemp);
	}
	//-----------------------------------------------------------------------
	void Matrix3::FromEulerAnglesZXY (REF_CONST(Radian) fYAngle, REF_CONST(Radian) fPAngle, REF_CONST(Radian) fRAngle)
	{
		Real fCos, fSin;

		fCos = UMath::Cos(fYAngle);
		fSin = UMath::Sin(fYAngle);
		INSTANCE(Matrix3) kZMat = dnonlynew Matrix3(fCos,-fSin,0.0,fSin,fCos,0.0,0.0,0.0,1.0);

		fCos = UMath::Cos(fPAngle);
		fSin = UMath::Sin(fPAngle);
		INSTANCE(Matrix3) kXMat = dnonlynew Matrix3(1.0,0.0,0.0,0.0,fCos,-fSin,0.0,fSin,fCos);

		fCos = UMath::Cos(fRAngle);
		fSin = UMath::Sin(fRAngle);
		INSTANCE(Matrix3) kYMat = dnonlynew Matrix3(fCos,0.0,fSin,0.0,1.0,0.0,-fSin,0.0,fCos);

		INSTANCE(Matrix3) kTemp = (kZMat*kYMat);
		operator=(kXMat*kTemp);
	}
	//-----------------------------------------------------------------------
	void Matrix3::FromEulerAnglesZYX (REF_CONST(Radian) fYAngle, REF_CONST(Radian) fPAngle, REF_CONST(Radian) fRAngle)
	{
		Real fCos, fSin;

		fCos = UMath::Cos(fYAngle);
		fSin = UMath::Sin(fYAngle);
		INSTANCE(Matrix3) kZMat = dnonlynew Matrix3(fCos,-fSin,0.0,fSin,fCos,0.0,0.0,0.0,1.0);

		fCos = UMath::Cos(fPAngle);
		fSin = UMath::Sin(fPAngle);
		INSTANCE(Matrix3) kYMat = dnonlynew Matrix3(fCos,0.0,fSin,0.0,1.0,0.0,-fSin,0.0,fCos);

		fCos = UMath::Cos(fRAngle);
		fSin = UMath::Sin(fRAngle);
		INSTANCE(Matrix3) kXMat = dnonlynew Matrix3(1.0,0.0,0.0,0.0,fCos,-fSin,0.0,fSin,fCos);

		INSTANCE(Matrix3) kTemp = (kZMat*kYMat);
		operator=(kXMat*kTemp);
	}
	//-----------------------------------------------------------------------
	void Matrix3::Tridiagonal (Real afDiag[3], Real afSubDiag[3])
	{
		// Householder reduction T = Q^t M Q
		//   Input:
		//     mat, symmetric 3x3 matrix M
		//   Output:
		//     mat, orthogonal matrix Q
		//     diag, diagonal entries of T
		//     subd, subdiagonal entries of T (T is symmetric)

		Real fA = IDX(m,0,0);
		Real fB = IDX(m,0,1);
		Real fC = IDX(m,0,2);
		Real fD = IDX(m,1,1);
		Real fE = IDX(m,1,2);
		Real fF = IDX(m,2,2);

		afDiag[0] = fA;
		afSubDiag[2] = 0.0;
		if ( UMath::Abs(fC) >= EPSILON )
		{
			Real fLength = UMath::Sqrt(fB*fB+fC*fC);
			Real fInvLength = 1.0f/fLength;
			fB *= fInvLength;
			fC *= fInvLength;
			Real fQ = 2.0f*fB*fE+fC*(fF-fD);
			afDiag[1] = fD+fC*fQ;
			afDiag[2] = fF-fC*fQ;
			afSubDiag[0] = fLength;
			afSubDiag[1] = fE-fB*fQ;
			IDX(m,0,0) = 1.0;
			IDX(m,0,1) = 0.0;
			IDX(m,0,2) = 0.0;
			IDX(m,1,0) = 0.0;
			IDX(m,1,1) = fB;
			IDX(m,1,2) = fC;
			IDX(m,2,0) = 0.0;
			IDX(m,2,1) = fC;
			IDX(m,2,2) = -fB;
		}
		else
		{
			afDiag[1] = fD;
			afDiag[2] = fF;
			afSubDiag[0] = fB;
			afSubDiag[1] = fE;
			IDX(m,0,0) = 1.0;
			IDX(m,0,1) = 0.0;
			IDX(m,0,2) = 0.0;
			IDX(m,1,0) = 0.0;
			IDX(m,1,1) = 1.0;
			IDX(m,1,2) = 0.0;
			IDX(m,2,0) = 0.0;
			IDX(m,2,1) = 0.0;
			IDX(m,2,2) = 1.0;
		}
	}
	//-----------------------------------------------------------------------
	bool Matrix3::QLAlgorithm (Real afDiag[3], Real afSubDiag[3])
	{
		// QL iteration with implicit shifting to reduce matrix from tridiagonal
		// to diagonal

		for (int i0 = 0; i0 < 3; i0++)
		{
			const unsigned int iMaxIter = 32;
			unsigned int iIter;
			for (iIter = 0; iIter < iMaxIter; iIter++)
			{
				int i1;
				for (i1 = i0; i1 <= 1; i1++)
				{
					Real fSum = UMath::Abs(afDiag[i1]) +
						UMath::Abs(afDiag[i1+1]);
					if ( UMath::Abs(afSubDiag[i1]) + fSum == fSum )
						break;
				}
				if ( i1 == i0 )
					break;

				Real fTmp0 = (afDiag[i0+1]-afDiag[i0])/(2.0f*afSubDiag[i0]);
				Real fTmp1 = UMath::Sqrt(fTmp0*fTmp0+1.0f);
				if ( fTmp0 < 0.0 )
					fTmp0 = afDiag[i1]-afDiag[i0]+afSubDiag[i0]/(fTmp0-fTmp1);
				else
					fTmp0 = afDiag[i1]-afDiag[i0]+afSubDiag[i0]/(fTmp0+fTmp1);
				Real fSin = 1.0;
				Real fCos = 1.0;
				Real fTmp2 = 0.0;
				for (int i2 = i1-1; i2 >= i0; i2--)
				{
					Real fTmp3 = fSin*afSubDiag[i2];
					Real fTmp4 = fCos*afSubDiag[i2];
					if ( UMath::Abs(fTmp3) >= UMath::Abs(fTmp0) )
					{
						fCos = fTmp0/fTmp3;
						fTmp1 = UMath::Sqrt(fCos*fCos+1.0f);
						afSubDiag[i2+1] = fTmp3*fTmp1;
						fSin = 1.0f/fTmp1;
						fCos *= fSin;
					}
					else
					{
						fSin = fTmp3/fTmp0;
						fTmp1 = UMath::Sqrt(fSin*fSin+1.0f);
						afSubDiag[i2+1] = fTmp0*fTmp1;
						fCos = 1.0f/fTmp1;
						fSin *= fCos;
					}
					fTmp0 = afDiag[i2+1]-fTmp2;
					fTmp1 = (afDiag[i2]-fTmp0)*fSin+2.0f*fTmp4*fCos;
					fTmp2 = fSin*fTmp1;
					afDiag[i2+1] = fTmp0+fTmp2;
					fTmp0 = fCos*fTmp1-fTmp4;

					for (int iRow = 0; iRow < 3; iRow++)
					{
						fTmp3 = IDX(m,iRow,i2+1);
						IDX(m,iRow,i2+1) = fSin*IDX(m,iRow,i2) + fCos*fTmp3;
						IDX(m,iRow,i2) = fCos*IDX(m,iRow,i2) - fSin*fTmp3;
					}
				}
				afDiag[i0] -= fTmp2;
				afSubDiag[i0] = fTmp0;
				afSubDiag[i1] = 0.0;
			}

			if ( iIter == iMaxIter )
			{
				// should not get here under normal circumstances
				return false;
			}
		}

		return true;
	}
	//-----------------------------------------------------------------------

#ifdef DOTNET
	void  Matrix3::EigenSolveSymmetric (Real afEigenvalue[3], ARR1_PTR(Vector3) akEigenvector) CONSTF
	{
		INSTANCE(Matrix3) kMatrix = dnonlynew Matrix3(THIS_OBJ);
		Real afSubDiag[3];
		OF(kMatrix,Tridiagonal(afEigenvalue,afSubDiag));
		OF(kMatrix,QLAlgorithm(afEigenvalue,afSubDiag));

		for (size_t i = 0; i < 3; i++)
		{
			akEigenvector[i][0] = IDX(kMatrix,0,i);
			akEigenvector[i][1] = IDX(kMatrix,1,i);
			akEigenvector[i][2] = IDX(kMatrix,2,i);
		}

		// make eigenvectors form a right--handed system
		INSTANCE(Vector3) kCross = OF(akEigenvector[1],crossProduct(akEigenvector[2]));
		Real fDet = OF( akEigenvector[0],dotProduct(kCross));
		if ( fDet < 0.0 )
		{
			akEigenvector[2][0] = - akEigenvector[2][0];
			akEigenvector[2][1] = - akEigenvector[2][1];
			akEigenvector[2][2] = - akEigenvector[2][2];
		}
	}

#else
	void Matrix3::EigenSolveSymmetric (Real afEigenvalue[3], ARR1_PTR(Vector3) akEigenvector) CONSTF
	{
		Matrix3 kMatrix = *this;
		Real afSubDiag[3];
		OF(kMatrix,Tridiagonal(afEigenvalue,afSubDiag));
		OF(kMatrix,QLAlgorithm(afEigenvalue,afSubDiag));

		for (size_t i = 0; i < 3; i++)
		{
			akEigenvector[i][0] = IDX(kMatrix,0,i);
			akEigenvector[i][1] = IDX(kMatrix,1,i);
			akEigenvector[i][2] = IDX(kMatrix,2,i);
		}

		// make eigenvectors form a right--handed system
		Vector3 kCross = akEigenvector[1].crossProduct(akEigenvector[2]);
		Real fDet = akEigenvector[0].dotProduct(kCross);
		if ( fDet < 0.0 )
		{
			akEigenvector[2][0] = - IDX(akEigenvector,2,0);
			akEigenvector[2][1] = - IDX(akEigenvector,2,1);
			akEigenvector[2][2] = - IDX(akEigenvector,2,2);
		}
	}
#endif
	//-----------------------------------------------------------------------
	void Matrix3::TensorProduct (REF_CONST(Vector3) rkU, REF_CONST(Vector3) rkV, REF(Matrix3) rkProduct)
	{
		for (size_t iRow = 0; iRow < 3; iRow++)
		{
			for (size_t iCol = 0; iCol < 3; iCol++)
				IDX(rkProduct,iRow,iCol) = rkU[iRow]*rkV[iCol];
		}
	}
	//-----------------------------------------------------------------------
}
}