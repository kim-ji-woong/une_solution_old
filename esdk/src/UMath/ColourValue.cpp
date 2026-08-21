
#include "stdafx.h"
#include "UColourValue.h"
#include "UMath.h"



namespace UnE
{
	namespace Math
	{
#ifndef DOTNET
	const ColourValue ColourValue::ZERO = ColourValue(0.0,0.0,0.0,0.0);
	const ColourValue ColourValue::Black = ColourValue(0.0,0.0,0.0);
	const ColourValue ColourValue::White = ColourValue(1.0,1.0,1.0);
	const ColourValue ColourValue::Red = ColourValue(1.0,0.0,0.0);
	const ColourValue ColourValue::Green = ColourValue(0.0,1.0,0.0);
	const ColourValue ColourValue::Blue = ColourValue(0.0,0.0,1.0);
#endif

	//---------------------------------------------------------------------
#if UNE_ENDIAN == UNE_ENDIAN_BIG
	ABGR ColourValue::getAsABGR(void) CONST
#else
	RGBA ColourValue::getAsRGBA(void) CONST
#endif
	{
		uint8 val8;
		uint32 val32 = 0;

		// Convert to 32bit pattern
		// (RGBA = 8888)

		// Red
		val8 = static_cast<uint8>(r * 255);
		val32 = val8 << 24;

		// Green
		val8 = static_cast<uint8>(g * 255);
		val32 += val8 << 16;

		// Blue
		val8 = static_cast<uint8>(b * 255);
		val32 += val8 << 8;

		// Alpha
		val8 = static_cast<uint8>(a * 255);
		val32 += val8;

		return val32;
	}
	//---------------------------------------------------------------------
#if UNE_ENDIAN == UNE_ENDIAN_BIG
	BGRA ColourValue::getAsBGRA(void) CONST
#else
	ARGB ColourValue::getAsARGB(void) CONST
#endif
	{
		uint8 val8;
		uint32 val32 = 0;

		// Convert to 32bit pattern
		// (ARGB = 8888)

		// Alpha
		val8 = static_cast<uint8>(a * 255);
		val32 = val8 << 24;

		// Red
		val8 = static_cast<uint8>(r * 255);
		val32 += val8 << 16;

		// Green
		val8 = static_cast<uint8>(g * 255);
		val32 += val8 << 8;

		// Blue
		val8 = static_cast<uint8>(b * 255);
		val32 += val8;


		return val32;
	}
	//---------------------------------------------------------------------
#if UNE_ENDIAN == UNE_ENDIAN_BIG
	ARGB ColourValue::getAsARGB(void) CONST
#else
	BGRA ColourValue::getAsBGRA(void) CONST
#endif
	{
		uint8 val8;
		uint32 val32 = 0;

		// Convert to 32bit pattern
		// (ARGB = 8888)

		// Blue
		val8 = static_cast<uint8>(b * 255);
		val32 = val8 << 24;

		// Green
		val8 = static_cast<uint8>(g * 255);
		val32 += val8 << 16;

		// Red
		val8 = static_cast<uint8>(r * 255);
		val32 += val8 << 8;

		// Alpha
		val8 = static_cast<uint8>(a * 255);
		val32 += val8;


		return val32;
	}
	//---------------------------------------------------------------------
#if UNE_ENDIAN == UNE_ENDIAN_BIG
	RGBA ColourValue::getAsRGBA(void) CONST
#else
	ABGR ColourValue::getAsABGR(void) CONST
#endif
	{
		uint8 val8;
		uint32 val32 = 0;

		// Convert to 32bit pattern
		// (ABRG = 8888)

		// Alpha
		val8 = static_cast<uint8>(a * 255);
		val32 = val8 << 24;

		// Blue
		val8 = static_cast<uint8>(b * 255);
		val32 += val8 << 16;

		// Green
		val8 = static_cast<uint8>(g * 255);
		val32 += val8 << 8;

		// Red
		val8 = static_cast<uint8>(r * 255);
		val32 += val8;


		return val32;
	}
	//---------------------------------------------------------------------
#if UNE_ENDIAN == UNE_ENDIAN_BIG
	void ColourValue::setAsABGR(const ABGR val)
#else
	void ColourValue::setAsRGBA(const RGBA val)
#endif
	{
		uint32 val32 = val;

		// Convert from 32bit pattern
		// (RGBA = 8888)

		// Red
		r = ((val32 >> 24) & 0xFF) / 255.0f;

		// Green
		g = ((val32 >> 16) & 0xFF) / 255.0f;

		// Blue
		b = ((val32 >> 8) & 0xFF) / 255.0f;

		// Alpha
		a = (val32 & 0xFF) / 255.0f;
	}
	//---------------------------------------------------------------------
#if UNE_ENDIAN == UNE_ENDIAN_BIG
	void ColourValue::setAsBGRA(const BGRA val)
#else
	void ColourValue::setAsARGB(const ARGB val)
#endif
	{
		uint32 val32 = val;

		// Convert from 32bit pattern
		// (ARGB = 8888)

		// Alpha
		a = ((val32 >> 24) & 0xFF) / 255.0f;

		// Red
		r = ((val32 >> 16) & 0xFF) / 255.0f;

		// Green
		g = ((val32 >> 8) & 0xFF) / 255.0f;

		// Blue
		b = (val32 & 0xFF) / 255.0f;
	}
	//---------------------------------------------------------------------
#if UNE_ENDIAN == UNE_ENDIAN_BIG
	void ColourValue::setAsARGB(const ARGB val)
#else
	void ColourValue::setAsBGRA(const BGRA val)
#endif
	{
		uint32 val32 = val;

		// Convert from 32bit pattern
		// (ARGB = 8888)

		// Blue
		b = ((val32 >> 24) & 0xFF) / 255.0f;

		// Green
		g = ((val32 >> 16) & 0xFF) / 255.0f;

		// Red
		r = ((val32 >> 8) & 0xFF) / 255.0f;

		// Alpha
		a = (val32 & 0xFF) / 255.0f;
	}
	//---------------------------------------------------------------------
#if UNE_ENDIAN == UNE_ENDIAN_BIG
	void ColourValue::setAsRGBA(const RGBA val)
#else
	void ColourValue::setAsABGR(const ABGR val)
#endif
	{
		uint32 val32 = val;

		// Convert from 32bit pattern
		// (ABGR = 8888)

		// Alpha
		a = ((val32 >> 24) & 0xFF) / 255.0f;

		// Blue
		b = ((val32 >> 16) & 0xFF) / 255.0f;

		// Green
		g = ((val32 >> 8) & 0xFF) / 255.0f;

		// Red
		r = (val32 & 0xFF) / 255.0f;
	}
	
