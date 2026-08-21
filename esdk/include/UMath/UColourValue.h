#ifndef _UMATH_COLOURVALUE_H__
#define _UMATH_COLOURVALUE_H__


#include "UMathAPI.h"

namespace UnE
{
	namespace Math
	{
	/** \addtogroup Core
	*  @{
	*/
	/** \addtogroup General
	*  @{
	*/

	typedef uint32 RGBA;
	typedef uint32 ARGB;
	typedef uint32 ABGR;
	typedef uint32 BGRA;

	/** Class representing colour.
		@remarks
			Colour is represented as 4 components, each of which is a
			floating-point value from 0.0 to 1.0.
		@par
			The 3 'normal' colour components are red, green and blue, a higher
			number indicating greater amounts of that component in the colour.
			The forth component is the 'alpha' value, which represents
			transparency. In this case, 0.0 is completely transparent and 1.0 is
			fully opaque.
	*/
	UMATH_DECLARE_EXPORT_CLASS(ColourValue)
	{
	public:

		static CONST ColourValue ZERO           SC_VALUE(dnonlynew ColourValue(0.0,0.0,0.0,0.0));
		static CONST ColourValue Black			SC_VALUE(dnonlynew ColourValue(0.0,0.0,0.0));
		static CONST ColourValue White			SC_VALUE(dnonlynew ColourValue(1.0,1.0,1.0));
		static CONST ColourValue Red			SC_VALUE(dnonlynew ColourValue(1.0,0.0,0.0));
		static CONST ColourValue Green			SC_VALUE(dnonlynew ColourValue(0.0,1.0,0.0));
		static CONST ColourValue Blue			SC_VALUE(dnonlynew ColourValue(0.0,0.0,1.0));

		explicit ColourValue( )
		{
			r = 1.0f; g = 1.0f; b = 1.0f; a = 1.0f;
		}
		explicit ColourValue( float red)
		{
			r = red; g = 1.0f; b = 1.0f; a = 1.0f;
		}
		explicit ColourValue( float red, float green)
		{
			r = red; g = green; b = 1.0f; a = 1.0f;
		}
		explicit ColourValue( float red, float green, float blue)
		{
			r = red; g = green; b = blue; a = 1.0f;
		}
		explicit ColourValue( float red, float green, float blue, float alpha)
		{
			r = red; g = green; b = blue; a = alpha;
		}

		ColourValue( REF_CONST(ColourValue) rhs)
		{
			r = OF(rhs, r);
			g = OF(rhs, g);
			b = OF(rhs, b);
			a = OF(rhs, a);
		}
		float r,g,b,a;

		/** Retrieves colour as RGBA.
		*/
		RGBA getAsRGBA(void) CONSTF;

		/** Retrieves colour as ARGB.
		*/
		ARGB getAsARGB(void) CONSTF;

		/** Retrieves colour as BGRA.
		*/
		BGRA getAsBGRA(void) CONSTF;

		/** Retrieves colours as ABGR */
		ABGR getAsABGR(void) CONSTF;

		/** Sets colour as RGBA.
		*/
		void setAsRGBA(const RGBA val);

		/** Sets colour as ARGB.
		*/
		void setAsARGB(const ARGB val);

		/** Sets colour as BGRA.
		*/
		void setAsBGRA(const BGRA val);

		/** Sets colour as ABGR.
		*/
		void setAsABGR(const ABGR val);

		/** Clamps colour value to the range [0, 1].
		*/
		void saturate(void)
		{
			if (r < 0)
				r = 0;
			else if (r > 1)
				r = 1;

			if (g < 0)
				g = 0;
			else if (g > 1)
				g = 1;

			if (b < 0)
				b = 0;
			else if (b > 1)
				b = 1;

			if (a < 0)
				a = 0;
			else if (a > 1)
				a = 1;
		}

		/** As saturate, except that this colour value is unaffected and
			the saturated colour value is returned as a copy. */
		INSTANCE(ColourValue) saturateCopy(void) CONSTF
		{
			INSTANCE(ColourValue) ret = dnonlynew ColourValue(THIS_OBJ);
			OF(ret,saturate());
			return ret;
		}


#ifdef DOTNET

		//	float r,g,b,a;
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
						return r;
					case 1:
						return g;
					case 2:
						return b;
					case 3:
						return a;

					}
				}
				return 0;
			}
			void set(int idx, Real value) {
				if (!(idx < 0 || idx >= 4))
				{
					switch(idx)
					{
					case 0:
						r = value;
						break;
					case 1:
						g = value;
						break;							
					case 2:
						b = value;
					case 3:
						a = value;
						break;					
					}
				}
			}
		}

