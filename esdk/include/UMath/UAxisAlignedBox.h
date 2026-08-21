
#ifndef __UMATH_AxisAlignedBox_H_
#define __UMATH_AxisAlignedBox_H_

#include "UMathAPI.h"

#include "UVector3.h"
#include "UMatrix4.h"


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

		/** A 3D box aligned with the x/y/z axes.
		@remarks
		This class represents a simple box which is aligned with the
		axes. Internally it only stores 2 points as the extremeties of
		the box, one which is the minima of all 3 axes, and the other
		which is the maxima of all 3 axes. This class is typically used
		for an axis-aligned bounding box (AABB) for collision and
		visibility determination.
		*/
		
		UMATH_DECLARE_EXPORT_CLASS(AxisAlignedBox)
		{
		public:
			ENUM_CLASS Extent
			{
				EXTENT_NULL,
				EXTENT_FINITE,
				EXTENT_INFINITE
			};
			
			/*
			1-----2
			/|    /|
			/ |   / |
			5-----4  |
			|  0--|--3
			| /   | /
			|/    |/
			6-----7
			*/
			ENUM_CLASS CornerEnum {
				FAR_LEFT_BOTTOM = 0,
				FAR_LEFT_TOP = 1,
				FAR_RIGHT_TOP = 2,
				FAR_RIGHT_BOTTOM = 3,
				NEAR_RIGHT_BOTTOM = 7,
				NEAR_LEFT_BOTTOM = 6,
				NEAR_LEFT_TOP = 5,
				NEAR_RIGHT_TOP = 4
			};

		protected:

			INSTANCE(Vector3) mMinimum;
			INSTANCE(Vector3) mMaximum;
			Extent mExtent;

#ifdef DOTNET
			array<Vector3^>^ mCorners;
#else
			mutable Vector3* mCorners;
#endif

		public:

			inline AxisAlignedBox() 
			{
				mMinimum = dnonlynew Vector3(Vector3::ZERO);
				mMaximum = dnonlynew Vector3(Vector3::UNIT_SCALE);
				mCorners = NULL_PTR;
				// Default to a null box 
				setMinimum( -0.5, -0.5, -0.5 );
				setMaximum( 0.5, 0.5, 0.5 );
				mExtent = ENUM_OF(Extent,EXTENT_NULL);
			}
			inline AxisAlignedBox(Extent e) 
			{
				mMinimum = dnonlynew Vector3(Vector3::ZERO);
				mMaximum = dnonlynew Vector3(Vector3::UNIT_SCALE);
				mCorners = NULL_PTR;

				setMinimum( -0.5, -0.5, -0.5 );
				setMaximum( 0.5, 0.5, 0.5 );
				mExtent = e;
			}

			inline AxisAlignedBox(REF_CONST(AxisAlignedBox) rkBox)
			{
				mMinimum = dnonlynew Vector3(Vector3::ZERO);
				mMaximum = dnonlynew Vector3(Vector3::UNIT_SCALE);
				mCorners = NULL_PTR;
				if (OF(rkBox,isNull()))
					setNull();
				else if (OF(rkBox,isInfinite()))
					setInfinite();
				else
					setExtents( OF(rkBox,mMinimum), OF(rkBox,mMaximum) );
			}

			inline AxisAlignedBox( REF_CONST(Vector3) min, REF_CONST(Vector3) max ) 
			{
				mMinimum = dnonlynew Vector3(Vector3::ZERO);
				mMaximum = dnonlynew Vector3(Vector3::UNIT_SCALE);
				mCorners = NULL_PTR;
				setExtents( min, max );
			}

			inline AxisAlignedBox(
				Real mx, Real my, Real mz,
				Real Mx, Real My, Real Mz )
			{
				mMinimum = dnonlynew Vector3(Vector3::ZERO);
				mMaximum = dnonlynew Vector3(Vector3::UNIT_SCALE);
				mCorners = NULL_PTR;
				setExtents( mx, my, mz, Mx, My, Mz );
			}

			REF(AxisAlignedBox) operator=(REF_CONST(AxisAlignedBox) rhs)
			{
				// Specifically override to avoid copying mCorners
				if (OF(rhs,isNull()))
					setNull();
				else if (OF(rhs,isInfinite()))
					setInfinite();
				else
					setExtents(OF(rhs,mMinimum), OF(rhs,mMaximum));

				return THIS_OBJ;
			}

			

#ifndef DOTNET

			~AxisAlignedBox()
			{
				if (mCorners)
					delete [] mCorners;

			}

			/** Gets the minimum corner of the box.
			*/
			inline REF_CONST(Vector3) getMinimum(void) CONSTF
			{ 
				return mMinimum; 
			}
			/** Gets the maximum corner of the box.
			*/
			inline REF_CONST(Vector3) getMaximum(void) CONSTF
			{ 
				return mMaximum;
			}
#endif

			/** Gets a modifiable version of the minimum
			corner of the box.
			*/
			inline REF(Vector3) getMinimum(void)
			{ 
				return mMinimum; 
			}			

			/** Gets a modifiable version of the maximum
			corner of the box.
			*/
			inline REF(Vector3) getMaximum(void)
			{ 
				return mMaximum;
			}


			/** Sets the minimum corner of the box.
			*/
			inline void setMinimum( REF_CONST(Vector3) vec )
			{
				mExtent = ENUM_OF(Extent,EXTENT_FINITE);
				mMinimum = vec;
			}

			inline void setMinimum( Real x, Real y, Real z )
			{
				mExtent = ENUM_OF(Extent,EXTENT_FINITE);
				OF(mMinimum,x) = x;
				OF(mMinimum,y) = y;
				OF(mMinimum,z) = z;
			}

			/** Changes one of the components of the minimum corner of the box
			used to resize only one dimension of the box
			*/
			inline void setMinimumX(Real x)
			{
				OF(mMinimum,x) = x;
			}

			inline void setMinimumY(Real y)
			{
				OF(mMinimum,y) = y;
			}

			inline void setMinimumZ(Real z)
			{
				OF(mMinimum,z) = z;
			}

			/** Sets the maximum corner of the box.
			*/
			inline void setMaximum( REF_CONST(Vector3) vec )
			{
				mExtent = ENUM_OF(Extent,EXTENT_FINITE);
				mMaximum = vec;
			}

			inline void setMaximum( Real x, Real y, Real z )
			{
				mExtent = ENUM_OF(Extent,EXTENT_FINITE);
				OF(mMaximum,x) = x;
				OF(mMaximum,y) = y;
				OF(mMaximum,z) = z;
			}

			/** Changes one of the components of the maximum corner of the box
			used to resize only one dimension of the box
			*/
			inline void setMaximumX( Real x )
			{
				OF(mMaximum,x) = x;
			}

			inline void setMaximumY( Real y )
			{
				OF(mMaximum,y) = y;
			}

			inline void setMaximumZ( Real z )
			{
				OF(mMaximum,z) = z;
			}

			/** Sets both minimum and maximum extents at once.
			*/
			inline void setExtents( REF_CONST(Vector3) min, REF_CONST(Vector3) max )
			{
				assert( (OF(min,x) <= OF(max,x) && OF(min,y) <= OF(max,y) && OF(min,z) <= OF(max,z)) &&
					"The minimum corner of the box must be less than or equal to maximum corner" );

				mExtent = ENUM_OF(Extent,EXTENT_FINITE);
				mMinimum = min;
				mMaximum = max;
			}

			inline void setExtents(
				Real mx, Real my, Real mz,
				Real Mx, Real My, Real Mz )
			{
				assert( (mx <= Mx && my <= My && mz <= Mz) &&
					"The minimum corner of the box must be less than or equal to maximum corner" );

				mExtent = ENUM_OF(Extent,EXTENT_FINITE);

				OF(mMinimum,x) = mx;
				OF(mMinimum,y) = my;
				OF(mMinimum,z) = mz;

				OF(mMaximum,x) = Mx;
				OF(mMaximum,y) = My;
				OF(mMaximum,z) = Mz;

			}

			/** Returns a pointer to an array of 8 corner points, useful for
			collision vs. non-aligned objects.
			@remarks
			If the order of these corners is important, they are as
			follows: The 4 points of the minimum Z face (note that
			because Ogre uses right-handed coordinates, the minimum Z is
			at the 'back' of the box) starting with the minimum point of
			all, then anticlockwise around this face (if you are looking
			onto the face from outside the box). Then the 4 points of the
			maximum Z face, starting with maximum point of all, then
			anticlockwise around this face (looking onto the face from
			outside the box). Like this:
			<pre>
			1-----2
			/|    /|
			/ |   / |
			5-----4  |
			|  0--|--3
			| /   | /
			|/    |/
			6-----7
			</pre>
			@remarks as this implementation uses a static member, make sure to use your own copy !
			*/

#ifdef DOTNET 
			inline array<Vector3^>^ getAllCorners(void) CONSTF
			{
				assert( (mExtent == Extent::EXTENT_FINITE) && "Can't get corners of a null or infinite AAB" );

				// The order of these items is, using right-handed co-ordinates:
				// Minimum Z face, starting with Min(all), then anticlockwise
				//   around face (looking onto the face)
				// Maximum Z face, starting with Max(all), then anticlockwise
				//   around face (looking onto the face)
				// Only for optimization/compatibility.
				if (mCorners == NULL_PTR)
					mCorners = dnonlynew array<Vector3^>(8);
				

				mCorners[0] = dnonlynew Vector3(mMinimum);
				mCorners[1] = dnonlynew Vector3(OF(mMinimum,x), OF(mMaximum,y), OF(mMinimum,z));
				mCorners[2] = dnonlynew Vector3(OF(mMaximum,x), OF(mMaximum,y), OF(mMinimum,z));
				mCorners[3] = dnonlynew Vector3(OF(mMaximum,x), OF(mMinimum,y), OF(mMinimum,z));       

				mCorners[4] = dnonlynew Vector3(mMaximum);
				mCorners[5] = dnonlynew Vector3(OF(mMinimum,x), OF(mMaximum,y), OF(mMaximum,z));       
				mCorners[6] = dnonlynew Vector3(OF(mMinimum,x), OF(mMinimum,y), OF(mMaximum,z));  
				mCorners[7] = dnonlynew Vector3(OF(mMaximum,x), OF(mMinimum,y), OF(mMaximum,z));  

				return mCorners;
			}
#else
			inline const Vector3* getAllCorners(void) CONSTF
			{
				assert( (mExtent == ENUM_OF(Extent,EXTENT_FINITE)) && "Can't get corners of a null or infinite AAB" );

				// The order of these items is, using right-handed co-ordinates:
				// Minimum Z face, starting with Min(all), then anticlockwise
				//   around face (looking onto the face)
				// Maximum Z face, starting with Max(all), then anticlockwise
				//   around face (looking onto the face)
				// Only for optimization/compatibility.
				if (!mCorners)
					mCorners = new  Vector3[8];

				mCorners[0] = mMinimum;
				mCorners[1].x = OF(mMinimum,x); mCorners[1].y = OF(mMaximum,y); mCorners[1].z = OF(mMinimum,z);
				mCorners[2].x = OF(mMaximum,x); mCorners[2].y = OF(mMaximum,y); mCorners[2].z = OF(mMinimum,z);
				mCorners[3].x = OF(mMaximum,x); mCorners[3].y = OF(mMinimum,y); mCorners[3].z = OF(mMinimum,z);            

				mCorners[4] = mMaximum;
				mCorners[5].x = OF(mMinimum,x); mCorners[5].y = OF(mMaximum,y); mCorners[5].z = OF(mMaximum,z);
				mCorners[6].x = OF(mMinimum,x); mCorners[6].y = OF(mMinimum,y); mCorners[6].z = OF(mMaximum,z);
				mCorners[7].x = OF(mMaximum,x); mCorners[7].y = OF(mMinimum,y); mCorners[7].z = OF(mMaximum,z);

				return mCorners;
			}
#endif

			/** gets the position of one of the corners
			*/
			INSTANCE(Vector3) getCorner(CornerEnum cornerToGet) CONSTF
			{
				switch(cornerToGet)
				{
				case ENUM_OF(CornerEnum,FAR_LEFT_BOTTOM):
					return dnonlynew Vector3(mMinimum);
				case ENUM_OF(CornerEnum,FAR_LEFT_TOP):
					return dnonlynew Vector3(OF(mMinimum,x), OF(mMaximum,y), OF(mMinimum,z));
				case ENUM_OF(CornerEnum,FAR_RIGHT_TOP):
					return dnonlynew Vector3(OF(mMaximum,x), OF(mMaximum,y), OF(mMinimum,z));
				case ENUM_OF(CornerEnum,FAR_RIGHT_BOTTOM):
					return dnonlynew Vector3(OF(mMaximum,x), OF(mMinimum,y), OF(mMinimum,z));
				case ENUM_OF(CornerEnum,NEAR_RIGHT_BOTTOM):
					return dnonlynew Vector3(OF(mMaximum,x), OF(mMinimum,y), OF(mMaximum,z));
				case ENUM_OF(CornerEnum,NEAR_LEFT_BOTTOM):
					return dnonlynew Vector3(OF(mMinimum,x), OF(mMinimum,y), OF(mMaximum,z));
				case ENUM_OF(CornerEnum,NEAR_LEFT_TOP):
					return dnonlynew Vector3(OF(mMinimum,x), OF(mMaximum,y), OF(mMaximum,z));
				case ENUM_OF(CornerEnum,NEAR_RIGHT_TOP):
					return dnonlynew Vector3(mMaximum);
				default:
					return dnonlynew Vector3();
				}
			}
#ifndef DOTNET
			UMATH_API FRIEND std::ostream& operator<<( std::ostream& o, REF_CONST(AxisAlignedBox) aab )
			{
				switch (OF(aab,mExtent))
				{
				case ENUM_OF(Extent,EXTENT_NULL):
					o << "AxisAlignedBox(null)";
					return o;

				case ENUM_OF(Extent,EXTENT_FINITE):
					o << "AxisAlignedBox(min=" << OF(aab,mMinimum) << ", max=" << OF(aab,mMaximum) << ")";
					return o;

				case ENUM_OF(Extent,EXTENT_INFINITE):
					o << "AxisAlignedBox(infinite)";
					return o;

				default: // shut up compiler
					assert( false && "Never reached" );
					return o;
				}
			}
#endif
			/** Merges the passed in box into the current box. The result is the
			box which encompasses both.
			*/
			void merge( REF_CONST(AxisAlignedBox) rhs )
			{
				// Do nothing if rhs null, or this is infinite
				if ((OF(rhs,mExtent) == ENUM_OF(Extent,EXTENT_NULL)) || (mExtent == ENUM_OF(Extent,EXTENT_INFINITE)))
				{
					return;
				}
				// Otherwise if rhs is infinite, make this infinite, too
				else if (OF(rhs,mExtent) == ENUM_OF(Extent,EXTENT_INFINITE))
				{
					mExtent = ENUM_OF(Extent,EXTENT_INFINITE);
				}
				// Otherwise if current null, just take rhs
				else if (mExtent == ENUM_OF(Extent,EXTENT_NULL))
				{
					setExtents(OF(rhs,mMinimum), OF(rhs,mMaximum));
				}
				// Otherwise merge
				else
				{
					INSTANCE(Vector3) min = mMinimum;
					INSTANCE(Vector3) max = mMaximum;
					OF(max,makeCeil(OF(rhs,mMaximum)));
					OF(min,makeFloor(OF(rhs,mMinimum)));

					setExtents(min, max);
				}

			}

			/** Extends the box to encompass the specified point (if needed).
			*/
			inline void merge( REF_CONST(Vector3) point )
			{
				switch (mExtent)
				{
				case ENUM_OF(Extent,EXTENT_NULL): // if null, use this point
					setExtents(point, point);
					return;

				case ENUM_OF(Extent,EXTENT_FINITE):
					OF(mMaximum,makeCeil(point));
					OF(mMinimum,makeFloor(point));
					return;

				case ENUM_OF(Extent,EXTENT_INFINITE): // if infinite, makes no difference
					return;
				}

				assert( false && "Never reached" );
			}

			/** Transforms the box according to the matrix supplied.
			@remarks
			By calling this method you get the axis-aligned box which
			surrounds the transformed version of this box. Therefore each
			corner of the box is transformed by the matrix, then the
			extents are mapped back onto the axes to produce another
			AABB. Useful when you have a local AABB for an object which
			is then transformed.
			*/
			inline void transform( REF_CONST(Matrix4) matrix )
			{
				// Do nothing if current null or infinite
				if( mExtent != ENUM_OF(Extent,EXTENT_FINITE) )
					return;

				INSTANCE(Vector3) oldMin = dnonlynew Vector3();
				INSTANCE(Vector3) oldMax = dnonlynew Vector3();
				INSTANCE(Vector3) currentCorner = dnonlynew Vector3();

				// Getting the old values so that we can use the existing merge method.
				oldMin = mMinimum;
				oldMax = mMaximum;

				// reset
				setNull();

				// We sequentially compute the corners in the following order :
				// 0, 6, 5, 1, 2, 4 ,7 , 3
				// This sequence allows us to only change one member at a time to get at all corners.

				// For each one, we transform it using the matrix
				// Which gives the resulting point and merge the resulting point.

				// First corner 
				// min min min
				currentCorner = oldMin;
				merge( matrix * currentCorner );

				// min,min,max
				OF(currentCorner,z) = OF(oldMax,z);
				merge( matrix * currentCorner );

				// min max max
				OF(currentCorner,y) = OF(oldMax,y);
				merge( matrix * currentCorner );

				// min max min
				OF(currentCorner,z) = OF(oldMin,z);
				merge( matrix * currentCorner );

				// max max min
				OF(currentCorner,x) = OF(oldMax,x);
				merge( matrix * currentCorner );

				// max max max
				OF(currentCorner,z) = OF(oldMax,z);
				merge( matrix * currentCorner );

				// max min max
				OF(currentCorner,y) = OF(oldMin,y);
				merge( matrix * currentCorner );

				// max min min
				OF(currentCorner,z) = OF(oldMin,z);
				merge( matrix * currentCorner ); 
			}

			/** Transforms the box according to the affine matrix supplied.
			@remarks
			By calling this method you get the axis-aligned box which
			surrounds the transformed version of this box. Therefore each
			corner of the box is transformed by the matrix, then the
			extents are mapped back onto the axes to produce another
			AABB. Useful when you have a local AABB for an object which
			is then transformed.
			@note
			The matrix must be an affine matrix. @see Matrix4::isAffine.
			*/
			void transformAffine(REF_CONST(Matrix4) m)
			{
				assert(OF(m,isAffine()));

				// Do nothing if current null or infinite
				if ( mExtent != ENUM_OF(Extent,EXTENT_FINITE))
					return;

				INSTANCE(Vector3) centre = getCenter();
				INSTANCE(Vector3) halfSize = getHalfSize();

				INSTANCE(Vector3) newCentre = OF( m,transformAffine(centre));
				INSTANCE(Vector3) newHalfSize = dnonlynew Vector3(
					UMath::Abs(IDX(m,0,0)) * OF(halfSize,x) + UMath::Abs(IDX(m,0,1)) * OF(halfSize,y) + UMath::Abs(IDX(m,0,2)) * OF(halfSize,z), 
					UMath::Abs(IDX(m,1,0)) * OF(halfSize,x) + UMath::Abs(IDX(m,1,1)) * OF(halfSize,y) + UMath::Abs(IDX(m,1,2)) * OF(halfSize,z),
					UMath::Abs(IDX(m,2,0)) * OF(halfSize,x) + UMath::Abs(IDX(m,2,1)) * OF(halfSize,y) + UMath::Abs(IDX(m,2,2)) * OF(halfSize,z));

				setExtents(newCentre - newHalfSize, newCentre + newHalfSize);
			}

			/** Sets the box to a 'null' value i.e. not a box.
			*/
			inline void setNull()
			{
				mExtent = ENUM_OF(Extent,EXTENT_NULL);
			}

			/** Returns true if the box is null i.e. empty.
			*/
			inline bool isNull(void) CONSTF
			{
				return (mExtent == ENUM_OF(Extent,EXTENT_NULL));
			}

			/** Returns true if the box is finite.
			*/
			bool isFinite(void) CONSTF
			{
				return (mExtent == ENUM_OF(Extent,EXTENT_FINITE));
			}

			/** Sets the box to 'infinite'
			*/
			inline void setInfinite()
			{
				mExtent = ENUM_OF(Extent,EXTENT_INFINITE);
			}

			/** Returns true if the box is infinite.
			*/
			bool isInfinite(void) CONSTF
			{
				return (mExtent == ENUM_OF(Extent,EXTENT_INFINITE));
			}

			/** Returns whether or not this box intersects another. */
			inline bool intersects(REF_CONST(AxisAlignedBox) b2) CONSTF
			{
				// Early-fail for nulls
				if (isNull() || OF(b2,isNull()))
					return false;

				// Early-success for infinites
				if (isInfinite() || OF(b2,isInfinite()))
					return true;

				// Use up to 6 separating planes
				if (OF(mMaximum,x) < OF(b2,OF(mMinimum,x)))
					return false;
				if (OF(mMaximum,y) < OF(b2,OF(mMinimum,y)))
					return false;
				if (OF(mMaximum,z) < OF(b2,OF(mMinimum,z)))
					return false;

				if (OF(mMinimum,x) > OF(b2,OF(mMaximum,x)))
					return false;
				if (OF(mMinimum,y) > OF(b2,OF(mMaximum,y)))
					return false;
				if (OF(mMinimum,z) > OF(b2,OF(mMaximum,z)))
					return false;

				// otherwise, must be intersecting
				return true;

			}

			/// Calculate the area of intersection of this box and another
			inline INSTANCE(AxisAlignedBox) intersection(REF_CONST(AxisAlignedBox) b2) CONSTF
			{
				if (isNull() || OF(b2,isNull()))
				{
					return dnonlynew AxisAlignedBox();
				}
				else if (this->isInfinite())
				{
					return b2;
				}
				else if (OF(b2,isInfinite()))
				{
					return THIS_OBJ;
				}

				INSTANCE(Vector3) intMin = mMinimum;
				INSTANCE(Vector3) intMax = mMaximum;

				OF(intMin,makeCeil(OF(b2,getMinimum())));
				OF(intMax,makeFloor(OF(b2,getMaximum())));

				// Check intersection isn't null
				if (OF(intMin,x) < OF(intMax,x) &&
					OF(intMin,y) < OF(intMax,y) &&
					OF(intMin,z) < OF(intMax,z))
				{
					return dnonlynew AxisAlignedBox(intMin, intMax);
				}

				return dnonlynew AxisAlignedBox();
			}

			/// Calculate the volume of this box
			Real volume(void) CONSTF
			{
				switch (mExtent)
				{
				case ENUM_OF(Extent,EXTENT_NULL):
					return 0.0f;

				case ENUM_OF(Extent,EXTENT_FINITE):
					{
						INSTANCE(Vector3) diff = mMaximum - mMinimum;
						return (OF(diff,x) * OF(diff,y) * OF(diff,z));
					}

				case ENUM_OF(Extent,EXTENT_INFINITE):
					return UMath::POS_INFINITY;

				default: // shut up compiler
					assert( false && "Never reached" );
					return 0.0f;
				}
			}

			/** Scales the AABB by the vector given. */
			inline void scale(REF_CONST(Vector3) s)
			{
				// Do nothing if current null or infinite
				if (mExtent != ENUM_OF(Extent,EXTENT_FINITE))
					return;

				// NB assumes centered on origin
				INSTANCE(Vector3) min = mMinimum * s;
				INSTANCE(Vector3) max = mMaximum * s;
				setExtents(min, max);
			}

			/** Tests whether this box intersects a sphere. */
			bool intersects(REF_CONST(Sphere) s) CONSTF
			{
				return UMath::intersects(s, THIS_OBJ); 
			}
			/** Tests whether this box intersects a plane. */
			bool intersects(REF_CONST(Plane) p) CONSTF
			{
				return UMath::intersects(p, THIS_OBJ);
			}
			/** Tests whether the vector point is within this box. */
			bool intersects(REF_CONST(Vector3) v) CONSTF
			{
				switch (mExtent)
				{
				case ENUM_OF(Extent,EXTENT_NULL):
					return false;

				case ENUM_OF(Extent,EXTENT_FINITE):
					return(OF(v,x) >= OF(mMinimum,x)  &&  OF(v,x) <= OF(mMaximum,x)  && 
						OF(v,y) >= OF(mMinimum,y)  &&  OF(v,y) <= OF(mMaximum,y)  && 
						OF(v,z) >= OF(mMinimum,z)  &&  OF(v,z) <= OF(mMaximum,z));

				case ENUM_OF(Extent,EXTENT_INFINITE):
					return true;

				default: // shut up compiler
					assert( false && "Never reached" );
					return false;
				}
			}
			/// Gets the centre of the box
			INSTANCE(Vector3) getCenter(void) CONSTF
			{
				assert( (mExtent == ENUM_OF(Extent,EXTENT_FINITE)) && "Can't get center of a null or infinite AAB" );

				return dnonlynew Vector3(
					(OF(mMaximum,x) + OF(mMinimum,x)) * 0.5f,
					(OF(mMaximum,y) + OF(mMinimum,y)) * 0.5f,
					(OF(mMaximum,z) + OF(mMinimum,z)) * 0.5f);
			}
			/// Gets the size of the box
			INSTANCE(Vector3) getSize(void) CONSTF
			{
				switch (mExtent)
				{
				case ENUM_OF(Extent,EXTENT_NULL):
					return dnonlynew Vector3(Vector3::ZERO);

				case ENUM_OF(Extent,EXTENT_FINITE):
					return mMaximum - mMinimum;

				case ENUM_OF(Extent,EXTENT_INFINITE):
					return dnonlynew Vector3(
						UMath::POS_INFINITY,
						UMath::POS_INFINITY,
						UMath::POS_INFINITY);

				default: // shut up compiler
					assert( false && "Never reached" );
					return dnonlynew Vector3(Vector3::ZERO);
				}
			}
			/// Gets the half-size of the box
			INSTANCE(Vector3) getHalfSize(void) CONSTF
			{
				switch (mExtent)
				{
				case ENUM_OF(Extent,EXTENT_NULL):
					return dnonlynew Vector3(Vector3::ZERO);

				case ENUM_OF(Extent,EXTENT_FINITE):
					return ((mMaximum - mMinimum) * 0.5);

				case ENUM_OF(Extent,EXTENT_INFINITE):
					return dnonlynew Vector3(
						UMath::POS_INFINITY,
						UMath::POS_INFINITY,
						UMath::POS_INFINITY);

				default: // shut up compiler
					assert( false && "Never reached" );
					return dnonlynew Vector3(Vector3::ZERO);
				}
			}

			/** Tests whether the given point contained by this box.
			*/
			bool contains(REF_CONST(Vector3) v) CONSTF
			{
				if (isNull())
					return false;
				if (isInfinite())
					return true;

				return OF(mMinimum,x) <= OF(v,x) && OF(v,x) <= OF(mMaximum,x) &&
					   OF(mMinimum,y) <= OF(v,y) && OF(v,y) <= OF(mMaximum,y) &&
					   OF(mMinimum,z) <= OF(v,z) && OF(v,z) <= OF(mMaximum,z);
			}
		
			/** Returns the minimum distance between a given point and any part of the box. */
			Real distance(REF_CONST(Vector3) v) CONSTF
			{
			
				if (this->contains(v))
					return 0;
				else
				{
					Real maxDist = std::numeric_limits<Real>::min();

					if (OF(v,x) < OF(mMinimum,x))
						maxDist = std::max<Real>(maxDist, OF(mMinimum,x) - OF(v,x));
					if (OF(v,y) < OF(mMinimum,y))
						maxDist = std::max<Real>(maxDist, OF(mMinimum,y) - OF(v,y));
					if (OF(v,z) < OF(mMinimum,z))
						maxDist = std::max<Real>(maxDist, OF(mMinimum,z) - OF(v,z));
				
					if (OF(v,x) > OF(mMaximum,x))
						maxDist = std::max<Real>(maxDist, OF(v,x) - OF(mMaximum,x));
					if (OF(v,y) > OF(mMaximum,y))
						maxDist = std::max<Real>(maxDist, OF(v,y) - OF(mMaximum,y));
					if (OF(v,z) > OF(mMaximum,z))
						maxDist = std::max<Real>(maxDist, OF(v,z) - OF(mMaximum,z));
				
					return maxDist;
				}
			}

			/** Tests whether another box contained by this box.
			*/
			bool contains(REF_CONST(AxisAlignedBox) other) CONSTF
			{
				if (OF(other,isNull()) || this->isInfinite())
					return true;

				if (this->isNull() || OF(other,isInfinite()))
					return false;

				return OF(mMinimum,x) <= OF(OF(other,mMinimum),x) &&
					   OF(mMinimum,y) <= OF(OF(other,mMinimum),y) &&
					   OF(mMinimum,z) <= OF(OF(other,mMinimum),z) &&
					   OF(OF(other,mMaximum),x) <= OF(mMaximum,x) &&
					   OF(OF(other,mMaximum),y) <= OF(mMaximum,y) &&
					   OF(OF(other,mMaximum),z) <= OF(mMaximum,z);
			}

			/** Tests 2 boxes for equality.
			*/
			bool operator== (REF_CONST(AxisAlignedBox) rhs) CONSTF
			{
				if (mExtent != OF(rhs,mExtent))
					return false;

				if (!isFinite())
					return true;

				return ( mMinimum == OF(rhs,mMinimum) && mMaximum == OF(rhs,mMaximum));
			}

			/** Tests 2 boxes for inequality.
			*/
			bool operator!= (REF_CONST(AxisAlignedBox) rhs) CONSTF
			{
				return !(THIS_OBJ == rhs);
			}

			// special values

			static CONST AxisAlignedBox BOX_NULL              SC_VALUE(dnonlynew AxisAlignedBox());
			static CONST AxisAlignedBox BOX_INFINITE          SC_VALUE(dnonlynew AxisAlignedBox(AxisAlignedBox::Extent::EXTENT_INFINITE));


		}; // class AxisAlignedBox

	} // namespace Math
} // namespace UnE

#endif