	//---------------------------------------------------------------------
	void ColourValue::setHSB(Real hue, Real saturation, Real brightness)
	{
		// wrap hue
		if (hue > 1.0f)
		{
			hue -= (int)hue;
		}
		else if (hue < 0.0f)
		{
			hue += (int)hue + 1;
		}
		// clamp saturation / brightness
		saturation = std::min(saturation, (Real)1.0);
		saturation = std::max(saturation, (Real)0.0);
		brightness = std::min(brightness, (Real)1.0);
		brightness = std::max(brightness, (Real)0.0);

		if (brightness == 0.0f)
		{   
			// early exit, this has to be black
			r = g = b = 0.0f;
			return;
		}

		if (saturation == 0.0f)
		{   
			// early exit, this has to be grey

			r = g = b = brightness;
			return;
		}


		Real hueDomain  = hue * 6.0f;
		if (hueDomain >= 6.0f)
		{
			// wrap around, and allow mathematical errors
			hueDomain = 0.0f;
		}
		unsigned short domain = (unsigned short)hueDomain;
		Real f1 = brightness * (1 - saturation);
		Real f2 = brightness * (1 - saturation * (hueDomain - domain));
		Real f3 = brightness * (1 - saturation * (1 - (hueDomain - domain)));

		switch (domain)
		{
		case 0:
			// red domain; green ascends
			r = brightness;
			g = f3;
			b = f1;
			break;
		case 1:
			// yellow domain; red descends
			r = f2;
			g = brightness;
			b = f1;
			break;
		case 2:
			// green domain; blue ascends
			r = f1;
			g = brightness;
			b = f3;
			break;
		case 3:
			// cyan domain; green descends
			r = f1;
			g = f2;
			b = brightness;
			break;
		case 4:
			// blue domain; red ascends
			r = f3;
			g = f1;
			b = brightness;
			break;
		case 5:
			// magenta domain; blue descends
			r = brightness;
			g = f1;
			b = f2;
			break;
		}
	}
	//---------------------------------------------------------------------
	void ColourValue::getHSB(CBR(Real) hue, CBR(Real) saturation, CBR(Real) brightness) CONSTF
	{
		Real rr = r;
		Real gg = g;
		Real bb = b;
		Real vMin = std::min(rr, std::min(gg, bb));
		Real vMax = std::max(rr, std::max(gg, bb));
		Real delta = vMax - vMin;

		brightness = vMax;

		if (UMath::RealEqual(delta, 0.0f, Real(1e-6)))
		{
			// grey
			hue = 0;
			saturation = 0;
		}
		else                                    
		{
			// a colour
			saturation = delta / vMax;

			Real deltaR = (((vMax - r) / 6.0f) + (delta / 2.0f)) / delta;
			Real deltaG = (((vMax - g) / 6.0f) + (delta / 2.0f)) / delta;
			Real deltaB = (((vMax - b) / 6.0f) + (delta / 2.0f)) / delta;

			if (UMath::RealEqual(r, vMax))
				hue = deltaB - deltaG;
			else if (UMath::RealEqual(g, vMax))
				hue = 0.3333333f + deltaR - deltaB;
			else if (UMath::RealEqual(b, vMax)) 
				hue = 0.6666667f + deltaG - deltaR;

			if (hue < 0.0f) 
				hue += 1.0f;
			if (hue > 1.0f)
				hue -= 1.0f;
		}

		
	}

	



#ifdef DOTNET 
	//---------------------------------------------------------------------
	bool ColourValue::operator==(REF(ColourValue) lhs, REF_CONST(ColourValue) rhs) CONST
	{
		return (OF(lhs,r) == OF(rhs,r) &&
			OF(lhs,g) == OF(rhs,g) &&
			OF(lhs,b) == OF(rhs,b) &&
			OF(lhs,a) == OF(rhs,a));
	}
	//---------------------------------------------------------------------
	bool ColourValue::operator!=(REF(ColourValue) lhs, REF_CONST(ColourValue) rhs) CONST
	{
		return !(lhs == rhs);
	}
	INSTANCE(ColourValue) ColourValue::operator + ( REF(ColourValue) rkVector1, REF(ColourValue)  rkVector2 ) CONSTF
	{
		INSTANCE(ColourValue) kSum = dnonlynew ColourValue();

		OF(kSum,r) = OF(rkVector1,r) + OF(rkVector2,r);
		OF(kSum,g) = OF(rkVector1,g) + OF(rkVector2,g);
		OF(kSum,b) = OF(rkVector1,b) + OF(rkVector2,b);
		OF(kSum,a) = OF(rkVector1,a) + OF(rkVector2,a);

		return kSum;
	}
	
