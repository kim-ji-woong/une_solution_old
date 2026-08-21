#ifndef __UMATH_Vector3_H__
#define __UMATH_Vector3_H__

#include "UMathAPI.h"
#include "UMath.h"
#include "UQuaternion.h"

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
		/** Standard 3-dimensional vector.
			@remarks
				A direction in 3D space represented as distances along the 3
				orthogonal axes (x, y, z). Note that positions, directions and
				scaling factors can be represented by a vector, depending on how
				you interpret the values.
		*/
		UMATH_DECLARE_EXPORT_CLASS(Vector3)
		{
		public:
			Real x, y, z;

		public:
			inline Vector3()
			{
			}



			inline Vector3(REF_CONST(Vector3) rhs)
			{
				x = OF(rhs, x);
				y = OF(rhs, y);
				z = OF(rhs, z);
			}

			inline Vector3( const Real fX, const Real fY, const Real fZ )
				: x( fX ), y( fY ), z( fZ )
			{
			}

			inline explicit Vector3( const Real afCoordinate[3] )
				: x( afCoordinate[0] ),
				  y( afCoordinate[1] ),
				  z( afCoordinate[2] )
			{
			}

			inline explicit Vector3( const int afCoordinate[3] )
			{
				x = (Real)afCoordinate[0];
				y = (Real)afCoordinate[1];
				z = (Real)afCoordinate[2];
			}

			inline explicit Vector3( Real* const r )
				: x( r[0] ), y( r[1] ), z( r[2] )
			{
			}

			inline explicit Vector3( const Real scaler )
				: x( scaler )
				, y( scaler )
				, z( scaler )
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
			inline void swap(REF(Vector3) other)
			{
				Swap(x, OF(other,x));
				Swap(y, OF(other,y));
				Swap(z, OF(other,z));
			}

#ifndef DOTNET
			inline Real operator [] ( const size_t i ) CONSTF
			{
				assert( i < 3 );

				return *(&x+i);
			}

			inline Real& operator [] ( const size_t i )
			{
				assert( i < 3 );

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
#else

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
						}
					}
				}
			}

#endif

			/** Assigns the value of the other vector.
				@param
					rkVector The other vector
			*/
			
			inline REF(Vector3) operator = ( REF_CONST(Vector3) rkVector )
			{
				x = OF(rkVector,x);
				y = OF(rkVector,y);
				z = OF(rkVector,z);

				return THIS_OBJ;
			}

			inline REF(Vector3) operator = ( const Real fScaler )
			{
				x = fScaler;
				y = fScaler;
				z = fScaler;

				return THIS_OBJ;
			}


