//=============================================================
//
// C4 Engine version 2.9.1
// Copyright 1999-2012, by Terathon Software LLC
//
// This copy is licensed to the following:
//
//     Registered user: Soo Ki Kim
//     Maximum number of users: 1
//     License #C4T0035002
//
// License is granted under terms of the license agreement
// entered by the registed user.
//
// Unauthorized redistribution of source code is strictly
// prohibited. Violators will be prosecuted.
//
//=============================================================


#include "C4String.h"
#include "C4Math.h"


using namespace C4;


const char Text::hexDigit[16] = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

const char Text::identifierCharFlag[256] =
{
	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
	0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0,
	0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1,
	0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
};


char String<0>::emptyString[1] = "";


int32 Text::ReadGlyphCodeUTF8(const char *text, unsigned_int32 *code)
{
	char c = text[0];
	if (c >= 0)
	{
		*code = c;
		return (1);
	}
	
	unsigned_int32 byte1 = c & 0xFF;
	if (byte1 >= 0xC0)
	{
		if (byte1 < 0xE0)
		{
			unsigned_int32 byte2 = reinterpret_cast<const unsigned_int8 *>(text)[1];
			*code = ((byte1 << 6) & 0x07C0) | (byte2 & 0x003F);
			return (2);
		}
		
		if (byte1 < 0xF0)
		{
			unsigned_int32 byte2 = reinterpret_cast<const unsigned_int8 *>(text)[1];
			unsigned_int32 byte3 = reinterpret_cast<const unsigned_int8 *>(text)[2];
			*code = ((byte1 << 12) & 0xF000) | ((byte2 << 6) & 0x0FC0) | (byte3 & 0x003F);
			return (3);
		}
	}
	
	*code = byte1;
	return (1);
}

int32 Text::WriteGlyphCodeUTF8(char *text, unsigned_int32 code)
{
	if (code <= 0x007F)
	{
		text[0] = (char) code;
		return (1);
	}
	
	if (code <= 0x07FF)
	{
		text[0] = (char) (((code >> 6) & 0x1F) | 0xC0);
		text[1] = (char) ((code & 0x3F) | 0x80);
		return (2);
	}
	
	if (code <= 0xFFFF)
	{
		text[0] = (char) (((code >> 12) & 0x0F) | 0xE0);
		text[1] = (char) (((code >> 6) & 0x3F) | 0x80);
		text[2] = (char) ((code & 0x3F) | 0x80);
		return (3);
	}
	
	return (0);
}

int32 Text::GetGlyphCodeByteCountUTF8(unsigned_int32 code)
{
	if (code <= 0x007F) return (1);
	if (code <= 0x07FF) return (2);
	if (code <= 0xFFFF) return (3);
	return (0);
}

int32 Text::GetGlyphCountUTF8(const char *text)
{
	int32 count = 0;
	for (;; count++)
	{
		unsigned_int32 c = *reinterpret_cast<const unsigned_int8 *>(text);
		if (c == 0) break;
		
		if ((c < 0xC0) || (c >= 0xF0)) text++;
		else if (c < 0xE0) text += 2;
		else text += 3; 
	}
	 
	return (count); 
} 

int32 Text::GetGlyphCountUTF8(const char *text, int32 max) 
{
	int32 count = 0;
	const char *end = text + max;
	for (; text < end; count++) 
	{
		unsigned_int32 c = *reinterpret_cast<const unsigned_int8 *>(text);
		if (c == 0) break;
		 
		if ((c < 0xC0) || (c >= 0xF0)) text++;
		else if (c < 0xE0) text += 2;
		else text += 3;
	}
	
	return (count);
}

int32 Text::GetPreviousGlyphByteCountUTF8(const char *text, int32 max)
{
	int32 count = 0;
	while (--max >= 0)
	{
		count--;
		unsigned_int32 c = reinterpret_cast<const unsigned_int8 *>(text)[count];
		if (c - 0x80 >= 0x40U) break;
	}
	
	return (-count);
}

int32 Text::GetNextGlyphByteCountUTF8(const char *text, int32 max)
{
	char c = text[0];
	if (c < 0)
	{
		unsigned_int32 byte1 = c & 0xFF;
		if (byte1 >= 0xC0)
		{
			if (byte1 < 0xE0) return (Min(max, 2));
			if (byte1 < 0xF0) return (Min(max, 3));
		}
	}
	
	return (Min(max, 1));
}

int32 Text::GetGlyphStringByteCountUTF8(const char *text, int32 glyphCount)
{
	int32 count = 0;
	for (machine a = 0; a < glyphCount; a++)
	{
		char c = text[count];
		if (c == 0) break;
		
		int32 size = 1;
		if (c < 0)
		{
			unsigned_int32 byte1 = c & 0xFF;
			if (byte1 >= 0xC0)
			{
				if (byte1 < 0xE0) size += 1;
				else if (byte1 < 0xF0) size += 2;
			}
		}
		
		count += size;
	}
	
	return (count);
}

int32 Text::GetTextLength(const char *text)
{
	const char *start = text;
	while (*text != 0) text++;
	return (text - start);
}