	INSTANCE(ColourValue) ColourValue::operator - ( REF(ColourValue) rkVector1, REF(ColourValue)  rkVector2 ) CONSTF
	{
		INSTANCE(ColourValue) kDiff = dnonlynew ColourValue();

		OF(kDiff,r) = OF(rkVector1,r) - OF(rkVector2,r);
		OF(kDiff,g) = OF(rkVector1,g) - OF(rkVector2,g);
		OF(kDiff,b) = OF(rkVector1,b) - OF(rkVector2,b);
		OF(kDiff,a) = OF(rkVector1,a) - OF(rkVector2,a);

		return kDiff;
	}

	INSTANCE(ColourValue) ColourValue::operator * ( REF(ColourValue) rkVector1, REF(ColourValue)  rkVector2 ) CONSTF
	{
		INSTANCE(ColourValue) kProd = dnonlynew ColourValue();

		OF(kProd,r) = OF(rkVector1,r) * OF(rkVector2,r);
		OF(kProd,g) = OF(rkVector1,g) * OF(rkVector2,g);
		OF(kProd,b) = OF(rkVector1,b) * OF(rkVector2,b);
		OF(kProd,a) = OF(rkVector1,a) * OF(rkVector2,a);

		return kProd;
	}

	INSTANCE(ColourValue) ColourValue::operator / ( REF(ColourValue) rkVector1, REF(ColourValue)  rkVector2 ) CONSTF
	{
		INSTANCE(ColourValue) kProd = dnonlynew ColourValue();

		OF(kProd,r) = OF(rkVector1,r) / OF(rkVector2,r);
		OF(kProd,g) = OF(rkVector1,g) / OF(rkVector2,g);
		OF(kProd,b) = OF(rkVector1,b) / OF(rkVector2,b);
		OF(kProd,a) = OF(rkVector1,a) / OF(rkVector2,a);

		return kProd;
	}