#ifdef DOTNET

			static bool operator == ( REF_CONST(Vector3) lhs, REF_CONST(Vector3) rkVector ) CONSTF;
			static bool operator != ( REF_CONST(Vector3) lhs, REF_CONST(Vector3) rkVector ) CONSTF;
			// arithmetic operations
			static INSTANCE(Vector3) operator + ( REF_CONST(Vector3) lhs, REF_CONST(Vector3) rkVector ) CONSTF;
			static INSTANCE(Vector3) operator - ( REF_CONST(Vector3) lhs, REF_CONST(Vector3) rkVector ) CONSTF;
			static INSTANCE(Vector3) operator * ( REF_CONST(Vector3) lhs, const Real fScalar ) CONSTF;
			static INSTANCE(Vector3) operator * ( REF_CONST(Vector3) lhs, REF_CONST(Vector3) rhs) CONSTF;
			static INSTANCE(Vector3) operator / ( REF_CONST(Vector3) lhs, const Real fScalar ) CONSTF;
			static INSTANCE(Vector3) operator / ( REF_CONST(Vector3) lhs, REF_CONST(Vector3) rhs) CONSTF;
			static REF_CONST(Vector3) operator + (REF_CONST(Vector3) lhs) CONSTF;
			static INSTANCE(Vector3) operator - (REF_CONST(Vector3) lhs) CONSTF;
			// overloaded operators to help Vector3
			static INSTANCE(Vector3) operator * ( const Real fScalar, REF_CONST(Vector3) rkVector );
			static INSTANCE(Vector3) operator / ( const Real fScalar, REF_CONST(Vector3) rkVector );
			static INSTANCE(Vector3) operator + ( REF_CONST(Vector3) lhs, const Real rhs);
			static INSTANCE(Vector3) operator + ( const Real lhs, REF_CONST(Vector3) rhs);
			static INSTANCE(Vector3) operator - ( REF_CONST(Vector3) lhs, const Real rhs);
			static INSTANCE(Vector3) operator - ( const Real lhs, REF_CONST(Vector3) rhs);

			// arithmetic updates
			static REF(Vector3) operator += ( REF_CONST(Vector3) lhs, REF_CONST(Vector3) rkVector );
			static REF(Vector3) operator += ( REF_CONST(Vector3) lhs, const Real fScalar );
			static REF(Vector3) operator -= ( REF_CONST(Vector3) lhs, REF_CONST(Vector3) rkVector );
			static REF(Vector3) operator -= ( REF_CONST(Vector3) lhs, const Real fScalar );
			static REF(Vector3) operator *= ( REF_CONST(Vector3) lhs, REF_CONST(Vector3) rkVector );
			static REF(Vector3) operator *= ( REF_CONST(Vector3) lhs, const Real fScalar );
			static REF(Vector3) operator /= ( REF_CONST(Vector3) lhs, REF_CONST(Vector3) rkVector );
			static REF(Vector3) operator /= ( REF_CONST(Vector3) lhs, const Real fScalar );

			static bool operator < ( REF_CONST(Vector3) lhs,REF_CONST(Vector3) rhs ) CONSTF;
			static bool operator > ( REF_CONST(Vector3) lhs,REF_CONST(Vector3) rhs ) CONSTF;
