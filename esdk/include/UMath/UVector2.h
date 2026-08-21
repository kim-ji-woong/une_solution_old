
#ifndef __UMATH_Vector2_H__
#define __UMATH_Vector2_H__


#include "UMathAPI.h"
#include "UMath.h"

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
	/** Standard 2-dimensional vector.
		@remarks
			A direction in 2D space represented as distances along the 2
			orthogonal axes (x, y). Note that positions, directions and
			scaling factors can be represented by a vector, depending on how
			you interpret the values.
	*/
	UMATH_DECLARE_EXPORT_CLASS(Vector2)
	{
	public:
		Real x, y;

	public:
		inline Vector2()
		{
		}
		inline Vector2(REF_CONST(Vector2) rhs)
		{
			x = OF( rhs, x);
			y = OF( rhs, y);
		}

		inline Vector2(const Real fX, const Real fY )
			: x( fX ), y( fY )
		{
		}

		inline explicit Vector2( const Real scaler )
			: x( scaler), y( scaler )
		{
		}

		inline explicit Vector2( const Real afCoordinate[2] )
			: x( afCoordinate[0] ),
			  y( afCoordinate[1] )
		{
		}

		inline explicit Vector2( const int afCoordinate[2] )
		{
			x = (Real)afCoordinate[0];
			y = (Real)afCoordinate[1];
		}

		inline explicit Vector2( Real* const r )
			: x( r[0] ), y( r[1] )
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
		inline void swap(REF(Vector2) other)
		{
			STD_SWAP(x, OF(other,x));
			STD_SWAP(y, OF(other,y));
		}

#ifdef DOTNET
		property Real default[int]
		{   // Indexer declaration
		public:
			Real get(int index) {
				// Check the index limits.
				if (index < 0 || index >= 2)
					return 0;
				else
				{
					switch(index)
					{
					case 0:
						return x;
					case 1:
						return y;			
					}
				}
				return 0;
			}
			void set(int idx, Real value) {
				if (!(idx < 0 || idx >= 2))
				{
					switch(idx)
					{
					case 0:
						x = value;
						break;
					case 1:
						y = value;
						break;	
					}
				}
			}
		}
#else
		inline Real operator [] ( const size_t i ) const
		{
			assert( i < 2 );

			return *(&x+i);
		}

		inline Real& operator [] ( const size_t i )
		{
			assert( i < 2 );

			return *(&x+i);
		}

		/// Pointer accessor for direct copying
		Real* ptr()
		{
			return &x;
		}
		/// Pointer accessor for direct copying
		const Real* ptr() const
		{
			return &x;
		}
#endif

		/** Assigns the value of the other vector.
			@param
				rkVector The other vector
		*/
		inline REF(Vector2) operator = ( REF_CONST(Vector2) rkVector )
		{
			x = OF(rkVector,x);
			y = OF(rkVector,y);

			return THIS_OBJ;
		}

		inline REF(Vector2) operator = ( const Real fScalar)
		{
			x = fScalar;
			y = fScalar;

			return THIS_OBJ;
		}

#ifdef DOTNET	

		static bool operator == ( REF_CONST(Vector2) lhs, REF_CONST(Vector2) rkVector ) CONSTF;
		static bool operator != ( REF_CONST(Vector2) lhs, REF_CONST(Vector2) rkVector ) CONSTF;
		// arithmetic operations
		static INSTANCE(Vector2) operator + ( REF_CONST(Vector2) lhs, REF_CONST(Vector2) rkVector ) CONSTF;
		static INSTANCE(Vector2) operator - ( REF_CONST(Vector2) lhs, REF_CONST(Vector2) rkVector ) CONSTF;
		static INSTANCE(Vector2) operator * ( REF_CONST(Vector2) lhs, const Real fScalar ) CONSTF;
		static INSTANCE(Vector2) operator * ( REF_CONST(Vector2) lhs, REF_CONST(Vector2) rhs) CONSTF;

		static INSTANCE(Vector2) operator / ( REF_CONST(Vector2) lhs, const Real fScalar ) CONSTF;
		static INSTANCE(Vector2) operator / ( REF_CONST(Vector2) lhs, REF_CONST(Vector2) rhs) CONSTF;
		
		static REF_CONST(Vector2) operator + (REF_CONST(Vector2) lhs) CONSTF;
		static INSTANCE(Vector2)  operator - (REF_CONST(Vector2) lhs) CONSTF;

		// overloaded operators to help Vector2
		static INSTANCE(Vector2) operator * ( const Real fScalar, REF_CONST(Vector2) rkVector );
		static INSTANCE(Vector2) operator / ( const Real fScalar, REF_CONST(Vector2) rkVector );
		static INSTANCE(Vector2) operator + ( REF_CONST(Vector2) lhs, const Real rhs);
		static INSTANCE(Vector2) operator + ( const Real lhs, REF_CONST(Vector2) rhs);
		static INSTANCE(Vector2) operator - ( REF_CONST(Vector2) lhs, const Real rhs);
		static INSTANCE(Vector2) operator - ( const Real lhs, REF_CONST(Vector2) rhs);

		// arithmetic updates
		static REF(Vector2) operator += ( REF_CONST(Vector2) lhs, REF_CONST(Vector2) rkVector );
		static REF(Vector2) operator += ( REF_CONST(Vector2) lhs, const Real fScaler );
		static REF(Vector2) operator -= ( REF_CONST(Vector2) lhs, REF_CONST(Vector2) rkVector );
		static REF(Vector2) operator -= ( REF_CONST(Vector2) lhs, const Real fScaler );
		static REF(Vector2) operator *= ( REF_CONST(Vector2) lhs, const Real fScalar );
		static REF(Vector2) operator *= ( REF_CONST(Vector2) lhs, REF_CONST(Vector2) rkVector );
		static REF(Vector2) operator /= ( REF_CONST(Vector2) lhs, const Real fScalar );
		static REF(Vector2) operator /= ( REF_CONST(Vector2) lhs, REF_CONST(Vector2) rkVector );
#else
		inline bool operator == ( REF_CONST(Vector2) rkVector ) CONSTF
		{
			return ( x == OF(rkVector,x) && y == OF(rkVector,y) );
		}

		inline bool operator != ( REF_CONST(Vector2) rkVector ) CONSTF
		{
			return ( x != OF(rkVector,x) || y != OF(rkVector,y)  );
		}

		// arithmetic operations
		inline INSTANCE(Vector2) operator + ( REF_CONST(Vector2) rkVector ) CONSTF
		{
			return dnonlynew Vector2(x + OF(rkVector,x) , y + OF( rkVector,y) );
		}

		inline INSTANCE(Vector2) operator - ( REF_CONST(Vector2) rkVector ) CONSTF
		{
			return dnonlynew Vector2( x - OF( rkVector, x ), y - OF( rkVector, y) );
		}

		inline INSTANCE(Vector2) operator * ( const Real fScalar ) CONSTF
		{
			return dnonlynew Vector2(x * fScalar, y * fScalar);
		}

		inline INSTANCE(Vector2) operator * ( REF_CONST(Vector2) rhs) CONSTF
		{
			return dnonlynew Vector2( x * OF( rhs, x) , y * OF( rhs, y ));
		}

		inline INSTANCE(Vector2) operator / ( const Real fScalar ) CONSTF
		{
			assert( fScalar != 0.0 );

			Real fInv = 1.0f / fScalar;

			return dnonlynew Vector2(  x * fInv, y * fInv);
		}

		inline INSTANCE(Vector2) operator / ( REF_CONST(Vector2) rhs) CONSTF
		{
			return dnonlynew Vector2( x / OF(rhs,x), y / OF(rhs,y));
		}

		inline REF_CONST(Vector2) operator + () CONSTF
		{
			return THIS_OBJ;
		}

		inline INSTANCE(Vector2) operator - () CONSTF
		{
			return dnonlynew Vector2(-x, -y);
		}

		// overloaded operators to help Vector2
		inline FRIEND INSTANCE(Vector2) operator * ( const Real fScalar, REF_CONST(Vector2) rkVector )
		{
			return dnonlynew Vector2(fScalar * OF(rkVector,x), fScalar * OF(rkVector,y));
		}

		inline FRIEND INSTANCE(Vector2) operator / ( const Real fScalar, REF_CONST(Vector2) rkVector )
		{
			return dnonlynew Vector2(fScalar / OF(rkVector,x), fScalar / OF(rkVector,y));
		}

		inline FRIEND INSTANCE(Vector2) operator + (REF_CONST(Vector2) lhs, const Real rhs)
		{
			return dnonlynew Vector2( OF(lhs,x) + rhs, OF(lhs,y) + rhs);
		}

		inline FRIEND INSTANCE(Vector2) operator + (const Real lhs, REF_CONST(Vector2) rhs)
		{
			return dnonlynew Vector2(	lhs + OF(rhs,x), lhs + OF(rhs,y));
		}

		inline FRIEND INSTANCE(Vector2) operator - (REF_CONST(Vector2) lhs, const Real rhs)
		{
			return dnonlynew Vector2( OF(lhs,x) - rhs, OF(lhs,y) - rhs);
		}

		inline FRIEND INSTANCE(Vector2) operator - (const Real lhs, REF_CONST(Vector2) rhs)
		{
			return dnonlynew Vector2( lhs - OF(rhs,x), lhs - OF(rhs,y));
		}

		// arithmetic updates
		inline REF(Vector2) operator += (REF_CONST(Vector2) rkVector )
		{
			x += OF(rkVector,x);
			y += OF(rkVector,y);

			return THIS_OBJ;
		}

		inline REF(Vector2) operator += ( const Real fScaler )
		{
			x += fScaler;
			y += fScaler;

			return THIS_OBJ;
		}

		inline REF(Vector2) operator -= ( REF_CONST(Vector2) rkVector )
		{
			x -= OF(rkVector,x);
			y -= OF(rkVector,y);

			return THIS_OBJ;
		}

		inline REF(Vector2) operator -= ( const Real fScaler )
		{
			x -= fScaler;
			y -= fScaler;

			return THIS_OBJ;
		}

		inline REF(Vector2) operator *= ( const Real fScalar )
		{
			x *= fScalar;
			y *= fScalar;

			return THIS_OBJ;
		}

		inline REF(Vector2) operator *= ( REF_CONST(Vector2) rkVector )
		{
			x *= OF(rkVector,x);
			y *= OF(rkVector,y);

			return THIS_OBJ;
		}

		inline REF(Vector2) operator /= ( const Real fScalar )
		{
			assert( fScalar != 0.0 );

			Real fInv = 1.0f / fScalar;

			x *= fInv;
			y *= fInv;

			return THIS_OBJ;
		}

		inline REF(Vector2) operator /= ( REF_CONST(Vector2) rkVector )
		{
			x /= OF(rkVector,x);
			y /= OF(rkVector,y);

			return THIS_OBJ;
		}
#endif

		/** Returns the length (magnitude) of the vector.
			@warning
				This operation requires a square root and is expensive in
				terms of CPU operations. If you don't need to know the exact
				length (e.g. for just comparing lengths) use squaredLength()
				instead.
		*/
		inline Real length () CONSTF
		{
			return UMath::Sqrt( x * x + y * y );
		}

		/** Returns the square of the length(magnitude) of the vector.
			@remarks
				This  method is for efficiency - calculating the actual
				length of a vector requires a square root, which is expensive
				in terms of the operations required. This method returns the
				square of the length of the vector, i.e. the same as the
				length but before the square root is taken. Use this if you
				want to find the longest / shortest vector without incurring
				the square root.
		*/
		inline Real squaredLength () CONSTF
		{
			return x * x + y * y;
		}

		/** Returns the distance to another vector.
			@warning
				This operation requires a square root and is expensive in
				terms of CPU operations. If you don't need to know the exact
				distance (e.g. for just comparing distances) use squaredDistance()
				instead.
		*/
		inline Real distance(REF_CONST(Vector2) rhs) CONSTF
		{
			return OF((THIS_OBJ - rhs),length());
		}

		/** Returns the square of the distance to another vector.
			@remarks
				This method is for efficiency - calculating the actual
				distance to another vector requires a square root, which is
				expensive in terms of the operations required. This method
				returns the square of the distance to another vector, i.e.
				the same as the distance but before the square root is taken.
				Use this if you want to find the longest / shortest distance
				without incurring the square root.
		*/
		inline Real squaredDistance(REF_CONST(Vector2) rhs) CONSTF
		{
			return OF((THIS_OBJ - rhs),squaredLength());
		}

		/** Calculates the dot (scalar) product of this vector with another.
			@remarks
				The dot product can be used to calculate the angle between 2
				vectors. If both are unit vectors, the dot product is the
				cosine of the angle; otherwise the dot product must be
				divided by the product of the lengths of both vectors to get
				the cosine of the angle. This result can further be used to
				calculate the distance of a point from a plane.
			@param
				vec Vector with which to calculate the dot product (together
				with this one).
			@return
				A float representing the dot product value.
		*/
		inline Real dotProduct(REF_CONST(Vector2) vec) CONSTF
		{
			return x * OF(vec,x) + y * OF(vec,y);
		}

		/** Normalises the vector.
			@remarks
				This method normalises the vector such that it's
				length / magnitude is 1. The result is called a unit vector.
			@note
				This function will not crash for zero-sized vectors, but there
				will be no changes made to their components.
			@return The previous length of the vector.
		*/

		inline Real normalise()
		{
			Real fLength = UMath::Sqrt( x * x + y * y);

			// Will also work for zero-sized vectors, but will change nothing
			// We're not using epsilons because we don't need to.
			// Read http://www.ogre3d.org/forums/viewtopic.php?f=4&t=61259
			if ( fLength > Real(0.0f) )
			{
				Real fInvLength = 1.0f / fLength;
				x *= fInvLength;
				y *= fInvLength;
			}

			return fLength;
		}

		/** Returns a vector at a point half way between this and the passed
			in vector.
		*/
		inline INSTANCE(Vector2) midPoint(REF_CONST(Vector2) vec ) CONSTF
		{
			return dnonlynew Vector2( (x + OF(vec,x) ) * 0.5f, ( y + OF(vec,y) ) * 0.5f );
		}

		/** Returns true if the vector's scalar components are all greater
			that the ones of the vector it is compared against.
		*/
		inline bool operator < ( REF_CONST(Vector2) rhs ) CONSTF
		{
			if( x < OF(rhs,x) && y < OF(rhs,y) )
				return true;
			return false;
		}

		/** Returns true if the vector's scalar components are all smaller
			that the ones of the vector it is compared against.
		*/
		inline bool operator > ( REF_CONST(Vector2) rhs ) CONSTF
		{
			if( x > OF(rhs,x) && y > OF(rhs,y) )
				return true;
			return false;
		}

		/** Sets this vector's components to the minimum of its own and the
			ones of the passed in vector.
			@remarks
				'Minimum' in this case means the combination of the lowest
				value of x, y and z from both vectors. Lowest is taken just
				numerically, not magnitude, so -1 < 0.
		*/
		inline void makeFloor( REF_CONST(Vector2) cmp )
		{
			if( OF(cmp,x) < x ) 
				x = OF(cmp,x);
			if( OF(cmp,y) < y ) 
				y = OF(cmp,y);
		}

		/** Sets this vector's components to the maximum of its own and the
			ones of the passed in vector.
			@remarks
				'Maximum' in this case means the combination of the highest
				value of x, y and z from both vectors. Highest is taken just
				numerically, not magnitude, so 1 > -3.
		*/
		inline void makeCeil( REF_CONST(Vector2) cmp )
		{
			if( OF(cmp,x) > x )
				x = OF(cmp,x);
			if( OF(cmp,y) > y ) 
				y = OF(cmp,y);
		}

		/** Generates a vector perpendicular to this vector (eg an 'up' vector).
			@remarks
				This method will return a vector which is perpendicular to this
				vector. There are an infinite number of possibilities but this
				method will guarantee to generate one of them. If you need more
				control you should use the Quaternion class.
		*/
		inline INSTANCE(Vector2) perpendicular(void) CONSTF
		{
			return dnonlynew Vector2 (-y, x);
		}

		/** Calculates the 2 dimensional cross-product of 2 vectors, which results
			in a single floating point value which is 2 times the area of the triangle.
		*/
		inline Real crossProduct(REF_CONST(Vector2) rkVector ) CONSTF
		{
			return x * OF(rkVector,y) - y * OF(rkVector,x);
		}

		/** Generates a new random vector which deviates from this vector by a
			given angle in a random direction.
			@remarks
				This method assumes that the random number generator has already
				been seeded appropriately.
			@param
				angle The angle at which to deviate in radians
			@param
				up Any vector perpendicular to this one (which could generated
				by cross-product of this vector and any other non-colinear
				vector). If you choose not to provide this the function will
				derive one on it's own, however if you provide one yourself the
				function will be faster (this allows you to reuse up vectors if
				you call this method more than once)
			@return
				A random vector which deviates from this vector by angle. This
				vector will not be normalised, normalise it if you wish
				afterwards.
		*/
		inline INSTANCE(Vector2) randomDeviant(Real angle) CONSTF
		{
			angle *=  UMath::UnitRandom() * UMath::TWO_PI;
			Real cosa = cos(angle);
			Real sina = sin(angle);
			return  dnonlynew Vector2(cosa * x - sina * y, sina * x + cosa * y);
		}

		/** Returns true if this vector is zero length. */
		inline bool isZeroLength(void) CONSTF
		{
			Real sqlen = (x * x) + (y * y);
			return (sqlen < (1e-06 * 1e-06));

		}

		/** As normalise, except that this vector is unaffected and the
			normalised vector is returned as a copy. */
		inline INSTANCE(Vector2) normalisedCopy(void) CONSTF
		{
			INSTANCE(Vector2) ret = THIS_OBJ;
			OF(ret, normalise());
			return ret;
		}

		/** Calculates a reflection vector to the plane with the given normal .
		@remarks NB assumes 'this' is pointing AWAY FROM the plane, invert if it is not.
		*/
		inline INSTANCE(Vector2) reflect(REF_CONST(Vector2) normal) CONSTF
		{
			return dnonlynew Vector2( THIS_OBJ - ( (2 * dotProduct(normal)) * normal ) );
		}

		/// Check whether this vector contains valid values
		inline bool isNaN() CONSTF
		{
			return UMath::isNaN(x) || UMath::isNaN(y);
		}

		/**	 Gets the angle between 2 vectors.
		@remarks
			Vectors do not have to be unit-length but must represent directions.
		*/
		inline INSTANCE(Radian) angleBetween(REF_CONST(Vector2) other) CONSTF
		{		
			Real lenProduct = length() * OF(other,length());
			// Divide by zero check
			if(lenProduct < 1e-6f)
				lenProduct = 1e-6f;
		
			Real f = dotProduct(other) / lenProduct;	
			f = UMath::Clamp(f, (Real)-1.0, (Real)1.0);
			return UMath::ACos(f);
		}

		/**	 Gets the oriented angle between 2 vectors.
		@remarks
			Vectors do not have to be unit-length but must represent directions.
			The angle is comprised between 0 and 2 PI.
		*/
		inline INSTANCE(Radian) angleTo(REF_CONST(Vector2) other) CONSTF
		{
			INSTANCE(Radian) angle = angleBetween(other);		
			if (crossProduct(other) < 0 ) 			
				angle =  dnonlynew Radian(UMath::TWO_PI) - angle;
			return angle;
		}

		// special points
		static CONST INSTANCE(Vector2) ZERO					SC_VALUE(dnonlynew Vector2(0.0f, 0.0f));
		static CONST INSTANCE(Vector2) UNIT_X				SC_VALUE(dnonlynew Vector2(1.0f, 0.0f));
		static CONST INSTANCE(Vector2) UNIT_Y				SC_VALUE(dnonlynew Vector2(0.0f, 1.0f));
		static CONST INSTANCE(Vector2) NEGATIVE_UNIT_X		SC_VALUE(dnonlynew Vector2(-1.0f, 0.0f));
		static CONST INSTANCE(Vector2) NEGATIVE_UNIT_Y		SC_VALUE(dnonlynew Vector2(0.0f, -1.0f));
		static CONST INSTANCE(Vector2) UNIT_SCALE			SC_VALUE(dnonlynew Vector2(1.0f, 1.0f));


#ifndef DOTNET
		/** Function for writing to a stream.
		*/
		inline UMATH_API FRIEND std::ostream& operator <<( std::ostream& o, REF_CONST(Vector2) v )
		{
			o << "Vector2(" << OF(v,x) << ", " << OF(v,y) <<  ")";
			return o;
		}
#endif
	};
	/** @} */
	/** @} */
}
}
#endif
