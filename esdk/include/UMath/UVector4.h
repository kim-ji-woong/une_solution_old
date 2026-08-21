#ifndef __UMATH_Vector4_H__
#define __UMATH_Vector4_H__

#pragma  once

#include "UMathAPI.h"
#include "UVector3.h"

namespace UnE 
{
	namespace Math
	{
		UMATH_DECLARE_EXPORT_CLASS(Vector4)
		{
		public:
			Real x, y, z, w;

		public:
			inline Vector4()
			{
			}

			inline Vector4( const Real fX, const Real fY, const Real fZ, const Real fW )
				: x( fX ), y( fY ), z( fZ ), w( fW)
			{
			}

			inline explicit Vector4( const Real afCoordinate[4] )
				: x( afCoordinate[0] ),
				  y( afCoordinate[1] ),
				  z( afCoordinate[2] ),
				  w( afCoordinate[3] )
			{
			}

			inline explicit Vector4( const int afCoordinate[4] )
			{
				x = (Real)afCoordinate[0];
				y = (Real)afCoordinate[1];
				z = (Real)afCoordinate[2];
				w = (Real)afCoordinate[3];
			}

			inline explicit Vector4( Real* const r )
				: x( r[0] ), y( r[1] ), z( r[2] ), w( r[3] )
			{
			}

			inline explicit Vector4( const Real scaler )
				: x( scaler )
				, y( scaler )
				, z( scaler )
				, w( scaler )
			{
			}

			inline explicit Vector4(REF_CONST(Vector3) rhs)
				: x(OF(rhs,x)), y(OF(rhs,y)), z(OF(rhs,z)), w(1.0f)
			{
			}

			inline void Swap(Real lhs,Real rhs)
			{
				Real temp;
				temp = lhs;
				lhs = rhs;
				rhs = temp;
			}
			/** Exchange the contents of this vector with another. 
			*/
			inline void swap(REF(Vector4) other)
			{
				STD_SWAP(x, OF(other,x));
				STD_SWAP(y, OF(other,y));
				STD_SWAP(z, OF(other,z));
				STD_SWAP(w, OF(other,w));
			}

			/** Assigns the value of the other vector.
				@param
					rkVector The other vector
			*/
			inline REF(Vector4) operator = ( REF_CONST(Vector4) rkVector )
			{
				x = OF(rkVector,x);
				y = OF(rkVector,y);
				z = OF(rkVector,z);
				w = OF(rkVector,w);

				return THIS_OBJ;
			}

			inline REF(Vector4) operator = ( const Real fScalar)
			{
				x = fScalar;
				y = fScalar;
				z = fScalar;
				w = fScalar;
				return THIS_OBJ;
			}

			inline REF(Vector4) operator = (REF_CONST(Vector3) rhs)
			{
				x = OF(rhs,x);
				y = OF(rhs,y);
				z = OF(rhs,z);
				w = 1.0f;
				return THIS_OBJ;
			}


#ifdef DOTNET	
			property Real default[int]
			{   // Indexer declaration
			public:
				Real get(int index) {
					// Check the index limits.
					if (index < 0 || index >= 3)
						return 0;
					else
					{
						switch(index)
						{
						case 0:
							return x;
						case 1:
							return y;
						case 2:
							return z;
						case 3:
							return w;
						}
					}
					return 0;
				}
				void set(int idx, Real value) {
					if (!(idx < 0 || idx >= 3))
					{
						switch(idx)
						{
						case 0:
							x = value;
							break;
						case 1:
							y = value;
							break;							
						case 2:
							z = value;
							break;	
						case 3:
							w = value;
							break;
						}
					}
				}
			}

			static bool operator == ( REF_CONST(Vector4) lhs, REF_CONST(Vector4) rkVector ) CONSTF;
			static bool operator != ( REF_CONST(Vector4) lhs, REF_CONST(Vector4) rkVector ) CONSTF;			
			// arithmetic operations
			static INSTANCE(Vector4) operator + ( REF_CONST(Vector4) lhs, REF_CONST(Vector4) rkVector ) CONSTF;
			static INSTANCE(Vector4) operator - ( REF_CONST(Vector4) lhs, REF_CONST(Vector4) rkVector ) CONSTF;
			static INSTANCE(Vector4) operator * ( REF_CONST(Vector4) lhs, SYS_CONST(Real) fScalar ) CONSTF;
			static INSTANCE(Vector4) operator * ( REF_CONST(Vector4) lhs, REF_CONST(Vector4) rhs) CONSTF;
			static INSTANCE(Vector4) operator / ( REF_CONST(Vector4) lhs, const Real fScalar ) CONSTF;
			static INSTANCE(Vector4) operator / ( REF_CONST(Vector4) lhs, REF_CONST(Vector4) rhs) CONSTF;
			static REF_CONST(Vector4) operator + (REF_CONST(Vector4) lhs) CONSTF;
			static INSTANCE(Vector4) operator - (REF_CONST(Vector4) lhs) CONSTF;
			static INSTANCE(Vector4) operator * ( SYS_CONST(Real)fScalar, REF_CONST(Vector4) rkVector );
			static INSTANCE(Vector4) operator / ( SYS_CONST(Real) fScalar, REF_CONST(Vector4) rkVector );
			static INSTANCE(Vector4) operator + (REF_CONST(Vector4) lhs, SYS_CONST(Real) rhs);
			static INSTANCE(Vector4) operator + (SYS_CONST(Real) lhs, REF_CONST(Vector4) rhs);
			static INSTANCE(Vector4) operator - (REF_CONST(Vector4) lhs, SYS_CONST(Real) rhs);
			static INSTANCE(Vector4) operator - (SYS_CONST(Real) lhs, REF_CONST(Vector4)rhs);
			// arithmetic updates
			static REF(Vector4) operator += ( REF_CONST(Vector4) lhs, REF_CONST(Vector4) rkVector );
			static REF(Vector4) operator -= ( REF_CONST(Vector4) lhs, REF_CONST(Vector4) rkVector );
			static REF(Vector4) operator *= ( REF_CONST(Vector4) lhs, const Real fScalar );
			static REF(Vector4) operator += ( REF_CONST(Vector4) lhs, const Real fScalar );
			static REF(Vector4) operator -= ( REF_CONST(Vector4) lhs, const Real fScalar );
			static REF(Vector4) operator *= ( REF_CONST(Vector4) lhs, REF_CONST(Vector4) rkVector );
			static REF(Vector4) operator /= ( REF_CONST(Vector4) lhs, const Real fScalar );
			static REF(Vector4) operator /= ( REF_CONST(Vector4) lhs, REF_CONST(Vector4) rkVector );
#else
			inline Real operator [] ( const size_t i ) CONSTF
			{
				assert( i < 4 );
				return *(&x+i);
			}

			inline Real& operator [] ( const size_t i )
			{
				assert( i < 4 );
				return *(&x+i);	
			}
			/// Pointer accessor for direct copying
			inline Real* ptr()
			{
				return &x;
			}
			/// Pointer accessor for direct copying
			inline const Real* ptr() CONSTF
			{
				return &x;
			}
			
			inline bool operator == ( REF_CONST(Vector4) rkVector ) CONSTF
			{
				return ( x == OF(rkVector,x) &&
					y ==  OF(rkVector,y) &&
					z ==  OF(rkVector,z) &&
					w ==  OF(rkVector,w) );
			}

			inline bool operator != ( REF_CONST(Vector4) rkVector ) CONSTF
			{
				return ( x != OF(rkVector,x) ||
					y != OF(rkVector,y) ||
					z != OF(rkVector,z) ||
					w != OF(rkVector,w) );
			}
		
			// arithmetic operations
			inline INSTANCE(Vector4) operator + ( REF_CONST(Vector4) rkVector ) CONSTF
			{
				return dnonlynew Vector4(
					x + OF(rkVector,x),
					y + OF(rkVector,y),
					z + OF(rkVector,z),
					w + OF(rkVector,w));
			}

			inline INSTANCE(Vector4) operator - ( REF_CONST(Vector4) rkVector ) CONSTF
			{
				return dnonlynew Vector4(
					x - OF(rkVector,x),
					y - OF(rkVector,y),
					z - OF(rkVector,z),
					w - OF(rkVector,w));
			}

			inline INSTANCE(Vector4) operator * ( SYS_CONST(Real) fScalar ) CONSTF
			{
				return dnonlynew Vector4(
					x * fScalar,
					y * fScalar,
					z * fScalar,
					w * fScalar);
			}

			inline INSTANCE(Vector4) operator * ( REF_CONST(Vector4) rhs) CONSTF
			{
				return dnonlynew Vector4(
					OF(rhs,x) * x,
					OF(rhs,y) * y,
					OF(rhs,z) * z,
					OF(rhs,w) * w);
			}

			inline INSTANCE(Vector4) operator / ( const Real fScalar ) CONSTF
			{
				assert( fScalar != 0.0 );

				Real fInv = 1.0f / fScalar;

				return dnonlynew Vector4(
					x * fInv,
					y * fInv,
					z * fInv,
					w * fInv);
			}

			inline INSTANCE(Vector4) operator / ( REF_CONST(Vector4) rhs) CONSTF
			{
				return dnonlynew Vector4(
					x / OF(rhs,x),
					y / OF(rhs,y),
					z / OF(rhs,z),
					w / OF(rhs,w));
			}

			inline REF_CONST(Vector4) operator + () CONSTF
			{
				return THIS_OBJ;
			}

			inline INSTANCE(Vector4) operator - () CONSTF
			{
				return dnonlynew Vector4(-x, -y, -z, -w);
			}

			inline FRIEND INSTANCE(Vector4) operator * ( SYS_CONST(Real)fScalar, REF_CONST(Vector4) rkVector )
			{
				return dnonlynew Vector4(
					fScalar * OF(rkVector,x),
					fScalar * OF(rkVector,y),
					fScalar * OF(rkVector,z),
					fScalar * OF(rkVector,w));
			}

			inline FRIEND INSTANCE(Vector4) operator / ( SYS_CONST(Real) fScalar, REF_CONST(Vector4) rkVector )
			{
				return dnonlynew Vector4(
					fScalar / OF(rkVector,x),
					fScalar / OF(rkVector,y),
					fScalar / OF(rkVector,z),
					fScalar / OF(rkVector,w));
			}

			inline FRIEND INSTANCE(Vector4) operator + (REF_CONST(Vector4) lhs, SYS_CONST(Real) rhs)
			{
				return dnonlynew Vector4(
					OF(lhs,x) + rhs,
					OF(lhs,y) + rhs,
					OF(lhs,z) + rhs,
					OF(lhs,w) + rhs);
			}

			inline FRIEND INSTANCE(Vector4) operator + (SYS_CONST(Real) lhs, REF_CONST(Vector4) rhs)
			{
				return dnonlynew Vector4(
					lhs + OF(rhs,x),
					lhs + OF(rhs,y),
					lhs + OF(rhs,z),
					lhs + OF(rhs,w));
			}

			inline FRIEND INSTANCE(Vector4) operator - (REF_CONST(Vector4) lhs, SYS_CONST(Real) rhs)
			{
				return dnonlynew Vector4(
					OF(lhs,x) - rhs,
					OF(lhs,y) - rhs,
					OF(lhs,z) - rhs,
					OF(lhs,w) - rhs);
			}

			inline FRIEND INSTANCE(Vector4) operator - (SYS_CONST(Real) lhs, REF_CONST(Vector4)rhs)
			{
				return dnonlynew Vector4(
					lhs - OF(rhs,x),
					lhs - OF(rhs,y),
					lhs - OF(rhs,z),
					lhs - OF(rhs,w));
			}

			// arithmetic updates
			inline REF(Vector4) operator += ( REF_CONST(Vector4) rkVector )
			{
				x += OF(rkVector,x);
				y += OF(rkVector,y);
				z += OF(rkVector,z);
				w += OF(rkVector,w);

				return THIS_OBJ;
			}

			inline REF(Vector4) operator -= ( REF_CONST(Vector4) rkVector )
			{
				x -= OF(rkVector,x);
				y -= OF(rkVector,y);
				z -= OF(rkVector,z);
				w -= OF(rkVector,w);

				return THIS_OBJ;
			}

			inline REF(Vector4) operator *= ( const Real fScalar )
			{
				x *= fScalar;
				y *= fScalar;
				z *= fScalar;
				w *= fScalar;
				return THIS_OBJ;
			}

			inline REF(Vector4) operator += ( const Real fScalar )
			{
				x += fScalar;
				y += fScalar;
				z += fScalar;
				w += fScalar;
				return THIS_OBJ;
			}

			inline REF(Vector4) operator -= ( const Real fScalar )
			{
				x -= fScalar;
				y -= fScalar;
				z -= fScalar;
				w -= fScalar;
				return THIS_OBJ;
			}

			inline REF(Vector4) operator *= ( REF_CONST(Vector4) rkVector )
			{
				x *= OF(rkVector,x);
				y *= OF(rkVector,y);
				z *= OF(rkVector,z);
				w *= OF(rkVector,w);

				return THIS_OBJ;
			}

			inline REF(Vector4) operator /= ( const Real fScalar )
			{
				assert( fScalar != 0.0 );

				Real fInv = 1.0f / fScalar;

				x *= fInv;
				y *= fInv;
				z *= fInv;
				w *= fInv;

				return THIS_OBJ;
			}

			inline REF(Vector4) operator /= ( REF_CONST(Vector4) rkVector )
			{
				x /= OF(rkVector,x);
				y /= OF(rkVector,y);
				z /= OF(rkVector,z);
				w /= OF(rkVector,w);

				return THIS_OBJ;
			}
#endif


			/** Calculates the dot (scalar) product of this vector with another.
				@param
					vec Vector with which to calculate the dot product (together
					with this one).
				@return
					A float representing the dot product value.
			*/
			inline Real dotProduct(REF_CONST(Vector4) vec) CONSTF
			{
				return x * OF(vec,x) + y * OF(vec,y) + z * OF(vec,z) + w * OF(vec,w);
			}
			/// Check whether this vector contains valid values
			inline bool isNaN() CONSTF
			{
				return UMath::isNaN(x) || UMath::isNaN(y) || UMath::isNaN(z) || UMath::isNaN(w);
			}

#ifndef DOTNET
			/** Function for writing to a stream.
			*/
			inline UMATH_API FRIEND std::ostream& operator << ( std::ostream& o, REF_CONST(Vector4) v )
			{
				o << "Vector4(" << OF(v,x) << ", " << OF(v,y) << ", " << OF(v,z) << ", " << OF(v,w) << ")";
				return o;
			}
#endif
			// special
			static CONST INSTANCE(Vector4) ZERO			SC_VALUE(dnonlynew Vector4(Real(0), Real(0), Real(0), Real(0))) ;
		};
		/** @} */
		/** @} */
	}

}
#endif

