#ifndef __UMATH_Matrix3_H__
#define __UMATH_Matrix3_H__

#include "UMathAPI.h"
#include "UVector3.h"

// NB All code adapted from Wild Magic 0.2 Matrix math (free source code)
// http://www.geometrictools.com/

// NOTE.  The (x,y,z) coordinate system is assumed to be right-handed.
// Coordinate axis rotation matrices are of the form
//   RX =    1       0       0
//           0     cos(t) -sin(t)
//           0     sin(t)  cos(t)
// where t > 0 indicates a counterclockwise rotation in the yz-plane
//   RY =  cos(t)    0     sin(t)
//           0       1       0
//        -sin(t)    0     cos(t)
// where t > 0 indicates a counterclockwise rotation in the zx-plane
//   RZ =  cos(t) -sin(t)    0
//         sin(t)  cos(t)    0
//           0       0       1
// where t > 0 indicates a counterclockwise rotation in the xy-plane.

#ifdef DOTNET
using namespace System::Runtime::InteropServices;
#endif

namespace UnE
{
	namespace Math
	{
		/** \addtogroup Core
		*  @{
		*/
		/** \addtogroup Math
		*  @{
		*/
		/** A 3x3 matrix which can represent rotations around axes.
			@note
				<b>All the code is adapted from the Wild Magic 0.2 Matrix
				library (http://www.geometrictools.com/).</b>
			@par
				The coordinate system is assumed to be <b>right-handed</b>.
		*/
		UMATH_DECLARE_EXPORT_CLASS(Matrix3)
		{
		public:
			/** Default constructor.
				@note
					It does <b>NOT</b> initialize the matrix for efficiency.
			*/
			inline Matrix3 ()
			{
#ifdef DOTNET
				m = dnonlynew array<Real,2>(3, 3);
#endif
			}

#ifdef DOTNET
			inline explicit Matrix3 (array<Real,2>^ arr)
			{
				m = dnonlynew array<Real,2>(3,3);	
				m = (array<Real,2>^)arr->Clone();
			}
#endif
			inline explicit Matrix3 (const Real arr[3][3])
			{
#ifdef DOTNET
				m = dnonlynew array<Real,2>(3,3);				
#else
				memcpy(m,arr,9*sizeof(Real));
#endif
			}

			inline Matrix3 (REF_CONST(Matrix3) rkMatrix)
			{
#ifdef DOTNET
				m = dnonlynew array<Real,2>(3,3);
				IDX(m,0,0) = IDX(rkMatrix,0,0);
				IDX(m,0,1) = IDX(rkMatrix,0,1);
				IDX(m,0,2) = IDX(rkMatrix,0,2);
				IDX(m,1,0) = IDX(rkMatrix,1,0);
				IDX(m,1,1) = IDX(rkMatrix,1,1);
				IDX(m,1,2) = IDX(rkMatrix,1,2);
				IDX(m,2,0) = IDX(rkMatrix,2,0);
				IDX(m,2,1) = IDX(rkMatrix,2,1);
				IDX(m,2,2) = IDX(rkMatrix,2,2);
#else
				memcpy(m,rkMatrix.m,9*sizeof(Real));
#endif
			}
			Matrix3 (Real fEntry00, Real fEntry01, Real fEntry02,
					 Real fEntry10, Real fEntry11, Real fEntry12,
					 Real fEntry20, Real fEntry21, Real fEntry22)
			{
#ifdef DOTNET
				m = dnonlynew array<Real,2>(3,3);
#endif

				IDX(m,0,0) = fEntry00;
				IDX(m,0,1) = fEntry01;
				IDX(m,0,2) = fEntry02;
				IDX(m,1,0) = fEntry10;
				IDX(m,1,1) = fEntry11;
				IDX(m,1,2) = fEntry12;
				IDX(m,2,0) = fEntry20;
				IDX(m,2,1) = fEntry21;
				IDX(m,2,2) = fEntry22;			
			}


			inline void Swap(Real lhs,Real rhs)
			{
				Real temp;
				temp = lhs;
				lhs = rhs;
				rhs = temp;
			}
			/** Exchange the contents of this matrix with another. 
			*/
			inline void swap(REF(Matrix3) other)
			{
				STD_SWAP(IDX(m,0,0), OF(other, IDX(m,0,0)));
				STD_SWAP(IDX(m,0,1), OF(other, IDX(m,0,1)));
				STD_SWAP(IDX(m,0,2), OF(other, IDX(m,0,2)));
				STD_SWAP(IDX(m,1,0), OF(other, IDX(m,1,0)));
				STD_SWAP(IDX(m,1,1), OF(other, IDX(m,1,1)));
				STD_SWAP(IDX(m,1,2), OF(other, IDX(m,1,2)));
				STD_SWAP(IDX(m,2,0), OF(other, IDX(m,2,0)));
				STD_SWAP(IDX(m,2,1), OF(other, IDX(m,2,1)));
				STD_SWAP(IDX(m,2,2), OF(other, IDX(m,2,2)));
			}


#ifndef DOTNET
			// member access, allows use of construct mat[r][c]
			inline Real* operator[] (size_t iRow) CONSTF
			{
				return (Real*)m[iRow];
			}
#else
			property Real default[int,int]
			{   // Indexer declaration
			public:
				Real get(int iRow, int iCol) {
					// Check the index limits.
					if ((iRow < 0 || iRow >= 3) || (iCol < 0 || iCol >= 3))
						return 0;
					else
					{
						return m[iRow, iCol];
					}
					return 0;
				}
				void set(int iRow, int iCol, Real value) {
					if (!((iRow < 0 || iRow >= 3) || (iCol < 0 || iCol >= 3)))
					{
						m[iRow, iCol] = value;
						//return value;
					}
					//return value;
				}
			}
#endif
			/*inline operator Real* ()
			{
				return (Real*)m[0];
			}*/
			INSTANCE(Vector3) GetColumn (size_t iCol) CONSTF;
			void SetColumn(size_t iCol, REF_CONST(Vector3) vec);
			void FromAxes(REF_CONST(Vector3) xAxis, REF_CONST(Vector3) yAxis, REF_CONST(Vector3) zAxis);

			// assignment and comparison
			inline REF(Matrix3) operator= (REF_CONST(Matrix3) rkMatrix)
			{
#ifdef DOTNET
				for( int i = 0 ; i < 3 ; i++)
				{
					for( int j = 0 ; j < 3 ; j++)
					IDX(m, i, j) = IDX(rkMatrix, i, j);
				}
#else
				memcpy(m,rkMatrix.m,9*sizeof(Real));
#endif
				return THIS_OBJ;
			}


			

#ifdef DOTNET
			/** Tests 2 matrices for equality.
			 */
			static bool operator== (REF_CONST(Matrix3) lkMatrix, REF_CONST(Matrix3) rkMatrix) CONSTF;

			static bool operator== (REF_CONST(Matrix3) lkMatrix, System::Object^ obj ) CONSTF;

			inline static  bool operator!= (REF_CONST(Matrix3) lkMatrix, REF_CONST(Matrix3) rkMatrix) CONSTF
			{
				return !(operator==(lkMatrix, rkMatrix));
			}
			static  INSTANCE(Matrix3) operator+ (REF_CONST(Matrix3) lkMatrix, REF_CONST(Matrix3) rkMatrix) CONSTF;
			/** Matrix subtraction.			 */
			static INSTANCE(Matrix3) operator- (REF_CONST(Matrix3) lkMatrix, REF_CONST(Matrix3) rkMatrix) CONSTF;
			/** Matrix concatenation using '*'.
			 */
			static INSTANCE(Matrix3) operator* (REF_CONST(Matrix3) lkMatrix, REF_CONST(Matrix3) rkMatrix) CONSTF;
			static INSTANCE(Matrix3) operator- (REF_CONST(Matrix3) lkMatrix) CONSTF;
			/// Matrix * vector [3x3 * 3x1 = 3x1]
			static INSTANCE(Vector3) operator* (REF_CONST(Matrix3) lkMatrix, REF_CONST(Vector3) rkVector) CONSTF;	
			/// Matrix * scalar
			static INSTANCE(Matrix3) operator* (REF_CONST(Matrix3) lkMatrix, Real fScalar) CONSTF;	

			static INSTANCE(Vector3) operator* (REF_CONST(Vector3) rkVector, REF_CONST(Matrix3) rkMatrix);
			/// Scalar * matrix
			static INSTANCE(Matrix3) operator* (Real fScalar, REF_CONST(Matrix3) rkMatrix);
#else
			/** Tests 2 matrices for equality.
			 */
			bool operator== (REF_CONST(Matrix3) rkMatrix) CONSTF;
			/** Tests 2 matrices for inequality.
			 */
			inline bool operator!= (REF_CONST(Matrix3) rkMatrix) CONSTF
			{
				return !operator==(rkMatrix);
			}
			// arithmetic operations
			/** Matrix addition.
			 */			
			INSTANCE(Matrix3) operator+ (REF_CONST(Matrix3) rkMatrix) CONSTF;
			/** Matrix subtraction.			 */
			INSTANCE(Matrix3) operator- (REF_CONST(Matrix3) rkMatrix) CONSTF;
			/** Matrix concatenation using '*'.
			 */
			INSTANCE(Matrix3) operator* (REF_CONST(Matrix3) rkMatrix) CONSTF;
			INSTANCE(Matrix3) operator- () CONSTF;
			/// Matrix * vector [3x3 * 3x1 = 3x1]
			INSTANCE(Vector3) operator* (REF_CONST(Vector3) rkVector) CONSTF;	
			/// Matrix * scalar
			INSTANCE(Matrix3) operator* (Real fScalar) CONSTF;
			/// Vector * matrix [1x3 * 3x3 = 1x3]
			UMATH_API FRIEND INSTANCE(Vector3) operator* (REF_CONST(Vector3) rkVector, REF_CONST(Matrix3) rkMatrix);
			/// Scalar * matrix
			UMATH_API FRIEND INSTANCE(Matrix3) operator* (Real fScalar, REF_CONST(Matrix3) rkMatrix);
#endif

			// utilities
			INSTANCE(Matrix3) Transpose () CONSTF;

			bool Inverse (REF(Matrix3) rkInverse) CONSTF
			{
				return Inverse(rkInverse, Real(1e-06));
			}
			bool Inverse (REF(Matrix3) rkInverse, Real fTolerance) CONSTF;
			

			INSTANCE(Matrix3) Inverse () CONSTF
			{
				return Inverse(Real(1e-06));
			}
			INSTANCE(Matrix3) Inverse (Real fTolerance) CONSTF;

			Real Determinant () CONSTF;

			// singular value decomposition
			void SingularValueDecomposition (REF(Matrix3) rkL,REF(Vector3) rkS, REF(Matrix3) rkR) CONSTF;
			void SingularValueComposition (REF_CONST(Matrix3) rkL, REF_CONST(Vector3) rkS, REF_CONST(Matrix3) rkR);

			/// Gram-Schmidt orthonormalization (applied to columns of rotation matrix)
			void Orthonormalize ();

			/// Orthogonal Q, diagonal D, upper triangular U stored as (u01,u02,u12)
			void QDUDecomposition (REF(Matrix3) rkQ, REF(Vector3) rkD, REF(Vector3) rkU) CONSTF;

			Real SpectralNorm () CONSTF;

			// matrix must be orthonormal
			void ToAngleAxis (REF(Vector3) rkAxis, REF(Radian) rfAngle) CONSTF;

			inline void ToAngleAxis (REF(Vector3) rkAxis, REF(Degree) rfAngle) CONSTF
			{
				INSTANCE(Radian)  r = dnonlynew Radian();
				ToAngleAxis ( rkAxis, r );
				rfAngle = dnonlynew Degree(r);
			}
			void FromAngleAxis (REF_CONST(Vector3) rkAxis, REF_CONST(Radian) fRadians);

			// The matrix must be orthonormal.  The decomposition is yaw*pitch*roll
			// where yaw is rotation about the Up vector, pitch is rotation about the
			// Right axis, and roll is rotation about the Direction axis.
			bool ToEulerAnglesXYZ (REF(Radian) rfYAngle, REF(Radian) rfPAngle, REF(Radian) rfRAngle) CONSTF;
			bool ToEulerAnglesXZY (REF(Radian) rfYAngle, REF(Radian) rfPAngle, REF(Radian) rfRAngle) CONSTF;
			bool ToEulerAnglesYXZ (REF(Radian) rfYAngle, REF(Radian) rfPAngle, REF(Radian) rfRAngle) CONSTF;
			bool ToEulerAnglesYZX (REF(Radian) rfYAngle, REF(Radian) rfPAngle, REF(Radian) rfRAngle) CONSTF;
			bool ToEulerAnglesZXY (REF(Radian) rfYAngle, REF(Radian) rfPAngle, REF(Radian) rfRAngle) CONSTF;
			bool ToEulerAnglesZYX (REF(Radian) rfYAngle, REF(Radian) rfPAngle, REF(Radian) rfRAngle) CONSTF;

			void FromEulerAnglesXYZ (REF_CONST(Radian) fYAngle, REF_CONST(Radian) fPAngle, REF_CONST(Radian) fRAngle);
			void FromEulerAnglesXZY (REF_CONST(Radian) fYAngle, REF_CONST(Radian) fPAngle, REF_CONST(Radian) fRAngle);
			void FromEulerAnglesYXZ (REF_CONST(Radian) fYAngle, REF_CONST(Radian) fPAngle, REF_CONST(Radian) fRAngle);
			void FromEulerAnglesYZX (REF_CONST(Radian) fYAngle, REF_CONST(Radian) fPAngle, REF_CONST(Radian) fRAngle);
			void FromEulerAnglesZXY (REF_CONST(Radian) fYAngle, REF_CONST(Radian) fPAngle, REF_CONST(Radian) fRAngle);
			void FromEulerAnglesZYX (REF_CONST(Radian) fYAngle, REF_CONST(Radian) fPAngle, REF_CONST(Radian) fRAngle);
			


#ifdef DOTNET
			void EigenSolveSymmetric (Real afEigenvalue[3], ARR1_PTR(Vector3) akEigenvector) CONSTF;
#else
			/// Eigensolver, matrix must be symmetric
			void EigenSolveSymmetric (Real afEigenvalue[3], Vector3 akEigenvector[3]) CONSTF;
#endif

			static void TensorProduct (REF_CONST(Vector3) rkU, REF_CONST(Vector3) rkV, REF(Matrix3) rkProduct);

			/** Determines if this matrix involves a scaling. */
			inline bool hasScale() CONSTF
			{
				// check magnitude of column vectors (==local axes)
				Real t = IDX(m,0,0) * IDX(m,0,0) + IDX(m,1,0) * IDX(m,1,0) + IDX(m,2,0) * IDX(m,2,0);

				if (!UMath::RealEqual(t, 1.0, (Real)1e-04))
					return true;
				t = IDX(m,0,1) * IDX(m,0,1) + IDX(m,1,1) * IDX(m,1,1) + IDX(m,2,1) * IDX(m,2,1);
				if (!UMath::RealEqual(t, 1.0, (Real)1e-04))
					return true;
				t = IDX(m,0,2) * IDX(m,0,2) + IDX(m,1,2) * IDX(m,1,2) + IDX(m,2,2) * IDX(m,2,2);
				if (!UMath::RealEqual(t, 1.0, (Real)1e-04))
					return true;

				return false;
			}


#ifndef DOTNET
			/** Function for writing to a stream.
			*/
			inline UMATH_API FRIEND std::ostream& operator <<( std::ostream& o, REF_CONST(Matrix3) mat )
			{
				o << "Matrix3(" << IDX(OF(mat, m),0,0) << ", " << IDX(OF(mat, m),0,1) << ", " << IDX(OF(mat, m),0,2) << ", " 
								<< IDX(OF(mat, m),1,0) << ", " << IDX(OF(mat, m),1,1) << ", " << IDX(OF(mat, m),1,2) << ", " 
								<< IDX(OF(mat, m),2,0) << ", " << IDX(OF(mat, m),2,1) << ", " << IDX(OF(mat, m),2,2) << ")";
				return o;
			}
#endif

			static CONST Real EPSILON						SC_VALUE(Real(1e-06));
			static CONST INSTANCE(Matrix3) ZERO						SC_VALUE(dnonlynew Matrix3(0.0f, 0.0f, 0.0f, 0.f,0.f, 0.f, 0.f, 0.f, 0.f));
			static CONST INSTANCE(Matrix3)  IDENTITY		SC_VALUE(dnonlynew Matrix3(1.0f, 0.0f, 0.0f, 0.f,1.f, 0.f, 0.f, 0.f, 1.f));


		protected:
			// support for eigensolver
			void Tridiagonal (Real afDiag[3], Real afSubDiag[3]);
			bool QLAlgorithm (Real afDiag[3], Real afSubDiag[3]);
			
			// support for spectral norm
			static Real MaxCubicRoot (Real afCoeff[3]);

			static void Bidiagonalize  (REF(Matrix3) kA, REF(Matrix3) kL, REF(Matrix3) kR);
			static void GolubKahanStep (REF(Matrix3) kA, REF(Matrix3) kL, REF(Matrix3) kR);
			
			// support for singular value decomposition
			static CONST Real msSvdEpsilon					SC_VALUE(Real(1e-04));
			static CONST unsigned int msSvdMaxIterations	SC_VALUE(32);

			
#ifdef DOTNET 
			array<Real,2>^ m;
#else
			Real m[3][3];		
#endif

			// for faster access
			//FRIEND class Matrix4;
		};
		/** @} */
		/** @} */
	}
}
#endif
