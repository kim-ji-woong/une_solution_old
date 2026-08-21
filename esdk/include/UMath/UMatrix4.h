#ifndef __UMATH_Matrix4_H__
#define __UMATH_Matrix4_H__

#include "UMathAPI.h"


#include "UVector3.h"
#include "UMatrix3.h"
#include "UVector4.h"
#include "UPlane.h"

namespace UnE
{
	namespace Math
	{


		UMATH_DECLARE_EXPORT_CLASS(Matrix4)
		{
		public:
	#ifdef DOTNET
			array<Real,2>^ m;
	#else
			/// The matrix entries, indexed by [row][col].
			union {
				Real m[4][4];
				Real _m[16];
			};		
	#endif

		public:
			/** Default constructor.
				@note
					It does <b>NOT</b> initialize the matrix for efficiency.
			*/
			inline Matrix4()
			{
	#ifdef DOTNET
				m = dnonlynew array<Real,2>(4,4);	
	#endif
			}

			inline Matrix4(
				Real m00, Real m01, Real m02, Real m03,
				Real m10, Real m11, Real m12, Real m13,
				Real m20, Real m21, Real m22, Real m23,
				Real m30, Real m31, Real m32, Real m33 )
			{
	#ifdef DOTNET
				m = dnonlynew array<Real,2>(4,4);	
	#endif
				IDX(m,0,0) = m00;
				IDX(m,0,1) = m01;
				IDX(m,0,2) = m02;
				IDX(m,0,3) = m03;
				IDX(m,1,0) = m10;
				IDX(m,1,1) = m11;
				IDX(m,1,2) = m12;
				IDX(m,1,3) = m13;
				IDX(m,2,0) = m20;
				IDX(m,2,1) = m21;
				IDX(m,2,2) = m22;
				IDX(m,2,3) = m23;
				IDX(m,3,0) = m30;
				IDX(m,3,1) = m31;
				IDX(m,3,2) = m32;
				IDX(m,3,3) = m33;
			}

			/** Creates a standard 4x4 transformation matrix with a zero translation part from a rotation/scaling 3x3 matrix.
			 */

			inline Matrix4(REF_CONST(Matrix3) m3x3)
			{
	#ifdef DOTNET
				m = dnonlynew array<Real,2>(4,4);	
	#endif
				operator=(Matrix3::IDENTITY);
				operator=(m3x3);
			}

			/** Creates a standard 4x4 transformation matrix with a zero translation part from a rotation/scaling Quaternion.
			 */
		
			inline Matrix4(REF_CONST(Quaternion) rot)
			{
	#ifdef DOTNET
				m = dnonlynew array<Real,2>(4,4);	
	#endif
				INSTANCE(Matrix3) m3x3 = dnonlynew Matrix3();
				OF(rot,ToRotationMatrix(m3x3));
				operator=(Matrix3::IDENTITY);
				operator=(m3x3);
			}
		
			inline void Swap(Real lhs,Real rhs)
			{
				Real temp;
				temp = lhs;
				lhs = rhs;
				rhs = temp;
			}

			

	#ifdef DOTNET
			/** Exchange the contents of this matrix with another. 
			*/
			inline void swap(REF(Matrix4) other)
			{
				STD_SWAP(IDX(m,0,0), IDX(other,0,0));
				STD_SWAP(IDX(m,0,1), IDX(other,0,1));
				STD_SWAP(IDX(m,0,2), IDX(other,0,2));
				STD_SWAP(IDX(m,0,3), IDX(other,0,3));
				STD_SWAP(IDX(m,1,0), IDX(other,1,0));
				STD_SWAP(IDX(m,1,1), IDX(other,1,1));
				STD_SWAP(IDX(m,1,2), IDX(other,1,2));
				STD_SWAP(IDX(m,1,3), IDX(other,1,3));
				STD_SWAP(IDX(m,2,0), IDX(other,2,0));
				STD_SWAP(IDX(m,2,1), IDX(other,2,1));
				STD_SWAP(IDX(m,2,2), IDX(other,2,2));
				STD_SWAP(IDX(m,2,3), IDX(other,2,3));
				STD_SWAP(IDX(m,3,0), IDX(other,3,0));
				STD_SWAP(IDX(m,3,1), IDX(other,3,1));
				STD_SWAP(IDX(m,3,2), IDX(other,3,2));
				STD_SWAP(IDX(m,3,3), IDX(other,3,3));
			}

			property Real default[int,int]
			{   // Indexer declaration
			public:
				Real get(int iRow, int iCol) {
					// Check the index limits.
					if ((iRow < 0 || iRow >= 4) || (iCol < 0 || iCol >= 4))
						return 0;
					else
					{
						return m[iRow, iCol];
					}
					return 0;
				}
				void set(int iRow, int iCol, Real value) {
					if (!((iRow < 0 || iRow >= 4) || (iCol < 0 || iCol >= 4)))
					{
						m[iRow, iCol] = value;
						//return value;
					}
					//return value;
				}
			}
	#else		

			inline void swap(Matrix4& other)
			{
				std::swap(m[0][0], other.m[0][0]);
				std::swap(m[0][1], other.m[0][1]);
				std::swap(m[0][2], other.m[0][2]);
				std::swap(m[0][3], other.m[0][3]);
				std::swap(m[1][0], other.m[1][0]);
				std::swap(m[1][1], other.m[1][1]);
				std::swap(m[1][2], other.m[1][2]);
				std::swap(m[1][3], other.m[1][3]);
				std::swap(m[2][0], other.m[2][0]);
				std::swap(m[2][1], other.m[2][1]);
				std::swap(m[2][2], other.m[2][2]);
				std::swap(m[2][3], other.m[2][3]);
				std::swap(m[3][0], other.m[3][0]);
				std::swap(m[3][1], other.m[3][1]);
				std::swap(m[3][2], other.m[3][2]);
				std::swap(m[3][3], other.m[3][3]);
			}

			inline Real* operator [] ( size_t iRow )
			{
				assert( iRow < 4 );
				return m[iRow];
			}

			inline const Real *operator [] ( size_t iRow ) const
			{
				assert( iRow < 4 );
				return m[iRow];
			}
			
	#endif
			///Real FRIEND MINOR(REF_CONST(Matrix4) m, const size_t r0, const size_t r1, const size_t r2, const size_t c0, const size_t c1, const size_t c2);

			INSTANCE(Matrix4) concatenate(REF_CONST(Matrix4) m2) CONSTF
			{
				INSTANCE(Matrix4) r = dnonlynew Matrix4();
				IDX(r,0,0) = IDX(m,0,0) * IDX(m2,0,0) + IDX(m,0,1) * IDX(m2,1,0) + IDX(m,0,2) * IDX(m2,2,0) + IDX(m,0,3) * IDX(m2,3,0);
				IDX(r,0,1) = IDX(m,0,0) * IDX(m2,0,1) + IDX(m,0,1) * IDX(m2,1,1) + IDX(m,0,2) * IDX(m2,2,1) + IDX(m,0,3) * IDX(m2,3,1);
				IDX(r,0,2) = IDX(m,0,0) * IDX(m2,0,2) + IDX(m,0,1) * IDX(m2,1,2) + IDX(m,0,2) * IDX(m2,2,2) + IDX(m,0,3) * IDX(m2,3,2);
				IDX(r,0,3) = IDX(m,0,0) * IDX(m2,0,3) + IDX(m,0,1) * IDX(m2,1,3) + IDX(m,0,2) * IDX(m2,2,3) + IDX(m,0,3) * IDX(m2,3,3);

				IDX(r,1,0) = IDX(m,1,0) * IDX(m2,0,0) + IDX(m,1,1) * IDX(m2,1,0) + IDX(m,1,2) * IDX(m2,2,0) + IDX(m,1,3) * IDX(m2,3,0);
				IDX(r,1,1) = IDX(m,1,0) * IDX(m2,0,1) + IDX(m,1,1) * IDX(m2,1,1) + IDX(m,1,2) * IDX(m2,2,1) + IDX(m,1,3) * IDX(m2,3,1);
				IDX(r,1,2) = IDX(m,1,0) * IDX(m2,0,2) + IDX(m,1,1) * IDX(m2,1,2) + IDX(m,1,2) * IDX(m2,2,2) + IDX(m,1,3) * IDX(m2,3,2);
				IDX(r,1,3) = IDX(m,1,0) * IDX(m2,0,3) + IDX(m,1,1) * IDX(m2,1,3) + IDX(m,1,2) * IDX(m2,2,3) + IDX(m,1,3) * IDX(m2,3,3);

				IDX(r,2,0) = IDX(m,2,0) * IDX(m2,0,0) + IDX(m,2,1) * IDX(m2,1,0) + IDX(m,2,2) * IDX(m2,2,0) + IDX(m,2,3) * IDX(m2,3,0);
				IDX(r,2,1) = IDX(m,2,0) * IDX(m2,0,1) + IDX(m,2,1) * IDX(m2,1,1) + IDX(m,2,2) * IDX(m2,2,1) + IDX(m,2,3) * IDX(m2,3,1);
				IDX(r,2,2) = IDX(m,2,0) * IDX(m2,0,2) + IDX(m,2,1) * IDX(m2,1,2) + IDX(m,2,2) * IDX(m2,2,2) + IDX(m,2,3) * IDX(m2,3,2);
				IDX(r,2,3) = IDX(m,2,0) * IDX(m2,0,3) + IDX(m,2,1) * IDX(m2,1,3) + IDX(m,2,2) * IDX(m2,2,3) + IDX(m,2,3) * IDX(m2,3,3);

				IDX(r,3,0) = IDX(m,3,0) * IDX(m2,0,0) + IDX(m,3,1) * IDX(m2,1,0) + IDX(m,3,2) * IDX(m2,2,0) + IDX(m,3,3) * IDX(m2,3,0);
				IDX(r,3,1) = IDX(m,3,0) * IDX(m2,0,1) + IDX(m,3,1) * IDX(m2,1,1) + IDX(m,3,2) * IDX(m2,2,1) + IDX(m,3,3) * IDX(m2,3,1);
				IDX(r,3,2) = IDX(m,3,0) * IDX(m2,0,2) + IDX(m,3,1) * IDX(m2,1,2) + IDX(m,3,2) * IDX(m2,2,2) + IDX(m,3,3) * IDX(m2,3,2);
				IDX(r,3,3) = IDX(m,3,0) * IDX(m2,0,3) + IDX(m,3,1) * IDX(m2,1,3) + IDX(m,3,2) * IDX(m2,2,3) + IDX(m,3,3) * IDX(m2,3,3);

				return r;
			}


#ifdef DOTNET
			static INSTANCE(Matrix4) operator * ( REF_CONST(Matrix4) lm, REF_CONST(Matrix4) m2 ) CONSTF;			
			static INSTANCE(Vector3) operator * ( REF_CONST(Matrix4) lm, REF_CONST(Vector3) v ) CONSTF;
			static INSTANCE(Vector4) operator * ( REF_CONST(Matrix4) lm, REF_CONST(Vector4) v ) CONSTF;
			static INSTANCE(Plane)   operator * ( REF_CONST(Matrix4) lm, REF_CONST(Plane)  ) CONSTF;
			static INSTANCE(Matrix4) operator * ( REF_CONST(Matrix4) lm, Real scalar) CONSTF;
			static INSTANCE(Vector4) operator * ( REF_CONST(Vector4) v, REF_CONST(Matrix4) mat) CONSTF;

			static INSTANCE(Matrix4) operator + ( REF_CONST(Matrix4) lm, REF_CONST(Matrix4) m2 ) CONSTF;
			static INSTANCE(Matrix4) operator - ( REF_CONST(Matrix4) lm, REF_CONST(Matrix4) m2 ) CONSTF;
			static bool operator == ( REF_CONST(Matrix4) lm, REF_CONST(Matrix4) m2 ) CONSTF;
			static bool operator != ( REF_CONST(Matrix4) lm, REF_CONST(Matrix4) m2 ) CONSTF;	
			
#else			
			INSTANCE(Matrix4) operator * ( REF_CONST(Matrix4) m2 ) CONSTF;			
			INSTANCE(Vector3) operator * ( REF_CONST(Vector3) v ) CONSTF;
			INSTANCE(Vector4) operator * ( REF_CONST(Vector4) v ) CONSTF;
			INSTANCE(Plane)   operator * ( REF_CONST(Plane)  ) CONSTF;
			INSTANCE(Matrix4) operator * (Real scalar) CONSTF;
			INSTANCE(Matrix4) operator + ( REF_CONST(Matrix4) m2 ) CONSTF;
			INSTANCE(Matrix4) operator - ( REF_CONST(Matrix4) m2 ) CONSTF;
			bool operator == ( REF_CONST(Matrix4) m2 ) CONSTF;
			bool operator != ( REF_CONST(Matrix4) m2 ) CONSTF;			
#endif
			void operator =  ( REF_CONST(Matrix3) mat3 );


			inline INSTANCE(Matrix4) transpose(void) CONSTF
			{
				return dnonlynew Matrix4(IDX(m,0,0), IDX(m,1,0), IDX(m,2,0), IDX(m,3,0),
										IDX(m,0,1), IDX(m,1,1), IDX(m,2,1), IDX(m,3,1),
										IDX(m,0,2), IDX(m,1,2), IDX(m,2,2), IDX(m,3,2),
										IDX(m,0,3), IDX(m,1,3), IDX(m,2,3), IDX(m,3,3));
			}

			/*
			-----------------------------------------------------------------------
			Translation Transformation
			-----------------------------------------------------------------------
			*/
			/** Sets the translation transformation part of the matrix.
			*/
			inline void setTrans( REF_CONST(Vector3) v )
			{
				IDX(m,0,3) = OF(v,x);
				IDX(m,1,3) = OF(v,y);
				IDX(m,2,3) = OF(v,z);
			}

			/** Extracts the translation transformation part of the matrix.
			 */
			inline INSTANCE(Vector3) getTrans() CONSTF
			{
			  return dnonlynew Vector3(IDX(m,0,3), IDX(m,1,3), IDX(m,2,3));
			}
		

			/** Builds a translation matrix
			*/
			inline void makeTrans( REF_CONST(Vector3) v )
			{
				IDX(m,0,0) = 1.0; IDX(m,0,1) = 0.0; IDX(m,0,2) = 0.0; IDX(m,0,3) = OF(v,x);
				IDX(m,1,0) = 0.0; IDX(m,1,1) = 1.0; IDX(m,1,2) = 0.0; IDX(m,1,3) = OF(v,y);
				IDX(m,2,0) = 0.0; IDX(m,2,1) = 0.0; IDX(m,2,2) = 1.0; IDX(m,2,3) = OF(v,z);
				IDX(m,3,0) = 0.0; IDX(m,3,1) = 0.0; IDX(m,3,2) = 0.0; IDX(m,3,3) = 1.0;
			}

			inline void makeTrans( Real tx, Real ty, Real tz )
			{
				IDX(m,0,0) = 1.0; IDX(m,0,1) = 0.0; IDX(m,0,2) = 0.0; IDX(m,0,3) = tx;
				IDX(m,1,0) = 0.0; IDX(m,1,1) = 1.0; IDX(m,1,2) = 0.0; IDX(m,1,3) = ty;
				IDX(m,2,0) = 0.0; IDX(m,2,1) = 0.0; IDX(m,2,2) = 1.0; IDX(m,2,3) = tz;
				IDX(m,3,0) = 0.0; IDX(m,3,1) = 0.0; IDX(m,3,2) = 0.0; IDX(m,3,3) = 1.0;
			}

			/** Gets a translation matrix.
			*/
			inline static INSTANCE(Matrix4) getTrans( REF_CONST(Vector3) v )
			{
				INSTANCE(Matrix4) r = dnonlynew Matrix4();

				IDX(r,0,0) = 1.0; IDX(r,0,1) = 0.0; IDX(r,0,2) = 0.0; IDX(r,0,3) = OF(v,x);
				IDX(r,1,0) = 0.0; IDX(r,1,1) = 1.0; IDX(r,1,2) = 0.0; IDX(r,1,3) = OF(v,y);
				IDX(r,2,0) = 0.0; IDX(r,2,1) = 0.0; IDX(r,2,2) = 1.0; IDX(r,2,3) = OF(v,z);
				IDX(r,3,0) = 0.0; IDX(r,3,1) = 0.0; IDX(r,3,2) = 0.0; IDX(r,3,3) = 1.0;

				return r;
			}

			/** Gets a translation matrix - variation for not using a vector.
			*/
			inline static INSTANCE(Matrix4) getTrans( Real t_x, Real t_y, Real t_z )
			{
				INSTANCE(Matrix4) r = dnonlynew Matrix4();

				IDX(r,0,0) = 1.0; IDX(r,0,1) = 0.0; IDX(r,0,2) = 0.0; IDX(r,0,3) = t_x;
				IDX(r,1,0) = 0.0; IDX(r,1,1) = 1.0; IDX(r,1,2) = 0.0; IDX(r,1,3) = t_y;
				IDX(r,2,0) = 0.0; IDX(r,2,1) = 0.0; IDX(r,2,2) = 1.0; IDX(r,2,3) = t_z;
				IDX(r,3,0) = 0.0; IDX(r,3,1) = 0.0; IDX(r,3,2) = 0.0; IDX(r,3,3) = 1.0;

				return r;
			}

			/*
			-----------------------------------------------------------------------
			Scale Transformation
			-----------------------------------------------------------------------
			*/
			/** Sets the scale part of the matrix.
			*/
			inline void setScale( REF_CONST(Vector3) v )
			{
				IDX(m,0,0) = OF(v,x);
				IDX(m,1,1) = OF(v,y);
				IDX(m,2,2) = OF(v,z);
			}

			/** Gets a scale matrix.
			*/
			inline static INSTANCE(Matrix4) getScale( REF_CONST(Vector3) v )
			{
				INSTANCE(Matrix4) r = dnonlynew Matrix4();
				IDX(r,0,0) = OF(v,x); IDX(r,0,1) = 0.0; IDX(r,0,2) = 0.0; IDX(r,0,3) = 0.0;
				IDX(r,1,0) = 0.0; IDX(r,1,1) = OF(v,y); IDX(r,1,2) = 0.0; IDX(r,1,3) = 0.0;
				IDX(r,2,0) = 0.0; IDX(r,2,1) = 0.0; IDX(r,2,2) = OF(v,z); IDX(r,2,3) = 0.0;
				IDX(r,3,0) = 0.0; IDX(r,3,1) = 0.0; IDX(r,3,2) = 0.0; IDX(r,3,3) = 1.0;

				return r;
			}

			/** Gets a scale matrix - variation for not using a vector.
			*/
			inline static INSTANCE(Matrix4) getScale( Real s_x, Real s_y, Real s_z )
			{
				INSTANCE(Matrix4) r = dnonlynew Matrix4();
				IDX(r,0,0) = s_x; IDX(r,0,1) = 0.0; IDX(r,0,2) = 0.0; IDX(r,0,3) = 0.0;
				IDX(r,1,0) = 0.0; IDX(r,1,1) = s_y; IDX(r,1,2) = 0.0; IDX(r,1,3) = 0.0;
				IDX(r,2,0) = 0.0; IDX(r,2,1) = 0.0; IDX(r,2,2) = s_z; IDX(r,2,3) = 0.0;
				IDX(r,3,0) = 0.0; IDX(r,3,1) = 0.0; IDX(r,3,2) = 0.0; IDX(r,3,3) = 1.0;

				return r;
			}

			/** Extracts the rotation / scaling part of the Matrix as a 3x3 matrix. 
			@param m3x3 Destination Matrix3
			*/
			inline void extract3x3Matrix(REF(Matrix3) m3x3) CONSTF
			{
				IDX(m3x3,0,0) = IDX(m,0,0);
				IDX(m3x3,0,1) = IDX(m,0,1);
				IDX(m3x3,0,2) = IDX(m,0,2);
				IDX(m3x3,1,0) = IDX(m,1,0);
				IDX(m3x3,1,1) = IDX(m,1,1);
				IDX(m3x3,1,2) = IDX(m,1,2);
				IDX(m3x3,2,0) = IDX(m,2,0);
				IDX(m3x3,2,1) = IDX(m,2,1);
				IDX(m3x3,2,2) = IDX(m,2,2);

			}

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

			/** Determines if this matrix involves a negative scaling. */
			inline bool hasNegativeScale() CONSTF
			{
				return determinant() < 0;
			}

			/** Extracts the rotation / scaling part as a quaternion from the Matrix.
			 */
			inline INSTANCE(Quaternion) extractQuaternion() CONSTF
			{
			  INSTANCE(Matrix3) m3x3 = dnonlynew Matrix3();
			  extract3x3Matrix(m3x3);
			  return dnonlynew Quaternion(m3x3);
			}

			static CONST INSTANCE(Matrix4) ZERO				SC_VALUE(dnonlynew Matrix4(0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0));
			static CONST INSTANCE(Matrix4) ZEROAFFINE		SC_VALUE(dnonlynew Matrix4(0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1));
			static CONST INSTANCE(Matrix4) IDENTITY			SC_VALUE(dnonlynew Matrix4(1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1));

			/** Useful little matrix which takes 2D clipspace {-1, 1} to {0,1}
				and inverts the Y. */
			static CONST INSTANCE(Matrix4) CLIPSPACE2DTOIMAGESPACE SC_VALUE(dnonlynew Matrix4(0.5,0,0,0.5,0,-0.5,0,0.5,0,0,1,0,0,0,0,1));		
		


			/** Function for writing to a stream.
			*/

#ifndef DOTNET
			inline UMATH_API FRIEND std::ostream& operator << ( std::ostream& o, REF_CONST(Matrix4) mat )
			{
				o << "Matrix4(";
				for (size_t i = 0; i < 4; ++i)
				{
					o << " row" << (unsigned)i << "{";
					for(size_t j = 0; j < 4; ++j)
					{
						o << IDX(mat,i,j) << " ";
					}
					o << "}";
				}
				o << ")";
				return o;
			}
#endif
			INSTANCE(Matrix4) adjoint() CONSTF;
			Real determinant() CONSTF;
			INSTANCE(Matrix4) inverse() CONSTF;

			/** Building a Matrix4 from orientation / scale / position.
			@remarks
				Transform is performed in the order scale, rotate, translation, i.e. translation is independent
				of orientation axes, scale does not affect size of translation, rotation and scaling are always
				centered on the origin.
			*/
			void makeTransform(REF_CONST(Vector3) position, REF_CONST(Vector3) scale, REF_CONST(Quaternion) orientation);

			/** Building an inverse Matrix4 from orientation / scale / position.
			@remarks
				As makeTransform except it build the inverse given the same data as makeTransform, so
				performing -translation, -rotate, 1/scale in that order.
			*/
			void makeInverseTransform(REF_CONST(Vector3) position, REF_CONST(Vector3) scale, REF_CONST(Quaternion) orientation);

			/** Decompose a Matrix4 to orientation / scale / position.
			*/
			void decomposition(REF(Vector3) position, REF(Vector3) scale, REF(Quaternion) orientation) CONSTF;

			/** Check whether or not the matrix is affine matrix.
				@remarks
					An affine matrix is a 4x4 matrix with row 3 equal to (0, 0, 0, 1),
					e.g. no projective coefficients.
			*/
			inline bool isAffine(void) CONSTF
			{
				return ( IDX(m,3,0) == 0 && IDX(m,3,1) == 0 && IDX(m,3,2) == 0 && IDX(m,3,3) == 1 );
			}

			/** Returns the inverse of the affine matrix.
				@note
					The matrix must be an affine matrix. @see Matrix4::isAffine.
			*/
			INSTANCE(Matrix4) inverseAffine(void) CONSTF;

			/** Concatenate two affine matrices.
				@note
					The matrices must be affine matrix. @see Matrix4::isAffine.
			*/
			inline INSTANCE(Matrix4) concatenateAffine(REF_CONST(Matrix4) m2) CONSTF
			{
				assert(isAffine() && OF(m2,isAffine()));

				return dnonlynew Matrix4(
					IDX(m,0,0) * IDX(m2,0,0) + IDX(m,0,1) * IDX(m2,1,0) + IDX(m,0,2) * IDX(m2,2,0),
					IDX(m,0,0) * IDX(m2,0,1) + IDX(m,0,1) * IDX(m2,1,1) + IDX(m,0,2) * IDX(m2,2,1),
					IDX(m,0,0) * IDX(m2,0,2) + IDX(m,0,1) * IDX(m2,1,2) + IDX(m,0,2) * IDX(m2,2,2),
					IDX(m,0,0) * IDX(m2,0,3) + IDX(m,0,1) * IDX(m2,1,3) + IDX(m,0,2) * IDX(m2,2,3) + IDX(m,0,3),

					IDX(m,1,0) * IDX(m2,0,0) + IDX(m,1,1) * IDX(m2,1,0) + IDX(m,1,2) * IDX(m2,2,0),
					IDX(m,1,0) * IDX(m2,0,1) + IDX(m,1,1) * IDX(m2,1,1) + IDX(m,1,2) * IDX(m2,2,1),
					IDX(m,1,0) * IDX(m2,0,2) + IDX(m,1,1) * IDX(m2,1,2) + IDX(m,1,2) * IDX(m2,2,2),
					IDX(m,1,0) * IDX(m2,0,3) + IDX(m,1,1) * IDX(m2,1,3) + IDX(m,1,2) * IDX(m2,2,3) + IDX(m,1,3),

					IDX(m,2,0) * IDX(m2,0,0) + IDX(m,2,1) * IDX(m2,1,0) + IDX(m,2,2) * IDX(m2,2,0),
					IDX(m,2,0) * IDX(m2,0,1) + IDX(m,2,1) * IDX(m2,1,1) + IDX(m,2,2) * IDX(m2,2,1),
					IDX(m,2,0) * IDX(m2,0,2) + IDX(m,2,1) * IDX(m2,1,2) + IDX(m,2,2) * IDX(m2,2,2),
					IDX(m,2,0) * IDX(m2,0,3) + IDX(m,2,1) * IDX(m2,1,3) + IDX(m,2,2) * IDX(m2,2,3) + IDX(m,2,3),

					0, 0, 0, 1);
			}

			/** 3-D Vector transformation specially for an affine matrix.
				@remarks
					Transforms the given 3-D vector by the matrix, projecting the 
					result back into <i>w</i> = 1.
				@note
					The matrix must be an affine matrix. @see Matrix4::isAffine.
			*/
			inline INSTANCE(Vector3) transformAffine(REF_CONST(Vector3) v) CONSTF
			{
				assert(isAffine());

				return dnonlynew Vector3(
						IDX(m,0,0) * OF(v,x) + IDX(m,0,1) * OF(v,y) + IDX(m,0,2) * OF(v,z) + IDX(m,0,3), 
						IDX(m,1,0) * OF(v,x) + IDX(m,1,1) * OF(v,y) + IDX(m,1,2) * OF(v,z) + IDX(m,1,3),
						IDX(m,2,0) * OF(v,x) + IDX(m,2,1) * OF(v,y) + IDX(m,2,2) * OF(v,z) + IDX(m,2,3));
			}

			/** 4-D Vector transformation specially for an affine matrix.
				@note
					The matrix must be an affine matrix. @see Matrix4::isAffine.
			*/
			inline INSTANCE(Vector4) transformAffine(REF_CONST(Vector4) v) CONSTF
			{
				assert(isAffine());

				return dnonlynew Vector4(
					IDX(m,0,0) * OF(v,x) + IDX(m,0,1) * OF(v,y) + IDX(m,0,2) * OF(v,z) + IDX(m,0,3) * OF(v,w), 
					IDX(m,1,0) * OF(v,x) + IDX(m,1,1) * OF(v,y) + IDX(m,1,2) * OF(v,z) + IDX(m,1,3) * OF(v,w),
					IDX(m,2,0) * OF(v,x) + IDX(m,2,1) * OF(v,y) + IDX(m,2,2) * OF(v,z) + IDX(m,2,3) * OF(v,w),
					OF(v,w));
			}

		};
#ifndef DOTNET
		inline INSTANCE(Vector4) operator * (REF_CONST(Vector4) v, REF_CONST(Matrix4) mat)
		{
			return dnonlynew Vector4(
				OF(v,x)*IDX(mat,0,0) + OF(v,y)*IDX(mat,1,0) + OF(v,z)*IDX(mat,2,0) + OF(v,w)*IDX(mat,3,0),
				OF(v,x)*IDX(mat,0,1) + OF(v,y)*IDX(mat,1,1) + OF(v,z)*IDX(mat,2,1) + OF(v,w)*IDX(mat,3,1),
				OF(v,x)*IDX(mat,0,2) + OF(v,y)*IDX(mat,1,2) + OF(v,z)*IDX(mat,2,2) + OF(v,w)*IDX(mat,3,2),
				OF(v,x)*IDX(mat,0,3) + OF(v,y)*IDX(mat,1,3) + OF(v,z)*IDX(mat,2,3) + OF(v,w)*IDX(mat,3,3)
				);
		}
#endif

	}
}

#endif