unsigned_int32 Text::GetTextHash(const char *text)
{
	unsigned_int32 hash = 0;
	for (;;)
	{
		unsigned_int32 c = *reinterpret_cast<const unsigned_int8 *>(text);
		if (c == 0) break;
		
		hash ^= c;
		hash = hash * 0x6B84DF47 + 1;
		
		text++;
	}
	
	return (hash);
}

int32 Text::FindChar(const char *text, unsigned_int32 k)
{
	if( text == nullptr)
		return -1;
	const char *start = text;
	for (;;)
	{
		unsigned_int32 c = *reinterpret_cast<const unsigned_int8 *>(text);
		if (c == 0) break;
		
		if (c == k) return (text - start);
		text++;
	}
	
	return (-1);
}

int32 Text::FindChar(const char *text, unsigned_int32 k, int32 max)
{
	const char *start = text;
	while (--max >= 0)
	{
		unsigned_int32 c = *reinterpret_cast<const unsigned_int8 *>(text);
		if (c == 0) break;
		
		if (c == k) return (text - start);
		text++;
	}
	
	return (-1);
}

int32 Text::FindUnquotedChar(const char *text, unsigned_int32 k)
{
	bool quote = false;
	bool backslash = false;
	
	const char *start = text;
	for (;;)
	{
		unsigned_int32 c = *reinterpret_cast<const unsigned_int8 *>(text);
		if (c == 0) break;
		
		if (c == 34)
		{
			if (!quote) quote = true;
			else if (!backslash) quote = false;
		}
		
		if ((c == k) && (!quote)) return (text - start);
		
		backslash = ((c == 92) && (!backslash));
		text++;
	}
	
	return (-1);
}

int32 Text::CopyText(const char *source, char *dest)
{
	const char *c = source;
	for (;;)
	{
		unsigned_int32 k = *reinterpret_cast<const unsigned_int8 *>(c);
		*dest++ = (char) k;
		if (k == 0) break;
		c++;
	}
	
	return (c - source);
}

int32 Text::CopyText(const char *source, char *dest, int32 max)
{
	const char *c = source;
	while (--max >= 0)
	{
		unsigned_int32 k = *reinterpret_cast<const unsigned_int8 *>(c);
		if (k == 0) break;
		*dest++ = (char) k;
		c++;
	}
	
	*dest = 0;
	return (c - source);
}

int32 Text::CopyTextUTF8(const char *source, char *dest, int32 maxCount, int32 *length)
{
	int32 count = 0;
	char *d = dest;
	while (--maxCount >= 0)
	{
		unsigned_int32	code;
		
		source += ReadGlyphCodeUTF8(source, &code);
		if (code == 0) break;
		
		d += WriteGlyphCodeUTF8(d, code);
		count++;
	}
	
	*d = 0;
	if (length) *length = d - dest;
	return (count);
}

void Text::ConvertToLowerCase(char *text)
{
	for (;;)
	{
		unsigned_int32 k = *reinterpret_cast<unsigned_int8 *>(text);
		if (k == 0) break;
		if (k - 'A' < 26U) *text = (char) (k + 32);
		text++;
	}
}

void Text::ConvertToUpperCase(char *text)
{
	for (;;)
	{
		unsigned_int32 k = *reinterpret_cast<unsigned_int8 *>(text);
		if (k == 0) break;
		if (k - 'a' < 26U) *text = (char) (k - 32);
		text++;
	}
}

bool Text::CompareText(const char *s1, const char *s2)
{
	for (machine a = 0;; a++)
	{
		unsigned_int32 x = *reinterpret_cast<const unsigned_int8 *>(s1 + a);
		unsigned_int32 y = *reinterpret_cast<const unsigned_int8 *>(s2 + a);
		if (x != y) return (false);
		if (x == 0) break;
	}
	
	return (true);
}

bool Text::CompareText(const char *s1, const char *s2, int32 max)
{
	for (machine a = 0;; a++)
	{
		if (--max < 0) break;
		unsigned_int32 x = *reinterpret_cast<const unsigned_int8 *>(s1 + a);
		unsigned_int32 y = *reinterpret_cast<const unsigned_int8 *>(s2 + a);
		if (x != y) return (false);
		if (x == 0) break;
	}
	
	return (true);
}

bool Text::CompareTextCaseless(const char *s1, const char *s2)
{
	for (machine a = 0;; a++)
	{
		unsigned_int32 x = *reinterpret_cast<const unsigned_int8 *>(s1 + a);
		unsigned_int32 y = *reinterpret_cast<const unsigned_int8 *>(s2 + a);
		if (x - 65 < 26UL) x += 32;
		if (y - 65 < 26UL) y += 32;
		if (x != y) return (false);
		if (x == 0) break;
	}
	
	return (true);
}

bool Text::CompareTextCaseless(const char *s1, const char *s2, int32 max)
{
	for (machine a = 0;; a++)
	{
		if (--max < 0) break;
		unsigned_int32 x = *reinterpret_cast<const unsigned_int8 *>(s1 + a);
		unsigned_int32 y = *reinterpret_cast<const unsigned_int8 *>(s2 + a);
		if (x - 65 < 26UL) x += 32;
		if (y - 65 < 26UL) y += 32;
		if (x != y) return (false);
		if (x == 0) break;
	}
	
	return (true);
}