#else
		/// Array accessor operator
		inline float operator [] ( const size_t i ) const
		{
			assert( i < 4 );

			return *(&r+i);
		}

		/// Array accessor operator
		inline float& operator [] ( const size_t i )
		{
			assert( i < 4 );

			return *(&r+i);
		}

		/// Pointer accessor for direct copying
		float* ptr()
		{
			return &r;
		}
		/// Pointer accessor for direct copying
		const float* ptr() CONSTF
		{
			return &r;
		}
#endif
		


#ifdef DOTNET 

		static bool operator==(REF_CONST(ColourValue)  lhs, REF_CONST(ColourValue)  rhs) CONSTF;
		static bool operator!=(REF_CONST(ColourValue)  lhs, REF_CONST(ColourValue)  rhs) CONSTF;

		// arithmetic operations
		static INSTANCE(ColourValue) operator + ( ColourValue^ rkVector1, ColourValue^  rkVector2 ) CONSTF;
		static INSTANCE(ColourValue) operator - ( REF(ColourValue) rkVector1, REF(ColourValue)  rkVector2 ) CONSTF;
		//static INSTANCE(ColourValue) operator * ( REF(ColourValue) rkVector1, const float fScalar ) CONSTF;
		static INSTANCE(ColourValue) operator * ( REF(ColourValue) rkVector1, REF(ColourValue)  rkVector2 ) CONSTF;
		static INSTANCE(ColourValue) operator / ( REF(ColourValue) rkVector1, REF(ColourValue)  rkVector2 ) CONSTF;
		static INSTANCE(ColourValue) operator / ( REF(ColourValue) rkVector1, const float fScalar ) CONSTF;		
		// arithmetic updates
		static REF(ColourValue) operator += ( REF(ColourValue) rkVector1, REF(ColourValue)  rkVector2 );
		static REF(ColourValue) operator -= ( REF(ColourValue) rkVector1, REF(ColourValue)  rkVector2 );
		static REF(ColourValue) operator *= ( REF(ColourValue) rkVector1, const float fScalar );
		static REF(ColourValue) operator /= ( REF(ColourValue) rkVector1, const float fScalar );
#else

		bool operator==(REF_CONST(ColourValue)  rhs) CONSTF;
		bool operator!=(REF_CONST(ColourValue)  rhs) CONSTF;
		// arithmetic operations
		INSTANCE(ColourValue) operator + ( REF(ColourValue)  rkVector ) CONSTF;
		INSTANCE(ColourValue) operator - ( REF(ColourValue)  rkVector ) CONSTF;
		INSTANCE(ColourValue) operator * (const float fScalar ) CONSTF;
		INSTANCE(ColourValue) operator * ( REF(ColourValue)  rhs) CONSTF;
		INSTANCE(ColourValue) operator / ( REF(ColourValue)  rhs) CONSTF;
		INSTANCE(ColourValue) operator / (const float fScalar ) CONSTF;		
		// arithmetic updates
		REF(ColourValue) operator += ( REF(ColourValue)  rkVector );
		REF(ColourValue) operator -= ( REF(ColourValue)  rkVector );
		REF(ColourValue) operator *= (const float fScalar );
		REF(ColourValue) operator /= (const float fScalar );

		inline UMATH_API FRIEND std::ostream& operator << ( std::ostream& o, REF(ColourValue)  c )
		{
			o << "ColourValue(" << OF(c,r) << ", " << OF(c,g) << ", " << OF(c,b) << ", " << OF(c,a) << ")";
			return o;
		}
#endif

		inline FRIEND INSTANCE(ColourValue) operator * (const float fScalar, REF(ColourValue)  rkVector )
		{
			INSTANCE(ColourValue) kProd = dnonlynew ColourValue();

			OF(kProd,r) = fScalar * OF(rkVector,r);
			OF(kProd,g) = fScalar * OF(rkVector,g);
			OF(kProd,b) = fScalar * OF(rkVector,b);
			OF(kProd,a) = fScalar * OF(rkVector,a);

			return kProd;
		}
		/** Set a colour value from Hue, Saturation and Brightness.
		@param hue Hue value, scaled to the [0,1] range as opposed to the 0-360
		@param saturation Saturation level, [0,1]
		@param brightness Brightness level, [0,1]
		*/
		void setHSB(Real hue, Real saturation, Real brightness);

		/** Convert the current colour to Hue, Saturation and Brightness values. 
		@param hue Output hue value, scaled to the [0,1] range as opposed to the 0-360
		@param saturation Output saturation level, [0,1]
		@param brightness Output brightness level, [0,1]
		*/
		//void getHSB(CBR(Real) hue, CBR(Real) saturation, CBR(Real) brightness) CONSTF;
		void getHSB(CBR(Real) hue, CBR(Real) saturation, CBR(Real) brightness) CONSTF;

	};
	/** @} */
	/** @} */
	}
} // namespace

#endif