	INSTANCE(ColourValue) ColourValue::operator / ( REF(ColourValue) rkVector1, const float fScalar ) CONSTF
	{
		assert( fScalar != 0.0 );

		INSTANCE(ColourValue) kDiv = dnonlynew ColourValue();

		float fInv = 1.0f / fScalar;
		OF(kDiv,r) = OF(rkVector1,r) * fInv;
		OF(kDiv,g) = OF(rkVector1,g) * fInv;
		OF(kDiv,b) = OF(rkVector1,b) * fInv;
		OF(kDiv,a) = OF(rkVector1,r) * fInv;

		return kDiv;
	}
	
	

	REF(ColourValue) ColourValue::operator += ( REF(ColourValue) rkVector1, REF(ColourValue)  rkVector2 )
	{
		//INSTANCE(ColourValue) kSum = dnonlynew ColourValue();

		OF(rkVector1,r) = OF(rkVector1,r) + OF(rkVector2,r);
		OF(rkVector1,g) = OF(rkVector1,g) + OF(rkVector2,g);
		OF(rkVector1,b) = OF(rkVector1,b) + OF(rkVector2,b);
		OF(rkVector1,a) = OF(rkVector1,a) + OF(rkVector2,a);

		return rkVector1;
	}

	REF(ColourValue) ColourValue::operator -= ( REF(ColourValue) rkVector1, REF(ColourValue)  rkVector2 )
	{
		//INSTANCE(ColourValue) kDiff = dnonlynew ColourValue();

		OF(rkVector1,r) = OF(rkVector1,r) - OF(rkVector2,r);
		OF(rkVector1,g) = OF(rkVector1,g) - OF(rkVector2,g);
		OF(rkVector1,b) = OF(rkVector1,b) - OF(rkVector2,b);
		OF(rkVector1,a) = OF(rkVector1,a) - OF(rkVector2,a);

		return rkVector1;
	}

	REF(ColourValue) ColourValue::operator*=( REF(ColourValue) rkVector1, const float fScalar )
	{
		OF(rkVector1,r) *= fScalar;
		OF(rkVector1,r) *= fScalar;
		OF(rkVector1,r) *= fScalar;
		OF(rkVector1,r) *= fScalar;
		return rkVector1;
	}

	REF(ColourValue) ColourValue::operator/=( REF(ColourValue) rkVector1, const float fScalar )
	{
		assert( fScalar != 0.0 );

		float fInv = 1.0f / fScalar;

		OF(rkVector1,r) *= fInv;
		OF(rkVector1,r) *= fInv;
		OF(rkVector1,r) *= fInv;
		OF(rkVector1,r) *= fInv;
		return rkVector1;
	}
#else