bool Text::CompareTextLessThan(const char *s1, const char *s2)
{
	for (machine a = 0;; a++)
	{
		unsigned_int32 x = *reinterpret_cast<const unsigned_int8 *>(s1 + a);
		unsigned_int32 y = *reinterpret_cast<const unsigned_int8 *>(s2 + a);
		if ((x != y) || (x == 0)) return (x < y);
	}
}

bool Text::CompareTextLessThan(const char *s1, const char *s2, int32 max)
{
	for (machine a = 0;; a++)
	{
		if (--max < 0) break;
		unsigned_int32 x = *reinterpret_cast<const unsigned_int8 *>(s1 + a);
		unsigned_int32 y = *reinterpret_cast<const unsigned_int8 *>(s2 + a);
		if ((x != y) || (x == 0)) return (x < y);
	}
	
	return (false);
}

bool Text::CompareTextLessThanCaseless(const char *s1, const char *s2)
{
	for (machine a = 0;; a++)
	{
		unsigned_int32 x = *reinterpret_cast<const unsigned_int8 *>(s1 + a);
		unsigned_int32 y = *reinterpret_cast<const unsigned_int8 *>(s2 + a);
		if (x - 'a' < 26UL) x -= 32;
		if (y - 'a' < 26UL) y -= 32;
		if ((x != y) || (x == 0)) return (x < y);
	}
}

bool Text::CompareTextLessThanCaseless(const char *s1, const char *s2, int32 max)
{
	for (machine a = 0;; a++)
	{
		if (--max < 0) break;
		unsigned_int32 x = *reinterpret_cast<const unsigned_int8 *>(s1 + a);
		unsigned_int32 y = *reinterpret_cast<const unsigned_int8 *>(s2 + a);
		if (x - 'a' < 26UL) x -= 32;
		if (y - 'a' < 26UL) y -= 32;
		if ((x != y) || (x == 0)) return (x < y);
	}
	
	return (false);
}

bool Text::CompareTextLessEqual(const char *s1, const char *s2)
{
	for (machine a = 0;; a++)
	{
		unsigned_int32 x = *reinterpret_cast<const unsigned_int8 *>(s1 + a);
		unsigned_int32 y = *reinterpret_cast<const unsigned_int8 *>(s2 + a);
		if ((x != y) || (x == 0)) return (x <= y);
	}
}

bool Text::CompareTextLessEqual(const char *s1, const char *s2, int32 max)
{
	for (machine a = 0;; a++)
	{
		if (--max < 0) break;
		unsigned_int32 x = *reinterpret_cast<const unsigned_int8 *>(s1 + a);
		unsigned_int32 y = *reinterpret_cast<const unsigned_int8 *>(s2 + a);
		if ((x != y) || (x == 0)) return (x <= y);
	}
	
	return (true);
}

bool Text::CompareTextLessEqualCaseless(const char *s1, const char *s2)
{
	for (machine a = 0;; a++)
	{
		unsigned_int32 x = *reinterpret_cast<const unsigned_int8 *>(s1 + a);
		unsigned_int32 y = *reinterpret_cast<const unsigned_int8 *>(s2 + a);
		if (x - 'a' < 26UL) x -= 32;
		if (y - 'a' < 26UL) y -= 32;
		if ((x != y) || (x == 0)) return (x <= y);
	}
}

bool Text::CompareTextLessEqualCaseless(const char *s1, const char *s2, int32 max)
{
	for (machine a = 0;; a++)
	{
		if (--max < 0) break;
		unsigned_int32 x = *reinterpret_cast<const unsigned_int8 *>(s1 + a);
		unsigned_int32 y = *reinterpret_cast<const unsigned_int8 *>(s2 + a);
		if (x - 'a' < 26UL) x -= 32;
		if (y - 'a' < 26UL) y -= 32;
		if ((x != y) || (x == 0)) return (x <= y);
	}
	
	return (true);
}

int32 Text::FindText(const char *s1, const char *s2)
{
	const char *start = s1;
	int32 first = *reinterpret_cast<const unsigned_int8 *>(s2);
	
	for (;;)
	{
		int32 c = *reinterpret_cast<const unsigned_int8 *>(s1++);
		if (c == 0) break;
		
		if (c == first)
		{
			const unsigned_int8 *s3 = reinterpret_cast<const unsigned_int8 *>(s1);
			const unsigned_int8 *s4 = reinterpret_cast<const unsigned_int8 *>(s2);
			
			for (;;)
			{
				int32 d = *++s4;
				if (d == 0) return (s1 - start - 1);
				
				int32 e = *s3++;
				if (e == 0) return (-1);
				
				if (e != d) break;
			}
		}
	}
	
	return (-1);
}

int32 Text::IntegerToString(int32 num, char *text, int32 max)
{
	char	c[16];
	
	bool negative = (num < 0);
	num = Abs(num) & 0x7FFFFFFF;
	
	machine length = 0;
	do
	{
		int32 p = num % 10;
		c[length++] = (char) (p + 48);
		num /= 10;
	} while (num != 0);
	
	machine a = -1;
	if (negative)
	{
		if (++a < max)
		{
			text[a] = '-';
		}
		else
		{
			text[a] = 0;
			return (a);
		}
	}
	
	do
	{
		if (++a < max)
		{
			text[a] = c[--length];
		}
		else
		{
			text[a] = 0;
			return (a);
		}
	} while (length != 0);
	
	text[++a] = 0;
	return (a);
}