#else
			inline bool operator == ( REF_CONST(Vector3) rkVector ) CONSTF
			{
				return ( x == OF(rkVector,x) && y == OF(rkVector,y) && z == OF(rkVector,z) );
			}

			inline bool operator != ( REF_CONST(Vector3) rkVector ) CONSTF
			{
				return ( x != OF(rkVector,x) || y != OF(rkVector,y) || z != OF(rkVector,z) );
			}

			// arithmetic operations
			inline INSTANCE(Vector3) operator + ( REF_CONST(Vector3) rkVector ) CONSTF
			{
				return dnonlynew Vector3(
					x + OF(rkVector,x),
					y + OF(rkVector,y),
					z + OF(rkVector,z));
			}

			inline INSTANCE(Vector3) operator - ( REF_CONST(Vector3) rkVector ) CONSTF
			{
				return dnonlynew Vector3(
					x - OF(rkVector,x),
					y - OF(rkVector,y),
					z - OF(rkVector,z));
			}

			inline INSTANCE(Vector3) operator * ( const Real fScalar ) CONSTF
			{
				return dnonlynew Vector3(
					x * fScalar,
					y * fScalar,
					z * fScalar);
			}

			inline INSTANCE(Vector3) operator * ( REF_CONST(Vector3) rhs) CONSTF
			{
				return dnonlynew Vector3(
					x * OF(rhs,x),
					y * OF(rhs,y),
					z * OF(rhs,z));
			}

			inline INSTANCE(Vector3) operator / ( const Real fScalar ) CONSTF
			{
				assert( fScalar != 0.0 );

				Real fInv = 1.0f / fScalar;

				return dnonlynew Vector3(
					x * fInv,
					y * fInv,
					z * fInv);
			}

			inline INSTANCE(Vector3) operator / ( REF_CONST(Vector3) rhs) CONSTF
			{
				return dnonlynew Vector3(
					x / OF(rhs,x),
					y / OF(rhs,y),
					z / OF(rhs,z));
			}

			inline REF_CONST(Vector3) operator + () CONSTF
			{
				return THIS_OBJ;
			}

			inline INSTANCE(Vector3) operator - () CONSTF
			{
				return dnonlynew Vector3(-x, -y, -z);
			}

			// overloaded operators to help Vector3
			inline FRIEND INSTANCE(Vector3) operator * ( const Real fScalar, REF_CONST(Vector3) rkVector )
			{
				return dnonlynew Vector3(
					fScalar * OF(rkVector,x),
					fScalar * OF(rkVector,y),
					fScalar * OF(rkVector,z));
			}

			inline FRIEND INSTANCE(Vector3) operator / ( const Real fScalar, REF_CONST(Vector3) rkVector )
			{
				return dnonlynew Vector3(
					fScalar / OF(rkVector,x),
					fScalar / OF(rkVector,y),
					fScalar / OF(rkVector,z));
			}

			inline FRIEND INSTANCE(Vector3) operator + (REF_CONST(Vector3) lhs, const Real rhs)
			{
				return dnonlynew Vector3(
					OF(lhs,x) + rhs,
					OF(lhs,y) + rhs,
					OF(lhs,z) + rhs);
			}

			inline FRIEND INSTANCE(Vector3) operator + (const Real lhs, REF_CONST(Vector3) rhs)
			{
				return dnonlynew Vector3(
					lhs + OF(rhs,x),
					lhs + OF(rhs,y),
					lhs + OF(rhs,z));
			}

			inline FRIEND INSTANCE(Vector3) operator - (REF_CONST(Vector3) lhs, const Real rhs)
			{
				return dnonlynew Vector3(
					OF(lhs,x) - rhs,
					OF(lhs,y) - rhs,
					OF(lhs,z) - rhs);
			}

			inline FRIEND INSTANCE(Vector3) operator - (const Real lhs, REF_CONST(Vector3) rhs)
			{
				return dnonlynew Vector3(
					lhs - OF(rhs,x),
					lhs - OF(rhs,y),
					lhs - OF(rhs,z));
			}

			// arithmetic updates
			inline REF(Vector3) operator += ( REF_CONST(Vector3) rkVector )
			{
				x += OF(rkVector,x);
				y += OF(rkVector,y);
				z += OF(rkVector,z);

				return THIS_OBJ;
			}

			inline REF(Vector3) operator += ( const Real fScalar )
			{
				x += fScalar;
				y += fScalar;
				z += fScalar;
				return THIS_OBJ;
			}

			inline REF(Vector3) operator -= ( REF_CONST(Vector3) rkVector )
			{
				x -= OF(rkVector,x);
				y -= OF(rkVector,y);
				z -= OF(rkVector,z);

				return THIS_OBJ;
			}

			inline REF(Vector3) operator -= ( const Real fScalar )
			{
				x -= fScalar;
				y -= fScalar;
				z -= fScalar;
				return THIS_OBJ;
			}

			inline REF(Vector3) operator *= ( const Real fScalar )
			{
				x *= fScalar;
				y *= fScalar;
				z *= fScalar;
				return THIS_OBJ;
			}

			inline REF(Vector3) operator *= ( REF_CONST(Vector3) rkVector )
			{
				x *= OF(rkVector,x);
				y *= OF(rkVector,y);
				z *= OF(rkVector,z);

				return THIS_OBJ;
			}

			inline REF(Vector3) operator /= ( const Real fScalar )
			{
				assert( fScalar != 0.0 );

				Real fInv = 1.0f / fScalar;

				x *= fInv;
				y *= fInv;
				z *= fInv;

				return THIS_OBJ;
			}

			inline REF(Vector3) operator /= ( REF_CONST(Vector3) rkVector )
			{
				x /= OF(rkVector,x);
				y /= OF(rkVector,y);
				z /= OF(rkVector,z);

				return THIS_OBJ;
			}
			/** Returns true if the vector's scalar components are all greater
				that the ones of the vector it is compared against.
			*/
			inline bool operator < ( REF_CONST(Vector3) rhs ) CONSTF
			{
				if( x < OF(rhs,x) && y < OF(rhs,y) && z < OF(rhs,z) )
					return true;
				return false;
			}

			/** Returns true if the vector's scalar components are all smaller
				that the ones of the vector it is compared against.
			*/
			inline bool operator > ( REF_CONST(Vector3) rhs ) CONSTF
			{
				if( x > OF(rhs,x) && y > OF(rhs,y) && z > OF(rhs,z) )
					return true;
				return false;
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
				return UMath::Sqrt( x * x + y * y + z * z );
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
				return x * x + y * y + z * z;
			}

			/** Returns the distance to another vector.
				@warning
					This operation requires a square root and is expensive in
					terms of CPU operations. If you don't need to know the exact
					distance (e.g. for just comparing distances) use squaredDistance()
					instead.
			*/
			inline Real distance(REF_CONST(Vector3) rhs) CONSTF
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
			inline Real squaredDistance(REF_CONST(Vector3) rhs) CONSTF
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
			inline Real dotProduct(REF_CONST(Vector3) vec) CONSTF
			{
				return x * OF(vec,x) + y * OF(vec,y) + z * OF(vec,z);
			}

			/** Calculates the absolute dot (scalar) product of this vector with another.
				@remarks
					This function work similar dotProduct, except it use absolute value
					of each component of the vector to computing.
				@param
					vec Vector with which to calculate the absolute dot product (together
					with this one).
				@return
					A Real representing the absolute dot product value.
			*/
			inline Real absDotProduct(REF_CONST(Vector3) vec) CONSTF
			{
				return UMath::Abs(x * OF(vec,x)) + UMath::Abs(y * OF(vec,y)) + UMath::Abs(z * OF(vec,z));
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
				Real fLength = UMath::Sqrt( x * x + y * y + z * z );

				// Will also work for zero-sized vectors, but will change nothing
				// We're not using epsilons because we don't need to.
				// Read http://www.ogre3d.org/forums/viewtopic.php?f=4&t=61259
				if ( fLength > Real(0.0f) )
				{
					Real fInvLength = 1.0f / fLength;
					x *= fInvLength;
					y *= fInvLength;
					z *= fInvLength;
				}

				return fLength;
			}

			/** Calculates the cross-product of 2 vectors, i.e. the vector that
				lies perpendicular to them both.
				@remarks
					The cross-product is normally used to calculate the normal
					vector of a plane, by calculating the cross-product of 2
					non-equivalent vectors which lie on the plane (e.g. 2 edges
					of a triangle).
				@param
					vec Vector which, together with this one, will be used to
					calculate the cross-product.
				@return
					A vector which is the result of the cross-product. This
					vector will <b>NOT</b> be normalised, to maximise efficiency
					- call Vector3::normalise on the result if you wish this to
					be done. As for which side the resultant vector will be on, the
					returned vector will be on the side from which the arc from 'this'
					to rkVector is anticlockwise, e.g. UNIT_Y.crossProduct(UNIT_Z)
					= UNIT_X, whilst UNIT_Z.crossProduct(UNIT_Y) = -UNIT_X.
					This is because OGRE uses a right-handed coordinate system.
				@par
					For a clearer explanation, look a the left and the bottom edges
					of your monitor's screen. Assume that the first vector is the
					left edge and the second vector is the bottom edge, both of
					them starting from the lower-left corner of the screen. The
					resulting vector is going to be perpendicular to both of them
					and will go <i>inside</i> the screen, towards the cathode tube
					(assuming you're using a CRT monitor, of course).
			*/
			inline INSTANCE(Vector3) crossProduct( REF_CONST(Vector3) rkVector ) CONSTF
			{
				return dnonlynew Vector3(
					y * OF(rkVector,z) - z * OF(rkVector,y),
					z * OF(rkVector,x) - x * OF(rkVector,z),
					x * OF(rkVector,y) - y * OF(rkVector,x));
			}

			/** Returns a vector at a point half way between this and the passed
				in vector.
			*/
			inline INSTANCE(Vector3) midPoint( REF_CONST(Vector3) vec ) CONSTF
			{
				return dnonlynew Vector3(
					( x + OF(vec,x) ) * 0.5f,
					( y + OF(vec,y) ) * 0.5f,
					( z + OF(vec,z) ) * 0.5f );
			}		

			/** Sets this vector's components to the minimum of its own and the
				ones of the passed in vector.
				@remarks
					'Minimum' in this case means the combination of the lowest
					value of x, y and z from both vectors. Lowest is taken just
					numerically, not magnitude, so -1 < 0.
			*/
			inline void makeFloor( REF_CONST(Vector3) cmp )
			{
				if( OF(cmp,x) < x ) x = OF(cmp,x);
				if( OF(cmp,y) < y ) y = OF(cmp,y);
				if( OF(cmp,z) < z ) z = OF(cmp,z);
			}

			/** Sets this vector's components to the maximum of its own and the
				ones of the passed in vector.
				@remarks
					'Maximum' in this case means the combination of the highest
					value of x, y and z from both vectors. Highest is taken just
					numerically, not magnitude, so 1 > -3.
			*/
			inline void makeCeil( REF_CONST(Vector3) cmp )
			{
				if( OF(cmp,x) > x ) x = OF(cmp,x);
				if( OF(cmp,y) > y ) y = OF(cmp,y);
				if( OF(cmp,z) > z ) z = OF(cmp,z);
			}

			/** Generates a vector perpendicular to this vector (eg an 'up' vector).
				@remarks
					This method will return a vector which is perpendicular to this
					vector. There are an infinite number of possibilities but this
					method will guarantee to generate one of them. If you need more
					control you should use the Quaternion class.
			*/
			inline INSTANCE(Vector3) perpendicular(void) CONSTF
			{
				static const Real fSquareZero = (Real)(1e-06 * 1e-06);

				INSTANCE(Vector3) perp = crossProduct( Vector3::UNIT_X );

				// Check length
				if( OF( perp,squaredLength()) < fSquareZero )
				{
					/* This vector is the Y axis multiplied by a scalar, so we have
					   to use another axis.
					*/
					perp = crossProduct( Vector3::UNIT_Y );
				}
				OF(perp, normalise());

				return perp;
			}
			/** Generates a new random vector which deviates from this vector by a
				given angle in a random direction.
				@remarks
					This method assumes that the random number generator has already
					been seeded appropriately.
				@param
					angle The angle at which to deviate
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
			inline INSTANCE(Vector3) randomDeviant( REF_CONST(Radian) angle ) CONSTF
			{
				REF(Vector3) up = dnonlynew Vector3(0,0,0);
				return randomDeviant(angle, up);
			}
			inline INSTANCE(Vector3) randomDeviant( REF_CONST(Radian) angle, REF_CONST(Vector3) up) CONSTF
			{
				INSTANCE(Vector3) newUp;

				if (up == Vector3::ZERO)
				{
					// Generate an up vector
					newUp = this->perpendicular();
				}
				else
				{
					newUp = up;
				}

				// Rotate up vector by random amount around this
				INSTANCE(Quaternion) q = dnonlynew Quaternion();
				OF( q, FromAngleAxis( dnonlynew Radian(UMath::UnitRandom() * UMath::TWO_PI), THIS_OBJ ));
				newUp = q * newUp;

				// Finally rotate this by given angle around randomised up
				OF(q ,FromAngleAxis( angle, newUp ));
				return q * (THIS_OBJ);
			}

			/** Gets the angle between 2 vectors.
			@remarks
				Vectors do not have to be unit-length but must represent directions.
			*/
			inline INSTANCE(Radian) angleBetween(REF_CONST(Vector3) dest) CONSTF
			{
				Real lenProduct = length() * OF(dest, length());

				// Divide by zero check
				if(lenProduct < 1e-6f)
					lenProduct = 1e-6f;

				Real f = dotProduct(dest) / lenProduct;

				f = UMath::Clamp(f, (Real)-1.0, (Real)1.0);
				return UMath::ACos(f);

			}
			/** Gets the shortest arc quaternion to rotate this vector to the destination
				vector.
			@remarks
				If you call this with a dest vector that is close to the inverse
				of this vector, we will rotate 180 degrees around the 'fallbackAxis'
				(if specified, or a generated axis if not) since in this case
				ANY axis of rotation is valid.
			*/
			INSTANCE(Quaternion) getRotationTo(REF_CONST(Vector3) dest) CONSTF
			{
				return getRotationTo(dest, Vector3::ZERO );
			}
			INSTANCE(Quaternion) getRotationTo(REF_CONST(Vector3) dest, REF_CONST(Vector3) fallbackAxis) CONSTF
			{
				// Based on Stan Melax's article in Game Programming Gems
				INSTANCE(Quaternion) q = dnonlynew Quaternion();
				// Copy, since cannot modify local
				INSTANCE(Vector3) v0 = THIS_OBJ;
				INSTANCE(Vector3) v1 = dest;
				OF(v0 , normalise());
				OF(v1 , normalise());

				Real d = OF(v0,dotProduct(v1));
				// If dot == 1, vectors are the same
				if (d >= 1.0f)
				{
					return Quaternion::IDENTITY;
				}
				if (d < (1e-6f - 1.0f))
				{
					if (fallbackAxis != Vector3::ZERO)
					{
						// rotate 180 degrees about the fallback axis
						OF(q,FromAngleAxis(dnonlynew Radian(UMath::PI), fallbackAxis));
					}
					else
					{
						// Generate an axis
						INSTANCE(Vector3) axis = OF( Vector3::UNIT_X , crossProduct(THIS_OBJ));
						if ( OF( axis, isZeroLength()) ) // pick another if colinear
							axis = OF(Vector3::UNIT_Y, crossProduct(THIS_OBJ));
						OF( axis , normalise());
						OF(q , FromAngleAxis(dnonlynew Radian(UMath::PI), axis));
					}
				}
				else
				{
					Real s = UMath::Sqrt( (1+d)*2 );
					Real invs = 1 / s;

					INSTANCE(Vector3) c = OF( v0, crossProduct(v1));

					OF(q,x) = OF(c,x) * invs;
					OF(q,y) = OF(c,y) * invs;
					OF(q,z) = OF(c,z) * invs;
					OF(q,w) = s * 0.5f;
					OF(q, normalise());
				}
				return q;
			}

			/** Returns true if this vector is zero length. */
			inline bool isZeroLength(void) CONSTF
			{
				Real sqlen = (x * x) + (y * y) + (z * z);
				return (sqlen < (1e-06 * 1e-06));

			}

			/** As normalise, except that this vector is unaffected and the
				normalised vector is returned as a copy. */
			inline INSTANCE(Vector3) normalisedCopy(void) CONSTF
			{
				INSTANCE(Vector3) ret = THIS_OBJ;
				OF(ret,normalise());
				return ret;
			}

			/** Calculates a reflection vector to the plane with the given normal .
			@remarks NB assumes 'this' is pointing AWAY FROM the plane, invert if it is not.
			*/
			inline INSTANCE(Vector3) reflect(REF_CONST(Vector3) normal) CONSTF
			{
				return dnonlynew Vector3( THIS_OBJ - ( normal * 2 * dotProduct(normal)));
			}

			/** Returns whether this vector is within a positional tolerance
				of another vector.
			@param rhs The vector to compare with
			@param tolerance The amount that each element of the vector may vary by
				and still be considered equal
			*/
			inline bool positionEquals(REF_CONST(Vector3) rhs) CONSTF
			{
				return positionEquals(rhs, Real(1e-03));
			}
			inline bool positionEquals(REF_CONST(Vector3) rhs, Real tolerance) CONSTF
			{
				return UMath::RealEqual(x, OF(rhs,x), tolerance) &&
					UMath::RealEqual(y, OF(rhs,y), tolerance) &&
					UMath::RealEqual(z, OF(rhs,z), tolerance);

			}

			/** Returns whether this vector is within a positional tolerance
				of another vector, also take scale of the vectors into account.
			@param rhs The vector to compare with
			@param tolerance The amount (related to the scale of vectors) that distance
				of the vector may vary by and still be considered close
			*/
			inline bool positionCloses(REF_CONST(Vector3) rhs) CONSTF
			{
				return positionCloses(rhs, Real(1e-03f));
			}
			inline bool positionCloses(REF_CONST(Vector3) rhs, Real tolerance) CONSTF
			{
				return squaredDistance(rhs) <= (squaredLength() + OF(rhs,squaredLength())) * tolerance;
			}

			/** Returns whether this vector is within a directional tolerance
				of another vector.
			@param rhs The vector to compare with
			@param tolerance The maximum angle by which the vectors may vary and
				still be considered equal
			@note Both vectors should be normalised.
			*/
			inline bool directionEquals(REF_CONST(Vector3) rhs, REF_CONST(Radian) tolerance) CONSTF
			{
				Real dot = dotProduct(rhs);
				INSTANCE(Radian) angle = UMath::ACos(dot);

				return UMath::Abs(OF(angle,valueRadians())) <= OF(tolerance, valueRadians());

			}

			/// Check whether this vector contains valid values
			inline bool isNaN() CONSTF
			{
				return UMath::isNaN(x) || UMath::isNaN(y) || UMath::isNaN(z);
			}

			/// Extract the primary (dominant) axis from this direction vector
			inline INSTANCE(Vector3) primaryAxis() CONSTF
			{
				Real absx = UMath::Abs(x);
				Real absy = UMath::Abs(y);
				Real absz = UMath::Abs(z);
				if (absx > absy)
					if (absx > absz)
						return x > 0 ? Vector3::UNIT_X : Vector3::NEGATIVE_UNIT_X;
					else
						return z > 0 ? Vector3::UNIT_Z : Vector3::NEGATIVE_UNIT_Z;
				else // absx <= absy
					if (absy > absz)
						return y > 0 ? Vector3::UNIT_Y : Vector3::NEGATIVE_UNIT_Y;
					else
						return z > 0 ? Vector3::UNIT_Z : Vector3::NEGATIVE_UNIT_Z;

			}

			// special points
			static CONST INSTANCE(Vector3) ZERO				SC_VALUE(dnonlynew Vector3( Real(0), Real(0), Real(0) ));
			static CONST INSTANCE(Vector3) UNIT_X			SC_VALUE(dnonlynew Vector3( Real(1), Real(0), Real(0) ));
			static CONST INSTANCE(Vector3) UNIT_Y			SC_VALUE(dnonlynew Vector3( Real(0), Real(1), Real(0) ));
			static CONST INSTANCE(Vector3) UNIT_Z			SC_VALUE(dnonlynew Vector3( Real(0), Real(0), Real(1) ));
			static CONST INSTANCE(Vector3) NEGATIVE_UNIT_X	SC_VALUE(dnonlynew Vector3( Real(-1), Real(0), Real(0) ));
			static CONST INSTANCE(Vector3) NEGATIVE_UNIT_Y	SC_VALUE(dnonlynew Vector3( Real(0), Real(-1), Real(0) ));
			static CONST INSTANCE(Vector3) NEGATIVE_UNIT_Z  SC_VALUE(dnonlynew Vector3( Real(0), Real(0), Real(-1) ));
			static CONST INSTANCE(Vector3) UNIT_SCALE		SC_VALUE(dnonlynew Vector3( Real(1), Real(1), Real(1) ));


#ifndef DOTNET
			/** Function for writing to a stream.
			*/
			inline UMATH_API FRIEND std::ostream& operator << ( std::ostream& o, REF_CONST(Vector3) v )
			{
				o << "Vector3(" << OF(v,x) << ", " << OF(v,y) << ", " << OF(v,z) << ")";
				return o;
			}

			FRIEND _DECLARE_CLASS(Matrix4);
#endif

		};
		/** @} */
		/** @} */

	} // namespace Math

} // namespace UnE 

#endif
