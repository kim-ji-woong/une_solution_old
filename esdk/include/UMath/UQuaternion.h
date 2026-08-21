#ifndef __UMATH_Quaternion_H__
#define __UMATH_Quaternion_H__

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
		/** Implementation of a Quaternion, i.e. a rotation around an axis.
			For more information about Quaternions and the theory behind it, we recommend reading:
			http://www.ogre3d.org/tikiwiki/Quaternion+and+Rotation+Primer
			http://www.cprogramming.com/tutorial/3d/quaternions.html
			http://www.gamedev.net/page/resources/_/reference/programming/math-and-physics/
			quaternions/quaternion-powers-r1095
		*/
		UMATH_DECLARE_EXPORT_CLASS(Quaternion)
		{
		public:
			/// Default constructor, initializes to identity rotation (aka 0°)
			inline Quaternion ()
				: w(1), x(0), y(0), z(0)
			{
			}
			/// Construct from an explicit list of values
			inline Quaternion (Real fW, Real fX, Real fY, Real fZ)
				: w(fW), x(fX), y(fY), z(fZ)
			{
			}
			inline Quaternion(REF_CONST(Quaternion) rhs)				
			{
				w = OF( rhs, w);
				x = OF( rhs, x);
				y = OF( rhs, y);
				z = OF( rhs, z);
			}
			/// Construct a quaternion from a rotation matrix
			inline Quaternion(REF_CONST(Matrix3) rot)
			{
				FromRotationMatrix(rot);
			}
			/// Construct a quaternion from an angle/axis
			inline Quaternion(REF_CONST(Radian) rfAngle, REF_CONST(Vector3) rkAxis)
			{
				FromAngleAxis(rfAngle, rkAxis);
			}
			/// Construct a quaternion from 3 orthonormal local axes
			inline Quaternion(REF_CONST(Vector3) xaxis, REF_CONST(Vector3) yaxis, REF_CONST(Vector3) zaxis)
			{
				FromAxes(xaxis, yaxis, zaxis);
			}
			/// Construct a quaternion from 3 orthonormal local axes

#ifndef DOTNET
			inline Quaternion(PTR_CONST(Vector3) akAxis)
			{
				FromAxes(akAxis);
			}
#endif

			
			/** Exchange the contents of this quaternion with another. 
			*/
			inline void swap(REF(Quaternion) other)
			{
				STD_SWAP(w, OF(other,w));
				STD_SWAP(x, OF(other,x));
				STD_SWAP(y, OF(other,y));
				STD_SWAP(z, OF(other,z));
			}
#ifndef DOTNET
			/// Construct a quaternion from 4 manual w/x/y/z values
			inline Quaternion(POINTER(Real) valptr)
			{

				memcpy(&w, valptr, sizeof(Real)*4);

			}
			/// Array accessor operator
			inline Real operator [] ( const size_t i ) const
			{
				assert( i < 4 );

				return *(&w+i);
			}

			/// Array accessor operator
			inline Real& operator [] ( const size_t i )
			{
				assert( i < 4 );

				return *(&w+i);
			}

			/// Pointer accessor for direct copying
			inline Real* ptr()
			{
				return &w;
			}

			/// Pointer accessor for direct copying
			inline const Real* ptr() const
			{
				return &w;
			}
#else
			property Real default[int]
			{   // Indexer declaration
			public:
				Real get(int index) {
					// Check the index limits.
					if (index < 0 || index >= 4)
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
			void Swap(Real lhs,Real rhs)
			{
				Real temp;
				temp = lhs;
				lhs = rhs;
				rhs = temp;
			}
#endif

			void FromRotationMatrix (REF_CONST(Matrix3) kRot);
			void ToRotationMatrix (REF(Matrix3) kRot) CONSTF;
			/** Setups the quaternion using the supplied vector, and "roll" around
				that vector by the specified radians.
			*/
			
			void FromAngleAxis (REF_CONST(Radian) rfAngle, REF_CONST(Vector3) rkAxis);
			
			void ToAngleAxis (REF(Radian) rfAngle, REF(Vector3) rkAxis) CONSTF;
			
			void ToAngleAxis (REF(Degree) dAngle, REF(Vector3) rkAxis) CONSTF;
			/** Constructs the quaternion using 3 axes, the axes are assumed to be orthonormal
				@see FromAxes
			*/
#ifdef DOTNET
			void FromAxes (array<Vector3^>^ akAxis) CONSTF;
#else
			void FromAxes (PTR_CONST(Vector3) akAxis);
#endif
			void FromAxes (REF_CONST(Vector3) xAxis, REF_CONST(Vector3) yAxis, REF_CONST(Vector3) zAxis);
			/** Gets the 3 orthonormal axes defining the quaternion. @see FromAxes */
#ifdef DOTNET
			void ToAxes (array<Vector3^>^ akAxis) CONSTF;
#else
			void ToAxes (POINTER(Vector3) akAxis) CONSTF;
#endif
			void ToAxes (REF(Vector3) xAxis, REF(Vector3) yAxis, REF(Vector3) zAxis) CONSTF;

			/** Returns the X orthonormal axis defining the quaternion. Same as doing
				xAxis = Vector3::UNIT_X * this. Also called the local X-axis
			*/
			INSTANCE(Vector3) xAxis(void) CONSTF;

			/** Returns the Y orthonormal axis defining the quaternion. Same as doing
				yAxis = Vector3::UNIT_Y * this. Also called the local Y-axis
			*/
			INSTANCE(Vector3) yAxis(void) CONSTF;

			/** Returns the Z orthonormal axis defining the quaternion. Same as doing
				zAxis = Vector3::UNIT_Z * this. Also called the local Z-axis
			*/
			INSTANCE(Vector3) zAxis(void) CONSTF;

			inline REF_CONST(Quaternion) operator= (REF_CONST(Quaternion) rkQ)
			{
				w = OF(rkQ,w);
				x = OF(rkQ,x);
				y = OF(rkQ,y);
				z = OF(rkQ,z);
				return THIS_OBJ;
			}


#ifdef DOTNET
			static INSTANCE(Quaternion) operator+ (REF_CONST(Quaternion) mq, REF_CONST(Quaternion) rkQ) CONSTF;
			static INSTANCE(Quaternion) operator- (REF_CONST(Quaternion) mq, REF_CONST(Quaternion) rkQ) CONSTF;
			static INSTANCE(Quaternion) operator* (REF_CONST(Quaternion) mq, REF_CONST(Quaternion) rkQ) CONSTF;
			static INSTANCE(Quaternion) operator* (REF_CONST(Quaternion) mq, Real fScalar) CONSTF;
			static INSTANCE(Quaternion) operator* (Real fScalar, REF_CONST(Quaternion) rkQ);
			static INSTANCE(Quaternion) operator- (REF_CONST(Quaternion) mq) CONSTF;
			static bool operator== (REF_CONST(Quaternion) mq, REF_CONST(Quaternion) rhs) CONSTF;
			static bool operator!= (REF_CONST(Quaternion) mq, REF_CONST(Quaternion) rhs) CONSTF;
#else
			INSTANCE(Quaternion) operator+ (REF_CONST(Quaternion) rkQ) CONSTF;
			INSTANCE(Quaternion) operator- (REF_CONST(Quaternion) rkQ) CONSTF;
			INSTANCE(Quaternion) operator* (REF_CONST(Quaternion) rkQ) CONSTF;
			INSTANCE(Quaternion) operator* (Real fScalar) CONSTF;
			UMATH_API FRIEND INSTANCE(Quaternion) operator* (Real fScalar, REF_CONST(Quaternion) rkQ);

			INSTANCE(Quaternion) operator- () CONSTF;
			bool operator== (REF_CONST(Quaternion) rhs) CONSTF;
			bool operator!= (REF_CONST(Quaternion) rhs) CONSTF;
#endif
			
			// functions of a quaternion
			/// Returns the dot product of the quaternion
			Real Dot (REF_CONST(Quaternion) rkQ) CONSTF;
			/* Returns the normal length of this quaternion.
				@note This does <b>not</b> alter any values.
			*/
			Real Norm () CONSTF;
			/// Normalises this quaternion, and returns the previous length
			Real normalise(void); 
			INSTANCE(Quaternion) Inverse () CONSTF;  // apply to non-zero quaternion
			INSTANCE(Quaternion) UnitInverse () CONSTF;  // apply to unit-length quaternion
			INSTANCE(Quaternion) Exp () CONSTF;
			INSTANCE(Quaternion) Log () CONSTF;

			/// Rotation of a vector by a quaternion
			INSTANCE(Vector3) operator* (REF_CONST(Vector3) rkVector) CONSTF;

			/** Calculate the local roll element of this quaternion.
			@param reprojectAxis By default the method returns the 'intuitive' result
				that is, if you projected the local Y of the quaternion onto the X and
				Y axes, the angle between them is returned. If set to false though, the
				result is the actual yaw that will be used to implement the quaternion,
				which is the shortest possible path to get to the same orientation and 
				 may involve less axial rotation.  The co-domain of the returned value is 
				 from -180 to 180 degrees.
			*/
			INSTANCE(Radian) getRoll() CONSTF 
			{
				return getRoll(true);
			}
			INSTANCE(Radian) getRoll(bool reprojectAxis) CONSTF;
			/** Calculate the local pitch element of this quaternion
			@param reprojectAxis By default the method returns the 'intuitive' result
				that is, if you projected the local Z of the quaternion onto the X and
				Y axes, the angle between them is returned. If set to true though, the
				result is the actual yaw that will be used to implement the quaternion,
				which is the shortest possible path to get to the same orientation and 
				may involve less axial rotation.  The co-domain of the returned value is 
				from -180 to 180 degrees.
			*/
			INSTANCE(Radian) getPitch() CONSTF
			{
				return getPitch(true);
			}
			INSTANCE(Radian) getPitch(bool reprojectAxis) CONSTF;
			/** Calculate the local yaw element of this quaternion
			@param reprojectAxis By default the method returns the 'intuitive' result
				that is, if you projected the local Y of the quaternion onto the X and
				Z axes, the angle between them is returned. If set to true though, the
				result is the actual yaw that will be used to implement the quaternion,
				which is the shortest possible path to get to the same orientation and 
				may involve less axial rotation. The co-domain of the returned value is 
				from -180 to 180 degrees.
			*/
			INSTANCE(Radian) getYaw() CONSTF
			{
				return getYaw(true);
			}

			INSTANCE(Radian) getYaw(bool reprojectAxis) CONSTF;	
			/// Equality with tolerance (tolerance is max angle difference)
			bool equals(REF_CONST(Quaternion) rhs, REF_CONST(Radian) tolerance) CONSTF;
		
			/** Performs Spherical linear interpolation between two quaternions, and returns the result.
				Slerp ( 0.0f, A, B ) = A
				Slerp ( 1.0f, A, B ) = B
				@return Interpolated quaternion
				@remarks
				Slerp has the proprieties of performing the interpolation at constant
				velocity, and being torque-minimal (unless shortestPath=false).
				However, it's NOT commutative, which means
				Slerp ( 0.75f, A, B ) != Slerp ( 0.25f, B, A );
				therefore be careful if your code relies in the order of the operands.
				This is specially important in IK animation.
			*/
			static INSTANCE(Quaternion) Slerp (Real fT, REF_CONST(Quaternion) rkP, REF_CONST(Quaternion) rkQ)
			{
				return Slerp(fT, rkP, rkQ, false);			
			}
			static INSTANCE(Quaternion) Slerp (Real fT, REF_CONST(Quaternion) rkP, REF_CONST(Quaternion) rkQ, bool shortestPath);

			/** @see Slerp. It adds extra "spins" (i.e. rotates several times) specified
				by parameter 'iExtraSpins' while interpolating before arriving to the
				final values
			*/
			static INSTANCE(Quaternion) SlerpExtraSpins (Real fT, REF_CONST(Quaternion) rkP, REF_CONST(Quaternion) rkQ, int iExtraSpins);

			// setup for spherical quadratic interpolation
			static void Intermediate (REF_CONST(Quaternion) rkQ0, REF_CONST(Quaternion) rkQ1, REF_CONST(Quaternion) rkQ2,
				REF(Quaternion) rka, REF(Quaternion) rkB);

			// spherical quadratic interpolation
			static INSTANCE(Quaternion) Squad (Real fT, REF_CONST(Quaternion) rkP, REF_CONST(Quaternion) rkA, REF_CONST(Quaternion) rkB,
				REF_CONST(Quaternion) rkQ)
			{
				return Squad(fT, rkP, rkA, rkB, rkQ, false);					
			}

			static INSTANCE(Quaternion) Squad (Real fT, REF_CONST(Quaternion) rkP, REF_CONST(Quaternion) rkA, REF_CONST(Quaternion) rkB,
				REF_CONST(Quaternion) rkQ, bool shortestPath);

			/** Performs Normalised linear interpolation between two quaternions, and returns the result.
				nlerp ( 0.0f, A, B ) = A
				nlerp ( 1.0f, A, B ) = B
				@remarks
				Nlerp is faster than Slerp.
				Nlerp has the proprieties of being commutative (@see Slerp;
				commutativity is desired in certain places, like IK animation), and
				being torque-minimal (unless shortestPath=false). However, it's performing
				the interpolation at non-constant velocity; sometimes this is desired,
				sometimes it is not. Having a non-constant velocity can produce a more
				natural rotation feeling without the need of tweaking the weights; however
				if your scene relies on the timing of the rotation or assumes it will point
				at a specific angle at a specific weight value, Slerp is a better choice.
			*/
			static INSTANCE(Quaternion) nlerp(Real fT, REF_CONST(Quaternion) rkP, REF_CONST(Quaternion) rkQ)
			{
				return nlerp(fT, rkP, rkQ, false);
			}
			static INSTANCE(Quaternion) nlerp(Real fT, REF_CONST(Quaternion) rkP, REF_CONST(Quaternion) rkQ, bool shortestPath);

			/// Cutoff for sine near zero
			static CONST Real msEpsilon SC_VALUE(Real(1e-03));
			// special values	
			static CONST INSTANCE(Quaternion) ZERO SC_VALUE(dnonlynew Quaternion(Real(0),Real(0),Real(0), Real(0)));			
			static CONST INSTANCE(Quaternion) IDENTITY SC_VALUE(dnonlynew Quaternion(Real(1),Real(0),Real(0),Real(0)));

			Real w, x, y, z;

			/// Check whether this quaternion contains valid values
			inline bool isNaN() CONSTF
			{
				return UMath::isNaN(x) || UMath::isNaN(y) || UMath::isNaN(z) || UMath::isNaN(w);
			}


#ifndef DOTNET
			/** Function for writing to a stream. Outputs "Quaternion(w, x, y, z)" with w,x,y,z
				being the member values of the quaternion.
			*/
			inline UMATH_API FRIEND std::ostream& operator << ( std::ostream& o, REF_CONST(Quaternion) q )
			{
				o << "Quaternion(" << OF(q,w) << ", " << OF(q,x) << ", " << OF(q,y) << ", " << OF(q,z) << ")";
				return o;
			}
#endif



		};
		/** @} */
		/** @} */
	}
}




#endif 