int32 Text::Integer64ToString(int64 num, char *text, int32 max)
{
	char	c[32];
	
	bool negative = (num < 0);
	num = Abs64(num) & 0x7FFFFFFFFFFFFFFFULL;
	
	machine length = 0;
	do
	{
		int32 p = num % 10;
		c[length++] = (char) (p + 48);
		num /= 10;
	} while (num != 0);
	
	machine a = -1;
	if (negative)
	{
		if (++a < max)
		{
			text[a] = '-';
		}
		else
		{
			text[a] = 0;
			return (a);
		}
	}
	
	do
	{
		if (++a < max)
		{
			text[a] = c[--length];
		}
		else
		{
			text[a] = 0;
			return (a);
		}
	} while (length != 0);
	
	text[++a] = 0;
	return (a);
}

int32 Text::StringToInteger(const char *text)
{
	int32 value = 0;
	bool negative = false;
	
	for (;;)
	{
		unsigned_int32 x = *text++;
		if (x == 0) break;
		
		if (x == '-')
		{
			negative = true;
		}
		else
		{
			x -= 48;
			if (x < 10) value = value * 10 + x;
		}
	}
	
	if (negative) value = -value;
	return (value);
}

int64 Text::StringToInteger64(const char *text)
{
	int64 value = 0;
	bool negative = false;
	
	for (;;)
	{
		unsigned_int32 x = *text++;
		if (x == 0) break;
		
		if (x == '-')
		{
			negative = true;
		}
		else
		{
			x -= 48;
			if (x < 10) value = value * 10 + x;
		}
	}
	
	if (negative) value = -value;
	return (value);
}

int32 Text::FloatToString(float num, char *text, int32 max)
{
	if (max < 1)
	{
		text[0] = 0;
		return (0);
	}
	
	int32 binary = *reinterpret_cast<int32 *>(&num);
	int32 exponent = (binary >> 23) & 0xFF;
	
	if (exponent == 0)
	{
		if (max >= 3)
		{
			text[0] = '0';
			text[1] = '.';
			text[2] = '0';
			text[3] = 0;
			return (3);
		}
		
		text[0] = '0';
		text[1] = 0;
		return (1);
	}
	
	int32 mantissa = binary & 0x007FFFFF;
	
	if (exponent == 0xFF)
	{
		if (max >= 4)
		{
			bool b = (binary < 0);
			if (b) *text++ = '-';
			
			if (mantissa == 0)
			{
				text[0] = 'I';
				text[1] = 'N';
				text[2] = 'F';
				text[3] = 0;
			}
			else
			{
				text[0] = 'N';
				text[1] = 'A';
				text[2] = 'N';
				text[3] = 0;
			}
			
			return (3 + b);
		}
		
		text[0] = 0;
		return (0);
	}
	
	int32 power = 0;
	float absolute = Fabs(num);
	if ((!(absolute > 1.0e-4F)) || (!(absolute < 1.0e5F)))
	{
		float f = Floor(Log10(absolute));
		absolute /= Pow(10.0F, f);
		power = (int32) f;
		
		binary = *reinterpret_cast<int32 *>(&absolute);
		exponent = (binary >> 23) & 0xFF;
		mantissa = binary & 0x007FFFFF;
	}
	
	exponent -= 0x7F;
	mantissa |= 0x00800000;
	
	machine len = 0;
	if (num < 0.0F)
	{
		text[0] = '-';
		len = 1;
	}
	
	if (exponent >= 0)
	{
		int32 whole = mantissa >> (23 - exponent);
		mantissa = (mantissa << exponent) & 0x007FFFFF;
		
		len += IntegerToString(whole, &text[len], max - len);
		if (len < max) text[len++] = '.';
		if (len == max) goto end;
	}
	else
	{
		if (len + 2 <= max)
		{
			text[len++] = '0';
			text[len++] = '.';
			if (len == max) goto end;
		}
		else
		{
			if (len < max) text[len++] = '0';
			goto end;
		}
		
		mantissa >>= -exponent;
	}
	
	for (machine a = 0, zeroCount = 0, nineCount = 0; (a < 7) && (len < max); a++)
	{
		mantissa *= 10;
		int32 n = (mantissa >> 23) + 48;
		text[len++] = (char) n;
		
		if (n == '0')
		{
			if ((++zeroCount >= 4) && (a >= 4)) break;
		}
		else if (n == '9')
		{
			if ((++nineCount >= 4) && (a >= 4)) break;
		}
		
		mantissa &= 0x007FFFFF;
		if (mantissa < 2) break;
	}
	
	if ((text[len - 1] == '9') && (text[len - 2] == '9'))
	{
		for (machine a = len - 3;; a--)
		{
			char c = text[a];
			if (c != '9')
			{
				if (c != '.')
				{
					text[a] = c + 1;
					len = a + 1;
				}
				
				break;
			}
		}
	}
	else
	{
		while (text[len - 1] == '0') len--;
		if (text[len - 1] == '.') text[len++] = '0';
	}
	
	if ((power != 0) && (len < max))
	{
		text[len++] = 'e';
		return (IntegerToString(power, &text[len], max - len));
	}
	
	end:
	text[len] = 0;
	return (len);
}