	//---------------------------------------------------------------------
	bool ColourValue::operator==(REF_CONST(ColourValue) rhs) CONST
	{
		return (r == OF(rhs,r) &&
			g == OF(rhs,g) &&
			b == OF(rhs,b) &&
			a == OF(rhs,a));
	}
	//---------------------------------------------------------------------
	bool ColourValue::operator!=(REF_CONST(ColourValue) rhs) CONST
	{
		return !(THIS_OBJ == rhs);
	}
	// arithmetic operations
	INSTANCE(ColourValue) ColourValue::operator + ( REF(ColourValue)  rkVector ) CONSTF
	{
		INSTANCE(ColourValue) kSum = dnonlynew ColourValue();

		OF(kSum,r) = r + OF(rkVector,r);
		OF(kSum,g) = g + OF(rkVector,g);
		OF(kSum,b) = b + OF(rkVector,b);
		OF(kSum,a) = a + OF(rkVector,a);

		return kSum;
	}
	INSTANCE(ColourValue) ColourValue::operator-( REF(ColourValue) rkVector ) CONSTF
	{
		INSTANCE(ColourValue) kDiff = dnonlynew ColourValue();

		OF(kDiff,r) = r - OF(rkVector,r);
		OF(kDiff,g) = g - OF(rkVector,g);
		OF(kDiff,b) = b - OF(rkVector,b);
		OF(kDiff,a) = a - OF(rkVector,a);

		return kDiff;
	}

	INSTANCE(ColourValue) ColourValue::operator*( const float fScalar ) CONSTF
	{
		INSTANCE(ColourValue) kProd = dnonlynew ColourValue();

		OF(kProd,r) = fScalar*r;
		OF(kProd,g) = fScalar*g;
		OF(kProd,b) = fScalar*b;
		OF(kProd,a) = fScalar*a;

		return kProd;
	}

	INSTANCE(ColourValue) ColourValue::operator*(  REF(ColourValue) rhs) CONSTF
	{
		INSTANCE(ColourValue) kProd = dnonlynew ColourValue();

		OF(kProd,r) = OF(rhs,r) * r;
		OF(kProd,g) = OF(rhs,g) * g;
		OF(kProd,b) = OF(rhs,b) * b;
		OF(kProd,a) = OF(rhs,a) * a;

		return kProd;
	}

	INSTANCE(ColourValue) ColourValue::operator/( REF(ColourValue) rhs) CONSTF
	{
		INSTANCE(ColourValue) kProd = dnonlynew ColourValue();

		OF(kProd,r) = r / OF(rhs,r);
		OF(kProd,g) = g / OF(rhs,g);
		OF(kProd,b) = b / OF(rhs,b);
		OF(kProd,a) = a / OF(rhs,a);

		return kProd;
	}

	INSTANCE(ColourValue) ColourValue::operator/(const float fScalar ) CONSTF
	{
		assert( fScalar != 0.0 );

		INSTANCE(ColourValue) kDiv = dnonlynew ColourValue();

		float fInv = 1.0f / fScalar;
		OF(kDiv,r) = r * fInv;
		OF(kDiv,g) = g * fInv;
		OF(kDiv,b) = b * fInv;
		OF(kDiv,a) = a * fInv;

		return kDiv;
	}
	//////////////////////////////////////////////////////////////////////////
	REF(ColourValue) ColourValue::operator += ( REF(ColourValue)  rkVector )
	{
		r += OF(rkVector,r);
		g += OF(rkVector,g);
		b += OF(rkVector,b);
		a += OF(rkVector,a);

		return THIS_OBJ;
	}

	REF(ColourValue) ColourValue::operator-=( REF(ColourValue)  rkVector )
	{
		r -= OF(rkVector,r);
		g -= OF(rkVector,g);
		b -= OF(rkVector,b);
		a -= OF(rkVector,a);

		return THIS_OBJ;
	}

	REF(ColourValue) ColourValue::operator*=( const float fScalar )
	{
		r *= fScalar;
		g *= fScalar;
		b *= fScalar;
		a *= fScalar;
		return THIS_OBJ;
	}

	REF(ColourValue) ColourValue::operator/=(const float fScalar )
	{
		assert( fScalar != 0.0 );

		float fInv = 1.0f / fScalar;

		r *= fInv;
		g *= fInv;
		b *= fInv;
		a *= fInv;

		return THIS_OBJ;
	}
#endif

	


}

}