float Text::StringToFloat(const char *text)
{
	float value = 0.0F;
	float expon = 0.0F;
	float decplace = 0.1F;
	
	bool negative = false;
	bool exponent = false;
	bool exponNeg = false;
	bool decimal = false;
	
	for (;;)
	{
		unsigned_int32 x = *text++;
		if (x == 0) break;
		
		if (x == '-')
		{
			if (exponent) exponNeg = true;
			else negative = true;
		}
		else if (x == '.')
		{
			decimal = true;
		}
		else if ((x == 'e') || (x == 'E'))
		{
			exponent = true;
		}
		else
		{
			x -= 48;
			if (x < 10)
			{
				if (exponent)
				{
					expon = expon * 10.0F + x;
				}
				else
				{
					if (decimal)
					{
						value += x * decplace;
						decplace *= 0.1F;
					}
					else
					{
						value = value * 10.0F + x;
					}
				}
			}
		}
	}
	
	if (exponent)
	{
		if (exponNeg) expon = -expon;
		value *= Pow(10.0F, expon);
	}
	
	if (negative) value = -value;
	return (value);
}

String<31> Text::Integer64ToHexString16(unsigned_int64 num)
{
	String<31>	text;
	
	text[0] = hexDigit[(num >> 60) & 15];
	text[1] = hexDigit[(num >> 56) & 15];
	text[2] = hexDigit[(num >> 52) & 15];
	text[3] = hexDigit[(num >> 48) & 15];
	text[4] = hexDigit[(num >> 44) & 15];
	text[5] = hexDigit[(num >> 40) & 15];
	text[6] = hexDigit[(num >> 36) & 15];
	text[7] = hexDigit[(num >> 32) & 15];
	text[8] = hexDigit[(num >> 28) & 15];
	text[9] = hexDigit[(num >> 24) & 15];
	text[10] = hexDigit[(num >> 20) & 15];
	text[11] = hexDigit[(num >> 16) & 15];
	text[12] = hexDigit[(num >> 12) & 15];
	text[13] = hexDigit[(num >> 8) & 15];
	text[14] = hexDigit[(num >> 4) & 15];
	text[15] = hexDigit[num & 15];
	text[16] = 0;
	
	return (text);
}

String<15> Text::IntegerToHexString8(unsigned_int32 num)
{
	String<15>	text;
	
	text[0] = hexDigit[(num >> 28) & 15];
	text[1] = hexDigit[(num >> 24) & 15];
	text[2] = hexDigit[(num >> 20) & 15];
	text[3] = hexDigit[(num >> 16) & 15];
	text[4] = hexDigit[(num >> 12) & 15];
	text[5] = hexDigit[(num >> 8) & 15];
	text[6] = hexDigit[(num >> 4) & 15];
	text[7] = hexDigit[num & 15];
	text[8] = 0;
	
	return (text);
}

String<7> Text::IntegerToHexString4(unsigned_int32 num)
{
	String<7>	text;
	
	text[0] = hexDigit[(num >> 12) & 15];
	text[1] = hexDigit[(num >> 8) & 15];
	text[2] = hexDigit[(num >> 4) & 15];
	text[3] = hexDigit[num & 15];
	text[4] = 0;
	
	return (text);
}

String<3> Text::IntegerToHexString2(unsigned_int32 num)
{
	String<3>	text;
	
	text[0] = hexDigit[(num >> 4) & 15];
	text[1] = hexDigit[num & 15];
	text[2] = 0;
	
	return (text);
}

String<4> Text::TypeToString(unsigned_int32 type)
{
	unsigned_int32 c = type >> 24;
	if (c != 0) return (String<4>((char) c, (char) (type >> 16), (char) (type >> 8), (char) type));
	return (String<4>((char) (type >> 16), (char) (type >> 8), (char) type, 0));
}

unsigned_int32 Text::StringToType(const char *string)
{
	unsigned_int32 type = 0;
	
	unsigned_int32 c = reinterpret_cast<const unsigned_int8 *>(string)[0];
	if (c != 0)
	{
		type = c << 24;
		
		c = reinterpret_cast<const unsigned_int8 *>(string)[1];
		if (c != 0)
		{
			type |= c << 16;
			
			c = reinterpret_cast<const unsigned_int8 *>(string)[2];
			if (c != 0)
			{
				type |= c << 8;
				
				c = reinterpret_cast<const unsigned_int8 *>(string)[3];
				if (c != 0) type |= c;
			}
		}
	}
	
	return (type);
}

int32 Text::GetResourceNameLength(const char *text)
{
	int32 len = GetTextLength(text);
	for (machine a = len - 1; a >= 0; a--)
	{
		unsigned_int32 c = *reinterpret_cast<const unsigned_int8 *>(&text[a]);
		if (c == '.') return (a);
		if (c == '/') break;
	}
	
	return (len);
}

int32 Text::GetDirectoryPathLength(const char *text)
{
	int32 len = 0;
	for (;;)
	{
		int32 x = FindChar(&text[len], '/');
		if (x == -1) break;
		len += x + 1;
	}
	
	return (len);
}

int32 Text::GetWhitespaceLength(const char *text)
{
	const char *start = text;
	for (;;)
	{
		for (;;)
		{
			unsigned_int32 c = *reinterpret_cast<const unsigned_int8 *>(text);
			if (c == 0) goto end;
			
			if (c > 32)
			{
				if ((c != '/') || (text[1] != '/')) goto end;
				break;
			}
			
			text++;
		}
		
		text += 2;
		for (;;)
		{
			unsigned_int32 c = *reinterpret_cast<const unsigned_int8 *>(text);
			if (c == 0) goto end;
			
			text++;
			if ((c == 10) || (c == 13)) break;
		}
	}
	
	end:
	return (text - start);
}

int32 Text::ReadIdentifier(const char *text, char *identifier, int32 max)
{
	const char *start = text;
	while (--max >= 0)
	{
		unsigned_int32 c = *reinterpret_cast<const unsigned_int8 *>(text);
		if (!identifierCharFlag[c]) break;
		
		*identifier++ = (char) c;
		text++;
	}
	
	*identifier = 0;
	return (text - start);
}

int32 Text::ReadInteger(const char *text, char *number, int32 max)
{
	const char *start = text;
	
	unsigned_int32 c = *reinterpret_cast<const unsigned_int8 *>(text);
	if (c == '-')
	{
		if (--max >= 0)
		{
			*number++ = (char) c;
			text++;
		}
	}
	
	while (--max >= 0)
	{
		c = *reinterpret_cast<const unsigned_int8 *>(text);
		if (c - '0' >= 10U) break;
		
		*number++ = (char) c;
		text++;
	}
	
	*number = 0;
	return (text - start);
}

int32 Text::ReadFloat(const char *text, char *number, int32 max)
{
	const char *start = text;
	
	unsigned_int32 c = *reinterpret_cast<const unsigned_int8 *>(text);
	if (c == '-')
	{
		if (--max >= 0)
		{
			*number++ = (char) c;
			text++;
		}
	}
	
	bool decimal = false;
	bool exponent = false;
	bool expneg = true;
	
	while (--max >= 0)
	{
		c = *reinterpret_cast<const unsigned_int8 *>(text);
		if (c == '.')
		{
			if (decimal) break;
			decimal = true;
		}
		else if ((c == 'e') || (c == 'E'))
		{
			if (exponent) break;
			exponent = true;
			expneg = false;
		}
		else
		{
			if ((c == '-') && (expneg)) break;
			else if (c - '0' >= 10U) break;
			
			expneg = true;
		}
		
		*number++ = (char) c;
		text++;
	}
	
	*number = 0;
	return (text - start);
}

int32 Text::ReadString(const char *text, char *string, int32 max)
{
	const char *start = text;
	
	if (*text == 34)
	{
		text++;
		bool backslash = false;
		
		while (--max >= 0)
		{
			unsigned_int32 c = *reinterpret_cast<const unsigned_int8 *>(text);
			if (c == 0) break;
			
			text++;
			
			if ((c != 92) || (backslash))
			{
				if ((c == 34) && (!backslash)) break;
				*string++ = (char) c;
				backslash = false;
			}
			else
			{
				backslash = true;
			}
		}
	}
	else
	{
		while (--max >= 0)
		{
			unsigned_int32 c = *reinterpret_cast<const unsigned_int8 *>(text);
			if ((c == 0) || (c < 33) || ((c == '/') && (text[1] == '/'))) break;
			
			*string++ = (char) c;
			text++;
		}
	}
	
	*string = 0;
	return (text - start);
}

int32 Text::ReadType(const char *text, unsigned_int32 *type)
{
	if (*text == '\'')
	{
		const char *start = text;
		unsigned_int32 value = 0;
		
		text++;
		bool backslash = false;
		
		for (;;)
		{
			unsigned_int32 c = *reinterpret_cast<const unsigned_int8 *>(text);
			if (c == 0) break;
			
			text++;
			
			if ((c != 92) || (backslash))
			{
				if ((c == '\'') && (!backslash)) break;
				value = (value << 8) | c;
				backslash = false;
			}
			else
			{
				backslash = true;
			}
		}
		
		*type = value;
		return (text - start);
	}
	
	*type = 0;
	return (0);
}


String<0>::String()
{
	logicalSize = 1;
	physicalSize = 0;
	stringPointer = emptyString;
}

String<0>::~String()
{
	if (stringPointer != emptyString) delete[] stringPointer;
}

String<0>::String(const String& s)
{
	int32 size = s.logicalSize;
	logicalSize = size;
	if (size > 1)
	{
		physicalSize = GetPhysicalSize(size);
		stringPointer = new char[physicalSize];
		Text::CopyText(s, stringPointer);
	}
	else
	{
		physicalSize = 0;
		stringPointer = emptyString;
	}
}

String<0>::String(const char *s)
{
	int32 size = Text::GetTextLength(s) + 1;
	logicalSize = size;
	if (size > 1)
	{
		physicalSize = GetPhysicalSize(size);
		stringPointer = new char[physicalSize];
		Text::CopyText(s, stringPointer);
	}
	else
	{
		physicalSize = 0;
		stringPointer = emptyString;
	}
}

String<0>::String(const char *s, int32 length)
{
	length = Min(length, Text::GetTextLength(s));
	
	int32 size = length + 1;
	logicalSize = size;
	if (size > 1)
	{
		physicalSize = GetPhysicalSize(size);
		stringPointer = new char[physicalSize];
		Text::CopyText(s, stringPointer, length);
	}
	else
	{
		physicalSize = 0;
		stringPointer = emptyString;
	}
}

String<0>::String(int32 n)
{
	physicalSize = kStringAllocSize + 1;
	stringPointer = new char[kStringAllocSize + 1];
	logicalSize = Text::IntegerToString(n, stringPointer, kStringAllocSize) + 1;
}

String<0>::String(unsigned_int32 n)
{
	physicalSize = kStringAllocSize + 1;
	stringPointer = new char[kStringAllocSize + 1];
	logicalSize = Text::IntegerToString(n, stringPointer, kStringAllocSize) + 1;
}

String<0>::String(int64 n)
{
	physicalSize = kStringAllocSize + 1;
	stringPointer = new char[kStringAllocSize + 1];
	logicalSize = Text::Integer64ToString(n, stringPointer, kStringAllocSize) + 1;
}

String<0>::String(float n)
{
	physicalSize = kStringAllocSize + 1;
	stringPointer = new char[kStringAllocSize + 1];
	logicalSize = Text::FloatToString(n, stringPointer, kStringAllocSize) + 1;
}

String<0>::String(const char *s1, const char *s2)
{
	int32 len1 = Text::GetTextLength(s1);
	int32 len2 = Text::GetTextLength(s2);
	
	int32 size = len1 + len2 + 1;
	logicalSize = size;
	if (size > 1)
	{
		physicalSize = GetPhysicalSize(size);
		stringPointer = new char[physicalSize];
		Text::CopyText(s1, stringPointer);
		Text::CopyText(s2, stringPointer + len1);
	}
	else
	{
		physicalSize = 0;
		stringPointer = emptyString;
	}
}

String<0>::String(int32 n, const char *s1)
{
	int32 len1 = Text::GetTextLength(s1);
	
	int32 size = len1 + kStringAllocSize + 1;
	physicalSize = GetPhysicalSize(size);
	stringPointer = new char[physicalSize];
	Text::CopyText(s1, stringPointer);
	logicalSize = len1 + Text::IntegerToString(n, stringPointer + len1, kStringAllocSize) + 1;
}

String<0>::String(unsigned_int32 n, const char *s1)
{
	int32 len1 = Text::GetTextLength(s1);
	
	int32 size = len1 + kStringAllocSize + 1;
	physicalSize = GetPhysicalSize(size);
	stringPointer = new char[physicalSize];
	Text::CopyText(s1, stringPointer);
	logicalSize = len1 + Text::IntegerToString(n, stringPointer + len1, kStringAllocSize) + 1;
}

String<0>::String(int64 n, const char *s1)
{
	int32 len1 = Text::GetTextLength(s1);
	
	int32 size = len1 + kStringAllocSize + 1;
	physicalSize = GetPhysicalSize(size);
	stringPointer = new char[physicalSize];
	Text::CopyText(s1, stringPointer);
	logicalSize = len1 + Text::Integer64ToString(n, stringPointer + len1, kStringAllocSize) + 1;
}

void String<0>::Clear(void)
{
	if (stringPointer != emptyString)
	{
		delete[] stringPointer;
		stringPointer = emptyString;
		
		logicalSize = 1;
		physicalSize = 0;
	}
}

void String<0>::Resize(int32 size)
{
	logicalSize = size;
	if ((size > physicalSize) || (size < physicalSize / 2))
	{
		if (stringPointer != emptyString) delete[] stringPointer;
		physicalSize = GetPhysicalSize(size);
		stringPointer = new char[physicalSize];
	}
}

String<0>& String<0>::Set(const char *s, int32 length)
{
	length = Min(length, Text::GetTextLength(s));
	
	int32 size = length + 1;
	if (size > 1)
	{
		Resize(size);
		Text::CopyText(s, stringPointer, length);
	}
	else
	{
		Clear();
	}
	
	return (*this);
}

String<0>& String<0>::operator =(const String& s)
{
	int32 size = s.logicalSize;
	if (size > 1)
	{
		Resize(size);
		Text::CopyText(s, stringPointer);
	}
	else
	{
		Clear();
	}
	
	return (*this);
}

String<0>& String<0>::operator =(const char *s)
{
	int32 size = Text::GetTextLength(s) + 1;
	if (size > 1)
	{
		Resize(size);
		Text::CopyText(s, stringPointer);
	}
	else
	{
		Clear();
	}
	
	return (*this);
}

String<0>& String<0>::operator =(int32 n)
{
	Resize(kStringAllocSize);
	logicalSize = Text::IntegerToString(n, stringPointer, kStringAllocSize - 1) + 1;
	return (*this);
}

String<0>& String<0>::operator =(unsigned_int32 n)
{
	Resize(kStringAllocSize);
	logicalSize = Text::IntegerToString(n, stringPointer, kStringAllocSize - 1) + 1;
	return (*this);
}

String<0>& String<0>::operator =(int64 n)
{
	Resize(kStringAllocSize);
	logicalSize = Text::Integer64ToString(n, stringPointer, kStringAllocSize - 1) + 1;
	return (*this);
}

String<0>& String<0>::operator =(float n)
{
	Resize(kStringAllocSize);
	logicalSize = Text::FloatToString(n, stringPointer, kStringAllocSize - 1) + 1;
	return (*this);
}

String<0>& String<0>::operator +=(const String<>& s)
{
	int32 length = s.Length();
	if (length > 0)
	{
		int32 size = logicalSize + length;
		if (size > 1)
		{
			if (size > physicalSize)
			{
				physicalSize = Max(GetPhysicalSize(size), physicalSize + physicalSize / 2);
				char *newPointer = new char[physicalSize];
				
				if (stringPointer != emptyString)
				{
					Text::CopyText(stringPointer, newPointer);
					delete[] stringPointer;
				}
				
				stringPointer = newPointer;
			}
			
			Text::CopyText(s, stringPointer + logicalSize - 1);
			logicalSize = size;
		}
	}
	
	return (*this);
}

String<0>& String<0>::operator +=(const char *s)
{
	int32 length = Text::GetTextLength(s);
	if (length > 0)
	{
		int32 size = logicalSize + length;
		if (size > 1)
		{
			if (size > physicalSize)
			{
				physicalSize = Max(GetPhysicalSize(size), physicalSize + physicalSize / 2);
				char *newPointer = new char[physicalSize];
				
				if (stringPointer != emptyString)
				{
					Text::CopyText(stringPointer, newPointer);
					delete[] stringPointer;
				}
				
				stringPointer = newPointer;
			}
			
			Text::CopyText(s, stringPointer + logicalSize - 1);
			logicalSize = size;
		}
	}
	
	return (*this);
}

String<0>& String<0>::operator +=(char k)
{
	int32 size = logicalSize + 1;
	if (size > physicalSize)
	{
		physicalSize = Max(GetPhysicalSize(size), physicalSize + physicalSize / 2);
		char *newPointer = new char[physicalSize];
		
		if (stringPointer != emptyString)
		{
			Text::CopyText(stringPointer, newPointer);
			delete[] stringPointer;
		}
		
		stringPointer = newPointer;
	}
	
	stringPointer[logicalSize - 1] = k;
	stringPointer[logicalSize] = 0;
	logicalSize = size;
	return (*this);
}

String<0>& String<0>::operator +=(int32 n)
{
	int32 size = logicalSize + kStringAllocSize;
	if (size > physicalSize)
	{
		physicalSize = Max(GetPhysicalSize(size), physicalSize + physicalSize / 2);
		char *newPointer = new char[physicalSize];
		
		if (stringPointer != emptyString)
		{
			Text::CopyText(stringPointer, newPointer);
			delete[] stringPointer;
		}
		
		stringPointer = newPointer;
	}
	
	logicalSize += Text::IntegerToString(n, stringPointer + logicalSize - 1, kStringAllocSize);
	return (*this);
}

String<0>& String<0>::operator +=(unsigned_int32 n)
{
	int32 size = logicalSize + kStringAllocSize;
	if (size > physicalSize)
	{
		physicalSize = Max(GetPhysicalSize(size), physicalSize + physicalSize / 2);
		char *newPointer = new char[physicalSize];
		
		if (stringPointer != emptyString)
		{
			Text::CopyText(stringPointer, newPointer);
			delete[] stringPointer;
		}
		
		stringPointer = newPointer;
	}
	
	logicalSize += Text::IntegerToString(n, stringPointer + logicalSize - 1, kStringAllocSize);
	return (*this);
}

String<0>& String<0>::operator +=(int64 n)
{
	int32 size = logicalSize + kStringAllocSize;
	if (size > physicalSize)
	{
		physicalSize = Max(GetPhysicalSize(size), physicalSize + physicalSize / 2);
		char *newPointer = new char[physicalSize];
		
		if (stringPointer != emptyString)
		{
			Text::CopyText(stringPointer, newPointer);
			delete[] stringPointer;
		}
		
		stringPointer = newPointer;
	}
	
	logicalSize += Text::Integer64ToString(n, stringPointer + logicalSize - 1, kStringAllocSize);
	return (*this);
}

String<0>& String<0>::SetLength(int32 length)
{
	int32 size = length + 1;
	if (size > 1)
	{
		if (size != logicalSize)
		{
			logicalSize = size;
			if ((size > physicalSize) || (size < physicalSize / 2))
			{
				physicalSize = GetPhysicalSize(size);
				char *newPointer = new char[physicalSize];
				
				if (stringPointer != emptyString)
				{
					Text::CopyText(stringPointer, newPointer, length);
					delete[] stringPointer;
				}
				
				stringPointer = newPointer;
			}
			
			stringPointer[length] = 0;
		}
	}
	else
	{
		Clear();
	}
	
	return (*this);
}

String<0>& String<0>::Append(const char *s, int32 length)
{
	if (length > 0)
	{
		int32 size = logicalSize + length;
		if (size > 1)
		{
			if (size > physicalSize)
			{
				physicalSize = Max(GetPhysicalSize(size), physicalSize + physicalSize / 2);
				char *newPointer = new char[physicalSize];
				
				if (stringPointer != emptyString)
				{
					Text::CopyText(stringPointer, newPointer);
					delete[] stringPointer;
				}
				
				stringPointer = newPointer;
			}
			
			Text::CopyText(s, stringPointer + logicalSize - 1, length);
			logicalSize = size;
		}
	}
	
	return (*this);
}

// ZYURVUR
